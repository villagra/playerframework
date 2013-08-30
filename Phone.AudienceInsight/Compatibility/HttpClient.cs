using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    /// <summary>
    /// Used for compatibility with Win8
    /// </summary>
    internal class HttpClient : IDisposable
    {
        public HttpClient()
        {
            DefaultRequestHeaders = new Dictionary<string, string>();
        }

        public TimeSpan Timeout { get; set; }

        public IDictionary<string, string> DefaultRequestHeaders { get; private set; }

        public async Task<Stream> GetStreamAsync(Uri address)
        {
            var request = WebRequest.CreateHttp(address);
            foreach (var header in DefaultRequestHeaders)
            {
                request.Headers[header.Key] = header.Value;
            }
            request.AllowReadStreamBuffering = true;
            var response = await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);
            return response.GetResponseStream();
        }

        public async Task<HttpResponseMessage> PostAsync(Uri address, StreamContent content, CancellationToken c)
        {
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(c))
            {
                cts.CancelAfter(Timeout);
                var request = WebRequest.CreateHttp(address);
                request.Method = "POST";
                foreach (var header in DefaultRequestHeaders)
                {
                    request.Headers[header.Key] = header.Value;
                }
                foreach (var header in content.Headers)
                {
                    request.Headers[header.Key] = header.Value;
                }
                request.AllowReadStreamBuffering = true;
                using (var stream = await Task.Factory.FromAsync<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream, null))
                {
                    await content.Stream.CopyToAsync(stream);
                }

                return new HttpResponseMessage((HttpWebResponse)await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null));
            }
        }

        public void Dispose()
        {
            DefaultRequestHeaders = null;
        }
    }

    internal class HttpResponseMessage : IDisposable
    {
        public HttpResponseMessage(HttpWebResponse response)
        {
            Response = response;
        }

        public HttpContent Content { get { return new HttpResponseContent(Response); } }

        public HttpWebResponse Response { get; set; }

        public void EnsureSuccessStatusCode()
        {
            if (Response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Http error");
            }
        }

        public void Dispose()
        {
            Response = null;
        }
    }

    internal abstract class HttpContent : IDisposable
    {
        public HttpContent()
        {
            Headers = new Dictionary<string, string>();
        }

        public abstract void Dispose();

        public abstract Task<Stream> ReadAsStreamAsync();

        public IDictionary<string, string> Headers { get; private set; }
    }

    internal class HttpResponseContent : HttpContent
    {
        public HttpResponseContent(HttpWebResponse response)
        {
            Response = response;
        }

        public HttpWebResponse Response { get; private set; }

        public override Task<Stream> ReadAsStreamAsync()
        {
#if WINDOWS_PHONE7
            return TaskEx.FromResult(Response.GetResponseStream());
#else
            return Task.FromResult(Response.GetResponseStream());
#endif
        }

        public override void Dispose()
        {
            Response = null;
        }
    }

    internal class StreamContent : HttpContent
    {
        public StreamContent(Stream stream)
        {
            Stream = stream;
        }

        public Stream Stream { get; set; }

        public override Task<Stream> ReadAsStreamAsync()
        {
#if WINDOWS_PHONE7
            return TaskEx.FromResult(Stream);
#else
            return Task.FromResult(Stream);
#endif
        }

        public override void Dispose()
        {
            Stream = null;
        }
    }
}
