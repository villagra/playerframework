using System;
using System.Collections.Generic;

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// A log object for when a specific amount of time during playback has been reached. For example, when the 20 minutes of video has been played regardless of where in the video you are.
    /// </summary>
    public sealed class PlayTimeReachedLog : ILog
    {
        /// <summary>
        /// Creates a new instance of PlayTimeReachedLog.
        /// </summary>
        /// <param name="playTime">The amount of time played that has been reached.</param>
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
