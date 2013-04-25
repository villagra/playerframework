using System;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// EventArgs used to return an exception that occured during batching.
    /// </summary>
#if SILVERLIGHT
    public class BatchingErrorEventArgs : EventArgs
#else
    public sealed class BatchingErrorEventArgs : Object
#endif
    {
        internal BatchingErrorEventArgs(Exception error)
        {
            Error = error;
        }

        /// <summary>
        /// Gets the error that occured from a batching operation.
        /// </summary>
        public Exception Error { get; private set; }
    }
}
