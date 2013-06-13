using Microsoft.AdaptiveStreaming.Dash.Smooth;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AdaptiveStreaming.Dash
{
    internal partial class CffDownloaderPlugin
    {
        const string RegexFragmentRequest = @"^.*/QualityLevels\((?<bitrate>[0-9]+)\)/(Fragments|KeyFrames)\((?<trackType>audio|video|text)=(?<timeOffset>[0-9]+)\)(/Language\((?<language>\w+)\))?$";

        Uri manifestUri;
        readonly CffFileParser parser;

        public CffDownloaderPlugin(CffFileParser parser)
        {
            this.parser = parser;
        }

        protected virtual async Task<Stream> DownloadManifestAsync(Uri source, CancellationToken c)
        {
            await this.parser.Parse(source);
            c.ThrowIfCancellationRequested();

            var manifestStream = this.parser.GenerateClientManifestStream();

#if DEBUG
            var reader = new System.IO.StreamReader(manifestStream);
            Debug.WriteLine(reader.ReadToEnd());
            manifestStream.Seek(0, SeekOrigin.Begin);
#endif
            parser.Boxes.Clear();

            return manifestStream;
        }

        protected virtual async Task<WebRequestorResponse> DownloadChunkAsync(Uri source, CancellationToken c)
        {
            Regex regex = new Regex(RegexFragmentRequest, RegexOptions.IgnoreCase);
            var match = regex.Match(source.AbsolutePath);

            if (match != null)
            {
                var trackType = match.Groups["trackType"].Value;
                var timeOffset = ulong.Parse(match.Groups["timeOffset"].Value);
                var bitrate = uint.Parse(match.Groups["bitrate"].Value);
                var language = match.Groups["language"].Value;
                var manifestTrackType = (ManifestTrackType)Enum.Parse(typeof(ManifestTrackType), trackType, true);

                var fragmentStream = await this.parser.GetTrackFragmentStream(manifestTrackType, bitrate, timeOffset, language);
                c.ThrowIfCancellationRequested();

                return fragmentStream;
            }
            else
            {
                return null;
            }
        }

        public void OnOpenMedia(Uri manifestUri)
        {
            this.manifestUri = manifestUri;
        }

        public void OnCloseMedia()
        {
            this.parser.Close();
            manifestUri = null;
        }
    }
}
