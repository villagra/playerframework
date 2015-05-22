using Microsoft.PlayerFramework;
using Microsoft.PlayerFramework.Advertising;
using Microsoft.Media.Advertising;
using Microsoft.PlayerFramework.Samples.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Microsoft.PlayerFramework.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProgrammaticAdPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private AdHandlerPlugin adHandler;

        public ProgrammaticAdPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            adHandler = new AdHandlerPlugin();
            player.Plugins.Add(new AdHandlerPlugin());
            player.Markers.Add(new TimelineMarker() { Time = TimeSpan.FromSeconds(5), Type = "myAd" });
            player.MarkerReached += pf_MarkerReached;
        }

        async void pf_MarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
        {
            if (e.Marker.Type == "myAd")
            {
                var adSource = new RemoteAdSource() { Type = VastAdPayloadHandler.AdType, Uri = new Uri("http://smf.blob.core.windows.net/samples/win8/ads/vast_linear.xml") };
                //var adSource = new AdSource() { Type = DocumentAdPayloadHandler.AdType, Payload = SampleAdDocument };
                var progress = new Progress<AdStatus>();
                try
                {
                    await player.PlayAd(adSource, progress, CancellationToken.None);
                }
                catch { /* ignore */ }
            }
        }

        static AdDocumentPayload SampleAdDocument
        {
            get
            {
                var result = AdModelFactory.CreateAdDocumentPayload();
                var adPod = AdModelFactory.CreateAdPod();
                var ad = AdModelFactory.CreateAd();

                var companions = AdModelFactory.CreateCreativeCompanions();
                companions.Sequence = 1;
                var companion = AdModelFactory.CreateCompanion();
                companion.AdSlotId = "banner";
                var staticResource = AdModelFactory.CreateStaticResource();
                staticResource.CreativeType = "image/jpg";
                staticResource.Value = new Uri("http://smf.blob.core.windows.net/samples/win8/ads/media/6.jpg");
                companion.Item = staticResource;
                companions.Companions.Add(companion);
                ad.Creatives.Add(companions);

                var linear = AdModelFactory.CreateCreativeLinear();
                linear.Sequence = 1;
                var media = AdModelFactory.CreateMediaFile();

                media.Type = "video/x-ms-wmv";
                media.Value = new Uri("http://smf.blob.core.windows.net/samples/win8/ads/media/XBOX_HD_DEMO_700_2_000_700_4x3.wmv");

                linear.MediaFiles.Add(media);
                ad.Creatives.Add(linear);

                var nonlinears = AdModelFactory.CreateCreativeNonLinears();
                nonlinears.Sequence = 2;
                var nonlinear = AdModelFactory.CreateNonLinear();
                nonlinear.MinSuggestedDuration = TimeSpan.FromSeconds(5);
                var staticResource2 = AdModelFactory.CreateStaticResource();
                staticResource2.CreativeType = "image/jpg";
                staticResource2.Value = new Uri("http://smf.blob.core.windows.net/samples/win8/ads/media/1.jpg");
                nonlinear.Item = staticResource2;

                nonlinears.NonLinears.Add(nonlinear);
                ad.Creatives.Add(nonlinears);

                adPod.Ads.Add(ad);
                result.AdPods.Add(adPod);
                return result;
            }
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

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
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            Windows.Graphics.Display.DisplayInformation.AutoRotationPreferences = Windows.Graphics.Display.DisplayOrientations.Landscape;
            var noawait = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            player.Dispose();
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
