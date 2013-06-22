using Microsoft.Media.ISO.Boxes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AdaptiveStreaming.Dash.Smooth
{
    internal static class SmoothFactory
    {
        public static Stream GenerateClientManifestStream(IEnumerable<Box> Boxes)
        {
            var ManifestTracks = ManifestTrack.InitializeTrackRegistry(Boxes);
            return GenerateClientManifestStream(Boxes, ManifestTracks);
        }

        public static Stream GenerateClientManifestStream(IEnumerable<Box> Boxes, IEnumerable<ManifestTrack> ManifestTracks)
        {
            SmoothStreamingMedia manifest = new SmoothStreamingMedia();

            var moov = Boxes.SingleOrDefault(b => b.Type == BoxType.Moov);
            var mvhd = moov.InnerBoxes.SingleOrDefault(b => b.Type == BoxType.Mvhd) as MovieHeaderFullBox;

            manifest.Duration = ManifestTracks.Select(t => t.Duration).Max();
            manifest.TimeScale = mvhd.TimeScale;

            manifest.StreamIndex.AddRange(GenerateClientManifestStreamIndexs(ManifestTracks, moov));

            //create protection data if it exists
            var protectionBox = moov.InnerBoxes.SingleOrDefault(b => b.Type == BoxType.Pssh) as ProtectionSystemSpecificHeaderFullBox;

            if (protectionBox != null)
            {
                manifest.Protection = new SmoothStreamingMediaProtection()
                {
                    ProtectionHeader = new SmoothStreamingMediaProtectionProtectionHeader()
                    {
                        SystemID = protectionBox.SystemId.ToString().ToUpper(),
                        Value = Convert.ToBase64String(protectionBox.Data)
                    }
                };
            }

            return manifest.ToStream();
        }

        public static IEnumerable<SmoothStreamingMediaStreamIndex> GenerateClientManifestStreamIndexs(IEnumerable<ManifestTrack> ManifestTracks, Box moov)
        {
            foreach (var track in ManifestTracks)
            {
                yield return GenerateClientManifestStreamIndex(track, moov);
            }
        }
        
        public static SmoothStreamingMediaStreamIndex GenerateClientManifestStreamIndex(IList<Box> boxes)
        {
            var track = ManifestTrack.InitializeTrackRegistry(boxes).First(); 
            var moov = boxes.SingleOrDefault(b => b.Type == BoxType.Moov);
            return GenerateClientManifestStreamIndex(track, moov);
        }

        public static SmoothStreamingMediaStreamIndex GenerateClientManifestStreamIndex(ManifestTrack track, Box moov)
        {
            var streamIndex = new SmoothStreamingMediaStreamIndex();
            streamIndex.Type = track.Type.ToString().ToLower();

            var qualityLevel = new SmoothStreamingMediaStreamIndexQualityLevel();
            streamIndex.QualityLevel.Add(qualityLevel);
            streamIndex.TimeScale = track.TimeScale;

            var mdhd = moov.InnerBoxes.Single(box =>
                    box.Type == BoxType.Trak
                    && (box.InnerBoxes.Single(tkhd => tkhd.Type == BoxType.Tkhd) as TrackHeaderFullBox).TrackId == track.Id)
                .InnerBoxes.Single(box => box.Type == BoxType.Mdia)
                .InnerBoxes.Single(box => box.Type == BoxType.Mdhd) as MediaHeaderFullBox;

            streamIndex.Language = mdhd.Language;

            switch (track.Type)
            {
                case ManifestTrackType.Video:
                    //populate track
                    streamIndex.MaxHeight = track.Height;
                    streamIndex.MaxWidth = track.Width;
                    streamIndex.DisplayWidth = track.DisplayWidth;
                    streamIndex.DisplayHeight = track.DisplayHeight;

                    //populate quality level
                    qualityLevel.Index = 0;
                    qualityLevel.Bitrate = track.Bitrate;
                    qualityLevel.FourCC = track.FourCodecCode;
                    qualityLevel.MaxHeight = track.Height;
                    qualityLevel.MaxWidth = track.Width;
                    qualityLevel.CodecPrivateData = track.CodecPrivateData;

                    break;
                case ManifestTrackType.Audio:

                    streamIndex.FourCC = track.FourCodecCode;
                    streamIndex.Index = 0;

                    //populate quality level
                    qualityLevel.Bitrate = track.Bitrate;
                    qualityLevel.SamplingRate = track.SampleRate;
                    qualityLevel.Channels = track.ChannelCount;
                    qualityLevel.BitsPerSample = track.SampleSize;
                    qualityLevel.PacketSize = track.PacketSize;
                    qualityLevel.AudioTag = track.AudioTag;
                    qualityLevel.CodecPrivateData = track.CodecPrivateData;
                    qualityLevel.FourCC = track.FourCodecCode;

                    break;
                case ManifestTrackType.Text:
                    int i = 0;
                    i++;
                    break;
            }

            if (track.Fragments != null)
            {
                streamIndex.c.AddRange(GenerateClientManifestChunks(track, moov));
                streamIndex.Chunks = (uint)streamIndex.c.Count;
            }
            return streamIndex;
        }

        private static IEnumerable<SmoothStreamingMediaStreamIndexC> GenerateClientManifestChunks(ManifestTrack track, Box moov)
        {
            // Pull entries that point to different moof boxes, duplicate boxes offsets cause playback issues.
            var entries = track.Fragments.TrackFragmentRandomAccessEntries
                .GroupBy(entry => entry.MoofOffset)
                .Select(entry => entry.First())
                .ToList();
            ulong entriesDuration = 0;

            //create chunks
            for (int i = 0; i < entries.Count; i++)
            {
                var c = new SmoothStreamingMediaStreamIndexC();
                c.n = i;

                if (i != entries.Count - 1)
                {
                    // The duration is the difference between this entry and the next's time
                    c.d = entries.ElementAt(i + 1).Time - entries.ElementAt(i).Time;
                    entriesDuration += c.d;
                }
                else
                {
                    var mvhd = moov.InnerBoxes.SingleOrDefault(b => b.Type == BoxType.Mvhd) as MovieHeaderFullBox;
                    // Final duration is what we have left, the track duration is in the timescale of the presentation,
                    // so we have to convert to the timescale of the actual media in the track
                    c.d = ConvertTimeToTimescale(track.Duration, mvhd.TimeScale, track.TimeScale) - entriesDuration;
                }
                yield return c;
            }
        }

        private static ulong ConvertTimeToTimescale(ulong time, uint sourceTimescale, uint targetTimescale)
        {
            return (ulong)(((double)targetTimescale / (double)sourceTimescale) * time);
        }
    }
}
