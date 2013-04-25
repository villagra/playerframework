using Microsoft.VideoAnalytics;
using System;

namespace Microsoft.PlayerFramework.Js.Analytics
{
    /// <summary>
    /// Provides an adapter between the advertising component and the player.
    /// </summary>
    public sealed class MediaPlayerAdapterBridge : IPlayerMonitor
    {
        /// <summary>
        /// Raised when the current position is requested
        /// </summary>
        public event EventHandler<PositionRequestedEventArgs> PositionRequested;

        /// <summary>
        /// Raised when the current duration is requested
        /// </summary>
        public event EventHandler<DurationRequestedEventArgs> DurationRequested;

        double IPlayerMonitor.DroppedFramesPerSecond
        {
            get { return 0; }
        }

        double IPlayerMonitor.RenderedFramesPerSecond
        {
            get { return 0; }
        }

        TimeSpan IPlayerMonitor.Duration
        {
            get
            {
                if (PositionRequested != null)
                {
                    var args = new DurationRequestedEventArgs();
                    DurationRequested(this, args);
                    return args.Result;
                }
                else return TimeSpan.Zero;
            }
        }

        TimeSpan IPlayerMonitor.Position
        {
            get
            {
                if (PositionRequested != null)
                {
                    var args = new PositionRequestedEventArgs();
                    PositionRequested(this, args);
                    return args.Result;
                }
                else return TimeSpan.Zero;
            }
        }

        bool isFullScreen;
        /// <inheritdoc /> 
        bool IPlayerMonitor.IsFullScreen { get { return isFullScreen; } }

        /// <summary>
        /// To be called when full screen changes.
        /// </summary>
        /// <param name="value">A boolean indicating if in fullscreen mode or not.</param>
        public void SetIsFullScreen(bool value)
        {
            if (isFullScreen != value)
            {
                isFullScreen = value;
                if (FullScreenChanged != null) FullScreenChanged(this, null);
            }
        }

        bool isBuffering;
        /// <inheritdoc /> 
        bool IPlayerMonitor.IsBuffering { get { return isBuffering; } }

        /// <summary>
        /// To be called when buffering starts or stops.
        /// </summary>
        /// <param name="value">A boolean indicating if buffering or not.</param>
        public void SetIsBuffering(bool value)
        {
            if (isBuffering != value)
            {
                isBuffering = value;
                if (IsBufferingChanged != null) IsBufferingChanged(this, null);
            }
        }

        bool isLive;
        /// <inheritdoc /> 
        bool IPlayerMonitor.IsLive { get { return isLive; } }

        /// <summary>
        /// To be called when is live changes.
        /// </summary>
        /// <param name="value">A boolean indicating if the stream is live or VOD.</param>
        public void SetIsLive(bool value)
        {
            if (isLive != value)
            {
                isLive = value;
                if (IsLiveChanged != null) IsLiveChanged(this, null);
            }
        }

        double playbackRate;
        /// <inheritdoc /> 
        double IPlayerMonitor.PlaybackRate { get { return playbackRate; } }

        /// <summary>
        /// To be called when the playback rate changes.
        /// </summary>
        /// <param name="value">The new playback rate.</param>
        public void SetPlaybackRate(double value)
        {
            if (playbackRate != value)
            {
                playbackRate = value;
                if (PlaybackRateChanged != null) PlaybackRateChanged(this, null);
            }
        }

        string audioTrackId;
        /// <inheritdoc /> 
        string IPlayerMonitor.AudioTrackId { get { return audioTrackId; } }

        /// <summary>
        /// To be called when the currently selected audio track changes.
        /// </summary>
        /// <param name="value">The ID of the audio track.</param>
        public void SetAudioTrackId(string value)
        {
            if (audioTrackId != value)
            {
                audioTrackId = value;
                if (AudioTrackChanged != null) AudioTrackChanged(this, null);
            }
        }

        string captionTrackId;
        /// <inheritdoc /> 
        string IPlayerMonitor.CaptionTrackId { get { return captionTrackId; } }

        /// <summary>
        /// To be called when the currently selected caption track changes.
        /// </summary>
        /// <param name="value">The ID of the caption track.</param>
        public void SetCaptionTrackId(string value)
        {
            if (captionTrackId != value)
            {
                captionTrackId = value;
                if (CaptionTrackChanged != null) CaptionTrackChanged(this, null);
            }
        }

        Uri source;
        /// <inheritdoc /> 
        Uri IPlayerMonitor.Source { get { return source; } }

        /// <summary>
        /// To be called when the source changes.
        /// </summary>
        /// <param name="value">The URI of the source.</param>
        public void SetSource(Uri value)
        {
            source = value;
        }

        /// <summary>
        /// To be called when a new clip starts.
        /// </summary>
        /// <param name="source">The clip media source (if available).</param>
        public void OnClipStarted(string source)
        {
            if (ClipStarted != null) ClipStarted(this, new ClipEventArgs(string.IsNullOrEmpty(source) ? null : new Uri(source)));
        }

        /// <summary>
        /// To be called when a clip ends.
        /// </summary>
        /// <param name="source">The clip media source (if available).</param>
        public void OnClipEnded(string source)
        {
            if (ClipEnded != null) ClipEnded(this, new ClipEventArgs(string.IsNullOrEmpty(source) ? null : new Uri(source)));
        }

        /// <summary>
        /// To be called when a stream fails to play or start.
        /// </summary>
        /// <param name="error">The error message associated with the failure.</param>
        public void OnStreamFailed(string error)
        {
            if (StreamFailed != null) StreamFailed(this, new StreamFailedEventArgs(error));
        }

        /// <summary>
        /// To be called when a stream first loads.
        /// </summary>
        public void OnStreamLoaded()
        {
            if (StreamLoaded != null) StreamLoaded(this, null);
        }

        /// <summary>
        /// To be called when a stream first starts to play.
        /// </summary>
        public void OnStreamStarted()
        {
            if (StreamStarted != null) StreamStarted(this, null);
        }

        /// <summary>
        /// To be called when a stream is closed.
        /// </summary>
        public void OnStreamClosed()
        {
            if (StreamClosed != null) StreamClosed(this, null);
        }

        /// <summary>
        /// To be called when a stream reaches the end.
        /// </summary>
        public void OnStreamEnded()
        {
            if (StreamEnded != null) StreamEnded(this, null);
        }

        /// <summary>
        /// To be called when a stream plays (either from starting or after pause).
        /// </summary>
        public void OnPlaying()
        {
            if (Playing != null) Playing(this, null);
        }

        /// <summary>
        /// To be called when a stream is paused.
        /// </summary>
        public void OnPaused()
        {
            if (Paused != null) Paused(this, null);
        }

        /// <summary>
        /// To be called when the user seeks.
        /// </summary>
        public void OnSeeked(TimeSpan previousPosition, TimeSpan position)
        {
            if (Seeked != null) Seeked(this, new SeekedEventArgs(previousPosition, position));
        }

        /// <summary>
        /// To be called when the user starts to scrub.
        /// </summary>
        public void OnScrubStarted()
        {
            if (ScrubStarted != null) ScrubStarted(this, null);
        }

        /// <summary>
        /// To be called when the user finishes scrubbing.
        /// </summary>
        public void OnScrubCompleted(TimeSpan position)
        {
            if (ScrubCompleted != null) ScrubCompleted(this, new ScrubCompletedEventArgs(position));
        }

        /// <summary>
        /// To be called when a monitored position has been reached
        /// </summary>
        public void OnPositionReached(TimeSpan position)
        {
            if (PositionReached != null) PositionReached(this, new PositionReachedEventArgs(position));
        }

        /// <summary>
        /// To be called when a monitored position (described as a percentage of the duration) has been reached
        /// </summary>
        public void OnPositionPercentageReached(double position)
        {
            if (PositionPercentageReached != null) PositionPercentageReached(this, new PositionPercentageReachedEventArgs(position));
        }

        /// <summary>
        /// To be called when a monitored play time has been reached
        /// </summary>
        public void OnPlayTimeReached(TimeSpan playTime)
        {
            if (PlayTimeReached != null) PlayTimeReached(this, new PlayTimeReachedEventArgs(playTime));
        }

        /// <summary>
        /// To be called when a monitored play time (described as a percentage of the duration) has been reached
        /// </summary>
        public void OnPlayTimePercentageReached(double playTime)
        {
            if (PlayTimePercentageReached != null) PlayTimePercentageReached(this, new PlayTimePercentageReachedEventArgs(playTime));
        }

        /// <inheritdoc /> 
        public event EventHandler<ClipEventArgs> ClipStarted;

        /// <inheritdoc /> 
        public event EventHandler<ClipEventArgs> ClipEnded;

        /// <inheritdoc /> 
        public event EventHandler<StreamFailedEventArgs> StreamFailed;

        /// <inheritdoc /> 
        public event EventHandler<SeekedEventArgs> Seeked;

        /// <inheritdoc /> 
        public event EventHandler<ScrubCompletedEventArgs> ScrubCompleted;

        /// <inheritdoc /> 
        public event EventHandler<object> StreamLoaded;

        /// <inheritdoc /> 
        public event EventHandler<object> StreamStarted;

        /// <inheritdoc /> 
        public event EventHandler<object> StreamClosed;

        /// <inheritdoc /> 
        public event EventHandler<object> StreamEnded;

        /// <inheritdoc /> 
        public event EventHandler<object> Playing;

        /// <inheritdoc /> 
        public event EventHandler<object> Paused;

        /// <inheritdoc /> 
        public event EventHandler<object> IsBufferingChanged;

        /// <inheritdoc /> 
        public event EventHandler<object> FullScreenChanged;

        /// <inheritdoc /> 
        public event EventHandler<object> ScrubStarted;

        /// <inheritdoc /> 
        public event EventHandler<object> PlaybackRateChanged;

        /// <inheritdoc /> 
        public event EventHandler<object> IsLiveChanged;

        /// <inheritdoc /> 
        public event EventHandler<object> CaptionTrackChanged;

        /// <inheritdoc /> 
        public event EventHandler<object> AudioTrackChanged;

        /// <inheritdoc /> 
        public event EventHandler<PositionReachedEventArgs> PositionReached;

        /// <inheritdoc /> 
        public event EventHandler<PlayTimeReachedEventArgs> PlayTimeReached;

        /// <inheritdoc /> 
        public event EventHandler<PositionPercentageReachedEventArgs> PositionPercentageReached;

        /// <inheritdoc /> 
        public event EventHandler<PlayTimePercentageReachedEventArgs> PlayTimePercentageReached;
    }
    
    /// <summary>
    /// Provides a way to return the current position when the PositionRequested event is raised.
    /// </summary>
    public sealed class PositionRequestedEventArgs
    {
        internal PositionRequestedEventArgs()
        { }

        /// <summary>
        /// The current Position. This should be set by the handler of the event.
        /// </summary>
        public TimeSpan Result { get; set; }
    }
    
    /// <summary>
    /// Provides a way to return the current position when the PositionRequested event is raised.
    /// </summary>
    public sealed class DurationRequestedEventArgs
    {
        internal DurationRequestedEventArgs()
        { }

        /// <summary>
        /// The current Duration. This should be set by the handler of the event.
        /// </summary>
        public TimeSpan Result { get; set; }
    }
}