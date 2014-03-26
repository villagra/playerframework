using Microsoft.Web.Media.SmoothStreaming;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Media.AdaptiveStreaming.Helper
{
    public interface IDownloaderPlugin
    {
        Task<CacheResponse> RequestAsync(CacheRequest pDownloaderRequest);
        void ResponseData(CacheRequest pDownloaderRequest, CacheResponse pDownloaderResponse);
    }

    public class CacheAsyncResult : IAsyncResult
    {
        public object AsyncState { get; set; }

        public WaitHandle AsyncWaitHandle { get; set; }

        public bool CompletedSynchronously { get; set; }

        public bool IsCompleted { get; set; }

        public CacheResponse Response { get; set; }
    }

    public class DownloaderPluginAdapter : ISmoothStreamingCache
    {
        IDownloaderPlugin downloaderPlugin;

        public DownloaderPluginAdapter(IDownloaderPlugin downloaderPlugin)
        {
            this.downloaderPlugin = downloaderPlugin;    
        }

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

#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
            TaskEx.Run(async () =>
#else
            Task.Run(async () =>
#endif
            {
                asyncResult.Response = await downloaderPlugin.RequestAsync(request);
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

        public void CloseMedia(Uri manifestUri)
        {
            if (downloaderPlugin is ILifetimeAwareDownloaderPlugin)
            {
                ((ILifetimeAwareDownloaderPlugin)downloaderPlugin).OnCloseMedia(manifestUri);
            }
        }

        public void OpenMedia(Uri manifestUri)
        {
            ((ILifetimeAwareDownloaderPlugin)downloaderPlugin).OnOpenMedia(manifestUri);
        }
    }
}
