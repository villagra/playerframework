using Microsoft.AdaptiveStreaming;
using Microsoft.AdaptiveStreaming.Dash;
using Microsoft.Media.AdaptiveStreaming;
using Microsoft.PlayerFramework.Adaptive;
using Microsoft.PlayerFramework.Xaml.DashDemo.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

namespace Microsoft.PlayerFramework.Xaml.DashDemo
{
    /// <summary>
    /// A page that displays a group title, a list of items within the group, and details for the
    /// currently selected item.
    /// </summary>
    public sealed partial class SplitPage : Page
    {
        readonly ObservableCollection<string> RequestCollection = new ObservableCollection<string>();
        DashDownloaderPlugin dashDownloaderPlugin;
        AdaptivePlugin adaptivePlugin;
        StreamState CurrentStreamState = StreamState.Closed;
        Uri manifestUri;
        NavigationHelper navigationHelper;

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        enum StreamState
        { 
            Closed,
            Starting,
            Playing,
        }

        public SplitPage()
        {
            this.InitializeComponent();
            
            // Setup the navigation helper
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

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
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            player.Source = new Uri((string)e.NavigationParameter);
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
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

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
