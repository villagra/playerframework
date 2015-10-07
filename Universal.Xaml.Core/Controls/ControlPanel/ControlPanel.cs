using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.ViewManagement;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A MediaPlayer control panel to allow user control over audio or video.
    /// </summary>
    public partial class ControlPanel : Control
    {
        bool IsTemplateApplied;

        /// <summary>
        /// Instantiates a new instance of the ControlPanel class.
        /// </summary>
        public ControlPanel()
        {
            this.DefaultStyleKey = typeof(ControlPanel);

            Init();
        }

        partial void Init();

        /// <inheritdoc /> 
#if SILVERLIGHT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            UninitializeTemplateChildren();
            base.OnApplyTemplate();
            GetTemplateChildren();
            InitializeTemplateChildren();
            SetDefaultVisualStates();
            IsTemplateApplied = true;
        }

        /// <summary>
        /// Identifies the MediaPlayer dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(IInteractiveViewModel), typeof(ControlPanel), new PropertyMetadata(null, (d, e) => ((ControlPanel)d).OnViewModelChanged(e.OldValue as IInteractiveViewModel, e.NewValue as IInteractiveViewModel)));

        void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            if (oldValue != null)
            {
                UninitializeViewModel(oldValue);
            }
            if (newValue != null)
            {
                if (IsTemplateApplied)
                {
                    InitializeViewModel(newValue);
                }
            }
        }

        /// <summary>
        /// The InteractiveMediaPlayer object used to provide state updates and serve user interaction requests.
        /// This is usually an instance of the MediaPlayer but could be a custom implementation to support unique interaction such as in the case of advertising.
        /// </summary>
        public IInteractiveViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as IInteractiveViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }

        /// <summary>
        /// Identifies the IsThumbnailVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsThumbnailVisibleProperty = DependencyProperty.Register("IsThumbnailVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the Thumbnail is visible or not.
        /// </summary>
        public bool IsThumbnailVisible
        {
            get { return (bool)GetValue(IsThumbnailVisibleProperty); }
            set { SetValue(IsThumbnailVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsGoLiveButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsGoLiveButtonVisibleProperty = DependencyProperty.Register("IsGoLiveButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the GoLiveButton is visible or not.
        /// </summary>
        public bool IsGoLiveButtonVisible
        {
            get { return (bool)GetValue(IsGoLiveButtonVisibleProperty); }
            set { SetValue(IsGoLiveButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsInfoButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInfoButtonVisibleProperty = DependencyProperty.Register("IsInfoButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the InfoButton is visible or not.
        /// </summary>
        public bool IsInfoButtonVisible
        {
            get { return (bool)GetValue(IsInfoButtonVisibleProperty); }
            set { SetValue(IsInfoButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsMoreButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMoreButtonVisibleProperty = DependencyProperty.Register("IsMoreButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the MoreButton is visible or not.
        /// </summary>
        public bool IsMoreButtonVisible
        {
            get { return (bool)GetValue(IsMoreButtonVisibleProperty); }
            set { SetValue(IsMoreButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsAudioSelectionButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsAudioSelectionButtonVisibleProperty = DependencyProperty.Register("IsAudioSelectionButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the AudioSelectionButton is visible or not.
        /// </summary>
        public bool IsAudioSelectionButtonVisible
        {
            get { return (bool)GetValue(IsAudioSelectionButtonVisibleProperty); }
            set { SetValue(IsAudioSelectionButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsCaptionSelectionButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCaptionSelectionButtonVisibleProperty = DependencyProperty.Register("IsCaptionSelectionButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the CaptionSelectionButton is visible or not.
        /// </summary>
        public bool IsCaptionSelectionButtonVisible
        {
            get { return (bool)GetValue(IsCaptionSelectionButtonVisibleProperty); }
            set { SetValue(IsCaptionSelectionButtonVisibleProperty, value); }
        }

#if WINDOWS_UWP
        /// <summary>
        /// Identifies the IsCaptionSelectionButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCastButtonVisibleProperty = DependencyProperty.Register("IsCastButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the CaptionSelectionButton is visible or not.
        /// </summary>
        public bool IsCastButtonVisible
        {
            get { return (bool)GetValue(IsCastButtonVisibleProperty); }
            set { SetValue(IsCastButtonVisibleProperty, value); }
        }
#endif

        /// <summary>
        /// Identifies the IsTimeElapsedButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTimeElapsedButtonVisibleProperty = DependencyProperty.Register("IsTimeElapsedButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets if the TimeElapsedButton is visible or not.
        /// </summary>
        public bool IsTimeElapsedButtonVisible
        {
            get { return (bool)GetValue(IsTimeElapsedButtonVisibleProperty); }
            set { SetValue(IsTimeElapsedButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsDurationButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDurationButtonVisibleProperty = DependencyProperty.Register("IsDurationButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the DurationButton is visible or not.
        /// </summary>
        public bool IsDurationButtonVisible
        {
            get { return (bool)GetValue(IsDurationButtonVisibleProperty); }
            set { SetValue(IsDurationButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsTimeRemainingButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTimeRemainingButtonVisibleProperty = DependencyProperty.Register("IsTimeRemainingButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets if the TimeRemainingButton is visible or not.
        /// </summary>
        public bool IsTimeRemainingButtonVisible
        {
            get { return (bool)GetValue(IsTimeRemainingButtonVisibleProperty); }
            set { SetValue(IsTimeRemainingButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsSkipNextButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSkipNextButtonVisibleProperty = DependencyProperty.Register("IsSkipNextButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the SkipNextButton is visible or not.
        /// </summary>
        public bool IsSkipNextButtonVisible
        {
            get { return (bool)GetValue(IsSkipNextButtonVisibleProperty); }
            set { SetValue(IsSkipNextButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsSkipPreviousButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSkipPreviousButtonVisibleProperty = DependencyProperty.Register("IsSkipPreviousButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the SkipPreviousButton is visible or not.
        /// </summary>
        public bool IsSkipPreviousButtonVisible
        {
            get { return (bool)GetValue(IsSkipPreviousButtonVisibleProperty); }
            set { SetValue(IsSkipPreviousButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsSkipAheadButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSkipAheadButtonVisibleProperty = DependencyProperty.Register("IsSkipAheadButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the SkipAheadButton is visible or not.
        /// </summary>
        public bool IsSkipAheadButtonVisible
        {
            get { return (bool)GetValue(IsSkipAheadButtonVisibleProperty); }
            set { SetValue(IsSkipAheadButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsSkipBackButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSkipBackButtonVisibleProperty = DependencyProperty.Register("IsSkipBackButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the SkipBackButton is visible or not.
        /// </summary>
        public bool IsSkipBackButtonVisible
        {
            get { return (bool)GetValue(IsSkipBackButtonVisibleProperty); }
            set { SetValue(IsSkipBackButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsFastForwardButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFastForwardButtonVisibleProperty = DependencyProperty.Register("IsFastForwardButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the FastForwardButton is visible or not.
        /// </summary>
        public bool IsFastForwardButtonVisible
        {
            get { return (bool)GetValue(IsFastForwardButtonVisibleProperty); }
            set { SetValue(IsFastForwardButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsStopButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsStopButtonVisibleProperty = DependencyProperty.Register("IsStopButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the StopButton is visible or not.
        /// </summary>
        public bool IsStopButtonVisible
        {
            get { return (bool)GetValue(IsStopButtonVisibleProperty); }
            set { SetValue(IsStopButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsRewindButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRewindButtonVisibleProperty = DependencyProperty.Register("IsRewindButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the RewindButton is visible or not.
        /// </summary>
        public bool IsRewindButtonVisible
        {
            get { return (bool)GetValue(IsRewindButtonVisibleProperty); }
            set { SetValue(IsRewindButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsReplayButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReplayButtonVisibleProperty = DependencyProperty.Register("IsReplayButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the ReplayButton is visible or not.
        /// </summary>
        public bool IsReplayButtonVisible
        {
            get { return (bool)GetValue(IsReplayButtonVisibleProperty); }
            set { SetValue(IsReplayButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsResolutionIndicatorVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsResolutionIndicatorVisibleProperty = DependencyProperty.Register("IsResolutionIndicatorVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the ResolutionIndicator is visible or not.
        /// </summary>
        public bool IsResolutionIndicatorVisible
        {
            get { return (bool)GetValue(IsResolutionIndicatorVisibleProperty); }
            set { SetValue(IsResolutionIndicatorVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsSignalStrengthVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSignalStrengthVisibleProperty = DependencyProperty.Register("IsSignalStrengthVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the SignalStrength is visible or not.
        /// </summary>
        public bool IsSignalStrengthVisible
        {
            get { return (bool)GetValue(IsSignalStrengthVisibleProperty); }
            set { SetValue(IsSignalStrengthVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsFullScreenButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFullScreenButtonVisibleProperty = DependencyProperty.Register("IsFullScreenButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the FullScreenButton is visible or not.
        /// </summary>
        public bool IsFullScreenButtonVisible
        {
            get { return (bool)GetValue(IsFullScreenButtonVisibleProperty); }
            set { SetValue(IsFullScreenButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsZoomButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsZoomButtonVisibleProperty = DependencyProperty.Register("IsZoomButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the ZoomButton is visible or not.
        /// </summary>
        public bool IsZoomButtonVisible
        {
            get { return (bool)GetValue(IsZoomButtonVisibleProperty); }
            set { SetValue(IsZoomButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsMuteButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMuteButtonVisibleProperty = DependencyProperty.Register("IsMuteButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the MuteButton is visible or not.
        /// </summary>
        public bool IsMuteButtonVisible
        {
            get { return (bool)GetValue(IsMuteButtonVisibleProperty); }
            set { SetValue(IsMuteButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsSlowMotionButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSlowMotionButtonVisibleProperty = DependencyProperty.Register("IsSlowMotionButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the SlowMotionButton is visible or not.
        /// </summary>
        public bool IsSlowMotionButtonVisible
        {
            get { return (bool)GetValue(IsSlowMotionButtonVisibleProperty); }
            set { SetValue(IsSlowMotionButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsPlayPauseButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPlayPauseButtonVisibleProperty = DependencyProperty.Register("IsPlayPauseButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets if the PlayPauseButton is visible or not.
        /// </summary>
        public bool IsPlayPauseButtonVisible
        {
            get { return (bool)GetValue(IsPlayPauseButtonVisibleProperty); }
            set { SetValue(IsPlayPauseButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsVolumeButtonVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVolumeButtonVisibleProperty = DependencyProperty.Register("IsVolumeButtonVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets if the VolumeButton is visible or not.
        /// </summary>
        public bool IsVolumeButtonVisible
        {
            get { return (bool)GetValue(IsVolumeButtonVisibleProperty); }
            set { SetValue(IsVolumeButtonVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsVolumeSliderVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVolumeSliderVisibleProperty = DependencyProperty.Register("IsVolumeSliderVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets if the VolumeSlider is visible or not.
        /// </summary>
        public bool IsVolumeSliderVisible
        {
            get { return (bool)GetValue(IsVolumeSliderVisibleProperty); }
            set { SetValue(IsVolumeSliderVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the IsTimelineVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTimelineVisibleProperty = DependencyProperty.Register("IsTimelineVisible", typeof(bool), typeof(ControlPanel), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets if the Timeline is visible or not.
        /// </summary>
        public bool IsTimelineVisible
        {
            get { return (bool)GetValue(IsTimelineVisibleProperty); }
            set { SetValue(IsTimelineVisibleProperty, value); }
        }
        
        /// <summary>
        /// Identifies the TransportBarButtonStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty TransportBarButtonStyleProperty = DependencyProperty.Register("TransportBarButtonStyle", typeof(Style), typeof(ControlPanel), null);

        /// <summary>
        /// Gets or sets the style to be used for displaying buttons in the transport bar.
        /// </summary>
        public Style TransportBarButtonStyle
        {
            get { return GetValue(TransportBarButtonStyleProperty) as Style; }
            set { SetValue(TransportBarButtonStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the TransportBarTextButtonStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty TransportBarTextButtonStyleProperty = DependencyProperty.Register("TransportBarTextButtonStyle", typeof(Style), typeof(ControlPanel), null);

        /// <summary>
        /// Gets or sets the style to be used for displaying text buttons in the transport bar.
        /// </summary>
        public Style TransportBarTextButtonStyle
        {
            get { return GetValue(TransportBarTextButtonStyleProperty) as Style; }
            set { SetValue(TransportBarTextButtonStyleProperty, value); }
        }
        
        /// <summary>
        /// Identifies the PrimaryButtonStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty PrimaryButtonStyleProperty = DependencyProperty.Register("PrimaryButtonStyle", typeof(Style), typeof(ControlPanel), null);

        /// <summary>
        /// Gets or sets the style to be used for displaying primary buttons.
        /// </summary>
        public Style PrimaryButtonStyle
        {
            get { return GetValue(PrimaryButtonStyleProperty) as Style; }
            set { SetValue(PrimaryButtonStyleProperty, value); }
        }
    }
}
