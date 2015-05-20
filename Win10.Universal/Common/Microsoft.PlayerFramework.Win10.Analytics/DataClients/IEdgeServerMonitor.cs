using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// Provides an interface for a class that can retrieve info about the edge server used to stream the media.
    /// </summary>
    public interface IEdgeServerMonitor
    {
        /// <summary>
        /// Initiates the async request to retrieve edge server and client IP info.
        /// </summary>
        /// <param name="currentStreamUri">The stream Uri.</param>
        IAsyncOperation<EdgeServerResult> GetEdgeServerAsync(Uri currentStreamUri);
    }
}
