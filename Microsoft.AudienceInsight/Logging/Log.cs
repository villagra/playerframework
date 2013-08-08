using System;
using System.Collections.Generic;

namespace Microsoft.AudienceInsight
{
    /// <summary>
    /// Provides an common interface for all log objects
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Gets a unique Id for the log entry.
        /// </summary>
        Guid Id { get;  }

        /// <summary>
        /// Gets or sets when the log was created
        /// </summary>
        DateTimeOffset TimeStamp { get; set; }

        /// <summary>
        /// Gets a unique identifier for the type of log
        /// </summary>
        string Type { get;  }

        /// <summary>
        /// Gets all log data
        /// </summary>
        IDictionary<string, object> GetData();

        /// <summary>
        /// Gets extra data about the log. This allows you to attach extra data to the log
        /// </summary>
        IDictionary<string, object> ExtraData { get; }
    }

    /// <summary>
    /// EventArgs for any event that needs to pass along a single log object.
    /// </summary>
#if SILVERLIGHT
    public class LogEventArgs : EventArgs
#else
    public sealed class LogEventArgs : Object
#endif
    {
        /// <summary>
        /// Creates a new instance of LogEventArgs.
        /// </summary>
        /// <param name="log">The log object associated with the event.</param>
        public LogEventArgs(ILog log)
        {
            Log = log;
        }

        /// <summary>
        /// Gets the log object associated with the event.
        /// </summary>
        public ILog Log { get; private set; }
    }
}
