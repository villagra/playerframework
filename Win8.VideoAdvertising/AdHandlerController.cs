using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
#if SILVERLIGHT
#else
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace Microsoft.VideoAdvertising
{
    public sealed class AdHandlerController
    {
        public IList<IAdPayloadHandler> AdPayloadHandlers { get; private set; }
        public TimeSpan? StartTimeout { get; set; }
        public IAdPayloadHandler ActiveHandler { get; private set; }
        private object activeAdContext;

        IPlayer player;
        public IPlayer Player
        {
            get { return player; }
            set
            {
                player = value;
                foreach (var handler in AdPayloadHandlers)
                {
                    handler.Player = player;
                }
            }
        }

#if SILVERLIGHT
        public event EventHandler AdStateChanged;
        public event EventHandler ActiveAdPlayerChanged;
#else
        public event EventHandler<object> AdStateChanged;
        public event EventHandler<object> ActiveAdPlayerChanged;
#endif
        public event EventHandler<NavigationRequestEventArgs> NavigationRequest;
        public event EventHandler<LoadPlayerEventArgs> LoadPlayer;
        public event EventHandler<UnloadPlayerEventArgs> UnloadPlayer;
        public event EventHandler<ActivateAdUnitEventArgs> ActivateAdUnit;
        public event EventHandler<DeactivateAdUnitEventArgs> DeactivateAdUnit;
        public event EventHandler<AdFailureEventArgs> AdFailure;

        public AdHandlerController()
        {
            var adPayloadHandlers = new ObservableCollection<IAdPayloadHandler>();
            adPayloadHandlers.CollectionChanged += AdPayloadHandlers_CollectionChanged;
            AdPayloadHandlers = adPayloadHandlers;
            // add the default ones that we already know about
            AdPayloadHandlers.Add(new VastAdPayloadHandler());
            AdPayloadHandlers.Add(new ClipAdPayloadHandler());
            AdPayloadHandlers.Add(new DocumentAdPayloadHandler());
            StartTimeout = TimeSpan.FromSeconds(8); // 8 second timeout by default.
        }

        void AdPayloadHandlers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var handler in e.NewItems.Cast<IAdPayloadHandler>())
                {
                    handler.Player = Player;
                    handler.LoadPlayer += Handler_LoadPlayer;
                    handler.UnloadPlayer += Handler_UnloadPlayer;
                    handler.ActivateAdUnit += Handler_ActivateAdUnit;
                    handler.DeactivateAdUnit += Handler_DeactivateAdUnit;
                }
            }

            if (e.OldItems != null)
            {
                foreach (var handler in e.OldItems.Cast<IAdPayloadHandler>())
                {
                    handler.Player = null;
                    handler.LoadPlayer -= Handler_LoadPlayer;
                    handler.UnloadPlayer -= Handler_UnloadPlayer;
                    handler.ActivateAdUnit -= Handler_ActivateAdUnit;
                    handler.DeactivateAdUnit -= Handler_DeactivateAdUnit;
                }
            }
        }


#if SILVERLIGHT
        public async Task PreloadAdAsync(IAdSource adSource, CancellationToken cancellationToken)
#else
        public IAsyncAction PreloadAdAsync(IAdSource adSource)
        {
            return AsyncInfo.Run(c => PreloadAdAsync(adSource, c));
        }

        internal async Task PreloadAdAsync(IAdSource adSource, CancellationToken cancellationToken)
#endif
        {
            var handler = AdPayloadHandlers.Where(h => h.SupportedTypes.Contains(adSource.Type)).FirstOrDefault();
            if (handler == null) throw new ArgumentException("No suitable handler was found to play this ad", "source");

            // resolve the source
            if (adSource is IResolveableAdSource)
            {
                var resolveableAdSource = adSource as IResolveableAdSource;
#if SILVERLIGHT
                await resolveableAdSource.LoadPayload(cancellationToken);
#else
                await (resolveableAdSource.LoadPayload().AsTask(cancellationToken));
#endif
                cancellationToken.ThrowIfCancellationRequested();
            }
#if SILVERLIGHT
            await handler.PreloadAdAsync(adSource, cancellationToken);
#else
            await handler.PreloadAdAsync(adSource).AsTask(cancellationToken);
#endif
        }

#if SILVERLIGHT
        public async Task PlayAdAsync(IAdSource adSource, CancellationToken cancellationToken, IProgress<AdStatus> progress)
#else
        public IAsyncActionWithProgress<AdStatus> PlayAdAsync(IAdSource adSource)
        {
            return AsyncInfo.Run<AdStatus>((c, p) => PlayAdAsync(adSource, c, p));
        }

        internal async Task PlayAdAsync(IAdSource adSource, CancellationToken cancellationToken, IProgress<AdStatus> progress)
#endif
        {
            try
            {
                if (ActiveHandler != null)
                {
                    if (!await ActiveHandler.CancelAd(false))
                    {
                        throw new Exception("Ad in progress and cannot be canceled.");
                    }
                }

                DateTime startTime = DateTime.Now;

                using (var timeoutTokenSource = new CancellationTokenSource())
                {
                    if (StartTimeout.HasValue)
                    {
                        timeoutTokenSource.CancelAfter(StartTimeout.Value);
                    }
                    var timeoutToken = timeoutTokenSource.Token;
                    using (var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutToken))
                    {
                        IsAdvertising = true;
                        var adContext = activeAdContext = new object();

                        progress.Report(AdStatus.Loading);

                        try
                        {
                            ActiveHandler = AdPayloadHandlers.Where(h => h.SupportedTypes.Contains(adSource.Type)).FirstOrDefault();
                            if (ActiveHandler == null) throw new ArgumentException("No suitable handler was found to play this ad", "source");

                            // resolve the source
                            if (adSource is IResolveableAdSource)
                            {
                                var resolveableAdSource = adSource as IResolveableAdSource;
                                try
                                {
#if SILVERLIGHT
                                    await resolveableAdSource.LoadPayload(cancellationTokenSource.Token);
#else
                                    await resolveableAdSource.LoadPayload().AsTask(cancellationTokenSource.Token);
#endif
                                }
                                catch (OperationCanceledException)
                                {
                                    if (timeoutToken.IsCancellationRequested) throw new TimeoutException(); else throw;
                                }
                            }

                            TimeSpan? timeRemaining = null;
                            if (StartTimeout.HasValue)
                            {
                                var timeElapsed = DateTime.Now.Subtract(startTime);
                                timeRemaining = StartTimeout.Value.Subtract(timeElapsed);
                            }

                            //var handlerProgress = new Progress<AdStatus>();
                            //handlerProgress.ProgressChanged += (s, e) => Progress_Handle(adContext, progress, e);
                            var handlerProgress = new Progress<AdStatus>(s => Progress_Handle(adContext, progress, s));
#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
                            await ActiveHandler.PlayAdAsync(adSource, timeRemaining, cancellationToken, handlerProgress);
                            await TaskEx.Yield(); // let evertyhing complete before finishing
#elif WINDOWS_PHONE
                            await ActiveHandler.PlayAdAsync(adSource, timeRemaining, cancellationToken, handlerProgress);
                            await Task.Yield(); // let evertyhing complete before finishing
#else
                            await ActiveHandler.PlayAdAsync(adSource, timeRemaining).AsTask(cancellationToken, handlerProgress);
                            await Task.Yield(); // let evertyhing complete before finishing
#endif
                            // leave the progress handler alone, it reports on secondary task which could still be running.
                        }
                        catch
                        {
                            TeardownAd();
                            progress.Report(AdStatus.Unloaded);
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (AdFailure != null) AdFailure(this, new AdFailureEventArgs(adSource, ex));
                throw;
            }
        }

        void Progress_Handle(object adContext, IProgress<AdStatus> progress, AdStatus status)
        {
            if (adContext == activeAdContext)
            {
                if (status == AdStatus.Unloaded)
                {
                    TeardownAd();
                }
                progress.Report(status);
            }
        }

        void TeardownAd()
        {
            IsAdvertising = false;
            ActiveHandler = null;
            activeAdContext = null;
        }

        void Handler_DeactivateAdUnit(object sender, DeactivateAdUnitEventArgs e)
        {
            if (DeactivateAdUnit != null) DeactivateAdUnit(this, e);
            ActiveAdPlayer = null;
        }

        void Handler_ActivateAdUnit(object sender, ActivateAdUnitEventArgs e)
        {
            ActiveAdPlayer = e.Player;
            try
            {
                if (ActivateAdUnit != null) ActivateAdUnit(this, e);
            }
            catch
            {
                ActiveAdPlayer = null;
                throw;
            }
        }

        void Handler_UnloadPlayer(object sender, UnloadPlayerEventArgs e)
        {
            if (UnloadPlayer != null) UnloadPlayer(this, e);
        }

        void Handler_LoadPlayer(object sender, LoadPlayerEventArgs e)
        {
            if (LoadPlayer != null) LoadPlayer(this, e);
        }


#if SILVERLIGHT
        public async Task CancelActiveAds()
#else
        public IAsyncAction CancelActiveAds()
        {
            return CancelActiveAdsInternal().AsAsyncAction();
        }

        internal async Task CancelActiveAdsInternal()
#endif
        {
            // cancel any active ads
            foreach (var handler in AdPayloadHandlers)
            {
                await handler.CancelAd(true);
            }
        }

        bool isAdvertising;
        public bool IsAdvertising
        {
            get { return isAdvertising; }
            private set
            {
                if (isAdvertising != value)
                {
                    isAdvertising = value;
                    if (isAdvertising)
                    {
                        AdState = AdState.Loading;
                    }
                    else
                    {
                        AdState = AdState.None;
                    }
                }
            }
        }

        private IVpaid activeAdPlayer;
        public IVpaid ActiveAdPlayer
        {
            get { return activeAdPlayer; }
            private set
            {
                if (activeAdPlayer != null)
                {
                    activeAdPlayer.AdClickThru -= Vpaid_AdClickThru;
                    activeAdPlayer.AdLinearChanged -= Vpaid_AdLinearChanged;
                }
                activeAdPlayer = value;
                if (ActiveAdPlayerChanged != null) ActiveAdPlayerChanged(this, EventArgs.Empty);
                if (activeAdPlayer != null)
                {
                    activeAdPlayer.AdClickThru += Vpaid_AdClickThru;
                    activeAdPlayer.AdLinearChanged += Vpaid_AdLinearChanged;
                    if (activeAdPlayer.AdLinear)
                    {
                        AdState = AdState.Linear;
                    }
                    else
                    {
                        AdState = AdState.NonLinear;
                    }
                }
            }
        }

        void Vpaid_AdLinearChanged(object sender, object e)
        {
            var vpaid = sender as IVpaid;
            if (vpaid.AdLinear)
            {
                AdState = AdState.Linear;
            }
            else
            {
                AdState = AdState.NonLinear;
            }
        }

        void Vpaid_AdClickThru(object sender, ClickThroughEventArgs e)
        {
            if (e.PlayerHandles)
            {
                if (NavigationRequest != null) NavigationRequest(this, new NavigationRequestEventArgs(e.Url));
            }
        }

        AdState adState;
        public AdState AdState
        {
            get { return adState; }
            private set
            {
                adState = value;
                if (AdStateChanged != null) AdStateChanged(this, EventArgs.Empty);
            }
        }
    }

    public enum AdState
    {
        None,
        Loading,
        Linear,
        NonLinear
    }
}
