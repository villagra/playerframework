//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using Windows.Foundation;
using Microsoft.Media.PlayReadyClient;

namespace Microsoft.PlayerFramework.Xaml.DashDemo
{
    public class RequestChain
    {
        public Uri LicenseRequestUri { get; set; }

        protected IPlayReadyServiceRequest _serviceRequest = null;
        ReportResultDelegate _reportResult = null;

        IndivAndReportResult _indivAndReportResult = null;
        LAAndReportResult _licenseAcquisition = null;
        public ServiceRequestConfigData RequestConfigData { get; set; }

        public RequestChain(IPlayReadyServiceRequest serviceRequest)
        {
            _serviceRequest = serviceRequest;
        }

        public void FinishAndReportResult(ReportResultDelegate callback)
        {
            _reportResult = callback;
            HandleServiceRequest();
        }

        void HandleServiceRequest()
        {
            if (_serviceRequest is PlayReadyIndividualizationServiceRequest)
            {
                HandleIndivServiceRequest((PlayReadyIndividualizationServiceRequest)_serviceRequest);
            }
            else if (_serviceRequest is PlayReadyLicenseAcquisitionServiceRequest)
            {
                HandleLicenseAcquisitionServiceRequest((PlayReadyLicenseAcquisitionServiceRequest)_serviceRequest);
            }
            else
            {
                //TestLogger.LogError("ERROR: Unsupported serviceRequest " + _serviceRequest.GetType());
            }
        }

        void HandleServiceRequest_Finished(bool bResult)
        {
            _reportResult(bResult);
        }

        void HandleIndivServiceRequest(PlayReadyIndividualizationServiceRequest serviceRequest)
        {
            _indivAndReportResult = new IndivAndReportResult(new ReportResultDelegate(HandleServiceRequest_Finished));
            _indivAndReportResult.RequestConfigData = RequestConfigData;
            _indivAndReportResult.IndivReactively(serviceRequest);
        }

        void HandleLicenseAcquisitionServiceRequest(PlayReadyLicenseAcquisitionServiceRequest serviceRequest)
        {
            _licenseAcquisition = new LAAndReportResult(new ReportResultDelegate(HandleServiceRequest_Finished));
            _licenseAcquisition.LicenseRequestUri = LicenseRequestUri;
            _licenseAcquisition.RequestConfigData = RequestConfigData;
            _licenseAcquisition.AcquireLicenseReactively(serviceRequest);
        }
    }
}
