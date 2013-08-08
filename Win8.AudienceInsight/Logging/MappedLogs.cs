using System;
using System.Collections.Generic;

namespace Microsoft.AudienceInsight
{
    /// <summary>
    /// Provides an implementation for the ILog interface to use after mapping key value pairs
    /// </summary>
    internal sealed class MappedLog : ILog
    {
        private IDictionary<string, object> data;

        public MappedLog(ILog log, IDictionary<string, object> data)
        {
            Id = log.Id;
            TimeStamp = log.TimeStamp;
            Type = log.Type;
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

    internal sealed class MappedBatch : IBatch
    {
        private IDictionary<string, object> data;

        private MappedBatch(IBatch batch, IDictionary<string, object> data)
        {
            Id = batch.Id;
            TimeStamp = batch.TimeStamp;
            Type = batch.Type;
            this.data = data;
        }

        public MappedBatch(IBatch batch, IDictionary<string, object> data, IEnumerable<ILog> logs)
            : this(batch, data)
        {
            Logs = logs;
        }

        /// <inheritdoc /> 
        public Guid Id { get; private set; }

        /// <inheritdoc /> 
        public DateTimeOffset TimeStamp { get; set; }

        /// <inheritdoc /> 
        public string Type { get; private set; }

        /// <inheritdoc /> 
        public IEnumerable<ILog> Logs { get; private set; }

        /// <inheritdoc /> 
        public IDictionary<string, object> ExtraData { get; private set; }

        /// <inheritdoc /> 
        public IDictionary<string, object> GetData()
        {
            return data;
        }
    }
}
