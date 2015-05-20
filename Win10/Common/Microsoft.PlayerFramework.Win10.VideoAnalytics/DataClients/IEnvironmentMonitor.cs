using System;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// Provides an interface that can be implemented in order to provide information about the system and environment.
    /// </summary>
    public interface IEnvironmentMonitor
    {
        /// <summary>
        /// Gets the current CPU load of the process.
        /// </summary>
        double ProcessCpuLoad { get; }

        /// <summary>
        /// Gets the current CPU load of the entire system.
        /// </summary>
        double SystemCpuLoad { get; }
    }
}
