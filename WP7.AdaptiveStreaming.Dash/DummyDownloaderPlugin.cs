using Microsoft.Web.Media.SmoothStreaming;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AdaptiveStreaming.Dash
{
    public class DummyDownloaderPlugin : ISmoothStreamingCache
    {
        public IAsyncResult BeginPersist(CacheRequest request, CacheResponse response, AsyncCallback callback, object state)
        {
            return null;
        }

        public bool EndPersist(IAsyncResult ar)
        {
            return true;
        }

        public IAsyncResult BeginRetrieve(CacheRequest request, AsyncCallback callback, object state)
        {
            var asyncResult = new CacheAsyncResult() { AsyncState = state, AsyncWaitHandle = new AutoResetEvent(false) };

            Task.Run(async () =>
            {
                asyncResult.Response = await GetCacheResponseAsync(request.CanonicalUri);
                asyncResult.IsCompleted = true;

                //callback
                if (callback != null)
                {
                    callback(asyncResult);
                }

                //signal the blocked SSME downloader thread waiting in EndRetrieve
                if (asyncResult.AsyncWaitHandle != null)
                {
                    (asyncResult.AsyncWaitHandle as AutoResetEvent).Set();
                }
            });

            return asyncResult;
        }

        public CacheResponse EndRetrieve(IAsyncResult ar)
        {
            // let the SSME handle this
            if (ar == null)
            {
                return null;
            }

            //block SSME downloader for the response
            if (ar.AsyncWaitHandle != null)
            {
                ar.AsyncWaitHandle.WaitOne();
            }

            //get the async result
            var cacheRequestAsyncResult = ar as CacheAsyncResult;
            if (cacheRequestAsyncResult != null && cacheRequestAsyncResult.IsCompleted)
            {
                return cacheRequestAsyncResult.Response;
            }
            else
            {
                return new CacheResponse(0, null, null, null, System.Net.HttpStatusCode.InternalServerError, "Internal Server Error", DateTime.UtcNow);
            }
        }

        private static async Task<CacheResponse> GetCacheResponseAsync(Uri source)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(source))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var mimeType = response.Content.Headers.ContentType.MediaType;
                        var memStream = new MemoryStream();
                        await response.Content.CopyToAsync(memStream);
                        //memStream.Seek(0, SeekOrigin.Begin); // not necessary
                        return new CacheResponse(memStream.Length, mimeType, null, memStream, response.StatusCode, response.StatusCode.ToString(), DateTime.UtcNow);
                    }
                    else
                    {
                        return new CacheResponse(0, null, null, null, response.StatusCode, response.StatusCode.ToString(), DateTime.UtcNow);
                    }
                }
            }
        }

    }
}
