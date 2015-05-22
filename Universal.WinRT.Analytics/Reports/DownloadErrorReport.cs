using System;
using System.Collections.Generic;

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// Contains information about a group of download errors. Info about the bitrate is not available.
    /// </summary>
    public sealed class DownloadErrorReport : ILog
    {
        /// <summary>
        /// Creates a new instance of DownloadErrorReport.
        /// </summary>
        public DownloadErrorReport()
        {
            Id = Guid.NewGuid();
            Type = ReportTypes.DownloadErrors;
            TimeStamp = DateTimeOffset.Now;
            ExtraData = new Dictionary<string, object>();
        }

        /// <inheritdoc /> 
        public IDictionary<string, object> ExtraData { get; private set; }

        /// <inheritdoc /> 
        public Guid Id { get; private set; }

        /// <inheritdoc /> 
        public DateTimeOffset TimeStamp { get; set; }

        /// <inheritdoc /> 
        public string Type { get; private set; }

        /// <inheritdoc /> 
        public IDictionary<string, object> GetData()
        {
            var result = this.CreateBasicLogData();
            result.Add("ChunkId", ChunkId);
            result.Add("DownloadErrorCount", Count);
            result.Add("SamplingWindowSeconds", SampleSizeSeconds);
            return result;
        }

        /// <summary>
        /// The index of the time stamp of the chunk
        /// </summary>
        public string ChunkId { get; set; }
        
        /// <summary>
        /// The total number of download errors for the given chunk (across all bitrates) for a given sample.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// The sample size in seconds
        /// </summary>
        public int SampleSizeSeconds { get; set; }

    }
}
