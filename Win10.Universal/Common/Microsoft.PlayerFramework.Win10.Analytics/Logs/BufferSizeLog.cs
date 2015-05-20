using System;
using System.Collections.Generic;

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// A log to indicate current buffer levels. This is generated at regular intervals.
    /// </summary>
    public sealed class BufferSizeLog : ILog
    {
        /// <summary>
        /// Creates a new instance of BufferSizeLog.
        /// </summary>
        /// <param name="bufferSize">The size in bytes of the buffer</param>
        /// <param name="streamType">The stream type for the buffer (e.g. 'audio' or 'video')</param>
        public BufferSizeLog(ulong bufferSize, string streamType)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.BufferSize;
            BufferSize = bufferSize;
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
            result.Add("BufferSize", BufferSize);
            result.Add("StreamType", StreamType);
            return result;
        }

        /// <summary>
        /// Gets the size of the buffer (in bytes)
        /// </summary>
        public ulong BufferSize { get; private set; }

        /// <summary>
        /// The stream type (e.g. audio or video)
        /// </summary>
        public string StreamType { get; private set; }
    }
}
