using Microsoft.Media.AdaptiveStreaming;
using System;

namespace Microsoft.Media.AdaptiveStreaming.Helper
{
    /// <summary>
    /// Extends the IDownloaderPlugin interface to support noficiation when the stream is opened and closed
    /// </summary>
    public interface ILifetimeAwareDownloaderPlugin : IDownloaderPlugin
    {
        /// <summary>
        /// Called when the IAdaptiveSource is opened.
        /// </summary>
        /// <param name="manifestUri">The manifest URI associated with the event</param>
        void OnOpenMedia(Uri manifestUri);

        /// <summary>
        /// Called when the IAdaptiveSource is closed.
        /// </summary>
        /// <param name="manifestUri">The manifest URI associated with the event</param>
        void OnCloseMedia(Uri manifestUri);
    }
}
