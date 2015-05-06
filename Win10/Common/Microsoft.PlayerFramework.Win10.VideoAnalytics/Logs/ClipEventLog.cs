using System;
using System.Collections.Generic;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// Enumerates the various types of clip events.
    /// </summary>
    public enum ClipEventType
    {
        /// <summary>
        /// The clip started
        /// </summary>
        Started,
        /// <summary>
        /// The clip ended/finished
        /// </summary>
        Ended,
    }

    /// <summary>
    /// A log generated when a clip (ususally an advertisement) is started or ends.
    /// </summary>
    public sealed class ClipEventLog : ILog
    {
        /// <summary>
        /// Creates a new instance of ClipEventLog.
        /// </summary>
        /// <param name="clipEventType">The type of clip event that occurred.</param>
        /// <param name="position">The position of playback on the main media when the event occurred.</param>
        /// <param name="source">The source of the clip (if available).</param>
        public ClipEventLog(ClipEventType clipEventType, TimeSpan position, Uri source)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.ClipEvent;
            ClipEventType = clipEventType;
            Position = position;
            Source = source;
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
            result.Add("ClipEventType", ClipEventType);
            return result;
        }

        /// <summary>
        /// Gets the type of stream event that occured
        /// </summary>
        public ClipEventType ClipEventType { get; private set; }

        /// <summary>
        /// Gets the position of the video at the time of the stream event
        /// </summary>
        public TimeSpan Position { get; private set; }

        /// <summary>
        /// Gets the source Uri of the clip
        /// </summary>
        public Uri Source { get; private set; }
    }
}