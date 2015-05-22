using Microsoft.Media.AdaptiveStreaming.Helper;

namespace Microsoft.PlayerFramework.Adaptive
{
    /// <summary>
    /// Wraps a smooth streaming AdaptiveAudioStream class to allow it to inherit AudioStream and participate in the player framework's audio selection APIs.
    /// </summary>
    public class AudioStreamWrapper: AudioStream
    {
        internal AudioStreamWrapper(AdaptiveAudioStream adaptiveAudioStream)
        {
            AdaptiveAudioStream = adaptiveAudioStream;
            base.Name = adaptiveAudioStream.Name;
            base.Language = adaptiveAudioStream.Language;
        }

        /// <summary>
        /// Gets the underlying smooth streaming AdaptiveAudioStream instance.
        /// </summary>
        public AdaptiveAudioStream AdaptiveAudioStream { get; private set; }
    }
}
