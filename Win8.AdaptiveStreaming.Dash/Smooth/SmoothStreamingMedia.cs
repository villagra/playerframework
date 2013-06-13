using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.AdaptiveStreaming.Dash.Smooth
{
    /// <summary>
    /// Specifies metadata for this Smooth Streaming media element/presentation
    /// </summary>
    internal partial class SmoothStreamingMedia
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SmoothStreamingMedia"/> class.
        /// </summary>
        public SmoothStreamingMedia()
        {
            this.MajorVersion = 2;
            this.MinorVersion = 1;
            this.StreamIndex = new List<SmoothStreamingMediaStreamIndex>();
        }

        /// <summary>
        /// Specifies the protection information.
        /// </summary>
        [XmlElement]
        public SmoothStreamingMediaProtection Protection { get; set; }

        /// <summary>
        /// Gets or sets the list of track streams.
        /// </summary>
        [XmlElement]
        public List<SmoothStreamingMediaStreamIndex> StreamIndex { get; set; }

        /// <summary>
        /// Specifies the Client Manifest Major Version. MUST be 2 for this release.
        /// </summary>
        [XmlAttribute]
        public byte MajorVersion { get; set; }

        /// <summary>
        /// Specifies the Client Manifest Minor Version. MUST be 0 for this release. 
        /// </summary>
        [XmlAttribute]
        public byte MinorVersion { get; set; }

        /// <summary>
        /// Specifies overall presentation duration of the media in increments of the TimeScale attribute. 
        /// Duration SHOULD be set to 0 for live presentations whose approximate duration is not known in advance.
        /// </summary>
        [XmlAttribute]
        public ulong Duration { get; set; }

        /// <summary>
        /// Specifies the timescale for the entire presentation as a number of units that pass in one second.        
        /// </summary>
        [XmlAttribute]
        public uint? TimeScale { get; set; }

        /// <summary>
        /// Specifies that this manifest describes a live presentation that is still in progress.
        /// </summary>
        [XmlAttribute]
        public bool IsLive { get; set; }

        /// <summary>
        /// Specifies the number of fragments in a lookahead.
        /// </summary>
        [XmlAttribute]
        public uint? LookaheadCount { get; set; }

        /// <summary>
        /// Specifies the length of the trailing window for a 24/7 broadcast.
        /// </summary>
        [XmlAttribute]
        public ulong? DVRWindowLength { get; set; }
    }

    /// <summary>
    /// Specifies the metadata for one type of track (audio, video, or text).
    /// </summary>
    internal partial class SmoothStreamingMediaStreamIndex
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SmoothStreamingMediaStreamIndex"/> class.
        /// </summary>
        public SmoothStreamingMediaStreamIndex()
        {
            this.QualityLevel = new List<SmoothStreamingMediaStreamIndexQualityLevel>();
            this.c = new List<SmoothStreamingMediaStreamIndexC>();
        }

        /// <summary>
        /// Gets or sets the list of track quality levels.
        /// </summary>
        [XmlElement]
        public List<SmoothStreamingMediaStreamIndexQualityLevel> QualityLevel { get; set; }

        /// <summary>
        /// Gets or sets the list of track fragments.
        /// </summary>
        [XmlElement]
        public List<SmoothStreamingMediaStreamIndexC> c { get; set; }

        /// <summary>
        /// [Video][Audio] Specifies the track type, and MUST be “audio”, “video” or “text”.
        /// </summary>
        [XmlAttribute]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the subtype of the stream. e.g. H264, AACL
        /// </summary>
        [XmlAttribute]
        public string Subtype { get; set; }

        /// <summary>
        /// [Video][Audio] Specifies the number of fragments in the track.
        /// </summary>
        [XmlAttribute]
        public uint Chunks { get; set; }

        /// <summary>
        /// [Video][Audio] specifies number of distinct quality level and hardware profile choices available for this track.
        /// </summary>
        [XmlAttribute]
        public uint QualityLevels { get { return (uint)QualityLevel.Count; } }

        /// <summary>
        /// [Video] Maximum coded width, in pixels, for all quality level choices.
        /// </summary>
        [XmlAttribute]
        public uint MaxWidth { get; set; }

        /// <summary>
        /// [Video] Maximum coded height, in pixels, for all quality level choices.
        /// </summary>
        [XmlAttribute]
        public uint MaxHeight { get; set; }

        /// <summary>
        /// [Video] The recommended display width, in pixels.
        /// </summary>
        [XmlAttribute]
        public uint DisplayWidth { get; set; }

        /// <summary>
        /// [Video] The recommended display height, in pixels.
        /// </summary>
        [XmlAttribute]
        public uint DisplayHeight { get; set; }

        string language;
        /// <summary>
        /// [Video][Audio][Text] Optional value that indicates the language code for the track.
        /// </summary>
        [XmlAttribute]
        public string Language
        {
            get { return language; }
            set
            {
                if (value != "und") language = value;
            }
        }

        /// <summary>
        /// [Video][Audio][Text] Optional value that gives a readable name to the stream.
        /// </summary>
        public string Name
        {
            get
            {
                return (!string.IsNullOrEmpty(this.Language)) ?
                    string.Format("{0}_{1}", this.Type, this.Language) :
                    string.Format("{0}", this.Type);
            }
        }

        /// <summary>
        /// The URL pattern for the stream index that includes the Language parameter
        /// </summary>
        private readonly string UrlPatternWithLanguage = @"QualityLevels({{bitrate}})/Fragments({0}={{start time}})/Language({1})";

        /// <summary>
        /// The URL pattern for the stream index that does not include a language parameter.
        /// </summary>
        private readonly string UrlPatternWithoutLanguage = @"QualityLevels({{bitrate}})/Fragments({0}={{start time}})";

        private string url;
        /// <summary>
        /// [Video][Audio] Specifies how to generate URLs for this track or group of tracks. Syntax and semantics for the Url attribute are specified in detail below. 
        /// </summary>
        [XmlAttribute]
        public string Url
        {
            get
            {
                if (string.IsNullOrEmpty(this.url))
                {
                    if (!string.IsNullOrEmpty(this.Language))
                    {
                        return string.Format(this.UrlPatternWithLanguage, this.Type, this.Language);
                    }
                    else
                    {
                        return string.Format(this.UrlPatternWithoutLanguage, this.Type);
                    }
                }
                else return this.url;
            }
            set { this.url = value; }
        }

        /// <summary>
        /// [Audio] Gets or sets the default index.
        /// </summary>
        [XmlAttribute]
        public byte Index { get; set; }

        /// <summary>
        /// [Audio] Specifies the FourCC code for the audio codec used.
        /// </summary>
        [XmlAttribute]
        public string FourCC { get; set; }

        /// <summary>
        /// Specifies the timescale for the entire presentation as a number of units that pass in one second.        
        /// </summary>
        [XmlAttribute]
        public uint TimeScale { get; set; }
    }

    /// <summary>
    /// The QualityLevel element specifies metadata for the video or audio track at a given quality level. 
    /// The QualityLevel element MUST be present for video and audio tracks, even if there is only a single quality level available for the track type.
    /// </summary>
    internal partial class SmoothStreamingMediaStreamIndexQualityLevel
    {

        /// <summary>
        /// [Video] Specifies an index for this QualityLevel.
        /// </summary>        
        [XmlAttribute]
        public uint Index { get; set; }

        /// <summary>
        /// [Video][Audio] Tte bitrate (for only this track) for this quality level  in bit-per-second (bps).
        /// </summary>
        [XmlAttribute]
        public uint Bitrate { get; set; }

        /// <summary>
        /// [Video] Specifies the FourCC code for the video codec used.
        /// </summary>
        [XmlAttribute]
        public string FourCC { get; set; }

        /// <summary>
        /// [Video] The maximum coded width in pixels for this quality level. Defaults to the value in the parent StreamIndex when omitted.
        /// </summary>
        [XmlAttribute]
        public uint MaxWidth { get; set; }

        /// <summary>
        /// [Video] The maximum coded height in pixels for this quality level. Defaults to the value in the parent StreamIndex when omitted.
        /// </summary>
        [XmlAttribute]
        public uint MaxHeight { get; set; }

        /// <summary>
        /// [Video][Audio] Specifies the codec private data property of this video stream, base-16 encoded.
        /// </summary>
        [XmlAttribute]
        public string CodecPrivateData { get; set; }

        /// <summary>
        /// [Audio] Specifies the audio sampling rate in Hertz (Hz).
        /// </summary>
        [XmlAttribute]
        public ushort SamplingRate { get; set; }

        /// <summary>
        /// [Audio] Specifies the number of audio channels in the track (e.g. 2 for stereo).
        /// </summary>
        [XmlAttribute]
        public ushort Channels { get; set; }

        /// <summary>
        /// [Audio] Specifies the size of each audio sample.
        /// </summary>
        [XmlAttribute]
        public ushort BitsPerSample { get; set; }

        /// <summary>
        /// [Audio] Specifies the block alignment, in bytes.
        /// </summary>
        [XmlAttribute]
        public ushort PacketSize { get; set; }

        /// <summary>
        /// [Audio] Specifies the audio format type.
        /// </summary>
        [XmlAttribute]
        public ushort AudioTag { get; set; }
    }

    /// <summary>
    /// The c element specifies fragment-level metadata for the video or audio. There are as many c elements as 
    /// the Chunks attribute specified in the parent StreamIndex element.
    /// </summary>
    internal partial class SmoothStreamingMediaStreamIndexC
    {

        /// <summary>
        /// A zero-based index for the nth fragment in the media track (time-ordered)
        /// </summary>
        [XmlIgnore]
        public int n { get; set; }

        /// <summary>
        /// Duration of the fragment, in units of the TimeScale attribute of the parent StreamIndex element.
        /// </summary>
        [XmlAttribute]
        public ulong d { get; set; }

        /// <summary>
        /// Specifies the start time, in units of the TimeScale attribute of the parent StreamIndex element that the first fragment in the series starts relative to the beginning.
        /// </summary>
        [XmlAttribute]
        public ulong? t { get; set; }
    }

    /// <summary>
    /// The Protection element specifies that presentation uses tracks with a content protection scheme and provides information that 
    /// a client runtime can use to play back the content.
    /// </summary>
    internal partial class SmoothStreamingMediaProtection
    {
        /// <summary>
        /// Specifes protection information.
        /// </summary>
        [XmlElement]
        public SmoothStreamingMediaProtectionProtectionHeader ProtectionHeader { get; set; }
    }

    /// <summary>
    /// The <see cref="ProtectionHeader"/> element provides a content Protection System-specific header used by the client to enable playback. 
    /// The information in each ProtectionHeader element is targeted only to the Content Protection System identified by the SystemID attribute.
    /// </summary>
    internal partial class SmoothStreamingMediaProtectionProtectionHeader
    {
        /// <summary>
        /// Gets or sets the protection system ID.
        /// </summary>
        [XmlAttribute]
        public string SystemID { get; set; }

        /// <summary>
        /// Specifies protection information.
        /// </summary>
        [XmlText]
        public string Value { get; set; }
    }

}
