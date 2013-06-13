using Microsoft.Media.AdaptiveStreaming;
using System;
using Windows.Foundation;
using Windows.Storage;

namespace Microsoft.AdaptiveStreaming.Dash
{
    public sealed class CffOfflineDownloaderPlugin : ILifetimeAwareDownloaderPlugin
    {
        public event EventHandler<OfflineMediaOpenedEventArgs> MediaOpened;
        public event EventHandler<OfflineMediaClosedEventArgs> MediaClosed;

        private readonly OfflineCffFileParser parser;
        private readonly CffDownloaderPlugin downloaderPluginBase;

        public CffOfflineDownloaderPlugin()
        {
            parser = new OfflineCffFileParser();
            downloaderPluginBase = new CffDownloaderPlugin(parser);
        }

        public CffOfflineDownloaderPlugin(IStorageFile storageFile)
            : this()
        {
            StorageFile = storageFile;
        }

        public IStorageFile StorageFile
        {
            get { return parser.StorageFile; }
            set { parser.StorageFile = value; }
        }

        public void OnOpenMedia(Uri manifestUri)
        {
            if (MediaOpened != null) MediaOpened(this, new OfflineMediaOpenedEventArgs(manifestUri));
            downloaderPluginBase.OnOpenMedia(manifestUri);
        }

        public void OnCloseMedia(Uri manifestUri)
        {
            downloaderPluginBase.OnCloseMedia();
            if (MediaClosed != null) MediaClosed(this, new OfflineMediaClosedEventArgs(manifestUri));
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
    
#if SILVERLIGHT
    public sealed class OfflineMediaOpenedEventArgs : EventArgs
#else
    public sealed class OfflineMediaOpenedEventArgs 
#endif
    {
        public OfflineMediaOpenedEventArgs(Uri source)
        {
            Source = source;
        }

        public Uri Source { get; private set; }
    }

#if SILVERLIGHT
    public sealed class OfflineMediaClosedEventArgs : EventArgs
#else
    public sealed class OfflineMediaClosedEventArgs 
#endif
    {
        public OfflineMediaClosedEventArgs(Uri source)
        {
            Source = source;
        }

        public Uri Source { get; private set; }
    }
}
