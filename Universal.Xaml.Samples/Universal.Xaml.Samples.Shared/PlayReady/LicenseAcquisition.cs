//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using Windows.Foundation;
using Microsoft.Media.PlayReadyClient;

namespace Microsoft.PlayerFramework.Samples
{

    public class LicenseAcquisition : ServiceRequest
    {
        protected virtual void LAServiceRequestCompleted( PlayReadyLicenseAcquisitionServiceRequest  sender, Exception hrCompletionStatus )
        {
        }

        public void  AcquireLicenseProactively()
        {
            PlayReadyContentHeader contentHeader = new PlayReadyContentHeader(
                                                                                RequestConfigData.KeyId,
                                                                                RequestConfigData.KeyIdString,
                                                                                RequestConfigData.EncryptionAlgorithm,
                                                                                RequestConfigData.Uri,
                                                                                RequestConfigData.Uri,
                                                                                String.Empty, 
                                                                                RequestConfigData.DomainServiceId);
            
            PlayReadyLicenseAcquisitionServiceRequest licenseRequest = new PlayReadyLicenseAcquisitionServiceRequest();
            licenseRequest.ContentHeader = contentHeader;
            AcquireLicenseReactively( licenseRequest );
            
        }

        void ConfigureServiceRequest()
        {
            PlayReadyLicenseAcquisitionServiceRequest licenseRequest = _serviceRequest as PlayReadyLicenseAcquisitionServiceRequest;

            licenseRequest.Uri = LicenseRequestUri;

            licenseRequest.ChallengeCustomData = "Custom Data";
        }
        
        async public void  AcquireLicenseReactively(PlayReadyLicenseAcquisitionServiceRequest licenseRequest)
        {
            Exception exception = null;
            
            try
            {   
                _serviceRequest = licenseRequest;
                ConfigureServiceRequest();

                if( RequestConfigData.ManualEnabling )
                {
                    HttpHelper httpHelper = new HttpHelper( licenseRequest );
                    await httpHelper.GenerateChallengeAndProcessResponse();
                }
                else
                {
                    await licenseRequest.BeginServiceRequest();
                }
            }
            catch( Exception ex )
            {
                exception = ex;
            }
            finally
            {
                LAServiceRequestCompleted( licenseRequest, exception );
            }
        }
        
    }

    public class LAAndReportResult : LicenseAcquisition
    {
        ReportResultDelegate _reportResult = null;
        string _strExpectedError = null;
        
        public string ExpectedError  
        {  
            set { this._strExpectedError =  value; }  
            get { return this._strExpectedError; } 
        }
        
        public LAAndReportResult( ReportResultDelegate callback)
        {
            _reportResult = callback;
        }
        
        protected override void LAServiceRequestCompleted( PlayReadyLicenseAcquisitionServiceRequest  sender, Exception hrCompletionStatus )
        {
            if( hrCompletionStatus == null )
            {
               _reportResult( true );
            }
            else
            {
                if( !PerformEnablingActionIfRequested(hrCompletionStatus) && !HandleExpectedError(hrCompletionStatus) )
                {
                   _reportResult( false );
                }
            }
        }
        
        protected override void EnablingActionCompleted(bool bResult)
        {
            _reportResult( bResult );
        }

        protected override bool HandleExpectedError(Exception ex)
        {
            if( string.IsNullOrEmpty( _strExpectedError ) )
            {
                _strExpectedError = RequestConfigData.ExpectedLAErrorCode;
            }
            
            bool bHandled = false;
            if( _strExpectedError != null )
            {
                if ( ex.Message.ToLower().Contains( _strExpectedError.ToLower() ) )
                {
                    bHandled = true;
                    _reportResult( true );
                }
            }

            return bHandled;
        }
        
    }

}
