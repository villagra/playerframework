using System;
using System.Collections.Generic;

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// A log generated when the latency drops below a certain threshold as defined in the AnalyticsConfig.LatencyAlertThreshold.
    /// </summary>
    public sealed class LatencyAlertLog : ILog
    {
        /// <summary>
        /// Creates a new instance of LatencyAlertLog.
        /// </summary>
        /// <param name="bitrate">The bitrate of the chunk</param>
        /// <param name="streamType">The type of stream (e.g. 'audio' or 'video')</param>
        /// <param name="chunkIndex">The index of the chunk.</param>
        /// <param name="startTime">The start time of the chunk.</param>
        /// <param name="downloadTimeMs">The total time in milliseconds for the chunk to download.</param>
        public LatencyAlertLog(uint bitrate, string streamType, int chunkIndex, ulong startTime, uint downloadTimeMs)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.LatencyAlert;
            BitRate = bitrate;
            StreamType = streamType;
            ChunkIndex = chunkIndex;
            StartTime = startTime;
            DownloadTimeMs = downloadTimeMs;
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
            result.Add("BitRate", BitRate);
            result.Add("ChunkIndex", ChunkIndex);
            result.Add("StreamType", StreamType);
            result.Add("StartTime", StartTime);
            result.Add("DownloadTimeMs", DownloadTimeMs);
            return result;
        }

        /// <summary>
        /// The bitrate of the chunk being downloaded
        /// </summary>
        public uint BitRate { get; set; }

        /// <summary>
        /// The index of the chunk
        /// </summary>
        public int ChunkIndex { get; set; }

        /// <summary>
        /// The stream type (e.g. audio or video)
        /// </summary>
        public string StreamType { get; set; }

        /// <summary>
        /// The timestamp of the chunk. Matches the timestamp that is part of the url for the chunk itself.
        /// </summary>
        public ulong StartTime { get; set; }

        /// <summary>
        /// The time it took in milliseconds to download the chunk
        /// </summary>
        public uint DownloadTimeMs { get; set; }
    }
}
