using Microsoft.Media.AdaptiveStreaming;

namespace Microsoft.Media.AdaptiveStreaming.Helper
{
    public sealed class AdaptiveAudioStream
    {
        /// <summary> 
        /// Gets or sets the name of the audio stream. 
        /// </summary> 
        public string Name { get; set; }

        /// <summary> 
        /// Gets or sets the Language of the audio stream. 
        /// </summary> 
        public string Language { get; set; }

        internal AdaptiveAudioStream(IManifestStream manifestStream)
        {
            ManifestStream = manifestStream;
            Name = manifestStream.Name;
            Language = manifestStream.Language;

            if (string.IsNullOrEmpty(manifestStream.Name))
            {
                if (!string.IsNullOrEmpty(manifestStream.Language))
                {
                    var name = new Windows.Globalization.Language(manifestStream.Language).DisplayName;
                    if (!string.IsNullOrEmpty(name))
                    {
                        name = manifestStream.Language;
                    }
                    Name = name;
                }
            }
        }

        public IManifestStream ManifestStream { get; private set; }
    }
}
