using System;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// The state of the MediaElement
    /// </summary>
    public class MediaState
    {
        /// <summary>
        /// Gets or sets if the media is paused (vs. playing).
        /// </summary>
        public bool IsPlaying { get; set; }

        /// <summary>
        /// Gets or sets the source Uri of the media.
        /// </summary>
        public Uri Source { get; set; }

        /// <summary>
        /// Gets or sets the last playback position of the media.
        /// </summary>
        public TimeSpan Position { get; set; }

        /// <summary>
        /// Gets or sets the player state.
        /// </summary>
        public bool IsStarted { get; set; }
    }
}
