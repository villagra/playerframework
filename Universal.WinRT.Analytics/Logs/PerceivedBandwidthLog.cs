using System;
using System.Collections.Generic;

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// A log to indicate the perceived bandwidth. This is generated at regular intervals.
    /// </summary>
    public sealed class PerceivedBandwidthLog : ILog
    {
        /// <summary>
        /// Creates a new instance of PerceivedBandwidthLog.
        /// </summary>
        /// <param name="perceivedBandwidth">The perceived bandwidth in bps at the time of the log.</param>
        public PerceivedBandwidthLog(uint perceivedBandwidth)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.PerceivedBandwidth;
            PerceivedBandwidth = perceivedBandwidth;
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
            result.Add("PerceivedBandwidth", PerceivedBandwidth);
            return result;
        }

        /// <summary>
        /// Gets the PerceivedBandwidth in bps
        /// </summary>
        public uint PerceivedBandwidth { get; private set; }
    }
}
