using System;
using System.Collections.Generic;

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// A log generated when the current bitrate changes.
    /// </summary>
    public sealed class BitrateChangedLog : ILog
    {
        /// <summary>
        /// Creates a new instance of BitrateChangedLog.
        /// </summary>
        /// <param name="bitrate">The new bitrate</param>
        /// <param name="streamType">The stream type associated with the bitrate (e.g. 'audio' or 'video')</param>
        public BitrateChangedLog(uint bitrate, string streamType)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.BitrateChanged;
            Bitrate = bitrate;
            StreamType = streamType;
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
            result.Add("Bitrate", Bitrate);
            result.Add("StreamType", StreamType);
            return result;
        }

        /// <summary>
        /// Gets the new bitrate (in bps)
        /// </summary>
        public uint Bitrate { get; private set; }

        /// <summary>
        /// The stream type (e.g. audio or video)
        /// </summary>
        public string StreamType { get; private set; }
    }
}
