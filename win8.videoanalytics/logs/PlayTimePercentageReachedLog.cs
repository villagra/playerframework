using System;
using System.Collections.Generic;

namespace Microsoft.VideoAnalytics
{
    public sealed class PlayTimePercentageReachedLog : ILog
    {
        public PlayTimePercentageReachedLog(double playTime)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.PlayTimePercentageReached;
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
        /// Gets or sets the percentage of play time compared to the duration that the media has been played before the tracking event will fire.
        /// .5 = 50%, 2 = 200%
        /// </summary>
        public double? PlayTime { get; set; }

    }
}
