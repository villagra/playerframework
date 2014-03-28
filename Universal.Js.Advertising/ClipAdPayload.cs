using System;
using Microsoft.Media.Advertising;

namespace Microsoft.PlayerFramework.Js.Advertising
{
    /// <summary>
    /// A single linear clip to play.
    /// </summary>
    public sealed class ClipAdPayload: IClipAdPayload
    {
        /// <summary>
        /// Creates a new instance of ClipAdPayload
        /// </summary>
        public ClipAdPayload()
        {
            MimeType = "";
        }

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