using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.VideoAdvertising;
using System.IO;
#if NETFX_CORE
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Net.Http.Headers;
#endif


namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// Provides an ad source that requires a Url to be downloaded and turned into a stream before passing to the ad handler.
    /// </summary>
    public sealed class RemoteAdSource :
#if NETFX_CORE
 FrameworkElement,
#endif
 IResolveableAdSource
    {
        /// <summary>
        /// Creates a new instance of RemoteAdSource
        /// </summary>
        public RemoteAdSource()
        {
            AllowMultipleAds = true;
        }

        /// <summary>
        /// Creates a new instance of RemoteAdSource
        /// </summary>
        /// <param name="uri">The Uri to download and turn into a stream payload.</param>
        /// <param name="type">The type of the ad. Normally this is "vast"</param>
        public RemoteAdSource(Uri uri, string type)
        {
            Uri = uri;
            Type = type;
        }

#if SILVERLIGHT
        /// <summary>
        /// Creates a new instance of RemoteAdSource
        /// </summary>
        /// <param name="webRequest">The WebRequest object to use to download the payload.</param>
        /// <param name="type">The type of the ad. Normally this is "vast"</param>
        public RemoteAdSource(WebRequest webRequest, string type)
        {
            WebRequest = webRequest;
            Type = type;
        }

        /// <summary>
        /// Gets or sets a WebRequest object to use when requesting the URLs over HTTP
        /// </summary>
        public WebRequest WebRequest { get; set; }
#else
        /// <summary>
        /// Creates a new instance of RemoteAdSource
        /// </summary>
        /// <param name="httpClient">The HttpClient object to use to download the payload.</param>
        /// <param name="type">The type of the ad. Normally this is "vast"</param>
        public RemoteAdSource(HttpClient httpClient, string type)
        {
            HttpClient = httpClient;
            Type = type;
        }

        /// <summary>
        /// Gets or sets an HttpClient object to use when requesting the URLs over HTTP
        /// </summary>
        public HttpClient HttpClient { get; set; }
#endif

        /// <summary>
        /// Gets if the Payload has been loaded yet.
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Gets or sets the Uri of the ad.
        /// </summary>
        public Uri Uri { get; set; }
        
        private Task<Stream> loadingTask;

        /// <summary>
        /// Loads the ad payload from the web.
        /// </summary>
        /// <returns>Awaitable action</returns>
#if SILVERLIGHT
        public async Task LoadPayload(CancellationToken cancellationToken)
#else
        public IAsyncAction LoadPayload()
        {
            return AsyncInfo.Run(c => LoadPayload(c));
        }

        internal async Task LoadPayload(CancellationToken cancellationToken)
#endif
        {
            if (IsLoaded) return;
            if (loadingTask == null)
            {
#if SILVERLIGHT
                if (WebRequest != null)
                {
                    loadingTask = WebRequest.GetStreamAsync();
                }
#else
                if (HttpClient != null)
                {
                    loadingTask = HttpClient.GetStreamAsync(Uri);
                }
#endif
                else
                {
                    loadingTask = Extensions.LoadStreamAsync(Uri);
                }
                try
                {
                    payload = await loadingTask;
                    IsLoaded = true;
                }
                finally
                {
                    loadingTask = null;
                }
            }
            else
            {
                await loadingTask;
            }
            cancellationToken.ThrowIfCancellationRequested();
        }

        object payload;
        object IAdSource.Payload
        {
            get { return payload; }
            set { payload = value; }
        }

        /// <inheritdoc /> 
        public string Key { get; set; }

        /// <inheritdoc /> 
        public string Type { get; set; }

        /// <inheritdoc />
        public bool AllowMultipleAds { get; set; }

        /// <inheritdoc />
        public int? MaxRedirectDepth { get; set; }
    }
}
