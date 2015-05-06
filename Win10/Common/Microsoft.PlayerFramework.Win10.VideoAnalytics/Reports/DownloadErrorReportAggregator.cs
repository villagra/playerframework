using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// Responsible for aggregating all of the http chunk download errors that occur and group them by stream type and chunk id (aka start time)
    /// </summary>
    internal class DownloadErrorReportAggregator : ReportAggregator
    {
        public IEnumerable<DownloadErrorReport> GetReport(TimeSpan duration)
        {
            var now = DateTimeOffset.Now;
            var windowStart = now.Subtract(duration);

            var entries = base.GetAllEntries();

            var sampleSizeSeconds = (int)duration.TotalSeconds;

            foreach (var group in GetEntries<DownloadErrorLog>(entries, windowStart, now, false, l => true).GroupBy(l => l.ChunkId))
            {
                var chunk = group.First(); // we can assume all chunks are the same
                yield return new DownloadErrorReport()
                {
                    ChunkId = chunk.ChunkId,
                    Count = group.Count(),
                    SampleSizeSeconds = sampleSizeSeconds
                };
            }
        }
    }
}
