using System;
using System.Collections.Generic;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// A log generated when the stream is first loaded.
    /// </summary>
    public sealed class StreamLoadedLog : ILog
    {
        /// <summary>
        /// Creates a new instance of StreamLoadedLog.
        /// </summary>
        /// <param name="source">The source URI for the media.</param>
        public StreamLoadedLog(Uri source)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.StreamLoaded;
            Source = source;
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
            result.Add("EdgeServer", EdgeServer);
            result.Add("ClientIp", ClientIp);
            result.Add("Source", Source);
            result.Add("MaxBitrate", MaxBitrate);
            result.Add("MinBitrate", MinBitrate);
            return result;
        }

        /// <summary>
        /// Gets the edge server IP or address
        /// </summary>
        public string EdgeServer { get; internal set; }

        /// <summary>
        /// Gets the client IP
        /// </summary>
        public string ClientIp { get; internal set; }

        /// <summary>
        /// Gets the source of the media
        /// </summary>
        public Uri Source { get; private set; }

        /// <summary>
        /// Gets the maximum bitrate for the stream
        /// </summary>
        public ulong MaxBitrate { get; internal set; }

        /// <summary>
        /// Gets the minimum bitrate for the stream
        /// </summary>
        public ulong MinBitrate { get; internal set; }
    }
}
