using System;
using System.Collections.Generic;

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// Enumerates the various types of streaming events.
    /// </summary>
    public enum StreamEventType
    {
        /// <summary>
        /// The media was loaded.
        /// </summary>
        Loaded,
        /// <summary>
        /// Playback was started.
        /// </summary>
        Started,
        /// <summary>
        /// Playback ended.
        /// </summary>
        Ended,
        /// <summary>
        /// The media was closed/unloaded.
        /// </summary>
        Unloaded,
        /// <summary>
        /// Playback failed.
        /// </summary>
        Failed,
        /// <summary>
        /// The user resumed playing the media
        /// </summary>
        Playing,
        /// <summary>
        /// The user paused the media
        /// </summary>
        Paused,
    }

    /// <summary>
    /// A log generated for a major lifecycle event in the stream (e.g. loaded, started, paused, ...etc).
    /// </summary>
    public sealed class StreamEventLog : IMarkerEntry
    {
        /// <summary>
        /// Creates a new instance of StreamEventLog
        /// </summary>
        /// <param name="streamEventType">The type of streaming event that occurred.</param>
        /// <param name="position">The position of playback when the operation occured.</param>
        /// <param name="duration">The duration of the stream.</param>
        public StreamEventLog(StreamEventType streamEventType, TimeSpan position, TimeSpan duration)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.StreamEvent;
            StreamEventType = streamEventType;
            Position = position;
            Duration = duration;
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
            result.Add("StreamEventType", StreamEventType);
            result.Add("Position", Position);
            result.Add("Duration", Duration);
            return result;
        }

        /// <summary>
        /// Gets the type of stream event that occured
        /// </summary>
        public StreamEventType StreamEventType { get; private set; }

        /// <summary>
        /// Gets the position of the video at the time of the stream event
        /// </summary>
        public TimeSpan Position { get; private set; }

        /// <summary>
        /// Gets the duration of the video
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <inheritdoc /> 
        public bool IsPlaying
        {
            get
            {
                switch (StreamEventType)
                {
                    case StreamEventType.Started:
                    case StreamEventType.Playing:
                        return true;
                    default:
                        return false;
                }
            }
        }
    }
}
