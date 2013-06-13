using Microsoft.Web.Media.SmoothStreaming;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AdaptiveStreaming.Dash
{
    internal partial class CffDownloaderPlugin
    {
        CancellationTokenSource cts;

        public async Task<CacheResponse> RequestAsync(CacheRequest request)
        {
            try
            {
                if (request.CanonicalUri == manifestUri)
                {
                    var stream = await DownloadManifestAsync(request.CanonicalUri, cts.Token);
                    cts.Token.ThrowIfCancellationRequested();
                    return new CacheResponse(stream.Length, "text/xml", null, stream, HttpStatusCode.OK, "OK", DateTime.UtcNow);
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

        public void OpenMedia(Uri manifestUri)
        {
            cts = new CancellationTokenSource();
            OnOpenMedia(manifestUri);
        }

        public void CloseMedia()
        {
            cts.Cancel();
            OnCloseMedia();
        }
    }
}
