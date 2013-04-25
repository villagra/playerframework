using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VideoAnalytics
{
    internal class QualityReportAggregator : ReportAggregator
    {
        private DateTimeOffset? LastReportTime;

        public QualityConfig QualityConfig { get; set; }

        public QualityReportAggregator()
        { }

        public QualityReportAggregator(QualityConfig qualityConfig)
        {
            QualityConfig = qualityConfig;
        }

        public QualityReport GetReport(TimeSpan duration)
        {
            var now = DateTimeOffset.Now;
            var windowStart = LastReportTime.GetValueOrDefault(now.Subtract(duration));
            var result = new QualityReport();

            var entries = base.GetAllEntries();

            if (QualityConfig.AudioBufferSize)
            {
                result.AudioBufferSize = GetEntries<BufferSizeLog>(entries, windowStart, now, true, l => l.StreamType == "audio")
                    .Select(s => (double)s.BufferSize)
                    .DefaultIfEmpty(0.0)
                    .Average();
            }

            if (QualityConfig.VideoBufferSize)
            {
                result.VideoBufferSize = GetEntries<BufferSizeLog>(entries, windowStart, now, true, l => l.StreamType == "video")
                    .Select(s => (double)s.BufferSize)
                    .DefaultIfEmpty(0.0)
                    .Average();
            }

            if (QualityConfig.AudioDownloadLatency)
            {
                result.AudioDownloadLatencyMilliseconds = GetEntries<ChunkDownloadLog>(entries, windowStart, now, false, l => l.StreamType == "audio")
                    .Select(s => (double)s.DownloadTimeMs)
                    .DefaultIfEmpty(0.0)
                    .Average();
            }

            if (QualityConfig.VideoDownloadLatency)
            {
                result.VideoDownloadLatencyMilliseconds = GetEntries<ChunkDownloadLog>(entries, windowStart, now, false, l => l.StreamType == "video")
                    .Select(s => (double)s.DownloadTimeMs)
                    .DefaultIfEmpty(0.0)
                    .Average();
            }

            if (QualityConfig.Bitrate)
            {
                result.Bitrate = GetSamples<BitrateChangedLog>(entries, windowStart, now, true, l => l.StreamType == "video")
                    .WeightedAverage(s => (double)s.Entry.Bitrate, s => s.Duration.TotalSeconds);
            }

            if (QualityConfig.BitrateChangeCount)
            {
                result.BitrateChangeCount = (uint)GetEntries<BitrateChangedLog>(entries, windowStart, now, false, l => l.StreamType == "video")
                    .Count();
            }

            if (QualityConfig.BitrateMax)
            {
                result.BitrateMax = GetEntries<BitrateChangedLog>(entries, windowStart, now, true, l => l.StreamType == "video")
                    .Select(s => s.Bitrate)
                    .DefaultIfEmpty<uint>(0)
                    .Max();
            }

            if (QualityConfig.BitrateMaxDuration)
            {
                result.BitrateMaxMilliseconds = GetSamples<BitrateChangedLog>(entries, windowStart, now, true, l => l.StreamType == "video" && l.Bitrate == result.BitrateMax)
                    .Sum(s => s.Duration.TotalMilliseconds);
            }

            if (QualityConfig.Buffering)
            {
                result.BufferingMilliseconds = GetSamples<BufferingChangedLog>(entries, windowStart, now, true, l => l.IsBuffering)
                    .Sum(s => s.Duration.TotalMilliseconds);
            }

            if (QualityConfig.DroppedFrames)
            {
                result.DroppedFrames = GetEntries<FpsLog>(entries, windowStart, now, true, l => true)
                    .Select(s => s.DroppedFramesPerSecond)
                    .DefaultIfEmpty(0.0)
                    .Average();
            }

            if (QualityConfig.RenderedFrames)
            {
                result.RenderedFrames = GetEntries<FpsLog>(entries, windowStart, now, true, l => true)
                    .Select(s => s.RenderedFramesPerSecond)
                    .DefaultIfEmpty(0.0)
                    .Average();
            }

            if (QualityConfig.DvrOperationCount)
            {
                result.DvrOperationCount = (uint)GetEntries<DvrOperationLog>(entries, windowStart, now, true, l => l.OperationType != DvrOperationType.ScrubStarted)
                    .Count();
            }

            if (QualityConfig.FullScreenChangeCount)
            {
                result.FullScreenChangeCount = (uint)GetEntries<FullscreenChangedLog>(entries, windowStart, now, true, l => true)
                    .Count();
            }

            if (QualityConfig.DownloadErrorCount)
            {
                result.HttpErrorCount = (uint)GetEntries<DownloadErrorLog>(entries, windowStart, now, true, l => true)
                    .Count();
            }

            if (QualityConfig.PerceivedBandwidth)
            {
                result.PerceivedBandwidth = (long)GetEntries<PerceivedBandwidthLog>(entries, windowStart, now, true, l => true)
                    .Select(s => (double)s.PerceivedBandwidth)
                    .DefaultIfEmpty(0.0)
                    .Average();
            }

            if (QualityConfig.ProcessCpuLoad)
            {
                result.ProcessCpuLoad = (long)GetEntries<CpuLog>(entries, windowStart, now, true, l => true)
                    .Select(s => (double)s.ProcessCpu)
                    .DefaultIfEmpty(0.0)
                    .Average();
            }

            if (QualityConfig.SystemCpuLoad)
            {
                result.SystemCpuLoad = (long)GetEntries<CpuLog>(entries, windowStart, now, true, l => true)
                    .Select(s => (double)s.SystemCpu)
                    .DefaultIfEmpty(0.0)
                    .Average();
            }

            result.SampleSizeSeconds = (uint)now.Subtract(windowStart).TotalSeconds;
            LastReportTime = now;

            return result;
        }
    }
}
