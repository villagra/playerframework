using System;
using System.Linq;
using Microsoft.Web.Media.SmoothStreaming;
using System.Globalization;
using Microsoft.Media.AdaptiveStreaming.Helper;

namespace Microsoft.PlayerFramework.Adaptive
{
    /// <summary>
    /// Wraps a smooth streaming StreamInfo class to allow it to inherit Caption and participate in the player framework's audio selection APIs.
    /// </summary>
    public class CaptionStreamWrapper : Caption
    {
        internal CaptionStreamWrapper(StreamInfo adaptiveCaptionStream)
        {
            AdaptiveCaptionStream = adaptiveCaptionStream;
            base.Id = adaptiveCaptionStream.GetName();
            base.Description = adaptiveCaptionStream.GetLanguage();
        }

        /// <summary>
        /// Gets the underlying smooth streaming StreamInfo instance.
        /// </summary>
        public StreamInfo AdaptiveCaptionStream { get; private set; }
    }
}
