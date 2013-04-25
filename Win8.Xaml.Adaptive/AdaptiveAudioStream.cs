using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Media.AdaptiveStreaming;

#if XAML
namespace Microsoft.PlayerFramework.Adaptive
#else
namespace Microsoft.PlayerFramework.Js.Adaptive
#endif
{
    public sealed class AdaptiveAudioStream : AudioStream
    {
        public AdaptiveAudioStream(IManifestStream manifestStream)
        {
            ManifestStream = manifestStream;
            base.Name = manifestStream.Name;
            base.Language = manifestStream.Language;
        }

        public IManifestStream ManifestStream { get; private set; }
    }
}
