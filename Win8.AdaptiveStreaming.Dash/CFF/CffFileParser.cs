using Microsoft.AdaptiveStreaming.Dash.Smooth;
using Microsoft.Media.ISO;
using Microsoft.Media.ISO.Boxes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AdaptiveStreaming.Dash
{
    internal abstract class CffFileParser
    {
        public CffFileParser()
        {
            this.Boxes = new List<Box>();
            this.ManifestTracks = new List<ManifestTrack>();
        }

        public List<Box> Boxes { get; private set; }
        public List<ManifestTrack> ManifestTracks { get; private set; }

        public abstract Task Parse(Uri path);

        public abstract Task<WebRequestorResponse> GetTrackFragmentStream(ManifestTrackType trackType, uint bitrate, ulong timeOffset, string language);
        
        public virtual Stream GenerateClientManifestStream()
        {
            return SmoothFactory.GenerateClientManifestStream(this.Boxes, this.ManifestTracks);
        }
        
        protected ulong ReadBoxSize(BoxBinaryReader reader)
        {
            ulong size = 0;

            size = reader.ReadUInt32();

            if (size == 1)
            {
                reader.BaseStream.Seek(4, SeekOrigin.Current);  // Skip over the box type
                size = reader.ReadUInt64();
            }

            return size;
        }

        protected virtual void InitializeTrackRegistry()
        {
            this.CalculateTfraEntrySizes();     // Figure out how large our samples are so we can initialize the track registry
            this.ManifestTracks = ManifestTrack.InitializeTrackRegistry(this.Boxes);
        }

        /// <summary>
        /// Calculates the size of each tfra sample entry, which represents a moof and mdat pair.
        /// </summary>
        private void CalculateTfraEntrySizes()
        {
            var entries = this.Boxes.Single(box => box.Type == BoxType.Mfra)
                .InnerBoxes.Where(box => box.Type == BoxType.Tfra).Cast<TrackFragmentRandomAccessFullBox>()
                .SelectMany(tfra => tfra.TrackFragmentRandomAccessEntries)
                .OrderBy(entry => entry.MoofOffset).ToList();
            var entryCount = entries.Count;

            for (int i = 0; i < entryCount; i++)
            {
                ulong nextOffset = 0;

                if (i + 1 < entryCount)
                {
                    nextOffset = entries.ElementAt(i + 1).MoofOffset;
                }
                else
                {
                    nextOffset = (ulong)this.Boxes.Single(box => box.Type == BoxType.Mfra).Offset;
                }

                entries.ElementAt(i).SampleSize = nextOffset - entries.ElementAt(i).MoofOffset;
            }
        }

        /// <summary>
        /// Resets any data that had been setup by a previous operation and prepares
        /// the parser for a new set of data.
        /// </summary>
        public virtual void Close()
        {
            this.Boxes.Clear();
            this.ManifestTracks.Clear();
        }
    }
}
