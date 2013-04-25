using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Microsoft.VideoAdvertising;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.PlayerFramework.Advertising
{
#if MEF
    //[System.ComponentModel.Composition.PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    //[System.ComponentModel.Composition.Export(typeof(IPlugin))]
#endif
    /// <summary>
    /// The primary plugin used to help schedule ads.
    /// Internally, this plugin calls AdHandlerPlugin when it is time to actually play or preload an ad.
    /// </summary>
    public partial class AdSchedulerPlugin : PluginBase
    {
        /// <summary>
        /// The TimelineMarker ID used to store when ads should play.
        /// </summary>
        public const string MarkerType_Play = "Advertisement.Play";

        /// <summary>
        /// The TimelineMarker ID used to store when ads should start loading.
        /// </summary>
        public const string MarkerType_Preload = "Advertisement.Preload";

        ObservableCollection<Advertisement> advertisements;
        PreloadOperation activePreloadOperation;
        IList<Advertisement> handledAds;
        private CancellationTokenSource cts;
#if WINDOWS_PHONE7
        static TimeSpan? DefaultPreloadTime = null;
#else
        static TimeSpan? DefaultPreloadTime = TimeSpan.FromSeconds(5);
#endif

        /// <summary>
        /// Creates a new instance of AdSchedulerPlugin
        /// </summary>
        public AdSchedulerPlugin()
        {
            Advertisements = new ObservableCollection<Advertisement>();
            HandledAds = new ObservableCollection<Advertisement>();
        }

        /// <inheritdoc /> 
        protected override void OnUpdate()
        {
            Advertisements.Clear();
            HandledAds.Clear();
            Advertisements = AdScheduler.GetAdvertisements((DependencyObject)CurrentMediaSource);
            base.OnUpdate();
        }

        /// <inheritdoc /> 
        protected override void OnUnload()
        {
            Advertisements = null;
            HandledAds = null;
            base.OnUnload();
        }

        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            cts = new CancellationTokenSource();
            WireMediaPlayer();
            return true;
        }

        /// <inheritdoc /> 
        protected override void OnDeactivate()
        {
            cts.Cancel();
            cts = null;
            UnwireMediaPlayer();
        }

        async Task SetOrReplaceActivePreloadOperation(PreloadOperation value)
        {
            if (activePreloadOperation != null)
            {
                await activePreloadOperation.CancelAsync();
            }
            activePreloadOperation = value;
        }

        /// <summary>
        /// Gets a list of ads that have been handled already. You can remove from this collection to reactivate an ad and make it eligible again.
        /// </summary>
        public IList<Advertisement> HandledAds
        {
            get { return handledAds; }
            private set { handledAds = value; }
        }

        /// <summary>
        /// Identifies the EvaluateOnForwardOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty EvaluateOnForwardOnlyProperty = DependencyProperty.Register("EvaluateOnForwardOnly", typeof(bool), typeof(AdSchedulerPlugin), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether seeking or scrubbing back in time can trigger ads. Set to false to allow ads to be played when seeking backwards. Default is true.
        /// </summary>
        public bool EvaluateOnForwardOnly
        {
            get { return (bool)GetValue(EvaluateOnForwardOnlyProperty); }
            set { SetValue(EvaluateOnForwardOnlyProperty, value); }
        }

        /// <summary>
        /// Identifies the SyncToAdPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty SeekToAdPositionProperty = DependencyProperty.Register("SeekToAdPosition", typeof(bool), typeof(AdSchedulerPlugin), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether the position should be set set to that of the ad when an ad is scrubbed or seeked over. Default is true.
        /// </summary>
        public bool SeekToAdPosition
        {
            get { return (bool)GetValue(SeekToAdPositionProperty); }
            set { SetValue(SeekToAdPositionProperty, value); }
        }

        /// <summary>
        /// Identifies the InterruptScrub dependency property.
        /// </summary>
        public static readonly DependencyProperty InterruptScrubProperty = DependencyProperty.Register("InterruptScrub", typeof(bool), typeof(AdSchedulerPlugin), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether or not scrubbing is interrupted if an ad is encountered. Default is true.
        /// </summary>
        public bool InterruptScrub
        {
            get { return (bool)GetValue(InterruptScrubProperty); }
            set { SetValue(InterruptScrubProperty, value); }
        }

        /// <summary>
        /// Identifies the PreloadTime dependency property.
        /// </summary>
        public static readonly DependencyProperty PreloadTimeProperty = DependencyProperty.Register("PreloadTime", typeof(TimeSpan?), typeof(AdSchedulerPlugin), new PropertyMetadata(DefaultPreloadTime));

        /// <summary>
        /// Gets or sets the amount of time before an ad will occur that preloading will begin. Set to null to disable preloading. Default is 5 seconds.
        /// </summary>
        public TimeSpan? PreloadTime
        {
            get { return (TimeSpan?)GetValue(PreloadTimeProperty); }
            set { SetValue(PreloadTimeProperty, value); }
        }

        /// <summary>
        /// Provides the list of ads to schedule. You can add or remove ads to/from this collection during playback.
        /// </summary>
        public ObservableCollection<Advertisement> Advertisements
        {
            get { return advertisements; }
            set
            {
                if (advertisements != null)
                {
                    advertisements.Clear();
                    advertisements.CollectionChanged -= Advertisements_CollectionChanged;
                }
                advertisements = value;
            }
        }

        void InitializeAdvertisements()
        {
            if (advertisements != null)
            {
                foreach (var item in advertisements.OfType<MidrollAdvertisement>())
                {
                    AddMarker(item);
                }
                advertisements.CollectionChanged += Advertisements_CollectionChanged;
            }
        }

        async void Advertisements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var tasks = new List<Task>();
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems.OfType<MidrollAdvertisement>())
                {
                    var marker = MediaPlayer.Markers.FirstOrDefault(t => (t.Type == MarkerType_Play || t.Type == MarkerType_Preload) && t.Text == item.Id);
                    if (marker != null)
                    {
                        MediaPlayer.Markers.Remove(marker);
                    }
                    if (activePreloadOperation != null && activePreloadOperation.AdSource == item.Source)
                    {
                        // no need to wait
                        tasks.Add(activePreloadOperation.CancelAsync());
                    }
                }
            }
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems.OfType<MidrollAdvertisement>())
                {
                    AddMarker(item);
                }
            }
#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
            await TaskEx.WhenAll(tasks);
#else
            await Task.WhenAll(tasks);
#endif
        }

        void AddMarker(MidrollAdvertisement ad)
        {
            if (ad.TimePercentage.HasValue)
            {
                ad.Time = TimeSpan.FromSeconds(ad.TimePercentage.Value * MediaPlayer.Duration.TotalSeconds);
            }

            MediaPlayer.Markers.Add(new TimelineMarker() { Type = MarkerType_Play, Text = ad.Id, Time = ad.Time });
            if (PreloadTime.HasValue)
            {
                MediaPlayer.Markers.Add(new TimelineMarker() { Type = MarkerType_Preload, Text = ad.Id, Time = TimeSpanExtensions.Max(TimeSpan.FromMilliseconds(1), ad.Time.Subtract(PreloadTime.Value)) }); // clamp to 1 ms
            }
        }

        void UnwireMediaPlayer()
        {
#if WINDOWS_PHONE7
            MediaPlayer.MediaLoading -= MediaPlayer_MediaLoading;
#else
            MediaPlayer.MediaStarting -= MediaPlayer_MediaStarting;
#endif
            MediaPlayer.MarkerReached -= MediaPlayer_MarkerReached;
            MediaPlayer.MediaEnding -= MediaPlayer_MediaEnding;
            MediaPlayer.Seeked -= MediaPlayer_Seeked;
            MediaPlayer.ScrubbingStarted -= MediaPlayer_ScrubbingStarted;
            MediaPlayer.ScrubbingCompleted -= MediaPlayer_ScrubbingCompleted;
            MediaPlayer.Scrubbing -= MediaPlayer_Scrubbing;
            MediaPlayer.MediaOpened -= MediaPlayer_MediaOpened;
        }

        void WireMediaPlayer()
        {
#if WINDOWS_PHONE7
            MediaPlayer.MediaLoading += MediaPlayer_MediaLoading;
#else
            MediaPlayer.MediaStarting += MediaPlayer_MediaStarting;
#endif
            MediaPlayer.MarkerReached += MediaPlayer_MarkerReached;
            MediaPlayer.MediaEnding += MediaPlayer_MediaEnding;
            MediaPlayer.Seeked += MediaPlayer_Seeked;
            MediaPlayer.ScrubbingStarted += MediaPlayer_ScrubbingStarted;
            MediaPlayer.ScrubbingCompleted += MediaPlayer_ScrubbingCompleted;
            MediaPlayer.Scrubbing += MediaPlayer_Scrubbing;
            MediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
        }

        void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            HandledAds.Clear();
            InitializeAdvertisements();

            OnMediaOpened();

            if (PreloadTime.HasValue)
            {
                // schedule the first postroll for preloading (x seconds before the duration)
                var postroll = Advertisements.FirstOrDefault(a => a is PostrollAdvertisement);
                if (postroll != null)
                {
                    MediaPlayer.Markers.Add(new TimelineMarker() { Type = MarkerType_Preload, Text = postroll.Id, Time = TimeSpanExtensions.Max(TimeSpan.FromMilliseconds(1), MediaPlayer.Duration.Subtract(PreloadTime.Value)) }); // clamp to 1 ms
                }
            }
        }

        /// <summary>
        /// Called when the media has opened.
        /// Responsible for skipping ads before the startup position.
        /// </summary>
        protected virtual void OnMediaOpened()
        {
            // remove all ads before the start position
            if (MediaPlayer.StartupPosition.HasValue)
            {
                HandledAds.AddRange(Advertisements.OfType<PrerollAdvertisement>().Cast<Advertisement>());
                HandledAds.AddRange(Advertisements.OfType<MidrollAdvertisement>().Where(a => a.Time < MediaPlayer.StartupPosition.Value).Cast<Advertisement>());
            }
        }

        void MediaPlayer_Seeked(object sender, SeekRoutedEventArgs e)
        {
            e.Canceled = EvaluateMarkers(e.PreviousPosition, e.Position);
        }

        async void MediaPlayer_ScrubbingStarted(object sender, ScrubRoutedEventArgs e)
        {
            if (activePreloadOperation != null)
            {
                await activePreloadOperation.CancelAsync();
            }
        }

        void MediaPlayer_Scrubbing(object sender, ScrubProgressRoutedEventArgs e)
        {
            if (InterruptScrub)
            {
                e.Canceled = EvaluateMarkers(e.StartPosition, e.Position);
            }
        }

        void MediaPlayer_ScrubbingCompleted(object sender, ScrubProgressRoutedEventArgs e)
        {
            if (!e.Canceled)
            {
                e.Canceled = EvaluateMarkers(e.StartPosition, e.Position);
            }
        }

        async void MediaPlayer_MediaEnding(object sender, MediaPlayerDeferrableEventArgs e)
        {
            if (EligableAds.OfType<PostrollAdvertisement>().Any(a => a.Source != null))
            {
                var deferral = e.DeferrableOperation.GetDeferral();
                try
                {
                    await PlayAdsOfType<PostrollAdvertisement>(CancellationTokenSource.CreateLinkedTokenSource(deferral.CancellationToken, cts.Token).Token);
                }
                catch { /* ignore */ }
                finally
                {
                    deferral.Complete();
                }
            }
        }

        /// <summary>
        /// Evaluates all markers in a window and plays an ad if applicable.
        /// </summary>
        /// <param name="originalPosition">The window start position.</param>
        /// <param name="newPosition">The window end position. Note: This can be before originalPosition if going backwards.</param>
        /// <returns>A boolean indicating that an ad was played.</returns>
        public bool EvaluateMarkers(TimeSpan originalPosition, TimeSpan newPosition)
        {
            if (!EvaluateOnForwardOnly || newPosition > originalPosition)
            {
                foreach (var marker in MediaPlayer.Markers.Where(m => m.Type == MarkerType_Play).ToList())
                {
                    if (marker.Time <= newPosition && marker.Time > originalPosition)
                    {
                        var ad = EligableAds.OfType<MidrollAdvertisement>().Where(a => a.Source != null).FirstOrDefault(a => a.Id == marker.Text);
                        if (ad != null)
                        {
                            var syncToPosition = SeekToAdPosition ? marker.Time.Add(ad.Duration) : newPosition;
                            PlayAd(ad, syncToPosition);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

#if WINDOWS_PHONE7
        async void MediaPlayer_MediaLoading(object sender, MediaPlayerDeferrableEventArgs e)
#else
        async void MediaPlayer_MediaStarting(object sender, MediaPlayerDeferrableEventArgs e)
#endif
        {
            if (MediaPlayer.AllowMediaStartingDeferrals)
            {
                IList<Advertisement> startupAds;
                if (MediaPlayer.StartupPosition.HasValue)
                {
                    startupAds = EligableAds
                        .OfType<MidrollAdvertisement>()
                        .Where(a => a.Source != null && a.Time == MediaPlayer.StartupPosition.Value)
                        .Cast<Advertisement>()
                        .ToList();
                }
                else
                {
                    startupAds = EligableAds
                        .OfType<PrerollAdvertisement>()
                        .Where(a => a.Source != null)
                        .Cast<Advertisement>()
                        .ToList();
                }


                if (startupAds.Any())
                {
                    var deferral = e.DeferrableOperation.GetDeferral();
                    try
                    {
                        using (var adCts = CancellationTokenSource.CreateLinkedTokenSource(deferral.CancellationToken, cts.Token))
                        {
                            HandledAds.AddRange(startupAds);
                            await PlayAds(startupAds, adCts.Token);
                        }
                    }
                    catch { /* ignore */ }
                    finally
                    {
                        deferral.Complete();
                    }
                }
            }
        }

        void MediaPlayer_MarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
        {
            if (e.Marker.Type == MarkerType_Play)
            {
                PlayAdMarker(e.Marker);
            }
            else if (e.Marker.Type == MarkerType_Preload)
            {
                PreloadAdMarker(e.Marker);
            }
        }

        async Task PlayAdsOfType<T>(CancellationToken cancellationToken) where T : Advertisement
        {
            var adsToPlay = EligableAds.OfType<T>().Where(a => a.Source != null).ToList();
            HandledAds.AddRange(adsToPlay.Cast<Advertisement>());
            if (adsToPlay.Any())
            {
                await PlayAds(adsToPlay.Cast<Advertisement>(), cancellationToken);
            }
        }

        async Task PlayAds(IEnumerable<Advertisement> advertisements, CancellationToken cancellationToken)
        {
            foreach (var advertisement in advertisements)
            {
                try
                {
                    await PlayAdAsync(advertisement, cancellationToken);
                }
                catch { /* swallow */ }
                if (cancellationToken.IsCancellationRequested) break;
            }
        }

        async void PreloadAdMarker(TimelineMarker marker)
        {
            var ad = EligableAds.OfType<Advertisement>().Where(a => a.Source != null).FirstOrDefault(a => a.Id == marker.Text);
            if (ad != null)
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var task = MediaPlayer.PreloadAd(ad.Source, cancellationTokenSource.Token);
                if (task != null)
                {
                    await SetOrReplaceActivePreloadOperation(new PreloadOperation(ad.Source, task, cancellationTokenSource));
                }
            }
        }

        bool PlayAdMarker(TimelineMarker marker)
        {
            var ad = Advertisements.OfType<MidrollAdvertisement>().Where(a => a.Source != null).FirstOrDefault(a => a.Id == marker.Text);
            if (ad != null)
            {
                if (EligableAds.Contains(ad))
                {
                    TimeSpan? syncToPosition = null;
                    if (ad.Duration > TimeSpan.Zero)
                    {
                        syncToPosition = ad.Time.Add(ad.Duration);
                    }
                    PlayAd(ad, syncToPosition);
                    return true;
                }
                else if (ad.Duration > TimeSpan.Zero)
                {
                    SyncMainContent(ad.Time.Add(ad.Duration));
                }
            }
            return false;
        }

        void PlayAd(Advertisement ad, TimeSpan? syncToPosition)
        {
            HandledAds.Add(ad);
            PlayAdAsync(ad, cts.Token);
            if (syncToPosition.HasValue)
            {
                SyncMainContent(syncToPosition.Value);
            }
        }

        /// <summary>
        /// Called right when an ad starts to force the position of the MediaPlayer.
        /// This is useful for making sure the main content resumes immediately at the ad or where ever they were trying to seek to.
        /// Position synced is determined by the SeekToAdPosition property.
        /// </summary>
        /// <param name="position"></param>
        protected virtual void SyncMainContent(TimeSpan position)
        {
            MediaPlayer.Position = position;
        }

        /// <summary>
        /// Plays an advertisement.
        /// </summary>
        /// <param name="ad">The Advertisement to play.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task to await for.</returns>
        protected virtual Task PlayAdAsync(Advertisement ad, CancellationToken cancellationToken)
        {
            var progress = new Progress<AdStatus>();
            return MediaPlayer.PlayAd(ad.Source, progress, cancellationToken);
        }

        IEnumerable<Advertisement> EligableAds
        {
            get { return Advertisements.Except(HandledAds); }
        }

        class PreloadOperation : ActiveOperation
        {
            public PreloadOperation(IAdSource adSource, Task task, CancellationTokenSource cancellationTokenSource)
                : base(task, cancellationTokenSource)
            {
                AdSource = adSource;
            }

            public IAdSource AdSource { get; private set; }
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
    }
}
