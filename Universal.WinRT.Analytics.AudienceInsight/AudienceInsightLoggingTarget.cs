using Microsoft.Media.AudienceInsight;

namespace Microsoft.Media.Analytics.AudienceInsight
{
    /// <summary>
    /// Adapter class that allows Audience Insight to be an ILoggingTarget
    /// </summary>
    public sealed class AudienceInsightLoggingTarget : ILoggingTarget
    {
        private Microsoft.Media.AudienceInsight.BatchingLogAgent batchingLogAgent { get; set; }

        /// <summary>
        /// Creates a new AudienceInsightLoggingTarget object
        /// </summary>
        /// <param name="batchingLogAgent">The Audience Insight BatchingLogAgent that will receive log data</param>
        public AudienceInsightLoggingTarget(Microsoft.Media.AudienceInsight.BatchingLogAgent batchingLogAgent)
        {
            this.batchingLogAgent = batchingLogAgent;
        }

        /// <inheritdoc />
        public void LogEntry(Microsoft.Media.Analytics.ILog log)
        {
            if (this.batchingLogAgent == null)
                return;

            var aiLog = new Microsoft.Media.AudienceInsight.Log(log.Id, log.TimeStamp, log.Type, log.GetData());

            this.batchingLogAgent.LogEntry(aiLog);
        }
    }
}
