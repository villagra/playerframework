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
    public class PlayTimeTrackingPlugin : TrackingPluginBase<PlayTimeTrackingEvent>
    {
        private readonly List<PlayTimeTrackingEvent> spentPlayTimeEvents = new List<PlayTimeTrackingEvent>();

        private DateTime? startTime;

        /// <summary>
        /// Gets the total time watched in the current session. Reset when new media is loaded but not reset when media plays to the end, loops, or restarts.
        /// </summary>
        public TimeSpan PlayTime { get; private set; }

        /// <summary>
        /// Gets or sets the percentage of time compared to the duration that the media has been played before the tracking event will fire.
        /// .5 = 50%, 2 = 200%
        /// </summary>
        public double PlayTimePercentage { get; private set; }

        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            MediaPlayer.UpdateCompleted += MediaPlayer_UpdateCompleted;
            MediaPlayer.CurrentStateChanged += MediaPlayer_CurrentStateChanged;
            MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            return base.OnActivate();
        }

        /// <inheritdoc /> 
        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            MediaPlayer.UpdateCompleted -= MediaPlayer_UpdateCompleted;
            MediaPlayer.CurrentStateChanged -= MediaPlayer_CurrentStateChanged;
            MediaPlayer.MediaEnded -= MediaPlayer_MediaEnded;
        }

        void MediaPlayer_MediaEnded(object sender, MediaPlayerActionEventArgs e)
        {
            EvaluteTrackingEvents();
        }

        private void MediaPlayer_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            switch (MediaPlayer.CurrentState)
            {
                case MediaElementState.Playing:
                    startTime = DateTime.Now.Subtract(PlayTime);
                    break;
                case MediaElementState.Closed:
                    spentPlayTimeEvents.Clear();
                    break;
                case MediaElementState.Opening:
                    spentPlayTimeEvents.Clear();
                    startTime = null;
                    PlayTime = TimeSpan.Zero;
                    PlayTimePercentage = 0;
                    break;
                case MediaElementState.Buffering:
                case MediaElementState.Paused:
                case MediaElementState.Stopped:
                    if (startTime.HasValue) PlayTime = DateTime.Now.Subtract(startTime.Value);
                    break;
            }
        }

        private void MediaPlayer_UpdateCompleted(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.AdvertisingState == AdvertisingState.None || MediaPlayer.AdvertisingState == AdvertisingState.NonLinear)
            {
                EvaluteTrackingEvents();
            }
        }

        private void EvaluteTrackingEvents()
        {
            if (MediaPlayer.CurrentState == MediaElementState.Playing && startTime.HasValue)
            {
                PlayTime = DateTime.Now.Subtract(startTime.Value);
                PlayTimePercentage = PlayTime.TotalSeconds / MediaPlayer.Duration.TotalSeconds;
                foreach (var playTimeTrackingEvent in TrackingEvents.Except(spentPlayTimeEvents).ToList())
                {
                    if ((!playTimeTrackingEvent.PlayTimePercentage.HasValue && playTimeTrackingEvent.PlayTime <= PlayTime) || (playTimeTrackingEvent.PlayTimePercentage.HasValue && playTimeTrackingEvent.PlayTimePercentage <= PlayTimePercentage))
                    {
                        spentPlayTimeEvents.Add(playTimeTrackingEvent);
                        OnTrackEvent(new EventTrackedEventArgs(playTimeTrackingEvent));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Used to identify and track when the media has been played a specified amount of time.
    /// </summary>
    public class PlayTimeTrackingEvent : TrackingEventBase
    {
        /// <summary>
        /// Gets or sets the absolute amount of time that the media has been played before the tracking event will fire.
        /// </summary>
        public TimeSpan PlayTime { get; set; }

        /// <summary>
        /// Gets or sets the percentage of time compared to the duration that the media has been played before the tracking event will fire.
        /// .5 = 50%, 2 = 200%
        /// </summary>
        public double? PlayTimePercentage { get; set; }
    }
}
