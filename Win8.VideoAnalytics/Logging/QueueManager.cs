using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#if SILVERLIGHT
#else
using Windows.System.Threading;
#endif

namespace Microsoft.VideoAnalytics
{
    internal class QueueManager : IDisposable
    {
        /// <summary>
        /// Batch is being sent to BatchAgent
        /// </summary>
        public event EventHandler<BatchEventArgs> BatchSending;

        /// <summary>
        /// Batch was successfully sent via BatchAgent
        /// </summary>
        public event EventHandler<BatchEventArgs> BatchSendSuccess;

        /// <summary>
        /// Batch failed to send
        /// </summary>
        public event EventHandler<BatchEventArgs> BatchSendFailed;

        private BatchingLogAgent logAgent;
        private IBatchAgent dataClient;

        private int failedSendCount;
        private int sendCount;
#if SILVERLIGHT
        private Timer pollingTimer;  // timer for polling queue
#else
        private ThreadPoolTimer pollingTimer;  // timer for polling queue
#endif
        private bool IsThrottled = false;
        private IBatch BatchToRetry;
        private int RetryCount = 0;

        private TimeSpan queuePollingInterval;
        public TimeSpan QueuePollingInterval
        {
            get { return queuePollingInterval; }
            set
            {
                if (queuePollingInterval != value)
                {
                    queuePollingInterval = value;
                    if (pollingTimer != null)
                    {
                        // adjust the timer with the polling interval in case it failed
#if SILVERLIGHT
                        pollingTimer.Dispose();
                        pollingTimer = new Timer(pollingTimer_Tick, null, QueuePollingInterval, QueuePollingInterval);
#else
                        pollingTimer.Cancel();
                        pollingTimer = ThreadPoolTimer.CreatePeriodicTimer(pollingTimer_Tick, QueuePollingInterval);
#endif
                    }
                }
            }
        }

        public QueueManager(BatchingLogAgent LogAgent)
        {
            logAgent = LogAgent;
            dataClient = logAgent.Configuration.BatchAgent;

            QueuePollingInterval = logAgent.Configuration.QueuePollingInterval;

            failedSendCount = 0;
            sendCount = 0;
#if SILVERLIGHT
            pollingTimer = new Timer(pollingTimer_Tick, null, QueuePollingInterval, QueuePollingInterval);
#else
            pollingTimer = ThreadPoolTimer.CreatePeriodicTimer(pollingTimer_Tick, QueuePollingInterval);
#endif
        }

        /// <summary>
        /// The total number of send failures
        /// </summary>
        public int FailedSendCount
        {
            get { return failedSendCount; }
            internal set
            {
                failedSendCount = value;

                if (logAgent.Configuration.MaxSendErrors.HasValue && failedSendCount >= logAgent.Configuration.MaxSendErrors.Value)
                {
                    // we are done forever, shut it down!
                    logAgent.State = BatchingLogAgentStates.Disabled;
                    QueuePollingInterval = TimeSpan.Zero;
                }
                else if (logAgent.Configuration.MaxSendErrorsThrottled.HasValue && failedSendCount >= logAgent.Configuration.MaxSendErrorsThrottled.Value)
                {
                    if (!IsThrottled)
                    {
                        // we reached a critical threshold, throttle the polling interval
                        IsThrottled = true;    // this is used to ignore the response from the server that would otherwise change this interval
                        QueuePollingInterval = logAgent.Configuration.QueuePollingIntervalThrottled;
                    }
                }
                else if (IsThrottled)
                {
                    // we came back from our lethargic state, start speeding things up again
                    IsThrottled = false;
                    QueuePollingInterval = logAgent.Configuration.QueuePollingInterval;
                }
            }
        }

        /// <summary>
        /// The total number of batches sent
        /// </summary>
        public int SendCount
        {
            get
            {
                return (sendCount);
            }
            internal set
            {
                sendCount = value;
            }
        }

        bool isProcessing = false;
        async void pollingTimer_Tick(object sender)
        {
            if (!isProcessing) // ignore reentrance
            {
                isProcessing = true;
                try
                {
                    await ProcessQueue();
                }
                finally { isProcessing = false; }
            }
        }

        private async Task ProcessQueue()
        {
            IBatch batchToSend = null;
            try
            {
                if (BatchToRetry != null)
                {
                    // we have a batch that need to be resent, try again
                    batchToSend = BatchToRetry;
                    BatchToRetry = null; // this will get reset if there's a problem
                }
                else
                {
                    RetryCount = 0;
                    if (logAgent.HasLogs())
                    {
                        var batch = await CreateBatch();
                        // apply mappings. If mappings are defined, it will create new instance of the batch and the logs using different keys and dropping come elements.
                        batchToSend = logAgent.MapBatchAndLogs(batch);
                    }
                }
                if (batchToSend != null)
                {
                    await SendBatch(batchToSend);
                }
            }
            catch (Exception ex)
            {
                FailedSendCount++;
                if (!logAgent.Configuration.MaxRetries.HasValue || RetryCount < logAgent.Configuration.MaxRetries.Value)
                {
                    BatchToRetry = batchToSend;
                    RetryCount++;
                }
                else // we exceeded the max number of retries, time to move on
                {
                    BatchToRetry = null;
                    RetryCount = 0;
                    logAgent.IncrementTotalLogsDropped(batchToSend.Logs.Count());
                }
                logAgent.BroadcastException(ex);
            }
        }

        private async Task<Batch> CreateBatch()
        {
            var batch = new Batch();
            batch.ApplicationName = logAgent.Configuration.ApplicationName;
            batch.ApplicationVersion = logAgent.Configuration.ApplicationVersion;
            batch.ApplicationId = logAgent.Configuration.ApplicationId;
            batch.SessionId = logAgent.SessionId;
            batch.InstanceId = await InstanceDataClient.GetInstanceId();

            batch.TotalFailures = FailedSendCount;
            batch.LogsDropped = logAgent.TotalLogsDropped;
            batch.LogsSent = logAgent.TotalLogsSent;

            return batch;
        }

        private async Task SendBatch(IBatch batch)
        {
            if (batch != null && batch.Logs != null && batch.Logs.Count() > 0)
            {
                try
                {
                    if (BatchSending != null) BatchSending(this, new BatchEventArgs(batch));
                }
                catch { /* swallow */ }

                SendCount++;

                // the data client does the work of actually sending the info to the server
                try
                {
#if SILVERLIGHT
                    var result = await dataClient.SendBatchAsync(batch, CancellationToken.None);
#else
                    var result = await dataClient.SendBatchAsync(batch);
#endif

                    logAgent.IncrementTotalLogsSent(batch.Logs.Count());
                    logAgent.IncrementTotalBatchesSent(1);

                    if (result != null)
                    {
                        if (result.IsEnabled.HasValue)
                            logAgent.State = result.IsEnabled.Value ? BatchingLogAgentStates.Enabled : BatchingLogAgentStates.Disabled;
                        if (result.QueuePollingInterval.HasValue && !IsThrottled)
                            QueuePollingInterval = result.QueuePollingInterval.Value;
                        if (result.ServerTime.HasValue)
                        {
                            if (!logAgent.ServerTimeOffset.HasValue)
                            {
                                logAgent.ServerTimeOffset = result.ServerTime.Value.Subtract(batch.TimeStamp);
                            }
                        }
                    }

                    // decrement the FailedSendCount
                    FailedSendCount = Math.Max(FailedSendCount - 1, 0);
                    try
                    {
                        if (BatchSendSuccess != null) BatchSendSuccess(this, new BatchEventArgs(batch));
                    }
                    catch { /* swallow */ }
                }
                catch
                {
                    logAgent.IncrementTotalBatchesFailed(1);
                    try
                    {
                        if (BatchSendFailed != null) BatchSendFailed(this, new BatchEventArgs(batch));
                    }
                    catch { /* swallow */ }
                    throw;
                }
            }
        }

        /// <summary>
        /// Cleans up the queue manager
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
#if SILVERLIGHT
                pollingTimer.Dispose();
#else
                pollingTimer.Cancel();
#endif
                pollingTimer = null;
            }
            BatchToRetry = null;
            logAgent = null;
            dataClient = null;
        }
    }
}
