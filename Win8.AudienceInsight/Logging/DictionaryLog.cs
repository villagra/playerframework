using System;
using System.Collections.Generic;

namespace Microsoft.AudienceInsight
{
    /// <summary>
    /// Provides an ILog implementation that contains a simple key value collection
    /// </summary>
    public sealed class DictionaryLog : ILog
    {
        public DictionaryLog(Guid id, DateTimeOffset timestamp, string type, IDictionary<string, object> data)
        {
            Id = id;
            TimeStamp = timestamp;
            Type = type;
            Data = data;
        }

        /// <inheritdoc /> 
        public Guid Id { get; set; }

        /// <inheritdoc /> 
        public DateTimeOffset TimeStamp { get; set; }

        /// <inheritdoc /> 
        public string Type { get; set; }

        /// <summary>
        /// The data contained by this log
        /// </summary>
        public IDictionary<string, object> Data { get; set; }

        /// <inheritdoc /> 
        public IDictionary<string, object> ExtraData { get; private set; }

        /// <inheritdoc /> 
        public IDictionary<string, object> GetData()
        {
            return Data;
        }
    }
}
