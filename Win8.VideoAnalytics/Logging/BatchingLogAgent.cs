using System;
using System.Linq;
using System.Collections.Generic;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// A special kind of LogAgent that is capable of batching logs.
    /// It will queue all logs, polling from that queue on a separate thread, and batch the results.
    /// Ultimately, the logs (wrapped in a Batch object) are passed onto an implementation of IBatchAgent.
    /// </summary>
    public sealed class BatchingLogAgent : ILoggingTarget, IDisposable
    {
        /// <summary>
        /// Fired when a handled exception occurs
        /// </summary>
        public event EventHandler<BatchingErrorEventArgs> BatchException;

        /// <summary>
        /// Fired before the batch is sent
        /// </summary>
        public event EventHandler<BatchEventArgs> BatchSending;

        /// <summary>
        /// Fired after the batch has been successfully sent
        /// </summary>
        public event EventHandler<BatchEventArgs> BatchSendSuccess;

        /// <summary>
        /// Fired after the batch failed to send
        /// </summary>
        public event EventHandler<BatchEventArgs> BatchSendFailed;

        /// <summary>
        /// Fired when more logs have been dropped
        /// </summary>
        public event EventHandler<object> TotalLogsDroppedChanged;

        private BatchingLogAgentStates state;
        private Queue<ILog> queue;
        private QueueManager queueManager;
        private Uri DefaultConfigUri = new Uri("LoggingConfiguration.xml", UriKind.Relative);

        internal BatchingLogAgent()
        {
            queue = new Queue<ILog>();

            state = BatchingLogAgentStates.Disabled;
        }

        /// <summary>
        /// Creates a new instance of BatchingLogAgent from the required configuration.
        /// </summary>
        /// <param name="Config">The required configuration object.</param>
        public BatchingLogAgent(BatchingConfig Config)
            : this()
        {
            if (Config == null) throw new ArgumentNullException("Config");
            Configuration = Config;
            if (Configuration.MappingRules != null)
            {
                Configuration.MappingRules.MappingError += MappingRules_MappingError;
            }
            StartSession();
        }

        void MappingRules_MappingError(object sender, BatchingErrorEventArgs args)
        {
            if (BatchException != null) BatchException(this, args);
        }

        /// <summary>
        /// Used for debugging. All ignorable exceptions that occur in the process of logging are sent here.
        /// </summary>
        public void BroadcastException(Exception ex)
        {
            if (BatchException != null) BatchException(this, new BatchingErrorEventArgs(ex));
        }

        /// <summary>
        /// Indicates that the agent is currently running
        /// </summary>
        public bool IsSessionStarted
        {
            get
            {
                return state == BatchingLogAgentStates.Enabled;
            }
        }

        /// <summary>
        /// Used to spin up the agent.
        /// </summary>
        /// <returns>Indicates success</returns>
        public bool StartSession()
        {
            if (state == BatchingLogAgentStates.Enabled)
                throw new Exception("Session is already started");

            try
            {
                if (Configuration.LoggingEnabled)
                {
                    // initialize new queue manager
                    queueManager = new QueueManager(this);
                    queueManager.BatchSending += queueManager_BatchSending;
                    queueManager.BatchSendSuccess += queueManager_BatchSendSuccess;
                    queueManager.BatchSendFailed += queueManager_BatchSendFailed;

                    // set SessionId & StartTime
                    SessionId = Guid.NewGuid();
                    SessionStartTime = DateTimeOffset.Now;

                    // enable logging
                    state = BatchingLogAgentStates.Enabled;
                }
                else
                    state = BatchingLogAgentStates.Disabled;

                return true;
            }
            catch (Exception ex)
            {
                BroadcastException(ex);
                state = BatchingLogAgentStates.Disabled;
                return false;
            }
        }

        /// <summary>
        /// Stops the agent.
        /// </summary>
        public void StopSession()
        {
            if (queueManager != null)
            {
                queueManager.Dispose();
                queueManager = null;
            }
            state = BatchingLogAgentStates.Disabled;
        }

        void queueManager_BatchSendFailed(object sender, BatchEventArgs e)
        {
            if (BatchSendFailed != null)
                BatchSendFailed(this, e);
        }

        void queueManager_BatchSending(object sender, BatchEventArgs e)
        {
            if (BatchSending != null)
                BatchSending(this, e);
        }

        void queueManager_BatchSendSuccess(object sender, BatchEventArgs e)
        {
            if (BatchSendSuccess != null)
                BatchSendSuccess(this, e);
        }

        /// <summary>
        /// The current generated session ID.
        /// </summary>
        public Guid SessionId { get; private set; }

        /// <summary>
        /// The Timestamp of when the session started
        /// </summary>
        public DateTimeOffset SessionStartTime { get; private set; }

        /// <summary>
        /// The server time offset (used to correct the local time so the server can get accurate time)
        /// </summary>
        public TimeSpan? ServerTimeOffset { get; internal set; }

        /// <summary>
        /// The current duration of the session
        /// </summary>
        public TimeSpan SessionDuration { get { return DateTimeOffset.Now.Subtract(SessionStartTime); } }

        /// <summary>
        /// Adds a log to the queue
        /// </summary>
        /// <param name="log">The new log being logged. This has already passed the filter condition if one was specified in the MappingPolicy.</param>
        public void LogEntry(ILog log)
        {
            try
            {
                if (State == BatchingLogAgentStates.Enabled)
                    // check to see if we have reached our max events threshold, if over we disable logging
                    if (Configuration.MaxSessionLogs.HasValue && TotalLogsQueued > Configuration.MaxSessionLogs.Value)
                    {
                        IncrementTotalLogsDropped(1);
                        State = BatchingLogAgentStates.Disabled;
                        return;
                    }
                    // check to see if we already have too many events to log
                    else if (Configuration.MaxQueueLength.HasValue && queue.Count > Configuration.MaxQueueLength.Value)
                    {
                        IncrementTotalLogsDropped(1);
                        return;
                    }
                    else
                    {
                        Enqueue(log);
                        IncrementTotalLogsQueued(1);
                    }
                else
                {
                    IncrementTotalLogsDropped(1);
                }
            }
            catch (Exception ex)
            {
                IncrementTotalLogsDropped(1);
                BroadcastException(ex);
            }
        }

        /// <summary>
        /// The config object to use. It is not recommended you change this while the session is running.
        /// </summary>
        public BatchingConfig Configuration { get; set; }

        /// <summary>
        /// The state of the agent.
        /// </summary>
        public BatchingLogAgentStates State
        {
            get { return state; }
            internal set { state = value; }
        }

        /// <summary>
        /// The total number of dropped logs
        /// </summary>
        public int TotalLogsDropped { get; private set; }
        private readonly object dropCountLock = new object();
        internal void IncrementTotalLogsDropped(int amount)
        {
            lock (dropCountLock)
            {
                TotalLogsDropped += amount;
            }
            // raise event
            if (TotalLogsDroppedChanged != null)
                TotalLogsDroppedChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// The total number of logs sent
        /// </summary>
        public int TotalLogsSent { get; private set; }
        private readonly object sentCountLock = new object();
        internal void IncrementTotalLogsSent(int amount)
        {
            lock (sentCountLock)
            {
                TotalLogsSent += amount;
            }
        }

        /// <summary>
        /// The total number of logs currently in the queue
        /// </summary>
        public int TotalLogsQueued { get; private set; }
        private readonly object queuedCountLock = new object();
        internal void IncrementTotalLogsQueued(int amount)
        {
            lock (queuedCountLock)
            {
                TotalLogsQueued += amount;
            }
        }

        /// <summary>
        /// The total number of batches sent
        /// </summary>
        public int TotalBatchesSent { get; private set; }
        private readonly object batchesSentCountLock = new object();
        internal void IncrementTotalBatchesSent(int amount)
        {
            lock (batchesSentCountLock)
            {
                TotalBatchesSent += amount;
            }
        }

        /// <summary>
        /// The total number of batches that failed to get sent.
        /// </summary>
        public int TotalBatchesFailed { get; private set; }
        private readonly object batchesFailedCountLock = new object();
        internal void IncrementTotalBatchesFailed(int amount)
        {
            lock (batchesFailedCountLock)
            {
                TotalBatchesFailed += amount;
            }
        }

        internal bool HasLogs()
        {
            lock (queue)
            {
                return queue.Any();
            }
        }

        private void Enqueue(ILog log)
        {
            lock (queue)
            {
                queue.Enqueue(log);
            }
        }

        internal IBatch MapBatchAndLogs(Batch batch)
        {
            lock (queue)
            {
                return MapBatchAndLogs(batch, queue);
            }
        }

        private IBatch MapBatchAndLogs(Batch batch, Queue<ILog> logs)
        {
            if (Configuration.MappingRules == null)
            {
                int logCount = 0;
                var batchLogs = new List<ILog>();
                while (logs.Any())
                {
                    batchLogs.Add(logs.Dequeue());
                    logCount++;
                    if (Configuration.MaxBatchLength.HasValue && logCount > Configuration.MaxBatchLength.Value) break;
                }
                if (logCount > 0)
                {
                    batch.Logs = batchLogs;
                    return batch;
                }
            }
            else
            {
                IDictionary<string, object> data = Configuration.MappingRules.Map(batch).FirstOrDefault();
                if (data != null)
                {
                    int logCount = 0;
                    var mappedLogs = new List<ILog>();
                    while (logs.Any())
                    {
                        var log = logs.Dequeue();
                        var mappedLogsData = Configuration.MappingRules.Map(log).ToList();
                        if (mappedLogsData.Any())
                        {
                            foreach (var logData in mappedLogsData)
                            {
                                var mappedLog = new MappedLog(log, logData);
                                if (ServerTimeOffset.HasValue)
                                {
                                    mappedLog.TimeStamp = mappedLog.TimeStamp.Add(ServerTimeOffset.Value);
                                }
                                mappedLogs.Add(mappedLog);
                            }
                            logCount++;
                            if (Configuration.MaxBatchLength.HasValue && logCount > Configuration.MaxBatchLength.Value) break;
                        }
                    }

                    if (logCount > 0)
                    {
                        var result = new MappedBatch(batch, data, mappedLogs);
                        if (ServerTimeOffset.HasValue)
                        {
                            result.TimeStamp = result.TimeStamp.Add(ServerTimeOffset.Value);
                        }
                        return result;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// The filter used by the LoggingService to know which logs to pass to the .Log function.
        /// </summary>
        public ILogFilter Filter
        {
            get { return Configuration.MappingRules; }
        }

        /// <inheritdoc /> 
        public void Dispose()
        {
            if (Configuration.MappingRules != null)
            {
                Configuration.MappingRules.MappingError -= MappingRules_MappingError;
            }
            if (IsSessionStarted)
                StopSession();
        }
    }

    /// <summary>
    /// The possible states for the log agent.
    /// </summary>
    public enum BatchingLogAgentStates
    {
        /// <summary>
        /// The log agent is in a failed state.
        /// </summary>
        Failed = 0,
        /// <summary>
        /// The log agent is enabled.
        /// </summary>
        Enabled = 1,
        /// <summary>
        /// The log agent is disabled.
        /// </summary>
        Disabled = 3,
    }
}
