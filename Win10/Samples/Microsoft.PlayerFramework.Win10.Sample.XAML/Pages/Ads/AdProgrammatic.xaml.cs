using Microsoft.PlayerFramework.Advertising;
using Microsoft.VideoAdvertising;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Microsoft.PlayerFramework.Win10.Sample.XAML.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdProgrammatic : Page
    {
        public AdProgrammatic()
        {
            this.InitializeComponent();
            LoadAds();
        }

        private void LoadAds()
        {
            player.Containers.Add(TopBanner);
            player.Markers.Add(new TimelineMarker() { Time = TimeSpan.FromSeconds(5), Type = "myAd" });
            player.MarkerReached += MarkerReached;
        }

        private async void MarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
        {
            if (e.Marker.Type == "myAd")
            {
                //var adSource = new RemoteAdSource() { Type = VastAdPayloadHandler.AdType, Uri = new Uri("http://smf.blob.core.windows.net/samples/win8/ads/vast_linear.xml") };
                var adSource = new AdSource() { Type = DocumentAdPayloadHandler.AdType, Payload = SampleAdDocument };
                var progress = new Progress<AdStatus>();
                try
                {
                    await player.PlayAd(adSource, progress, CancellationToken.None);
                }
                catch { /* ignore */ }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
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
                companion.AdSlotId = "TopBanner";
                var staticResource = AdModelFactory.CreateStaticResource();
                staticResource.CreativeType = "image/jpg";
                staticResource.Value = new Uri("http://smf.blob.core.windows.net/samples/win8/ads/media/6.jpg");
                companion.Item = staticResource;
                companions.Companions.Add(companion);
                ad.Creatives.Add(companions);

                var nonlinears = AdModelFactory.CreateCreativeNonLinears();
                nonlinears.Sequence = 1;
                var nonlinear = AdModelFactory.CreateNonLinear();
                nonlinear.MinSuggestedDuration = TimeSpan.FromSeconds(3);
                var staticResource2 = AdModelFactory.CreateStaticResource();
                staticResource2.CreativeType = "image/jpg";
                staticResource2.Value = new Uri("http://smf.blob.core.windows.net/samples/win8/ads/media/1.jpg");
                nonlinear.Item = staticResource2;

                nonlinears.NonLinears.Add(nonlinear);
                ad.Creatives.Add(nonlinears);

                var linear = AdModelFactory.CreateCreativeLinear();
                linear.Sequence = 2;
                var media = AdModelFactory.CreateMediaFile();

                media.Type = "video/x-ms-wmv";
                media.Value = new Uri("http://smf.blob.core.windows.net/samples/win8/ads/media/XBOX_HD_DEMO_700_2_000_700_4x3.wmv");

                linear.MediaFiles.Add(media);
                ad.Creatives.Add(linear);

                adPod.Ads.Add(ad);
                result.AdPods.Add(adPod);
                return result;
            }
        }
    }
}
