using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.PlayerFramework.Advertising;
using Microsoft.Media.Advertising;
using System.Threading;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Microsoft.PlayerFramework.Samples.Advertising
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProgrammaticAdPage : Microsoft.PlayerFramework.Samples.Common.LayoutAwarePage
    {
        AdHandlerPlugin adHandler;

        public ProgrammaticAdPage()
        {
            this.InitializeComponent();
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            player.Dispose();
            base.OnNavigatedFrom(e);
        }
    }
}
