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
using System.Windows.Threading;
#else
using Windows.UI.Xaml;
using Windows.System.Threading;
#endif

namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// A plugin that is capable of downloading a VMAP source file, parsing it and using it to schedule when ads should play.
    /// </summary>
    public class VmapSchedulerPlugin : AdSchedulerPlugin
    {
        readonly Dictionary<Advertisement, VmapAdBreak> adBreaks = new Dictionary<Advertisement, VmapAdBreak>();
        private CancellationTokenSource cts;
        private DispatcherTimer timer;

        /// <summary>
        /// Creates a new instance of VmapSchedulerPlugin
        /// </summary>
        public VmapSchedulerPlugin()
        {
            PollingInterval = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Gets or sets the amount of time to check the server for updated data. Only applies when MediaPlayer.IsLive = true
        /// </summary>
        public TimeSpan PollingInterval { get; set; }

        /// <summary>
        /// Identifies the Source dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(VmapSchedulerPlugin), null);

        /// <summary>
        /// Gets or sets the source Uri of the VMAP file
        /// </summary>
        public Uri Source
        {
            get { return GetValue(SourceProperty) as Uri; }
            set { SetValue(SourceProperty, value); }
        }

        /// <inheritdoc /> 
        protected override void OnUpdate()
        {
            adBreaks.Clear();
            Source = VmapScheduler.GetSource((DependencyObject)CurrentMediaSource);
            base.OnUpdate();
        }

        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            cts = new CancellationTokenSource();
            WirePlayer();
            return base.OnActivate();
        }

        /// <inheritdoc /> 
        protected override void OnDeactivate()
        {
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
            MediaPlayer.IsLiveChanged += MediaPlayer_IsLiveChanged;
            if (MediaPlayer.IsLive) InitializeTimer();
        }

        private void UnwirePlayer()
        {
#if !WINDOWS_PHONE7
            MediaPlayer.MediaLoading -= mediaPlayer_MediaLoading;
#endif
            MediaPlayer.IsLiveChanged -= MediaPlayer_IsLiveChanged;
            ShutdownTimer();
        }

        void MediaPlayer_IsLiveChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (MediaPlayer.IsLive)
            {
                InitializeTimer();
            }
            else
            {
                ShutdownTimer();
            }
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = PollingInterval;
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void ShutdownTimer()
        {
            if (timer != null)
            {
                timer.Tick -= timer_Tick;
                if (timer.IsEnabled) timer.Stop();
                timer = null;
            }
        }

        async void timer_Tick(object sender, object e)
        {
            try
            {
#if SILVERLIGHT
                var vmap = await VmapFactory.LoadSource(Source, cts.Token);
#else
                var vmap = await VmapFactory.LoadSource(Source).AsTask(cts.Token);
#endif
                // remove all ads that were not found new info
                foreach (var adBreak in adBreaks.Where(existingBreak => !vmap.AdBreaks.Any(newBreak => newBreak.BreakId == existingBreak.Value.BreakId)))
                {
                    Advertisements.Remove(adBreak.Key);
                }
                // create new ads for those that do not already exist
                foreach (var adBreak in vmap.AdBreaks.Where(newBreak => !adBreaks.Values.Any(existingBreak => existingBreak.BreakId == newBreak.BreakId)))
                {
                    CreateAdvertisement(adBreak);
                }
            }
            catch { /* ignore */ }
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
        /// <param name="source">The VMAP source URI</param>
        /// <param name="c">A cancellation token that allows you to cancel a pending operation</param>
        /// <returns>A task to await on.</returns>
        public async Task LoadAds(Uri source, CancellationToken c)
        {
            adBreaks.Clear();
            var cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(c, cts.Token).Token;
#if SILVERLIGHT
            var vmap = await VmapFactory.LoadSource(source, cancellationToken);
#else
            var vmap = await VmapFactory.LoadSource(source).AsTask(cancellationToken);
#endif
            foreach (var adBreak in vmap.AdBreaks)
            {
                CreateAdvertisement(adBreak);
            }
        }

        private void CreateAdvertisement(VmapAdBreak adBreak)
        {
            Advertisement ad = null;
            switch (adBreak.TimeOffset)
            {
                case "start":
                    ad = new PrerollAdvertisement();
                    break;
                case "end":
                    ad = new PostrollAdvertisement();
                    break;
                default:
                    var offset = FlexibleOffset.Parse(adBreak.TimeOffset);
                    if (offset != null)
                    {
                        var midroll = new MidrollAdvertisement();
                        if (!offset.IsAbsolute)
                        {
                            midroll.TimePercentage = offset.RelativeOffset;
                        }
                        else
                        {
                            midroll.Time = offset.AbsoluteOffset;
                        }
                        ad = midroll;
                    }
                    break;
            }

            if (ad != null)
            {
                ad.Source = GetAdSource(adBreak.AdSource);
                if (ad.Source != null)
                {
                    Advertisements.Add(ad);
                    adBreaks.Add(ad, adBreak);
                }
            }
        }

        /// <inheritdoc /> 
        protected override async Task PlayAdAsync(Advertisement ad, CancellationToken cancellationToken)
        {
            if (adBreaks.ContainsKey(ad)) // app could have manually added ads besides those in vmap
            {
                VmapAdBreak adBreak = adBreaks[ad];
                try
                {
                    TrackEvent(adBreak.TrackingEvents.Where(te => te.EventType == VmapTrackingEventType.BreakStart));
                    await base.PlayAdAsync(ad, cancellationToken);
                    TrackEvent(adBreak.TrackingEvents.Where(te => te.EventType == VmapTrackingEventType.BreakEnd));
                }
                catch
                {
                    TrackEvent(adBreak.TrackingEvents.Where(te => te.EventType == VmapTrackingEventType.Error));
                }
            }
            else
            {
                await base.PlayAdAsync(ad, cancellationToken);
            }
        }

        static void TrackEvent(IEnumerable<VmapTrackingEvent> events)
        {
            foreach (var trackingUri in events.Select(e => e.TrackingUri))
            {
                AdTracking.Current.FireTrackingUri(trackingUri);
            }
        }

        /// <summary>
        /// Creates an IAdSource from a VMAP AdSource (required for the AdHandlerPlugin to play the ad).
        /// </summary>
        /// <param name="source">The VMAP AdSource object</param>
        /// <returns>An IAdSource object that can be played by the AdHandlerPlugin. Returns null if insufficient data is available.</returns>
        public static IAdSource GetAdSource(VmapAdSource source)
        {
            IAdSource result = null;
            if (!string.IsNullOrEmpty(source.VastData))
            {
                result = new AdSource(source.VastData, VastAdPayloadHandler.AdType);
            }
            else if (!string.IsNullOrEmpty(source.CustomAdData))
            {
                result = new AdSource(source.CustomAdData, source.CustomAdDataTemplateType);
            }
            else if (source.AdTag != null)
            {
                result = new RemoteAdSource(source.AdTag, source.AdTagTemplateType);
            }

            if (result != null)
            {
                result.AllowMultipleAds = source.AllowMultipleAds;
                result.MaxRedirectDepth = source.FollowsRedirect ? new int?() : 0;
            }

            return result;
        }
    }
}
