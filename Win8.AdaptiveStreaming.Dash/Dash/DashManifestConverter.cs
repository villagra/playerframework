using Microsoft.Media.ISO;
using Microsoft.Media.ISO.Boxes;
using Microsoft.AdaptiveStreaming.Dash.Smooth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.AdaptiveStreaming.Dash
{
    internal class ChunkLocation
    {
        public Uri Uri { get; set; }
        public ulong From { get; set; }
        public ulong To { get; set; }
    }

    internal class DashManifestConversionResult
    {
        public DashManifestConversionResult()
        {
            ChunkLookup = new Dictionary<string, ChunkLocation>();
        }

        public Dictionary<string, ChunkLocation> ChunkLookup { get; private set; }
        public SmoothStreamingMedia Manifest { get; set; }
    }

    internal static class DashManifestConverter
    {
        public static async Task<DashManifestConversionResult> ConvertToSmoothManifest(XDocument source, Uri rootUri)
        {
            var result = new DashManifestConversionResult();

            SmoothStreamingMedia manifest = new SmoothStreamingMedia();
            result.Manifest = manifest;

            var mpd = MPDFactory.LoadMPD(source.Root);

            // Get Duration
            if (mpd.MediaPresentationDuration.HasValue)
            {
                manifest.Duration = (ulong)mpd.MediaPresentationDuration.Value.Ticks;
            }
            manifest.IsLive = (mpd.Type == Presentation.Dynamic);
            //manifest.LookaheadCount = 2;
            if (mpd.AvailabilityEndTime.HasValue && mpd.AvailabilityStartTime.HasValue)
            {
                manifest.DVRWindowLength = (ulong)mpd.AvailabilityEndTime.Value.Subtract(mpd.AvailabilityStartTime.Value).Ticks;
            }

            foreach (var period in mpd.Period)
            {
                foreach (var adaptationSet in period.AdaptationSet)
                {
                    SmoothStreamingMediaStreamIndex streamIndex = null;
                    int representationIndex = 0;
                    foreach (var representation in adaptationSet.Representation)
                    {
                        string mediaUrl = null;
                        Uri initializationUri = null;
                        WebRequestor.Range initializationRange = null;
                        if (adaptationSet.SegmentTemplate != null)
                        {
                            var segmentTemplate = adaptationSet.SegmentTemplate;
                            var initializationTemplateUrl = segmentTemplate.InitializationValue;

                            initializationTemplateUrl = initializationTemplateUrl
                                .Replace("$$", "$")
                                .Replace("$RepresentationId$", representation.Id)
                                .Replace("$Number$", representationIndex.ToString())
                                .Replace("$Bandwidth$", representation.Bandwidth.ToString());
                            initializationUri = new Uri(rootUri, initializationTemplateUrl);
                            mediaUrl = segmentTemplate.Media
                                .Replace("$$", "$")
                                .Replace("$Bandwidth$", "{bitrate}")
                                .Replace("$Time$", "{start time}");
                        }
                        else if (representation.SegmentBase != null)
                        {
                            var baseUrl = representation.BaseURL.First().Value;
                            var segmentBase = representation.SegmentBase;
                            if (segmentBase.Initialization != null)
                            {
                                if (!string.IsNullOrEmpty(segmentBase.Initialization.SourceURL))
                                {
                                    initializationUri = new Uri(rootUri, segmentBase.Initialization.SourceURL);
                                }
                                initializationRange = WebRequestor.Range.FromString(segmentBase.Initialization.Range);
                            }
                            if (initializationUri == null)
                            {
                                initializationUri = new Uri(rootUri, baseUrl);
                            }
                        }
                        else throw new NotImplementedException();

                        var initializationBoxes = await GetBoxesAsync(initializationUri, initializationRange);

                        if (manifest.Protection == null) // support for CENC encryption
                        {
                            var moov = initializationBoxes.SingleOrDefault(b => b.Type == BoxType.Moov);
                            if (moov != null)
                            {
                                manifest.Protection = SmoothFactory.GetProtectionHeader(moov);
                            }
                        }

                        var trackStreamIndex = SmoothFactory.GenerateClientManifestStreamIndex(initializationBoxes);
                        var track = trackStreamIndex.QualityLevel.First();
                        if (streamIndex != null)
                        {
                            track.Index = (uint)streamIndex.QualityLevel.Count; // index is zero based so this is always equal to the current count
                            streamIndex.QualityLevel.Add(track);
                            streamIndex.MaxWidth = Math.Max(trackStreamIndex.MaxWidth, streamIndex.MaxWidth);
                            streamIndex.MaxHeight = Math.Max(trackStreamIndex.MaxHeight, streamIndex.MaxHeight);
                            streamIndex.DisplayWidth = streamIndex.MaxWidth;
                            streamIndex.DisplayHeight = streamIndex.MaxHeight;
                        }
                        else
                        {
                            streamIndex = trackStreamIndex;
                            if (mediaUrl != null) streamIndex.Url = mediaUrl;
                        }

                        track.Bitrate = representation.Bandwidth;

                        // create chunks
                        if (adaptationSet.SegmentTemplate != null)
                        {
                            if (adaptationSet.SegmentTemplate.SegmentTimeline != null)
                            {
                                if (!streamIndex.c.Any())
                                {
                                    streamIndex.c.AddRange(CreateChunks(adaptationSet.SegmentTemplate.SegmentTimeline));
                                    if (!manifest.IsLive) streamIndex.Chunks = (uint)streamIndex.c.Count;
                                }
                            }
                            else throw new NotImplementedException();
                        }
                        else if (representation.SegmentBase != null)
                        {
                            // TODO:/OPTIMIZE: request at the same time as initialization header
                            var segmentBase = representation.SegmentBase;
                            var indexRange = segmentBase.IndexRange.Split('-').Select(r => long.Parse(r)).ToArray();
                            var baseUrl = representation.BaseURL.First().Value;
                            var segmentIndexUri = new Uri(rootUri, baseUrl);
                            var segmentIndexRange = WebRequestor.Range.FromString(segmentBase.IndexRange);
                            var segmentIndexBoxes = await GetBoxesAsync(segmentIndexUri, segmentIndexRange);
                            var sidx = segmentIndexBoxes.OfType<SegmentIndexBox>().First();

                            // remove the track if the sidx durations don't match the chunk durations.
                            //if (streamIndex.c.Any() && sidx.Subsegments.First().Duration != streamIndex.c.First().d)
                            //{
                            //    streamIndex.QualityLevel.Remove(track);
                            //    break;
                            //}

                            track.Bitrate = CalculateBitrate(sidx);

                            if (!streamIndex.c.Any())
                            {
                                streamIndex.c.AddRange(CreateChunks(sidx));
                                if (!manifest.IsLive) streamIndex.Chunks = (uint)streamIndex.c.Count;
                            }
                            foreach (var kvp in GetChunkLookups(streamIndex, track, segmentIndexUri, sidx))
                            {
                                result.ChunkLookup.Add(kvp.Key, kvp.Value);
                            }
                        }
                        else throw new NotImplementedException();

                        representationIndex++;
                    }
                    
                    manifest.StreamIndex.Add(streamIndex);
                }
            }

            return result;
        }

        private static async Task<IList<Box>> GetBoxesAsync(Uri uri, WebRequestor.Range range)
        {
            using (var stream = await WebRequestor.GetStreamRangeAsync(uri, range))
            {
                using (var reader = new BoxBinaryReader(stream))
                {
                    return reader.GetAllBoxes();
                }
            }
        }

        private static IEnumerable<SmoothStreamingMediaStreamIndexC> CreateChunks(SegmentIndexBox sidx)
        {
            int i = 0;
            foreach (var subsegment in sidx.Subsegments)
            {
                var c = new SmoothStreamingMediaStreamIndexC();
                if (i == 0) c.t = sidx.EarliestPresentationTime;
                c.n = i;
                c.d = subsegment.Duration;
                //c.d = ConvertTimeToTimescale(track.Duration, mvhd.TimeScale, track.TimeScale) - entriesDuration;
                yield return c;
            }
        }

        private static IEnumerable<SmoothStreamingMediaStreamIndexC> CreateChunks(SegmentTimeline segmentTimeline)
        {
            int i = 0;

            foreach (var s in segmentTimeline.S)
            {
                for (int repeatIndex = 0; repeatIndex <= s.R; repeatIndex++)
                {
                    var c = new SmoothStreamingMediaStreamIndexC();
                    c.n = i;
                    c.t = s.T;
                    c.d = s.D;
                    //c.d = ConvertTimeToTimescale(track.Duration, mvhd.TimeScale, track.TimeScale) - entriesDuration;

                    yield return c;
                    i++;

                }
            }
        }

        private static IEnumerable<KeyValuePair<string, ChunkLocation>> GetChunkLookups(SmoothStreamingMediaStreamIndex streamIndex, SmoothStreamingMediaStreamIndexQualityLevel track, Uri chunkUri, SegmentIndexBox sidx)
        {
            int i = 0;
            ulong byteOffset = sidx.FirstOffset;
            ulong timeOffset = 0;
            foreach (var c in streamIndex.c)
            {
                var subsegment = sidx.Subsegments[i];
                // create a lookup table for each chunk
                var key = "/" + streamIndex.Url
                    .Replace("{bitrate}", track.Bitrate.ToString())
                    .Replace("{start time}", timeOffset.ToString());

                var location = new ChunkLocation()
                {
                    Uri = chunkUri,
                    From = byteOffset,
                    To = byteOffset += subsegment.ReferencedSize
                };
                yield return new KeyValuePair<string, ChunkLocation>(key, location);

                timeOffset += subsegment.Duration;
                i++;
            }
        }

        private static uint CalculateBitrate(SegmentIndexBox sidx)
        {
            var trackSize = sidx.Subsegments.Sum(subsegment => (long)subsegment.ReferencedSize);   // Track size in bits
            var trackDuration = sidx.Subsegments.Sum(subsegment => (long)subsegment.Duration);   // Track duration
            var bitrate = ((double)(trackSize * 8) / (double)trackDuration) * (double)sidx.Timescale;
            return (uint)bitrate;
        }
    }
}
