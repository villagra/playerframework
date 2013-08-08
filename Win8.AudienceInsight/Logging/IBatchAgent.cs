using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#if !SILVERLIGHT
using Windows.Foundation;
#endif

namespace Microsoft.AudienceInsight
{
    /// <summary>
    /// Represents an interface to send batched data to a server.
    /// </summary>
    public interface IBatchAgent
    {
#if SILVERLIGHT
        /// <summary>
        /// Called when a batch of logs are ready to be logged.
        /// </summary>
        /// <param name="batch">The batch to be logged</param>
        /// <param name="c">The cancellation token to allow the async operation to be canceled</param>
        /// <returns>An awaitable result from sending the batch.</returns>
        Task<LogBatchResult> SendBatchAsync(IBatch batch, CancellationToken c);
#else
        /// <summary>
        /// Called when a batch of logs are ready to be logged.
        /// </summary>
        /// <param name="batch">The batch to be logged</param>
        /// <returns>An awaitable result from sending the batch.</returns>
        IAsyncOperation<LogBatchResult> SendBatchAsync(IBatch batch);
#endif
    }

    /// <summary>
    /// The result from a successful async batch log operation.
    /// </summary>
    public sealed class LogBatchResult
    {
        /// <summary>
        /// Indicates that the main log agent should continue to run. This is essentiall a kill switch.
        /// </summary>
        public bool? IsEnabled { get; set; }

        /// <summary>
        /// The new polling interval. Overrides the one specified in the config.
        /// </summary>
        public TimeSpan? QueuePollingInterval { get; set; }

        /// <summary>
        /// The server time. Used to calibrate the timestamp sent on the logs.
        /// </summary>
        public DateTimeOffset? ServerTime { get; set; }
    }
}
