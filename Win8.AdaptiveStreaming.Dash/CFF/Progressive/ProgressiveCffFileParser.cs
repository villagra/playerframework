using Microsoft.AdaptiveStreaming.Dash.Smooth;
using Microsoft.Media.ISO;
using Microsoft.Media.ISO.Boxes;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AdaptiveStreaming.Dash
{
    internal class ProgressiveCffFileParser : CffFileParser
    {
        private long fileOffset;
        private Uri fileUri;

        public override async Task Parse(Uri path)
        {
            fileOffset = 0;
            fileUri = path;

            await this.ReadMovieHeaderBoxes();
            await this.ReadMovieFragmentRandomAccess();
            InitializeTrackRegistry();
        }

        public override async Task<WebRequestorResponse> GetTrackFragmentStream(ManifestTrackType trackType, uint bitrate, ulong timeOffset, string language)
        {
            if (language == string.Empty) language = null;

            var track = this.ManifestTracks
                .Where(t => t.Type == trackType && t.Bitrate == bitrate && t.Language == language)
                .SingleOrDefault();

            if (track != null)
            {
                var entry = track.Fragments.TrackFragmentRandomAccessEntries.FirstOrDefault(e => e.Time == timeOffset);

                if (entry != null)
                {
                    return await DownloadFragment(this.fileUri, (long)entry.MoofOffset, (long)entry.SampleSize);
                }
            }
            return null;
        }

        private static async Task<WebRequestorResponse> DownloadFragment(Uri uri, long offset, long size)
        {
            return await WebRequestor.GetResponseAsync(uri, offset, offset + size - 1);
        }

        /// <summary>
        /// Reads the header boxes for the video, which includes 
        /// ftyp, pdin, bloc, moov, and the optional mdat.
        /// </summary>
        /// <param name="callback"></param>
        private async Task ReadMovieHeaderBoxes()
        {
            var moov = await this.GetBox(BoxType.Moov);
            this.Boxes.Add(moov);

            // See if we have an mdat next and grab it if we do
            var stream = await WebRequestor.GetStreamRangeAsync(this.fileUri, this.fileOffset, this.fileOffset + 8);
            using (var reader = new BoxBinaryReader(stream))
            {
                if (reader.PeekNextBoxType() == BoxType.Mdat)
                {
                    var mdat = await this.GetNextBox();
                    this.Boxes.Add(mdat);
                }
            }
        }
        
        /// <summary>
        /// This method of building the mfra will make web requests in order to download the data
        /// from the online source.
        /// </summary>
        /// <param name="callback">The action that should be notified when the process is complete.</param>
        private async Task ReadMovieFragmentRandomAccess()
        {
            // grab the mfra offset
            var offsetStream = await WebRequestor.GetStreamRangeAsync(this.fileUri, -4);
            uint mfraOffset = 0;

            using (var reader = new BoxBinaryReader(offsetStream))
            {
                mfraOffset = reader.ReadUInt32();
            }

            // grab the mfra data
            var mfraStream = await WebRequestor.GetStreamRangeAsync(this.fileUri, -mfraOffset);
            // Write the bytes to our TOC file
            using (var reader = new BoxBinaryReader(mfraStream))
            {
                reader.GotoPosition(0);

                Box box = null;

                do
                {
                    box = reader.ReadNextBox();
                    if (box != null)
                    {
                        this.Boxes.Add(box);
                    }
                } while (box != null);
            }
        }

        private async Task<Box> GetNextBox()
        {
            var size = await this.GetBoxSize(this.fileOffset);
            var boxStream = await WebRequestor.GetStreamRangeAsync(this.fileUri, this.fileOffset, this.fileOffset + (long)size);
            Box box = null;

            using (var boxReader = new BoxBinaryReader(boxStream))
            {
                box = boxReader.ReadNextBox();
                this.fileOffset += (long)size;
            }

            return box;
        }

        private async Task<Box> GetBox(BoxType boxType)
        {
            // get the box size
            var size = await this.GetBoxSize(this.fileOffset);
            // gets the box
            var stream = await WebRequestor.GetStreamRangeAsync(this.fileUri, this.fileOffset, this.fileOffset + (long)size);
            Box box = null;

            using (var boxReader = new BoxBinaryReader(stream))
            {
                box = boxReader.ReadNextBox();
                this.fileOffset += (long)size;
            }

            if (box.Type == boxType)
            {
                return box;
            }
            else
            {
                return await this.GetBox(boxType);
            }
        }

        private async Task<ulong> GetBoxSize(long position)
        {
            var stream = await WebRequestor.GetStreamRangeAsync(this.fileUri, position, position + 16);
            using (var reader = new BoxBinaryReader(stream))
            {
                var size = this.ReadBoxSize(reader);
                return size;
            }
        }

        public override void Close()
        {
            base.Close();
            this.fileUri = null;
        }
    }
}