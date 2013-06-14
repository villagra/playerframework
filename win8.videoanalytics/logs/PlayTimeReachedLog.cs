using System;
using System.Collections.Generic;

namespace Microsoft.VideoAnalytics
{
    public sealed class PlayTimeReachedLog : ILog
    {
        public PlayTimeReachedLog(TimeSpan playTime)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.PlayTimeReached;
            PlayTime = playTime;
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
            result.Add("PlayTime", PlayTime);
            return result;
        }

        /// <summary>
        /// Gets the amount of time that has been played when the event occurred.
        /// </summary>
        public TimeSpan PlayTime { get; private set; }

    }
}
