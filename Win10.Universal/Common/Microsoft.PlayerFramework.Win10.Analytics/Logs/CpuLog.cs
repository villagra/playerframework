using System;
using System.Collections.Generic;

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// A log to provide current CPU levels. This log is generated at regular intervals.
    /// </summary>
    public sealed class CpuLog : ILog
    {
        /// <summary>
        /// Creates a new instance of CpuLog.
        /// </summary>
        /// <param name="systemCpu">The CPU load for the entire system</param>
        /// <param name="processCpu">The CPU load for only the process</param>
        public CpuLog(double systemCpu, double processCpu)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.CpuLoad;
            SystemCpu = systemCpu;
            ProcessCpu = processCpu;
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
            result.Add("SystemCpu", SystemCpu);
            result.Add("ProcessCpu", ProcessCpu);
            return result;
        }

        /// <summary>
        /// Gets the number of rendered frames per second
        /// </summary>
        public double SystemCpu { get; private set; }

        /// <summary>
        /// Gets the number of rendered frames per second
        /// </summary>
        public double ProcessCpu { get; private set; }
    }
}
