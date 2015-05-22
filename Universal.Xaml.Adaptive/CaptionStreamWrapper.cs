using Microsoft.Media.AdaptiveStreaming.Helper;

namespace Microsoft.PlayerFramework.Adaptive
{
    /// <summary>
    /// Wraps a smooth streaming AdaptiveCaptionStream class to allow it to inherit Caption and participate in the player framework's caption selection APIs.
    /// </summary>
    public class CaptionStreamWrapper: Caption
    {
        internal CaptionStreamWrapper(AdaptiveCaptionStream adaptiveCaptionStream)
        {
            AdaptiveCaptionStream = adaptiveCaptionStream;
            base.Id = adaptiveCaptionStream.Name;
            base.Description = adaptiveCaptionStream.Language;
        }

        /// <summary>
        /// Gets the underlying smooth streaming AdaptiveCaptionStream instance.
        /// </summary>
        public AdaptiveCaptionStream AdaptiveCaptionStream { get; private set; }
    }
}
