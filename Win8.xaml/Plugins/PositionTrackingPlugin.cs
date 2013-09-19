using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A plugin used to help track when specific events occur during playback.
    /// </summary>
    public class PositionTrackingPlugin : TrackingPluginBase<PositionTrackingEvent>
    {
        private readonly Dictionary<string, PositionTrackingEvent> activeMarkers = new Dictionary<string, PositionTrackingEvent>();
        private readonly List<PositionTrackingEvent> trackingEventsToInitializeOnStart = new List<PositionTrackingEvent>();

        /// <summary>
        /// The TimelineMarker ID used to store tracking events.
        /// </summary>
        public const string MarkerType_TrackingEvent = "TrackingEvent";

        /// <summary>
        /// Identifies the EvaluateOnForwardOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty EvaluateOnForwardOnlyProperty = DependencyProperty.Register("EvaluateOnForwardOnly", typeof(bool), typeof(PositionTrackingPlugin), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether seeking or scrubbing back in time can trigger ads. Set to false to allow ads to be played when seeking backwards. Default is true.
        /// </summary>
        public bool EvaluateOnForwardOnly
        {
            get { return (bool)GetValue(EvaluateOnForwardOnlyProperty); }
            set { SetValue(EvaluateOnForwardOnlyProperty, value); }
        }

        /// <inheritdoc /> 
        protected override void InitializeTrackingEvent(PositionTrackingEvent positionTrackingEvent)
        {
            if (MediaPlayer.PlayerState == PlayerState.Opened || MediaPlayer.PlayerState == PlayerState.Starting || MediaPlayer.PlayerState == PlayerState.Started)
            {
                InitializePositionTrackingEvent(positionTrackingEvent);
            }
            else
            {
                trackingEventsToInitializeOnStart.Add(positionTrackingEvent);
            }
        }

        private void InitializePositionTrackingEvent(PositionTrackingEvent positionTrackingEvent)
        {
            if (!positionTrackingEvent.PositionPercentage.HasValue || (positionTrackingEvent.PositionPercentage.Value > 0 && positionTrackingEvent.PositionPercentage.Value < 1))
            {
                var timelineMarker = new TimelineMarker();
                timelineMarker.Type = MarkerType_TrackingEvent;
                timelineMarker.Text = Guid.NewGuid().ToString();
                if (positionTrackingEvent.PositionPercentage.HasValue)
                {
                    timelineMarker.Time = TimeSpan.FromSeconds(positionTrackingEvent.PositionPercentage.Value * MediaPlayer.Duration.TotalSeconds);
                }
                else
                {
                    timelineMarker.Time = positionTrackingEvent.Position;
                }
                MediaPlayer.Markers.Add(timelineMarker);
                activeMarkers.Add(timelineMarker.Text, positionTrackingEvent);
            }
        }

        /// <inheritdoc /> 
        protected override void UninitializeTrackingEvent(PositionTrackingEvent positionTrackingEvent)
        {
            if (trackingEventsToInitializeOnStart.Contains(positionTrackingEvent))
            {
                trackingEventsToInitializeOnStart.Remove(positionTrackingEvent);
            }
            else
            {
                if (!positionTrackingEvent.PositionPercentage.HasValue || (positionTrackingEvent.PositionPercentage.Value > 0 && positionTrackingEvent.PositionPercentage.Value < 1))
                {
                    var timelineMarkerKey = activeMarkers.First(kvp => kvp.Value == positionTrackingEvent).Key;
                    var timelineMarker = MediaPlayer.Markers.FirstOrDefault(m => m.Type == MarkerType_TrackingEvent && m.Text == timelineMarkerKey);
                    MediaPlayer.Markers.Remove(timelineMarker);
                    activeMarkers.Remove(timelineMarkerKey);
                }
            }
        }

        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            MediaPlayer.MediaClosed += MediaPlayer_MediaClosed;
            MediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            MediaPlayer.MediaStarted += MediaPlayer_MediaStarted;
            MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            MediaPlayer.MarkerReached += MediaPlayer_MarkerReached;
            MediaPlayer.Seeked += MediaPlayer_Seeked;
            MediaPlayer.ScrubbingCompleted += MediaPlayer_ScrubbingCompleted;
            return base.OnActivate();
        }

        /// <inheritdoc /> 
        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            MediaPlayer.MediaClosed -= MediaPlayer_MediaClosed;
            MediaPlayer.MediaOpened -= MediaPlayer_MediaOpened;
            MediaPlayer.MediaStarted -= MediaPlayer_MediaStarted;
            MediaPlayer.MediaEnded -= MediaPlayer_MediaEnded;
            MediaPlayer.MarkerReached -= MediaPlayer_MarkerReached;
            MediaPlayer.Seeked -= MediaPlayer_Seeked;
            MediaPlayer.ScrubbingCompleted -= MediaPlayer_ScrubbingCompleted;
        }

        void MediaPlayer_MediaClosed(object sender, RoutedEventArgs e)
        {
            trackingEventsToInitializeOnStart.Clear();
        }

        void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            foreach (var positionTrackingEvent in trackingEventsToInitializeOnStart)
            {
                InitializePositionTrackingEvent(positionTrackingEvent);
            }
        }

        private void MediaPlayer_MarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
        {
            if (e.Marker.Type == MarkerType_TrackingEvent && activeMarkers.ContainsKey(e.Marker.Text))
            {
                var positionTrackingEvent = activeMarkers[e.Marker.Text];
                OnTrackEvent(new PositionEventTrackedEventArgs(positionTrackingEvent, false));
            }
        }

        private void MediaPlayer_MediaStarted(object sender, RoutedEventArgs e)
        {
            if (!MediaPlayer.StartupPosition.HasValue)
            {
                foreach (var trackingEvent in TrackingEvents.Where(t => t.PositionPercentage.HasValue && t.PositionPercentage.Value == 0).ToList())
                {
                    OnTrackEvent(new PositionEventTrackedEventArgs(trackingEvent, false));
                }
            }
        }

        private void MediaPlayer_MediaEnded(object sender, MediaPlayerActionEventArgs e)
        {
            foreach (var trackingEvent in TrackingEvents.Where(t => t.PositionPercentage.HasValue && t.PositionPercentage.Value == 1).ToList())
            {
                OnTrackEvent(new PositionEventTrackedEventArgs(trackingEvent, false));
            }
        }

        private void MediaPlayer_Seeked(object sender, SeekRoutedEventArgs e)
        {
            EvaluateMarkers(e.PreviousPosition, e.Position, true);
        }

        private void MediaPlayer_ScrubbingCompleted(object sender, ScrubProgressRoutedEventArgs e)
        {
            EvaluateMarkers(e.StartPosition, e.Position, true);
        }

        /// <summary>
        /// Evaluates all markers in a window and plays an ad if applicable.
        /// </summary>
        /// <param name="originalPosition">The window start position.</param>
        /// <param name="newPosition">The window end position. Note: This can be before originalPosition if going backwards.</param>
        /// <param name="isSeeking">A flag indicating that the user is actively seeking</param>
        public void EvaluateMarkers(TimeSpan originalPosition, TimeSpan newPosition, bool isSeeking)
        {
            if (!EvaluateOnForwardOnly || newPosition > originalPosition)
            {
                foreach (var marker in MediaPlayer.Markers.Where(m => m.Type == MarkerType_TrackingEvent).ToList())
                {
                    if (marker.Time <= newPosition && marker.Time > originalPosition)
                    {
                        if (activeMarkers.ContainsKey(marker.Text))
                        {
                            var positionTrackingEvent = activeMarkers[marker.Text];
                            OnTrackEvent(new PositionEventTrackedEventArgs(positionTrackingEvent, isSeeking));
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Used to identify and track when the media reaches a specific position.
    /// </summary>
    public class PositionTrackingEvent : TrackingEventBase
    {
        /// <summary>
        /// Gets or sets the playback position when the tracking event will fire.
        /// </summary>
        public TimeSpan Position { get; set; }

        /// <summary>
        /// Gets or sets the percentage of time compared to the duration of the playback position when the tracking event will fire.
        /// .5 = 50%, 2 = 200%
        /// </summary>
        public double? PositionPercentage { get; set; }
    }

    /// <summary>
    /// Contains additional information about a tracking event that has occurred.
    /// </summary>
    public class PositionEventTrackedEventArgs : EventTrackedEventArgs
    {
        /// <summary>
        /// Creates a new instance of PositionEventTrackedEventArgs
        /// </summary>
        public PositionEventTrackedEventArgs()
            : base()
        { }

        /// <summary>
        /// Creates a new instance of PositionEventTrackedEventArgs
        /// </summary>
        /// <param name="trackingEvent">The event that was tracked</param>
        public PositionEventTrackedEventArgs(PositionTrackingEvent trackingEvent)
            : base(trackingEvent)
        { }

        /// <summary>
        /// Creates a new instance of EventTrackedEventArgs
        /// </summary>
        /// <param name="trackingEvent">The event that was tracked</param>
        /// <param name="skippedPast">A flag indicating whether or not the user was seeking when the event occurred</param>
        public PositionEventTrackedEventArgs(PositionTrackingEvent trackingEvent, bool skippedPast)
            : this(trackingEvent)
        {
            SkippedPast = skippedPast;
        }

        /// <summary>
        /// Gets a flag indicating whether or not the user was seeking or scrubbing when the event occurred.
        /// </summary>
        public bool SkippedPast { get; private set; }
    }
}
