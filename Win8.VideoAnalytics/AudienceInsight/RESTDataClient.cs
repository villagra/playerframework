using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// An implementation of IBatchAgent used to send data to a REST endpoint.
    /// </summary>
    public sealed class RESTDataClient : IBatchAgent
    {
        /// <summary>
        /// Creates a new instance of RESTDataClient.
        /// </summary>
        /// <param name="serviceUrl">The url endpoint of the service to send data to.</param>
        /// <param name="timeout">A timeout for all requests.</param>
        /// <param name="version">The version number to send to the server.</param>
        public RESTDataClient(Uri serviceUrl, int timeout, int version)
        {
            Version = version;
            ServiceUrl = serviceUrl;
            Timeout = TimeSpan.FromSeconds(timeout);
        }

        /// <summary>
        /// Gets the url endpoint of the service to send data to.
        /// </summary>
        public Uri ServiceUrl { get; private set; }

        /// <summary>
        /// Gets the version number to send to the server.
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Gets a timeout for all requests.
        /// </summary>
        public TimeSpan Timeout { get; private set; }

#if NETFX_CORE
        /// <inheritdoc /> 
        public IAsyncOperation<LogBatchResult> SendBatchAsync(IBatch batch)
        {
            return AsyncInfo.Run(c => SendBatchAsync(batch, c));
        }

        internal async Task<LogBatchResult> SendBatchAsync(IBatch batch, CancellationToken c)
#else
        /// <inheritdoc /> 
        public async Task<LogBatchResult> SendBatchAsync(IBatch batch, CancellationToken c)
#endif
        {
            if (batch.Logs != null)
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.Timeout = Timeout;
                    httpClient.DefaultRequestHeaders.Add("Ver", Version.ToString());

                    using (var stream = new MemoryStream())
                    {
                        batch.SerializeCompressed(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        using (var content = new StreamContent(stream))
                        {
                            using (var response = await httpClient.PostAsync(ServiceUrl, content, c))
                            {
                                response.EnsureSuccessStatusCode();

                                using (var responseStream = await response.Content.ReadAsStreamAsync())
                                {
                                    c.ThrowIfCancellationRequested();
                                    return ResponseDeserializer.Deserialize(responseStream);
                                }
                            }
                        }
                    }
                }
            }
            else return null;
        }

    }
}
