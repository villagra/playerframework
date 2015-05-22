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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Microsoft.PlayerFramework.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdPodPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public AdPodPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

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
