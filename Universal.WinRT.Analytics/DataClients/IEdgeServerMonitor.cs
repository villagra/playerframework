using System;
using System.Threading;
using System.Threading.Tasks;
#if SILVERLIGHT
#else
using Windows.Foundation;
#endif

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// Provides an interface for a class that can retrieve info about the edge server used to stream the media.
    /// </summary>
    public interface IEdgeServerMonitor
    {
#if SILVERLIGHT
        /// <summary>
        /// Initiates the async request to retrieve edge server and client IP info.
        /// </summary>
        /// <param name="currentStreamUri">The stream Uri.</param>
        /// <param name="cancellationToken">The CancellationToken to abort the download.</param>
        Task<EdgeServerResult> GetEdgeServerAsync(Uri currentStreamUri, CancellationToken cancellationToken);
#else
        /// <summary>
        /// Initiates the async request to retrieve edge server and client IP info.
        /// </summary>
        /// <param name="currentStreamUri">The stream Uri.</param>
        IAsyncOperation<EdgeServerResult> GetEdgeServerAsync(Uri currentStreamUri);
#endif
    }
}
