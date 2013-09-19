using System;

namespace Microsoft.VideoAdvertising
{
    /// <summary>
    /// Defines all the properties and events that MAST conditions can be based on
    /// </summary>
    public interface IMastAdapter
    {
        #region Events
        
        /// <summary>
        /// Defined as anytime the play command is issued, even after a pause
        /// </summary>
#if SILVERLIGHT
        event EventHandler OnPlay;
#else
        event EventHandler<object> OnPlay;
#endif

        /// <summary>
        /// The stop command is given
        /// </summary>
#if SILVERLIGHT
        event EventHandler OnStop;
#else
        event EventHandler<object> OnStop;
#endif

        /// <summary>
        /// The pause command is given
        /// </summary>
#if SILVERLIGHT
        event EventHandler OnPause;
#else
        event EventHandler<object> OnPause;
#endif

        /// <summary>
        /// The player was muted, or volume brought to 0
        /// </summary>
#if SILVERLIGHT
        event EventHandler OnMute;
#else
        event EventHandler<object> OnMute;
#endif

        /// <summary>
        /// Volume was changed
        /// </summary>
#if SILVERLIGHT
        event EventHandler OnVolumeChange;
#else
        event EventHandler<object> OnVolumeChange;
#endif

        /// <summary>
        /// The player has stopped naturally, with no new content
        /// </summary>
#if SILVERLIGHT
        event EventHandler OnEnd;
#else
        event EventHandler<object> OnEnd;
#endif

        /// <summary>
        /// The player was manually seeked
        /// </summary>
#if SILVERLIGHT
        event EventHandler OnSeek;
#else
        event EventHandler<object> OnSeek;
#endif

        /// <summary>
        /// A new item is being started
        /// </summary>
#if SILVERLIGHT
        event EventHandler OnItemStart;
#else
        event EventHandler<object> OnItemStart;
#endif

        /// <summary>
        /// The current item is coming to the end
        /// </summary>
#if SILVERLIGHT
        event EventHandler OnItemEnd;
#else
        event EventHandler<object> OnItemEnd;
#endif

        /// <summary>
        /// Full screen state has changed
        /// </summary>
#if SILVERLIGHT
        event EventHandler OnFullScreenChange;
#else
        event EventHandler<object> OnFullScreenChange;
#endif

        /// <summary>
        /// Player size has changed
        /// </summary>
#if SILVERLIGHT
        event EventHandler OnPlayerSizeChanged;
#else
        event EventHandler<object> OnPlayerSizeChanged;
#endif

        /// <summary>
        /// An error has occurred, typically of enough severity to warrant display to the user
        /// </summary>
#if SILVERLIGHT
        event EventHandler OnError;
#else
        event EventHandler<object> OnError;
#endif

        /// <summary>
        /// The mouse has moved 
        /// </summary>
#if SILVERLIGHT
        event EventHandler OnMouseOver;
#else
        event EventHandler<object> OnMouseOver;
#endif

        #endregion

        #region Properties

        /// <summary>
        /// The duration of the current content
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// The position of the current content
        /// </summary>
        TimeSpan Position { get; }

        /// <summary>
        /// The amount of time that this item has rendered, regardless of seeks
        /// </summary>
        TimeSpan WatchedTime { get; }

        /// <summary>
        /// The total amount of content that has been rendered in this session
        /// </summary>
        TimeSpan TotalWatchedTime { get; }

#if !WINDOWS_PHONE
        /// <summary>
        /// True if the player is fullscreen
        /// </summary>
        bool FullScreen { get; }
#endif
        
        /// <summary>
        /// True if the player is playing content
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// True if the player is paused
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// True if the player is stopped, or not yet started
        /// </summary>
        bool IsStopped { get; }

        /// <summary>
        /// True if captions are active and being shown
        /// </summary>
        bool CaptionsActive { get; }

        /// <summary>
        /// True if the current content has a video stream
        /// </summary>
        bool HasVideo { get; }

        /// <summary>
        /// True if the current content has an audio stream
        /// </summary>
        bool HasAudio { get; }

        /// <summary>
        /// True if the current content has captions available
        /// </summary>
        bool HasCaptions { get; }

        /// <summary>
        /// The count of items that have been displayed in full or part
        /// </summary>
        int ItemsPlayed { get; }

        /// <summary>
        /// The physical width of the player application
        /// </summary>
        int PlayerWidth { get; }

        /// <summary>
        /// The physical height of the player application
        /// </summary>
        int PlayerHeight { get; }

        /// <summary>
        /// The native width of the current content
        /// </summary>
        int ContentWidth { get; }

        /// <summary>
        /// The native height of the current content
        /// </summary>
        int ContentHeight { get; }

        /// <summary>
        /// The bitrate-in-use of the current content
        /// </summary>
        long ContentBitrate { get; }

        /// <summary>
        /// The title of the current content
        /// </summary>
        string ContentTitle { get; }

        /// <summary>
        /// The URL that the current content was received from 
        /// </summary>
        string ContentUrl { get; }

        #endregion
    }
}
