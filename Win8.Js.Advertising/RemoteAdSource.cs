using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VideoAdvertising;
using Windows.Foundation;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Linq;

namespace Microsoft.PlayerFramework.Js.Advertising
{
    /// <summary>
    /// Provides an ad source that requires a Url to be downloaded and turned into a stream before passing to the ad handler.
    /// </summary>
    public sealed class RemoteAdSource : IResolveableAdSource
    {
        /// <summary>
        /// Creates a new instance of RemoteAdSource
        /// </summary>
        public RemoteAdSource()
        {
            AllowMultipleAds = true;
            Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Creates a new instance of RemoteAdSource
        /// </summary>
        /// <param name="uri">The Uri to download and turn into a stream payload.</param>
        public RemoteAdSource(Uri uri)
            : this()
        {
            Uri = uri;
        }

        /// <summary>
        /// Creates a new instance of RemoteAdSource
        /// </summary>
        /// <param name="uri">The Uri to download and turn into a stream payload.</param>
        /// <param name="type">The type of the ad. Normally this is "vast"</param>
        public RemoteAdSource(Uri uri, string type)
            : this(uri)
        {
            Type = type;
        }

        /// <summary>
        /// Gets if the Payload has been loaded yet.
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Gets or sets the URI of the ad.
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Gets or sets the headers to send with ad requests.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }

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
                loadingTask = LoadToStream(Uri, Headers);
                try
                {
                    Payload = await loadingTask;
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

        static async Task<Stream> LoadToStream(Uri source, IDictionary<string, string> headers)
        {
            switch (source.Scheme.ToLowerInvariant())
            {
                case "ms-appx":
                case "ms-appdata":
                    var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(source);
                    return await file.OpenStreamForReadAsync();
                default:
                    using (var client = new HttpClient())
                    {
                        if (headers != null)
                        {
                            foreach (var header in headers)
                            {
                                client.DefaultRequestHeaders.Add(header.Key, header.Value);
                            }
                        }

                        using (var stream = await client.GetStreamAsync(source))
                        {
                            var result = new MemoryStream();
                            await stream.CopyToAsync(result);
                            result.Seek(0, SeekOrigin.Begin);
                            return result;
                        }
                    }
            }
        }

        /// <inheritdoc /> 
        public object Payload { get; set; }

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