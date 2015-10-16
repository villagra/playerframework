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
using Microsoft.PlayerFramework.Samples.Common;
using Microsoft.PlayerFramework.Advertising;
using Microsoft.Media.Advertising;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Microsoft.PlayerFramework.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdPodPage : Page
    {
        public AdPodPage()
        {
            this.InitializeComponent();

            //TrackAdPodTimeRemaining();
        }

        #region VAST adpod time remaining tracking
        private void TrackAdPodTimeRemaining()
        {
            var adHandler = player.GetAdHandlerPlugin();
            adHandler.ActivateAdUnit += adHandler_ActivateAdUnit;
        }
        
        TimeSpan timeAfterAdRemaining;
        void adHandler_ActivateAdUnit(object sender, ActivateAdUnitEventArgs e)
        {
            var pendingCreatives = GetSubsequentCreatives(e.CreativeSource as IDocumentCreativeSource, e.CreativeConcept as Ad, e.AdSource.Payload as AdDocumentPayload);
            var pendingLinearCreatives = pendingCreatives.TakeWhile(c => c is CreativeLinear).Cast<CreativeLinear>();
            timeAfterAdRemaining = TimeSpan.FromSeconds(pendingLinearCreatives.Sum(lc => lc.Duration.GetValueOrDefault(TimeSpan.Zero).TotalSeconds));
            e.Player.AdRemainingTimeChange += Player_AdRemainingTimeChange;
        }

        void Player_AdRemainingTimeChange(object sender, object e)
        {
            var player = sender as IVpaid;
            var totalTimeRemaining = player.AdRemainingTime + timeAfterAdRemaining;
            System.Diagnostics.Debug.WriteLine(string.Format("{0:00} seconds before your video resumes", totalTimeRemaining.TotalSeconds));
        }

        private static IEnumerable<ICreative> GetSubsequentCreatives(IDocumentCreativeSource creativeSource, Ad creativeConcept, AdDocumentPayload payload)
        {
            if (creativeSource == null) throw new ArgumentNullException("creativeSource");
            if (payload == null) throw new ArgumentNullException("payload");
            if (creativeConcept == null) throw new ArgumentNullException("creativeConcept");

            var adPod = payload.AdPods.FirstOrDefault(ap => ap.Ads.Contains(creativeConcept));
            if (adPod != null)
            {
                return adPod.Ads
                    .SelectMany(a => a.Creatives.OrderBy(c => c.Sequence.GetValueOrDefault(int.MaxValue)))
                    .Where(c => !(c is CreativeCompanions))
                    .SkipWhile(c => c != creativeSource.Creative)
                    .Skip(1);
            }
            return null;
        }
        #endregion

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            player.Stop();
            var adPlugin = player.Plugins.FirstOrDefault(p => p.GetType() == typeof(AdHandlerPlugin));
            if (adPlugin != null)
            {
                adPlugin.MediaPlayer.Stop();
                adPlugin.MediaPlayer.Dispose();
            }
            player.Dispose();
            base.OnNavigatedFrom(e);
        }
    }
}
