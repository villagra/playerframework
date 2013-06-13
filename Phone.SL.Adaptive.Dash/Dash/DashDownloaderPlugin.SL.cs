using Microsoft.Web.Media.SmoothStreaming;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AdaptiveStreaming.Dash
{
    public partial class DashDownloaderPlugin : ILifetimeAwareDownloaderPlugin
    {
        CancellationTokenSource cts;

        public async Task<CacheResponse> RequestAsync(CacheRequest request)
        {
            try
            {
                if (request.CanonicalUri == manifestUri)
                {
                    var response = await DownloadManifestAsync(request.CanonicalUri, cts.Token);
                    cts.Token.ThrowIfCancellationRequested();
                    return new CacheResponse(response.Stream.Length, "text/xml", response.Headers, response.Stream, response.HttpStatusCode, response.HttpStatusCode.ToString(), DateTime.UtcNow);
                }
                else
                {
                    var response = await DownloadChunkAsync(request.CanonicalUri, cts.Token);
                    cts.Token.ThrowIfCancellationRequested();
                    return new CacheResponse(response.Stream.Length, response.ContentType, response.Headers, response.Stream, response.HttpStatusCode, response.HttpStatusCode.ToString(), DateTime.UtcNow);
                }
            }
            catch (WebRequestorFailure ex)
            {
                return new CacheResponse(0, null, ex.Headers, null, ex.HttpStatusCode, ex.HttpStatusCode.ToString(), DateTime.UtcNow);
            }
            catch
            {
                return new CacheResponse(0, null, null, null, HttpStatusCode.BadRequest, "BadRequest", DateTime.UtcNow);
            }
        }

        public void ResponseData(CacheRequest pDownloaderRequest, CacheResponse pDownloaderResponse)
        {
            // do nothing
        }

        void ILifetimeAwareDownloaderPlugin.OnOpenMedia(Uri manifestUri)
        {
            this.OnOpenMedia(manifestUri);
            cts = new CancellationTokenSource();
        }

        void ILifetimeAwareDownloaderPlugin.OnCloseMedia(Uri manifestUri)
        {
            this.OnCloseMedia();
            cts.Cancel();
        }
    }
}
