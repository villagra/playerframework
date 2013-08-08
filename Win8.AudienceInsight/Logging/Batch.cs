using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.AudienceInsight
{
    /// <summary>
    /// Provides an interface for a group of logs to send as a batch to the server.
    /// </summary>
    public interface IBatch : ILog
    {
        /// <summary>
        /// The actual logs
        /// </summary>
        IEnumerable<ILog> Logs { get; }
    }

    /// <summary>
    /// A container for a set of logs. Includes data of it's own.
    /// </summary>
    public sealed class Batch : IBatch
    {
        // used for deserialization (.Deserialize())
        internal Batch()
        {
            Type = "Batch";
            TimeStamp = DateTimeOffset.Now;
            Id = Guid.NewGuid();
            ExtraData = new Dictionary<string, object>();
        }

        /// <inheritdoc /> 
        public IDictionary<string, object> ExtraData { get; private set; }

        /// <inheritdoc /> 
        public Guid Id { get; private set; }

        /// <inheritdoc /> 
        public DateTimeOffset TimeStamp { get; set; }

        /// <inheritdoc /> 
        public string Type { get; private set; }

        /// <inheritdoc /> 
        public IDictionary<string, object> GetData()
        {
            var result = this.CreateBasicLogData();
            result.Remove("LogId"); // rename LogId to BatchId for backward compatibility
            result.Add("BatchId", Id);
            result.Add("ApplicationName", ApplicationName);
            result.Add("ApplicationVersion", ApplicationVersion);
            result.Add("ApplicationId", ApplicationId);
            result.Add("SessionId", SessionId);
            result.Add("InstanceId", InstanceId);
            result.Add("LogsDropped", LogsDropped);
            result.Add("LogsSent", LogsSent);
            result.Add("TotalFailures", TotalFailures);
            return result;
        }

        /// <summary>
        /// The name of the client application. Pulled from the config file.
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// The version of the client application. Pulled from the config file.
        /// </summary>
        public string ApplicationVersion { get; set; }

        /// <summary>
        /// A unique ID for the client application. Pulled from the config file.
        /// </summary>
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// The session ID. This is randomly generated for each session of the app.
        /// </summary>
        public Guid SessionId { get; set; }

        /// <summary>
        /// And instance ID that is persisted locally to help group users across multiple sessions.
        /// </summary>
        public Guid InstanceId { get; set; }

        /// <summary>
        /// The total number of logs dropped because of log failures in the current session
        /// </summary>
        public int LogsDropped { get; set; }

        /// <summary>
        /// The total number of logs sent in the current session
        /// </summary>
        public int LogsSent { get; set; }

        /// <summary>
        /// The total number of failures in the current session
        /// </summary>
        public int TotalFailures { get; set; }

        /// <summary>
        /// The actual logs
        /// </summary>
        public IEnumerable<ILog> Logs { get; set; }
    }

    /// <summary>
    /// EventArgs used to return a batch object.
    /// </summary>
#if SILVERLIGHT
    public class BatchEventArgs : EventArgs
#else
    public sealed class BatchEventArgs : Object
#endif
    {
        /// <summary>
        /// Creates a new instance of BatchEventArgs.
        /// </summary>
        /// <param name="Batch">The batch object associated with the event</param>
        public BatchEventArgs(IBatch Batch)
        {
            this.Batch = Batch;
        }

        /// <summary>
        /// Gets the batch object associated with the event
        /// </summary>
        public IBatch Batch { get; private set; }
    }
}

