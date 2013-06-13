using Microsoft.Web.Media.SmoothStreaming;
using System;
using System.Threading.Tasks;

namespace Microsoft.AdaptiveStreaming.Dash
{
    public sealed class CffProgressiveDownloaderPlugin : ILifetimeAwareDownloaderPlugin
    {
        private readonly ProgressiveCffFileParser parser;
        private readonly CffDownloaderPlugin downloaderPluginBase;

        public CffProgressiveDownloaderPlugin()
        {
            parser = new ProgressiveCffFileParser();
            downloaderPluginBase = new CffDownloaderPlugin(parser);
        }

        public async Task<CacheResponse> RequestAsync(CacheRequest request)
        {
            return await downloaderPluginBase.RequestAsync(request);
        }

        public void ResponseData(CacheRequest pDownloaderRequest, CacheResponse pDownloaderResponse)
        {
            // do nothing
        }

        public void OnOpenMedia(Uri manifestUri)
        {
            downloaderPluginBase.OpenMedia(manifestUri);
        }

        public void OnCloseMedia(Uri manifestUri)
        {
            downloaderPluginBase.CloseMedia();
        }
    }
}
