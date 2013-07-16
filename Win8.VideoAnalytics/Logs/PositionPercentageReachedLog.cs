using System;
using System.Collections.Generic;

namespace Microsoft.VideoAnalytics
{
    public sealed class PositionPercentageReachedLog : ILog
    {
        public PositionPercentageReachedLog(double position)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.PositionPercentageReached;
            Position = position;
            Id = Guid.NewGuid();
            ExtraData = new Dictionary<string, object>();
        }

        /// <inheritdoc /> 
        public IDictionary<string, object> ExtraData { get; private set; }

        /// <inheritdoc /> 
        public Guid Id { get; set; }

        /// <inheritdoc /> 
        public DateTimeOffset TimeStamp { get; set; }

        /// <inheritdoc /> 
        public string Type { get; private set; }

        /// <inheritdoc /> 
        public IDictionary<string, object> GetData()
        {
            var result = this.CreateBasicLogData();
            result.Add("Position", Position);
            return result;
        }

        /// <summary>
        /// Gets the percentage of the position compared to the duration when the event occurred.
        /// .5 = 50%, 2 = 200%
        /// </summary>
        public double Position { get; set; }

    }
}
