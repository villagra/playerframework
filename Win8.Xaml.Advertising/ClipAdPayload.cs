using System;
using Microsoft.VideoAdvertising;

namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// A single linear clip to play.
    /// </summary>
    public sealed class ClipAdPayload :
#if NETFX_CORE
 Windows.UI.Xaml.FrameworkElement,
#endif
 IClipAdPayload
    {
        /// <summary>
        /// Gets or sets the source Uri of the ad clip.
        /// </summary>
        public Uri MediaSource { get; set; }

        /// <summary>
        /// Gets or sets the MimeType of the ad clip.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Gets or sets the click through Uri for the ad. Note: This is optional.
        /// </summary>
        public Uri ClickThrough { get; set; }
    }
}
