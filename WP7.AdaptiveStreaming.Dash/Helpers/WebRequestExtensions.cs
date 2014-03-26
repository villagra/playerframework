using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.Media.AdaptiveStreaming.Dash
{
    internal static class WebRequestExtensions
    {
        public static void AddRange(this HttpWebRequest request, long from, long to)
        {
            request.Headers["Range"] = string.Format("bytes={0}-{1}", from, to);
        }

        public static void AddRange(this HttpWebRequest request, long range)
        {
            if (range < 0)
            {
                request.Headers["Range"] = string.Format("bytes={0}", range);
            }
            else
            {
                throw new NotSupportedException("Only supporting final open range requests.");
                //request.Headers["Range"] = string.Format("bytes={0}-", range);
            }
        }

        public static async Task<HttpWebResponse> GetResponseAsync(this HttpWebRequest request)
        {
            var tcs = new TaskCompletionSource<HttpWebResponse>();

            request.BeginGetResponse(ar =>
            {
                try
                {
                    var response = (HttpWebResponse)request.EndGetResponse(ar);
                    tcs.SetResult(response);
                }
                catch (WebException ex)
                {
                    HttpWebResponse response = (HttpWebResponse)ex.Response;
                    tcs.SetException(new WebRequestorFailure(response.StatusCode, response.Headers));
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }, null);

            return await tcs.Task;
        }
    }

    internal class WebRequestorFailure : Exception
    {
        public WebRequestorFailure(HttpStatusCode httpStatusCode, WebHeaderCollection headers)
        {
            HttpStatusCode = httpStatusCode;
            Headers = headers;
        }

        public HttpStatusCode HttpStatusCode { get; private set; }
        public WebHeaderCollection Headers { get; private set; }
    }

    internal class WebRequestorResponse
    {
        public WebRequestorResponse(Stream stream, HttpStatusCode httpStatusCode, WebHeaderCollection headers, string contentType)
        {
            ContentType = contentType;
            Stream = stream;
            HttpStatusCode = httpStatusCode;
            Headers = headers;
        }

        public string ContentType { get; private set; }
        public Stream Stream { get; set; }
        public HttpStatusCode HttpStatusCode { get; private set; }
        public WebHeaderCollection Headers { get; private set; }
    }
}
