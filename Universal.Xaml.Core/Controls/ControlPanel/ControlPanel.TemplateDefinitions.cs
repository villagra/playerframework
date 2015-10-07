using System;
using System.Linq;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Threading;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Input;
using Windows.UI.ViewManagement;
using Windows.Foundation;
using Windows.Graphics.Display;
#endif

namespace Microsoft.PlayerFramework
{

    internal static class ControlPanelTemplateParts
    {
        public const string BorderElement = "Border";
        public const string LeftItemsPanelElement = "LeftItemsPanel";

        public const string ReplayButtonElement = "ReplayButton";
        public const string SkipPreviousButtonElement = "SkipPreviousButton";
        public const string RewindButtonElement = "RewindButton";
        public const string SkipBackButtonElement = "SkipBackButton";
        public const string PlayPauseButtonElement = "PlayPauseButton";
        public const string StopButtonElement = "StopButton";
        public const string SkipAheadButtonElement = "SkipAheadButton";
        public const string FastForwardButtonElement = "FastForwardButton";
        public const string SlowMotionButtonElement = "SlowMotionButton";
        public const string SkipNextButtonElement = "SkipNextButton";
        public const string TimeElapsedButtonElement = "TimeElapsedButton";

        public const string TimelineElement = "Timeline";

        public const string RightItemsPanelElement = "RightItemsPanel";
        public const string DurationButtonElement = "DurationButton";
        public const string TimeRemainingButtonElement = "TotalTimeRemainingButton";
        public const string CaptionSelectionButtonElement = "CaptionSelectionButton";
        public const string AudioSelectionButtonElement = "AudioSelectionButton";

        public const string VolumeButtonElement = "VolumeButton";
        public const string MuteButtonElement = "MuteButton";
        public const string VolumeSliderElement = "VolumeSlider";
        public const string FullScreenButtonElement = "FullScreenButton";
        public const string GoLiveButtonElement = "GoLiveButton";
        public const string InfoButtonElement = "InfoButton";
        public const string MoreButtonElement = "MoreButton";
#if !WINDOWS80
        public const string ZoomButtonElement = "ZoomButton";
#endif
        public const string SignalStrengthElement = "SignalStrength";
        public const string ResolutionIndicatorElement = "ResolutionIndicator";

#if WINDOWS_UWP
        public const string CastButtonElement = "CastButton";
#endif
    }

    internal static class ControlPanelVisualStates
    {
        internal static class GroupNames
        {
            internal const string LayoutStates = "LayoutStates";
            internal const string MediaStates = "MediaStates";
            internal const string MediaTypeStates = "MediaType";
        }

        internal static class MediaStates
        {
            internal const string Playing = "Playing";
            internal const string Closed = "Closed";
            internal const string Opening = "Opening";
            internal const string Paused = "Paused";
            internal const string Buffering = "Buffering";
        }

        internal static class MediaTypeStates
        {
            internal const string Video = "Video";
            internal const string Audio = "Audio";
        }

        internal static class LayoutStates
        {
            internal const string Compact = "Compact";
            internal const string Normal = "FullSize";
        }

        internal static class OrientationStates
        {
            internal const string Landscape = "Landscape";
            internal const string Portrait = "Portrait";
        }
    }

    [TemplatePart(Name = ControlPanelTemplateParts.BorderElement, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = ControlPanelTemplateParts.LeftItemsPanelElement, Type = typeof(Panel))]
    [TemplatePart(Name = ControlPanelTemplateParts.ReplayButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.SkipPreviousButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.RewindButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.SkipBackButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.PlayPauseButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.StopButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.SkipAheadButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.FastForwardButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.SlowMotionButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.SkipNextButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.TimeElapsedButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.TimelineElement, Type = typeof(Timeline))]
    [TemplatePart(Name = ControlPanelTemplateParts.RightItemsPanelElement, Type = typeof(Panel))]
    [TemplatePart(Name = ControlPanelTemplateParts.TimeRemainingButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.DurationButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.CaptionSelectionButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.AudioSelectionButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.VolumeButtonElement, Type = typeof(VolumeButton))]
    [TemplatePart(Name = ControlPanelTemplateParts.MuteButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.VolumeSliderElement, Type = typeof(Slider))]
    [TemplatePart(Name = ControlPanelTemplateParts.FullScreenButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.GoLiveButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.InfoButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.MoreButtonElement, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ControlPanelTemplateParts.SignalStrengthElement, Type = typeof(SignalStrength))]
    [TemplatePart(Name = ControlPanelTemplateParts.ResolutionIndicatorElement, Type = typeof(ResolutionIndicator))]
#if !WINDOWS80
    [TemplatePart(Name = ControlPanelTemplateParts.ZoomButtonElement, Type = typeof(ButtonBase))]
#endif

#if WINDOWS_UWP
    [TemplatePart(Name = ControlPanelTemplateParts.CastButtonElement, Type = typeof(ButtonBase))]
#endif
    [TemplateVisualState(Name = ControlPanelVisualStates.LayoutStates.Normal, GroupName = ControlPanelVisualStates.GroupNames.LayoutStates)]
    [TemplateVisualState(Name = ControlPanelVisualStates.LayoutStates.Compact, GroupName = ControlPanelVisualStates.GroupNames.LayoutStates)]
    [TemplateVisualState(Name = ControlPanelVisualStates.MediaStates.Buffering, GroupName = ControlPanelVisualStates.GroupNames.MediaStates)]
    [TemplateVisualState(Name = ControlPanelVisualStates.MediaStates.Closed, GroupName = ControlPanelVisualStates.GroupNames.MediaStates)]
    [TemplateVisualState(Name = ControlPanelVisualStates.MediaStates.Opening, GroupName = ControlPanelVisualStates.GroupNames.MediaStates)]
    [TemplateVisualState(Name = ControlPanelVisualStates.MediaStates.Paused, GroupName = ControlPanelVisualStates.GroupNames.MediaStates)]
    [TemplateVisualState(Name = ControlPanelVisualStates.MediaStates.Playing, GroupName = ControlPanelVisualStates.GroupNames.MediaStates)]
    [TemplateVisualState(Name = ControlPanelVisualStates.MediaTypeStates.Audio, GroupName = ControlPanelVisualStates.GroupNames.MediaTypeStates)]
    [TemplateVisualState(Name = ControlPanelVisualStates.MediaTypeStates.Video, GroupName = ControlPanelVisualStates.GroupNames.MediaTypeStates)]
    public partial class ControlPanel
    {
        ControlPanelOrientation orientation = ControlPanelOrientation.Landscape;
        bool isCompact = false;

        #region Initialization

        /// <summary>
        /// The left panel where all the controls before the timeline are placed.
        /// </summary>
        public Panel LeftItemsPanel { get; private set; }
        /// <summary>
        /// The panel that contains all controls to the right of the timeline.
        /// </summary>
        public Panel RightItemsPanel { get; private set; }
        /// <summary>
        /// The main panel element containing the controls.
        /// </summary>
        protected FrameworkElement BorderElement { get; private set; }
        /// <summary>
        /// The replay / instant replay button.
        /// </summary>
        protected ButtonBase ReplayButtonElement { get; private set; }
        /// <summary>
        /// The skip previous button (sends the user to the previous marker).
        /// </summary>
        protected ButtonBase SkipPreviousButtonElement { get; private set; }
        /// <summary>
        /// The skip back button (sends the user x seconds back in the timeline).
        /// </summary>
        protected ButtonBase SkipBackButtonElement { get; private set; }
        /// <summary>
        /// The rewind/reverse button.
        /// </summary>
        protected ButtonBase RewindButtonElement { get; private set; }
        /// <summary>
        /// The play and pause toggle button.
        /// </summary>
        protected ButtonBase PlayPauseButtonElement { get; private set; }
        /// <summary>
        /// The stop button.
        /// </summary>
        protected ButtonBase StopButtonElement { get; private set; }
        /// <summary>
        /// The fast forward button.
        /// </summary>
        protected ButtonBase FastForwardButtonElement { get; private set; }
        /// <summary>
        /// The slow motion button.
        /// </summary>
        protected ButtonBase SlowMotionButtonElement { get; private set; }
        /// <summary>
        /// The skip next button. Sends the user to the next marker.
        /// </summary>
        protected ButtonBase SkipNextButtonElement { get; private set; }
        /// <summary>
        /// The skip ahead button (sends the user x seconds forward in the timeline).
        /// </summary>
        protected ButtonBase SkipAheadButtonElement { get; private set; }
        /// <summary>
        /// The current position of the media. Also sends the user back 30 seconds in the timeline.
        /// </summary>
        protected ButtonBase TimeElapsedButtonElement { get; private set; }
        /// <summary>
        /// The panel that contains the timeline.
        /// </summary>
        protected Timeline TimelineElement { get; private set; }
        /// <summary>
        /// The total duration of the media. Also Sends user 30 seconds ahead in the timeline.
        /// </summary>
        protected ButtonBase DurationButtonElement { get; private set; }
        /// <summary>
        /// The time remaining in the media. Also Sends user 30 seconds ahead in the timeline.
        /// </summary>
        protected ButtonBase TimeRemainingButtonElement { get; private set; }
        /// <summary>
        /// The Captions button.
        /// </summary>
        protected ButtonBase CaptionSelectionButtonElement { get; private set; }
        /// <summary>
        /// Allow you to select an audio track.
        /// </summary>
        protected ButtonBase AudioSelectionButtonElement { get; private set; }
        /// <summary>
        /// The mute and volume slider toggle button (used for video media).
        /// </summary>
        protected VolumeButton VolumeButtonElement { get; private set; }
        /// <summary>
        /// The mute toggle button (used for audio media).
        /// </summary>
        protected ButtonBase MuteButtonElement { get; private set; }
        /// <summary>
        /// The horizontal volume slider (used for audio media where you can't extend vertically).
        /// </summary>
        protected VolumeSlider VolumeSliderElement { get; private set; }
        /// <summary>
        /// The fullscreen toggle button.
        /// </summary>
        protected ButtonBase FullScreenButtonElement { get; private set; }
        /// <summary>
        /// The go live button.
        /// </summary>
        protected ButtonBase GoLiveButtonElement { get; private set; }
        /// <summary>
        /// The info button.
        /// </summary>
        protected ButtonBase InfoButtonElement { get; private set; }
        /// <summary>
        /// The more button.
        /// </summary>
        protected ButtonBase MoreButtonElement { get; private set; }
#if !WINDOWS80
        /// <summary>
        /// The zoom button.
        /// </summary>
        protected ButtonBase ZoomButtonElement { get; private set; }
#endif
        /// <summary>
        /// The signal strength indicator (usually adaptive only).
        /// </summary>
        protected SignalStrength SignalStrengthElement { get; private set; }
        /// <summary>
        /// The signal strength indicator (usually adaptive only).
        /// </summary>
        protected ResolutionIndicator ResolutionIndicatorElement { get; private set; }

#if WINDOWS_UWP
        /// <summary>
        /// The cast button.
        /// </summary>
        internal ButtonBase CastButtonElement { get; private set; }
#endif

        /// <inheritdoc /> 
        protected virtual void GetTemplateChildren()
        {
            BorderElement = GetTemplateChild(ControlPanelTemplateParts.BorderElement) as FrameworkElement;
            LeftItemsPanel = GetTemplateChild(ControlPanelTemplateParts.LeftItemsPanelElement) as Panel;
            RightItemsPanel = GetTemplateChild(ControlPanelTemplateParts.RightItemsPanelElement) as Panel;

            ReplayButtonElement = GetTemplateChild(ControlPanelTemplateParts.ReplayButtonElement) as ButtonBase;
            SkipPreviousButtonElement = GetTemplateChild(ControlPanelTemplateParts.SkipPreviousButtonElement) as ButtonBase;
            RewindButtonElement = GetTemplateChild(ControlPanelTemplateParts.RewindButtonElement) as ButtonBase;
            SkipBackButtonElement = GetTemplateChild(ControlPanelTemplateParts.SkipBackButtonElement) as ButtonBase;
            PlayPauseButtonElement = GetTemplateChild(ControlPanelTemplateParts.PlayPauseButtonElement) as ButtonBase;
            StopButtonElement = GetTemplateChild(ControlPanelTemplateParts.StopButtonElement) as ButtonBase;
            SkipAheadButtonElement = GetTemplateChild(ControlPanelTemplateParts.SkipAheadButtonElement) as ButtonBase;
            FastForwardButtonElement = GetTemplateChild(ControlPanelTemplateParts.FastForwardButtonElement) as ButtonBase;
            SlowMotionButtonElement = GetTemplateChild(ControlPanelTemplateParts.SlowMotionButtonElement) as ButtonBase;
            SkipNextButtonElement = GetTemplateChild(ControlPanelTemplateParts.SkipNextButtonElement) as ButtonBase;
            TimeElapsedButtonElement = GetTemplateChild(ControlPanelTemplateParts.TimeElapsedButtonElement) as ButtonBase;

            TimelineElement = GetTemplateChild(ControlPanelTemplateParts.TimelineElement) as Timeline;

            DurationButtonElement = GetTemplateChild(ControlPanelTemplateParts.DurationButtonElement) as ButtonBase;
            TimeRemainingButtonElement = GetTemplateChild(ControlPanelTemplateParts.TimeRemainingButtonElement) as ButtonBase;
            CaptionSelectionButtonElement = GetTemplateChild(ControlPanelTemplateParts.CaptionSelectionButtonElement) as ButtonBase;
            AudioSelectionButtonElement = GetTemplateChild(ControlPanelTemplateParts.AudioSelectionButtonElement) as ButtonBase;

            VolumeButtonElement = GetTemplateChild(ControlPanelTemplateParts.VolumeButtonElement) as VolumeButton;
            MuteButtonElement = GetTemplateChild(ControlPanelTemplateParts.MuteButtonElement) as ButtonBase;
            VolumeSliderElement = GetTemplateChild(ControlPanelTemplateParts.VolumeSliderElement) as VolumeSlider;

            FullScreenButtonElement = GetTemplateChild(ControlPanelTemplateParts.FullScreenButtonElement) as ButtonBase;
            GoLiveButtonElement = GetTemplateChild(ControlPanelTemplateParts.GoLiveButtonElement) as ButtonBase;
            InfoButtonElement = GetTemplateChild(ControlPanelTemplateParts.InfoButtonElement) as ButtonBase;
            MoreButtonElement = GetTemplateChild(ControlPanelTemplateParts.MoreButtonElement) as ButtonBase;
#if !WINDOWS80
            ZoomButtonElement = GetTemplateChild(ControlPanelTemplateParts.MoreButtonElement) as ButtonBase;
#endif
            SignalStrengthElement = GetTemplateChild(ControlPanelTemplateParts.SignalStrengthElement) as SignalStrength;
            ResolutionIndicatorElement = GetTemplateChild(ControlPanelTemplateParts.ResolutionIndicatorElement) as ResolutionIndicator;

#if WINDOWS_UWP
            CastButtonElement = GetTemplateChild(ControlPanelTemplateParts.CastButtonElement) as ButtonBase;
#endif
        }

        partial void Init()
        {
#if !SILVERLIGHT && !WINDOWS80
            CompactThresholdInInches = 5.0;
#endif

            this.SizeChanged += ControlPanel_SizeChanged;
        }

        void SetDefaultVisualStates()
        {
            UpdateCurrentStateVisualState();
            UpdateOrientationVisualState();
            UpdateLayoutVisualState();
        }

        void ControlPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Orientation = (e.NewSize.Height > e.NewSize.Width) ? ControlPanelOrientation.Portrait : ControlPanelOrientation.Landscape;

#if !SILVERLIGHT
            if (!IsInDesignMode)
            {
#if !WINDOWS80
                var physicalSize = GetPhysicalSize(e.NewSize);
                IsCompact = (physicalSize.Width <= CompactThresholdInInches);
#else
                IsCompact = ApplicationView.Value == ApplicationViewState.Snapped;
#endif
            }
            else
#endif
            {
                IsCompact = e.NewSize.Width <= 853.0;
            }
        }

#if SILVERLIGHT
        bool IsInDesignMode
#else
        static bool IsInDesignMode
#endif
        {
            get
            {
#if SILVERLIGHT
                return DesignerProperties.GetIsInDesignMode(this);
#else
                return Windows.ApplicationModel.DesignMode.DesignModeEnabled;
#endif
            }
        }

#if !SILVERLIGHT && !WINDOWS80
        /// <summary>
        /// Gets or sets the minimum size (in inches) that the control can be resized to before going into "Compact" mode. When this happens, the Compact Visual State is triggered.
        /// </summary>
        public double CompactThresholdInInches { get; set; }
#endif

        /// <summary>
        /// Gets whether or not the control is in "compact" mode. Compact mode is determined when the physical size of the control is less than the value of CompactThresholdInInches.
        /// </summary>
        public bool IsCompact
        {
            get { return isCompact; }
            private set
            {
                if (isCompact != value)
                {
                    isCompact = value;
                    UpdateLayoutVisualState();
                }
            }
        }

        private void UpdateLayoutVisualState()
        {
            VisualStateManager.GoToState(this, isCompact ? ControlPanelVisualStates.LayoutStates.Compact : ControlPanelVisualStates.LayoutStates.Normal, true);
        }

        /// <summary>
        /// Gets the current orientation of the control based on the height and width. Note: this may be different from the orientation of the app or device if the control does not consume the full screen.
        /// </summary>
        public ControlPanelOrientation Orientation
        {
            get { return orientation; }
            private set
            {
                if (orientation != value)
                {
                    orientation = value;
                    UpdateOrientationVisualState();
                }
            }
        }

        private void UpdateOrientationVisualState()
        {
            VisualStateManager.GoToState(this, orientation == ControlPanelOrientation.Portrait ? ControlPanelVisualStates.OrientationStates.Portrait : ControlPanelVisualStates.OrientationStates.Landscape, true);
        }

        private void UpdateCompactVisualState()
        {
            if (IsCompact)
            {
                this.GoToVisualState(ControlPanelVisualStates.LayoutStates.Compact);
            }
            else
            {
                this.GoToVisualState(ControlPanelVisualStates.LayoutStates.Normal);
            }
        }

        void InitializeTemplateChildren()
        {
            if (ViewModel != null)
            {
                InitializeViewModel(ViewModel);
            }
        }

        void UninitializeTemplateChildren()
        {
            if (ViewModel != null)
            {
                UninitializeViewModel(ViewModel);
            }
        }

        void InitializeViewModel(IInteractiveViewModel vm)
        {
            vm.CurrentStateChanged += ViewModel_CurrentStateChanged;
        }

        void UninitializeViewModel(IInteractiveViewModel vm)
        {
            vm.CurrentStateChanged -= ViewModel_CurrentStateChanged;
        }

        #endregion

        #region MediaPlayer Events

        void ViewModel_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            UpdateCurrentStateVisualState();
        }

        private void UpdateCurrentStateVisualState()
        {
            if (ViewModel == null)
            {
                this.GoToVisualState(ControlPanelVisualStates.MediaStates.Closed);
            }
            else
            {
                switch (ViewModel.CurrentState)
                {
#if SILVERLIGHT
                    case MediaElementState.AcquiringLicense:
                    case MediaElementState.Individualizing:
#endif
                    case MediaElementState.Opening:
                        this.GoToVisualState(ControlPanelVisualStates.MediaStates.Opening);
                        break;
                    case MediaElementState.Buffering:
                        this.GoToVisualState(ControlPanelVisualStates.MediaStates.Buffering);
                        break;
                    case MediaElementState.Playing:
                        this.GoToVisualState(ControlPanelVisualStates.MediaStates.Playing);
                        break;
                    case MediaElementState.Closed:
                        this.GoToVisualState(ControlPanelVisualStates.MediaStates.Closed);
                        break;
                    case MediaElementState.Paused:
                    case MediaElementState.Stopped:
                        this.GoToVisualState(ControlPanelVisualStates.MediaStates.Paused);
                        break;
                }
            }
        }

        #endregion

        #region Helpers

#if NETFX_CORE && !WINDOWS80
        static Size GetPhysicalSize(Size size)
        {
            var displayInfo = Windows.Graphics.Display.DisplayInformation.GetForCurrentView();

#if WINDOWS_PHONE_APP
            var scale = displayInfo.RawPixelsPerViewPixel;
#else
            var scale = (double)(int)displayInfo.ResolutionScale / 100;
#endif

            var w = size.Width * scale / displayInfo.RawDpiX;
            var h = size.Height * scale / displayInfo.RawDpiY;
            return new Size(w, h);
        }
#endif
        #endregion
    }

    /// <summary>
    /// Enumerable to indicate whether the control is in portrait or landscape mode.
    /// </summary>
    public enum ControlPanelOrientation
    {
        /// <summary>
        /// The width is greater than or equal to the height.
        /// </summary>
        Landscape,
        /// <summary>
        /// The height is greater than the width.
        /// </summary>
        Portrait
    }
}
