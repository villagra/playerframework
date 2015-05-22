using System;
using System.Collections.Generic;

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// Contains all the aggregated values for the sampled quality data
    /// </summary>
    public sealed class QualityReport : ILog
    {
        /// <summary>
        /// Creates a new instance of QualityReport.
        /// </summary>
        public QualityReport()
        {
            Id = Guid.NewGuid();
            Type = ReportTypes.Quaility;
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
            result.Add("VideoDownloadLatencyMilliseconds", VideoDownloadLatencyMilliseconds);
            result.Add("AudioDownloadLatencyMilliseconds", AudioDownloadLatencyMilliseconds);
            result.Add("ProcessCPULoad", ProcessCpuLoad);
            result.Add("SystemCPULoad", SystemCpuLoad);
            result.Add("RenderedFrames", RenderedFrames);
            result.Add("DroppedFrames", DroppedFrames);
            result.Add("PerceivedBandwidth", PerceivedBandwidth);
            result.Add("VideoBufferSize", VideoBufferSize);
            result.Add("AudioBufferSize", AudioBufferSize);
            result.Add("BufferingMilliseconds", BufferingMilliseconds);
            result.Add("BitRateChangeCount", BitrateChangeCount);
            result.Add("BitRate", Bitrate);
            result.Add("MaxBitRate", BitrateMax);
            result.Add("MaxBitRateMilliseconds", BitrateMaxMilliseconds);
            result.Add("SamplingWindowSeconds", SampleSizeSeconds);
            result.Add("DvrOperationCount", DvrOperationCount);
            result.Add("FullScreenChangeCount", FullScreenChangeCount);
            result.Add("HttpErrorCount", HttpErrorCount);
            return result;
        }

        /// <summary>
        /// Average dropped frames per second
        /// </summary>
        public double DroppedFrames { get; set; }

        /// <summary>
        /// Average rendered frames per second
        /// </summary>
        public double RenderedFrames { get; set; }

        /// <summary>
        /// The average CPU load for the current process (e.g. 15 = 15%)
        /// </summary>
        public double ProcessCpuLoad { get; set; }

        /// <summary>
        /// The average CPU load for the entire system (e.g. 15 = 15%)
        /// </summary>
        public double SystemCpuLoad { get; set; }

        /// <summary>
        /// The average bitrate currently being played
        /// </summary>
        public double Bitrate { get; set; }

        /// <summary>
        /// The average perceived bandwidth in bytes per second
        /// </summary>
        public double PerceivedBandwidth { get; set; }

        /// <summary>
        /// The average size of the video buffer in bytes
        /// </summary>
        public double VideoBufferSize { get; set; }

        /// <summary>
        /// The average size of the audio buffer in bytes
        /// </summary>
        public double AudioBufferSize { get; set; }

        /// <summary>
        /// The total duration in milliseconds that the player was buffering
        /// </summary>
        public double BufferingMilliseconds { get; set; }

        /// <summary>
        /// The average latency time for video chunks in milliseconds
        /// </summary>
        public double VideoDownloadLatencyMilliseconds { get; set; }

        /// <summary>
        /// The average latency time for audio chunks in milliseconds
        /// </summary>
        public double AudioDownloadLatencyMilliseconds { get; set; }

        /// <summary>
        /// The max bitrate achieved in the current sampling period
        /// </summary>
        public uint BitrateMax { get; set; }

        /// <summary>
        /// The total amount of time that the max bitrate was achieved within the current sampling period
        /// </summary>
        public double BitrateMaxMilliseconds { get; set; }

        /// <summary>
        /// The total times that the bitrate changed within the current sampling period
        /// </summary>
        public uint BitrateChangeCount { get; set; }

        /// <summary>
        /// The total times that a DVR operation occured
        /// </summary>
        public uint DvrOperationCount { get; set; }

        /// <summary>
        /// The total times that the video came in or out of full screen mode
        /// </summary>
        public uint FullScreenChangeCount { get; set; }

        /// <summary>
        /// The total number of HTTP errors
        /// </summary>
        public uint HttpErrorCount { get; set; }

        /// <summary>
        /// The sample size in seconds
        /// </summary>
        public uint SampleSizeSeconds { get; set; }
    }
}
