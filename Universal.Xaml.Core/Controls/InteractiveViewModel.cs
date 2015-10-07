using System;
using System.Net;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.Generic;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Provides an IInteractiveViewModel implementation for MediaPlayer
    /// </summary>
    public class InteractiveViewModel : InteractiveViewModelBase
    {
        private MediaPlayer mediaPlayer;

        /// <summary>
        /// Creates a new instance of InteractiveViewModel
        /// </summary>
        public InteractiveViewModel() { }

        /// <summary>
        /// Creates a new instance of InteractiveViewModel
        /// </summary>
        /// <param name="mediaPlayer">The mediaplayer instance to adapt.</param>
        public InteractiveViewModel(MediaPlayer mediaPlayer)
        {
            MediaPlayer = mediaPlayer;
        }

        /// <summary>
        /// The MediaPlayer instance the ViewModel is wrapping
        /// </summary>
        public MediaPlayer MediaPlayer
        {
            get { return mediaPlayer; }
            set
            {
                if (mediaPlayer != null)
                {
                    UnwireMediaPlayer();
                }
                mediaPlayer = value;
                if (mediaPlayer != null)
                {
                    WireMediaPlayer();
                }
            }
        }

        private void UnwireMediaPlayer()
        {
            // this is meant to exist for the lifetime of the MediaPlayer.
            throw new NotImplementedException();
        }

        private void WireMediaPlayer()
        {
            MediaPlayer.IsPlayResumeEnabledChanged += (s, e) => NotifyIsPlayResumeEnabledChanged();
            MediaPlayer.IsPauseEnabledChanged += (s, e) => NotifyIsPauseEnabledChanged();
            MediaPlayer.IsStopEnabledChanged += (s, e) => NotifyIsStopEnabledChanged();
            MediaPlayer.IsReplayEnabledChanged += (s, e) => NotifyIsReplayEnabledChanged();
            MediaPlayer.IsAudioSelectionEnabledChanged += (s, e) => NotifyIsAudioSelectionEnabledChanged();
            MediaPlayer.IsCaptionSelectionEnabledChanged += (s, e) => NotifyIsCaptionSelectionEnabledChanged();
            MediaPlayer.IsRewindEnabledChanged += (s, e) => NotifyIsRewindEnabledChanged();
            MediaPlayer.IsFastForwardEnabledChanged += (s, e) => NotifyIsFastForwardEnabledChanged();
            MediaPlayer.IsSlowMotionEnabledChanged += (s, e) => NotifyIsSlowMotionEnabledChanged();
            MediaPlayer.IsSeekEnabledChanged += (s, e) => NotifyIsSeekEnabledChanged();
            MediaPlayer.IsSkipPreviousEnabledChanged += (s, e) => NotifyIsSkipPreviousEnabledChanged();
            MediaPlayer.IsSkipNextEnabledChanged += (s, e) => NotifyIsSkipNextEnabledChanged();
            MediaPlayer.IsSkipBackEnabledChanged += (s, e) => NotifyIsSkipBackEnabledChanged();
            MediaPlayer.IsSkipAheadEnabledChanged += (s, e) => NotifyIsSkipAheadEnabledChanged();
            MediaPlayer.IsScrubbingEnabledChanged += (s, e) => NotifyIsScrubbingEnabledChanged();
            MediaPlayer.IsGoLiveEnabledChanged += (s, e) => NotifyIsGoLiveEnabledChanged();
            MediaPlayer.IsInfoEnabledChanged += (s, e) => NotifyIsInfoEnabledChanged();
            MediaPlayer.IsMoreEnabledChanged += (s, e) => NotifyIsMoreEnabledChanged();
            MediaPlayer.IsFullScreenEnabledChanged += (s, e) => NotifyIsFullScreenEnabledChanged();
#if !WINDOWS80
            MediaPlayer.IsZoomEnabledChanged += (s, e) => NotifyIsZoomEnabledChanged();
#endif

            MediaPlayer.IsPlayResumeAllowedChanged += (s, e) => NotifyIsPlayResumeEnabledChanged();
            MediaPlayer.IsPauseAllowedChanged += (s, e) => NotifyIsPauseEnabledChanged();
            MediaPlayer.IsStopAllowedChanged += (s, e) => NotifyIsStopEnabledChanged();
            MediaPlayer.IsReplayAllowedChanged += (s, e) => NotifyIsReplayEnabledChanged();
            MediaPlayer.IsAudioSelectionAllowedChanged += (s, e) => NotifyIsAudioSelectionEnabledChanged();
            MediaPlayer.IsCaptionSelectionAllowedChanged += (s, e) => NotifyIsCaptionSelectionEnabledChanged();
            MediaPlayer.IsRewindAllowedChanged += (s, e) => NotifyIsRewindEnabledChanged();
            MediaPlayer.IsFastForwardAllowedChanged += (s, e) => NotifyIsFastForwardEnabledChanged();
            MediaPlayer.IsSlowMotionAllowedChanged += (s, e) => NotifyIsSlowMotionEnabledChanged();
            MediaPlayer.IsSeekAllowedChanged += (s, e) => NotifyIsSeekEnabledChanged();
            MediaPlayer.IsSkipPreviousAllowedChanged += (s, e) => NotifyIsSkipPreviousEnabledChanged();
            MediaPlayer.IsSkipNextAllowedChanged += (s, e) => NotifyIsSkipNextEnabledChanged();
            MediaPlayer.IsSkipBackAllowedChanged += (s, e) => NotifyIsSkipBackEnabledChanged();
            MediaPlayer.IsSkipAheadAllowedChanged += (s, e) => NotifyIsSkipAheadEnabledChanged();
            MediaPlayer.IsScrubbingAllowedChanged += (s, e) => NotifyIsScrubbingEnabledChanged();
            MediaPlayer.IsGoLiveAllowedChanged += (s, e) => NotifyIsGoLiveEnabledChanged();
            MediaPlayer.IsInfoAllowedChanged += (s, e) => NotifyIsInfoEnabledChanged();

            MediaPlayer.IsMutedChanged += (s, e) => OnPropertyChanged(() => IsMuted);
            MediaPlayer.IsFullScreenChanged += (s, e) => OnPropertyChanged(() => IsFullScreen);
#if !WINDOWS80
            MediaPlayer.StretchChanged += (s, e) => OnPropertyChanged(() => Zoom);
#endif
            MediaPlayer.IsSlowMotionChanged += (s, e) => OnPropertyChanged(() => IsSlowMotion);
            MediaPlayer.CurrentStateChanged += (s, e) => OnCurrentStateChanged(e);
            MediaPlayer.BufferingProgressChanged += (s, e) => OnPropertyChanged(() => BufferingProgress);
            MediaPlayer.DownloadProgressChanged += (s, e) => OnPropertyChanged(() => DownloadProgress);
            MediaPlayer.VolumeChanged += (s, e) => OnPropertyChanged(() => Volume);
            MediaPlayer.StartTimeChanged += (s, e) => OnPropertyChanged(() => StartTime);
            MediaPlayer.EndTimeChanged += (s, e) => OnPropertyChanged(() => EndTime);
            MediaPlayer.EndTimeChanged += (s, e) => OnPropertyChanged(() => MaxPosition);
            MediaPlayer.DurationChanged += (s, e) => OnPropertyChanged(() => Duration);
            MediaPlayer.TimeRemainingChanged += (s, e) => OnPropertyChanged(() => TimeRemaining);
            MediaPlayer.LivePositionChanged += (s, e) => OnPropertyChanged(() => MaxPosition);
            MediaPlayer.TimeFormatConverterChanged += (s, e) => OnPropertyChanged(() => TimeFormatConverter);
            MediaPlayer.SkipBackIntervalChanged += (s, e) => OnPropertyChanged(() => SkipBackInterval);
            MediaPlayer.SkipAheadIntervalChanged += (s, e) => OnPropertyChanged(() => SkipAheadInterval);
            MediaPlayer.VirtualPositionChanged += (s, e) => OnPropertyChanged(() => Position);
            MediaPlayer.SignalStrengthChanged += (s, e) => OnPropertyChanged(() => SignalStrength);
            MediaPlayer.MediaQualityChanged += (s, e) => OnPropertyChanged(() => MediaQuality);
            MediaPlayer.ThumbnailImageSourceChanged += (s, e) => OnPropertyChanged(() => ThumbnailImageSource);

#if WINDOWS_UWP
            MediaPlayer.IsCastingEnabledChanged += (s, e) => OnPropertyChanged(() => IsCastingEnabled);
#endif
        }

        #region Methods

        /// <inheritdoc /> 
        protected override void OnStop()
        {
            MediaPlayer.Stop();
        }

        /// <inheritdoc /> 
        protected override void OnPause()
        {
            MediaPlayer.Pause();
        }

        /// <inheritdoc /> 
        protected override void OnInvokeCaptionSelection()
        {
            MediaPlayer.InvokeCaptionSelection();
        }

#if WINDOWS_UWP
        /// <inheritdoc /> 
        protected override void OnInvokeCast()
        {
            MediaPlayer.InvokeCast();
        }
#endif

        /// <inheritdoc /> 
        protected override void OnInvokeAudioSelection()
        {
            MediaPlayer.InvokeAudioSelection();
        }

        /// <inheritdoc /> 
        protected override void OnSkipPrevious(VisualMarker marker)
        {
            MediaPlayer.SkipBack(marker != null ? GetMediaPlayerPosition(marker.Time) : MediaPlayer.StartTime);
        }

        /// <inheritdoc /> 
        protected override void OnSkipNext(VisualMarker marker)
        {
            MediaPlayer.SkipAhead(marker != null ? GetMediaPlayerPosition(marker.Time) : MediaPlayer.LivePosition.GetValueOrDefault(MediaPlayer.EndTime));
        }

        /// <inheritdoc /> 
        protected override void OnSkipBack(TimeSpan position)
        {
            MediaPlayer.SkipBack(GetMediaPlayerPosition(position));
        }

        /// <inheritdoc /> 
        protected override void OnSkipAhead(TimeSpan position)
        {
            MediaPlayer.SkipAhead(GetMediaPlayerPosition(position));
        }

        /// <inheritdoc /> 
        protected override void OnSeek(TimeSpan position, out bool canceled)
        {
            MediaPlayer.Seek(GetMediaPlayerPosition(position), out canceled);
        }

        /// <inheritdoc /> 
        protected override void OnCompleteScrub(TimeSpan position, ref bool canceled)
        {
            MediaPlayer.CompleteScrub(GetMediaPlayerPosition(position), ref canceled);
        }

        /// <inheritdoc /> 
        protected override void OnStartScrub(TimeSpan position, out bool canceled)
        {
            MediaPlayer.StartScrub(GetMediaPlayerPosition(position), out canceled);
        }

        /// <inheritdoc /> 
        protected override void OnScrub(TimeSpan position, out bool canceled)
        {
            MediaPlayer.Scrub(GetMediaPlayerPosition(position), out canceled);
        }

        /// <inheritdoc /> 
        protected override void OnGoLive()
        {
            MediaPlayer.SeekToLive();
        }

        /// <inheritdoc /> 
        protected override void OnInvokeInfo()
        {
            MediaPlayer.InvokeInfo();
        }

        /// <inheritdoc /> 
        protected override void OnInvokeMore()
        {
            MediaPlayer.InvokeMore();
        }

        /// <inheritdoc /> 
        protected override void OnPlayResume()
        {
            MediaPlayer.PlayResume();
        }

        /// <inheritdoc /> 
        protected override void OnReplay()
        {
            MediaPlayer.Replay();
        }

        /// <inheritdoc /> 
        protected override void OnDecreasePlaybackRate()
        {
            MediaPlayer.DecreasePlaybackRate();
        }

        /// <inheritdoc /> 
        protected override void OnIncreasePlaybackRate()
        {
            MediaPlayer.IncreasePlaybackRate();
        }

        /// <summary>
        /// Converts the position of the view model to that of the media player
        /// </summary>
        /// <param name="viewModelPosition">The view model's position</param>
        /// <returns>The media player's position</returns>
        protected virtual TimeSpan GetMediaPlayerPosition(TimeSpan viewModelPosition)
        {
            return MediaPlayer.IsStartTimeOffset ? viewModelPosition : MediaPlayer.StartTime.Add(viewModelPosition);
        }

        /// <summary>
        /// Converts the position of the media player to that of the view model
        /// </summary>
        /// <param name="mediaPlayerPosition">The media player's position</param>
        /// <returns>The view model's position</returns>
        protected virtual TimeSpan GetViewModelPosition(TimeSpan mediaPlayerPosition)
        {
            return MediaPlayer.IsStartTimeOffset ? mediaPlayerPosition : mediaPlayerPosition.Subtract(MediaPlayer.StartTime);
        }

        TimeSpan? GetViewModelPosition(TimeSpan? mediaPlayerPosition)
        {
            return mediaPlayerPosition.HasValue ? GetViewModelPosition(mediaPlayerPosition.Value) : mediaPlayerPosition;
        }
        #endregion

        #region AvailableCaptions
        /// <inheritdoc /> 
        public override IEnumerable<Caption> AvailableCaptions
        {
            get { return MediaPlayer.AvailableCaptions; }
        }
        #endregion

        #region SelectedCaption
        /// <inheritdoc /> 
        public override Caption SelectedCaption
        {
            get { return MediaPlayer.SelectedCaption; }
            set { MediaPlayer.SelectedCaption = value; }
        }
        #endregion

        #region AvailableAudioStreams
        /// <inheritdoc /> 
        public override IEnumerable<AudioStream> AvailableAudioStreams
        {
            get { return MediaPlayer.AvailableAudioStreams; }
        }
        #endregion

        #region SelectedAudioStream
        /// <inheritdoc /> 
        public override AudioStream SelectedAudioStream
        {
            get { return MediaPlayer.SelectedAudioStream; }
            set { MediaPlayer.SelectedAudioStream = value; }
        }
        #endregion

        #region VisualMarkers
        /// <inheritdoc /> 
        public override IEnumerable<VisualMarker> VisualMarkers
        {
            get { return MediaPlayer.VisualMarkers; }
        }
        #endregion

        #region IsGoLiveEnabled

        /// <inheritdoc /> 
        public override bool IsGoLiveEnabled
        {
            get { return MediaPlayer.IsGoLiveEnabled && MediaPlayer.IsGoLiveAllowed; }
        }
        #endregion

        #region IsInfoEnabled

        /// <inheritdoc /> 
        public override bool IsInfoEnabled
        {
            get { return MediaPlayer.IsInfoEnabled && MediaPlayer.IsInfoAllowed; }
        }
        #endregion

        #region IsMoreEnabled

        /// <inheritdoc /> 
        public override bool IsMoreEnabled
        {
            get { return MediaPlayer.IsMoreEnabled; }
        }
        #endregion

        #region IsFullScreenEnabled

        /// <inheritdoc /> 
        public override bool IsFullScreenEnabled
        {
            get { return MediaPlayer.IsFullScreenEnabled; }
        }
        #endregion

#if WINDOWS_UWP
        #region IsCastingEnabled
        /// <inheritdoc /> 
        public override bool IsCastingEnabled
        {
            get { return MediaPlayer.IsCastingEnabled; }
        }
        #endregion
#endif

#if !WINDOWS80
        #region IsZoomEnabled

        /// <inheritdoc /> 
        public override bool IsZoomEnabled
        {
            get { return MediaPlayer.IsZoomEnabled; }
        }
        #endregion
#endif

        #region IsPlayResumeEnabled

        /// <inheritdoc /> 
        public override bool IsPlayResumeEnabled
        {
            get { return MediaPlayer.IsPlayResumeEnabled && MediaPlayer.IsPlayResumeAllowed; }
        }
        #endregion

        #region IsPauseEnabled

        /// <inheritdoc /> 
        public override bool IsPauseEnabled
        {
            get { return MediaPlayer.IsPauseEnabled && MediaPlayer.IsPauseAllowed; }
        }

        #endregion

        #region IsStopEnabled

        /// <inheritdoc /> 
        public override bool IsStopEnabled
        {
            get { return MediaPlayer.IsStopEnabled && MediaPlayer.IsStopAllowed; }
        }

        #endregion

        #region IsReplayEnabled

        /// <inheritdoc /> 
        public override bool IsReplayEnabled
        {
            get { return MediaPlayer.IsReplayEnabled && MediaPlayer.IsReplayAllowed; }
        }

        #endregion

        #region IsAudioSelectionEnabled

        /// <inheritdoc /> 
        public override bool IsAudioSelectionEnabled
        {
            get { return MediaPlayer.IsAudioSelectionEnabled && MediaPlayer.IsAudioSelectionAllowed; }
        }

        #endregion

        #region IsCaptionSelectionEnabled

        /// <inheritdoc /> 
        public override bool IsCaptionSelectionEnabled
        {
            get { return MediaPlayer.IsCaptionSelectionEnabled && MediaPlayer.IsCaptionSelectionAllowed; }
        }

        #endregion

        #region IsRewindEnabled

        /// <inheritdoc /> 
        public override bool IsRewindEnabled
        {
            get { return MediaPlayer.IsRewindEnabled && MediaPlayer.IsRewindAllowed; }
        }

        #endregion

        #region IsFastForwardEnabled

        /// <inheritdoc /> 
        public override bool IsFastForwardEnabled
        {
            get { return MediaPlayer.IsFastForwardEnabled && MediaPlayer.IsFastForwardAllowed; }
        }

        #endregion

        #region IsSlowMotionEnabled

        /// <inheritdoc /> 
        public override bool IsSlowMotionEnabled
        {
            get { return MediaPlayer.IsSlowMotionEnabled && MediaPlayer.IsSlowMotionAllowed; }
        }

        #endregion

        #region IsSeekEnabled

        /// <inheritdoc /> 
        public override bool IsSeekEnabled
        {
            get { return MediaPlayer.IsSeekEnabled && MediaPlayer.IsSeekAllowed; }
        }

        #endregion

        #region IsSkipPreviousEnabled

        /// <inheritdoc /> 
        public override bool IsSkipPreviousEnabled
        {
            get { return MediaPlayer.IsSkipPreviousEnabled && MediaPlayer.IsSkipPreviousAllowed; }
        }

        #endregion

        #region IsSkipNextEnabled

        /// <inheritdoc /> 
        public override bool IsSkipNextEnabled
        {
            get
            {
                return MediaPlayer.IsSkipNextEnabled && MediaPlayer.IsSkipNextAllowed;
            }
        }

        #endregion

        #region IsSkipBackEnabled

        /// <inheritdoc /> 
        public override bool IsSkipBackEnabled
        {
            get { return MediaPlayer.IsSkipBackEnabled && MediaPlayer.IsSkipBackAllowed; }
        }

        #endregion

        #region IsSkipAheadEnabled

        /// <inheritdoc /> 
        public override bool IsSkipAheadEnabled
        {
            get
            {
                return MediaPlayer.IsSkipAheadEnabled && MediaPlayer.IsSkipAheadAllowed;
            }
        }

        #endregion

        #region IsScrubbingEnabled

        /// <inheritdoc /> 
        public override bool IsScrubbingEnabled
        {
            get
            {
                return MediaPlayer.IsScrubbingEnabled && MediaPlayer.IsScrubbingAllowed;
            }
        }

        #endregion

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
            get { return MediaPlayer.IsSlowMotion; }
            set { MediaPlayer.IsSlowMotion = value; }
        }
        /// <inheritdoc /> 
        protected override double _Volume
        {
            get { return MediaPlayer.Volume; }
            set { MediaPlayer.Volume = value; }
        }
#if !WINDOWS80
        /// <inheritdoc /> 
        protected override bool _Zoom
        {
            get { return MediaPlayer.Stretch == Stretch.UniformToFill; }
            set { MediaPlayer.Stretch = value ? Stretch.UniformToFill : Stretch.Uniform; }
        }
#endif
        /// <inheritdoc /> 
        public override double BufferingProgress { get { return MediaPlayer.BufferingProgress; } }
        /// <inheritdoc /> 
        public override double DownloadProgress { get { return MediaPlayer.DownloadProgress; } }
        /// <inheritdoc /> 
        public override TimeSpan StartTime { get { return GetViewModelPosition(MediaPlayer.StartTime); } }
        /// <inheritdoc /> 
        public override TimeSpan EndTime { get { return GetViewModelPosition(MediaPlayer.EndTime); } }
        /// <inheritdoc /> 
        public override TimeSpan Duration { get { return MediaPlayer.Duration; } }
        /// <inheritdoc /> 
        public override TimeSpan TimeRemaining { get { return MediaPlayer.TimeRemaining; } }
        /// <inheritdoc /> 
        public override TimeSpan Position { get { return GetViewModelPosition(MediaPlayer.VirtualPosition); } }
        /// <inheritdoc /> 
        public override TimeSpan MaxPosition { get { return GetViewModelPosition(MediaPlayer.LivePosition.GetValueOrDefault(MediaPlayer.EndTime)); } }
        /// <inheritdoc /> 
        public override MediaElementState CurrentState { get { return MediaPlayer.CurrentState; } }
        /// <inheritdoc /> 
        public override IValueConverter TimeFormatConverter { get { return MediaPlayer.TimeFormatConverter; } }
        /// <inheritdoc /> 
        public override TimeSpan? SkipBackInterval { get { return MediaPlayer.SkipBackInterval; } }
        /// <inheritdoc /> 
        public override TimeSpan? SkipAheadInterval { get { return MediaPlayer.SkipAheadInterval; } }
        /// <inheritdoc /> 
        public override double SignalStrength { get { return MediaPlayer.SignalStrength; } }
        /// <inheritdoc /> 
        public override MediaQuality MediaQuality { get { return MediaPlayer.MediaQuality; } }
        /// <inheritdoc /> 
        public override ImageSource ThumbnailImageSource { get { return MediaPlayer.ThumbnailImageSource; } }
    }
}
