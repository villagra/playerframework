//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Media.PlayReadyClient;


namespace Microsoft.PlayerFramework.Xaml.DashDemo
{
    public class HttpHelper
    {
        protected IPlayReadyServiceRequest _serviceRequest = null;
        Uri  _uri = null;

        public HttpHelper( IPlayReadyServiceRequest serviceRequest)
        {
            _serviceRequest = serviceRequest;
        }
        
        public async Task GenerateChallengeAndProcessResponse()
        {
            PlayReadySoapMessage soapMessage = _serviceRequest.GenerateManualEnablingChallenge();
            if( _uri == null )
            {
                _uri = soapMessage.Uri;
            }

            byte[] messageBytes = soapMessage.GetMessageBody();
            HttpContent httpContent = new ByteArrayContent( messageBytes );

            IPropertySet propertySetHeaders = soapMessage.MessageHeaders;
            foreach( string strHeaderName in propertySetHeaders.Keys )
            {
                string strHeaderValue = propertySetHeaders[strHeaderName].ToString();
                
                // The Add method throws an ArgumentException try to set protected headers like "Content-Type"
                // so set it via "ContentType" property
                if ( strHeaderName.Equals( "Content-Type", StringComparison.OrdinalIgnoreCase ) )
                {
                    httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse(strHeaderValue);
                }
                else
                {
                    httpContent.Headers.Add(strHeaderName.ToString(), strHeaderValue);
                }
                
            }

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.PostAsync( _uri, httpContent );
            string strResponse = await response.Content.ReadAsStringAsync();

            Exception exResult = _serviceRequest.ProcessManualEnablingResponse( await response.Content.ReadAsByteArrayAsync()) ;
            if( exResult != null)
            {
                throw exResult;
            }
        }


    }


}
