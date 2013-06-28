using Microsoft.AdaptiveStreaming;
using Microsoft.AdaptiveStreaming.Dash;
using Microsoft.Media.AdaptiveStreaming;
using Microsoft.Media.PlayReadyClient;
using Microsoft.PlayerFramework.Adaptive;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Protection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

namespace Microsoft.PlayerFramework.Xaml.DashDemo
{
    /// <summary>
    /// A page that displays a group title, a list of items within the group, and details for the
    /// currently selected item.
    /// </summary>
    public sealed partial class SplitPage : Microsoft.PlayerFramework.Xaml.DashDemo.Common.LayoutAwarePage
    {
        readonly ObservableCollection<string> RequestCollection = new ObservableCollection<string>();
        DashDownloaderPlugin dashDownloaderPlugin;
        AdaptivePlugin adaptivePlugin;
        StreamState CurrentStreamState = StreamState.Closed;
        Uri manifestUri;

        #region PlayReady
        const string LAURL = "http://playready.directtaps.net/pr/svc/rightsmanager.asmx?PlayRight=1&UseSimpleNonPersistentLicense=1";

        MediaProtectionServiceCompletion _serviceCompletionNotifier = null;
        RequestChain _requestChain = null;
        ServiceRequestConfigData _requestConfigData = null;

        void InitPlayReady()
        {
            var protectionManager = new MediaProtectionManager();
            protectionManager.ComponentLoadFailed += ProtectionManager_ComponentLoadFailed;
            protectionManager.ServiceRequested += ProtectionManager_ServiceRequested;

            Windows.Foundation.Collections.PropertySet cpSystems = new Windows.Foundation.Collections.PropertySet();
            cpSystems.Add("{F4637010-03C3-42CD-B932-B48ADF3A6A54}", "Microsoft.Media.PlayReadyClient.PlayReadyWinRTTrustedInput"); //Playready
            protectionManager.Properties.Add("Windows.Media.Protection.MediaProtectionSystemIdMapping", cpSystems);
            protectionManager.Properties.Add("Windows.Media.Protection.MediaProtectionSystemId", "{F4637010-03C3-42CD-B932-B48ADF3A6A54}");

            player.ProtectionManager = protectionManager;
        }

        public ServiceRequestConfigData RequestConfigData
        {
            set { this._requestConfigData = value; }
            get { return this._requestConfigData; }
        }

        void ProtectionManager_ComponentLoadFailed(MediaProtectionManager sender, ComponentLoadFailedEventArgs e)
        {
            e.Completion.Complete(false);
        }

        void ProtectionManager_ServiceRequested(MediaProtectionManager sender, ServiceRequestedEventArgs srEvent)
        {
            _serviceCompletionNotifier = srEvent.Completion;
            IPlayReadyServiceRequest serviceRequest = (IPlayReadyServiceRequest)srEvent.Request;

            _requestChain = new RequestChain(serviceRequest);
            _requestChain.LicenseRequestUri = new Uri(LAURL);
            _requestChain.RequestConfigData = this.RequestConfigData;
            _requestChain.FinishAndReportResult(new ReportResultDelegate(HandleServiceRequest_Finished));
        }

        void HandleServiceRequest_Finished(bool bResult)
        {
            _serviceCompletionNotifier.Complete(bResult);
        }
        #endregion

        enum StreamState
        {
            Closed,
            Starting,
            Playing,
        }

        public SplitPage()
        {
            this.InitializeComponent();

            InitPlayReady();

            adaptivePlugin = new AdaptivePlugin();
            player.Plugins.Add(adaptivePlugin);

            dashDownloaderPlugin = new DashDownloaderPlugin();
            adaptivePlugin.DownloaderPlugin = dashDownloaderPlugin;
            dashDownloaderPlugin.ChunkRequested += dashDownloaderPlugin_ChunkRequested;
            dashDownloaderPlugin.ManifestRequested += dashDownloaderPlugin_ManifestRequested;

            adaptivePlugin.Manager.OpenedBackground += manager_Opened;
            adaptivePlugin.Manager.ClosedBackground += manager_Closed;

            player.IsFullScreenChanged += player_IsFullScreenChanged;
            Requests.ItemsSource = RequestCollection;
        }

        async void dashDownloaderPlugin_ManifestRequested(object sender, ManifestRequestedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                TextBoxManifest.Text = e.OriginalManifest;
            });
        }

        async void dashDownloaderPlugin_ChunkRequested(object sender, ChunkRequestedEventArgs e)
        {
            int i = 0;
            var manifestUrl = manifestUri.OriginalString.ToCharArray();
            var requestUrl = e.Source.OriginalString.ToCharArray();
            while (manifestUrl[i] == requestUrl[i])
            {
                i++;
            }
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (RequestCollection.Count > 1000) RequestCollection.RemoveAt(1000);
                RequestCollection.Insert(0, e.Source.OriginalString.Substring(i));
            });
        }

        void player_IsFullScreenChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            BottomPanel.Visibility = e.NewValue ? Visibility.Collapsed : Visibility.Visible;
            LeftPanel.Visibility = e.NewValue ? Visibility.Collapsed : Visibility.Visible;
            titlePanel.Visibility = e.NewValue ? Visibility.Collapsed : Visibility.Visible;
        }

        void manager_Opened(object sender, object e)
        {
            var manager = sender as AdaptiveStreamingManager;
            manager.AdaptiveSrcManager.ManifestReadyEvent += AdaptiveSrcManager_ManifestReadyEvent;
            manager.AdaptiveSrcManager.AdaptiveSourceStatusUpdatedEvent += AdaptiveSrcManager_AdaptiveSourceStatusUpdatedEvent;
        }

        void manager_Closed(object sender, object e)
        {
            var manager = sender as AdaptiveStreamingManager;
            manager.AdaptiveSrcManager.ManifestReadyEvent -= AdaptiveSrcManager_ManifestReadyEvent;
            manager.AdaptiveSrcManager.AdaptiveSourceStatusUpdatedEvent -= AdaptiveSrcManager_AdaptiveSourceStatusUpdatedEvent;
        }

        async void AdaptiveSrcManager_ManifestReadyEvent(Media.AdaptiveStreaming.AdaptiveSource sender, Media.AdaptiveStreaming.ManifestReadyEventArgs args)
        {
            CurrentStreamState = StreamState.Starting;
            manifestUri = args.AdaptiveSource.Uri;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var videoStream = args.AdaptiveSource.Manifest.SelectedStreams.Where(i => i.Type == MediaStreamType.Video).FirstOrDefault();
                if (videoStream != null)
                {
                    videoTracks.ItemsSource = videoStream.AvailableTracks;
                }

                var audioStreams = args.AdaptiveSource.Manifest.AvailableStreams.Where(i => i.Type == MediaStreamType.Audio);
                if (audioStreams != null)
                {
                    audioStreamsList.ItemsSource = audioStreams;

                    var selectedAudioStream = audioStreams.FirstOrDefault(a => a.Name == args.AdaptiveSource.Manifest.SelectedStreams.First(i => i.Type == MediaStreamType.Audio).Name);
                    audioStreamsList.SelectedItem = selectedAudioStream;
                }

                RequestCollection.Clear();
            });
        }

        async void AdaptiveSrcManager_AdaptiveSourceStatusUpdatedEvent(Media.AdaptiveStreaming.AdaptiveSource sender, Media.AdaptiveStreaming.AdaptiveSourceStatusUpdatedEventArgs args)
        {
            if (CurrentStreamState == StreamState.Starting)
            {
                CurrentStreamState = StreamState.Playing;
            }
            else if (CurrentStreamState == StreamState.Playing)
            {
                await Task.Delay(6000); // wait 6 seconds because this is actually the time the chunk was downloaded, not the time the chunk was played
            }
            if (CurrentStreamState == StreamState.Closed) return;

            switch (args.UpdateType)
            {
                case AdaptiveSourceStatusUpdateType.BitrateChanged:
                    var manifest = args.AdaptiveSource.Manifest;

                    var videoStream = manifest.SelectedStreams.Where(i => i.Type == MediaStreamType.Video).FirstOrDefault();
                    if (videoStream != null)
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            var bitrateInfo = args.AdditionalInfo.Split(';');
                            var newBitrate = uint.Parse(bitrateInfo[0]);
                            videoTracks.SelectedItem = ((IEnumerable<IManifestTrack>)videoTracks.ItemsSource).FirstOrDefault(t => t.Bitrate == newBitrate);
                        });
                    }

                    break;
            }
        }

        void Clear()
        {
            CurrentStreamState = StreamState.Closed;
            videoTracks.ItemsSource = null;
            videoTracks.SelectedItem = null;
            audioStreamsList.ItemsSource = null;
            audioStreamsList.SelectedItem = null;
            RequestCollection.Clear();
            TextBoxManifest.Text = "";
        }

        #region Page state management

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            player.Source = new Uri((string)navigationParameter);
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            Clear();

            dashDownloaderPlugin.ChunkRequested -= dashDownloaderPlugin_ChunkRequested;
            dashDownloaderPlugin.ManifestRequested -= dashDownloaderPlugin_ManifestRequested;

            adaptivePlugin.Manager.OpenedBackground -= manager_Opened;
            adaptivePlugin.Manager.ClosedBackground -= manager_Closed;

            player.IsFullScreenChanged -= player_IsFullScreenChanged;

            player.Dispose();
            adaptivePlugin = null;
            dashDownloaderPlugin = null;
        }

        #endregion
    }
}
