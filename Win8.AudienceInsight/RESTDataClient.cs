using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
#if SILVERLIGHT
#else
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections.Generic;
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
                            if (Compress)
                            {
                                batch.SerializeCompressedXml(stream);
                            }
                            else
                            {
                                batch.SerializeUncompressedXml(stream);
                            }
                        }
                        else if (SerializationFormat == AudienceInsight.SerializationFormat.Json)
                        {
                            if (Compress)
                            {
                                batch.SerializeCompressedJson(stream);
                                headers.Add("Content-Type", "application/gzip");
                            }
                            else
                            {
                                batch.SerializeUncompressedJson(stream);
                                headers.Add("Content-Type", "application/json; charset=utf-8");
                            }
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

                                using (var responseStream = await response.Content.ReadAsStreamAsync())
                                {
                                    c.ThrowIfCancellationRequested();
                                    //return ResponseDeserializer.Deserialize(responseStream);
                                    return new LogBatchResult() { IsEnabled = true };
                                }
                            }
                        }
                    }
                }
            }
            else return null;
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
