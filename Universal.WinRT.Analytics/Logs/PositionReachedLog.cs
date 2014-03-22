using System;
using System.Collections.Generic;

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// A log object for when a specific position has been reached during playback. For example, when the 20 minute mark has been reached.
    /// </summary>
    public sealed class PositionReachedLog : ILog
    {
        /// <summary>
        /// Creates a new instance of PositionReachedLog
        /// </summary>
        /// <param name="position">The postion that was reached.</param>
        public PositionReachedLog(TimeSpan position)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.PositionReached;
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
        /// Gets the position that playback was at when the event occurred.
        /// </summary>
        public TimeSpan Position { get; private set; }

    }
}
