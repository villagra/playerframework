using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using Windows.Foundation;
using System.Diagnostics;

namespace Microsoft.Media.Advertising
{
    internal class PlayerAwareVpaidController : VpaidController
    {
        IPlayer player;
        public IPlayer Player
        {
            get { return player; }
            set
            {
                if (player != null)
                {
                    player.IsMutedChanged -= player_IsMutedChanged;
                    player.VolumeChanged -= player_VolumeChanged;
                    player.FullscreenChanged -= player_FullscreenChanged;
                    player.DimensionsChanged -= player_DimensionsChanged;
                }
                player = value;
                if (player != null)
                {
                    player.IsMutedChanged += player_IsMutedChanged;
                    player.VolumeChanged += player_VolumeChanged;
                    player.FullscreenChanged += player_FullscreenChanged;
                    player.DimensionsChanged += player_DimensionsChanged;
                }
            }
        }

        void player_IsMutedChanged(object sender, object e)
        {
            foreach (var activeAdUnit in base.ActiveAdUnits)
            {
                activeAdUnit.Player.AdVolume = player.IsMuted ? 0 : player.Volume;
                if (player.IsMuted)
                {
                    TrackEvent(activeAdUnit, TrackingType.Mute);
                }
                else
                {
                    TrackEvent(activeAdUnit, TrackingType.Unmute);
                }
            }
        }

        void player_VolumeChanged(object sender, object e)
        {
            if (!player.IsMuted)
            {
                foreach (var adPlayer in base.ActivePlayers)
                {
                    adPlayer.AdVolume = player.Volume;
                }
            }
        }

        void player_DimensionsChanged(object sender, object e)
        {
            foreach (var adPlayer in base.ActivePlayers)
            {
                adPlayer.ResizeAd(player.Dimensions.Width, player.Dimensions.Height, player.IsFullScreen ? "fullscreen" : "normal");
            }
        }

        void player_FullscreenChanged(object sender, object e)
        {
            if (Player.IsFullScreen)
            {
                foreach (var activeAdUnit in base.ActiveAdUnits)
                {
                    TrackEvent(activeAdUnit, TrackingType.Fullscreen);
                }
            }
            else
            {
                foreach (var activeAdUnit in base.ActiveAdUnits)
                {
                    TrackEvent(activeAdUnit, TrackingType.ExitFullscreen);
                }
            }
        }

        public Task InitAdAsync(ActiveAdUnit ad, CancellationToken cancellationToken)
        {
            return base.InitAdAsync(ad, Player.Dimensions, Player.IsFullScreen, Player.CurrentBitrate, cancellationToken);
        }

        public Task StartAdAsync(ActiveAdUnit ad, CancellationToken cancellationToken)
        {
            return base.StartAdAsync(ad, Player.IsMuted ? 0 : Player.Volume, cancellationToken);
        }

        protected override string GetContentPlayhead()
        {
            return Player.CurrentPosition.ToString();
        }
    }

    /// <summary>
    /// Helps control the lifecyle of and track VPAID ads.
    /// </summary>
    internal class VpaidController
    {
        /// <summary>
        /// The ad has changed to linear mode.
        /// </summary>
        public event EventHandler<ActiveAdUnitEventArgs> AdIsLinear;

        /// <summary>
        /// The ad has changed to nonlinear mode.
        /// </summary>
        public event EventHandler<ActiveAdUnitEventArgs> AdIsNotLinear;

        /// <summary>
        /// The ad successfully to loaded.
        /// </summary>
        public event EventHandler<ActiveAdUnitEventArgs> AdLoaded;

        /// <summary>
        /// The ad successfully to started.
        /// </summary>
        public event EventHandler<ActiveAdUnitEventArgs> AdStarted;

        /// <summary>
        /// The ad successfully to stopped.
        /// </summary>
        public event EventHandler<ActiveAdUnitEventArgs> AdStopped;

        /// <summary>
        /// The ad failed for some reason.
        /// </summary>
        public event EventHandler<ActiveAdUnitEventArgs> AdFailed;

        /// <summary>
        /// The ad was removed and destroyed.
        /// </summary>
        public event EventHandler<ActiveAdUnitEventArgs> AdRemoved;

        /// <summary>
        /// The ad was paused.
        /// </summary>
        public event EventHandler<ActiveAdUnitEventArgs> AdPaused;

        /// <summary>
        /// The ad was resumed from a pause.
        /// </summary>
        public event EventHandler<ActiveAdUnitEventArgs> AdPlaying;

        /// <summary>
        /// The end of the ad is approaching
        /// </summary>
        public event EventHandler<ActiveAdUnitEventArgs> AdApproachingEnd;

        /// <summary>
        /// The ad player is requesting to log something.
        /// </summary>
        public event EventHandler<ActiveAdUnitLogEventArgs> Log;

        /// <summary>
        /// An ad tracking event occurred.
        /// </summary>
        public event EventHandler<AdTrackingEventEventArgs> AdTrackingEventOccurred;

        /// <summary>
        /// The progress of the ad has changed.
        /// </summary>
        public event EventHandler<ActiveAdUnitEventArgs> AdProgressChanged;

        private readonly Dictionary<IVpaid, ActiveAdUnit> activeAds = new Dictionary<IVpaid, ActiveAdUnit>();
        private readonly Dictionary<ActiveAdUnit, IList<TrackingEvent>> trackedProgressEvents = new Dictionary<ActiveAdUnit, IList<TrackingEvent>>();
        private readonly Dictionary<ActiveAdUnit, IList<DateTime>> quartileHistory = new Dictionary<ActiveAdUnit, IList<DateTime>>();

        private static Version HandlerVersion = new Version("2.0");

        public IEnumerable<IVpaid> ActivePlayers
        {
            get
            {
                return activeAds.Select(a => a.Key);
            }
        }

        public IEnumerable<ActiveAdUnit> ActiveAdUnits
        {
            get
            {
                return activeAds.Select(a => a.Value);
            }
        }

        /// <summary>
        /// Loads a creative. This causes VPAID.InitAd to execute.
        /// </summary>
        /// <param name="ad">The creative to load</param>
        /// <param name="bitrate">The current bitrate of the player. This is passed onto the VPAID player which can use it to initialize itself.</param>
        /// <param name="userState">A user state associated with this ad. This will be included with the various events associated with this ad.</param>
        public async Task InitAdAsync(ActiveAdUnit ad, Size dimensions, bool isFullScreen, int desiredBitrate, CancellationToken cancellationToken)
        {
            try
            {
                await ad.Player.InitAdAsync(
                    dimensions.Width,
                    dimensions.Height,
                    isFullScreen ? "fullscreen" : "normal",
                    desiredBitrate,
                    ad.CreativeSource.MediaSource,
                    ad.CreativeSource.ExtraInfo,
                    cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
            catch (Exception ex)
            {
                OnLog(new ActiveAdUnitLogEventArgs(ad, "VPAID.AdInit Exception: " + ex.Message));
                throw ex;
            }
        }

        /// <summary>
        /// Tells the ad to start.
        /// </summary>
        /// <param name="ad">The ad creative that should start playing</param>
        /// <param name="userState">A user state associated with this ad. This will be included with the various events associated with this ad.</param>
        public async Task StartAdAsync(ActiveAdUnit ad, double defaultVolume, CancellationToken cancellationToken)
        {
            try
            {
                ad.Player.AdVolume = defaultVolume;
                await ad.Player.StartAdAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                OnLog(new ActiveAdUnitLogEventArgs(ad, "VPAID.StartAd Exception: " + ex.Message));
                throw ex;
            }
        }

        public async Task<bool> PlayAdAsync(ActiveAdUnit ad, CancellationToken cancellationToken)
        {
            try
            {
                return await ad.Player.PlayAdAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                OnLog(new ActiveAdUnitLogEventArgs(ad, "VPAID.StartAd Exception: " + ex.Message));
                throw ex;
            }
        }

        public async Task FinishAdAsync(ActiveAdUnit ad, CancellationToken cancellationToken)
        {
            try
            {
                await ad.Player.FinishAdAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                OnLog(new ActiveAdUnitLogEventArgs(ad, "VPAID.StartAd Exception: " + ex.Message));
                throw ex;
            }
        }

        /// <summary>
        /// Stops the ad.
        /// </summary>
        /// <param name="ad">The ad creative that should stop playing</param>
        /// <param name="userState">A user state associated with this ad. This will be included with the various events associated with this ad.</param>
        public async Task StopAdAsync(ActiveAdUnit ad, CancellationToken cancellationToken)
        {
            try
            {
                await ad.Player.StopAdAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                OnLog(new ActiveAdUnitLogEventArgs(ad, "VPAID.StopAd Exception: " + ex.Message));
                throw ex;
            }
        }


        public void AddAd(ActiveAdUnit ad)
        {
            if (activeAds.ContainsKey(ad.Player))
            {
                throw new Exception("Ad is already added");
            }

            activeAds.Add(ad.Player, ad);
            trackedProgressEvents.Add(ad, new List<TrackingEvent>());
            quartileHistory.Add(ad, new List<DateTime>());
            HookupPlayer(ad.Player);
        }

        /// <summary>
        /// Removes the ad.
        /// </summary>
        /// <param name="ad">The ad creative that should be removed.</param>
        public void RemoveAd(ActiveAdUnit ad)
        {
            UnhookPlayer(ad.Player);
            trackedProgressEvents.Remove(ad);
            quartileHistory.Remove(ad);
            activeAds.Remove(ad.Player);

            if (ad.Player is IDisposable)
            {
                try
                {
                    ((IDisposable)ad.Player).Dispose();
                }
                catch (Exception ex)
                {
                    OnLog(new ActiveAdUnitLogEventArgs(ad, "VPAID.Dispose Exception: " + ex.Message));
                }
            }

            if (AdRemoved != null)
                AdRemoved(this, new ActiveAdUnitEventArgs(ad));
        }

        private void HookupPlayer(IVpaid player)
        {
            player.AdLoaded += player_AdLoaded;
            player.AdStarted += player_AdStarted;
            player.AdLinearChanged += player_AdLinearChanged;
            player.AdLog += player_AdLog;
            player.AdUserAcceptInvitation += player_AdUserAcceptInvitation;
            player.AdUserClose += player_AdUserClose;
            player.AdExpandedChanged += player_AdExpandedChanged;
            player.AdPlaying += player_AdPlaying;
            player.AdPaused += player_AdPaused;
            player.AdVolumeChanged += player_AdVolumeChanged;
            player.AdClickThru += player_AdClickThru;
            player.AdError += player_AdError;
            player.AdImpression += player_AdImpression;
            player.AdVideoFirstQuartile += player_AdVideoFirstQuartile;
            player.AdVideoMidpoint += player_AdVideoMidpoint;
            player.AdVideoThirdQuartile += player_AdVideoThirdQuartile;
            player.AdVideoComplete += player_AdVideoComplete;
            player.AdRemainingTimeChange += player_AdRemainingTimeChange;
            player.AdStopped += player_AdStopped;
            if (player is IVpaid2)
            {
                var vpaid2 = player as IVpaid2;
                vpaid2.AdSkipped += vpaid2_AdSkipped;
                vpaid2.AdInteraction += vpaid2_AdInteraction;
            }
        }

        private void UnhookPlayer(IVpaid player)
        {
            player.AdLoaded -= player_AdLoaded;
            player.AdStarted -= player_AdStarted;
            player.AdLinearChanged -= player_AdLinearChanged;
            player.AdLog -= player_AdLog;
            player.AdUserAcceptInvitation -= player_AdUserAcceptInvitation;
            player.AdUserClose -= player_AdUserClose;
            player.AdExpandedChanged -= player_AdExpandedChanged;
            player.AdPlaying -= player_AdPlaying;
            player.AdPaused -= player_AdPaused;
            player.AdVolumeChanged -= player_AdVolumeChanged;
            player.AdClickThru -= player_AdClickThru;
            player.AdError -= player_AdError;
            player.AdImpression -= player_AdImpression;
            player.AdVideoFirstQuartile -= player_AdVideoFirstQuartile;
            player.AdVideoMidpoint -= player_AdVideoMidpoint;
            player.AdVideoThirdQuartile -= player_AdVideoThirdQuartile;
            player.AdVideoComplete -= player_AdVideoComplete;
            player.AdRemainingTimeChange -= player_AdRemainingTimeChange;
            player.AdStopped -= player_AdStopped;
            if (player is IVpaid2)
            {
                var vpaid2 = player as IVpaid2;
                vpaid2.AdSkipped -= vpaid2_AdSkipped;
                vpaid2.AdInteraction -= vpaid2_AdInteraction;
            }
        }

        /// <summary>
        /// Handshakes with the current player to test version compatibility.
        /// </summary>
        /// <param name="adPlayer">The player to handshake with.</param>
        /// <returns>Flag indicating whether the player's version is supported.</returns>
        public bool Handshake(IVpaid adPlayer)
        {
            // handshake with the ad player to make sure the version of VAST is OK
            string strPlayerVersion = null;
            try
            {
                strPlayerVersion = adPlayer.HandshakeVersion(HandlerVersion.ToString());
            }
            catch (Exception ex)
            {
                OnLog(new ActiveAdUnitLogEventArgs(null, "VPAID.HandshakeVersion Exception: " + ex.Message));
            }
            try
            {
                Version playerVersion = new Version(strPlayerVersion);
                return (playerVersion <= HandlerVersion);
            }
            catch
            {
                return false;
            }
        }

        #region Ad Player events

        void player_AdLog(object sender, VpaidMessageEventArgs e)
        {
            var activeAd = activeAds[sender as IVpaid];
            OnLog(new ActiveAdUnitLogEventArgs(activeAd, e.Message));
        }

        void player_AdExpandedChanged(object sender, object e)
        {
            var vp = sender as IVpaid;
            var adUnit = activeAds[vp];

            if (vp.AdExpanded)
                TrackEvent(adUnit, TrackingType.Expand);
            else
                TrackEvent(adUnit, TrackingType.Collapse);
        }

        void player_AdLinearChanged(object sender, object e)
        {
            var vp = sender as IVpaid;
            var activeAdUnit = activeAds[vp];
            bool isLinear;
            try
            {
                isLinear = vp.AdLinear;
            }
            catch (Exception ex)
            {
                OnLog(new ActiveAdUnitLogEventArgs(activeAdUnit, "VPAID.AdLinear Exception: " + ex.Message));
                return;
            }
            var args = new ActiveAdUnitEventArgs(activeAdUnit);
            if (isLinear)
            {
                if (AdIsLinear != null) AdIsLinear(this, args);
            }
            else
            {
                if (AdIsNotLinear != null) AdIsNotLinear(this, args);
            }
        }

        void player_AdUserClose(object sender, object e)
        {
            var vp = sender as IVpaid;
            var adUnit = activeAds[vp];
            TrackEvent(adUnit, TrackingType.Close);
        }

        void player_AdUserAcceptInvitation(object sender, object e)
        {
            var vp = sender as IVpaid;
            var adUnit = activeAds[vp];
            TrackEvent(adUnit, TrackingType.AcceptInvitation);
        }

        void player_AdPlaying(object sender, object e)
        {
            var vp = sender as IVpaid;
            var adUnit = activeAds[vp];
            TrackEvent(adUnit, TrackingType.Resume);
            if (AdPlaying != null)
            {
                var ad = activeAds[vp];
                AdPlaying(this, new ActiveAdUnitEventArgs(ad));
            }
        }

        void player_AdPaused(object sender, object e)
        {
            var vp = sender as IVpaid;
            var adUnit = activeAds[vp];
            TrackEvent(adUnit, TrackingType.Pause);
            if (AdPaused != null)
            {
                var ad = activeAds[vp];
                AdPaused(this, new ActiveAdUnitEventArgs(ad));
            }
        }

        void player_AdVolumeChanged(object sender, object e)
        {
            // nothing to do
        }

        void player_AdImpression(object sender, object e)
        {
            var vp = sender as IVpaid;
            var adUnit = activeAds[vp];
            TrackEvent(adUnit, TrackingType.CreativeView);
        }

        void player_AdVideoFirstQuartile(object sender, object e)
        {
            var vp = sender as IVpaid;
            var adUnit = activeAds[vp];
            TrackEvent(adUnit, TrackingType.FirstQuartile);
        }

        void player_AdVideoMidpoint(object sender, object e)
        {
            var vp = sender as IVpaid;
            var adUnit = activeAds[vp];
            TrackEvent(adUnit, TrackingType.Midpoint);
        }

        void player_AdVideoThirdQuartile(object sender, object e)
        {
            var vp = sender as IVpaid;
            var adUnit = activeAds[vp];

            TrackEvent(adUnit, TrackingType.ThirdQuartile);

            if (AdApproachingEnd != null)
            {
                var ad = activeAds[vp];
                AdApproachingEnd(this, new ActiveAdUnitEventArgs(ad));
            }
        }

        void player_AdVideoComplete(object sender, object e)
        {
            var vp = sender as IVpaid;
            var adUnit = activeAds[vp];
            TrackEvent(adUnit, TrackingType.Complete);
        }

        void player_AdRemainingTimeChange(object sender, object e)
        {
            var vp = sender as IVpaid;
            var activeAdUnit = activeAds[vp];
            var adSource = activeAdUnit.CreativeSource;

            if (AdProgressChanged != null)
            {
                var ad = activeAds[vp];
                AdProgressChanged(this, new ActiveAdUnitEventArgs(ad));
            }

            // track progress events. This is a VAST 3.0 feature and is only supported for VPAID 2.0 players because AdDuration property is needed.
            var vp2 = sender as IVpaid2;
            if (vp2 != null)
            {
                TimeSpan timeEllapsed = vp2.AdDuration.Subtract(vp2.AdRemainingTime);
                double percentCompleted = timeEllapsed.TotalSeconds / vp2.AdDuration.TotalSeconds;
                TrackProgress(activeAdUnit, timeEllapsed, percentCompleted);
            }
        }

        private void TrackProgress(ActiveAdUnit activeAdUnit, TimeSpan timeEllapsed, double percentCompleted)
        {
            var trackedEvents = trackedProgressEvents[activeAdUnit];
            var eligableTrackingEvents = activeAdUnit.CreativeSource.TrackingEvents.Where(te => te.Type == TrackingType.Progress && te.Offset != null)
                .Except(trackedEvents)
                .Where(t => t.Offset.IsAbsolute ? timeEllapsed >= t.Offset.AbsoluteOffset : percentCompleted >= t.Offset.RelativeOffset);
            foreach (var trackingEvent in eligableTrackingEvents.ToList())
            {
                TrackUrl(trackingEvent.Value, activeAdUnit.CreativeSource);
                trackedEvents.Add(trackingEvent);
            }
        }

        protected void TrackEvent(ActiveAdUnit adUnit, TrackingType eventToTrack)
        {
            if (AdTrackingEventOccurred != null) 
                AdTrackingEventOccurred(this, new AdTrackingEventEventArgs(adUnit.CreativeSource, eventToTrack));
            
            foreach (var trackingEvent in adUnit.CreativeSource.TrackingEvents.Where(te => te.Type == eventToTrack))
            {
                var url = trackingEvent.Value;
                switch (trackingEvent.Type)
                {
                    case TrackingType.FirstQuartile:
                    case TrackingType.Midpoint:
                    case TrackingType.ThirdQuartile:
                    case TrackingType.Complete:
                        var previousEvents = quartileHistory[adUnit];
                        var currentTime = DateTime.Now;
                        url = url.Replace(Macro_PreviousQuartile, previousEvents.Any() ? ((int)Math.Round(currentTime.Subtract(previousEvents.Last()).TotalSeconds)).ToString() : "0");
                        previousEvents.Add(currentTime);
                        break;
                }
                TrackUrl(url, adUnit.CreativeSource);
            }
        }

        internal void TrackUrl(string url, ICreativeSource creativeSource = null)
        {
            if (url != null)
            {
                var resolvedUrl = GetMacroUrl(url);
                if (creativeSource != null)
                {
                    resolvedUrl = resolvedUrl.Replace(Macro_AssetUri, System.Net.WebUtility.UrlEncode(creativeSource.MediaSource));
                }

                AdTracking.Current.FireTracking(resolvedUrl);
            }
        }

        internal void TrackErrorUrl(string url, int errorCode, ICreativeSource creativeSource = null)
        {
            if (url != null)
            {
                TrackUrl(url.Replace(Macro_ErrorCode, errorCode.ToString()), creativeSource);
            }
        }

        void player_AdClickThru(object sender, ClickThroughEventArgs e)
        {
            var vp = sender as IVpaid;
            var adSource = activeAds[vp].CreativeSource;
            foreach (var url in adSource.ClickTracking)
            {
                TrackUrl(url, adSource);
            }
        }

        void player_AdError(object sender, VpaidMessageEventArgs e)
        {
            var vp = sender as IVpaid;
            var ad = activeAds[vp];

            if (AdFailed != null)
                AdFailed(this, new ActiveAdUnitEventArgs(ad));
        }

        void player_AdLoaded(object sender, object args)
        {
            var vp = sender as IVpaid;
            if (vp != null)
            {
                var ad = activeAds[vp];
                if (AdLoaded != null)
                    AdLoaded(this, new ActiveAdUnitEventArgs(ad));
            }
        }

        void player_AdStarted(object sender, object e)
        {
            var vp = sender as IVpaid;
            var adUnit = activeAds[vp];
            var adSource = adUnit.CreativeSource;
            if (AdStarted != null)
                AdStarted(this, new ActiveAdUnitEventArgs(adUnit));

            TrackEvent(adUnit, TrackingType.Start);
        }

        void player_AdStopped(object sender, object e)
        {
            var vp = sender as IVpaid;

            var ad = activeAds[vp];
            if (AdStopped != null)
                AdStopped(this, new ActiveAdUnitEventArgs(ad));
        }

        void vpaid2_AdInteraction(object sender, AdInteractionEventArgs e)
        {
            //var vp = sender as IVpaid;
            //var adSource = activeAds[vp].Source;
            //TrackEvent(adSource, TrackingType.interaction);
            //VAST does not have a tracking event for interaction
        }

        void vpaid2_AdSkipped(object sender, object e)
        {
            var vp = sender as IVpaid;
            var adUnit = activeAds[vp];
            TrackEvent(adUnit, TrackingType.Skip);
        }

        #endregion

        void OnLog(ActiveAdUnitLogEventArgs args)
        {
            if (Log != null) Log(this, args);
        }

        #region Tracking

        /// <summary>
        /// Replaced with one of the error codes listed in section 2.4.2.3 when the associated error occurs; reserved for error tracking URIs. Note: Currently not supported.
        /// </summary>
        const string Macro_ErrorCode = "[ERRORCODE]";
        /// <summary>
        /// Replaced with the current time offset "HH:MM:SS.mmm" of the video content. Note: Currently not supported.
        /// </summary>
        const string Macro_ContentPlayhead = "[CONTENTPLAYHEAD]";
        /// <summary>
        /// Replaced with a random 8-­‐digit number.
        /// </summary>
        const string Macro_CacheBusting = "[CACHEBUSTING]";
        /// <summary>
        /// Replaced with the URI of the ad asset being played. Note: Currently not supported.
        /// </summary>
        const string Macro_AssetUri = "[ASSETURI]";
        /// <summary>
        /// Replaced with the number of seconds since the previous quartile event.
        /// </summary>
        const string Macro_PreviousQuartile = "[LASTQUARTILE]";

        protected virtual string GetMacroUrl(string url)
        {
            return url
                .Replace(Macro_CacheBusting, GetCacheBuster())
                .Replace(Macro_ContentPlayhead, System.Net.WebUtility.UrlEncode(GetContentPlayhead()));
        }

        private static Random rnd = new Random();
        private static string GetCacheBuster()
        {
            return rnd.Next(100000000).ToString();
        }

        protected virtual string GetContentPlayhead()
        {
            return string.Empty;
        }

        /// <summary>
        /// No Ads VAST response after one or more Wrappers.
        /// </summary>
        public const int Error_NoAd = 303;

        /// <summary>
        /// Undefined error.
        /// </summary>
        public const int Error_Undefined = 900;

        /// <summary>
        /// General VPAID error.
        /// </summary>
        public const int Error_Vpaid = 901;

        #endregion
    }

}
