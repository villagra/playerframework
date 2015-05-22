using System;
using System.Collections.Generic;

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// A log generated when a download error occurs.
    /// </summary>
    public sealed class DownloadErrorLog : ILog
    {
        /// <summary>
        /// Creates a new instance of DownloadErrorLog.
        /// </summary>
        public DownloadErrorLog()
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.DownloadError;
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
            result.Add("ChunkId", ChunkId);
            result.Add("HttpResponse", HttpResponse);
            return result;
        }

        /// <summary>
        /// Gets a unique ID for the chunk.
        /// </summary>
        public string ChunkId { get; set; }

        /// <summary>
        /// The HTTP response code
        /// </summary>
        public int HttpResponse { get; set; }
    }
}
