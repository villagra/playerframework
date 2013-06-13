using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.AdaptiveStreaming.Dash
{
    internal static class HttpClientExtensions
    {
        public static void AddRange(this HttpClient request, long from, long to)
        {
            request.DefaultRequestHeaders.Range = new RangeHeaderValue(from, to);
        }

        public static void AddRange(this HttpClient request, long range)
        {
            if (range < 0)
            {
                request.DefaultRequestHeaders.Range = new RangeHeaderValue(null, -range);
            }
            else
            {
                request.DefaultRequestHeaders.Range = new RangeHeaderValue(range, null);
            }
        }

        public static async Task<WebRequestorResponse> GetResponse(this HttpClient httpClient, Uri uri)
        {
            using (var response = await httpClient.GetAsync(uri))
            {
                if (response.IsSuccessStatusCode)
                {
                    var stream = new MemoryStream();
                    await response.Content.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    return new WebRequestorResponse(stream, response.StatusCode, response.Headers, response.Content.Headers.ContentType.MediaType);
                }
                else
                {
                    throw new WebRequestorFailure(response.StatusCode, response.Headers);
                }
            }
        }
    }

    internal class WebRequestorFailure : Exception
    {
        public WebRequestorFailure(HttpStatusCode httpStatusCode, HttpHeaders headers)
        {
            HttpStatusCode = httpStatusCode;
            Headers = headers.ToDictionary(i => i.Key, i => string.Join(";", i.Value));
        }

        public HttpStatusCode HttpStatusCode { get; private set; }
        public IReadOnlyDictionary<string, string> Headers { get; private set; }
    }

    internal class WebRequestorResponse
    {
        public WebRequestorResponse(Stream stream, HttpStatusCode httpStatusCode, HttpHeaders headers, string contentType)
        {
            ContentType = contentType;
            Stream = stream;
            HttpStatusCode = httpStatusCode;
            if (headers != null)
            {
                Headers = headers.ToDictionary(i => i.Key, i => string.Join(";", i.Value));
            }
        }

        public string ContentType { get; private set; }
        public Stream Stream { get; set; }
        public HttpStatusCode HttpStatusCode { get; private set; }
        public IReadOnlyDictionary<string, string> Headers { get; private set; }
    }
}
