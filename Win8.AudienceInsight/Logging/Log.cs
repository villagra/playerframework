using System;
using System.Collections.Generic;
#if WINDOWS_PHONE
#else
using Windows.Data.Json;
#endif

namespace Microsoft.AudienceInsight
{
    /// <summary>
    /// Provides an common interface for all log objects
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Gets a unique Id for the log entry.
        /// </summary>
        Guid Id { get;  }

        /// <summary>
        /// Gets or sets when the log was created
        /// </summary>
        DateTimeOffset TimeStamp { get; set; }

        /// <summary>
        /// Gets a unique identifier for the type of log
        /// </summary>
        string Type { get;  }

        /// <summary>
        /// Gets all log data
        /// </summary>
        IDictionary<string, object> GetData();

        /// <summary>
        /// Gets extra data about the log. This allows you to attach extra data to the log
        /// </summary>
        IDictionary<string, object> ExtraData { get; }
    }

    /// <summary>
    /// EventArgs for any event that needs to pass along a single log object.
    /// </summary>
#if SILVERLIGHT
    public class LogEventArgs : EventArgs
#else
    public sealed class LogEventArgs : Object
#endif
    {
        /// <summary>
        /// Creates a new instance of LogEventArgs.
        /// </summary>
        /// <param name="log">The log object associated with the event.</param>
        public LogEventArgs(ILog log)
        {
            Log = log;
        }

        /// <summary>
        /// Gets the log object associated with the event.
        /// </summary>
        public ILog Log { get; private set; }
    }

    /// <summary>
    /// Provides a default implementation of ILog
    /// </summary>
    public sealed class Log : ILog
    {
        /// <summary>
        /// Creates a new instance of Log
        /// </summary>
        /// <param name="type">The type of log</param>
        public Log(string type)
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTimeOffset.Now;
            Type = type;
            ExtraData = new Dictionary<string, object>();
        }

        /// <summary>
        /// Creates a new instance of Log
        /// </summary>
        /// <param name="id">The log id</param>
        /// <param name="jsondata">The log timestamp</param>
        /// <param name="type">The type of log</param>
        /// <param name="jsondata">The data to be included in the log</param>
        public Log(Guid id, DateTimeOffset timestamp, string type, IDictionary<string, object> data)
        {
            Id = id;
            TimeStamp = timestamp;
            Type = type;
            ExtraData = data;
        }

#if !WINDOWS_PHONE

        /// <summary>
        /// Creates a new instance of Log
        /// </summary>
        /// <param name="type">The type of log</param>
        /// <param name="jsondata">The JSON data to be included in the log</param>
        public Log(string type, string jsondata)
            : this(type)
        {
            ExtraData = ConvertToDictionary(JsonObject.Parse(jsondata));
        }

        static IDictionary<string, object> ConvertToDictionary(JsonObject jsonObject)
        {
            var result = new Dictionary<string, object>();
            foreach (var item in jsonObject)
            {
                object value = null;
                switch (item.Value.ValueType)
                {
                    case JsonValueType.Boolean:
                        value = item.Value.GetBoolean();
                        break;
                    case JsonValueType.Number:
                        value = item.Value.GetNumber();
                        break;
                    case JsonValueType.String:
                        value = item.Value.GetString();
                        break;
                    case JsonValueType.Array:
                        value = item.Value.GetArray();
                        break;
                    case JsonValueType.Object:
                        value = item.Value.GetObject();
                        break;
                }
                result.Add(item.Key, value);
            }
            return result;
        }
#endif
        /// <inheritdoc /> 
        public IDictionary<string, object> ExtraData { get; private set; }

        /// <inheritdoc /> 
        public Guid Id { get; set; }

        /// <inheritdoc /> 
        public DateTimeOffset TimeStamp { get; set; }

        /// <inheritdoc /> 
        public string Type { get; set; }

        /// <inheritdoc /> 
        public IDictionary<string, object> GetData()
        {
            return this.CreateBasicLogData();
        }
    }
}
