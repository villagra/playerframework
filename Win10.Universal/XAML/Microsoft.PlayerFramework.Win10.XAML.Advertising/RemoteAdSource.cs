using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Media.Advertising;
using System.IO;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Net.Http.Headers;


namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// Provides an ad source that requires a Url to be downloaded and turned into a stream before passing to the ad handler.
    /// </summary>
    public sealed class RemoteAdSource : FrameworkElement, IResolveableAdSource
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
        public IAsyncAction LoadPayload()
        {
            return AsyncInfo.Run(c => LoadPayload(c));
        }

        internal async Task LoadPayload(CancellationToken cancellationToken)
        {
            if (IsLoaded) return;
            if (loadingTask == null)
            {
                if (HttpClient != null)
                {
                    loadingTask = HttpClient.GetStreamAsync(Uri);
                }
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
