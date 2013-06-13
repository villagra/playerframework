using Microsoft.Media.AdaptiveStreaming;
using System;
using Windows.Foundation;

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

        public void OnOpenMedia(Uri manifestUri)
        {
            downloaderPluginBase.OnOpenMedia(manifestUri);
        }

        public void OnCloseMedia(Uri manifestUri)
        {
            downloaderPluginBase.OnCloseMedia();
        }

        public IAsyncOperation<DownloaderResponse> RequestAsync(DownloaderRequest pDownloaderRequest)
        {
            return downloaderPluginBase.RequestAsync(pDownloaderRequest);
        }

        public void ResponseData(DownloaderRequest pDownloaderRequest, DownloaderResponse pDownloaderResponse)
        {
            downloaderPluginBase.ResponseData(pDownloaderRequest, pDownloaderResponse);
        }
    }
}
