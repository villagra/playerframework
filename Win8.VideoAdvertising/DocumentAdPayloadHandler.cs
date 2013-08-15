using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace Microsoft.VideoAdvertising
{
    public sealed class DocumentAdPayloadHandler : IAdPayloadHandler
    {
        public DocumentAdPayloadHandler()
        {
            VpaidController = new PlayerAwareVpaidController();
            VpaidController.AdFailed += VpaidController_AdFailed;
            VpaidController.AdTrackingEventOccurred += VpaidController_AdTrackingEventOccurred;
        }

        public static string AdType { get { return "document"; } }
        static string[] SupportedTypeIds = new[] { AdType };
        public string[] SupportedTypes { get { return SupportedTypeIds; } }

        PreloadOperation loadOperation;
        ActiveAdUnit activeAd = null;
        ActiveOperation activeOperation = null;
        readonly PlayerAwareVpaidController VpaidController;
        IPlayer player;

        public event EventHandler<LoadPlayerEventArgs> LoadPlayer;
        public event EventHandler<UnloadPlayerEventArgs> UnloadPlayer;
        public event EventHandler<ActivateAdUnitEventArgs> ActivateAdUnit;
        public event EventHandler<DeactivateAdUnitEventArgs> DeactivateAdUnit;
        public event EventHandler<AdFailureEventArgs> AdFailure;
        public event EventHandler<AdTrackingEventEventArgs> AdTrackingEventOccurred;

        public IPlayer Player
        {
            get { return player; }
            set
            {
                player = value;
                VpaidController.Player = player;
            }
        }

        void VpaidController_AdFailed(object sender, ActiveAdUnitEventArgs e)
        {
            var ad = e.ActiveAdUnit.CreativeConcept as Ad;
            if (ad != null)
            {
                foreach (var error in ad.Errors)
                {
                    VpaidController.TrackErrorUrl(error, Microsoft.VideoAdvertising.VpaidController.Error_Vpaid, e.ActiveAdUnit.CreativeSource);
                }
            }
        }

        void VpaidController_AdTrackingEventOccurred(object sender, AdTrackingEventEventArgs e)
        {
            if (AdTrackingEventOccurred != null) AdTrackingEventOccurred(this, e);
        }

        IVpaid OnLoadPlayer(ICreativeSource creativeSource)
        {
            var args = new LoadPlayerEventArgs(creativeSource);
            if (LoadPlayer != null) LoadPlayer(this, args);
            return args.Player;
        }

        void OnUnloadPlayer(IVpaid player)
        {
            if (UnloadPlayer != null) UnloadPlayer(this, new UnloadPlayerEventArgs(player));
        }

        void OnActivateAd(ActiveAdUnit activeAdUnit, IEnumerable<ICompanionSource> companions, CompanionAdsRequired suggestedCompanionRules)
        {
            if (ActivateAdUnit != null) ActivateAdUnit(this, new ActivateAdUnitEventArgs(activeAdUnit.CreativeSource, activeAdUnit.Player, activeAdUnit.CreativeConcept, activeAdUnit.AdSource, companions, suggestedCompanionRules));
        }

        void OnDeactivateAd(ActiveAdUnit activeAdUnit, Exception error)
        {
            if (DeactivateAdUnit != null) DeactivateAdUnit(this, new DeactivateAdUnitEventArgs(activeAdUnit.CreativeSource, activeAdUnit.Player, activeAdUnit.CreativeConcept, activeAdUnit.AdSource, error));
        }

#if SILVERLIGHT
        public async Task<bool> CancelAd(bool force)
#else
        public IAsyncOperation<bool> CancelAd(bool force)
        {
            return AsyncInfo.Run(c => CancelActiveAd(force));
        }

        internal async Task<bool> CancelActiveAd(bool force)
#endif
        {
            // if there is a linear ad playing, return false.
            if (activeAd != null && !force && activeAd.Player.AdLinear) return false;
            if (activeOperation != null)
            {
                var cancelingOperation = activeOperation;
                try
                {
                    // there are active ads still. Force them to close.
                    await cancelingOperation.CancelAsync();
                }
                catch { /* ignore */ }
            }
            if (loadOperation != null && force)
            {
                try
                {
                    // there are preloads happening, force them to stop.
                    await loadOperation.CancelAsync();
                }
                catch { /* ignore */ }
                finally
                {
                    loadOperation = null;
                }
            }
            return true;
        }

#if SILVERLIGHT
        public async Task PlayAdAsync(IAdSource adSource, TimeSpan? startTimeout, CancellationToken cancellationToken, IProgress<AdStatus> progress)
#else
        public IAsyncActionWithProgress<AdStatus> PlayAdAsync(IAdSource adSource, TimeSpan? startTimeout)
        {
            return AsyncInfo.Run<AdStatus>((c, p) => PlayAdAsync(adSource, startTimeout, c, p));
        }

        internal async Task PlayAdAsync(IAdSource adSource, TimeSpan? startTimeout, CancellationToken cancellationToken, IProgress<AdStatus> progress)
#endif
        {
            if (!(adSource.Payload is AdDocumentPayload)) throw new ArgumentException("adSource must contain a payload of type AdDocumentPayload", "adPayload");
            var adDoc = (AdDocumentPayload)adSource.Payload;

            // create a new token that we can cancel independently
            var masterCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var playTasks = PlayAdAsync(adDoc, adSource, startTimeout, masterCancellationToken.Token, progress);
            try
            {
                activeOperation = new ActiveOperation(playTasks.Secondary, masterCancellationToken);
                await playTasks.Primary;
            }
            catch (PlayException)
            {
                activeOperation = null;
                throw;
            }
        }

        /// <summary>
        /// Note: This method is re-entrant safe but not thread safe.
        /// </summary>
        /// <param name="adDocument">The ad document to execute.</param>
        /// <param name="adSource">The original ad source that the ad document came from.</param>
        /// <param name="startTimeout">The timeout for the ad</param>
        /// <param name="cancellationToken">A cancellation token to cancel the ad.</param>
        /// <param name="progress">Reports progress of the ad.</param>
        /// <returns>Two tasks that indicate when the ad is finished and another for when the ad is done with its linear portion</returns>
        PlayAdAsyncResult PlayAdAsync(AdDocumentPayload adDocument, IAdSource adSource, TimeSpan? startTimeout, CancellationToken cancellationToken, IProgress<AdStatus> progress)
        {
            // primary task wll hold a task that finishes when the ad document finishes or a non-linear ad starts.
            var primaryTask = new TaskCompletionSource<object>();

            // secondary task will hold a task that finishes when the entire ad document finishes.
            var secondaryTask = TaskHelpers.Create<Task>(async () =>
                {
                    LoadException loadException = null; // keep around to re-throw in case we can't recover

                    using (var timeoutTokenSource = new CancellationTokenSource())
                    {
                        if (startTimeout.HasValue)
                        {
                            timeoutTokenSource.CancelAfter(startTimeout.Value);
                        }
                        var timeoutToken = timeoutTokenSource.Token;
                        using (var mainCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutToken))
                        {
                            var mainCancellationToken = mainCancellationTokenSource.Token;
                            // NOTE: mainCancellationToken is reset to just the initial token once we have successfully started an ad.

                            progress.Report(AdStatus.Opening);
                            try
                            {
                                try
                                {
                                    bool podPlayed = false; // throw an exception if no ad ever played
                                    // a task to hold the currently playing ad
                                    Task activeAdTask = null;

                                    foreach (var adPod in adDocument.AdPods)
                                    {
                                        try
                                        {
                                            bool adPlayed = false; // throw an exception if no ad ever played
                                            // model expects ads to be in the correct order.
                                            foreach (var defaultAd in adPod.Ads)
                                            {
                                                var adCandidates = new List<Ad>();
                                                adCandidates.Add(defaultAd);
                                                adCandidates.AddRange(defaultAd.FallbackAds);
                                                foreach (var ad in adCandidates)
                                                {
                                                    try // **** try ad ****
                                                    {
                                                        // group the creatives by sequence number. Always put the group without sequence number at the back of the piority list in compliance with VAST spec.
                                                        foreach (var creativeSet in ad.Creatives.GroupBy(c => c.Sequence).OrderBy(cs => cs.Key.GetValueOrDefault(int.MaxValue)).Select(cs => cs.ToList()))
                                                        {
                                                            var newAdUnit = await LoadAdUnitAsync(creativeSet, ad, adSource, mainCancellationToken);

                                                            // if there is an ad playing, wait for it to finish before proceeding.
                                                            if (activeAdTask.IsRunning())
                                                            {
                                                                await activeAdTask;
                                                                mainCancellationToken.ThrowIfCancellationRequested(); // we can safely assume this is not a timeout and only check the token passed as a param
                                                            }

                                                            if (!newAdUnit.Player.AdLinear)
                                                            {
                                                                // if the next ad is nonlinear, we've successfully finished the primary task.
                                                                primaryTask.TrySetResult(null);
                                                            }

                                                            LoadAdUnit(newAdUnit, creativeSet);

                                                            adPlayed = true;
                                                            loadException = null; // if there was a load error, we recovered, reset.
                                                            progress.Report(AdStatus.Loaded);
                                                            activeAd = newAdUnit;
                                                            VpaidController.AddAd(activeAd);

                                                            try
                                                            {
                                                                await StartAdUnitAsync(newAdUnit, ad, mainCancellationToken);

                                                                // we successfully started an ad, create a new cancellation token that is not linked to timeout
                                                                mainCancellationToken = cancellationToken;
                                                                progress.Report(AdStatus.Playing);

                                                                // returns when task is over or approaching end
                                                                activeAdTask = await PlayAdUnitAsync(newAdUnit, mainCancellationToken);
                                                                if (activeAdTask != null)
                                                                {
                                                                    activeAdTask = activeAdTask.ContinueWith((Task t) =>
                                                                    {
                                                                        progress.Report(AdStatus.Complete);
                                                                        activeAd = null;
                                                                    }, TaskScheduler.FromCurrentSynchronizationContext());
                                                                }
                                                                else
                                                                {
                                                                    progress.Report(AdStatus.Complete);
                                                                    activeAd = null;
                                                                }
                                                            }
                                                            catch
                                                            {
                                                                CleanupAd(activeAd);
                                                                progress.Report(AdStatus.Complete);
                                                                activeAd = null;
                                                                throw;
                                                            }
                                                        }
                                                        break; // we successfully played an ad, break out of loop.
                                                    } // **** end try ad ****
                                                    catch (LoadException)
                                                    {
                                                        if (ad == adCandidates.Last()) throw; // ignore if it's not the last ad in order ot go to next fallback ad 
                                                    }
                                                }
                                            }
                                            if (!adPlayed) throw new LoadException(new Exception("No ads found."));
                                            podPlayed = true;
                                            break;  // we should only play one adPod per the VAST spec
                                        }
                                        catch (LoadException ex)
                                        {
                                            // keep around to re-throw in case we can't successfully load an ad
                                            loadException = ex;
                                            // move to the next adpod, ignore for now. We'll log this later if it's relevant
                                        }
                                    }
                                    // always wait for the playing ad to complete before returning
                                    if (activeAdTask.IsRunning())
                                    {
                                        await activeAdTask;
                                    }
                                    if (!podPlayed)
                                    {
                                        VpaidController.TrackErrorUrl(adDocument.Error, Microsoft.VideoAdvertising.VpaidController.Error_NoAd);
                                        throw new LoadException(new Exception("No ads found."));
                                    }
                                }
                                catch (OperationCanceledException)
                                {
                                    if (timeoutToken.IsCancellationRequested) throw new TimeoutException(); else throw;
                                }
                                catch (Exception ex)
                                {
                                    throw new PlayException(ex);
                                }
                                if (loadException != null)
                                {
                                    throw loadException;
                                }
                            }
                            catch (Exception ex)
                            {
                                LogError(adSource, ex);
                                primaryTask.TrySetException(ex);
                                throw;
                            }
                            finally
                            {
                                progress.Report(AdStatus.Unloaded);
                            }
                            primaryTask.TrySetResult(null);
                        }
                    }
                });

            return new PlayAdAsyncResult(primaryTask.Task, secondaryTask);
        }

        async Task<ActiveAdUnit> LoadAdUnitAsync(ICollection<ICreative> creativeSet, Ad ad, IAdSource adSource, CancellationToken cancellationToken)
        {
            bool preloaded = false;
            ActiveAdUnit result = null;

            // check to see if there are any pre-loading ads.
            if (loadOperation != null)
            {
                try
                {
                    if (loadOperation.AdSource == adSource)
                    {
                        cancellationToken.Register(() =>
                        {
                            if (loadOperation != null)
                            {
                                // no need to wait
                                var dummyTask = loadOperation.CancelAsync();
                            }
                        });
                        result = await loadOperation.Task;
                        preloaded = true;
                    }
                    else
                    {
                        await loadOperation.CancelAsync();
                    }
                }
                catch { /* ignore, we'll try again regardless of reason, it was just a preload optimization anyway */ }
                finally
                {
                    loadOperation = null;
                }
                cancellationToken.ThrowIfCancellationRequested();
            }
            if (result == null)
            {
                result = CreateAdUnit(creativeSet, ad, adSource);
            }
            if (result == null) throw new LoadException(new Exception("No ad unit could be created"));

            if (!preloaded)
            {
                // Start initializing the ad. It is OK if there are other ads playing still.
                loadOperation = LoadAdUnit(result, cancellationToken);
                try
                {
                    await loadOperation.Task;
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception ex) { throw new LoadException(ex); }
                finally
                {
                    loadOperation = null;
                }
                cancellationToken.ThrowIfCancellationRequested();
            }

            return result;
        }

        void LoadAdUnit(ActiveAdUnit newAdUnit, ICollection<ICreative> creativeSet)
        {
            // load companions
            CompanionAdsRequired required = CompanionAdsRequired.None;
            IEnumerable<ICompanionSource> companionsToLoad = Enumerable.Empty<ICompanionSource>();
            var companions = creativeSet.OfType<CreativeCompanions>().FirstOrDefault();
            if (companions != null)
            {
                companionsToLoad = companions.Companions.Cast<ICompanionSource>();
                required = companions.Required;
            }
            else
            {
                // VPAID 2.0 supports companions that come from the VPAID player.
                // these are only to be used with the VAST file contains none.
                if (newAdUnit.Player is IVpaid2)
                {
                    var companionData = ((IVpaid2)newAdUnit.Player).AdCompanions;
                    if (!string.IsNullOrEmpty(companionData))
                    {
                        CreativeCompanions vpaidCompanions;
                        using (var stream = companionData.ToStream())
                        {
                            vpaidCompanions = AdModelFactory.CreateCompanionsFromVast(stream);
                        }
                        companionsToLoad = vpaidCompanions.Companions.Cast<ICompanionSource>();
                        required = vpaidCompanions.Required;
                    }
                }
            }

            try
            {
                OnActivateAd(newAdUnit, companionsToLoad, required);
            }
            catch (Exception ex)
            {
                throw new LoadException(ex);
            }
        }

        async Task StartAdUnitAsync(ActiveAdUnit adUnit, Ad ad, CancellationToken cancellationToken)
        {
            // start the ad
            await VpaidController.StartAdAsync(adUnit, cancellationToken);

            // fire the impression beacon
            foreach (var url in ad.Impressions)
            {
                VpaidController.TrackUrl(url, adUnit.CreativeSource);
            }
        }

        async Task<Task> PlayAdUnitAsync(ActiveAdUnit adUnit, CancellationToken cancellationToken)
        {
#if WINDOWS_PHONE7
            // WP doesn't support 3 MediaElements all with a source loaded. Instead of preloading, just wait until we finish the ad.
            await VpaidController.FinishAdAsync(activeAd, cancellationToken);
            var finished = true;
#else
            var finished = await VpaidController.PlayAdAsync(adUnit, cancellationToken);
#endif
            if (!finished)
            {
                // if approaching end has happened, retain a new task that will run to completion
                return TaskHelpers.Create<Task>(async () =>
                {
                    await VpaidController.FinishAdAsync(adUnit, cancellationToken);
                    CleanupAd(adUnit);
                });
            }
            else
            {
                CleanupAd(adUnit);
                return null;
            }
        }

        void LogError(IAdSource adSource, Exception ex)
        {
            if (AdFailure != null) AdFailure(this, new AdFailureEventArgs(adSource, ex));
        }

        void CleanupAd(ActiveAdUnit adUnit)
        {
            OnDeactivateAd(adUnit, null);
            OnUnloadPlayer(adUnit.Player);
            VpaidController.RemoveAd(adUnit);
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
            if (!(adSource.Payload is AdDocumentPayload)) throw new ArgumentException("adSource must contain a payload of type AdDocumentPayload", "adPayload");
            var adDoc = (AdDocumentPayload)adSource.Payload;

            if (loadOperation != null && loadOperation.Task.IsRunning())
            {
                return;
            }

            await PreloadAdAsync(adDoc, adSource, cancellationToken);
        }

        /// <summary>
        /// Preloads an ad that will be played later.
        /// </summary>
        /// <param name="adDocument">The ad document to execute.</param>
        /// <param name="adSource">The original ad source that the ad document came from.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the ad.</param>
        /// <returns></returns>
        async Task PreloadAdAsync(AdDocumentPayload adDocument, IAdSource adSource, CancellationToken cancellationToken)
        {
            foreach (var adPod in adDocument.AdPods)
            {
                try
                {
                    // model expects ads to be in the correct order.
                    foreach (var ad in adPod.Ads)
                    {
                        // group the creatives by sequence number. Always put the group without sequence number at the back of the piority list in compliance with VAST spec.
                        foreach (var creativeSet in ad.Creatives.GroupBy(c => c.Sequence).OrderBy(cs => cs.Key.GetValueOrDefault(int.MaxValue)))
                        {
                            var newAdUnit = CreateAdUnit(creativeSet, ad, adSource);
                            if (newAdUnit != null) // a violation of the VAST spec but we will just ignore
                            {
                                loadOperation = LoadAdUnit(newAdUnit, cancellationToken);
                                try
                                {
                                    await loadOperation.Task;
                                    cancellationToken.ThrowIfCancellationRequested();
                                }
                                catch
                                {
                                    loadOperation = null;
                                    throw;
                                }
                                return;
                            }
                        }
                    }
                }
                catch (LoadException) { /* ignore, move to the next adpod */ }
            }
        }

        PreloadOperation LoadAdUnit(ActiveAdUnit adUnit, CancellationToken cancellationToken)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            Task<ActiveAdUnit> task;
            if (loadOperation != null)
            {
                if (loadOperation.AdSource != adUnit.AdSource)  // cancel active task and wait for it to finish
                {
                    task = TaskHelpers.Create<Task<ActiveAdUnit>>(async () =>
                    {
                        try
                        {
                            await loadOperation.CancelAsync();
                        }
                        finally
                        {
                            loadOperation = null;
                        }
                        cancellationToken.ThrowIfCancellationRequested();
                        return await GetInitializationTask(adUnit, cts.Token);
                    });
                }
                else
                {
                    task = loadOperation.Task;
                }
            }
            else
            {
                task = GetInitializationTask(adUnit, cts.Token);
            }

            return new PreloadOperation(task, adUnit.AdSource, cts);
        }

        async Task<ActiveAdUnit> GetInitializationTask(ActiveAdUnit adUnit, CancellationToken cancellationToken)
        {
            try
            {
                await VpaidController.InitAdAsync(adUnit, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch
            {
                OnUnloadPlayer(adUnit.Player);
                throw;
            }
            return adUnit;
        }

        ActiveAdUnit CreateAdUnit(IEnumerable<ICreative> creativeSet, Ad ad, IAdSource adSource)
        {
            CreativeCompanions companions = null;
            var activeAdUnits = new List<ActiveAdUnit>();
            foreach (var creative in creativeSet)
            {
                if (creative is CreativeLinear)
                {
                    var linear = (CreativeLinear)creative;
                    var eligableMediaFiles = new Queue<MediaFile>(PrioritizeMedia(linear.MediaFiles));
                    ActiveAdUnit chosenAd = null;
                    while (eligableMediaFiles.Any())
                    {
                        var media = eligableMediaFiles.Dequeue();
                        var creativeSource = new LinearSource(linear, media);
                        var vpaid = OnLoadPlayer(creativeSource);
                        if (vpaid != null)
                        {
                            if (VpaidController.Handshake(vpaid))
                            {
                                chosenAd = new ActiveAdUnit(creativeSource, vpaid, ad, adSource);
                                break;
                            }
                            else
                            {
                                OnUnloadPlayer(vpaid);
                            }
                        }
                    }

                    if (chosenAd == null) throw new LoadException(new Exception("Unable to find a player to play the linear ad."));
                    activeAdUnits.Add(chosenAd);
                }
                else if (creative is CreativeNonLinears)
                {
                    bool found = false;
                    var nonLinears = (CreativeNonLinears)creative;
                    foreach (var nonLinear in (nonLinears).NonLinears)
                    {
                        var creativeSource = new NonLinearSource(nonLinear, nonLinears);
                        var vpaid = OnLoadPlayer(creativeSource);
                        if (vpaid != null)
                        {
                            if (VpaidController.Handshake(vpaid))
                            {
                                activeAdUnits.Add(new ActiveAdUnit(creativeSource, vpaid, ad, adSource));
                                found = true;
                                break;
                            }
                            else
                            {
                                OnUnloadPlayer(vpaid);
                            }
                        }
                    }
                    if (!found)
                    {
                        throw new LoadException(new Exception("Unable to find a player to play any nonlinear ads."));
                    }
                }
                else if (creative is CreativeCompanions)
                {
                    companions = (CreativeCompanions)creative;
                }
                else { /* not supported, ignore */ }
            }
            return activeAdUnits.FirstOrDefault(); // there should only be one linear or nonlinear ad in the sequenced group. Others are ignored.
        }

        private IOrderedEnumerable<MediaFile> PrioritizeMedia(IEnumerable<MediaFile> mediaFiles)
        {
            double targetBitrateKbps = (double)Player.CurrentBitrate / 1024;
            if (targetBitrateKbps <= 0)
            {
                // if there is no bitrate, target based on the one in the middle
                var mediaWithBitrates = mediaFiles.Where(m => m.Bitrate.HasValue);
                if (mediaWithBitrates.Any())
                {
                    targetBitrateKbps = mediaWithBitrates.Average(m => m.Bitrate.Value);
                }
            }

            // get the media with the closest aspect ratio, then bitrate, then dimensions
            return mediaFiles
                .OrderByDescending(m => m.Ranking)
                .OrderBy(IsStreaming)
                .ThenBy(m => CompareAspectRatio(m, Player.Dimensions))
                .ThenBy(m => CompareBitrate(m, targetBitrateKbps))
                .ThenBy(m => CompareSize(m, Player.Dimensions));
        }

        private static double CompareAspectRatio(MediaFile mediaFile, Size targetSize)
        {
            // round to 1 decimal place in case width/height isn't 100% exactly the same for 2 different media files with nearly identical aspect ratios.
            return Math.Abs(Math.Round((double)mediaFile.Width / (double)mediaFile.Height, 1) - Math.Round(targetSize.Width / targetSize.Height, 1));
        }

        private static double CompareSize(MediaFile mediaFile, Size targetSize)
        {
            return Math.Abs(mediaFile.Height * mediaFile.Width - targetSize.Height * targetSize.Width);
        }

        private static double CompareBitrate(MediaFile mediaFile, double targetBitrateKbps)
        {
            return Math.Abs(mediaFile.Bitrate.GetValueOrDefault(int.MaxValue) - targetBitrateKbps);
        }

        private static bool IsStreaming(MediaFile mediaFile)
        {
            return mediaFile.Delivery == MediaFileDelivery.Streaming;
        }

        class PlayAdAsyncResult
        {
            public PlayAdAsyncResult(Task primary, Task secondary)
            {
                Primary = primary;
                Secondary = secondary;
            }

            public Task Primary { get; private set; }
            public Task Secondary { get; private set; }
        }

        class LoadException : PlayException
        {
            public LoadException(Exception ex) : base("Ad could not load", ex) { }
        }

        class PlayException : Exception
        {
            public PlayException(string message, Exception ex) : base(message, ex) { }
            public PlayException(Exception ex) : this("Ad could not play", ex) { }
        }

        class ActiveOperation
        {
            readonly CancellationTokenSource cts;

            public ActiveOperation(Task task, CancellationTokenSource cancellationTokenSource)
            {
                cts = cancellationTokenSource;
                Task = task;
            }

            public Task Task { get; private set; }

            public async Task CancelAsync()
            {
                if (Task.IsRunning())
                {
                    if (!cts.IsCancellationRequested)
                    {
                        cts.Cancel();
                    }
                    try
                    {
                        await Task;
                    }
                    catch { /* ignore */ }
                }
            }
        }

        class PreloadOperation : ActiveOperation
        {
            public PreloadOperation(Task<ActiveAdUnit> task, IAdSource adSource, CancellationTokenSource cancellationTokenSource)
                : base(task, cancellationTokenSource)
            {
                AdSource = adSource;
            }

            public new Task<ActiveAdUnit> Task { get { return (Task<ActiveAdUnit>)base.Task; } }
            public IAdSource AdSource { get; private set; }
        }
    }
}
