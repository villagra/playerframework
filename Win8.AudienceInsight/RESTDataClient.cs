using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
#if SILVERLIGHT
#else
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net.Http.Headers;
#endif

namespace Microsoft.AudienceInsight
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
        /// <param name="compress">Whether to compress data before sending. Only applies when posting JSON or XML, not with HttpQueryString mode.</param>
        /// <param name="serializationFormat">The version number to send to the server.</param>
        /// <param name="version">The version number to send to the server.</param>
        public RESTDataClient(Uri serviceUrl, int timeout, bool compress, SerializationFormat serializationFormat, int version)
        {
            Version = version;
            ServiceUrl = serviceUrl;
            Timeout = TimeSpan.FromSeconds(timeout);
            Compress = compress;
            SerializationFormat = serializationFormat;
            AdditionalHttpHeaders = new Dictionary<string, string>();
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

        /// <summary>
        /// Gets whether sent data is compressed.
        /// </summary>
        public bool Compress { get; private set; }

        /// <summary>
        /// Gets the serialization format to be used.
        /// </summary>
        public SerializationFormat SerializationFormat { get; private set; }

        /// <summary>
        /// Gets a set of http headers to be included on all HTTP requests
        /// </summary>
        public IDictionary<string, string> AdditionalHttpHeaders { get; private set; }

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
                    
                    Dictionary<string, string> headers = new Dictionary<string, string>();

                    foreach (var headerKey in AdditionalHttpHeaders.Keys)
                        headers.Add(headerKey, AdditionalHttpHeaders[headerKey]);

                    using (var stream = new MemoryStream())
                    {
                        if (SerializationFormat == AudienceInsight.SerializationFormat.Xml)
                        {
#if COMPRESSION
                            if (Compress)
                                batch.SerializeCompressedXml(stream);
                            else
#endif
                                batch.SerializeUncompressedXml(stream);
                        }
                        else if (SerializationFormat == AudienceInsight.SerializationFormat.Json)
                        {
#if COMPRESSION                            
                            if (Compress)
                            {
                                batch.SerializeCompressedJson(stream);
                                headers.Add("Content-Type", "application/gzip");
                            }
                            else
                            {
#endif
                                batch.SerializeUncompressedJson(stream);
                                headers.Add("Content-Type", "application/json; charset=utf-8");
#if COMPRESSION
                            }
#endif
                        }
                        else if (SerializationFormat == AudienceInsight.SerializationFormat.HttpQueryString)
                        {
                            throw new NotImplementedException();
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        stream.Seek(0, SeekOrigin.Begin);

                        var sr = new StreamReader(stream);
                        var myStr = sr.ReadToEnd();

                        stream.Seek(0, SeekOrigin.Begin);
                        using (var content = new StreamContent(stream))
                        {
                            foreach (var headerKey in headers.Keys)
                                content.Headers.Add(headerKey, headers[headerKey]);

                            using (var response = await httpClient.PostAsync(ServiceUrl, content, c))
                            {
                                response.EnsureSuccessStatusCode();

                                c.ThrowIfCancellationRequested();

                                var responseHeaders = new Dictionary<string, string>();
#if SILVERLIGHT                                
                                foreach (var responseHeaderKey in response.Response.Headers.AllKeys)
                                    responseHeaders.Add(responseHeaderKey, response.Response.Headers[responseHeaderKey]);
#else
                                foreach (var responseHeader in response.Headers)
                                    responseHeaders.Add(responseHeader.Key, responseHeader.Value.FirstOrDefault());
#endif
                                return CreateLogBatchResult(responseHeaders);
                            }
                        }
                    }
                }
            }
            else return null;
        }

        LogBatchResult CreateLogBatchResult(Dictionary<string, string> responseHeaders)
        {
            LogBatchResult result = new LogBatchResult();

            if (responseHeaders.Keys.Contains("LoggingEnabled"))
                result.IsEnabled = Convert.ToInt32(responseHeaders["LoggingEnabled"]) != 0 ;

            if (responseHeaders.Keys.Contains("QueuePollingIntervalSeconds"))
                result.QueuePollingInterval = TimeSpan.FromSeconds(Convert.ToDouble(responseHeaders["QueuePollingIntervalSeconds"]));

            if (responseHeaders.Keys.Contains("ServerTime"))
                result.ServerTime = new DateTimeOffset(Convert.ToInt64(responseHeaders["ServerTime"]), TimeSpan.Zero);

            return result;
        }
    }

    /// <summary>
    /// Serialization output types
    /// </summary>
    public enum SerializationFormat
    {
        Unknown,
        Xml,
        Json,
        HttpQueryString
    }
}
