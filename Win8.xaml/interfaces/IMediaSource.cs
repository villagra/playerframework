using System;
using System.Collections.Generic;
#if SILVERLIGHT
using System.Windows.Media;
using System.Collections.ObjectModel;
#else
using Windows.UI.Xaml.Media;
using Windows.Media.Protection;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A source and associated properties required to control initial state and desired playback behavior of the MediaPlayer.
    /// </summary>
    public interface IMediaSource
    {
#if SILVERLIGHT
        /// <summary>
        /// Gets or sets the System.Windows.Media.LicenseAcquirer associated with the MediaElement. The LicenseAcquirer handles acquiring licenses for DRM encrypted content.
        /// </summary>
        LicenseAcquirer LicenseAcquirer { get; set; }
#else
        /// <summary>
        /// Gets or sets the dedicated object for media content protection that is associated with this MediaElement.
        /// </summary>
        MediaProtectionManager ProtectionManager { get; set; }
        
        /// <summary>
        /// Gets or sets an enumeration value that determines the stereo 3-D video frame-packing mode for the current media source.
        /// </summary>
        Stereo3DVideoPackingMode Stereo3DVideoPackingMode { get; set; }

        /// <summary>
        /// Gets or sets an enumeration value that determines the stereo 3-D video render mode for the current media source.
        /// </summary>
        Stereo3DVideoRenderMode Stereo3DVideoRenderMode { get; set; }
#endif

        /// <summary>
        /// The image source for a poster image.
        /// </summary>
        ImageSource PosterSource { get; set; }

        /// <summary>
        /// Gets or sets a gate for loading the source. Setting this to false postpones any subsequent calls to the Source property and SetSource method.
        /// Once the source is set on the underlying MediaElement, the media begins to download.
        /// Note: There is another opportunity to block setting the source by using the awaitable BeforeMediaLoaded event.
        /// </summary>
        bool AutoLoad { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether media will begin playback automatically when the Source property is set.
        /// </summary>
        bool AutoPlay { get; set; }

        /// <summary>
        /// Gets or sets the position at which to start the video at. This is useful for resuming videos at the place they were left off at.
        /// </summary>
        TimeSpan? StartupPosition { get; set; }

        /// <summary>
        /// Gets or sets a media source on the MediaElement.
        /// A string that specifies the source of the element, as a Uniform Resource Identifier (URI).
        /// </summary>
        Uri Source { get; set; }

        /// <summary>
        /// Gets this audio stream names to be displayed to the user for selecting from multiple audio tracks.
        /// </summary>
        ICollection<AudioStream> AvailableAudioStreams { get; }

        /// <summary>
        /// Gets a collection of timeline markers to display in the timeline.
        /// </summary>
        ICollection<VisualMarker> VisualMarkers { get; }

        /// <summary>
        /// Gets a collection of available captions.
        /// </summary>
        ICollection<Caption> AvailableCaptions { get; }
    }
}
