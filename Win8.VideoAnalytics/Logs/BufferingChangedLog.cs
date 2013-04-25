using System;
using System.Collections.Generic;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// A log generated when buffering of playback either starts or stops.
    /// </summary>
    public sealed class BufferingChangedLog : ILog
    {
        /// <summary>
        /// Creates a new instance of BufferingChangedLog.
        /// </summary>
        /// <param name="isBuffering">Whether or not the media is currently buffering</param>
        public BufferingChangedLog(bool isBuffering)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.BufferingStateChanged;
            IsBuffering = isBuffering;
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
            result.Add("IsBuffering", IsBuffering);
            return result;
        }

        /// <summary>
        /// Gets if the player was changing to a buffering state or not.
        /// </summary>
        public bool IsBuffering { get; private set; }
    }
}
