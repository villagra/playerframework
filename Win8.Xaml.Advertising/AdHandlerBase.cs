using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VideoAdvertising;
using System.ComponentModel;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
#if !WINDOWS_PHONE
using System.Windows.Browser;
#else
using Microsoft.Phone.Tasks;
#endif
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.System;
#endif

namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// A base class used to play ads.
    /// </summary>
    public abstract class AdHandlerBase : INotifyPropertyChanged
    {
        readonly AdHandlerController controller;
        readonly Dictionary<IVpaid, List<CancellationTokenSource>> activeIcons = new Dictionary<IVpaid, List<CancellationTokenSource>>();

        /// <inheritdoc /> 
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the current active VPAID ad player. This is null until an ad is actually playing.
        /// </summary>
        public IVpaid ActiveAdPlayer
        {
            get { return controller.ActiveAdPlayer; }
        }

        /// <summary>
        /// Gets or sets the Xaml Style to be used on the CompanionHost HyperlinkButton control used to show companion ads.
        /// </summary>
        public Style CompanionHostStyle { get; set; }

        /// <summary>
        /// Indicates that the active ad player has changed.
        /// </summary>
        public event EventHandler ActiveAdPlayerChanged;

        /// <summary>
        /// Indicates that the advertising state has changed.
        /// </summary>
        public event EventHandler<AdStateEventArgs> AdStateChanged;

        /// <summary>
        /// Indicates that an ad failed.
        /// </summary>
        public event EventHandler<AdFailureEventArgs> AdFailure;

        /// <summary>
        /// Indicates that a new ad unit has started playing.
        /// </summary>
        public event EventHandler<ActivateAdUnitEventArgs> ActivateAdUnit;

        /// <summary>
        /// Indicates that a new ad unit has started playing.
        /// </summary>
        public event EventHandler<DeactivateAdUnitEventArgs> DeactivateAdUnit;

        /// <summary>
        /// Indicates that the active ad player has changed.
        /// </summary>
        public event EventHandler<NavigationRequestEventArgs> NavigationRequested;

        /// <summary>
        /// Creates a new instance of AdHandlerBase
        /// </summary>
        public AdHandlerBase()
        {
            controller = new AdHandlerController();
        }

        /// <summary>
        /// Wires up the AdHandlerController events
        /// </summary>
        protected void WireController()
        {
            controller.NavigationRequest += controller_NavigationRequest;
            controller.LoadPlayer += controller_LoadPlayer;
            controller.UnloadPlayer += controller_UnloadPlayer;
            controller.ActivateAdUnit += controller_ActivateAdUnit;
            controller.DeactivateAdUnit += controller_DeactivateAdUnit;
            controller.AdStateChanged += controller_AdStateChanged;
            controller.ActiveAdPlayerChanged += controller_ActiveAdPlayerChanged;
            controller.AdFailure += controller_AdFailure;
        }

        /// <summary>
        /// Unwires the AdHandlerController events
        /// </summary>
        protected void UnwireController()
        {
            controller.NavigationRequest -= controller_NavigationRequest;
            controller.LoadPlayer -= controller_LoadPlayer;
            controller.UnloadPlayer -= controller_UnloadPlayer;
            controller.ActivateAdUnit -= controller_ActivateAdUnit;
            controller.DeactivateAdUnit -= controller_DeactivateAdUnit;
            controller.AdStateChanged -= controller_AdStateChanged;
            controller.ActiveAdPlayerChanged -= controller_ActiveAdPlayerChanged;
            controller.AdFailure -= controller_AdFailure;
        }

        /// <summary>
        /// Gets or set the player adapter needed by the VideoAdvertising component.
        /// </summary>
        protected IPlayer Player
        {
            get { return controller.Player; }
            set { controller.Player = value; }
        }

        /// <summary>
        /// Cancels all active ads.
        /// </summary>
        public async Task CancelActiveAds()
        {
            await controller.CancelActiveAds();
        }

        void controller_AdFailure(object sender, AdFailureEventArgs e)
        {
            if (AdFailure != null) AdFailure(this, e);
        }

#if SILVERLIGHT
        void controller_ActiveAdPlayerChanged(object sender, EventArgs e)
#else
        void controller_ActiveAdPlayerChanged(object sender, object e)
#endif
        {
            if (ActiveAdPlayerChanged != null)
            {
                ActiveAdPlayerChanged(this, EventArgs.Empty);
            }
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("ActiveAdPlayer"));
            }
        }

#if SILVERLIGHT
        void controller_AdStateChanged(object sender, EventArgs e)
#else
        void controller_AdStateChanged(object sender, object e)
#endif
        {
            SetAdvertisingState(controller.AdState);
            if (AdStateChanged != null) AdStateChanged(this, new AdStateEventArgs(controller.AdState));
        }

        void controller_NavigationRequest(object sender, NavigationRequestEventArgs e)
        {
            RequestNavigation(e.Url);
        }

        void controller_UnloadPlayer(object sender, UnloadPlayerEventArgs e)
        {
            UnloadPlayer(e.Player);
        }

        void controller_LoadPlayer(object sender, LoadPlayerEventArgs e)
        {
            e.Player = GetPlayer(e.CreativeSource);
            if (e.Player != null)
            {
                LoadPlayer(e.Player);
            }
        }

        void controller_ActivateAdUnit(object sender, ActivateAdUnitEventArgs e)
        {
            if (ActivateAdUnit != null) ActivateAdUnit(this, e);
            // show companions
            LoadCompanions(e.Companions, e.SuggestedCompanionRules, e.CreativeSource, e.Player, e.CreativeConcept, e.AdSource);
            // show icons
            if (e.CreativeSource.Icons != null)
            {
                var vpaid2 = e.Player as IVpaid2;
                var preventIcons = vpaid2 != null ? vpaid2.AdIcons : false;
                if (!preventIcons)
                {
                    var canellationTokens = new List<CancellationTokenSource>();
                    activeIcons.Add(e.Player, canellationTokens);
                    foreach (var icon in e.CreativeSource.Icons)
                    {
                        var staticResource = icon.Item as StaticResource;
                        if (staticResource != null)
                        {
                            CancellationTokenSource cts = new CancellationTokenSource();
                            canellationTokens.Add(cts);
                            ShowIcon(icon, staticResource, cts);
                        }
                    }
                }
            }
        }

        /// <inheritdoc /> 
        protected virtual async void ShowIcon(Microsoft.VideoAdvertising.Icon icon, StaticResource staticResource, CancellationTokenSource cts)
        {
            if (icon.Offset.HasValue)
            {
                try
                {
#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
                    await TaskEx.Delay((int)icon.Offset.Value.TotalMilliseconds, cts.Token);
#else
                    await Task.Delay((int)icon.Offset.Value.TotalMilliseconds, cts.Token);
#endif
                }
                catch (OperationCanceledException) { /* swallow */ }
            }

            if (!cts.IsCancellationRequested)
            {
                var iconHost = new HyperlinkButton();
                iconHost.NavigateUri = icon.ClickThrough;

                double topMargin = 0;
                double leftMargin = 0;
                if (icon.Width.HasValue)
                {
                    iconHost.Width = icon.Width.Value;
                }
                if (icon.Height.HasValue)
                {
                    iconHost.Height = icon.Height.Value;
                }
                switch (icon.XPosition)
                {
                    case "left":
                        iconHost.HorizontalAlignment = HorizontalAlignment.Left;
                        break;
                    case "right":
                        iconHost.HorizontalAlignment = HorizontalAlignment.Right;
                        break;
                    default:
                        iconHost.HorizontalAlignment = HorizontalAlignment.Left;
                        int xPositionValue;
                        if (int.TryParse(icon.XPosition, out xPositionValue))
                        {
                            leftMargin = xPositionValue;
                        }
                        break;
                }
                switch (icon.YPosition)
                {
                    case "top":
                        iconHost.VerticalAlignment = VerticalAlignment.Top;
                        break;
                    case "bottom":
                        iconHost.VerticalAlignment = VerticalAlignment.Bottom;
                        break;
                    default:
                        iconHost.VerticalAlignment = VerticalAlignment.Top;
                        int yPositionValue;
                        if (int.TryParse(icon.YPosition, out yPositionValue))
                        {
                            topMargin = yPositionValue;
                        }
                        break;
                }
                iconHost.Margin = new Thickness(leftMargin, topMargin, 0, 0);

                var iconElement = new Image();
                iconElement.Stretch = Stretch.Fill;
                iconElement.Source = new BitmapImage(staticResource.Value);
                iconHost.Content = iconElement;

                //TODO: avoid visually overlaping icons

                iconHost.Tag = icon;
                iconElement.Tag = icon;

                iconHost.Click += iconHost_Click;
                iconElement.ImageOpened += iconElement_ImageOpened;
                AdContainer.Children.Add(iconHost);

                try
                {
                    if (icon.Duration.HasValue)
                    {
#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
                        await TaskEx.Delay((int)icon.Duration.Value.TotalMilliseconds, cts.Token);
#else
                        await Task.Delay((int)icon.Duration.Value.TotalMilliseconds, cts.Token);
#endif
                    }
                    else
                    {
                        await cts.Token.AsTask();
                    }
                }
                catch (OperationCanceledException) { /* swallow */ }
                finally
                {
                    AdContainer.Children.Remove(iconHost);
                    iconHost.Click -= iconHost_Click;
                    iconElement.ImageOpened -= iconElement_ImageOpened;
                }
            }
        }

        void iconElement_ImageOpened(object sender, RoutedEventArgs e)
        {
            var icon = (Microsoft.VideoAdvertising.Icon)((FrameworkElement)sender).Tag;
            foreach (var url in icon.ViewTracking)
            {
                VastHelpers.FireTracking(url);
            }
        }

        void iconHost_Click(object sender, RoutedEventArgs e)
        {
            var icon = (Microsoft.VideoAdvertising.Icon)((FrameworkElement)sender).Tag;
            foreach (var url in icon.ClickTracking)
            {
                VastHelpers.FireTracking(url);
            }
        }

        void controller_DeactivateAdUnit(object sender, DeactivateAdUnitEventArgs e)
        {
            if (DeactivateAdUnit != null) DeactivateAdUnit(this, e);
            if (e.Error != null)
            {
                UnloadCompanions();
            }
            // hide all icons
            HideIcons(e.Player);
        }

        private void HideIcons(IVpaid player)
        {
            if (activeIcons.ContainsKey(player))
            {
                foreach (var cts in activeIcons[player])
                {
                    cts.Cancel();
                }
                activeIcons.Remove(player);
            }
        }

        #region Hiding and showing players and companions
        /// <summary>
        /// Gets or sets the container to show the ads in. Note: this does not apply to companion ads.
        /// </summary>
        protected Panel AdContainer { get; set; }

        /// <summary>
        /// Called when an ad player should be unloaded.
        /// </summary>
        /// <param name="player">The VPAID player to unload.</param>
        protected virtual void UnloadPlayer(IVpaid player)
        {
            var uiElement = player as UIElement;
            AdContainer.Children.Remove(uiElement);
        }

        /// <summary>
        /// Called to retrieve a VPAID player from a creative source.
        /// </summary>
        /// <param name="creativeSource">The creative source that needs a VPAID player to play.</param>
        /// <returns>The VPAID ad player</returns>
        protected abstract IVpaid GetPlayer(ICreativeSource creativeSource);

        /// <summary>
        /// Called when a new VPAID player should be loaded.
        /// This does not mean it should be made visibile since it could be pre-loading.
        /// </summary>
        /// <param name="adPlayer">The VPAID player to load.</param>
        protected virtual void LoadPlayer(IVpaid adPlayer)
        {
            var uiElement = adPlayer as UIElement;
            AdContainer.Children.Add(uiElement);
        }

        object previouscreativeConcept = null;
        /// <summary>
        /// Called to help load companion ads.
        /// </summary>
        /// <param name="companions">The companion ads that should show.</param>
        /// <param name="suggestedCompanionRules">The suggested rules for how to show companions.</param>
        /// <param name="creativeSource">The creative source associated with the companions.</param>
        /// <param name="adPlayer">The VPAID ad player associated with the companions.</param>
        /// <param name="creativeConcept">The creative concept for the companions. Can help provide info to assist with companion life cycle business logic.</param>
        /// <param name="adSource">The ad source from which the companion ads came.</param>
        protected virtual void LoadCompanions(IEnumerable<ICompanionSource> companions, CompanionAdsRequired suggestedCompanionRules, ICreativeSource creativeSource, IVpaid adPlayer, object creativeConcept, IAdSource adSource)
        {
            if (previouscreativeConcept != null && creativeConcept != previouscreativeConcept)
            {
                // remove all old companions
                UnloadCompanions();
            }

            int failureCount = 0;
            int total = 0;

            companionUnloadActions.Clear();
            try
            {
                if (companions != null)
                {
                    foreach (var companion in companions)
                    {
                        Action undoAction;
                        if (!TryLoadCompanion(companion, out undoAction))
                        {
                            failureCount++;
                        }
                        else
                        {
                            companionUnloadActions.Add(undoAction);
                        }
                        total++;
                    }
                }

                if (suggestedCompanionRules == CompanionAdsRequired.Any && total > 0 && failureCount == total) throw new Exception("All companion ads failed");
                if (suggestedCompanionRules == CompanionAdsRequired.All && failureCount > 0) throw new Exception("Not all companion ads succeeded");

                previouscreativeConcept = creativeConcept;
            }
            catch
            {
                UnloadCompanions();
                throw;
            }
        }

        /// <summary>
        /// Loads the companion ad into the UI.
        /// </summary>
        /// <param name="source">Source information for the companion ad</param>
        /// <param name="unloadAction">The action to perform when it is time to unload the companion.</param>
        /// <returns>An action to undo the loaded companion if successful. Null if not.</returns>
        public virtual bool TryLoadCompanion(ICompanionSource source, out Action unloadAction)
        {
            if ((source.Type == CompanionType.Static))
            {
                FrameworkElement container = GetCompanionContainer(source);
                if (container != null)
                {
                    var companionHost = new CompanionHost();
                    if (CompanionHostStyle != null) companionHost.Style = CompanionHostStyle;
                    var companionElement = new Image()
                    {
                        Source = new BitmapImage(new Uri(source.Content)),
                        Stretch = Stretch.Fill,
                        Tag = source
                    };
                    companionHost.Content = companionElement;
                    companionHost.NavigateUri = source.ClickThrough;
                    companionHost.Tag = source;

                    if (!string.IsNullOrEmpty(source.AltText))
                    {
                        ToolTipService.SetToolTip(companionHost, new ToolTip() { Content = source.AltText });
                    }

                    Action unwireViewTrackingAction = null;
                    if (source.ViewTracking != null && source.ViewTracking.Any())
                    {
                        companionElement.ImageOpened += companionElement_ImageOpened;
                        unwireViewTrackingAction = () => companionElement.ImageOpened -= companionElement_ImageOpened;
                    }

                    Action unwireClickTrackingAction = null;
                    if (source.ClickTracking != null && source.ClickTracking.Any())
                    {
                        companionHost.Click += companionHost_Click;
                        unwireClickTrackingAction = () => companionHost.Click -= companionHost_Click;
                    }

                    if (container is Border)
                    {
                        ((Border)container).Child = companionHost;
                        unloadAction = () =>
                        {
                            if (unwireClickTrackingAction != null) unwireClickTrackingAction();
                            if (unwireViewTrackingAction != null) unwireViewTrackingAction();
                            ((Border)container).Child = null;
                        };
                        return true;
                    }
                    else if (container is Panel)
                    {
                        ((Panel)container).Children.Add(companionHost);
                        unloadAction = () =>
                        {
                            if (unwireClickTrackingAction != null) unwireClickTrackingAction();
                            if (unwireViewTrackingAction != null) unwireViewTrackingAction();
                            ((Panel)container).Children.Remove(companionHost);
                        };
                        return true;
                    }
                    else if (container is ContentControl)
                    {
                        ((ContentControl)container).Content = companionHost;
                        unloadAction = () =>
                        {
                            if (unwireClickTrackingAction != null) unwireClickTrackingAction();
                            if (unwireViewTrackingAction != null) unwireViewTrackingAction();
                            ((ContentControl)container).Content = null;
                        };
                        return true;
                    }
                    if (unwireClickTrackingAction != null) unwireClickTrackingAction();
                    if (unwireViewTrackingAction != null) unwireViewTrackingAction();
                }
            }
            unloadAction = null;
            return false;
        }

        void companionElement_ImageOpened(object sender, RoutedEventArgs e)
        {
            var source = (ICompanionSource)((FrameworkElement)sender).Tag;
            foreach (var url in source.ViewTracking)
            {
                VastHelpers.FireTracking(url);
            }
        }

        void companionHost_Click(object sender, RoutedEventArgs e)
        {
            var source = (ICompanionSource)((FrameworkElement)sender).Tag;
            foreach (var url in source.ClickTracking)
            {
                VastHelpers.FireTracking(url);
            }
        }

        /// <summary>
        /// Called when the container for a companion ad needs to be displayed.
        /// </summary>
        /// <param name="source">The source info for the companion ad.</param>
        /// <returns>The element in the UI to place the companion ad.</returns>
        protected abstract FrameworkElement GetCompanionContainer(ICompanionSource source);

        List<Action> companionUnloadActions = new List<Action>();
        /// <summary>
        /// Unloads all companion ads.
        /// </summary>
        public virtual void UnloadCompanions()
        {
            foreach (var unloadAction in companionUnloadActions)
            {
                unloadAction();
            }
            companionUnloadActions.Clear();
            previouscreativeConcept = null;
        }

        #endregion

        /// <summary>
        /// Called when navigation is requested from a click on an ad.
        /// </summary>
        /// <param name="url"></param>
#if SILVERLIGHT
        protected virtual void RequestNavigation(string url)
#else
        protected virtual async void RequestNavigation(string url)
#endif
        {
            if (NavigationRequested != null)
            {
                var eventArgs = new NavigationRequestEventArgs(url);
                NavigationRequested(this, eventArgs);
                if (eventArgs.Cancel) return;
            }

            if (!string.IsNullOrEmpty(url))
            {
#if WINDOWS_PHONE 
                WebBrowserTask task = new WebBrowserTask(); 
                task.Uri = new Uri(url); 
                task.Show(); 
#elif SILVERLIGHT
                HtmlPage.Window.Navigate(new Uri(url), "_blank");
#elif NETFX_CORE
                await Launcher.LaunchUriAsync(new Uri(url));
#endif
            }
        }

        /// <summary>
        /// Called when the advertising state has changed.
        /// Allows the subclass to change the UI or allowed behaviors appropriately.
        /// </summary>
        /// <param name="adState"></param>
        protected virtual void SetAdvertisingState(AdState adState)
        {
            // only here to support overriding. Not abstract in order to prevent it from being mandatory.
        }

        /// <summary>
        /// The timeout for an ad to start playing. If this is exceeded, the ad will be skipped
        /// </summary>
        public TimeSpan? StartTimeout
        {
            get { return controller.StartTimeout; }
            set { controller.StartTimeout = value; }
        }

        /// <summary>
        /// Preloads an ad so it is ready to play immediately at the right time.
        /// </summary>
        /// <param name="source">The source of the ad.</param>
        /// <param name="cancellationToken">A cancellation token that can later be used to abort the ad.</param>
        /// <returns>An awaitable Task that returns when the ad is done preloading.</returns>
        public Task PreloadAd(IAdSource source, CancellationToken cancellationToken)
        {
#if NETFX_CORE
            return controller.PreloadAdAsync(source).AsTask(cancellationToken);
#else
            return controller.PreloadAdAsync(source, cancellationToken);
#endif
        }

        /// <summary>
        /// Plays an ad.
        /// </summary>
        /// <param name="source">The source of the ad.</param>
        /// <param name="cancellationToken">A cancellation token that can later be used to abort the ad.</param>
        /// <param name="progress">An object that allows the progress to be monitored</param>
        /// <returns>An awaitable Task that returns when the ad is over, fails, or turns into a nonlinear ad.</returns>
        public Task PlayAd(IAdSource source, CancellationToken cancellationToken, IProgress<AdStatus> progress)
        {
#if NETFX_CORE
            return controller.PlayAdAsync(source).AsTask(cancellationToken, progress);
#else
            return controller.PlayAdAsync(source, cancellationToken, progress);
#endif
        }

        /// <summary>
        /// Gets a list of AdHandlers that are capable of playing different types of ads.
        /// </summary>
        public IList<IAdPayloadHandler> AdHandlers
        {
            get { return controller.AdPayloadHandlers; }
        }

        /// <summary>
        /// Supplies the ad state for the AdStateChangedEventArgs
        /// </summary>
        public sealed class AdStateEventArgs
#if SILVERLIGHT
 : EventArgs
#endif
        {
            internal AdStateEventArgs(AdState adState)
            {
                AdState = adState;
            }

            /// <summary>
            /// The new ad state.
            /// </summary>
            public AdState AdState { get; private set; }
        }
    }
}
