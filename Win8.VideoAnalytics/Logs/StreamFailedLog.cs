using System;
using System.Collections.Generic;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// A log generated when there is an error playing back media (e.g. the internet connection is lost for streaming media).
    /// </summary>
    public sealed class StreamFailedLog : ILog
    {
        /// <summary>
        /// Creates a new instance of StreamFailedLog.
        /// </summary>
        /// <param name="reason">The reason for the failure.</param>
        public StreamFailedLog(string reason)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.StreamFailed;
            Reason = reason;
            Id = Guid.NewGuid();
            ExtraData = new Dictionary<string, object>();
        }

        /// <inheritdoc /> 
        public IDictionary<string, object> ExtraData { get; private set; }

        /// <inheritdoc /> 
        public Guid Id { get; set; }

        /// <inheritdoc /> 
        public DateTimeOffset TimeStamp { get; set; }

        /// <inheritdoc /> 
        public string Type { get; private set; }

        /// <inheritdoc /> 
        public IDictionary<string, object> GetData()
        {
            var result = this.CreateBasicLogData();
            result.Add("Reason", Reason);
            return result;
        }

        /// <summary>
        /// Gets the reason why the media failed
        /// </summary>
        public string Reason { get; private set; }

    }
}
