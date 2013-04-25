using Microsoft.Web.Media.SmoothStreaming;
using System;

namespace Microsoft.AdaptiveStreaming
{
    /// <summary>
    /// Extends the IDownloaderPlugin interface to support noficiation when the stream is opened and closed
    /// </summary>
    public interface ILifetimeAwareDownloaderPlugin : IDownloaderPlugin
    {
        /// <summary>
        /// Called when the IAdaptiveSource is opened.
        /// </summary>
        /// <param name="manifestUri">The Uri of the manifest being opened</param>
        void OnOpenMedia(Uri manifestUri);

        /// <summary>
        /// Called when the IAdaptiveSource is closed.
        /// </summary>
        void OnCloseMedia(Uri manifestUri);
    }
}
