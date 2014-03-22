using System;

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// An interface to allow an object to become a source of logging events.
    /// </summary>
    public interface ILoggingSource
    {
        /// <summary>
        /// Notifies the consumer that a new log is available
        /// </summary>
        event EventHandler<LogEventArgs> LogCreated;
    }
}
