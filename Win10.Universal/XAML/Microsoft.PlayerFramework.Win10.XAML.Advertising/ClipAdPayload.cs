using System;
using Microsoft.Media.Advertising;

namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// A single linear clip to play.
    /// </summary>
    public sealed class ClipAdPayload : Windows.UI.Xaml.FrameworkElement, IClipAdPayload
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
