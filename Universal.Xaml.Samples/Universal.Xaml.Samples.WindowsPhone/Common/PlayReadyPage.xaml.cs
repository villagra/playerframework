using Microsoft.Media.PlayReadyClient;
using Microsoft.PlayerFramework.Samples.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media.Protection;
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
    public sealed partial class PlayReadyPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        const string LAURL = "http://playready.directtaps.net/win/rightsmanager.asmx";

        MediaProtectionServiceCompletion _serviceCompletionNotifier = null;
        RequestChain _requestChain = null;
        ServiceRequestConfigData _requestConfigData = null;

        public PlayReadyPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            var protectionManager = new MediaProtectionManager();
            protectionManager.ComponentLoadFailed += ProtectionManager_ComponentLoadFailed;
            protectionManager.ServiceRequested += ProtectionManager_ServiceRequested;

            Windows.Foundation.Collections.PropertySet cpSystems = new Windows.Foundation.Collections.PropertySet();
            cpSystems.Add("{F4637010-03C3-42CD-B932-B48ADF3A6A54}", "Microsoft.Media.PlayReadyClient.PlayReadyWinRTTrustedInput"); //Playready
            protectionManager.Properties.Add("Windows.Media.Protection.MediaProtectionSystemIdMapping", cpSystems);
            protectionManager.Properties.Add("Windows.Media.Protection.MediaProtectionSystemId", "{F4637010-03C3-42CD-B932-B48ADF3A6A54}");

            player.ProtectionManager = protectionManager;

            var extensions = player.MediaExtensionManager;
            extensions.RegisterByteStreamHandler("Microsoft.Media.PlayReadyClient.PlayReadyByteStreamHandler", ".pyv", "PRvideo");
            extensions.RegisterByteStreamHandler("Microsoft.Media.PlayReadyClient.PlayReadyByteStreamHandler", ".pya", "PRaudio");
            extensions.RegisterByteStreamHandler("Microsoft.Media.PlayReadyClient.PlayReadyByteStreamHandler", ".wma", "PRaudio");
            extensions.RegisterByteStreamHandler("Microsoft.Media.PlayReadyClient.PlayReadyByteStreamHandler", ".wmv", "PRvideo");
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
