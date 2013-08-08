using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AudienceInsight;

namespace Microsoft.VideoAnalytics.AudienceInsight
{
    public sealed class AudienceInsightLoggingTarget : ILoggingTarget
    {
        public Microsoft.AudienceInsight.BatchingLogAgent BatchingLogAgent { get; set; }

        public AudienceInsightLoggingTarget(Microsoft.AudienceInsight.BatchingLogAgent batchingLogAgent)
        {
            this.BatchingLogAgent = batchingLogAgent;
        }
        
        public void LogEntry(Microsoft.VideoAnalytics.ILog log)
        {
            if (this.BatchingLogAgent == null)
                return;

            var aiLog = new DictionaryLog(log.Id, log.TimeStamp, log.Type, log.GetData());

            this.BatchingLogAgent.LogEntry(aiLog);
        }
    }
}
