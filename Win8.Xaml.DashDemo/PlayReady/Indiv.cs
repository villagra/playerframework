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
    public delegate void ReportResultDelegate( bool bResult );

    public class Indiv :ServiceRequest
    {
        protected virtual void IndivServiceRequestCompleted( PlayReadyIndividualizationServiceRequest  sender, Exception hrCompletionStatus ) 
        {
        }

        public void  IndivProactively()
        {
            PlayReadyIndividualizationServiceRequest indivRequest = new PlayReadyIndividualizationServiceRequest();
            IndivReactively(indivRequest);
        }
        async public void  IndivReactively(PlayReadyIndividualizationServiceRequest indivRequest)
        {
            Exception exception = null;
            
            try
            {
                _serviceRequest = indivRequest;

                await indivRequest.BeginServiceRequest();
            }
            catch ( Exception ex )
            {
                exception = ex;
            }
            finally
            {
                IndivServiceRequestCompleted( indivRequest, exception );
            }
        }

    }


    public class IndivAndReportResult : Indiv
    {
        ReportResultDelegate _reportResult = null;
        public IndivAndReportResult( ReportResultDelegate callback)
        {
            _reportResult = callback;
        }
        
        protected override void IndivServiceRequestCompleted( PlayReadyIndividualizationServiceRequest  sender, Exception hrCompletionStatus )
        {
            if( hrCompletionStatus == null )
            {
                _reportResult( true );
            }
            else
            {
                //needed for LA revoke->Re-Indiv->LA sequence
                if( !PerformEnablingActionIfRequested(hrCompletionStatus) )
                {
                   _reportResult( false );
                }
            }
        }
        
        protected override void EnablingActionCompleted(bool bResult)
        {
            _reportResult( bResult );
        }
        
    }

}
