using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VideoAdvertising;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data;
#endif

namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// Binds a vpaid ad player to the UI. This does 2 things: 1) reacts to changes in the vpaid player 2) pushes commands at the vpaid player to pause, resume, stop and skip.
    /// Supports both VPAID 1.1 and VPAID 2.0 players
    /// </summary>
    public class VpaidLinearAdViewModel : InteractiveViewModelBase
    {
        /// <summary>
        /// HACK: Allows an instance to be created from Xaml. Without this, xamltypeinfo is not generated and binding will not work.
        /// </summary>
        public VpaidLinearAdViewModel() { }

        /// <summary>
        /// Gets the VPAID 1.1 player associated with the view model.
        /// </summary>
        public IVpaid Vpaid { get; private set; }

        /// <summary>
        /// Gets the VPAID 2.0 player associated with the view mode. This is the same object as Vpaid and is null if the player only supports v1.1
        /// </summary>
        public IVpaid2 Vpaid2 { get; private set; }

        /// <summary>
        /// Gets the MediaPlayer instance the ViewModel is associated with.
        /// </summary>
        public MediaPlayer MediaPlayer { get; private set; }

        private vPaidState state;
        private TimeSpan estimatedDuration;

        enum vPaidState
        {
            None,
            Loaded,
            Playing,
            Paused,
            Completed,
            Failure
        }

        internal VpaidLinearAdViewModel(IVpaid vpaid, MediaPlayer mediaPlayer)
        {
            MediaPlayer = mediaPlayer;
            Vpaid = vpaid;
            if (Vpaid is IVpaid2)
            {
                Vpaid2 = Vpaid as IVpaid2;
            }
            state = vPaidState.None;

            WireVpaid();
            WireMediaPlayer();
        }

        private void WireMediaPlayer()
        {
            MediaPlayer.IsFullScreenChanged += MediaPlayer_IsFullScreenChanged;
            MediaPlayer.IsMutedChanged += MediaPlayer_IsMutedChanged;
            MediaPlayer.VolumeChanged += MediaPlayer_VolumeChanged;
            MediaPlayer.IsInfoEnabledChanged += MediaPlayer_IsInfoEnabledChanged;
            MediaPlayer.IsInfoAllowedChanged += MediaPlayer_IsInfoEnabledChanged;
        }

        private void UnwireMediaPlayer()
        {
            MediaPlayer.IsFullScreenChanged -= MediaPlayer_IsFullScreenChanged;
            MediaPlayer.IsMutedChanged -= MediaPlayer_IsMutedChanged;
            MediaPlayer.VolumeChanged -= MediaPlayer_VolumeChanged;
            MediaPlayer.IsInfoEnabledChanged -= MediaPlayer_IsInfoEnabledChanged;
            MediaPlayer.IsInfoAllowedChanged -= MediaPlayer_IsInfoEnabledChanged;
        }

        void MediaPlayer_IsInfoEnabledChanged(object sender, RoutedEventArgs e)
        {
            NotifyIsInfoEnabledChanged();
        }

        void MediaPlayer_VolumeChanged(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged(() => Volume);
        }

        void MediaPlayer_IsMutedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            OnPropertyChanged(() => IsMuted);
        }

        void MediaPlayer_IsFullScreenChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            OnPropertyChanged(() => IsFullScreen);
        }

        private void WireVpaid()
        {
            Vpaid.AdRemainingTimeChange += vpaid_AdRemainingTimeChange;
            Vpaid.AdError += vpaid_AdError;
            Vpaid.AdLoaded += vpaid_AdLoaded;
            Vpaid.AdStarted += vpaid_AdStarted;
            Vpaid.AdStopped += vpaid_AdStopped;
            Vpaid.AdPlaying += vpaid_AdPlaying;
            Vpaid.AdPaused += vpaid_AdPaused;
            Vpaid.AdClickThru += Vpaid_AdClickThru;
            if (Vpaid2 != null)
            {
                Vpaid2 = Vpaid as IVpaid2;
                Vpaid2.AdSkippableStateChange += Vpaid2_AdSkippableStateChange;
                Vpaid2.AdDurationChange += Vpaid2_AdDurationChange;
            }
        }

        private void UnwireVpaid()
        {
            Vpaid.AdRemainingTimeChange -= vpaid_AdRemainingTimeChange;
            Vpaid.AdError -= vpaid_AdError;
            Vpaid.AdLoaded -= vpaid_AdLoaded;
            Vpaid.AdStarted -= vpaid_AdStarted;
            Vpaid.AdStopped -= vpaid_AdStopped;
            Vpaid.AdPlaying -= vpaid_AdPlaying;
            Vpaid.AdPaused -= vpaid_AdPaused;
            Vpaid.AdClickThru -= Vpaid_AdClickThru;
            if (Vpaid2 != null)
            {
                Vpaid2.AdSkippableStateChange -= Vpaid2_AdSkippableStateChange;
                Vpaid2.AdDurationChange -= Vpaid2_AdDurationChange;
                Vpaid2 = null;
            }
            Vpaid = null;
        }

        void Vpaid2_AdDurationChange(object sender, object e)
        {
            OnPropertyChanged(() => Duration);
            OnPropertyChanged(() => EndTime);
            OnPropertyChanged(() => TimeRemaining);
        }

        void Vpaid2_AdSkippableStateChange(object sender, object e)
        {
            NotifyIsSkipNextEnabledChanged();
        }

        void Vpaid_AdClickThru(object sender, ClickThroughEventArgs e)
        {
            OnInteracting();
        }

        void vpaid_AdPaused(object sender, object e)
        {
            state = vPaidState.Paused;
            NotifyIsPlayResumeEnabledChanged();
            NotifyIsPauseEnabledChanged();
            OnCurrentStateChanged(new RoutedEventArgs());
        }

        void vpaid_AdPlaying(object sender, object e)
        {
            state = vPaidState.Playing;
            NotifyIsPlayResumeEnabledChanged();
            NotifyIsPauseEnabledChanged();
            OnCurrentStateChanged(new RoutedEventArgs());
        }

        void vpaid_AdStopped(object sender, object e)
        {
            state = vPaidState.Completed;
            NotifyIsPlayResumeEnabledChanged();
            NotifyIsPauseEnabledChanged();
            OnCurrentStateChanged(new RoutedEventArgs());

            UnwireVpaid();
            UnwireMediaPlayer();
        }

        void vpaid_AdStarted(object sender, object e)
        {
            state = vPaidState.Playing;
            NotifyIsPlayResumeEnabledChanged();
            NotifyIsPauseEnabledChanged();
            OnCurrentStateChanged(new RoutedEventArgs());
        }

        void vpaid_AdLoaded(object sender, object e)
        {
            state = vPaidState.Loaded;
            OnPropertyChanged(() => Duration);
            OnPropertyChanged(() => TimeRemaining);
            OnPropertyChanged(() => SignalStrength);
            OnPropertyChanged(() => MediaQuality);
            OnCurrentStateChanged(new RoutedEventArgs());
            estimatedDuration = Vpaid.AdRemainingTime;  // used to estimate the duration of the ad for vpaid 1.1
        }

        void vpaid_AdError(object sender, VpaidMessageEventArgs e)
        {
            state = vPaidState.Failure;
            NotifyIsPlayResumeEnabledChanged();
            NotifyIsPauseEnabledChanged();
            OnCurrentStateChanged(new RoutedEventArgs());
        }

        void vpaid_AdRemainingTimeChange(object sender, object e)
        {
            OnPropertyChanged(() => TimeRemaining);
            OnPropertyChanged(() => MaxPosition);
            OnPropertyChanged(() => Position);
        }

        /// <inheritdoc /> 
        protected override void OnStop()
        {
            Vpaid.StopAd();
        }

        /// <inheritdoc /> 
        protected override void OnPause()
        {
            Vpaid.PauseAd();
        }
        
        /// <inheritdoc /> 
        protected override void OnInvokeCaptionSelection()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc /> 
        protected override void OnInvokeAudioSelection()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc /> 
        protected override void OnSkipPrevious(VisualMarker marker)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc /> 
        protected override void OnSkipNext(VisualMarker marker)
        {
            if (Vpaid2 != null) Vpaid2.SkipAd();
        }

        /// <inheritdoc /> 
        protected override void OnSkipBack(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc /> 
        protected override void OnSkipAhead(TimeSpan position)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc /> 
        protected override void OnSeek(TimeSpan position, out bool canceled)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc /> 
        protected override void OnCompleteScrub(TimeSpan position, ref bool canceled)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc /> 
        protected override void OnStartScrub(TimeSpan position, out bool canceled)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc /> 
        protected override void OnScrub(TimeSpan position, out bool canceled)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc /> 
        protected override void OnGoLive()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc /> 
        protected override void OnInvokeInfo()
        {
            MediaPlayer.InvokeInfo();
        }

        /// <inheritdoc /> 
        protected override void OnPlayResume()
        {
            Vpaid.ResumeAd();
        }

        /// <inheritdoc /> 
        protected override void OnReplay()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc /> 
        protected override void OnDecreasePlaybackRate()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc /> 
        protected override void OnIncreasePlaybackRate()
        {
            throw new NotImplementedException();
        }

#if SILVERLIGHT
        /// <inheritdoc /> 
        protected override void OnCycleDisplayMode()
        {
            throw new NotImplementedException();
        }
#endif

        /// <inheritdoc /> 
        public override IEnumerable<Caption> AvailableCaptions
        {
            get { return Enumerable.Empty<Caption>(); }
        }

        /// <inheritdoc /> 
        public override Caption SelectedCaption { get; set; }

        /// <inheritdoc /> 
        public override IEnumerable<VisualMarker> VisualMarkers
        {
            get { return Enumerable.Empty<VisualMarker>(); }
        }

        /// <inheritdoc /> 
        public override bool IsGoLiveEnabled
        {
            get { return false; }
        }

        /// <inheritdoc /> 
        public override bool IsInfoEnabled
        {
            get { return MediaPlayer.IsInfoEnabled && MediaPlayer.IsInfoAllowed; }
        }

        /// <inheritdoc /> 
        public override bool IsPlayResumeEnabled
        {
            get { return state == vPaidState.Paused; }
        }

        /// <inheritdoc /> 
        public override bool IsPauseEnabled
        {
            get { return state == vPaidState.Playing; }
        }

        /// <inheritdoc /> 
        public override bool IsStopEnabled
        {
            get { return false; }
        }

        /// <inheritdoc /> 
        public override bool IsReplayEnabled
        {
            get { return false; }
        }

        /// <inheritdoc /> 
        public override bool IsAudioSelectionEnabled
        {
            get { return false; }
        }

        /// <inheritdoc /> 
        public override bool IsCaptionSelectionEnabled
        {
            get { return false; }
        }

        /// <inheritdoc /> 
        public override bool IsRewindEnabled
        {
            get { return false; }
        }

        /// <inheritdoc /> 
        public override bool IsFastForwardEnabled
        {
            get { return false; }
        }

        /// <inheritdoc /> 
        public override bool IsSlowMotionEnabled
        {
            get { return false; }
        }

        /// <inheritdoc /> 
        public override bool IsSeekEnabled
        {
            get { return false; }
        }

        /// <inheritdoc /> 
        public override bool IsSkipPreviousEnabled
        {
            get { return false; }
        }

        /// <inheritdoc /> 
        public override bool IsSkipNextEnabled
        {
            get { return Vpaid2 != null ? Vpaid2.AdSkippableState : false; }
        }

        /// <inheritdoc /> 
        public override bool IsSkipBackEnabled
        {
            get { return false; }
        }

        /// <inheritdoc /> 
        public override bool IsSkipAheadEnabled
        {
            get { return false; }
        }

        /// <inheritdoc /> 
        public override bool IsScrubbingEnabled
        {
            get { return false; }
        }

        /// <inheritdoc /> 
        protected override bool _IsMuted
        {
            get { return MediaPlayer.IsMuted; }
            set { MediaPlayer.IsMuted = value; }
        }

        /// <inheritdoc /> 
        protected override bool _IsFullScreen
        {
            get { return MediaPlayer.IsFullScreen; }
            set { MediaPlayer.IsFullScreen = value; }
        }
        
        /// <inheritdoc /> 
        protected override bool _IsSlowMotion
        {
            get { return false; }
            set { throw new NotImplementedException(); }
        }

        /// <inheritdoc /> 
        protected override double _Volume
        {
            get { return MediaPlayer.Volume; }
            set
            {
                // this will end up setting the vpaid player because it's monitoring the main player
                MediaPlayer.Volume = value;
            }
        }

        /// <inheritdoc /> 
        public override double BufferingProgress
        {
            get { return 0; }
        }

        /// <inheritdoc /> 
        public override double DownloadProgress
        {
            get { return 1; }
        }

        /// <inheritdoc /> 
        public override TimeSpan Duration
        {
            get
            {
                if (Vpaid2 != null)
                {
                    return Vpaid2.AdDuration;
                }
                else
                {
                    return estimatedDuration;   // assume the timeremaining when the ad started is the duration.
                }
            }
        }

        /// <inheritdoc /> 
        public override TimeSpan EndTime
        {
            get { return Duration; }
        }

        /// <inheritdoc /> 
        public override TimeSpan StartTime
        {
            get { return TimeSpan.Zero; }
        }

        /// <inheritdoc /> 
        public override TimeSpan TimeRemaining
        {
            get { return Vpaid.AdRemainingTime; }
        }

        /// <inheritdoc /> 
        public override TimeSpan Position
        {
            get { return Duration.Subtract(TimeRemaining); }
        }

        /// <inheritdoc /> 
        public override TimeSpan MaxPosition
        {
            get { return Position; }
        }

        /// <inheritdoc /> 
        public override MediaElementState CurrentState
        {
            get
            {
                switch (state)
                {
                    case vPaidState.None:
                        return MediaElementState.Closed;
                    case vPaidState.Loaded:
                        return MediaElementState.Opening;
                    case vPaidState.Paused:
                        return MediaElementState.Paused;
                    case vPaidState.Playing:
                        return MediaElementState.Playing;
                    case vPaidState.Failure:
                        return MediaElementState.Closed;
                    case vPaidState.Completed:
                        return MediaElementState.Stopped;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <inheritdoc /> 
        public override IEnumerable<AudioStream> AvailableAudioStreams
        {
            get { return Enumerable.Empty<AudioStream>(); }
        }

        /// <inheritdoc /> 
        public override AudioStream SelectedAudioStream { get; set; }

        /// <inheritdoc /> 
        public override IValueConverter TimeFormatConverter
        {
            get { return new StringFormatConverter() { StringFormat = MediaPlayer.DefaultTimeFormat }; }
        }

        /// <inheritdoc /> 
        public override TimeSpan? SkipBackInterval
        {
            get { return TimeSpan.FromSeconds(5); }
        }

        /// <inheritdoc /> 
        public override TimeSpan? SkipAheadInterval
        {
            get { return TimeSpan.FromSeconds(5); }
        }

        /// <inheritdoc /> 
        public override double SignalStrength
        {
            get { return 1; }
        }

        /// <inheritdoc /> 
        public override MediaQuality MediaQuality
        {
            get
            {
                if (Vpaid2 != null)
                {
                    return Vpaid2.AdHeight >= 720 ? MediaQuality.HighDefinition : MediaQuality.StandardDefinition;
                }
                else
                {
                    return MediaPlayer.MediaQuality; // vpaid 1.1 has no way to determine this, fall back on whatever the content of the main video was.
                }
            }
        }

        /// <inheritdoc /> 
        public override ImageSource ThumbnailImageSource
        {
            get { return null; }
        }
    }
}
