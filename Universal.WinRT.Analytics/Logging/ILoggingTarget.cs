using System;

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// An interface to allow an object to become a target of logging events.
    /// </summary>
    public interface ILoggingTarget
    {
        /// <summary>
        /// Called to pass a new log to the target.
        /// </summary>
        /// <param name="log">The log object that was generated.</param>
        void LogEntry(ILog log);
    }
}
