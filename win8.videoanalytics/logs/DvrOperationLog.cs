using System;
using System.Collections.Generic;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// Enumerates the different types of DVR operations
    /// </summary>
    public enum DvrOperationType
    {
        /// <summary>
        /// The user seeked
        /// </summary>
        Seeked,
        /// <summary>
        /// The user started a scrub action
        /// </summary>
        ScrubStarted,
        /// <summary>
        /// The user completed or commited the scrub action
        /// </summary>
        ScrubCompleted,
        /// <summary>
        /// The user changed the play rate (e.g. FF, RW, or slow motion)
        /// </summary>
        PlayrateChanged,
        /// <summary>
        /// The user paused the media
        /// </summary>
        Pause,
        /// <summary>
        /// The user started or resumed the media
        /// </summary>
        Play,
    }

    /// <summary>
    /// A log generated each time the user performs a new DVR operation
    /// </summary>
    public sealed class DvrOperationLog : ILog
    {
        /// <summary>
        /// Creates a new instance of DvrOperationLog.
        /// </summary>
        /// <param name="operationType">The operation that occurred.</param>
        /// <param name="position">The position of playback when the operation occured.</param>
        /// <param name="playbackRate">The playback rate of the DVR operation. If this operation is PlayrateChanged, this will be the new playback rate</param>
        public DvrOperationLog(DvrOperationType operationType, TimeSpan position, double playbackRate, bool isPaused, TimeSpan? previousPosition)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = EventTypes.DvrOperation;
            OperationType = operationType;
            Position = position;
            PreviousPosition = previousPosition;
            PlaybackRate = playbackRate;
            IsPaused = isPaused;
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
            result.Add("OperationType", OperationType);
            result.Add("Position", Position);
            if (PreviousPosition.HasValue)
            {
                result.Add("PreviousPosition", PreviousPosition.Value);
            }
            result.Add("PlaybackRate", PlaybackRate);
            result.Add("IsPaused", IsPaused);
            return result;
        }

        /// <summary>
        /// Gets the type of DVR operation that occured
        /// </summary>
        public DvrOperationType OperationType { get; private set; }

        /// <summary>
        /// Gets the position of the video at the time of the DVR operation
        /// </summary>
        public TimeSpan Position { get; private set; }

        /// <summary>
        /// Gets the previous position of the video at the time of the DVR operation.
        /// Note: This is only applicable to seeking.
        /// </summary>
        public TimeSpan? PreviousPosition { get; private set; }

        /// <summary>
        /// Gets the playrate at the time of the DVR operation
        /// </summary>
        public double PlaybackRate { get; private set; }

        /// <summary>
        /// Gets a flag indicating if playback was paused at the time of the DVR operation.
        /// </summary>
        public bool IsPaused { get; private set; }
    }
}
