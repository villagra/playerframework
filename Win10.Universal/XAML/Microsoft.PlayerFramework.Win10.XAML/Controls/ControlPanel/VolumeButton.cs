using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using System.Windows.Input;

namespace Microsoft.PlayerFramework
{
    internal static class VolumeVisibilityStates
    {
        internal const string Requested = "VolumeRequested";
        internal const string Dismissed = "VolumeDismissed";
        internal const string Hidden = "VolumeHidden";
        internal const string Visible = "VolumeVisible";
    }

    internal static class VolumeGroupNames
    {
        internal const string VolumeVisibilityStates = "VolumeVisibilityStates";
    }

    internal static class VolumeTemplateParts
    {
        public const string VolumeSliderContainer = "VolumeSliderContainer";
        public const string VolumeSlider = "VolumeSlider";
        public const string MuteButton = "MuteButton";
    }

    /// <summary>
    /// Represents a button that will allow the user to both mute and change the volume.
    /// </summary>
    [TemplatePart(Name = VolumeTemplateParts.VolumeSliderContainer, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = VolumeTemplateParts.VolumeSlider, Type = typeof(VolumeSlider))]
    [TemplatePart(Name = VolumeTemplateParts.MuteButton, Type = typeof(ButtonBase))]
    [TemplateVisualState(Name = VolumeVisibilityStates.Requested, GroupName = VolumeGroupNames.VolumeVisibilityStates)]
    [TemplateVisualState(Name = VolumeVisibilityStates.Dismissed, GroupName = VolumeGroupNames.VolumeVisibilityStates)]
    [TemplateVisualState(Name = VolumeVisibilityStates.Hidden, GroupName = VolumeGroupNames.VolumeVisibilityStates)]
    [TemplateVisualState(Name = VolumeVisibilityStates.Visible, GroupName = VolumeGroupNames.VolumeVisibilityStates)]
    public class VolumeButton : Control
    {
        /// <summary>
        /// The  volume slider panel (used for video media).
        /// </summary>
        protected FrameworkElement VolumeSliderContainerElement { get; private set; }
        /// <summary>
        /// The  volume slider control (used for video media).
        /// </summary>
        protected VolumeSlider VolumeSliderElement { get; private set; }
        /// <summary>
        /// The mute and volume slider toggle button (used for video media).
        /// </summary>
        protected ButtonBase MuteButtonElement { get; private set; }

        /// <summary>
        /// Creates a new instance of VolumeButton.
        /// </summary>
        public VolumeButton()
        {
            DefaultStyleKey = typeof(VolumeButton);
        }

        /// <inheritdoc /> 
        protected override void OnApplyTemplate()
        {
            UninitializeTemplateChildren();
            base.OnApplyTemplate();
            GetTemplateChildren();
            InitializeTemplateChildren();
            SetDefaultVisualStates();
        }

        /// <inheritdoc /> 
        protected virtual void GetTemplateChildren()
        {
            VolumeSliderContainerElement = GetTemplateChild(VolumeTemplateParts.VolumeSliderContainer) as FrameworkElement;
            VolumeSliderElement = GetTemplateChild(VolumeTemplateParts.VolumeSlider) as VolumeSlider;
            MuteButtonElement = GetTemplateChild(VolumeTemplateParts.MuteButton) as ButtonBase;
        }

        void SetDefaultVisualStates()
        {
            UpdateVolumeVisualState();
            UpdateVolumeLabel();
        }

        private void UpdateVolumeVisualState()
        {
            if (IsVolumeVisible)
            {
                this.GoToVisualState(VolumeVisibilityStates.Visible);
            }
            else
            {
                this.GoToVisualState(VolumeVisibilityStates.Hidden);
            }
        }

        private void UpdateVolumeLabel()
        {
            if (MuteButtonElement != null)
            {
                var behavior = MediaControls.GetBehavior(MuteButtonElement) as MediaToggleControlBehavior;
                if (behavior != null)
                {
                    if (IsVolumeVisible)
                    {
                        behavior.UnsetLabel = MediaPlayer.GetResourceString("MuteButtonLabel");
                    }
                    else
                    {
                        behavior.UnsetLabel = MediaPlayer.GetResourceString("VolumeMuteButtonLabel");
                    }
                }
            }
        }

        void InitializeTemplateChildren()
        {
            if (MuteButtonElement != null)
            {
                MuteButtonElement.GotFocus += VolumeButtonElement_GotFocus;
                var vmCommand = MuteButtonElement.Command as ViewModelCommand;
                if (vmCommand != null) vmCommand.Executing += vmCommand_Executing;
            }

            if (VolumeSliderContainerElement != null)
            {
                VolumeSliderContainerElement.PointerEntered += VolumeSliderContainerElement_PointerEntered;
                VolumeSliderContainerElement.PointerExited += VolumeSliderContainerElement_PointerExited;
            }

            volumeCollapseTimer.Tick += volumeCollapseTimer_Tick;
        }

        void UninitializeTemplateChildren()
        {
            if (MuteButtonElement != null)
            {
                MuteButtonElement.GotFocus -= VolumeButtonElement_GotFocus;
                var vmCommand = MuteButtonElement.Command as ViewModelCommand;
                if (vmCommand != null) vmCommand.Executing -= vmCommand_Executing;
            }

            if (VolumeSliderContainerElement != null)
            {
                VolumeSliderContainerElement.PointerEntered -= VolumeSliderContainerElement_PointerEntered;
                VolumeSliderContainerElement.PointerExited -= VolumeSliderContainerElement_PointerExited;
            }

            volumeCollapseTimer.Tick -= volumeCollapseTimer_Tick;
        }
        void VolumeSliderContainerElement_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!volumeCollapseTimer.IsEnabled) volumeCollapseTimer.Start();
        }
        void VolumeSliderContainerElement_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (volumeCollapseTimer.IsEnabled) volumeCollapseTimer.Stop();
        }

        void vmCommand_Executing(object sender, CancelEventArgs e)
        {
            if (!IsVolumeVisible)
            {
                e.Cancel = e.Cancel || !ViewModel.IsMuted;
                this.GoToVisualState(VolumeVisibilityStates.Requested);
                IsVolumeVisible = true;
            }
            else
            {
                this.GoToVisualState(VolumeVisibilityStates.Dismissed);
                IsVolumeVisible = false;
            }
        }

        readonly DispatcherTimer volumeCollapseTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(3) };

        bool isVolumeVisible;
        /// <summary>
        /// Gets or sets whether the volume panel is visible.
        /// </summary>
        public bool IsVolumeVisible
        {
            get { return isVolumeVisible; }
            set
            {
                if (isVolumeVisible != value)
                {
                    isVolumeVisible = value;
                    if (isVolumeVisible)
                    {
                        if (!volumeCollapseTimer.IsEnabled) volumeCollapseTimer.Start();
                    }
                    else
                    {
                        if (volumeCollapseTimer.IsEnabled) volumeCollapseTimer.Stop();
                    }
                    UpdateVolumeLabel();
                }
            }
        }
        void volumeCollapseTimer_Tick(object sender, object e)
        {
            if (VolumeSliderElement == null || VolumeSliderElement.InnerFocusState != FocusState.Keyboard)
            {
                DismissVolume();
            }
        }

        /// <summary>
        /// Forces the volume slider popout to hide.
        /// </summary>
        public void DismissVolume()
        {
            if (IsVolumeVisible)
            {
                IsVolumeVisible = false;
                this.GoToVisualState(VolumeVisibilityStates.Dismissed);
            }
        }

        void VolumeButtonElement_GotFocus(object sender, RoutedEventArgs e)
        {
            if (MuteButtonElement.FocusState == FocusState.Keyboard)
            {
                RequestVolume();
            }
        }

        /// <summary>
        /// Forces the volume slider popout to show.
        /// </summary>
        public void RequestVolume()
        {
            if (!IsVolumeVisible)
            {
                this.GoToVisualState(VolumeVisibilityStates.Requested);
                IsVolumeVisible = true;
            }
        }

        /// <summary>
        /// Identifies the ViewModel dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(IInteractiveViewModel), typeof(VolumeButton), null);

        /// <summary>
        /// Gets or sets the IInteractiveViewModel implementation used to provide state updates and serve user interaction requests.
        /// This property is usually bound to MediaPlayer.InteractiveViewModel
        /// </summary>
        public IInteractiveViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as IInteractiveViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }


        /// <summary>
        /// Identifies the MuteButtonStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty MuteButtonStyleProperty = DependencyProperty.Register("MuteButtonStyle", typeof(Style), typeof(VolumeButton), null);

        /// <summary>
        /// Gets or sets the Style used to display the mute button.
        /// </summary>
        public Style MuteButtonStyle
        {
            get { return GetValue(MuteButtonStyleProperty) as Style; }
            set { SetValue(MuteButtonStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the PanelBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelBackgroundProperty = DependencyProperty.Register("PanelBackground", typeof(Brush), typeof(VolumeButton), null);

        /// <summary>
        /// Gets or sets the Background brush on the volume panel.
        /// </summary>
        public Brush PanelBackground
        {
            get { return GetValue(PanelBackgroundProperty) as Brush; }
            set { SetValue(PanelBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the PanelPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty PanelPositionProperty = DependencyProperty.Register("PanelPosition", typeof(Thickness), typeof(VolumeButton), null);

        /// <summary>
        /// Gets or sets the Background position on the volume panel.
        /// </summary>
        public Thickness PanelPosition
        {
            get { return (Thickness)GetValue(PanelPositionProperty); }
            set { SetValue(PanelPositionProperty, value); }
        }
    }
}
