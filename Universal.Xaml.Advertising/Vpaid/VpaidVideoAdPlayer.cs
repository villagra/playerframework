#if WINDOWS_PHONE
#define HACK_MARKERREACHED
#endif

using System;
using System.Linq;
using Microsoft.Media.Advertising;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// A VPAID implementation for a linear video ad.
    /// </summary>
    public class VpaidVideoAdPlayer : AdHost, IVpaid2
    {
        private DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(250) };
        private bool adCompleted;

        protected MediaElement mediaElement;
#if WINDOWS_PHONE
        private static bool IsActive;
        private Uri pendingMediaUri;
#endif

        const string Marker_SkippableOffset = "SkippableOffset";
        const string Marker_FirstQuartile = "FirstQuartile";
        const string Marker_Midpoint = "Midpoint";
        const string Marker_ThirdQuartile = "ThirdQuartile";
        const string Marker_DurationReached = "DurationReached";

        /// <summary>
        /// Gets the position in the ad at which the ad can be skipped. If null, the ad cannot be skipped.
        /// </summary>
        public FlexibleOffset SkippableOffset { get; private set; }

        /// <summary>
        /// Gets the max duration of the ad. If not specified, the length of the video is assumed.
        /// </summary>
        public TimeSpan? MaxDuration { get; private set; }

        /// <summary>
        /// Creates a new instance of VpaidVideoAdPlayer.
        /// </summary>
        /// <param name="skippableOffset">The position in the ad at which the ad can be skipped. If null, the ad cannot be skipped.</param>
        /// <param name="maxDuration">The max duration of the ad. If not specified, the length of the video is assumed.</param>
        /// <param name="clickThru">The Uri to navigate to when the ad is clicked or tapped. Can be null of no action should take place.</param>
        public VpaidVideoAdPlayer(FlexibleOffset skippableOffset, TimeSpan? maxDuration, Uri clickThru)
        {
            IsHitTestVisible = false;
            mediaElement = new MediaElement();
            Background = new SolidColorBrush(Colors.Black);
#if !WINDOWS80
            Opacity = 0.01; // HACK: Win8.1 won't load the video if opacity = 0
#else
            Opacity = 0;
#endif
            State = AdState.None;
            AdLinear = true;

            SkippableOffset = skippableOffset;
            MaxDuration = maxDuration;
            this.NavigateUri = clickThru;
        }

#if NETFX_CORE
        /// <summary>
        /// Gets or sets the AudioCategory on the underlying MediaElement
        /// </summary>
        public AudioCategory AudioCategory
        {
            get { return mediaElement.AudioCategory; }
            set { mediaElement.AudioCategory = value; }
        }
#endif

        /// <summary>
        /// Restarts the ad from the beginning. Note: any quartile events that have already fired will not fire again.
        /// </summary>
        public void Restart()
        {
            mediaElement.Position = TimeSpan.Zero;
        }

        /// <inheritdoc />
        public string HandshakeVersion(string version)
        {
            if (version.StartsWith("1."))
            {
                return version;
            }
            else
            {
                return "2.0";   // return the highest version of VPAID that we support
            }
        }

        /// <inheritdoc />
        public void InitAd(double width, double height, string viewMode, int desiredBitrate, string creativeData, string environmentVariables)
        {
            this.Content = mediaElement;
            mediaElement.MediaOpened += MediaElement_MediaOpened;
            mediaElement.MediaFailed += MediaElement_MediaFailed;
            State = AdState.Loading;
            adSkippableState = false;
            OnInitAd(creativeData);
        }

        protected virtual void OnInitAd(string creativeData)
        {
#if WINDOWS_PHONE
            if (!IsActive)
            {
                mediaElement.AutoPlay = false;
                IsActive = true;
                mediaElement.Source = new Uri(creativeData);
                pendingMediaUri = null;
            }
            else
            {
                pendingMediaUri = new Uri(creativeData);
                OnLoaded();
            }
#else
            mediaElement.AutoPlay = false;
            mediaElement.Source = new Uri(creativeData);
#endif
        }

        void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (State != AdState.Complete && State != AdState.Failed)
            {
                State = AdState.Failed;
                Teardown();
#if SILVERLIGHT
                if (AdError != null) AdError(this, new VpaidMessageEventArgs() { Message = e.ErrorException.Message });
#else
                if (AdError != null) AdError(this, new VpaidMessageEventArgs() { Message = e.ErrorMessage });
#endif
            }
        }

        void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            OnLoaded();

            if (SkippableOffset != null)
            {
                // create a marker for the skippable offset
                TimelineMarker skippableOffsetMarker = null;
                if (!SkippableOffset.IsAbsolute)
                {
                    skippableOffsetMarker = new TimelineMarker() { Time = TimeSpan.FromSeconds(AdDuration.TotalSeconds * SkippableOffset.RelativeOffset) };
                }
                else
                {
                    skippableOffsetMarker = new TimelineMarker() { Time = SkippableOffset.AbsoluteOffset };
                }
                if (skippableOffsetMarker != null)
                {
                    skippableOffsetMarker.Type = Marker_SkippableOffset;
                    mediaElement.Markers.Add(skippableOffsetMarker);
                }
            }

            adCompleted = false;
            // create markers for the quartile events
            mediaElement.Markers.Add(new TimelineMarker() { Type = Marker_FirstQuartile, Time = TimeSpan.FromSeconds(AdDuration.TotalSeconds * .25) });
            mediaElement.Markers.Add(new TimelineMarker() { Type = Marker_Midpoint, Time = TimeSpan.FromSeconds(AdDuration.TotalSeconds * .5) });
            mediaElement.Markers.Add(new TimelineMarker() { Type = Marker_ThirdQuartile, Time = TimeSpan.FromSeconds(AdDuration.TotalSeconds * .75) });

            if (MaxDuration.HasValue)
            {
                // create marker for max duration
                mediaElement.Markers.Add(new TimelineMarker() { Type = Marker_DurationReached, Time = MaxDuration.Value });
            }

#if !HACK_MARKERREACHED
            mediaElement.MarkerReached += MediaElement_MarkerReached;
#endif
        }

        protected void OnLoaded()
        {
            if (State == AdState.Loading)
            {
                State = AdState.Loaded;
                if (AdLoaded != null) AdLoaded(this, EventArgs.Empty);
            }
        }

#if SILVERLIGHT
        void timer_Tick(object sender, EventArgs e)
#else
        void timer_Tick(object sender, object e)
#endif
        {
            if (timer.IsEnabled)
            {
                if (AdRemainingTimeChange != null) AdRemainingTimeChange(this, EventArgs.Empty);
#if HACK_MARKERREACHED
                var now = mediaElement.Position;
                foreach (var marker in mediaElement.Markers.ToList())
                {
                    if (marker.Time <= now)
                    {
                        OnMarkerReached(marker);
                    }
                }
#endif
            }
        }

#if WINDOWS_PHONE
        void MediaElement_MarkerReached(object sender, System.Windows.Media.TimelineMarkerRoutedEventArgs e)
#else
        void MediaElement_MarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
#endif
        {
            OnMarkerReached(e.Marker);
        }

        private void OnMarkerReached(TimelineMarker marker)
        {
            switch (marker.Type)
            {
                case Marker_SkippableOffset:
                    AdSkippableState = true;
                    mediaElement.Markers.Remove(marker);
                    break;
                case Marker_FirstQuartile:
                    if (AdVideoFirstQuartile != null) AdVideoFirstQuartile(this, EventArgs.Empty);
                    mediaElement.Markers.Remove(marker);
                    break;
                case Marker_Midpoint:
                    if (AdVideoMidpoint != null) AdVideoMidpoint(this, EventArgs.Empty);
                    mediaElement.Markers.Remove(marker);
                    break;
                case Marker_ThirdQuartile:
                    if (AdVideoThirdQuartile != null) AdVideoThirdQuartile(this, EventArgs.Empty);
                    mediaElement.Markers.Remove(marker);
                    break;
                case Marker_DurationReached:
                    if (!adCompleted)
                    {
                        adCompleted = true;
                        if (AdVideoComplete != null) AdVideoComplete(this, EventArgs.Empty);
                    }
                    mediaElement.Markers.Remove(marker);
                    StopAd();
                    break;
                default:
                    mediaElement.Markers.Remove(marker);
                    break;
            }
        }

        /// <inheritdoc />
        public void StartAd()
        {
            State = AdState.Starting;
            mediaElement.CurrentStateChanged += MediaElement_CurrentStateChanged;
            mediaElement.MediaEnded += MediaElement_MediaEnded;

            OnStartAd();
        }

        protected virtual async void OnStartAd()
        {
            var startTask = GetStateChangedTask(MediaElementState.Playing);
#if WINDOWS_PHONE
            if (pendingMediaUri == null)
            {
                mediaElement.Play();
            }
            else
            {
                mediaElement.AutoPlay = true;
                IsActive = true;
                mediaElement.Source = pendingMediaUri;
                pendingMediaUri = null;
            }
#else
            mediaElement.Play();
#endif
            try
            {
                await startTask;
                OnAdStarted();
            }
            catch (OperationCanceledException) { /* teardown occured */ }
        }

        void AdPlayer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AdSizeChanged != null) AdSizeChanged(this, EventArgs.Empty);
        }

        void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (!adCompleted)
            {
                adCompleted = true;
                if (AdVideoComplete != null) AdVideoComplete(this, EventArgs.Empty);
            }
            OnAdEnding();
        }

        readonly Dictionary<MediaElementState, TaskCompletionSource<object>> stateChangedTasks = new Dictionary<MediaElementState, TaskCompletionSource<object>>();
        Task GetStateChangedTask(MediaElementState state)
        {
            if (stateChangedTasks.ContainsKey(state))
            {
                return stateChangedTasks[state].Task;
            }
            else
            {
                var result = new TaskCompletionSource<object>();
                stateChangedTasks.Add(state, result);
                return result.Task;
            }
        }

        void MediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (mediaElement.CurrentState == MediaElementState.Playing)
            {
                if (!timer.IsEnabled) timer.Start();
            }
            else if (timer.IsEnabled)
            {
                timer.Stop();
            }

            // completes the appropriate task created via GetStateChangedTask and removes it from the collection.
            if (stateChangedTasks.ContainsKey(mediaElement.CurrentState))
            {
                var tcs = stateChangedTasks[mediaElement.CurrentState];
                stateChangedTasks.Remove(mediaElement.CurrentState);
                tcs.TrySetResult(null);
            }
        }

        void AdPlayer_Navigated(object sender, RoutedEventArgs e)
        {
            var clickEventArgs = new ClickThroughEventArgs() { Url = NavigateUri.OriginalString };
            clickEventArgs.PlayerHandles = !(e.OriginalSource is HyperlinkButton);
            if (AdClickThru != null) AdClickThru(this, clickEventArgs);
        }

        private void OnAdStarted()
        {
            State = AdState.Playing;

            IsHitTestVisible = true;
            Opacity = 1;

            this.Navigated += AdPlayer_Navigated;
            this.SizeChanged += AdPlayer_SizeChanged;
            timer.Tick += timer_Tick;

            if (AdStarted != null) AdStarted(this, EventArgs.Empty);
            if (AdImpression != null) AdImpression(this, EventArgs.Empty);
            if (AdVideoStart != null) AdVideoStart(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public async void StopAd()
        {
            var stopTask = GetStateChangedTask(MediaElementState.Stopped);
            var closeTask = GetStateChangedTask(MediaElementState.Closed);
            mediaElement.Stop();
            try
            {
#if WINDOWS_PHONE7
                await TaskEx.WhenAny(stopTask, closeTask);
#else
                await Task.WhenAny(stopTask, closeTask);
#endif
                OnAdEnding();
            }
            catch (OperationCanceledException) { /* teardown occured */ }
        }

        /// <summary>
        /// Called when the ad is ending for any reason besides a failure.
        /// </summary>
        protected void OnAdEnding()
        {
            if (State != AdState.Complete && State != AdState.Failed)
            {
                State = AdState.Complete;
                Teardown();
                if (AdStopped != null) AdStopped(this, EventArgs.Empty);
            }
        }

        private void Teardown()
        {
            this.Navigated -= AdPlayer_Navigated;
            this.SizeChanged -= AdPlayer_SizeChanged;
            timer.Tick -= timer_Tick;
            if (timer.IsEnabled) timer.Stop();
            timer = null;
            mediaElement.MediaOpened -= MediaElement_MediaOpened;
            mediaElement.MediaFailed -= MediaElement_MediaFailed;
            mediaElement.MediaEnded -= MediaElement_MediaEnded;
            mediaElement.CurrentStateChanged -= MediaElement_CurrentStateChanged;
#if !HACK_MARKERREACHED
            mediaElement.MarkerReached -= MediaElement_MarkerReached;
#endif
            // cancel any active tasks and remove them from the dictionary
            foreach (var task in stateChangedTasks)
            {
                task.Value.TrySetCanceled();
            }
            stateChangedTasks.Clear();

            OnTeardown();
            this.Content = null;
            mediaElement = null;
            Opacity = 0;
        }

        protected virtual void OnTeardown()
        {
            mediaElement.Source = null;
#if WINDOWS_PHONE
            IsActive = false;
#endif
        }

        /// <inheritdoc />
        public void ResizeAd(double width, double height, string viewMode)
        {
            // normally we don't have to do anything since we rely on Xaml scaling to resize us automatically.
        }

        /// <inheritdoc />
        public async void PauseAd()
        {
            var pauseTask = GetStateChangedTask(MediaElementState.Paused);
            mediaElement.Pause();
            try
            {
                await pauseTask;
                if (State == AdState.Playing)
                {
                    State = AdState.Paused;
                    if (AdPaused != null) AdPaused(this, EventArgs.Empty);
                }
            }
            catch (OperationCanceledException) { /* teardown occured */ }
        }

        /// <inheritdoc />
        public async void ResumeAd()
        {
            var playTask = GetStateChangedTask(MediaElementState.Playing);
            mediaElement.Play();
            try
            {
                await playTask;
                if (State == AdState.Paused)
                {
                    State = AdState.Playing;
                    if (AdPlaying != null) AdPlaying(this, EventArgs.Empty);
                }
            }
            catch (OperationCanceledException) { /* teardown occured */ }
        }

        /// <inheritdoc />
        public void ExpandAd()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void CollapseAd()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool AdExpanded
        {
            get { return true; }
        }

        /// <inheritdoc />
        public TimeSpan AdRemainingTime
        {
            get { return mediaElement != null ? AdDuration.Subtract(mediaElement.Position) : TimeSpan.Zero; }
        }

        /// <inheritdoc />
        public double AdVolume
        {
            get
            {
                return mediaElement.Volume;
            }
            set
            {
                mediaElement.Volume = value;
                if (AdVolumeChanged != null) AdVolumeChanged(this, EventArgs.Empty);
            }
        }

#pragma warning disable 0067

#if SILVERLIGHT
        /// <inheritdoc />
        public event EventHandler AdLoaded;
        /// <inheritdoc />
        public event EventHandler AdStarted;
        /// <inheritdoc />
        public event EventHandler AdStopped;
        /// <inheritdoc />
        public event EventHandler AdPaused;
        /// <inheritdoc />
        public event EventHandler AdPlaying;
        /// <inheritdoc />
        public event EventHandler AdExpandedChanged;
        /// <inheritdoc />
        public event EventHandler AdLinearChanged;
        /// <inheritdoc />
        public event EventHandler AdVolumeChanged;
        /// <inheritdoc />
        public event EventHandler AdVideoStart;
        /// <inheritdoc />
        public event EventHandler AdVideoFirstQuartile;
        /// <inheritdoc />
        public event EventHandler AdVideoMidpoint;
        /// <inheritdoc />
        public event EventHandler AdVideoThirdQuartile;
        /// <inheritdoc />
        public event EventHandler AdVideoComplete;
        /// <inheritdoc />
        public event EventHandler AdUserAcceptInvitation;
        /// <inheritdoc />
        public event EventHandler AdUserClose;
        /// <inheritdoc />
        public event EventHandler AdUserMinimize;
        /// <inheritdoc />
        public event EventHandler AdRemainingTimeChange;
        /// <inheritdoc />
        public event EventHandler AdImpression;
#else
        /// <inheritdoc />
        public event EventHandler<object> AdLoaded;
        /// <inheritdoc />
        public event EventHandler<object> AdStarted;
        /// <inheritdoc />
        public event EventHandler<object> AdStopped;
        /// <inheritdoc />
        public event EventHandler<object> AdPaused;
        /// <inheritdoc />
        public event EventHandler<object> AdPlaying;
        /// <inheritdoc />
        public event EventHandler<object> AdExpandedChanged;
        /// <inheritdoc />
        public event EventHandler<object> AdLinearChanged;
        /// <inheritdoc />
        public event EventHandler<object> AdVolumeChanged;
        /// <inheritdoc />
        public event EventHandler<object> AdVideoStart;
        /// <inheritdoc />
        public event EventHandler<object> AdVideoFirstQuartile;
        /// <inheritdoc />
        public event EventHandler<object> AdVideoMidpoint;
        /// <inheritdoc />
        public event EventHandler<object> AdVideoThirdQuartile;
        /// <inheritdoc />
        public event EventHandler<object> AdVideoComplete;
        /// <inheritdoc />
        public event EventHandler<object> AdUserAcceptInvitation;
        /// <inheritdoc />
        public event EventHandler<object> AdUserClose;
        /// <inheritdoc />
        public event EventHandler<object> AdUserMinimize;
        /// <inheritdoc />
        public event EventHandler<object> AdRemainingTimeChange;
        /// <inheritdoc />
        public event EventHandler<object> AdImpression;
#endif
        /// <inheritdoc />
        public event EventHandler<ClickThroughEventArgs> AdClickThru;
        /// <inheritdoc />
        public event EventHandler<VpaidMessageEventArgs> AdError;
        /// <inheritdoc />
        public event EventHandler<VpaidMessageEventArgs> AdLog;

        // VPAID 2.0
#if SILVERLIGHT
        /// <inheritdoc />
        public event EventHandler AdSkipped;
        /// <inheritdoc />
        public event EventHandler AdSizeChanged;
        /// <inheritdoc />
        public event EventHandler AdSkippableStateChange;
        /// <inheritdoc />
        public event EventHandler AdDurationChange;
#else
        /// <inheritdoc />
        public event EventHandler<object> AdSkipped;
        /// <inheritdoc />
        public event EventHandler<object> AdSizeChanged;
        /// <inheritdoc />
        public event EventHandler<object> AdSkippableStateChange;
        /// <inheritdoc />
        public event EventHandler<object> AdDurationChange;
#endif
        /// <inheritdoc />
        public event EventHandler<AdInteractionEventArgs> AdInteraction; // TODO: raise this event once VPAID 2.0 defines the correct IDs to pass.

#pragma warning restore 0067

        private AdState State { get; set; }

        private enum AdState
        {
            None,
            Loading,
            Loaded,
            Starting,
            Playing,
            Paused,
            Complete,
            Failed
        }

        /// <inheritdoc />
        public void SkipAd()
        {
            if (AdSkippableState)
            {
                if (AdSkipped != null) AdSkipped(this, EventArgs.Empty);
                OnAdEnding();
            }
        }

        /// <inheritdoc />
        public double AdWidth
        {
            get { return mediaElement.NaturalVideoWidth; }
        }

        /// <inheritdoc />
        public double AdHeight
        {
            get { return mediaElement.NaturalVideoHeight; }
        }

        bool adSkippableState;
        /// <inheritdoc />
        public bool AdSkippableState
        {
            get { return adSkippableState; }
            protected set
            {
                if (adSkippableState != value)
                {
                    adSkippableState = value;
                    if (AdSkippableStateChange != null) AdSkippableStateChange(this, EventArgs.Empty);
                }
            }
        }

        /// <inheritdoc />
        public TimeSpan AdDuration
        {
            get { return MaxDuration.GetValueOrDefault(mediaElement.NaturalDuration.TimeSpan); }
        }

        /// <inheritdoc />
        public string AdCompanions
        {
            get { return string.Empty; }
        }

        /// <inheritdoc />
        public bool AdIcons
        {
            get { return false; }
        }
    }
}
