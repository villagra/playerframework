using System;

namespace Microsoft.AudienceInsight
{
	/// <summary>
	/// EventArgs used to return an exception that occured during batching.
	/// </summary>
	public sealed class BatchingErrorEventArgs : Object
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
