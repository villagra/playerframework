using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Media.ISO.Boxes;

namespace Microsoft.AdaptiveStreaming.Dash.Smooth
{
    /// <summary>
    /// Defines the information needed to generate a smooth streaming manifest for a track
    /// </summary>
    internal class ManifestTrack
    {
        /// <summary>
        /// Gets the track id.
        /// </summary>      
        public int Id { get; private set; }
        /// <summary>
        /// Gets the track type.
        /// </summary>
        public ManifestTrackType Type { get; private set; }
        /// <summary>
        /// Gets the list of track fragments.
        /// </summary>
        public TrackFragmentRandomAccessFullBox Fragments { get; private set; }
        /// <summary>
        /// Gets the track four codec code.
        /// </summary>
        public string FourCodecCode { get; private set; }
        /// <summary>
        /// Gets the track duration.
        /// </summary>
        public ulong Duration { get; private set; }
        /// <summary>
        /// Gets the track codec private data.
        /// </summary>
        public string CodecPrivateData { get; private set; }
        /// <summary>
        /// Gets the track bitrate.
        /// </summary>
        public uint Bitrate { get; private set; }

        /// <summary>
        /// Gets the video track height.
        /// </summary>
        public uint Height { get; private set; }
        /// <summary>
        /// Gets the video track width.
        /// </summary>
        public uint Width { get; private set; }
        /// <summary>
        /// Gets the video track display height.
        /// </summary>
        public uint DisplayHeight { get; private set; }
        /// <summary>
        /// Gets the video track display width.
        /// </summary>
        public uint DisplayWidth { get; private set; }

        /// <summary>
        /// Gets the audio track sample rate.
        /// </summary>
        public ushort SampleRate { get; private set; }
        /// <summary>
        /// Gets the audio track size of the sample.
        /// </summary>        
        public ushort SampleSize { get; private set; }
        /// <summary>
        /// Gets the audio track channel count.
        /// </summary>
        public ushort ChannelCount { get; private set; }
        /// <summary>
        /// Gets the audio track size of the packet.
        /// </summary>       
        public ushort PacketSize { get; private set; }
        /// <summary>
        /// Gets the timescale for the media in this track.
        /// </summary>
        public uint TimeScale { get; private set; }
        /// <summary>
        /// Gets the audio track tag.
        /// </summary>
        public ushort AudioTag { get; private set; }

        string language;
        /// <summary>
        /// Gets the language code for the track.
        /// </summary>
        public string Language
        {
            get { return language; }
            set
            {
                if (value != "und") language = value;
            }
        }

        public bool IsSupported { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestTrack"/> class.
        /// </summary>
        /// <param name="type">The track type.</param>
        /// <param name="tkhd">The <see cref="TrackHeaderFullBox"/> with track general information.</param>
        /// <param name="tfra">The <see cref="TrackFragmentRandomAccessFullBox"/> with list of fragments corresponding to the track.</param>
        /// <param name="stsd">The <see cref="SampleDescriptionFullBox"/> with track codec information.</param>
        public ManifestTrack(string type, TrackHeaderFullBox tkhd, MediaHeaderFullBox mdhd, TrackFragmentRandomAccessFullBox tfra, SampleDescriptionFullBox stsd)
        {
            this.IsSupported = false;
            this.Type = (ManifestTrackType)Enum.Parse(typeof(ManifestTrackType), type, true);
            this.Id = (int)tkhd.TrackId;
            this.Fragments = tfra;
            this.Duration = tkhd.Duration;
            this.Height = tkhd.Height >> 16;
            this.Width = tkhd.Width >> 16;
            this.TimeScale = mdhd.Timescale;
            this.Language = mdhd.Language;

            switch (this.Type)
            {
                case ManifestTrackType.Video:
                    BuildVideoTrack(stsd, tfra);
                    break;
                case ManifestTrackType.Audio:
                    BuildAudioTrack(stsd);
                    break;
                case ManifestTrackType.Text:
                    BuildSubtitleTrack(stsd);
                    break;
            }
        }

        private void BuildAudioTrack(SampleDescriptionFullBox stsd)
        {
            var soun = stsd.InnerBoxes.SingleOrDefault(b => b.Type == BoxType.Soun) as AudioSampleEntryBox;
            var enca = stsd.InnerBoxes.SingleOrDefault(b => b.Type == BoxType.Enca) as ProtectedSampleEntryBox;

            if (soun == null && enca != null)
            {
                soun = enca.OriginalSampleEntryData as AudioSampleEntryBox;
            }

            if (soun != null && soun.AudioCodecData != null && soun.AudioCodecData.CodecPrivateData != null)
            {
                this.ChannelCount = soun.ChannelCount;
                this.SampleRate = (ushort)(soun.SampleRate >> 16);
                this.SampleSize = soun.SampleSize;
                this.Bitrate = (uint)(this.SampleRate * this.ChannelCount);
                this.CodecPrivateData = soun.AudioCodecData.CodecPrivateData;
                this.PacketSize = soun.AudioCodecData.PacketSize;
                this.AudioTag = soun.AudioCodecData.AudioTag;
                this.FourCodecCode = soun.AudioCodecData.FourCodecCode;
                this.IsSupported = true;
            }
        }

        private void BuildVideoTrack(SampleDescriptionFullBox stsd, TrackFragmentRandomAccessFullBox tfra)
        {
            var vide = stsd.InnerBoxes.SingleOrDefault(b => b.Type == BoxType.Vide) as VisualSampleEntryBox;
            var encv = stsd.InnerBoxes.SingleOrDefault(b => b.Type == BoxType.Encv) as ProtectedSampleEntryBox;

            if (vide == null && encv != null)
            {
                vide = encv.OriginalSampleEntryData as VisualSampleEntryBox;
            }

            if (vide != null)
            {
                if (tfra != null)
                {
                    this.Bitrate = CalculateBitrate(tfra);
                }
                this.DisplayWidth = vide.VideoCodecData.DisplayWidth;
                this.DisplayHeight = vide.VideoCodecData.DisplayHeight;
                this.FourCodecCode = vide.VideoCodecData.FourCodecCode;
                this.CodecPrivateData = vide.VideoCodecData.CodecPrivateData;
                this.IsSupported = true;
            }
        }

        private uint CalculateBitrate(TrackFragmentRandomAccessFullBox tfra)
        {
            var trackSize = tfra.TrackFragmentRandomAccessEntries.Sum(entry => (long)entry.SampleSize);   // Track size in bits
            var bitrate = ((double)(trackSize * 8) / (double)this.Duration) * (double)this.TimeScale;
            return (uint)bitrate;
        }

        private void BuildSubtitleTrack(SampleDescriptionFullBox stsd)
        {
            var subt = stsd.InnerBoxes.SingleOrDefault(box => box.Type == BoxType.Subt) as SubtitleSampleEntryBox;

            if (subt != null)
            {
                this.IsSupported = true;
            }
        }

        #region Track Manifest generation

        public static List<ManifestTrack> InitializeTrackRegistry(IEnumerable<Box> boxes)
        {
            List<ManifestTrack> trackManifests = new List<ManifestTrack>();
            var moov = boxes.SingleOrDefault(b => b.Type == BoxType.Moov);
            var mfra = boxes.SingleOrDefault(b => b.Type == BoxType.Mfra);

            foreach (var trak in moov.InnerBoxes.Where(b => b.Type == BoxType.Trak))
            {
                var tkhd = trak.InnerBoxes.SingleOrDefault(b => b.Type == BoxType.Tkhd) as TrackHeaderFullBox;
                var mdia = trak.InnerBoxes.SingleOrDefault(b => b.Type == BoxType.Mdia);
                var mdhd = mdia.InnerBoxes.SingleOrDefault(b => b.Type == BoxType.Mdhd) as MediaHeaderFullBox;

                var tfra = mfra == null ? null : mfra.InnerBoxes.Where(b => b.Type == BoxType.Tfra).Cast<TrackFragmentRandomAccessFullBox>()
                               .SingleOrDefault(b => b.TrackId == tkhd.TrackId);

                var stsd = mdia.InnerBoxes.SingleOrDefault(b => b.Type == BoxType.Minf)
                               .InnerBoxes.SingleOrDefault(b => b.Type == BoxType.Stbl)
                               .InnerBoxes.SingleOrDefault(b => b.Type == BoxType.Stsd) as SampleDescriptionFullBox;

                var handlerType = GetTrackHandlerType(
                    mdia.InnerBoxes.SingleOrDefault(box => box.Type == BoxType.Hdlr) as HandlerReferenceFullBox);

                var manTrack = new ManifestTrack(handlerType, tkhd, mdhd, tfra, stsd);

                if (manTrack.IsSupported)
                {
                    trackManifests.Add(manTrack);
                }
            }

            return trackManifests;
        }

        public static string GetTrackHandlerType(HandlerReferenceFullBox hdlr)
        {
            var handlerType = string.Empty;

            switch (hdlr.HandlerType)
            {
                case "soun":
                    handlerType = "Audio";
                    break;
                case "vide":
                    handlerType = "Video";
                    break;
                default:
                    handlerType = "Text";
                    break;
            }

            return handlerType;
        }

        #endregion
    }
}
