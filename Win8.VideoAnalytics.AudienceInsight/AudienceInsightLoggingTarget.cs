using Microsoft.AudienceInsight;

namespace Microsoft.VideoAnalytics.AudienceInsight
{
    /// <summary>
    /// Adapter class that allows Audience Insight to be an ILoggingTarget
    /// </summary>
    public sealed class AudienceInsightLoggingTarget : ILoggingTarget
    {
        private Microsoft.AudienceInsight.BatchingLogAgent batchingLogAgent { get; set; }

        /// <summary>
        /// Creates a new AudienceInsightLoggingTarget object
        /// </summary>
        /// <param name="batchingLogAgent">The Audience Insight BatchingLogAgent that will receive log data</param>
        public AudienceInsightLoggingTarget(Microsoft.AudienceInsight.BatchingLogAgent batchingLogAgent)
        {
            this.batchingLogAgent = batchingLogAgent;
        }

        /// <inheritdoc />
        public void LogEntry(Microsoft.VideoAnalytics.ILog log)
        {
            if (this.batchingLogAgent == null)
                return;

            var aiLog = new DictionaryLog(log.Id, log.TimeStamp, log.Type, log.GetData());

            this.batchingLogAgent.LogEntry(aiLog);
        }
    }
}
