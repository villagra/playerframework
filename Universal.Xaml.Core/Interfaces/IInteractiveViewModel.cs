#define CODE_ANALYSIS

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.ObjectModel;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Provides an interface to control media interactivity. 
    /// The MediaPlayer implements this by default but a plugin could also provide an implementation for features like advertising.
    /// </summary>
    public interface IInteractiveViewModel
    {
        /// <summary>
        /// Can be called by UI elements to indicate that the user is interacting
        /// </summary>
        void OnInteracting(InteractionType activityType);

        /// <summary>
        /// Raised when the user interacts
        /// </summary>
        event EventHandler<InteractionEventArgs> Interacting;

        /// <summary>
        /// Gets a collection of markers to display
        /// </summary>
        IEnumerable<VisualMarker> VisualMarkers { get; }

        /// <summary>
        /// Gets whether or not he media is playing at high definition. Usually this means a resolution >= 1280×720 pixels (720p)
        /// </summary>
        MediaQuality MediaQuality { get; }

        /// <summary>
        /// Gets a value that indicates the current buffering progress.
        /// The amount of buffering that is completed for media content. The value ranges from 0 to 1. Multiply by 100 to obtain a percentage.
        /// </summary>
        double BufferingProgress { get; }

        /// <summary>
        /// Gets a percentage value indicating the amount of download completed for content located on a remote server.
        /// The value ranges from 0 to 1. Multiply by 100 to obtain a percentage.
        /// </summary>
        double DownloadProgress { get; }

        /// <summary>
        /// Gets the start time of the current video or audio.
        /// </summary>
        TimeSpan StartTime { get; }

        /// <summary>
        /// Gets the end time of the current video or audio.
        /// </summary>
        TimeSpan EndTime { get; }

        /// <summary>
        /// Gets the duration of the current video or audio.
        /// </summary>
        TimeSpan Duration { get; }

        /// <summary>
        /// Gets the time remaining before the media will finish.
        /// </summary>
        TimeSpan TimeRemaining { get; }

        /// <summary>
        /// Gets the current position of progress through the media's playback time (or the amount of time since the beginning of the media).
        /// </summary>
        TimeSpan Position { get; }

        /// <summary>
        /// Gets the maximum position that the user can seek or scrub to in the timeline.
        /// Useful for realtime/live playback.
        /// </summary>
        TimeSpan MaxPosition { get; }

        /// <summary>
        /// Gets the status of the MediaElement.
        /// The state can be one of the following (as defined in the MediaElementState enumeration):
        /// Buffering, Closed, Opening, Paused, Playing, Stopped.
        /// </summary>
        MediaElementState CurrentState { get; }

        /// <summary>
        /// Gets or sets the media's volume.
        /// The media's volume represented on a linear scale between 0 and 1. The default is 0.5.
        /// </summary>
        double Volume { get; set; }

#if !WINDOWS80
        /// <summary>
        /// Gets or sets whether the video frame is trimmed, on the top and bottom or left and right, to fit the video display.
        /// When true, trims the video frame to the display space. When false, the video frame uses letter box or pillarbox to display video. Default is false.
        /// </summary>
        bool Zoom { get; set; }
#endif

        /// <summary>
        /// Gets or sets the selected caption.
        /// </summary>
        Caption SelectedCaption { get; set; }

        /// <summary>
        /// Gets the caption stream names to be displayed to the user for selecting from multiple captions.
        /// </summary>
        IEnumerable<Caption> AvailableCaptions { get; }

        /// <summary>
        /// Gets or sets the selected caption.
        /// </summary>
        AudioStream SelectedAudioStream { get; set; }

        /// <summary>
        /// Gets the caption stream names to be displayed to the user for selecting from multiple captions.
        /// </summary>
        IEnumerable<AudioStream> AvailableAudioStreams { get; }

        /// <summary>
        /// Gets a an IValueConverter that is used to display the time to the user such as the position, duration, and time remaining.
        /// The default value applies the string format of "h\\:mm\\:ss".
        /// </summary>
        IValueConverter TimeFormatConverter { get; }

        /// <summary>
        /// Gets the amount of time in the video to skip back when the user selects skip back.
        /// This can be set to null to cause the skip back action to go back to the beginning.
        /// </summary>
        TimeSpan? SkipBackInterval { get; }

        /// <summary>
        /// Gets the amount of time in the video to skip next when the user selects skip ahead.
        /// This can be set to null to cause the skip next action to go directly to the end.
        /// </summary>
        TimeSpan? SkipAheadInterval { get; }

        /// <summary>
        /// Gets or sets whether or not the media is playing in slow motion.
        /// </summary>
        bool IsSlowMotion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the audio is muted.
        /// </summary>
        bool IsMuted { get; set; }

        /// <summary>
        /// Gets or sets if the player should indicate it is in fullscreen mode.
        /// </summary>
        bool IsFullScreen { get; set; }

        /// <summary>
        /// Gets the signal strength used to indicate visually to the user the quality of the bitrate.
        /// Note: This is only useful for adaptive streaming.
        /// </summary>
        double SignalStrength { get; }

        /// <summary>
        /// Gets the enabled state of the closed captions feature.
        /// </summary>
        bool IsCaptionSelectionEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the play/resume feature.
        /// </summary>
        bool IsPlayResumeEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the pause feature.
        /// </summary>
        bool IsPauseEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the stop feature.
        /// </summary>
        bool IsStopEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the replay feature.
        /// </summary>
        bool IsReplayEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the audio stream selection feature.
        /// </summary>
        bool IsAudioSelectionEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the rewind feature.
        /// </summary>
        bool IsRewindEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the fast forward feature.
        /// </summary>
        bool IsFastForwardEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the slow motion feature.
        /// </summary>
        bool IsSlowMotionEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the seek feature.
        /// </summary>
        bool IsSeekEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the skip previous feature.
        /// </summary>
        bool IsSkipPreviousEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the skip next feature.
        /// </summary>
        bool IsSkipNextEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the skip back feature.
        /// </summary>
        bool IsSkipBackEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the skip ahead feature.
        /// </summary>
        bool IsSkipAheadEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the scrubbing feature.
        /// </summary>
        bool IsScrubbingEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the go live feature.
        /// </summary>
        bool IsGoLiveEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the info feature.
        /// </summary>
        bool IsInfoEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the more feature.
        /// </summary>
        bool IsMoreEnabled { get; }

        /// <summary>
        /// Gets the enabled state of the fullscreen feature.
        /// </summary>
        bool IsFullScreenEnabled { get; }

#if !WINDOWS80
        /// <summary>
        /// Gets the enabled state of the zoom feature.
        /// </summary>
        bool IsZoomEnabled { get; }
#endif


#if WINDOWS_UWP
        /// <summary>
        /// Gets the enabled state of the casting feature.
        /// </summary>
        bool IsCastingEnabled { get; }
#endif

        /// <summary>
        /// Gets the thumbnail image to display
        /// </summary>
        ImageSource ThumbnailImageSource { get; }

        /// <summary>
        /// Raised when the IsPlayResumeEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsPlayResumeEnabledChanged;

        /// <summary>
        /// Raised when the IsPauseEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsPauseEnabledChanged;

        /// <summary>
        /// Raised when the IsStopEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsStopEnabledChanged;

        /// <summary>
        /// Raised when the IsReplayEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsReplayEnabledChanged;

        /// <summary>
        /// Raised when the IsAudioSelectionEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsAudioSelectionEnabledChanged;

        /// <summary>
        /// Raised when the IsAudioSelectionEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsCaptionSelectionEnabledChanged;

        /// <summary>
        /// Raised when the IsRewindEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsRewindEnabledChanged;

        /// <summary>
        /// Raised when the IsFastForwardEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsFastForwardEnabledChanged;

        /// <summary>
        /// Raised when the IsSlowMotionEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsSlowMotionEnabledChanged;

        /// <summary>
        /// Raised when the IsSeekEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsSeekEnabledChanged;

        /// <summary>
        /// Raised when the IsSkipPreviousEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsSkipPreviousEnabledChanged;

        /// <summary>
        /// Raised when the IsSkipNextEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsSkipNextEnabledChanged;

        /// <summary>
        /// Raised when the IsSkipBackEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsSkipBackEnabledChanged;

        /// <summary>
        /// Raised when the IsSkipAheadEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsSkipAheadEnabledChanged;

        /// <summary>
        /// Raised when the IsScrubbingEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsScrubbingEnabledChanged;

        /// <summary>
        /// Raised when the IsGoLiveEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsGoLiveEnabledChanged;

        /// <summary>
        /// Raised when the IsInfoEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsInfoEnabledChanged;

        /// <summary>
        /// Raised when the IsMoreEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsMoreEnabledChanged;

        /// <summary>
        /// Raised when the IsFullScreenEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsFullScreenEnabledChanged;

#if !WINDOWS80
        /// <summary>
        /// Raised when the IsZoomEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsZoomEnabledChanged;
#endif


#if WINDOWS_UWP
        /// <summary>
        /// Raised when the IsCastingEnabled property changes.
        /// </summary>
        event RoutedEventHandler IsCastingEnabledChanged;

        /// <summary>
        /// Invokes the cast feature
        /// </summary>
        void InvokeCast();
#endif

        /// <summary>
        /// Occurs when the value of the CurrentState property changes.
        /// </summary>
        event RoutedEventHandler CurrentStateChanged;

        /// <summary>
        /// Invokes the closed captioning feature.
        /// </summary>
        void InvokeCaptionSelection();

        /// <summary>
        /// Invokes the audio stream selection feature.
        /// </summary>
        void InvokeAudioSelection();

        /// <summary>
        /// Invokes the more info feature
        /// </summary>
        void InvokeInfo();

        /// <summary>
        /// Invokes the more feature
        /// </summary>
        void InvokeMore();

        /// <summary>
        /// Seeks to the live position on the media. Only supported during live media playback.
        /// </summary>
        void GoLive();

        /// <summary>
        /// Actively scrubbing at the indicated position.
        /// </summary>
        /// <param name="position">The position on the timeline that the user is actively scrubbing over.</param>
        /// <param name="canceled">Gets or sets whether the operation should be cancelled</param>
        void Scrub(TimeSpan position, out bool canceled);

        /// <summary>
        /// A scrub action has been initiated.
        /// </summary>
        /// <param name="position">The position that the scrubbing action was initiated at.</param>
        /// <param name="canceled">Gets or sets whether the operation should be cancelled</param>
        void StartScrub(TimeSpan position, out bool canceled);

        /// <summary>
        /// A scrub action has completed.
        /// </summary>
        /// <param name="position">The position that the scrubbing action was completed.</param>
        /// <param name="canceled">Gets or sets whether the operation should be cancelled</param>
        void CompleteScrub(TimeSpan position, ref bool canceled);

        /// <summary>
        /// Skip back to the previous marker in the timeline.
        /// </summary>
        void SkipPrevious();

        /// <summary>
        /// Skip forward to the next position in the timeline.
        /// </summary>
        void SkipNext();

        /// <summary>
        /// Skip back to a previous position in the timeline. (usually 30 seconds)
        /// </summary>
        void SkipBack();

        /// <summary>
        /// Skip ahead to a future position in the timeline. (usually 30 seconds)
        /// </summary>
        void SkipAhead();

        /// <summary>
        /// Seek to a specific position.
        /// </summary>
        /// <param name="position">The position to seek to.</param>
        /// <param name="canceled">Allows the consumer to indicate the seek should be canceled.</param>
        void Seek(TimeSpan position, out bool canceled);

        /// <summary>
        /// Play or resume media playback. If playback is in rewind or fastforward mode, restore the original playback rate.
        /// </summary>
        void PlayResume();

        /// <summary>
        /// Pause the media.
        /// </summary>
        void Pause();

        /// <summary>
        /// Stop the media.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "MediaElement compatibility")]
        void Stop();

        /// <summary>
        /// Replay the media (e.g. instant replay).
        /// </summary>
        void Replay();

        /// <summary>
        /// Decrease the playback rate.
        /// </summary>
        void DecreasePlaybackRate();

        /// <summary>
        /// Increase the playback rate.
        /// </summary>
        void IncreasePlaybackRate();
    }

    /// <summary>
    /// Provides information about the interaction that occurred.
    /// </summary>
    public sealed class InteractionEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the InteractionEventArgs
        /// </summary>
        /// <param name="interactionType">The type of interaction that occurred.</param>
        public InteractionEventArgs(InteractionType interactionType)
        {
            InteractionType = interactionType;
        }

        /// <summary>
        /// The type of interaction that occurred.
        /// </summary>
        public InteractionType InteractionType { get; private set; }
    }
}
