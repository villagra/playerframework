using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Media.PlayReadyClient;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Protection;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.PlayerFramework.Samples.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Microsoft.PlayerFramework.Samples
{
    /// <summary>
    /// WARNING: you must set your Build Configuration to AnyCPU to play this demo.
    /// </summary>
    public sealed partial class PlayReadyPage : Page
    {
        const string LAURL = "http://playready.directtaps.net/win/rightsmanager.asmx";

        MediaProtectionServiceCompletion _serviceCompletionNotifier = null;
        RequestChain _requestChain = null;
        ServiceRequestConfigData _requestConfigData = null;


        public PlayReadyPage()
        {
            this.InitializeComponent();

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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            player.Dispose();
            base.OnNavigatedFrom(e);
        }
    }
}
