using Microsoft.Media.AdaptiveStreaming;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Microsoft.AdaptiveStreaming.Dash
{
    public sealed partial class DashDownloaderPlugin : ILifetimeAwareDownloaderPlugin
    {
        public IAsyncOperation<DownloaderResponse> RequestAsync(DownloaderRequest pDownloaderRequest)
        {
            if (pDownloaderRequest.RequestUri == manifestUri)
            {
                return AsyncInfo.Run(c => RequestManifestAsync(pDownloaderRequest, c));
            }
            else
            {
                return AsyncInfo.Run(c => RequestChunkAsync(pDownloaderRequest, c));
            }
        }

        private async Task<DownloaderResponse> RequestChunkAsync(DownloaderRequest pDownloaderRequest, CancellationToken c)
        {
            try
            {
                var response = await DownloadChunkAsync(pDownloaderRequest.RequestUri, c);
                c.ThrowIfCancellationRequested();
                if (response == null)
                {
                    return new DownloaderResponse(pDownloaderRequest.RequestUri, pDownloaderRequest.Headers, pDownloaderRequest.Cookies);
                }
                else
                {
                    return new DownloaderResponse(pDownloaderRequest.RequestUri, response.Stream.AsInputStream(), (ulong)response.Stream.Length, response.ContentType, response.Headers, false);
                }
            }
            catch (WebRequestorFailure ex)
            {
                return new DownloaderResponse(pDownloaderRequest.RequestUri, null, 0, string.Empty, ex.Headers, false);
            }
            catch
            {
                return new DownloaderResponse(pDownloaderRequest.RequestUri, null, 0, string.Empty, null, false);
            }
        }

        private async Task<DownloaderResponse> RequestManifestAsync(DownloaderRequest pDownloaderRequest, CancellationToken c)
        {
            try
            {
                var response = await DownloadManifestAsync(pDownloaderRequest.RequestUri, c);
                c.ThrowIfCancellationRequested();
                if (response == null)
                {
                    return new DownloaderResponse(pDownloaderRequest.RequestUri, pDownloaderRequest.Headers, pDownloaderRequest.Cookies);
                }
                else
                {
                    return new DownloaderResponse(pDownloaderRequest.RequestUri, response.Stream.AsInputStream(), (ulong)response.Stream.Length, "text/xml", response.Headers, false);
                }
            }
            catch (WebRequestorFailure ex)
            {
                return new DownloaderResponse(pDownloaderRequest.RequestUri, null, 0, string.Empty, ex.Headers, false);
            }
            catch
            {
                return new DownloaderResponse(pDownloaderRequest.RequestUri, null, 0, string.Empty, null, false);
            }
        }

        public void ResponseData(DownloaderRequest pDownloaderRequest, DownloaderResponse pDownloaderResponse)
        {
            if (pDownloaderResponse != null && pDownloaderResponse.ResponseStream != null)
            {
                pDownloaderResponse.ResponseStream.Dispose();
            }
        }

        void ILifetimeAwareDownloaderPlugin.OnCloseMedia(Uri manifestUri)
        {
            this.OnCloseMedia();
        }

        void ILifetimeAwareDownloaderPlugin.OnOpenMedia(Uri manifestUri)
        {
            this.OnOpenMedia(manifestUri);
        }
    }
}
