using Microsoft.Media.AdaptiveStreaming;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Microsoft.AdaptiveStreaming.Dash
{
    internal partial class CffDownloaderPlugin
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
                await Task.Yield();

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
                var stream = await DownloadManifestAsync(pDownloaderRequest.RequestUri, c);
                c.ThrowIfCancellationRequested();
                await Task.Yield();

                return new DownloaderResponse(pDownloaderRequest.RequestUri, stream.AsInputStream(), (ulong)stream.Length, "text/xml", new Dictionary<string, string>(), false);
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
    }
}
