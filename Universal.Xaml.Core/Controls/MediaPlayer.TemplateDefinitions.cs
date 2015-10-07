using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media;
using System.IO;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.Media.PlayTo;
using Windows.Media.Protection;
using Windows.Foundation.Collections;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Input;
#endif

namespace Microsoft.PlayerFramework
{
    [TemplatePart(Name = MediaPlayerTemplateParts.MediaElement, Type = typeof(MediaElement))]
    [TemplatePart(Name = MediaPlayerTemplateParts.MediaContainer, Type = typeof(Panel))]
    [TemplatePart(Name = MediaPlayerTemplateParts.LayoutRootElement, Type = typeof(Panel))]
    [TemplatePart(Name = MediaPlayerTemplateParts.CaptionsContainer, Type = typeof(Panel))]
    [TemplatePart(Name = MediaPlayerTemplateParts.AdvertisingContainer, Type = typeof(Panel))]
    [TemplatePart(Name = MediaPlayerTemplateParts.BufferingContainer, Type = typeof(Panel))]
    [TemplatePart(Name = MediaPlayerTemplateParts.ErrorsContainer, Type = typeof(Panel))]
    [TemplatePart(Name = MediaPlayerTemplateParts.InteractivityContainer, Type = typeof(Panel))]
    [TemplatePart(Name = MediaPlayerTemplateParts.LoaderViewContainer, Type = typeof(Panel))]
    [TemplatePart(Name = MediaPlayerTemplateParts.ControlPanel, Type = typeof(Control))]
#if SILVERLIGHT
    [TemplatePart(Name = MediaPlayerTemplateParts.PosterContainer, Type = typeof(Panel))]
#else
    [TemplateVisualState(Name = MediaPlayerVisualStates.PlayToStates.Connected, GroupName = MediaPlayerVisualStates.GroupNames.PlayToStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.PlayToStates.Disconnected, GroupName = MediaPlayerVisualStates.GroupNames.PlayToStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.PlayToStates.Rendering, GroupName = MediaPlayerVisualStates.GroupNames.PlayToStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.MediaTypeStates.AudioOnly, GroupName = MediaPlayerVisualStates.GroupNames.MediaTypeStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.MediaTypeStates.AudioVideo, GroupName = MediaPlayerVisualStates.GroupNames.MediaTypeStates)]
#endif
    [TemplateVisualState(Name = MediaPlayerVisualStates.InteractiveStates.Hidden, GroupName = MediaPlayerVisualStates.GroupNames.InteractiveStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.InteractiveStates.StartInteracting, GroupName = MediaPlayerVisualStates.GroupNames.InteractiveStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.InteractiveStates.StopInteracting, GroupName = MediaPlayerVisualStates.GroupNames.InteractiveStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.InteractiveStates.Visible, GroupName = MediaPlayerVisualStates.GroupNames.InteractiveStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.MediaStates.Buffering, GroupName = MediaPlayerVisualStates.GroupNames.MediaStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.MediaStates.Closed, GroupName = MediaPlayerVisualStates.GroupNames.MediaStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.MediaStates.Opening, GroupName = MediaPlayerVisualStates.GroupNames.MediaStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.MediaStates.Paused, GroupName = MediaPlayerVisualStates.GroupNames.MediaStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.MediaStates.Playing, GroupName = MediaPlayerVisualStates.GroupNames.MediaStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.PlayerStates.Unloaded, GroupName = MediaPlayerVisualStates.GroupNames.PlayerStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.PlayerStates.Pending, GroupName = MediaPlayerVisualStates.GroupNames.PlayerStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.PlayerStates.Loading, GroupName = MediaPlayerVisualStates.GroupNames.PlayerStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.PlayerStates.Loaded, GroupName = MediaPlayerVisualStates.GroupNames.PlayerStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.PlayerStates.Opened, GroupName = MediaPlayerVisualStates.GroupNames.PlayerStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.PlayerStates.Starting, GroupName = MediaPlayerVisualStates.GroupNames.PlayerStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.PlayerStates.Started, GroupName = MediaPlayerVisualStates.GroupNames.PlayerStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.PlayerStates.Ending, GroupName = MediaPlayerVisualStates.GroupNames.PlayerStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.PlayerStates.Failed, GroupName = MediaPlayerVisualStates.GroupNames.PlayerStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.CaptionsStates.CapitonsActive, GroupName = MediaPlayerVisualStates.GroupNames.CaptionsStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.CaptionsStates.CapitonsInactive, GroupName = MediaPlayerVisualStates.GroupNames.CaptionsStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.FullScreenStates.FullScreen, GroupName = MediaPlayerVisualStates.GroupNames.FullScreenStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.FullScreenStates.NotFullScreen, GroupName = MediaPlayerVisualStates.GroupNames.FullScreenStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.AdvertisingStates.NoAd, GroupName = MediaPlayerVisualStates.GroupNames.AdvertisingStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.AdvertisingStates.LoadingAd, GroupName = MediaPlayerVisualStates.GroupNames.AdvertisingStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.AdvertisingStates.LinearAd, GroupName = MediaPlayerVisualStates.GroupNames.AdvertisingStates)]
    [TemplateVisualState(Name = MediaPlayerVisualStates.AdvertisingStates.NonLinearAd, GroupName = MediaPlayerVisualStates.GroupNames.AdvertisingStates)]
    public partial class MediaPlayer
    {
        /// <summary>
        /// The collection of interactive elements being tracked
        /// </summary>
        readonly List<InteractiveElement> interactiveElements = new List<InteractiveElement>();

        /// <summary>
        /// The timer userd to help trigger auto hide.
        /// </summary>
        DispatcherTimer autoHideTimer;

        /// <summary>
        /// A collection to store the markers before the real Markers collection is created on the MediaElement
        /// </summary>
        TimelineMarkerCollection preTemplateAppliedMarkers = null;

        /// <summary>
        /// Gets the state at the time the last media failure occured.
        /// </summary>
        protected MediaState FailedMediaState { get; private set; }

        /// <summary>
        /// Identifies the ControlPanelTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ControlPanelTemplateProperty = DependencyProperty.Register("ControlPanelTemplate", typeof(ControlTemplate), typeof(MediaPlayer), null);

        /// <summary>
        /// Gets or sets the ControlTemplate to use for the control panel control.
        /// </summary>
        public ControlTemplate ControlPanelTemplate
        {
            get { return GetValue(ControlPanelTemplateProperty) as ControlTemplate; }
            set { SetValue(ControlPanelTemplateProperty, value); }
        }

        #region Template Children

        /// <summary>
        /// Gets the control panel element.
        /// HACK: Type is Control instead of ControlPanel because of Win8 bug.
        /// </summary>
        public Control ControlPanel { get; private set; }


        /// <summary>
        /// The main panel of the player that contains all interactive elements.
        /// </summary>
        protected Panel MediaContainer { get; private set; }

        /// <summary>
        /// The main panel of the player that contains all interactive elements.
        /// </summary>
        protected Panel InteractivityContainer { get; private set; }

        /// <summary>
        /// The main panel of the player that contains all child elements.
        /// </summary>
        protected Panel LayoutRootElement { get; private set; }

#if SILVERLIGHT
        private IMediaElement mediaElement;
#else
        private MediaElement mediaElement;
#endif
        /// <summary>
        /// Gets the underlying MediaElement
        /// </summary>
#if SILVERLIGHT
        protected IMediaElement MediaElementElement
#else
        protected MediaElement MediaElementElement
#endif
        {
            get { return mediaElement; }
            private set
            {
                if (mediaElement != value)
                {
                    if (mediaElement != null)
                    {
                        UninitializeMediaElement();
                    }

                    mediaElement = value;

                    if (mediaElement != null)
                    {
                        InitializeMediaElement();
                    }
                }
            }
        }

        #endregion

        #region Initialization

        partial void InitializeTemplateDefinitions()
        {
            Containers = new List<UIElement>();
            InteractiveViewModel = DefaultInteractiveViewModel = new InteractiveViewModel(this);
            // dump all markers created before the template was applied into the MediaElement.
            RegisterApplyTemplateAction(() =>
            {
                if (preTemplateAppliedMarkers != null)
                {
                    var markersToAdd = preTemplateAppliedMarkers.ToList();
                    preTemplateAppliedMarkers.Clear();
                    preTemplateAppliedMarkers = null;
                    if (mediaElement != null)
                    {
                        foreach (var marker in markersToAdd)
                        {
                            mediaElement.Markers.Add(marker);
                        }
                    }
                }
            });
        }

        partial void UninitializeTemplateDefinitions()
        {
            Containers = null;
            InteractiveViewModel = DefaultInteractiveViewModel = null;
        }

        void DestroyTemplateChildren()
        {
            Containers.Clear();

            MediaElementElement = null;
            LayoutRootElement = null;
            InteractivityContainer = null;
            MediaContainer = null;
            ControlPanel = null;
        }

        void GetTemplateChildren()
        {
            LayoutRootElement = GetTemplateChild(MediaPlayerTemplateParts.LayoutRootElement) as Panel;
            InteractivityContainer = GetTemplateChild(MediaPlayerTemplateParts.InteractivityContainer) as Panel;
            MediaContainer = GetTemplateChild(MediaPlayerTemplateParts.MediaContainer) as Panel;
            ControlPanel = GetTemplateChild(MediaPlayerTemplateParts.ControlPanel) as Control;

#if SILVERLIGHT
            //MediaElementElement = GetTemplateChild(MediaPlayerTemplateParts.MediaElement) as IMediaElement;
            var mediaPlugin = Plugins.OfType<IMediaPlugin>().FirstOrDefault();
            UIElement mediaElementElement;
            if (mediaPlugin == null)
            {
                mediaElementElement = new MediaElementWrapper();
            }
            else
            {
                mediaElementElement = mediaPlugin.MediaElement as FrameworkElement;
            }
            MediaContainer.Children.Insert(0, mediaElementElement);
            MediaElementElement = mediaElementElement as IMediaElement;
#else
            MediaElementElement = GetTemplateChild(MediaPlayerTemplateParts.MediaElement) as MediaElement;
#endif

            if (LayoutRootElement != null)
            {
                foreach (var containers in LayoutRootElement.Children)
                {
                    Containers.Add(containers);
                }
            }
        }

        void InitializeTemplateChildren()
        {
#if SILVERLIGHT && !WINDOWS_PHONE
            IsFullScreen = Application.Current.Host.Content.IsFullScreen;
            Application.Current.Host.Content.FullScreenChanged += Content_FullScreenChanged;
#endif
            autoHideTimer = new DispatcherTimer();
            autoHideTimer.Tick += autoHideTimer_Tick;

            if (MediaContainer != null)
            {
#if SILVERLIGHT
                MediaContainer.MouseMove += MediaContainer_MouseMove;
                MediaContainer.MouseLeftButtonDown += MediaContainer_MouseLeftButtonDown;
#else
                MediaContainer.PointerMoved += MediaContainer_PointerMoved;
                MediaContainer.PointerPressed += MediaContainer_PointerPressed;
#endif
            }

            if (ControlPanel != null)
            {
                AddInteractiveElement(ControlPanel, true, true);
            }
        }

        /// <inheritdoc /> 
#if SILVERLIGHT
        protected override void OnKeyDown(KeyEventArgs e)
        {
#else
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Escape)
            {
#if WINDOWS_UWP
                mediaElement.IsFullWindow = false;
#else                
                IsFullScreen = false;
#endif
            }
#endif
            base.OnKeyDown(e);
            OnUserInteraction(InteractionType.Hard, true);
        }

        void UninitializeTemplateChildren()
        {
            if (autoHideTimer != null)
            {
                autoHideTimer.Tick -= autoHideTimer_Tick;
                StopAutoHideTimer();
                autoHideTimer = null;
            }

            if (MediaContainer != null)
            {
#if SILVERLIGHT
                MediaContainer.MouseMove -= MediaContainer_MouseMove;
                MediaContainer.MouseLeftButtonDown -= MediaContainer_MouseLeftButtonDown;
#else
                MediaContainer.PointerMoved -= MediaContainer_PointerMoved;
                MediaContainer.PointerPressed -= MediaContainer_PointerPressed;
#endif
            }

            if (ControlPanel != null)
            {
                RemoveInteractiveElement(ControlPanel);
            }

#if SILVERLIGHT && !WINDOWS_PHONE
            Application.Current.Host.Content.FullScreenChanged -= Content_FullScreenChanged;
#endif
            InteractiveViewModel = null;
        }

        void SetDefaultVisualStates()
        {
            this.GoToVisualState(MediaPlayerVisualStates.MediaStates.Closed);
            this.GoToVisualState(MediaPlayerVisualStates.PlayerStates.Unloaded);
            this.GoToVisualState(MediaPlayerVisualStates.AdvertisingStates.NoAd);
            if (IsFullScreen)
            {
                this.GoToVisualState(MediaPlayerVisualStates.FullScreenStates.FullScreen);
            }
            else
            {
                this.GoToVisualState(MediaPlayerVisualStates.FullScreenStates.NotFullScreen);
            }
            if (IsCaptionsActive)
            {
                this.GoToVisualState(MediaPlayerVisualStates.CaptionsStates.CapitonsActive);
            }
            else
            {
                this.GoToVisualState(MediaPlayerVisualStates.CaptionsStates.CapitonsInactive);
            }
            if (IsInteractive)
            {
                this.GoToVisualState(MediaPlayerVisualStates.InteractiveStates.Visible);
            }
            else
            {
                this.GoToVisualState(MediaPlayerVisualStates.InteractiveStates.Hidden);
            }
#if !SILVERLIGHT
            this.GoToVisualState(MediaPlayerVisualStates.MediaTypeStates.AudioVideo);
            this.GoToVisualState(MediaPlayerVisualStates.PlayToStates.Disconnected);
#endif
        }

        private void InitializeMediaElement()
        {
#if POSITIONBINDING
            BindingOperations.SetBinding(this, MediaPlayer.InternalPositionProperty, new Binding() { Path = new PropertyPath("Position"), Source = mediaElement, Mode = BindingMode.OneWay });
#endif
#if !WINDOWS_PHONE
            mediaElement.RateChanged += MediaElement_RateChanged;
#endif
            mediaElement.MarkerReached += MediaElement_MarkerReached;
            mediaElement.BufferingProgressChanged += MediaElement_BufferingProgressChanged;
            mediaElement.DownloadProgressChanged += MediaElement_DownloadProgressChanged;
            mediaElement.CurrentStateChanged += MediaElement_CurrentStateChanged;
            mediaElement.MediaOpened += MediaElement_MediaOpened;
            mediaElement.MediaEnded += MediaElement_MediaEnded;
            mediaElement.MediaFailed += MediaElement_MediaFailed;
#if SILVERLIGHT
            mediaElement.LogReady += MediaElement_LogReady;
#else
            mediaElement.VolumeChanged += MediaElement_VolumeChanged;
            mediaElement.SeekCompleted += MediaElement_SeekCompleted;
#endif

            if (IsInDesignMode)
            {
                mediaElement.AutoPlay = false;
                IsInteractive = true;
                AutoHide = false;
            }

#if WINDOWS_UWP
            mediaElement.RegisterPropertyChangedCallback(MediaElement.IsFullWindowProperty, new DependencyPropertyChangedCallback(IsFullWindowChanged));
#endif
        }


#if WINDOWS_UWP
        bool _FullWindowTransportControlsEnabled, _AreTransportControlsEnabledFlag;
        private void IsFullWindowChanged(DependencyObject obj, DependencyProperty prop)
        {
            if (mediaElement.IsFullWindow)
            {
                if (!_AreTransportControlsEnabledFlag)
                {
                    _FullWindowTransportControlsEnabled = AreTransportControlsEnabled;
                    _AreTransportControlsEnabledFlag = true;
                }

                mediaElement.AreTransportControlsEnabled = true;
            }
            else
            {
                IsFullScreen = false;
                mediaElement.AreTransportControlsEnabled = _FullWindowTransportControlsEnabled;
            }

        }
#endif

        private void UninitializeMediaElement()
        {
#if POSITIONBINDING
            BindingOperations.SetBinding(this, MediaPlayer.InternalPositionProperty, new Binding());
#endif
#if !WINDOWS_PHONE
            mediaElement.RateChanged -= MediaElement_RateChanged;
#endif
            mediaElement.MarkerReached -= MediaElement_MarkerReached;
            mediaElement.BufferingProgressChanged -= MediaElement_BufferingProgressChanged;
            mediaElement.DownloadProgressChanged -= MediaElement_DownloadProgressChanged;
            mediaElement.CurrentStateChanged -= MediaElement_CurrentStateChanged;
            mediaElement.MediaOpened -= MediaElement_MediaOpened;
            mediaElement.MediaEnded -= MediaElement_MediaEnded;
            mediaElement.MediaFailed -= MediaElement_MediaFailed;
#if SILVERLIGHT
            mediaElement.LogReady -= MediaElement_LogReady;
#else
            mediaElement.VolumeChanged -= MediaElement_VolumeChanged;
            mediaElement.SeekCompleted -= MediaElement_SeekCompleted;
#endif
            OnMediaElementUnititialized();
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

        bool isTemplateApplied;
        bool IsTemplateApplied
        {
            get { return isTemplateApplied; }
            set
            {
                isTemplateApplied = value;
                OnAfterApplyTemplate();
            }
        }

        readonly Queue<Action> OnApplyTemplateActions = new Queue<Action>();

        /// <summary>
        /// Run all the queued actions that should happen after the template is applied
        /// </summary>
#if SILVERLIGHT
        private async void OnAfterApplyTemplate()
#else
        private void OnAfterApplyTemplate()
#endif
        {
#if SILVERLIGHT
            if (mediaElement != null) // The SSME needs to complete template loading before we should start setting properties
            {
                await mediaElement.TemplateAppliedTask;
            }
#endif
            while (OnApplyTemplateActions.Any())
            {
                var action = OnApplyTemplateActions.Dequeue();
                action();
            }
        }

        /// <summary>
        /// Defers the action until after the template is applied. This is useful to help avoid NullRefExceptions from template children not existing yet.
        /// </summary>
        /// <param name="action">The action to run after the template is applied.</param>
        void RegisterApplyTemplateAction(Action action)
        {
            if (action == null) throw new ArgumentNullException("action");

            if (IsTemplateApplied)
            {
                action();
            }
            else
            {
                OnApplyTemplateActions.Enqueue(action);
            }
        }

        #endregion

        #region Helper Methods

        static T? GetDefaultNullableValue<T>(DependencyProperty dp) where T : struct
        {
            var result = dp.GetMetadata(typeof(MediaElement)).DefaultValue;
            if (result is ValueType)
            {
                return (T?)result;
            }
            else
            {
                return new Nullable<T>();
            }
        }

        static T GetDefaultValue<T>(DependencyProperty dp) where T : struct
        {
            return (T)dp.GetMetadata(typeof(MediaElement)).DefaultValue;
        }

        static T GetDefaultRefValue<T>(DependencyProperty dp) where T : class
        {
            return dp.GetMetadata(typeof(MediaElement)).DefaultValue as T;
        }

        #endregion

        #region Properties

        #region AutoHideInterval
        /// <summary>
        /// Identifies the AutoHideInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoHideIntervalProperty = RegisterDependencyProperty<TimeSpan>("AutoHideInterval", TimeSpan.FromSeconds(2));

        /// <summary>
        /// Gets or sets the time before the control will automatically collapse all interactive elements.
        /// This is only used if AutoHide = true. The default is 2 seconds.
        /// </summary>
        [Category(Categories.Advanced)]
        public TimeSpan AutoHideInterval
        {
            get { return (TimeSpan)GetValue(AutoHideIntervalProperty); }
            set { SetValue(AutoHideIntervalProperty, value); }
        }
        #endregion

        #region Containers
        /// <summary>
        /// Gets the colleciton of UIElement in the player.
        /// This is useful for programmatically adding UIElements to the player without having to template the control.
        /// </summary>
        [Category(Categories.Advanced)]
        public IList<UIElement> Containers { get; set; }
        #endregion

        #region InteractiveViewModel
        /// <summary>
        /// Identifies the InteractiveViewModel dependency property.
        /// </summary>
        public static readonly DependencyProperty InteractiveViewModelProperty = RegisterDependencyProperty<IInteractiveViewModel>("InteractiveViewModel", (t, o, n) => t.OnInteractiveViewModelChanged(o, n));

        void OnInteractiveViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            if (oldValue != null)
            {
                oldValue.Interacting -= InteractiveViewModel_Interacting;
            }

            if (newValue != null)
            {
                newValue.Interacting += InteractiveViewModel_Interacting;
            }

            if (InteractiveViewModelChanged != null) InteractiveViewModelChanged(this, new RoutedPropertyChangedEventArgs<IInteractiveViewModel>(oldValue, newValue));
        }

        void InteractiveViewModel_Interacting(object sender, InteractionEventArgs e)
        {
            OnUserInteraction(e.InteractionType, true);
        }

        /// <summary>
        /// Occurs when the InteractiveViewModel property changes.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<IInteractiveViewModel> InteractiveViewModelChanged;

        /// <summary>
        /// Gets or sets the view model used by all interactive elements to control playback or report on the status of playback.
        /// By default this is set to the current object but can be substituted for a custom implementation for supporting features such as advertising.
        /// </summary>
        [Category(Categories.Advanced)]
        public IInteractiveViewModel InteractiveViewModel
        {
            get { return GetValue(InteractiveViewModelProperty) as IInteractiveViewModel; }
            set { SetValue(InteractiveViewModelProperty, value); }
        }

        /// <summary>
        /// Gets or sets the default InteractiveViewModel. This property will be used to reset the viewmodel in cases where a custom viewmodel was supplied and needs to be reverted (e.g. from the advertising plugin)
        /// </summary>
        public IInteractiveViewModel DefaultInteractiveViewModel { get; set; }
        #endregion

        #region InteractiveActivationMode
        /// <summary>
        /// Identifies the InteractiveActivationMode dependency property.
        /// </summary>
        public static readonly DependencyProperty InteractiveActivationModeProperty = RegisterDependencyProperty<InteractionType>("InteractiveActivationMode", DefaultInteractiveActivationMode);

        static InteractionType DefaultInteractiveActivationMode
        {
            get
            {
#if WINDOWS_PHONE
                return InteractionType.Hard;
#else
                return InteractionType.All;
#endif
            }
        }

        /// <summary>
        /// Gets or sets the action that will cause the player to be in interactive mode. (IsInteractive = true)
        /// </summary>
        [Category(Categories.Advanced)]
        public InteractionType InteractiveActivationMode
        {
            get { return (InteractionType)GetValue(InteractiveActivationModeProperty); }
            set { SetValue(InteractiveActivationModeProperty, value); }
        }

        #endregion

        #region InteractiveDeactivationMode
        /// <summary>
        /// Identifies the InteractiveDeactivationMode dependency property.
        /// </summary>
        public static readonly DependencyProperty InteractiveDeactivationModeProperty = RegisterDependencyProperty<InteractionType>("InteractiveDeactivationMode", (t, o, n) => t.OnInteractiveDeactivationModeChanged(o, n), DefaultInteractiveDeactivationMode);

        static InteractionType DefaultInteractiveDeactivationMode
        {
            get
            {
#if WINDOWS_PHONE
                return InteractionType.All;
#else
                return InteractionType.Soft;
#endif
            }
        }

        void OnInteractiveDeactivationModeChanged(InteractionType oldValue, InteractionType newValue)
        {
            if (IsInteractive)
            {
                if ((newValue & InteractionType.Soft) != InteractionType.Soft || !AutoHide)
                {
                    StopAutoHideTimer();
                }
                else if ((newValue & InteractionType.Soft) == InteractionType.Soft && (oldValue & InteractionType.Soft) != InteractionType.Soft && AutoHide)
                {
                    ResetAutoHideTimer();
                }
            }
        }

        /// <summary>
        /// Gets or sets the action that will cause the player to be taken out of interactive mode. (IsInteractive = false)
        /// </summary>
        [Category(Categories.Advanced)]
        public InteractionType InteractiveDeactivationMode
        {
            get { return (InteractionType)GetValue(InteractiveDeactivationModeProperty); }
            set { SetValue(InteractiveDeactivationModeProperty, value); }
        }

        #endregion

        #region AutoHide
        /// <summary>
        /// Identifies the AutoHide dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoHideProperty = RegisterDependencyProperty<bool>("AutoHide", (t, o, n) => t.OnAutoHideChanged(n), true);

        void OnAutoHideChanged(bool newValue)
        {
            if (IsInteractive)
            {
                if ((InteractiveDeactivationMode & InteractionType.Soft) == InteractionType.Soft && newValue)
                {
                    ResetAutoHideTimer();
                }
                else
                {
                    StopAutoHideTimer();
                }
            }
        }

        /// <summary>
        /// Gets or sets if the control panel (and other interactive elements) will automatically be hidden. Default is true.
        /// </summary>
        [Category(Categories.Common)]
        public bool AutoHide
        {
            get { return (bool)GetValue(AutoHideProperty); }
            set { SetValue(AutoHideProperty, value); }
        }
        #endregion

        #region AutoHideBehavior
        /// <summary>
        /// Identifies the AutoHideBehavior dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoHideBehaviorProperty = RegisterDependencyProperty<AutoHideBehavior>("AutoHideBehavior", AutoHideBehavior.PreventDuringInteractiveHover);

        /// <summary>
        /// Gets or sets when the control panel (and other interactive elements) will automatically be hidden. Default is AlwaysAllow.
        /// This is only applicable if AutoHide = true
        /// </summary>
        [Category(Categories.Common)]
        public AutoHideBehavior AutoHideBehavior
        {
            get { return (AutoHideBehavior)GetValue(AutoHideBehaviorProperty); }
            set { SetValue(AutoHideBehaviorProperty, value); }
        }
        #endregion

        #region IsInteractive
        /// <summary>
        /// Occurs when the IsInteractive property changes.
        /// </summary>
        public event RoutedEventHandler IsInteractiveChanged;

        /// <summary>
        /// Identifies the IsInteractive dependency property.
        /// </summary>
        public static readonly DependencyProperty IsInteractiveProperty = RegisterDependencyProperty<bool>("IsInteractive", (t, o, n) => t.OnIsInteractiveChanged(o, n), false);

        void OnIsInteractiveChanged(bool oldValue, bool newValue)
        {
            RegisterApplyTemplateAction(() =>
            {
                if (newValue)
                {
                    if (IsFullScreen)
                    {
#if SILVERLIGHT
                        this.Cursor = Cursors.Arrow;
#endif
                    }
                    if ((InteractiveDeactivationMode & InteractionType.Soft) == InteractionType.Soft && AutoHide)
                    {
                        ResetAutoHideTimer();
                    }
                }
                else
                {
                    if (IsFullScreen)
                    {
#if SILVERLIGHT
                        this.Cursor = Cursors.None;
#endif
                    }
                }

                if (newValue != oldValue)
                {
                    if (newValue)
                    {
                        this.GoToVisualState(MediaPlayerVisualStates.InteractiveStates.StartInteracting);
                    }
                    else
                    {
                        this.GoToVisualState(MediaPlayerVisualStates.InteractiveStates.StopInteracting);
                    }
                    if (IsInteractiveChanged != null) IsInteractiveChanged(this, new RoutedEventArgs());
                }
            });
        }

        /// <summary>
        /// Gest or sets whether the UI used to control playback should be displayed.
        /// If AutoHide = true, this is automatically set to true when the user interacts in any way with the player and set back to false after a length of time specified by the AutoHideInterval property.
        /// </summary>
        [Category(Categories.Advanced)]
        public bool IsInteractive
        {
            get { return (bool)GetValue(IsInteractiveProperty); }
            set { SetValue(IsInteractiveProperty, value); }
        }
        #endregion

        #region PlayerState
        /// <summary>
        /// Identifies the PlayerState dependency property.
        /// </summary>
        public static readonly DependencyProperty PlayerStateProperty = RegisterDependencyProperty<PlayerState>("PlayerState", (t, o, n) => t.OnPlayerStateChanged(n, o), PlayerState.Unloaded);

        void OnPlayerStateChanged(PlayerState newValue, PlayerState oldValue)
        {
            NotifyIsCaptionSelectionAllowedChanged();
            NotifyIsGoLiveAllowedChanged();
            NotifyIsPlayResumeAllowedChanged();
            NotifyIsPauseAllowedChanged();
            NotifyIsReplayAllowedChanged();
            NotifyIsAudioSelectionAllowedChanged();
            NotifyIsRewindAllowedChanged();
            NotifyIsFastForwardAllowedChanged();
            NotifyIsSlowMotionAllowedChanged();
            NotifyIsSeekAllowedChanged();
            NotifyIsSkipPreviousAllowedChanged();
            NotifyIsSkipNextAllowedChanged();
            NotifyIsSkipBackAllowedChanged();
            NotifyIsSkipAheadAllowedChanged();
            NotifyIsScrubbingAllowedChanged();
            NotifyIsInfoAllowedChanged();

            this.GoToVisualState(newValue.ToString());
            OnPlayerStateChanged(new RoutedPropertyChangedEventArgs<PlayerState>(oldValue, newValue));
        }

        /// <summary>
        /// Gets the player state. This is is different from the MediaState (CurrentState property) in that it indicates what stage of loading the media the player is in.
        /// Once the media is loaded, you should use CurrentState to examine the state of the media.
        /// </summary>
        [Category(Categories.Common)]
        public PlayerState PlayerState
        {
            get { return (PlayerState)GetValue(PlayerStateProperty); }
        }
        #endregion

#if SILVERLIGHT
#if !WINDOWS_PHONE
        #region Attributes

        Dictionary<string, string> _Attributes
        {
            get { return (MediaElementElement != null) ? MediaElementElement.Attributes : DefaultAttributes; }
        }

        static Dictionary<string, string> DefaultAttributes
        {
            get { return GetDefaultRefValue<Dictionary<string, string>>(MediaElement.AttributesProperty); }
        }

        #endregion

        #region IsDecodingOnGPU

        bool _IsDecodingOnGPU
        {
            get { return (MediaElementElement != null) ? MediaElementElement.IsDecodingOnGPU : DefaultIsDecodingOnGPU; }
        }

        static bool DefaultIsDecodingOnGPU
        {
            get { return GetDefaultValue<bool>(MediaElement.IsDecodingOnGPUProperty); }
        }

        #endregion
#endif
        #region DroppedFramesPerSecond

        double _DroppedFramesPerSecond
        {
            get { return (MediaElementElement != null) ? MediaElementElement.DroppedFramesPerSecond : DefaultDroppedFramesPerSecond; }
        }

        static double DefaultDroppedFramesPerSecond
        {
            get { return GetDefaultValue<double>(MediaElement.DroppedFramesPerSecondProperty); }
        }

        #endregion

        #region RenderedFramesPerSecond

        double _RenderedFramesPerSecond
        {
            get { return (MediaElementElement != null) ? MediaElementElement.RenderedFramesPerSecond : DefaultRenderedFramesPerSecond; }
        }

        static double DefaultRenderedFramesPerSecond
        {
            get { return GetDefaultValue<double>(MediaElement.RenderedFramesPerSecondProperty); }
        }

        #endregion

        #region LicenseAcquirer

        LicenseAcquirer _LicenseAcquirer
        {
            get { return MediaElementElement != null ? MediaElementElement.LicenseAcquirer : null; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.LicenseAcquirer = value; }); }
        }

        #endregion

        #region BufferingTime

        TimeSpan _BufferingTime
        {
            get { return MediaElementElement != null ? MediaElementElement.BufferingTime : DefaultBufferingTime; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.BufferingTime = value; }); }
        }

        static TimeSpan DefaultBufferingTime
        {
            get { return GetDefaultValue<TimeSpan>(MediaElement.BufferingTimeProperty); }
        }

        #endregion
#else

        #region AspectRatioWidth

        int _AspectRatioWidth
        {
            get { return MediaElementElement != null ? MediaElementElement.AspectRatioWidth : DefaultAspectRatioWidth; }
        }

        static int DefaultAspectRatioWidth
        {
            get { return GetDefaultValue<int>(MediaElement.AspectRatioWidthProperty); }
        }

        #endregion

        #region AspectRatioHeight

        int _AspectRatioHeight
        {
            get { return MediaElementElement != null ? MediaElementElement.AspectRatioHeight : DefaultAspectRatioHeight; }
        }

        static int DefaultAspectRatioHeight
        {
            get { return GetDefaultValue<int>(MediaElement.AspectRatioHeightProperty); }
        }

        #endregion

        #region AudioCategory

        AudioCategory _AudioCategory
        {
            get { return MediaElementElement != null ? MediaElementElement.AudioCategory : DefaultAudioCategory; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.AudioCategory = value; }); }
        }

        static AudioCategory DefaultAudioCategory
        {
            get { return GetDefaultValue<AudioCategory>(MediaElement.AudioCategoryProperty); }
        }

        #endregion

        #region AudioDeviceType

        AudioDeviceType _AudioDeviceType
        {
            get { return MediaElementElement != null ? MediaElementElement.AudioDeviceType : DefaultAudioDeviceType; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.AudioDeviceType = value; }); }
        }

        static AudioDeviceType DefaultAudioDeviceType
        {
            get { return GetDefaultValue<AudioDeviceType>(MediaElement.AudioDeviceTypeProperty); }
        }

        #endregion

#if !WINDOWS_UWP
        #region PlayToSource

        PlayToSource _PlayToSource
        {
            get { return MediaElementElement != null ? MediaElementElement.PlayToSource : DefaultPlayToSource; }
        }

        static PlayToSource DefaultPlayToSource
        {
            get { return GetDefaultRefValue<PlayToSource>(MediaElement.PlayToSourceProperty); }
        }

        partial void OnPlayToSourceChanged(PlayToSource oldValue, PlayToSource newValue)
        {
            if (oldValue != null)
            {
                try
                {
                    if (oldValue.Connection != null)
                    {
                        oldValue.Connection.StateChanged -= PlayToConnection_StateChanged;
                    }
                }
                catch { /* HACK: gettting .Connection can sometimes cause ignorable exception */ }
            }

            if (newValue != null && newValue.Connection != null)
            {
                OnPlayToStateChanged(newValue.Connection.State);
                newValue.Connection.StateChanged += PlayToConnection_StateChanged;
            }
            else
            {
                OnPlayToStateChanged(PlayToConnectionState.Disconnected);
            }
        }

        async void PlayToConnection_StateChanged(PlayToConnection sender, PlayToConnectionStateChangedEventArgs args)
        {
            await Dispatcher.BeginInvoke(() =>
            {
                OnPlayToStateChanged(args.CurrentState);
            });
        }

        private void OnPlayToStateChanged(PlayToConnectionState state)
        {
            switch (state)
            {
                case PlayToConnectionState.Connected:
                    this.GoToVisualState(MediaPlayerVisualStates.PlayToStates.Connected);
                    IsInteractive = true;
                    break;
                case PlayToConnectionState.Rendering:
                    this.GoToVisualState(MediaPlayerVisualStates.PlayToStates.Rendering);
                    break;
                case PlayToConnectionState.Disconnected:
                    this.GoToVisualState(MediaPlayerVisualStates.PlayToStates.Disconnected);
                    break;
            }
        }

        #endregion
#endif

        #region ProtectionManager

        MediaProtectionManager _ProtectionManager
        {
            get { return MediaElementElement != null ? MediaElementElement.ProtectionManager : DefaultProtectionManager; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.ProtectionManager = value; }); }
        }

        static MediaProtectionManager DefaultProtectionManager
        {
            get { return GetDefaultRefValue<MediaProtectionManager>(MediaElement.ProtectionManagerProperty); }
        }

        #endregion

        #region RealTimePlayback

        bool _RealTimePlayback
        {
            get { return MediaElementElement != null ? MediaElementElement.RealTimePlayback : DefaultRealTimePlayback; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.RealTimePlayback = value; }); }
        }

        static bool DefaultRealTimePlayback
        {
            get { return GetDefaultValue<bool>(MediaElement.RealTimePlaybackProperty); }
        }

        #endregion

        #region Stereo3DVideoPackingMode

        Stereo3DVideoPackingMode _Stereo3DVideoPackingMode
        {
            get { return MediaElementElement != null ? MediaElementElement.Stereo3DVideoPackingMode : DefaultStereo3DVideoPackingMode; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.Stereo3DVideoPackingMode = value; }); }
        }

        static Stereo3DVideoPackingMode DefaultStereo3DVideoPackingMode
        {
            get { return GetDefaultValue<Stereo3DVideoPackingMode>(MediaElement.Stereo3DVideoPackingModeProperty); }
        }

        #endregion

        #region ActualStereo3DVideoPackingMode

        Stereo3DVideoPackingMode _ActualStereo3DVideoPackingMode
        {
            get { return MediaElementElement != null ? MediaElementElement.ActualStereo3DVideoPackingMode : DefaultStereo3DVideoPackingMode; }
        }

        static Stereo3DVideoPackingMode DefaultActualStereo3DVideoPackingMode
        {
            get { return GetDefaultValue<Stereo3DVideoPackingMode>(MediaElement.ActualStereo3DVideoPackingModeProperty); }
        }

        #endregion

        #region Stereo3DVideoRenderMode

        Stereo3DVideoRenderMode _Stereo3DVideoRenderMode
        {
            get { return MediaElementElement != null ? MediaElementElement.Stereo3DVideoRenderMode : DefaultStereo3DVideoRenderMode; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.Stereo3DVideoRenderMode = value; }); }
        }

        static Stereo3DVideoRenderMode DefaultStereo3DVideoRenderMode
        {
            get { return GetDefaultValue<Stereo3DVideoRenderMode>(MediaElement.Stereo3DVideoRenderModeProperty); }
        }

        #endregion

        #region IsStereo3DVideo

        bool _IsStereo3DVideo
        {
            get { return MediaElementElement != null ? MediaElementElement.IsStereo3DVideo : DefaultIsStereo3DVideo; }
        }

        static bool DefaultIsStereo3DVideo
        {
            get { return GetDefaultValue<bool>(MediaElement.IsStereo3DVideoProperty); }
        }

        #endregion

        #region DefaultPlaybackRate

        double _DefaultPlaybackRate
        {
            get { return MediaElementElement != null ? MediaElementElement.DefaultPlaybackRate : DefaultDefaultPlaybackRate; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.DefaultPlaybackRate = value; }); }
        }

        static double DefaultDefaultPlaybackRate
        {
            get { return GetDefaultValue<double>(MediaElement.DefaultPlaybackRateProperty); }
        }

        #endregion

        #region IsAudioOnly

        bool _IsAudioOnly
        {
            get { return MediaElementElement != null ? MediaElementElement.IsAudioOnly : DefaultIsAudioOnly; }
        }

        partial void OnIsAudioOnlyUpdated(bool newValue)
        {
            if (newValue)
            {
                this.GoToVisualState(MediaPlayerVisualStates.MediaTypeStates.AudioOnly);
            }
            else
            {
                this.GoToVisualState(MediaPlayerVisualStates.MediaTypeStates.AudioVideo);
            }
        }

        static bool DefaultIsAudioOnly
        {
            get { return GetDefaultValue<bool>(MediaElement.IsAudioOnlyProperty); }
        }

        #endregion

        #region IsLooping

        bool _IsLooping
        {
            get { return MediaElementElement != null ? MediaElementElement.IsLooping : DefaultIsLooping; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.IsLooping = value; }); }
        }

        static bool DefaultIsLooping
        {
            get { return GetDefaultValue<bool>(MediaElement.IsLoopingProperty); }
        }

        #endregion

        #region PosterSource

        ImageSource _PosterSource
        {
            get { return MediaElementElement != null ? MediaElementElement.PosterSource : DefaultPosterSource; }
            set
            {
                if (!IsInDesignMode)
                {
                    RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.PosterSource = value; });
                }
            }
        }

        static ImageSource DefaultPosterSource
        {
            get { return GetDefaultRefValue<ImageSource>(MediaElement.PosterSourceProperty); }
        }

        #endregion

#if !WINDOWS80
        #region AreTransportControlsEnabled

        bool _AreTransportControlsEnabled
        {
            get { return MediaElementElement != null ? MediaElementElement.AreTransportControlsEnabled : DefaultAreTransportControlsEnabled; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.AreTransportControlsEnabled = value; }); }
        }

        static bool DefaultAreTransportControlsEnabled
        {
            get { return GetDefaultValue<bool>(MediaElement.AreTransportControlsEnabledProperty); }
        }

        #endregion

        #region IsFullWindow

        bool _IsFullWindow
        {
            get { return MediaElementElement != null ? MediaElementElement.IsFullWindow : DefaultIsFullWindow; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.IsFullWindow = value; }); }
        }

        static bool DefaultIsFullWindow
        {
            get { return GetDefaultValue<bool>(MediaElement.IsFullWindowProperty); }
        }

        #endregion

#if !WINDOWS_UWP
        #region PlayToPreferredSourceUri

        Uri _PlayToPreferredSourceUri
        {
            get { return MediaElementElement != null ? MediaElementElement.PlayToPreferredSourceUri : DefaultPlayToPreferredSourceUri; }
            set
            {
                if (!IsInDesignMode)
                {
                    RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.PlayToPreferredSourceUri = value; });
                }
            }
        }

        static Uri DefaultPlayToPreferredSourceUri
        {
            get { return GetDefaultRefValue<Uri>(MediaElement.PlayToPreferredSourceUriProperty); }
        }

        #endregion
#endif
#endif
#endif

#if !WINDOWS80

        #region Stretch

        Stretch _Stretch
        {
            get { return MediaElementElement != null ? MediaElementElement.Stretch : DefaultStretch; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.Stretch = value; }); }
        }

        static Stretch DefaultStretch
        {
            get { return GetDefaultValue<Stretch>(MediaElement.StretchProperty); }
        }

        #endregion
#endif

        #region IsCaptionsActive

        bool _IsCaptionsActive
        {
            get { throw new NotImplementedException(); }
            set
            {
                RegisterApplyTemplateAction(() =>
                {
                    if (value)
                    {
                        this.GoToVisualState(MediaPlayerVisualStates.CaptionsStates.CapitonsActive);
                    }
                    else
                    {
                        this.GoToVisualState(MediaPlayerVisualStates.CaptionsStates.CapitonsInactive);
                    }
                });
            }
        }

        #endregion

#if WINDOWS_UWP
        #region IsCasting

        bool _IsCasting { get; set; }

        #endregion
#endif

        #region IsFullScreen

        bool _IsFullScreen
        {
            get { throw new NotImplementedException(); }
            set
            {
                RegisterApplyTemplateAction(() =>
                {
                    if (value)
                    {
                        this.GoToVisualState(MediaPlayerVisualStates.FullScreenStates.FullScreen);
                    }
                    else
                    {
                        this.GoToVisualState(MediaPlayerVisualStates.FullScreenStates.NotFullScreen);
                    }
#if SILVERLIGHT && !WINDOWS_PHONE
                    if (Application.Current.Host.Content.IsFullScreen != value)
                    {
                        Application.Current.Host.Content.IsFullScreen = value;
                    }
#elif NETFX_CORE && WINDOWS80
                    if (value && Windows.UI.ViewManagement.ApplicationView.Value == Windows.UI.ViewManagement.ApplicationViewState.Snapped)
                    {
                        Windows.UI.ViewManagement.ApplicationView.TryUnsnap();
                    }
#endif
                });
            }
        }

        #endregion

        #region AdvertisingState

        AdvertisingState _AdvertisingState
        {
            get { throw new NotImplementedException(); }
            set
            {
                NotifyIsCaptionSelectionAllowedChanged();
                NotifyIsGoLiveAllowedChanged();
                NotifyIsPlayResumeAllowedChanged();
                NotifyIsPauseAllowedChanged();
                NotifyIsReplayAllowedChanged();
                NotifyIsAudioSelectionAllowedChanged();
                NotifyIsRewindAllowedChanged();
                NotifyIsFastForwardAllowedChanged();
                NotifyIsSlowMotionAllowedChanged();
                NotifyIsSeekAllowedChanged();
                NotifyIsSkipPreviousAllowedChanged();
                NotifyIsSkipNextAllowedChanged();
                NotifyIsSkipBackAllowedChanged();
                NotifyIsSkipAheadAllowedChanged();
                NotifyIsScrubbingAllowedChanged();

                RegisterApplyTemplateAction(() =>
                {
                    switch (value)
                    {
                        case AdvertisingState.None:
                            this.GoToVisualState(MediaPlayerVisualStates.AdvertisingStates.NoAd);
                            break;
                        case AdvertisingState.Loading:
                            this.GoToVisualState(MediaPlayerVisualStates.AdvertisingStates.LoadingAd);
                            break;
                        case AdvertisingState.Linear:
                            this.GoToVisualState(MediaPlayerVisualStates.AdvertisingStates.LinearAd);
                            break;
                        case AdvertisingState.NonLinear:
                            this.GoToVisualState(MediaPlayerVisualStates.AdvertisingStates.NonLinearAd);
                            break;
                    }
                });
            }
        }

        #endregion

        #region Markers

        /// <summary>
        /// Gets the collection of timeline markers associated with the currently loaded media file.
        /// </summary>
        TimelineMarkerCollection _Markers
        {
            get
            {
                if (!IsTemplateApplied)
                {
                    if (preTemplateAppliedMarkers == null) preTemplateAppliedMarkers = new TimelineMarkerCollection();
                    return preTemplateAppliedMarkers;
                }
                else
                {
                    return MediaElementElement.Markers;
                }
            }
        }

        #endregion

        #region AudioStreamCount

        int _AudioStreamCount
        {
            get { return MediaElementElement != null ? MediaElementElement.AudioStreamCount : DefaultAudioStreamCount; }
        }

        static int DefaultAudioStreamCount
        {
            get { return GetDefaultValue<int>(MediaElement.AudioStreamCountProperty); }
        }

        #endregion

        #region AudioStreamIndex

        int? _AudioStreamIndex
        {
            get { return MediaElementElement != null ? MediaElementElement.AudioStreamIndex : DefaultAudioStreamIndex; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.AudioStreamIndex = value; }); }
        }

        static int? DefaultAudioStreamIndex
        {
            get
            {
                // the default is zero at first (which causes binding errors). Instead default to null.
                return new int?();
                //return GetDefaultNullableValue<int>(MediaElement.AudioStreamIndexProperty);
            }
        }

        #endregion

        #region AutoPlay

        bool _AutoPlay
        {
            get { return MediaElementElement != null ? MediaElementElement.AutoPlay : DefaultAutoPlay; }
            set
            {
                if (!IsInDesignMode)
                {
                    RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.AutoPlay = value; });
                }
            }
        }

        static bool DefaultAutoPlay
        {
            get
            {
                return GetDefaultValue<bool>(MediaElement.AutoPlayProperty);
            }
        }

        #endregion

        #region BufferingProgress

        double _BufferingProgress
        {
            get { return MediaElementElement != null ? MediaElementElement.BufferingProgress : DefaultBufferingProgress; }
        }

        static double DefaultBufferingProgress
        {
            get { return GetDefaultValue<double>(MediaElement.BufferingProgressProperty); }
        }

        #endregion

        #region CanPause

        bool _CanPause
        {
            get { return MediaElementElement != null ? MediaElementElement.CanPause : DefaultCanPause; }
        }

        static bool DefaultCanPause
        {
            get
            {
                return GetDefaultValue<bool>(MediaElement.CanPauseProperty);
            }
        }

        #endregion

        #region CanSeek

        bool _CanSeek
        {
            get { return MediaElementElement != null ? MediaElementElement.CanSeek : DefaultCanSeek; }
        }

        static bool DefaultCanSeek
        {
            get
            {
                return GetDefaultValue<bool>(MediaElement.CanSeekProperty);
            }
        }

        #endregion

        #region Balance

        double _Balance
        {
            get { return MediaElementElement != null ? MediaElementElement.Balance : DefaultBalance; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.Balance = value; }); }
        }

        static double DefaultBalance
        {
            get { return GetDefaultValue<double>(MediaElement.BalanceProperty); }
        }

        #endregion

        #region NaturalDuration

        Duration _NaturalDuration
        {
            get { return MediaElementElement != null ? MediaElementElement.NaturalDuration : DefaultNaturalDuration; }
        }

        static Duration DefaultNaturalDuration
        {
            get
            {
                return GetDefaultValue<Duration>(MediaElement.NaturalDurationProperty);
            }
        }

        #endregion

        #region PlaybackRate

#if WINDOWS_PHONE
        double _PlaybackRate { get; set; }

        static double DefaultRate
        {
            get { return 1.0; }
        }
#else
        double _PlaybackRate
        {
            get { return MediaElementElement != null ? MediaElementElement.PlaybackRate : DefaultRate; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.PlaybackRate = value; }); }
        }

        static double DefaultRate
        {
            get { return GetDefaultValue<double>(MediaElement.PlaybackRateProperty); }
        }
#endif
        #endregion

        #region Position

#if POSITIONBINDING
        static readonly DependencyProperty InternalPositionProperty = DependencyProperty.Register("InternalPosition", typeof(TimeSpan), typeof(MediaPlayer), new PropertyMetadata(DefaultPosition, OnInternalPositionChanged));

        static void OnInternalPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MediaPlayer)d).MediaElement_PositionChanged((TimeSpan)e.NewValue);
        }
#endif

        TimeSpan _Position
        {
            get { return MediaElementElement != null ? MediaElementElement.Position : DefaultPosition; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.Position = value; }); }
        }

        static TimeSpan DefaultPosition
        {
            get { return GetDefaultValue<TimeSpan>(MediaElement.PositionProperty); }
        }

        #endregion

        #region CurrentState

        MediaElementState _CurrentState
        {
            get { return MediaElementElement != null ? MediaElementElement.CurrentState : DefaultCurrentState; }
        }

        static MediaElementState DefaultCurrentState
        {
            get { return GetDefaultValue<MediaElementState>(MediaElement.CurrentStateProperty); }
        }

        #endregion

        #region Source

        Uri _Source
        {
            get { return MediaElementElement != null ? MediaElementElement.Source : DefaultSource; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.Source = value; }); }
        }

        static Uri DefaultSource
        {
            get { return GetDefaultRefValue<Uri>(MediaElement.SourceProperty); }
        }

        #endregion

        #region NaturalVideoWidth

        int _NaturalVideoWidth
        {
            get { return MediaElementElement != null ? MediaElementElement.NaturalVideoWidth : DefaultNaturalVideoWidth; }
        }

        static int DefaultNaturalVideoWidth
        {
            get { return GetDefaultValue<int>(MediaElement.NaturalVideoWidthProperty); }
        }

        #endregion

        #region NaturalVideoHeight

        int _NaturalVideoHeight
        {
            get { return MediaElementElement != null ? MediaElementElement.NaturalVideoHeight : DefaultNaturalVideoHeight; }
        }

        static int DefaultNaturalVideoHeight
        {
            get { return GetDefaultValue<int>(MediaElement.NaturalVideoHeightProperty); }
        }

        #endregion

        #region Volume

        double _Volume
        {
            get { return MediaElementElement != null ? MediaElementElement.Volume : DefaultVolume; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.Volume = value; }); }
        }

        static double DefaultVolume
        {
            get { return GetDefaultValue<double>(MediaElement.VolumeProperty); }
        }

        #endregion

        #region IsMuted

        bool _IsMuted
        {
            get { return MediaElementElement != null ? MediaElementElement.IsMuted : DefaultIsMuted; }
            set { RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.IsMuted = value; }); }
        }

        static bool DefaultIsMuted
        {
            get { return GetDefaultValue<bool>(MediaElement.IsMutedProperty); }
        }

        #endregion

        #region DownloadProgress

        double _DownloadProgress
        {
            get { return MediaElementElement != null ? MediaElementElement.DownloadProgress : DefaultDownloadProgress; }
        }

        static double DefaultDownloadProgress
        {
            get { return GetDefaultValue<double>(MediaElement.DownloadProgressProperty); }
        }

        #endregion

        #region DownloadProgressOffset

        double _DownloadProgressOffset
        {
            get { return MediaElementElement != null ? MediaElementElement.DownloadProgressOffset : DefaultDownloadProgressOffset; }
        }

        static double DefaultDownloadProgressOffset
        {
            get { return GetDefaultValue<double>(MediaElement.DownloadProgressOffsetProperty); }
        }

        #endregion

        #endregion

        #region Methods

#if !WINDOWS_UWP && !SILVERLIGHT
        bool IsPlayToConnected()
        {
            try
            {
                return PlayToSource != null && PlayToSource.Connection != null && PlayToSource.Connection.State != PlayToConnectionState.Disconnected;
            }
            catch
            {
                // HACK: gettting .Connection can sometimes cause ignorable exception 
                return false;
            }
        }
#endif

        bool IsPointerOverInteractiveElement()
        {
#if SILVERLIGHT
            return false;
            // TODO: SL implementation should use a different approach since you cannot get the current mouse position at random time.
#else
            try
            {
                var elementsUnderPointer = VisualTreeHelper.FindElementsInHostCoordinates(Window.Current.CoreWindow.PointerPosition, this, false);
                return elementsUnderPointer.Intersect(interactiveElements.Where(ie => ie.HoverParticipation).Select(ie => ie.Element)).Any();
            }
            catch (UnauthorizedAccessException) // this exception is raised when Windows is locked.
            {
                return false;
            }
#endif
        }

        /// <summary>
        /// Indicates the user has interacted with the app and causes the player to reset the auto hide timer (if enabled).
        /// </summary>
        /// <param name="interactionType">The type of interaction the user peformed.</param>
        /// <param name="isFunctional">Indicates that the interaction performed a function or action of some kind.</param>
        public void OnUserInteraction(InteractionType interactionType, bool isFunctional)
        {
            if (IsInteractive)
            {
                if (!isFunctional && (interactionType & InteractionType.Hard) == InteractionType.Hard && (InteractiveDeactivationMode & InteractionType.Hard) == InteractionType.Hard)
                {
                    IsInteractive = false;
                }
                else if ((InteractiveDeactivationMode & InteractionType.Soft) == InteractionType.Soft && AutoHide)
                {
                    ResetAutoHideTimer();
                }
            }
            else
            {
                if ((InteractiveActivationMode & interactionType) == interactionType)
                {
                    IsInteractive = true;
                }
            }
        }

        /// <summary>
        /// Adds a UI element whoes mouse/pointer events should participate in causing the interactive elements of the control to appear.
        /// </summary>
        /// <param name="uiElement">The UIElement to participate in the interactivity state of the control</param>
        /// <param name="hoverParticipation">Prevent autohide if pointer is hovering over UIElement. Requires AutoHideBehavior to be set to PreventDuringInteractiveHover to have any affect.</param>
        /// <param name="isFunctional">Indicates that clicking on the UIElement may cause the an action to occur. Functional elements do not cause the control panel to disappear when clicked if tap to dismiss is enabled.</param>
        public void AddInteractiveElement(UIElement uiElement, bool hoverParticipation = false, bool isFunctional = true)
        {
#if SILVERLIGHT
            uiElement.MouseMove += uiElement_MouseMove;
            uiElement.MouseLeftButtonDown += uiElement_MouseLeftButtonDown;
#else
            uiElement.PointerMoved += uiElement_PointerMoved;
            uiElement.PointerPressed += uiElement_PointerPressed;
#endif
            interactiveElements.Add(new InteractiveElement(uiElement, hoverParticipation, isFunctional));
        }

        /// <summary>
        /// Removes a UI element after it was added via AddIntertiveElement.
        /// </summary>
        /// <param name="uiElement">The UIElement to no longer participate in the interactivity state of the control</param>
        public void RemoveInteractiveElement(UIElement uiElement)
        {
#if SILVERLIGHT
            uiElement.MouseMove -= uiElement_MouseMove;
            uiElement.MouseLeftButtonDown -= uiElement_MouseLeftButtonDown;
#else
            uiElement.PointerMoved -= uiElement_PointerMoved;
            uiElement.PointerPressed -= uiElement_PointerPressed;
#endif
            var interactiveElement = interactiveElements.FirstOrDefault(ie => ie.Element == uiElement);
            if (interactiveElement != null)
            {
                interactiveElements.Remove(interactiveElement);
            }
        }

        class InteractiveElement
        {
            internal InteractiveElement(UIElement element, bool hoverParticipation, bool isFunctional)
            {
                Element = element;
                HoverParticipation = hoverParticipation;
                IsFunctional = isFunctional;
            }

            public UIElement Element { get; private set; }
            public bool HoverParticipation { get; private set; }
            public bool IsFunctional { get; private set; }
        }

        /// <summary>
        /// Retries the media after an error by reopening the media and seeking to the last position
        /// </summary>
        public virtual void Retry()
        {
            RestoreMediaState(FailedMediaState ?? GetMediaState());
        }

        /// <summary>
        /// Restores the state of playback
        /// </summary>
        /// <param name="mediaState">An object containing information about the state of playback.</param>
        public virtual void RestoreMediaState(MediaState mediaState)
        {
            Close();
            AutoLoad = true;
            StartupPosition = mediaState.IsStarted ? mediaState.Position : (TimeSpan?)null;
            AutoPlay = mediaState.IsPlaying;
            Source = mediaState.Source;
        }

        /// <summary>
        /// Gets the current state of the MediaElement so it can be restored later.
        /// </summary>
        /// <returns>The MediaElement state</returns>
        public virtual MediaState GetMediaState()
        {
            var result = new MediaState();
            result.Source = Source;
            result.Position = VirtualPosition;
            result.IsStarted = PlayerState == PlayerState.Started;
            if (result.IsStarted)
            {
                result.IsPlaying = stateWithoutBufferg == MediaElementState.Playing || AdvertisingState == AdvertisingState.Linear || AdvertisingState == AdvertisingState.Loading;
            }
            else
            {
                result.IsPlaying = AutoPlay;
            }
            return result;
        }

        private void StopAutoHideTimer()
        {
            if (autoHideTimer != null)
            {
                if (autoHideTimer.IsEnabled) autoHideTimer.Stop();
            }
        }

        private void ResetAutoHideTimer()
        {
            if (autoHideTimer != null)
            {
                // reset the autohide timer
                if (autoHideTimer.IsEnabled) autoHideTimer.Stop();
                if (AutoHideInterval > TimeSpan.Zero)
                {
                    autoHideTimer.Interval = AutoHideInterval;
                    autoHideTimer.Start();
                }
            }
        }

        void _Stop()
        {
            RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.Stop(); });
        }

        void _Pause()
        {
            RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.Pause(); });
        }

        void _Play()
        {
            RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.Play(); });
        }

#if SILVERLIGHT

        void _RequestLog()
        {
            RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.RequestLog(); });
        }

        /// <summary>
        /// This sets the source of the MediaElement to a subclass of System.Windows.Media.MediaStreamSource.
        /// </summary>
        /// <param name="mediaStreamSource">A subclass of System.Windows.Media.MediaStreamSource.</param>
        void _SetSource(MediaStreamSource mediaStreamSource)
        {
            RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.SetSource(mediaStreamSource); });
        }

        /// <summary>
        /// Sets the MediaElement.Source property using the supplied stream.
        /// </summary>
        /// <param name="stream">A stream that contains a valid media source.</param>
        void _SetSource(Stream stream)
        {
            RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.SetSource(stream); });
        }

        /// <summary>
        /// Returns the default audio stream name or description.
        /// </summary>
        public static string DefaultAudioStreamName
        {
            get
            {
                return GetResourceString("DefaultAudioStreamName");
            }
        }
#else

        void _AddAudioEffect(string effectID, bool effectOptional, IPropertySet effectConfiguration)
        {
            RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.AddAudioEffect(effectID, effectOptional, effectConfiguration); });
        }

        void _AddVideoEffect(string effectID, bool effectOptional, IPropertySet effectConfiguration)
        {
            RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.AddVideoEffect(effectID, effectOptional, effectConfiguration); });
        }

        void _RemoveAllEffects()
        {
            RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.RemoveAllEffects(); });
        }

        MediaCanPlayResponse _CanPlayType(string type)
        {
            return MediaElementElement != null ? MediaElementElement.CanPlayType(type) : MediaCanPlayResponse.Maybe;
        }

        /// <summary>
        /// Returns the default audio stream name from a localizable resource file.
        /// </summary>
        public static string DefaultAudioStreamName
        {
            get
            {
                if (!IsInDesignMode)
                {
                    return GetResourceString("DefaultAudioStreamName");
                }
                else
                {
                    return "Untitled";
                }
            }
        }

        string _GetAudioStreamLanguage(int? index)
        {
            return MediaElementElement != null ? (index.HasValue ? MediaElementElement.GetAudioStreamLanguage(index) : DefaultAudioStreamName) : null;
        }

        void _SetSource(Windows.Storage.Streams.IRandomAccessStream stream, string mimeType)
        {
            RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.SetSource(stream, mimeType); });
        }

#if !WINDOWS80
        void _SetMediaStreamSource(Windows.Media.Core.IMediaSource source)
        {
            RegisterApplyTemplateAction(() => { if (MediaElementElement != null) MediaElementElement.SetMediaStreamSource(source); });
        }
#endif
#endif

        #endregion

        #region Template children event handlers

        #region MediaContainer
#if SILVERLIGHT
        void MediaContainer_MouseMove(object sender, MouseEventArgs e)
        {
            OnUserInteraction(InteractionType.Soft, false);
        }
#else
        void MediaContainer_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                OnUserInteraction(InteractionType.Soft, false);
            }
        }
#endif

#if SILVERLIGHT
        void MediaContainer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
#else
        void MediaContainer_PointerPressed(object sender, PointerRoutedEventArgs e)
#endif
        {
            OnUserInteraction(InteractionType.Hard, false);
        }
        #endregion

        #region MediaElement

#if POSITIONBINDING
        /// <summary>
        /// This is a fake event that comes from an internal binding
        /// </summary>
        private void MediaElement_PositionChanged(TimeSpan newValue)
        {
            var oldValue = this.Position;
            SetValueWithoutCallback(PositionProperty, newValue);
            OnPositionChanged(new RoutedPropertyChangedEventArgs<TimeSpan>(oldValue, newValue));
        }
#endif

        void MediaElement_RateChanged(object sender, RateChangedRoutedEventArgs e)
        {
            if (IsTrickPlayEnabled)
            {
#if SILVERLIGHT
                SetValueWithoutCallback(PlaybackRateProperty, e.NewRate);
#else
                SetValueWithoutCallback(PlaybackRateProperty, _PlaybackRate);
#endif
            }
            OnRateChanged(e);
        }

        void MediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            SetValue(CurrentStateProperty, _CurrentState);
            OnCurrentStateChanged(e);

            // go to a visual state
            switch (CurrentState)
            {
#if SILVERLIGHT
                case MediaElementState.AcquiringLicense:
                case MediaElementState.Individualizing:
#endif
                case MediaElementState.Opening:
                    this.GoToVisualState(MediaPlayerVisualStates.MediaStates.Opening);
                    break;
                case MediaElementState.Buffering:
                    this.GoToVisualState(MediaPlayerVisualStates.MediaStates.Buffering);
                    break;
                case MediaElementState.Playing:
                    this.GoToVisualState(MediaPlayerVisualStates.MediaStates.Playing);
                    break;
                case MediaElementState.Closed:
                    this.GoToVisualState(MediaPlayerVisualStates.MediaStates.Closed);
                    break;
                case MediaElementState.Paused:
                case MediaElementState.Stopped:
                    this.GoToVisualState(MediaPlayerVisualStates.MediaStates.Paused);
                    break;
            }
        }

        void MediaElement_DownloadProgressChanged(object sender, RoutedEventArgs e)
        {
            SetValue(DownloadProgressProperty, _DownloadProgress);
            SetValue(DownloadProgressOffsetProperty, _DownloadProgressOffset);
            OnDownloadProgressChanged(e);
        }

        void MediaElement_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
            SetValue(BufferingProgressProperty, _BufferingProgress);
            OnBufferingProgressChanged(e);
        }

#if SILVERLIGHT
        void MediaElement_LogReady(object sender, LogReadyRoutedEventArgs e)
        {
            OnLogReady(e);
        }
#else
        void MediaElement_VolumeChanged(object sender, RoutedEventArgs e)
        {
            OnVolumeChanged(e);
        }

        void MediaElement_SeekCompleted(object sender, RoutedEventArgs e)
        {
            OnSeekCompleted(e);
        }
#endif

        void MediaElement_MarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
        {
            OnMarkerReached(e);
        }

        void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            OnMediaFailure(e);
        }

        private void OnMediaFailure(ExceptionRoutedEventArgs e)
        {
            // it is important to save the state here as opposed to within OnMediaFailed because that method may get called from a retry failure and we don't want to reset the state from a retry.
            FailedMediaState = GetMediaState();
            FailedMediaState.Position = previousPosition; // Position will be reset to zero by the time we get here. Therefore remember the previous position instead.
            OnMediaFailed(e);
        }

        void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            OnMediaEnded(new MediaPlayerActionEventArgs());
        }

        void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            FailedMediaState = null;
#if WINDOWS_PHONE // HACK: MediaElement will stay in "Opening" state until playback has began. Force Paused visual state to prevent buffering view from displaying.
            if (!AutoPlay)
            {
                VisualStateManager.GoToState(this, "Paused", true);
            }
#endif
            OnMediaOpened(e);
        }
        #endregion

        #region Others

#if SILVERLIGHT && !WINDOWS_PHONE
        void Content_FullScreenChanged(object sender, EventArgs e)
        {
            IsFullScreen = Application.Current.Host.Content.IsFullScreen;
        }
#endif

#if SILVERLIGHT
        void autoHideTimer_Tick(object sender, EventArgs e)
#else
        void autoHideTimer_Tick(object sender, object e)
#endif
        {
            bool preventAutoHide = (
                ((AutoHideBehavior & AutoHideBehavior.AllowDuringPlaybackOnly) == AutoHideBehavior.AllowDuringPlaybackOnly && InteractiveViewModel.CurrentState != MediaElementState.Playing) ||
                ((AutoHideBehavior & AutoHideBehavior.PreventDuringInteractiveHover) == AutoHideBehavior.PreventDuringInteractiveHover && IsPointerOverInteractiveElement())
#if !SILVERLIGHT
                || interactiveElements.Select(ie => ie.Element).Any(el => el.HasKeyboardFocus())
#if !WINDOWS_UWP
                || IsPlayToConnected()
#endif
#endif
);

            if (!preventAutoHide)
            {
                StopAutoHideTimer();
                IsInteractive = false;
            }
        }

#if SILVERLIGHT
        void uiElement_MouseMove(object sender, MouseEventArgs e)
#else
        void uiElement_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
#endif
        {
            var interactiveElement = interactiveElements.FirstOrDefault(ie => ie.Element == sender);
            if (interactiveElement != null)
            {
                OnUserInteraction(InteractionType.Soft, interactiveElement.IsFunctional);
            }
        }

#if SILVERLIGHT
        void uiElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
#else
        void uiElement_PointerPressed(object sender, PointerRoutedEventArgs e)
#endif
        {
            var interactiveElement = interactiveElements.FirstOrDefault(ie => ie.Element == sender);
            if (interactiveElement != null)
            {
                OnUserInteraction(InteractionType.Hard, interactiveElement.IsFunctional);
            }
        }

        #endregion

        #endregion

    }
}
