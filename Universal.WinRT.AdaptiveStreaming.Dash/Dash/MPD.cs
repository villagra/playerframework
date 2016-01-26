using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.Media.AdaptiveStreaming.Dash
{
    internal class MPD
    {
        private IList<ProgramInformation> _ProgramInformation;
        public IList<ProgramInformation> ProgramInformation
        {
            get { if (_ProgramInformation == null) _ProgramInformation = new List<ProgramInformation>(); return _ProgramInformation; }
            private set { _ProgramInformation = value; }
        }

        private IList<BaseURL> _BaseURL;
        public IList<BaseURL> BaseURL
        {
            get { if (_BaseURL == null) _BaseURL = new List<BaseURL>(); return _BaseURL; }
            private set { _BaseURL = value; }
        }

        private IList<string> _Location;
        public IList<string> Location
        {
            get { if (_Location == null) _Location = new List<string>(); return _Location; }
            private set { _Location = value; }
        }

        private IList<Period> _Period;
        public IList<Period> Period
        {
            get { if (_Period == null) _Period = new List<Period>(); return _Period; }
            private set { _Period = value; }
        }

        private IList<Metrics> _Metrics;
        public IList<Metrics> Metrics
        {
            get { if (_Metrics == null) _Metrics = new List<Metrics>(); return _Metrics; }
            private set { _Metrics = value; }
        }

        private string _Id = string.Empty;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private string _Profiles = string.Empty;
        public string Profiles
        {
            get { return _Profiles; }
            set { _Profiles = value; }
        }

        private Presentation _Type;
        public Presentation Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public DateTimeOffset? AvailabilityStartTime { get; set; }

        public DateTimeOffset? AvailabilityEndTime { get; set; }

        public TimeSpan? MediaPresentationDuration { get; set; }

        public TimeSpan? MinimumUpdatePeriod { get; set; }

        public TimeSpan? MinBufferTime { get; set; }

        public TimeSpan? TimeShiftBufferDepth { get; set; }

        public TimeSpan? SuggestedPresentationDelay { get; set; }

        public TimeSpan? MaxSegmentDuration { get; set; }

        public TimeSpan? MaxSubsegmentDuration { get; set; }

        private IList<XAttribute> _AnyAttr;
        public IList<XAttribute> AnyAttr
        {
            get { if (_AnyAttr == null) _AnyAttr = new List<XAttribute>(); return _AnyAttr; }
            private set { _AnyAttr = value; }
        }

        private IList<XElement> _Any;
        public IList<XElement> Any
        {
            get { if (_Any == null) _Any = new List<XElement>(); return _Any; }
            private set { _Any = value; }
        }
    }

    internal class ProgramInformation
    {
        private string _Title = string.Empty;
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        private string _Source = string.Empty;
        public string Source
        {
            get { return _Source; }
            set { _Source = value; }
        }

        private string _Copyright = string.Empty;
        public string Copyright
        {
            get { return _Copyright; }
            set { _Copyright = value; }
        }

        private string _Lang = string.Empty;
        public string Lang
        {
            get { return _Lang; }
            set { _Lang = value; }
        }

        private string _MoreInformationURL = string.Empty;
        public string MoreInformationURL
        {
            get { return _MoreInformationURL; }
            set { _MoreInformationURL = value; }
        }

        private IList<XAttribute> _AnyAttr;
        public IList<XAttribute> AnyAttr
        {
            get { if (_AnyAttr == null) _AnyAttr = new List<XAttribute>(); return _AnyAttr; }
            private set { _AnyAttr = value; }
        }

        private IList<XElement> _Any;
        public IList<XElement> Any
        {
            get { if (_Any == null) _Any = new List<XElement>(); return _Any; }
            private set { _Any = value; }
        }
    }

    internal class Range
    {
        public TimeSpan? StartTime { get; set; }

        public TimeSpan? Duration { get; set; }
    }

    internal class Metrics
    {
        private IList<Descriptor> _Reporting;
        public IList<Descriptor> Reporting
        {
            get { if (_Reporting == null) _Reporting = new List<Descriptor>(); return _Reporting; }
            private set { _Reporting = value; }
        }

        private IList<Range> _Range;
        public IList<Range> Range
        {
            get { if (_Range == null) _Range = new List<Range>(); return _Range; }
            private set { _Range = value; }
        }

        private string _MetricsValue = string.Empty;
        public string MetricsValue
        {
            get { return _MetricsValue; }
            set { _MetricsValue = value; }
        }

        private IList<XAttribute> _AnyAttr;
        public IList<XAttribute> AnyAttr
        {
            get { if (_AnyAttr == null) _AnyAttr = new List<XAttribute>(); return _AnyAttr; }
            private set { _AnyAttr = value; }
        }

        private IList<XElement> _Any;
        public IList<XElement> Any
        {
            get { if (_Any == null) _Any = new List<XElement>(); return _Any; }
            private set { _Any = value; }
        }
    }

    internal class Descriptor
    {
        private string _SchemeIdUri = string.Empty;
        public string SchemeIdUri
        {
            get { return _SchemeIdUri; }
            set { _SchemeIdUri = value; }
        }

        private string _Value = string.Empty;
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        private IList<XAttribute> _AnyAttr;
        public IList<XAttribute> AnyAttr
        {
            get { if (_AnyAttr == null) _AnyAttr = new List<XAttribute>(); return _AnyAttr; }
            private set { _AnyAttr = value; }
        }

        private IList<XElement> _Any;
        public IList<XElement> Any
        {
            get { if (_Any == null) _Any = new List<XElement>(); return _Any; }
            private set { _Any = value; }
        }
    }

    internal class Subset
    {
        private IList<uint> _Contains;
        public IList<uint> Contains
        {
            get { if (_Contains == null) _Contains = new List<uint>(); return _Contains; }
            private set { _Contains = value; }
        }

        private IList<XAttribute> _AnyAttr;
        public IList<XAttribute> AnyAttr
        {
            get { if (_AnyAttr == null) _AnyAttr = new List<XAttribute>(); return _AnyAttr; }
            private set { _AnyAttr = value; }
        }
    }

    internal class ContentComponent
    {
        private IList<Descriptor> _Accessibility;
        public IList<Descriptor> Accessibility
        {
            get { if (_Accessibility == null) _Accessibility = new List<Descriptor>(); return _Accessibility; }
            private set { _Accessibility = value; }
        }

        private IList<Descriptor> _Role;
        public IList<Descriptor> Role
        {
            get { if (_Role == null) _Role = new List<Descriptor>(); return _Role; }
            private set { _Role = value; }
        }

        private IList<Descriptor> _Rating;
        public IList<Descriptor> Rating
        {
            get { if (_Rating == null) _Rating = new List<Descriptor>(); return _Rating; }
            private set { _Rating = value; }
        }

        private IList<Descriptor> _Viewpoint;
        public IList<Descriptor> Viewpoint
        {
            get { if (_Viewpoint == null) _Viewpoint = new List<Descriptor>(); return _Viewpoint; }
            private set { _Viewpoint = value; }
        }

        public uint? Id { get; set; }

        private string _Lang = string.Empty;
        public string Lang
        {
            get { return _Lang; }
            set { _Lang = value; }
        }

        private string _ContentType = string.Empty;
        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }

        private string _Par = string.Empty;
        public string Par
        {
            get { return _Par; }
            set { _Par = value; }
        }

        private IList<XAttribute> _AnyAttr;
        public IList<XAttribute> AnyAttr
        {
            get { if (_AnyAttr == null) _AnyAttr = new List<XAttribute>(); return _AnyAttr; }
            private set { _AnyAttr = value; }
        }

        private IList<XElement> _Any;
        public IList<XElement> Any
        {
            get { if (_Any == null) _Any = new List<XElement>(); return _Any; }
            private set { _Any = value; }
        }
    }

    internal class RepresentationBase
    {
        private IList<Descriptor> _FramePacking;
        public IList<Descriptor> FramePacking
        {
            get { if (_FramePacking == null) _FramePacking = new List<Descriptor>(); return _FramePacking; }
            private set { _FramePacking = value; }
        }

        private IList<Descriptor> _AudioChannelConfiguration;
        public IList<Descriptor> AudioChannelConfiguration
        {
            get { if (_AudioChannelConfiguration == null) _AudioChannelConfiguration = new List<Descriptor>(); return _AudioChannelConfiguration; }
            private set { _AudioChannelConfiguration = value; }
        }

        private IList<Descriptor> _ContentProtection;
        public IList<Descriptor> ContentProtection
        {
            get { if (_ContentProtection == null) _ContentProtection = new List<Descriptor>(); return _ContentProtection; }
            private set { _ContentProtection = value; }
        }

        private string _Profiles = string.Empty;
        public string Profiles
        {
            get { return _Profiles; }
            set { _Profiles = value; }
        }

        public uint? Width { get; set; }

        public uint? Height { get; set; }

        private string _Sar = string.Empty;
        public string Sar
        {
            get { return _Sar; }
            set { _Sar = value; }
        }

        private string _FrameRate = string.Empty;
        public string FrameRate
        {
            get { return _FrameRate; }
            set { _FrameRate = value; }
        }

        private string _AudioSamplingRate = string.Empty;
        public string AudioSamplingRate
        {
            get { return _AudioSamplingRate; }
            set { _AudioSamplingRate = value; }
        }

        private string _MimeType = string.Empty;
        public string MimeType
        {
            get { return _MimeType; }
            set { _MimeType = value; }
        }

        private string _SegmentProfiles = string.Empty;
        public string SegmentProfiles
        {
            get { return _SegmentProfiles; }
            set { _SegmentProfiles = value; }
        }

        private string _Codecs = string.Empty;
        public string Codecs
        {
            get { return _Codecs; }
            set { _Codecs = value; }
        }

        public double? MaximumSAPPeriod { get; set; }

        public uint? StartWithSAP { get; set; }

        public double? MaxPlayoutRate { get; set; }

        public bool? CodingDependency { get; set; }

        public VideoScan? ScanType { get; set; }

        private IList<XAttribute> _AnyAttr;
        public IList<XAttribute> AnyAttr
        {
            get { if (_AnyAttr == null) _AnyAttr = new List<XAttribute>(); return _AnyAttr; }
            private set { _AnyAttr = value; }
        }

        private IList<XElement> _Any;
        public IList<XElement> Any
        {
            get { if (_Any == null) _Any = new List<XElement>(); return _Any; }
            private set { _Any = value; }
        }
    }

    public enum VideoScan
    {
        Progressive,
        Interlaced,
        Unknown,
    }

    internal class SubRepresentation : RepresentationBase
    {
        public uint? Level { get; set; }

        private IList<uint> _DependencyLevel;
        public IList<uint> DependencyLevel
        {
            get { if (_DependencyLevel == null) _DependencyLevel = new List<uint>(); return _DependencyLevel; }
            private set { _DependencyLevel = value; }
        }

        public uint? Bandwidth { get; set; }

        private IList<string> _ContentComponent;
        public IList<string> ContentComponent
        {
            get { if (_ContentComponent == null) _ContentComponent = new List<string>(); return _ContentComponent; }
            private set { _ContentComponent = value; }
        }
    }

    internal class Representation : RepresentationBase
    {
        private IList<BaseURL> _BaseURL;
        public IList<BaseURL> BaseURL
        {
            get { if (_BaseURL == null) _BaseURL = new List<BaseURL>(); return _BaseURL; }
            private set { _BaseURL = value; }
        }

        private IList<SubRepresentation> _SubRepresentation;
        public IList<SubRepresentation> SubRepresentation
        {
            get { if (_SubRepresentation == null) _SubRepresentation = new List<SubRepresentation>(); return _SubRepresentation; }
            private set { _SubRepresentation = value; }
        }

        private SegmentBase _SegmentBase;
        public SegmentBase SegmentBase
        {
            get { return _SegmentBase; }
            set { _SegmentBase = value; }
        }

        private SegmentList _SegmentList;
        public SegmentList SegmentList
        {
            get { return _SegmentList; }
            set { _SegmentList = value; }
        }

        private SegmentTemplate _SegmentTemplate;
        public SegmentTemplate SegmentTemplate
        {
            get { return _SegmentTemplate; }
            set { _SegmentTemplate = value; }
        }

        private string _Id = string.Empty;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private uint _Bandwidth;
        public uint Bandwidth
        {
            get { return _Bandwidth; }
            set { _Bandwidth = value; }
        }

        public uint? QualityRanking { get; set; }

        private IList<string> _DependencyId;
        public IList<string> DependencyId
        {
            get { if (_DependencyId == null) _DependencyId = new List<string>(); return _DependencyId; }
            private set { _DependencyId = value; }
        }

        private IList<string> _MediaStreamStructureId;
        public IList<string> MediaStreamStructureId
        {
            get { if (_MediaStreamStructureId == null) _MediaStreamStructureId = new List<string>(); return _MediaStreamStructureId; }
            private set { _MediaStreamStructureId = value; }
        }
    }

    internal class BaseURL
    {
        private string _ServiceLocation = string.Empty;
        public string ServiceLocation
        {
            get { return _ServiceLocation; }
            set { _ServiceLocation = value; }
        }

        private string _ByteRange = string.Empty;
        public string ByteRange
        {
            get { return _ByteRange; }
            set { _ByteRange = value; }
        }

        private IList<XAttribute> _AnyAttr;
        public IList<XAttribute> AnyAttr
        {
            get { if (_AnyAttr == null) _AnyAttr = new List<XAttribute>(); return _AnyAttr; }
            private set { _AnyAttr = value; }
        }

        public string Value { get; set; }
    }

    internal class SegmentBase
    {
        public SegmentBase()
        {
            this.AnyAttr = new List<XAttribute>();
            this.Any = new List<XElement>();
            this.RepresentationIndex = new URL();
            this.Initialization = new URL();
        }

        private URL _Initialization;
        public URL Initialization
        {
            get { return _Initialization; }
            set { _Initialization = value; }
        }

        private URL _RepresentationIndex;
        public URL RepresentationIndex
        {
            get { return _RepresentationIndex; }
            set { _RepresentationIndex = value; }
        }

        public uint? Timescale { get; set; }

        public ulong? PresentationTimeOffset { get; set; }

        private string _IndexRange;
        public string IndexRange
        {
            get { return _IndexRange; }
            set { _IndexRange = value; }
        }

        public bool? IndexRangeExact { get; set; }

        private IList<XAttribute> _AnyAttr;
        public IList<XAttribute> AnyAttr
        {
            get { if (_AnyAttr == null) _AnyAttr = new List<XAttribute>(); return _AnyAttr; }
            private set { _AnyAttr = value; }
        }

        private IList<XElement> _Any;
        public IList<XElement> Any
        {
            get { if (_Any == null) _Any = new List<XElement>(); return _Any; }
            private set { _Any = value; }
        }
    }

    internal class URL
    {
        private string _SourceURL;
        public string SourceURL
        {
            get { return _SourceURL; }
            set { _SourceURL = value; }
        }

        private string _Range;
        public string Range
        {
            get { return _Range; }
            set { _Range = value; }
        }

        private IList<XAttribute> _AnyAttr;
        public IList<XAttribute> AnyAttr
        {
            get { if (_AnyAttr == null) _AnyAttr = new List<XAttribute>(); return _AnyAttr; }
            private set { _AnyAttr = value; }
        }

        private IList<XElement> _Any;
        public IList<XElement> Any
        {
            get { if (_Any == null) _Any = new List<XElement>(); return _Any; }
            private set { _Any = value; }
        }
    }

    internal class MultipleSegmentBase : SegmentBase
    {
        private SegmentTimeline _SegmentTimeline;
        public SegmentTimeline SegmentTimeline
        {
            get { return _SegmentTimeline; }
            set { _SegmentTimeline = value; }
        }

        private URL _BitstreamSwitching;
        public URL BitstreamSwitching
        {
            get { return _BitstreamSwitching; }
            set { _BitstreamSwitching = value; }
        }

        public uint? Duration { get; set; }

        public uint? StartNumber { get; set; }
    }

    internal class SegmentTimeline
    {
        private IList<SegmentTimelineS> _S;
        public IList<SegmentTimelineS> S
        {
            get { if (_S == null) _S = new List<SegmentTimelineS>(); return _S; }
            private set { _S = value; }
        }

        private IList<XElement> _Any;
        public IList<XElement> Any
        {
            get { if (_Any == null) _Any = new List<XElement>(); return _Any; }
            private set { _Any = value; }
        }

        private IList<XAttribute> _AnyAttr;
        public IList<XAttribute> AnyAttr
        {
            get { if (_AnyAttr == null) _AnyAttr = new List<XAttribute>(); return _AnyAttr; }
            private set { _AnyAttr = value; }
        }
    }

    internal class SegmentTimelineS
    {
        public SegmentTimelineS()
        {
            this.AnyAttr = new List<XAttribute>();
            this.R = ((uint)(0));
        }

        public ulong? T { get; set; }

        private ulong _D;
        public ulong D
        {
            get { return _D; }
            set { _D = value; }
        }

        private uint _R;
        public uint R
        {
            get { return _R; }
            set { _R = value; }
        }

        private IList<XAttribute> _AnyAttr;
        public IList<XAttribute> AnyAttr
        {
            get { if (_AnyAttr == null) _AnyAttr = new List<XAttribute>(); return _AnyAttr; }
            private set { _AnyAttr = value; }
        }
    }

    internal class SegmentTemplate : MultipleSegmentBase
    {
        private string _Media;
        public string Media
        {
            get { return _Media; }
            set { _Media = value; }
        }

        private string _Index;
        public string Index
        {
            get { return _Index; }
            set { _Index = value; }
        }

        private string _InitializationValue;
        public string InitializationValue
        {
            get { return _InitializationValue; }
            set { _InitializationValue = value; }
        }

        private string _BitstreamSwitchingValue;
        public string BitstreamSwitchingValue
        {
            get { return _BitstreamSwitchingValue; }
            set { _BitstreamSwitchingValue = value; }
        }
    }

    internal class SegmentList : MultipleSegmentBase
    {
        private IList<SegmentURL> _SegmentURL;
        public IList<SegmentURL> SegmentURL
        {
            get { if (_SegmentURL == null) _SegmentURL = new List<SegmentURL>(); return _SegmentURL; }
            private set { _SegmentURL = value; }
        }

        private string _Href;
        public string Href
        {
            get { return _Href; }
            set { _Href = value; }
        }

        private Actuate _Actuate;
        public Actuate Actuate
        {
            get { return _Actuate; }
            set { _Actuate = value; }
        }
    }

    internal class SegmentURL
    {
        private string _Media;
        public string Media
        {
            get { return _Media; }
            set { _Media = value; }
        }

        private string _MediaRange;
        public string MediaRange
        {
            get { return _MediaRange; }
            set { _MediaRange = value; }
        }

        private string _Index;
        public string Index
        {
            get { return _Index; }
            set { _Index = value; }
        }

        private string _IndexRange;
        public string IndexRange
        {
            get { return _IndexRange; }
            set { _IndexRange = value; }
        }

        private IList<XAttribute> _AnyAttr;
        public IList<XAttribute> AnyAttr
        {
            get { if (_AnyAttr == null) _AnyAttr = new List<XAttribute>(); return _AnyAttr; }
            private set { _AnyAttr = value; }
        }

        private IList<XElement> _Any;
        public IList<XElement> Any
        {
            get { if (_Any == null) _Any = new List<XElement>(); return _Any; }
            private set { _Any = value; }
        }
    }

    public enum Actuate
    {
        OnLoad,
        OnRequest,
        Other,
        None
    }

    internal class AdaptationSet : RepresentationBase
    {
        private IList<Descriptor> _Accessibility;
        public IList<Descriptor> Accessibility
        {
            get { if (_Accessibility == null) _Accessibility = new List<Descriptor>(); return _Accessibility; }
            private set { _Accessibility = value; }
        }

        private IList<Descriptor> _Role;
        public IList<Descriptor> Role
        {
            get { if (_Role == null) _Role = new List<Descriptor>(); return _Role; }
            private set { _Role = value; }
        }

        private IList<Descriptor> _Rating;
        public IList<Descriptor> Rating
        {
            get { if (_Rating == null) _Rating = new List<Descriptor>(); return _Rating; }
            private set { _Rating = value; }
        }

        private IList<Descriptor> _Viewpoint;
        public IList<Descriptor> Viewpoint
        {
            get { if (_Viewpoint == null) _Viewpoint = new List<Descriptor>(); return _Viewpoint; }
            private set { _Viewpoint = value; }
        }

        private IList<ContentComponent> _ContentComponent;
        public IList<ContentComponent> ContentComponent
        {
            get { if (_ContentComponent == null) _ContentComponent = new List<ContentComponent>(); return _ContentComponent; }
            private set { _ContentComponent = value; }
        }

        private IList<BaseURL> _BaseURL;
        public IList<BaseURL> BaseURL
        {
            get { if (_BaseURL == null) _BaseURL = new List<BaseURL>(); return _BaseURL; }
            private set { _BaseURL = value; }
        }

        private SegmentBase _SegmentBase;
        public SegmentBase SegmentBase
        {
            get { return _SegmentBase; }
            set { _SegmentBase = value; }
        }

        private SegmentList _SegmentList;
        public SegmentList SegmentList
        {
            get { return _SegmentList; }
            set { _SegmentList = value; }
        }

        private SegmentTemplate _SegmentTemplate;
        public SegmentTemplate SegmentTemplate
        {
            get { return _SegmentTemplate; }
            set { _SegmentTemplate = value; }
        }

        private IList<Representation> _Representation;
        public IList<Representation> Representation
        {
            get { if (_Representation == null) _Representation = new List<Representation>(); return _Representation; }
            private set { _Representation = value; }
        }

        private string _Href;
        public string Href
        {
            get { return _Href; }
            set { _Href = value; }
        }

        private Actuate _Actuate;
        public Actuate Actuate
        {
            get { return _Actuate; }
            set { _Actuate = value; }
        }

        public uint? Id { get; set; }

        public uint? Group { get; set; }

        private string _Lang;
        public string Lang
        {
            get { return _Lang; }
            set { _Lang = value; }
        }

        private string _ContentType;
        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }

        private string _Par;
        public string Par
        {
            get { return _Par; }
            set { _Par = value; }
        }

        public uint? MinBandwidth { get; set; }

        public uint? MaxBandwidth { get; set; }

        public uint? MinWidth { get; set; }

        public uint? MaxWidth { get; set; }

        public uint? MinHeight { get; set; }

        public uint? MaxHeight { get; set; }

        private ConditionalUInt _MinFrameRate;
        public ConditionalUInt MinFrameRate
        {
            get { return _MinFrameRate; }
            set { _MinFrameRate = value; }
        }

        private ConditionalUInt _MaxFrameRate;
        public ConditionalUInt MaxFrameRate
        {
            get { return _MaxFrameRate; }
            set { _MaxFrameRate = value; }
        }

        private string _SegmentAlignment;
        public string SegmentAlignment
        {
            get { return _SegmentAlignment; }
            set { _SegmentAlignment = value; }
        }

        private string _SubsegmentAlignment;
        public string SubsegmentAlignment
        {
            get { return _SubsegmentAlignment; }
            set { _SubsegmentAlignment = value; }
        }

        private uint _SubsegmentStartsWithSAP;
        public uint SubsegmentStartsWithSAP
        {
            get { return _SubsegmentStartsWithSAP; }
            set { _SubsegmentStartsWithSAP = value; }
        }

        public bool? BitstreamSwitching { get; set; }
    }

    internal class Period
    {
        private IList<BaseURL> _BaseURL;
        public IList<BaseURL> BaseURL
        {
            get { if (_BaseURL == null) _BaseURL = new List<BaseURL>(); return _BaseURL; }
            private set { _BaseURL = value; }
        }

        private SegmentBase _SegmentBase;
        public SegmentBase SegmentBase
        {
            get { return _SegmentBase; }
            set { _SegmentBase = value; }
        }

        private SegmentList _SegmentList;
        public SegmentList SegmentList
        {
            get { return _SegmentList; }
            set { _SegmentList = value; }
        }

        private SegmentTemplate _SegmentTemplate;
        public SegmentTemplate SegmentTemplate
        {
            get { return _SegmentTemplate; }
            set { _SegmentTemplate = value; }
        }

        private IList<AdaptationSet> _AdaptationSet;
        public IList<AdaptationSet> AdaptationSet
        {
            get { if (_AdaptationSet == null) _AdaptationSet = new List<AdaptationSet>(); return _AdaptationSet; }
            private set { _AdaptationSet = value; }
        }

        private IList<Subset> _Subset;
        public IList<Subset> Subset
        {
            get { if (_Subset == null) _Subset = new List<Subset>(); return _Subset; }
            private set { _Subset = value; }
        }

        private string _Href;
        public string Href
        {
            get { return _Href; }
            set { _Href = value; }
        }

        private Actuate _Actuate;
        public Actuate Actuate
        {
            get { return _Actuate; }
            set { _Actuate = value; }
        }

        private string _Id;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public TimeSpan? Start { get; set; }

        public TimeSpan? Duration { get; set; }

        private bool _BitstreamSwitching;
        public bool BitstreamSwitching
        {
            get { return _BitstreamSwitching; }
            set { _BitstreamSwitching = value; }
        }

        private IList<XAttribute> _AnyAttr;
        public IList<XAttribute> AnyAttr
        {
            get { if (_AnyAttr == null) _AnyAttr = new List<XAttribute>(); return _AnyAttr; }
            private set { _AnyAttr = value; }
        }

        private IList<XElement> _Any;
        public IList<XElement> Any
        {
            get { if (_Any == null) _Any = new List<XElement>(); return _Any; }
            private set { _Any = value; }
        }
    }

    public enum Presentation
    {
        Static,
        Dynamic
    }
}
