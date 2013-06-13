using Microsoft.AdaptiveStreaming.Dash.Smooth;
using Microsoft.Media.ISO;
using Microsoft.Media.ISO.Boxes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.AdaptiveStreaming.Dash
{
    public sealed partial class DashDownloaderPlugin
    {
        Uri manifestUri;
        Dictionary<string, ChunkLocation> ChunkLookup;

        public event EventHandler<ChunkRequestedEventArgs> ChunkRequested;
        public event EventHandler<ManifestRequestedEventArgs> ManifestRequested;

        private async Task<WebRequestorResponse> DownloadChunkAsync(Uri source, CancellationToken c)
        {
            var response = await GetChunkAsync(source, c);
            c.ThrowIfCancellationRequested();
#if SILVERLIGHT
            if (response != null)
            {
                // SSME for Silverlight requires some additional changes to the chunk for DASH WAMS to work.
                response.Stream = HackFragment(response.Stream);
            }
#endif
            return response;
        }

        private async Task<WebRequestorResponse> GetChunkAsync(Uri source, CancellationToken c)
        {
            if (ChunkLookup.Any())
            {
                var key = source.AbsolutePath;

                if (ChunkLookup.ContainsKey(key))
                {
                    ChunkLocation chunkLocation = ChunkLookup[key];
                    if (ChunkRequested != null) ChunkRequested(this, new ChunkRequestedEventArgs(source, chunkLocation.Uri, string.Format("{0}-{1}", chunkLocation.From, chunkLocation.To)));
                    return await DownloadResolvedChunkAsync(source, chunkLocation, c);
                }
            }
            if (ChunkRequested != null) ChunkRequested(this, new ChunkRequestedEventArgs(source, source, null));
#if SILVERLIGHT // SILVERLIGHT requires that we download the chunk
            return await WebRequestor.GetResponseAsync(source);
#else
            return null;
#endif
        }

#if WINDOWS_PHONE
        private static Stream HackFragment(Stream stream)
        {
            long offset = 0;
            long versionBitOffset = 0;

            BoxBinaryReader reader = new BoxBinaryReader(stream);
            Box box = null;
            do
            {
                box = reader.ReadNextBox();
                if (box != null)
                {
                    if (box.Type == BoxType.Moof)
                    {
                        offset = box.Offset;
                        var traf = box.InnerBoxes.FirstOrDefault(b => b.Type == BoxType.Traf) as TrackFragmentBox;
                        if (traf != null)
                        {
                            var trun = traf.InnerBoxes.First(b => b.Type == BoxType.Trun) as TrackFragmentRunFullBox;
                            if (trun != null && trun.Version != 0)
                            {
                                versionBitOffset = trun.Offset + 8;
                            }
                        }
                        break;
                    }
                }
            } while (box != null);

            if (offset == 0 && versionBitOffset == 0)
            {
                return stream;
            }
            else
            {
                stream.Seek(offset, SeekOrigin.Begin);
                var buffer = new byte[stream.Length - offset];
                stream.Read(buffer, 0, buffer.Length);
                if (versionBitOffset != 0)
                {
                    versionBitOffset -= offset;
                    buffer[versionBitOffset] = 0;
                }
                return new MemoryStream(buffer);
            }
        }
#elif SILVERLIGHT
        private static Stream HackFragment(Stream stream)
        {
            return TrimToBox(stream, BoxType.Moof);
        }

        private static Stream TrimToBox(Stream stream, BoxType boxType)
        {
            long offset = 0;

            BoxBinaryReader reader = new BoxBinaryReader(stream);
            Box box = null;
            do
            {
                box = reader.ReadNextBox();
                if (box != null && box.Type == boxType)
                {
                    offset = box.Offset;
                    break;
                }
            } while (box != null);

            if (offset == 0)
            {
                return stream;
            }
            else
            {
                stream.Seek(offset, SeekOrigin.Begin);
                var buffer = new byte[stream.Length - offset];
                stream.Read(buffer, 0, buffer.Length);
                return new MemoryStream(buffer);
            }
        }
#endif

        private static async Task<WebRequestorResponse> DownloadResolvedChunkAsync(Uri source, ChunkLocation chunkLocation, CancellationToken c)
        {
            // download the chunk and keep the stream open
            return await WebRequestor.GetResponseAsync(chunkLocation.Uri, (long)chunkLocation.From, (long)chunkLocation.To);
        }

        private async Task<WebRequestorResponse> DownloadManifestAsync(Uri source, CancellationToken c)
        {
            // download and convert the manifest
            var response = await WebRequestor.GetResponseAsync(source);
            c.ThrowIfCancellationRequested();
            // convert the DASH stream to Smooth Streaming format
            XDocument sourceXml = XDocument.Load(response.Stream);
            Stream destStream;
            switch (sourceXml.Root.Name.LocalName)
            {
                case "SmoothStreamingMedia":
                    destStream = response.Stream;
                    break;
                case "MPD":
                    response.Stream.Dispose(); // we don't need it anymore
                    var conversionResult = await DashManifestConverter.ConvertToSmoothManifest(sourceXml, source);
                    destStream = conversionResult.Manifest.ToStream();
                    ChunkLookup = conversionResult.ChunkLookup;
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (ManifestRequested != null)
            {
                var destXml = new StreamReader(destStream).ReadToEnd();
                ManifestRequested(this, new ManifestRequestedEventArgs(source, sourceXml.ToString(), destXml));
            }

            destStream.Seek(0, SeekOrigin.Begin);
            response.Stream = destStream;
            return response;
        }

        void OnCloseMedia()
        {
            manifestUri = null;
        }

        void OnOpenMedia(Uri manifestUri)
        {
            this.manifestUri = manifestUri;
        }
    }

#if SILVERLIGHT
    public sealed class ChunkRequestedEventArgs : EventArgs
#else
    public sealed class ChunkRequestedEventArgs
#endif
    {
        internal ChunkRequestedEventArgs(Uri source, Uri newChunkUri, string newChunkRange)
        {
            Source = source;
            NewChunkUri = newChunkUri;
            NewChunkRange = newChunkRange;
        }

        public Uri Source { get; private set; }
        public Uri NewChunkUri { get; private set; }
        public string NewChunkRange { get; private set; }
    }

#if SILVERLIGHT
    public sealed class ManifestRequestedEventArgs : EventArgs
#else
    public sealed class ManifestRequestedEventArgs
#endif
    {
        internal ManifestRequestedEventArgs(Uri source, string originalManifest, string newManifest)
        {
            Source = source;
            OriginalManifest = originalManifest;
            NewManifest = newManifest;
        }

        public Uri Source { get; private set; }
        public string OriginalManifest { get; private set; }
        public string NewManifest { get; private set; }
    }
}
