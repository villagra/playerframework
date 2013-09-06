using System;
using System.Linq;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VideoAdvertising;
using System.Collections.Generic;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// A plugin that is capable of downloading a FreeWheel SmartXML source file, parsing it and using it to schedule when ads should play.
    /// </summary>
    public class FreeWheelPlugin : AdSchedulerPlugin
    {
        readonly Dictionary<Advertisement, FWTemporalAdSlot> adSlots = new Dictionary<Advertisement, FWTemporalAdSlot>();
        private CancellationTokenSource cts;
        private FWAdResponse adResponse;
        private bool trackingEnded;
        private PlayTimeTrackingEvent lastTrackingEvent;
        const string TrackingEventArea = "FreeWheel";

        /// <summary>
        /// Identifies the Source dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(FreeWheelPlugin), null);

        /// <summary>
        /// Gets or sets the source Uri of the FreeWheel SmartXML file
        /// </summary>
        public Uri Source
        {
            get { return GetValue(SourceProperty) as Uri; }
            set { SetValue(SourceProperty, value); }
        }

        /// <inheritdoc /> 
        protected override void OnUpdate()
        {
            Source = FreeWheel.GetSource((DependencyObject)CurrentMediaSource);
            base.OnUpdate();
        }

        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            cts = new CancellationTokenSource();
            WirePlayer();
            if (adResponse != null) ShowCompanions();
            return base.OnActivate();
        }

        /// <inheritdoc /> 
        protected override void OnDeactivate()
        {
            MediaPlayer.UnloadAllCompanions();
            cts.Cancel();
            cts = null;
            UnwirePlayer();
            base.OnDeactivate();
        }

        private void WirePlayer()
        {
#if !WINDOWS_PHONE7
            MediaPlayer.MediaLoading += mediaPlayer_MediaLoading;
#endif
            MediaPlayer.MediaClosed += MediaPlayer_MediaClosed;
        }

        private void UnwirePlayer()
        {
#if !WINDOWS_PHONE7
            MediaPlayer.MediaLoading -= mediaPlayer_MediaLoading;
#endif
            MediaPlayer.MediaClosed -= MediaPlayer_MediaClosed;
        }

        private void trackingPlugin_EventTracked(object sender, EventTrackedEventArgs e)
        {
            if (e.TrackingEvent.Area == TrackingEventArea)
            {
                var eventCallback = e.TrackingEvent.Data as FWEventCallback;
                if (eventCallback != null)
                {
                    foreach (var url in eventCallback.GetUrls())
                    {
                        TrackVideoViewMarker(url, e);
                    }
                }
            }
        }

        private void TrackVideoViewMarker(string url, EventTrackedEventArgs e)
        {
            if (!trackingEnded)
            {
                bool isStart;
                bool isEnd;
                TimeSpan currentPlayTime;
                if (e.TrackingEvent is PositionTrackingEvent)
                {
                    var playTimeTrackingPlugin = MediaPlayer.Plugins.OfType<PlayTimeTrackingPlugin>().FirstOrDefault();
                    if (playTimeTrackingPlugin == null) throw new Exception("PlayTimeTrackingPlugin not found; required for FreeWheelPlugin");
                    var positionTrackingEvent = (PositionTrackingEvent)e.TrackingEvent;
                    if (!positionTrackingEvent.PositionPercentage.HasValue || positionTrackingEvent.PositionPercentage.Value != 1) throw new Exception("Invalid tracking event was registered for FreeWheelPlugin");
                    isStart = false;
                    isEnd = true;
                    currentPlayTime = playTimeTrackingPlugin.PlayTime;
                    trackingEnded = true; // set this flag to prevent further tracking
                }
                else if (e.TrackingEvent is PlayTimeTrackingEvent)
                {
                    var playTimeTrackingEvent = (PlayTimeTrackingEvent)e.TrackingEvent;
                    isStart = playTimeTrackingEvent.PlayTime == TimeSpan.Zero;
                    isEnd = false;
                    currentPlayTime = playTimeTrackingEvent.PlayTime;
                }
                else
                {
                    throw new ArgumentException();
                }

                TimeSpan delta;
                if (lastTrackingEvent != null)
                {
                    delta = currentPlayTime - lastTrackingEvent.PlayTime;
                }
                else
                {
                    delta = currentPlayTime;
                }
                lastTrackingEvent = e.TrackingEvent as PlayTimeTrackingEvent;

                var newUrl = url + string.Format("{3}init={0}&ct={1}&last={2}", isStart ? 1 : 0, (int)Math.Round(delta.TotalSeconds), isEnd ? 1 : 0, url.Contains("?") ? "&" : "?");
                AdTracking.Current.FireTracking(newUrl);
            }
        }

#if WINDOWS_PHONE7
        protected override async void MediaPlayer_MediaLoading(object sender, MediaPlayerDeferrableEventArgs e)
#else
        async void mediaPlayer_MediaLoading(object sender, MediaPlayerDeferrableEventArgs e)
#endif
        {
            if (IsEnabled)
            {
                if (Source != null)
                {
                    var deferral = e.DeferrableOperation.GetDeferral();
                    try
                    {
                        await LoadAds(Source, deferral.CancellationToken);
#if WINDOWS_PHONE7
                        await base.PlayStartupAds(deferral.CancellationToken);
#endif
                    }
                    catch { /* ignore */ }
                    finally
                    {
                        deferral.Complete();
                    }
                }
            }
        }

        /// <summary>
        /// Loads ads from a source URI. Note, this is called automatically if you set the source before the MediaLoading event fires and normally does not need to be called.
        /// </summary>
        /// <param name="source">The SmartXML source URI</param>
        /// <param name="c">A cancellation token that allows you to cancel a pending operation</param>
        /// <returns>A task to await on.</returns>
        public async Task LoadAds(Uri source, CancellationToken c)
        {
            adSlots.Clear();
            var cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(c, cts.Token).Token;

#if SILVERLIGHT
            adResponse = await FreeWheelFactory.LoadSource(source, cancellationToken);
#else
            adResponse = await FreeWheelFactory.LoadSource(source).AsTask(cancellationToken);
#endif

            var videoTracking = adResponse.SiteSection.VideoPlayer.VideoAsset.EventCallbacks.FirstOrDefault(ec => ec.Name == FWEventCallback.VideoView);
            if (videoTracking != null)
            {
                // use the tracking plugins to help with tracking markers. Create it if it doesn't exist.
                var positionTrackingPlugin = MediaPlayer.Plugins.OfType<PositionTrackingPlugin>().FirstOrDefault();
                if (positionTrackingPlugin == null)
                {
                    positionTrackingPlugin = new PositionTrackingPlugin();
                    MediaPlayer.Plugins.Add(positionTrackingPlugin);
                }
                positionTrackingPlugin.EventTracked += trackingPlugin_EventTracked;
                lastTrackingEvent = null; // reset
                trackingEnded = false;
                positionTrackingPlugin.TrackingEvents.Add(new PositionTrackingEvent() { PositionPercentage = 1, Data = videoTracking, Area = TrackingEventArea });

                var playTimeTrackingPlugin = MediaPlayer.Plugins.OfType<PlayTimeTrackingPlugin>().FirstOrDefault();
                if (playTimeTrackingPlugin == null)
                {
                    playTimeTrackingPlugin = new PlayTimeTrackingPlugin();
                    MediaPlayer.Plugins.Add(playTimeTrackingPlugin);
                }
                playTimeTrackingPlugin.EventTracked += trackingPlugin_EventTracked;
                for (int i = 0; i < 60; i = i + 15)
                    playTimeTrackingPlugin.TrackingEvents.Add(new PlayTimeTrackingEvent() { PlayTime = TimeSpan.FromSeconds(i), Data = videoTracking, Area = TrackingEventArea });
                for (int i = 60; i < 60 * 3; i = i + 30)
                    playTimeTrackingPlugin.TrackingEvents.Add(new PlayTimeTrackingEvent() { PlayTime = TimeSpan.FromSeconds(i), Data = videoTracking, Area = TrackingEventArea });
                for (int i = 60 * 3; i < 60 * 10; i = i + 60)
                    playTimeTrackingPlugin.TrackingEvents.Add(new PlayTimeTrackingEvent() { PlayTime = TimeSpan.FromSeconds(i), Data = videoTracking, Area = TrackingEventArea });
                for (int i = 60 * 10; i < 60 * 30; i = i + 120)
                    playTimeTrackingPlugin.TrackingEvents.Add(new PlayTimeTrackingEvent() { PlayTime = TimeSpan.FromSeconds(i), Data = videoTracking, Area = TrackingEventArea });
                for (int i = 60 * 30; i < 60 * 60; i = i + 300)
                    playTimeTrackingPlugin.TrackingEvents.Add(new PlayTimeTrackingEvent() { PlayTime = TimeSpan.FromSeconds(i), Data = videoTracking, Area = TrackingEventArea });
                for (int i = 60 * 60; i < 60 * 180; i = i + 600)
                    playTimeTrackingPlugin.TrackingEvents.Add(new PlayTimeTrackingEvent() { PlayTime = TimeSpan.FromSeconds(i), Data = videoTracking, Area = TrackingEventArea });
            }

            var videoAsset = adResponse.SiteSection.VideoPlayer.VideoAsset;
            if (videoAsset != null)
            {
                foreach (var adSlot in videoAsset.AdSlots)
                {
                    Advertisement ad = null;
                    switch (adSlot.TimePositionClass)
                    {
                        case "preroll":
                            ad = new PrerollAdvertisement();
                            break;
                        case "postroll":
                            ad = new PostrollAdvertisement();
                            break;
                        default:
                            var midroll = new MidrollAdvertisement();
                            midroll.Time = adSlot.TimePosition;
                            ad = midroll;
                            break;
                    }

#if SILVERLIGHT
                    var payload = await FreeWheelFactory.GetAdDocumentPayload(adSlot, adResponse, cancellationToken);
#else
                    var payload = await FreeWheelFactory.GetAdDocumentPayload(adSlot, adResponse).AsTask(cancellationToken);
#endif
                    ad.Source = new AdSource(payload, DocumentAdPayloadHandler.AdType);

                    Advertisements.Add(ad);
                    adSlots.Add(ad, adSlot);
                }
            }

            ShowCompanions();
        }

        private void MediaPlayer_MediaClosed(object sender, RoutedEventArgs e)
        {
            var playTimeTrackingPlugin = MediaPlayer.Plugins.OfType<PlayTimeTrackingPlugin>().FirstOrDefault();
            if (playTimeTrackingPlugin != null)
            {
                playTimeTrackingPlugin.EventTracked -= trackingPlugin_EventTracked;
            }

            var positionTrackingPlugin = MediaPlayer.Plugins.OfType<PlayTimeTrackingPlugin>().FirstOrDefault();
            if (positionTrackingPlugin != null)
            {
                positionTrackingPlugin.EventTracked -= trackingPlugin_EventTracked;
            }

            adSlots.Clear();
        }

        private void ShowCompanions()
        {
            foreach (var companionCreative in FreeWheelFactory.GetNonTemporalCompanions(adResponse))
            {
                MediaPlayer.ShowCompanion(companionCreative);
            }
        }

        /// <inheritdoc /> 
        protected override async Task PlayAdAsync(Advertisement ad, CancellationToken cancellationToken)
        {
            if (adSlots.ContainsKey(ad)) // app could have manually added ads besides those from FreeWheel
            {
                var adSlot = adSlots[ad];
                try
                {
                    var slotImpression = adSlot.EventCallbacks.FirstOrDefault(ec => ec.Type == FWCallbackType.Impression && ec.Name == FWEventCallback.SlotImpression);
                    if (slotImpression != null)
                    {
                        foreach (var url in slotImpression.GetUrls())
                        {
                            AdTracking.Current.FireTracking(url);
                        }
                    }
                    if (ad.Source != null)
                    {
                        await base.PlayAdAsync(ad, cancellationToken);
                    }
                }
                catch { /* swallow */ }
            }
            else
            {
                await base.PlayAdAsync(ad, cancellationToken);
            }
        }

    }
}
