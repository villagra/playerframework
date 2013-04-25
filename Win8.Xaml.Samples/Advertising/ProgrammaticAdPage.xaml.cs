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
using Microsoft.VideoAdvertising;
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
                var result = new AdDocumentPayload();
                var adPod = new AdPod();
                var ad = new Ad();

                var companions = new CreativeCompanions() { Sequence = 1 };
                var companion = new Companion();
                companion.AdSlotId = "banner";
                companion.Item = new StaticResource() { CreativeType = "image/jpg", Value = new Uri("http://smf.blob.core.windows.net/samples/win8/ads/media/6.jpg") };
                companions.Companions.Add(companion);
                ad.Creatives.Add(companions);

                var linear = new CreativeLinear() { Sequence = 1 };
                var media = new MediaFile();

                media.Type = "video/x-ms-wmv";
                media.Value = new Uri("http://smf.blob.core.windows.net/samples/win8/ads/media/XBOX_HD_DEMO_700_2_000_700_4x3.wmv");

                linear.MediaFiles.Add(media);
                ad.Creatives.Add(linear);

                var nonlinears = new CreativeNonLinears() { Sequence = 2 };
                var nonlinear = new NonLinear();
                nonlinear.MinSuggestedDuration = TimeSpan.FromSeconds(5);
                nonlinear.Item = new StaticResource()
                {
                    CreativeType = "image/jpg",
                    Value = new Uri("http://smf.blob.core.windows.net/samples/win8/ads/media/1.jpg")
                };

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
