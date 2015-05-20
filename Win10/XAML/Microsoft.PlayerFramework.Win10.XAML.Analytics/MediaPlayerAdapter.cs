using Microsoft.VideoAnalytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Microsoft.PlayerFramework.Analytics
{
    internal class MediaPlayerAdapter : IPlayerMonitor
    {
        MediaPlayer mediaPlayer;
        double playbackRate;
        readonly IList<IEventTracker> trackingPlugins = new List<IEventTracker>();

        public MediaPlayerAdapter(MediaPlayer mediaPlayer)
        {
            playbackRate = mediaPlayer.DefaultPlaybackRate;
            MediaPlayer = mediaPlayer;
        }

        public MediaPlayer MediaPlayer
        {
            get { return mediaPlayer; }
            set
            {
                if (mediaPlayer != null)
                {
                    mediaPlayer.MediaOpened -= mediaPlayer_MediaOpened;
                    mediaPlayer.MediaClosed += mediaPlayer_MediaClosed;
                    mediaPlayer.MediaEnding -= mediaPlayer_MediaEnding;
                    mediaPlayer.MediaFailed -= mediaPlayer_MediaFailed;
                    mediaPlayer.MediaStarted -= mediaPlayer_MediaStarted;
                    mediaPlayer.CurrentStateChanged -= mediaPlayer_CurrentStateChanged;
                    mediaPlayer.IsFullScreenChanged -= mediaPlayer_IsFullScreenChanged;
                    mediaPlayer.Seeked -= mediaPlayer_Seeked;
                    mediaPlayer.ScrubbingStarted -= mediaPlayer_ScrubbingStarted;
                    mediaPlayer.ScrubbingCompleted -= mediaPlayer_ScrubbingCompleted;
                    mediaPlayer.RateChanged -= mediaPlayer_RateChanged;
                    mediaPlayer.IsLiveChanged -= mediaPlayer_IsLiveChanged;
                    mediaPlayer.SelectedCaptionChanged -= mediaPlayer_SelectedCaptionChanged;
                    mediaPlayer.SelectedAudioStreamChanged -= mediaPlayer_SelectedAudioStreamChanged;
                    mediaPlayer.AdvertisingStateChanged -= mediaPlayer_AdvertisingStateChanged;
                    mediaPlayer.Plugins.CollectionChanged -= Plugins_CollectionChanged;
                    foreach (var trackingPlugin in trackingPlugins)
                    {
                        trackingPlugin.EventTracked -= trackingPlugin_EventTracked;
                    }
                    trackingPlugins.Clear();
                }
                mediaPlayer = value;
                if (mediaPlayer != null)
                {
                    mediaPlayer.MediaOpened += mediaPlayer_MediaOpened;
                    mediaPlayer.MediaClosed += mediaPlayer_MediaClosed;
                    mediaPlayer.MediaEnding += mediaPlayer_MediaEnding;
                    mediaPlayer.MediaFailed += mediaPlayer_MediaFailed;
                    mediaPlayer.MediaStarted += mediaPlayer_MediaStarted;
                    mediaPlayer.CurrentStateChanged += mediaPlayer_CurrentStateChanged;
                    mediaPlayer.IsFullScreenChanged += mediaPlayer_IsFullScreenChanged;
                    mediaPlayer.Seeked += mediaPlayer_Seeked;
                    mediaPlayer.ScrubbingStarted += mediaPlayer_ScrubbingStarted;
                    mediaPlayer.ScrubbingCompleted += mediaPlayer_ScrubbingCompleted;
                    mediaPlayer.RateChanged += mediaPlayer_RateChanged;
                    mediaPlayer.IsLiveChanged += mediaPlayer_IsLiveChanged;
                    mediaPlayer.SelectedCaptionChanged += mediaPlayer_SelectedCaptionChanged;
                    mediaPlayer.SelectedAudioStreamChanged += mediaPlayer_SelectedAudioStreamChanged;
                    mediaPlayer.AdvertisingStateChanged += mediaPlayer_AdvertisingStateChanged;
                    foreach (var trackingPlugin in mediaPlayer.Plugins.OfType<IEventTracker>())
                    {
                        trackingPlugin.EventTracked += trackingPlugin_EventTracked;
                        trackingPlugins.Add(trackingPlugin);
                    }
                    mediaPlayer.Plugins.CollectionChanged += Plugins_CollectionChanged;
                }
            }
        }

        void Plugins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var trackingPlugin in trackingPlugins)
                {
                    trackingPlugin.EventTracked -= trackingPlugin_EventTracked;
                }
                trackingPlugins.Clear();
                foreach (var trackingPlugin in mediaPlayer.Plugins.OfType<IEventTracker>())
                {
                    trackingPlugin.EventTracked += trackingPlugin_EventTracked;
                    trackingPlugins.Add(trackingPlugin);
                }
            }
            else
            {
                if (e.NewItems != null)
                {
                    foreach (var trackingPlugin in e.NewItems.OfType<IEventTracker>())
                    {
                        trackingPlugin.EventTracked += trackingPlugin_EventTracked;
                        trackingPlugins.Add(trackingPlugin);
                    }
                }

                if (e.OldItems != null)
                {
                    foreach (var trackingPlugin in e.OldItems.OfType<IEventTracker>())
                    {
                        trackingPlugin.EventTracked -= trackingPlugin_EventTracked;
                        trackingPlugins.Remove(trackingPlugin);
                    }
                }
            }
        }

        void trackingPlugin_EventTracked(object sender, EventTrackedEventArgs e)
        {
            if (e.TrackingEvent.Area == AnalyticsPlugin.TrackingEventArea)
            {
                if (e.TrackingEvent is PositionTrackingEvent)
                {
                    var positionTracked = (PositionTrackingEvent)e.TrackingEvent;
                    if (positionTracked.PositionPercentage.HasValue)
                    {
                        if (PositionPercentageReached != null) PositionPercentageReached(this, new PositionPercentageReachedEventArgs(positionTracked.PositionPercentage.Value));
                    }
                    else
                    {
                        if (PositionReached != null) PositionReached(this, new PositionReachedEventArgs(positionTracked.Position));
                    }
                }
                else if (e.TrackingEvent is PlayTimeTrackingEvent)
                {
                    var PlayTimeTracked = (PlayTimeTrackingEvent)e.TrackingEvent;
                    if (PlayTimeTracked.PlayTimePercentage.HasValue)
                    {
                        if (PlayTimePercentageReached != null) PlayTimePercentageReached(this, new PlayTimePercentageReachedEventArgs(PlayTimeTracked.PlayTimePercentage.Value));
                    }
                    else
                    {
                        if (PlayTimeReached != null) PlayTimeReached(this, new PlayTimeReachedEventArgs(PlayTimeTracked.PlayTime));
                    }
                }
            }
        }

        void mediaPlayer_AdvertisingStateChanged(object sender, RoutedPropertyChangedEventArgs<AdvertisingState> e)
        {
            if (e.NewValue == AdvertisingState.Linear)
            {
                if (ClipStarted != null) ClipStarted(this, new ClipEventArgs(null));
            }
            else
            {
                if (ClipEnded != null) ClipEnded(this, new ClipEventArgs(null));
            }
        }

        void mediaPlayer_SelectedAudioStreamChanged(object sender, SelectedAudioStreamChangedEventArgs e)
        {
            if (AudioTrackChanged != null) AudioTrackChanged(this, EventArgs.Empty);
        }

        void mediaPlayer_SelectedCaptionChanged(object sender, RoutedPropertyChangedEventArgs<Caption> e)
        {
            if (CaptionTrackChanged != null) CaptionTrackChanged(this, EventArgs.Empty);
        }

        void mediaPlayer_IsLiveChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (IsLiveChanged != null) IsLiveChanged(this, EventArgs.Empty);
        }

        void mediaPlayer_RateChanged(object sender, RateChangedRoutedEventArgs e)
        {
            if (!mediaPlayer.IsScrubbing)
            {
                if (playbackRate != mediaPlayer.PlaybackRate) // this prevents playback rate changes due to finishing a scrub.
                {
                    playbackRate = mediaPlayer.PlaybackRate;
                    if (PlaybackRateChanged != null) PlaybackRateChanged(this, EventArgs.Empty);
                }
            }
        }

        void mediaPlayer_ScrubbingCompleted(object sender, ScrubProgressRoutedEventArgs e)
        {
            if (ScrubCompleted != null) ScrubCompleted(this, new ScrubCompletedEventArgs(e.Position));
        }

        void mediaPlayer_ScrubbingStarted(object sender, ScrubRoutedEventArgs e)
        {
            if (ScrubStarted != null) ScrubStarted(this, EventArgs.Empty);
        }

        void mediaPlayer_Seeked(object sender, SeekRoutedEventArgs e)
        {
            if (Seeked != null) Seeked(this, new SeekedEventArgs(e.PreviousPosition, e.Position));
        }

        void mediaPlayer_IsFullScreenChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (FullScreenChanged != null) FullScreenChanged(this, EventArgs.Empty);
        }

        void mediaPlayer_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (!mediaPlayer.IsScrubbing)
            {
                IsBuffering = mediaPlayer.CurrentState == MediaElementState.Buffering;
                switch (mediaPlayer.CurrentState)
                {
                    case MediaElementState.Playing:
                        if (Playing != null) Playing(this, EventArgs.Empty);
                        break;
                    case MediaElementState.Buffering:
                    case MediaElementState.Paused:
                        if (Paused != null) Paused(this, EventArgs.Empty);
                        break;
                }
            }
        }

        void mediaPlayer_MediaStarted(object sender, RoutedEventArgs e)
        {
            if (StreamStarted != null) StreamStarted(this, EventArgs.Empty);
        }

        void mediaPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
		{
			if (StreamFailed != null) StreamFailed(this, new StreamFailedEventArgs(e.ErrorMessage));
		}

        void mediaPlayer_MediaClosed(object sender, RoutedEventArgs e)
        {
            if (StreamClosed != null) StreamClosed(this, EventArgs.Empty);
        }

        void mediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (StreamLoaded != null) StreamLoaded(this, EventArgs.Empty);
        }

        void mediaPlayer_MediaEnding(object sender, MediaPlayerDeferrableEventArgs e)
        {
            if (StreamEnded != null) StreamEnded(this, EventArgs.Empty);
        }

        public double DroppedFramesPerSecond
        {
            get
			{
				return 0;
			}
        }

        public double RenderedFramesPerSecond
        {
            get
			{
				return 0;
			}
        }

        public double PlaybackRate
        {
            get { return MediaPlayer.PlaybackRate; }
        }

        public bool IsFullScreen
        {
            get { return MediaPlayer.IsFullScreen; }
        }

        public bool IsLive
        {
            get { return MediaPlayer.IsLive; }
        }

        bool isBuffering;
        public bool IsBuffering
        {
            get { return isBuffering; }
            private set
            {
                if (isBuffering != value)
                {
                    isBuffering = value;
                    if (IsBufferingChanged != null) IsBufferingChanged(this, EventArgs.Empty);
                }
            }
        }

        public Uri Source
        {
            get { return MediaPlayer.Source; }
        }

        public TimeSpan Position
        {
            get { return MediaPlayer.Position; }
        }

        public TimeSpan Duration
        {
            get { return MediaPlayer.Duration; }
        }

        public string AudioTrackId
        {
            get { return MediaPlayer.SelectedAudioStream != null ? MediaPlayer.SelectedAudioStream.Language : null; }
        }

        public string CaptionTrackId
        {
            get { return MediaPlayer.SelectedCaption != null ? MediaPlayer.SelectedCaption.Id : null; }
        }

        /// <inheritdoc /> 
        public event EventHandler<ClipEventArgs> ClipStarted;

        /// <inheritdoc /> 
        public event EventHandler<ClipEventArgs> ClipEnded;

        /// <inheritdoc /> 
        public event EventHandler<StreamFailedEventArgs> StreamFailed;

        /// <inheritdoc /> 
        public event EventHandler<PositionReachedEventArgs> PositionReached;

        /// <inheritdoc /> 
        public event EventHandler<PlayTimeReachedEventArgs> PlayTimeReached;

        /// <inheritdoc /> 
        public event EventHandler<PositionPercentageReachedEventArgs> PositionPercentageReached;

        /// <inheritdoc /> 
        public event EventHandler<PlayTimePercentageReachedEventArgs> PlayTimePercentageReached;

        /// <inheritdoc /> 
        public event EventHandler<SeekedEventArgs> Seeked;

        /// <inheritdoc /> 
        public event EventHandler<ScrubCompletedEventArgs> ScrubCompleted;

        /// <inheritdoc /> 
        public event EventHandler<object> StreamLoaded;

        /// <inheritdoc /> 
        public event EventHandler<object> StreamStarted;

        /// <inheritdoc /> 
        public event EventHandler<object> StreamClosed;

        /// <inheritdoc /> 
        public event EventHandler<object> StreamEnded;

        /// <inheritdoc /> 
        public event EventHandler<object> Playing;

        /// <inheritdoc /> 
        public event EventHandler<object> Paused;

        /// <inheritdoc /> 
        public event EventHandler<object> IsBufferingChanged;

        /// <inheritdoc /> 
        public event EventHandler<object> FullScreenChanged;

        /// <inheritdoc /> 
        public event EventHandler<object> ScrubStarted;

        /// <inheritdoc /> 
        public event EventHandler<object> PlaybackRateChanged;

        /// <inheritdoc /> 
        public event EventHandler<object> IsLiveChanged;

        /// <inheritdoc /> 
        public event EventHandler<object> CaptionTrackChanged;

        /// <inheritdoc /> 
        public event EventHandler<object> AudioTrackChanged;
    }
}
