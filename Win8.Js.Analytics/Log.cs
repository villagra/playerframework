using Microsoft.VideoAnalytics;
using System;
using System.Collections.Generic;
using Windows.Data.Json;

namespace Microsoft.PlayerFramework.Js.Analytics
{
    /// <summary>
    /// Provides a default implementation of ILog where the data comes from JSON
    /// </summary>
    public sealed class Log : ILog
    {
        /// <summary>
        /// Creates a new instance of Log
        /// </summary>
        /// <param name="type">The type of log</param>
        /// <param name="jsondata">The JSON data to be included in the log</param>
        public Log(string type, string jsondata)
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTimeOffset.Now;
            Type = type;
            ExtraData = ConvertToDictionary(JsonObject.Parse(jsondata));
        }

        /// <inheritdoc /> 
        public IDictionary<string, object> ExtraData { get; private set; }

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

        /// <inheritdoc /> 
        public Guid Id { get; private set; }

        /// <inheritdoc /> 
        public DateTimeOffset TimeStamp { get; set; }

        /// <inheritdoc /> 
        public string Type { get; private set; }

        /// <inheritdoc /> 
        public IDictionary<string, object> GetData()
        {
            return this.CreateBasicLogData();
        }
    }
}
