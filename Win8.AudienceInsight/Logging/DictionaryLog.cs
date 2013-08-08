using System;
using System.Collections.Generic;

namespace Microsoft.AudienceInsight
{
    public sealed class DictionaryLog : ILog
    {
        private IDictionary<string, object> data;

        public DictionaryLog(Guid id, DateTimeOffset timestamp, string type, IDictionary<string, object> data)
        {
            Id = id;
            TimeStamp = timestamp;
            Type = type;
            this.data = data;
        }

        /// <inheritdoc /> 
        public Guid Id { get; set; }

        /// <inheritdoc /> 
        public DateTimeOffset TimeStamp { get; set; }

        /// <inheritdoc /> 
        public string Type { get; set; }

        /// <inheritdoc /> 
        public IDictionary<string, object> ExtraData { get; private set; }

        /// <inheritdoc /> 
        public IDictionary<string, object> GetData()
        {
            return data;
        }
    }
}
