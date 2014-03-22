using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// Provides an interface to relay information about the player
    /// </summary>
    public interface IPlayerMonitor
    {
        /// <summary>
        /// Gets the current dropped frames per second.
        /// </summary>
        double DroppedFramesPerSecond { get; }

        /// <summary>
        /// Gets the current rendered frames per second.
        /// </summary>
        double RenderedFramesPerSecond { get; }

        /// <summary>
        /// Gets the current play back rate (e.g. 1 = normal, -1 = RW at 1x, 2 = FF at 2x)
        /// </summary>
        double PlaybackRate { get; }

        /// <summary>
        /// Gets whether or not the player is fullscreen.
        /// </summary>
        bool IsFullScreen { get; }

        /// <summary>
        /// Gets whether or not the stream is a live (vs. VOD) stream.
        /// </summary>
        bool IsLive { get; }

        /// <summary>
        /// Gets whether or not the player is currently buffering.
        /// </summary>
        bool IsBuffering { get; }

        /// <summary>
        /// Gets the source URI of the stream.
        /// </summary>
        Uri Source { get; }

        /// <summary>
        /// Gets the current playback position of the stream.
        /// </summary>
        TimeSpan Position { get; }

        /// <summary>
        /// Gets the current duration of the stream.
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// Gets the ID of the currently selected audio track.
        /// </summary>
        string AudioTrackId { get; }

        /// <summary>
        /// Gets the ID of the currently selected caption track.
        /// </summary>
        string CaptionTrackId { get; }

        /// <summary>
        /// Indicates that a clip has started.
        /// </summary>
        event EventHandler<ClipEventArgs> ClipStarted;

        /// <summary>
        /// Indicates that a clip has ended.
        /// </summary>
        event EventHandler<ClipEventArgs> ClipEnded;

        /// <summary>
        /// Indicates that the stream failed.
        /// </summary>
        event EventHandler<StreamFailedEventArgs> StreamFailed;

        /// <summary>
        /// Indicates that a monitored position has been reached
        /// </summary>
        event EventHandler<PositionReachedEventArgs> PositionReached;

        /// <summary>
        /// Indicates that a monitored playtime has been reached
        /// </summary>
        event EventHandler<PlayTimeReachedEventArgs> PlayTimeReached;

        /// <summary>
        /// Indicates that a monitored position (by percentage) has been reached
        /// </summary>
        event EventHandler<PositionPercentageReachedEventArgs> PositionPercentageReached;

        /// <summary>
        /// Indicates that a monitored play time (by percentage) has been reached
        /// </summary>
        event EventHandler<PlayTimePercentageReachedEventArgs> PlayTimePercentageReached;

        /// <summary>
        /// Indicates that the user has seeked.
        /// </summary>
        event EventHandler<SeekedEventArgs> Seeked;

        /// <summary>
        /// Indicates that the user has completed a scrub.
        /// </summary>
        event EventHandler<ScrubCompletedEventArgs> ScrubCompleted;

#if SILVERLIGHT
        /// <summary>
        /// Indicates that the stream loaded.
        /// </summary>
        event EventHandler StreamLoaded;

        /// <summary>
        /// Indicates that the stream started playback.
        /// </summary>
        event EventHandler StreamStarted;

        /// <summary>
        /// Indicates that the stream closed.
        /// </summary>
        event EventHandler StreamClosed;

        /// <summary>
        /// Indicates that the stream ended.
        /// </summary>
        event EventHandler StreamEnded;

        /// <summary>
        /// Indicates that playback started.
        /// </summary>
        event EventHandler Playing;

        /// <summary>
        /// Indicates that playback was paused.
        /// </summary>
        event EventHandler Paused;

        /// <summary>
        /// Indicates that the IsBuffering property has changed.
        /// </summary>
        event EventHandler IsBufferingChanged;

        /// <summary>
        /// Indicates that the IsFullScreen property has changed.
        /// </summary>
        event EventHandler FullScreenChanged;

        /// <summary>
        /// Indicates that the user has started to scrub.
        /// </summary>
        event EventHandler ScrubStarted;

        /// <summary>
        /// Indicates that the property PlaybackRate has changed.
        /// </summary>
        event EventHandler PlaybackRateChanged;

        /// <summary>
        /// Indicates that the IsLive property has changed.
        /// </summary>
        event EventHandler IsLiveChanged;

        /// <summary>
        /// Indicates that the CaptionTrackId property has changed.
        /// </summary>
        event EventHandler CaptionTrackChanged;

        /// <summary>
        /// Indicates that the AudioTrackId property has changed.
        /// </summary>
        event EventHandler AudioTrackChanged;
#else
        /// <summary>
        /// Indicates that the stream loaded.
        /// </summary>
        event EventHandler<object> StreamLoaded;

        /// <summary>
        /// Indicates that the stream started playback.
        /// </summary>
        event EventHandler<object> StreamStarted;

        /// <summary>
        /// Indicates that the stream closed.
        /// </summary>
        event EventHandler<object> StreamClosed;

        /// <summary>
        /// Indicates that the stream ended.
        /// </summary>
        event EventHandler<object> StreamEnded;

        /// <summary>
        /// Indicates that playback started.
        /// </summary>
        event EventHandler<object> Playing;

        /// <summary>
        /// Indicates that playback was paused.
        /// </summary>
        event EventHandler<object> Paused;

        /// <summary>
        /// Indicates that the IsBuffering property has changed.
        /// </summary>
        event EventHandler<object> IsBufferingChanged;

        /// <summary>
        /// Indicates that the IsFullScreen property has changed.
        /// </summary>
        event EventHandler<object> FullScreenChanged;

        /// <summary>
        /// Indicates that the user has started to scrub.
        /// </summary>
        event EventHandler<object> ScrubStarted;

        /// <summary>
        /// Indicates that the property PlaybackRate has changed.
        /// </summary>
        event EventHandler<object> PlaybackRateChanged;

        /// <summary>
        /// Indicates that the IsLive property has changed.
        /// </summary>
        event EventHandler<object> IsLiveChanged;

        /// <summary>
        /// Indicates that the CaptionTrackId property has changed.
        /// </summary>
        event EventHandler<object> CaptionTrackChanged;

        /// <summary>
        /// Indicates that the AudioTrackId property has changed.
        /// </summary>
        event EventHandler<object> AudioTrackChanged;
#endif
    }

    /// <summary>
    /// Provides info about a stream failure event.
    /// </summary>
#if SILVERLIGHT
    public sealed class StreamFailedEventArgs : EventArgs
#else
    public sealed class StreamFailedEventArgs : object
#endif
    {
        /// <summary>
        /// Creates a new instance of StreamFailedEventArgs.
        /// </summary>
        /// <param name="errorMessage">An error message related to the reason for the failure.</param>
        public StreamFailedEventArgs(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets the error message associated with the stream failure.
        /// </summary>
        public string ErrorMessage { get; private set; }
    }

    /// <summary>
    /// Provides info about a clip event.
    /// </summary>
#if SILVERLIGHT
    public sealed class ClipEventArgs : EventArgs
#else
    public sealed class ClipEventArgs : object
#endif
    {
        /// <summary>
        /// Creates a new instance of ClipEventArgs.
        /// </summary>
        /// <param name="source">The source URI of the clip (if available).</param>
        public ClipEventArgs(Uri source)
        {
            Source = source;
        }

        /// <summary>
        /// Gets the source URI for the clip (this may not be set if one is not available).
        /// </summary>
        public Uri Source { get; private set; }
    }

    /// <summary>
    /// Provides info about a PlayTimeReached event.
    /// </summary>
#if SILVERLIGHT
    public sealed class PlayTimeReachedEventArgs : EventArgs
#else
    public sealed class PlayTimeReachedEventArgs : object
#endif
    {
        /// <summary>
        /// Creates a new instance of PlayTimeReachedEventArgs.
        /// </summary>
        /// <param name="playTime">The play time that triggered this event.</param>
        public PlayTimeReachedEventArgs(TimeSpan playTime)
        {
            PlayTime = playTime;
        }

        /// <summary>
        /// Gets the play time that triggered the event.
        /// </summary>
        public TimeSpan PlayTime { get; private set; }
    }

    /// <summary>
    /// Provides info about a PositionReached event.
    /// </summary>
#if SILVERLIGHT
    public sealed class PositionReachedEventArgs : EventArgs
#else
    public sealed class PositionReachedEventArgs : object
#endif
    {
        /// <summary>
        /// Creates a new instance of PositionReachedEventArgs.
        /// </summary>
        /// <param name="position">The position that triggered this event.</param>
        public PositionReachedEventArgs(TimeSpan position)
        {
            Position = position;
        }

        /// <summary>
        /// Gets the position that triggered the event.
        /// </summary>
        public TimeSpan Position { get; private set; }
    }

    /// <summary>
    /// Provides info about a PlayTimePercentageReached event.
    /// </summary>
#if SILVERLIGHT
    public sealed class PlayTimePercentageReachedEventArgs : EventArgs
#else
    public sealed class PlayTimePercentageReachedEventArgs : object
#endif
    {
        /// <summary>
        /// Creates a new instance of PlayTimePercentageReachedEventArgs.
        /// </summary>
        /// <param name="playTime">The play time that triggered this event.</param>
        public PlayTimePercentageReachedEventArgs(double playTime)
        {
            PlayTime = playTime;
        }

        /// <summary>
        /// Gets the play time that triggered the event.
        /// </summary>
        public double PlayTime { get; private set; }
    }

    /// <summary>
    /// Provides info about a PositionPercentageReached event.
    /// </summary>
#if SILVERLIGHT
    public sealed class PositionPercentageReachedEventArgs : EventArgs
#else
    public sealed class PositionPercentageReachedEventArgs : object
#endif
    {
        /// <summary>
        /// Creates a new instance of PositionPercentageReachedEventArgs.
        /// </summary>
        /// <param name="position">The position that triggered this event.</param>
        public PositionPercentageReachedEventArgs(double position)
        {
            Position = position;
        }

        /// <summary>
        /// Gets the position that triggered the event.
        /// </summary>
        public double Position { get; private set; }
    }

    /// <summary>
    /// Provides info about a Seeked event.
    /// </summary>
#if SILVERLIGHT
    public sealed class SeekedEventArgs : EventArgs
#else
    public sealed class SeekedEventArgs : object
#endif
    {
        /// <summary>
        /// Creates a new instance of SeekedEventArgs.
        /// </summary>
        /// <param name="previousPosition">The previous position</param>
        /// <param name="newPosition">The new position</param>
        public SeekedEventArgs(TimeSpan previousPosition, TimeSpan newPosition)
        {
            PreviousPosition = previousPosition;
            NewPosition = newPosition;
        }

        /// <summary>
        /// Gets the previous position.
        /// </summary>
        public TimeSpan PreviousPosition { get; private set; }

        /// <summary>
        /// Gets the new position.
        /// </summary>
        public TimeSpan NewPosition { get; private set; }
    }

    /// <summary>
    /// Provides info about a ScrubCompleted event.
    /// </summary>
#if SILVERLIGHT
    public sealed class ScrubCompletedEventArgs : EventArgs
#else
    public sealed class ScrubCompletedEventArgs : object
#endif
    {
        /// <summary>
        /// Creates a new instance of SeekedEventArgs.
        /// </summary>
        /// <param name="position">The position in the timeline where the scrub completed at.</param>
        public ScrubCompletedEventArgs(TimeSpan position)
        {
            Position = position;
        }

        /// <summary>
        /// Gets the new position.
        /// </summary>
        public TimeSpan Position { get; private set; }
    }
}
