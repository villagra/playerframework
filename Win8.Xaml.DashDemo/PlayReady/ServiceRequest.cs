//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

using Windows.Foundation.Collections;
using System.Text;

using Windows.Foundation;
using Windows.Media.Protection;
using Microsoft.Media.PlayReadyClient;

namespace Microsoft.PlayerFramework.Xaml.DashDemo
{
    
    public class ServiceRequestConfigData
    {

        Guid    _guidKeyId = Guid.Empty;
        string  _strKeyIdString = String.Empty;
        Guid    _guidDomainServiceId = Guid.Empty;
        Guid    _guidDomainAccountId = Guid.Empty;
        Uri     _domainUri = null;
        
        Uri     _Uri = null;
        string  _strChallengeCustomData = String.Empty;
        string  _strResponseCustomData = String.Empty;
        PlayReadyEncryptionAlgorithm  _encryptionAlgorithm;
        string  _strExpectedLAErrorCode = String.Empty;

        bool    _manualEnabling = false;
        public bool ManualEnabling  
        {  
            set { this._manualEnabling=  value; }  
            get { return this._manualEnabling; } 
        }
        public Guid KeyId  
        {  
            set { this._guidKeyId=  value; }  
            get { return this._guidKeyId; } 
        }
        public string KeyIdString  
        {  
            set { this._strKeyIdString=  value; }  
            get { return this._strKeyIdString; } 
        }

        public Uri Uri  
        {  
            set { this._Uri=  value; }  
            get { return this._Uri; } 
        }  
        public string ChallengeCustomData  
        {  
            set { this._strChallengeCustomData =  value; }  
            get { return this._strChallengeCustomData; } 
        }  
        public string ResponseCustomData  
        {  
            set { this._strResponseCustomData =  value; }  
            get { return this._strResponseCustomData; } 
        }  
        
        //
        //  Domain related config
        //
        public Guid DomainServiceId
        {  
            set { this._guidDomainServiceId =  value; }  
            get { return this._guidDomainServiceId; } 
        }  
        public Guid DomainAccountId
        {  
            set { this._guidDomainAccountId =  value; }  
            get { return this._guidDomainAccountId; } 
        }  
        public Uri DomainUri  
        {  
            set { this._domainUri=  value; }  
            get { return this._domainUri; } 
        }  

        //
        // License acquisition related config
        //
        public PlayReadyEncryptionAlgorithm EncryptionAlgorithm  
        {  
            set { this._encryptionAlgorithm =  value; }  
            get { return this._encryptionAlgorithm; } 
        }
        public string ExpectedLAErrorCode  
        {  
            set { this._strExpectedLAErrorCode =  value; }  
            get { return this._strExpectedLAErrorCode; } 
        }
        
    }

    public class ServiceRequest
    {
        public Uri LicenseRequestUri { get; set; }

        ServiceRequestConfigData _requestConfigData = null;
        protected IPlayReadyServiceRequest _serviceRequest = null;
        RequestChain   _requestChain = null;
        
        protected const int MSPR_E_CONTENT_ENABLING_ACTION_REQUIRED = -2147174251;
        protected const int DRM_E_NOMORE_DATA = -2147024637; //( 0x80070103 )
             
        public ServiceRequestConfigData RequestConfigData  
        {  
            set { this._requestConfigData =  value; }  
            get { 
                    if( this._requestConfigData == null )
                        return new ServiceRequestConfigData();
                    else
                        return this._requestConfigData;
                } 
        }

        protected bool IsEnablingActionRequested(Exception ex)
        {
            bool bRequested = false;
            
            COMException comException = ex as COMException;
            if ( comException != null && comException.HResult == MSPR_E_CONTENT_ENABLING_ACTION_REQUIRED )
            {
                bRequested = true;
            }

            return bRequested;
        }

        protected virtual void EnablingActionCompleted(bool bResult)
        {
            
        }

        protected virtual bool HandleExpectedError(Exception ex)
        {
            return false;
        }
        protected bool PerformEnablingActionIfRequested(Exception ex)
        {
            bool bPerformed = false;
            
            if ( IsEnablingActionRequested(ex) ) 
            {
                IPlayReadyServiceRequest nextServiceRequest = _serviceRequest.NextServiceRequest();
                if( nextServiceRequest != null )
                {
                    _requestChain = new RequestChain( nextServiceRequest);
                    _requestChain.LicenseRequestUri = LicenseRequestUri;
                    _requestChain.RequestConfigData = _requestConfigData;
                    _requestChain.FinishAndReportResult( new ReportResultDelegate(RequestChain_Finished));
                    
                    bPerformed = true;
                }
            }
            
            return bPerformed;
        }
        
        void RequestChain_Finished(bool bResult)
        {
            EnablingActionCompleted( bResult );
        }
        
    }

}
