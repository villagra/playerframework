using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.Media.AdaptiveStreaming.Dash
{
    internal class MPD
    {
        public MPD()
        {
            this.AnyAttr = new List<XAttribute>();
            this.Any = new List<XElement>();
            this.Metrics = new List<Metrics>();
            this.Period = new List<Period>();
            this.Location = new List<string>();
            this.BaseURL = new List<BaseURL>();
            this.ProgramInformation = new List<ProgramInformation>();
            this.Type = Presentation.Static;
        }

        public IList<ProgramInformation> ProgramInformation { get; private set; }

        public IList<BaseURL> BaseURL { get; private set; }

        public IList<string> Location { get; private set; }

        public IList<Period> Period { get; private set; }

        public IList<Metrics> Metrics { get; private set; }

        public string Id { get; set; }

        public string Profiles { get; set; }

        public Presentation Type { get; set; }

        public DateTimeOffset? AvailabilityStartTime { get; set; }

        public DateTimeOffset? AvailabilityEndTime { get; set; }

        public TimeSpan? MediaPresentationDuration { get; set; }

        public TimeSpan? MinimumUpdatePeriod { get; set; }

        public TimeSpan? MinBufferTime { get; set; }

        public TimeSpan? TimeShiftBufferDepth { get; set; }

        public TimeSpan? SuggestedPresentationDelay { get; set; }

        public TimeSpan? MaxSegmentDuration { get; set; }

        public TimeSpan? MaxSubsegmentDuration { get; set; }

        public IList<XAttribute> AnyAttr { get; private set; }

        public IList<XElement> Any { get; private set; }
    }

    internal class ProgramInformation
    {
        public ProgramInformation()
        {
            this.AnyAttr = new List<XAttribute>();
            this.Any = new List<XElement>();
        }

        public string Title { get; set; }

        public string Source { get; set; }

        public string Copyright { get; set; }

        public string Lang { get; set; }

        public string MoreInformationURL { get; set; }

        public IList<XAttribute> AnyAttr { get; private set; }

        public IList<XElement> Any { get; private set; }
    }

    internal class Range
    {
        public TimeSpan? StartTime { get; set; }

        public TimeSpan? Duration { get; set; }
    }

    internal class Metrics
    {
        public Metrics()
        {
            this.AnyAttr = new List<XAttribute>();
            this.Any = new List<XElement>();
            this.Range = new List<Range>();
            this.Reporting = new List<Descriptor>();
        }

        public IList<Descriptor> Reporting { get; private set; }

        public IList<Range> Range { get; private set; }

        public string MetricsValue { get; set; }

        public IList<XAttribute> AnyAttr { get; private set; }

        public IList<XElement> Any { get; private set; }
    }

    internal class Descriptor
    {
        public Descriptor()
        {
            this.AnyAttr = new List<XAttribute>();
            this.Any = new List<XElement>();
        }

        public string SchemeIdUri { get; set; }

        public string Value { get; set; }

        public IList<XAttribute> AnyAttr { get; private set; }

        public IList<XElement> Any { get; private set; }
    }

    internal class Subset
    {
        public Subset()
        {
            this.AnyAttr = new List<XAttribute>();
            this.Contains = new List<uint>();
        }

        public IList<uint> Contains { get; private set; }

        public IList<XAttribute> AnyAttr { get; private set; }
    }

    internal class ContentComponent
    {
        public ContentComponent()
        {
            this.AnyAttr = new List<XAttribute>();
            this.Any = new List<XElement>();
            this.Viewpoint = new List<Descriptor>();
            this.Rating = new List<Descriptor>();
            this.Role = new List<Descriptor>();
            this.Accessibility = new List<Descriptor>();
        }

        public IList<Descriptor> Accessibility { get; private set; }

        public IList<Descriptor> Role { get; private set; }

        public IList<Descriptor> Rating { get; private set; }

        public IList<Descriptor> Viewpoint { get; private set; }

        public uint? Id { get; set; }

        public string Lang { get; set; }

        public string ContentType { get; set; }

        public string Par { get; set; }

        public IList<XAttribute> AnyAttr { get; private set; }

        public IList<XElement> Any { get; private set; }
    }

    internal class RepresentationBase
    {
        public RepresentationBase()
        {
            this.AnyAttr = new List<XAttribute>();
            this.Any = new List<XElement>();
            this.ContentProtection = new List<Descriptor>();
            this.AudioChannelConfiguration = new List<Descriptor>();
            this.FramePacking = new List<Descriptor>();
        }

        public IList<Descriptor> FramePacking { get; private set; }

        public IList<Descriptor> AudioChannelConfiguration { get; private set; }

        public IList<Descriptor> ContentProtection { get; private set; }

        public string Profiles { get; set; }

        public uint? Width { get; set; }

        public uint? Height { get; set; }

        public string Sar { get; set; }

        public string FrameRate { get; set; }

        public string AudioSamplingRate { get; set; }

        public string MimeType { get; set; }

        public string SegmentProfiles { get; set; }

        public string Codecs { get; set; }

        public double? MaximumSAPPeriod { get; set; }

        public uint? StartWithSAP { get; set; }

        public double? MaxPlayoutRate { get; set; }

        public bool? CodingDependency { get; set; }

        public VideoScan? ScanType { get; set; }

        public IList<XAttribute> AnyAttr { get; private set; }

        public IList<XElement> Any { get; private set; }
    }

    public enum VideoScan
    {
        Progressive,
        Interlaced,
        Unknown,
    }

    internal class SubRepresentation : RepresentationBase
    {
        public SubRepresentation()
        {
            this.ContentComponent = new List<string>();
            this.DependencyLevel = new List<uint>();
        }

        public uint? Level { get; set; }

        public IList<uint> DependencyLevel { get; private set; }

        public uint? Bandwidth { get; set; }

        public IList<string> ContentComponent { get; set; }
    }

    internal class Representation : RepresentationBase
    {
        public Representation()
        {
            this.MediaStreamStructureId = new List<string>();
            this.DependencyId = new List<string>();
            this.SegmentTemplate = new SegmentTemplate();
            this.SegmentList = new SegmentList();
            this.SegmentBase = new SegmentBase();
            this.SubRepresentation = new List<SubRepresentation>();
            this.BaseURL = new List<BaseURL>();
        }

        public IList<BaseURL> BaseURL { get; private set; }

        public IList<SubRepresentation> SubRepresentation { get; private set; }

        public SegmentBase SegmentBase { get; set; }

        public SegmentList SegmentList { get; set; }

        public SegmentTemplate SegmentTemplate { get; set; }

        public string Id { get; set; }

        public uint Bandwidth { get; set; }

        public uint? QualityRanking { get; set; }

        public IList<string> DependencyId { get; private set; }

        public IList<string> MediaStreamStructureId { get; private set; }
    }

    internal class BaseURL
    {
        public BaseURL()
        {
            this.AnyAttr = new List<XAttribute>();
        }

        public string ServiceLocation { get; set; }

        public string ByteRange { get; set; }

        public IList<XAttribute> AnyAttr { get; private set; }

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

        public URL Initialization { get; set; }

        public URL RepresentationIndex { get; set; }

        public uint? Timescale { get; set; }

        public uint? PresentationTimeOffset { get; set; }

        public string IndexRange { get; set; }

        public bool? IndexRangeExact { get; set; }

        public IList<XAttribute> AnyAttr { get; private set; }
        public IList<XElement> Any { get; private set; }
    }

    internal class URL
    {
        public URL()
        {
            this.AnyAttr = new List<XAttribute>();
            this.Any = new List<XElement>();
        }

        public string SourceURL { get; set; }

        public string Range { get; set; }

        public IList<XAttribute> AnyAttr { get; private set; }

        public IList<XElement> Any { get; private set; }
    }

    internal class MultipleSegmentBase : SegmentBase
    {
        public MultipleSegmentBase()
        {
            this.BitstreamSwitching = new URL();
            this.SegmentTimeline = new SegmentTimeline();
        }

        public SegmentTimeline SegmentTimeline { get; set; }

        public URL BitstreamSwitching { get; set; }

        public uint? Duration { get; set; }

        public uint? StartNumber { get; set; }
    }

    internal class SegmentTimeline
    {
        public SegmentTimeline()
        {
            this.AnyAttr = new List<XAttribute>();
            this.Any = new List<XElement>();
            this.S = new List<SegmentTimelineS>();
        }

        public IList<SegmentTimelineS> S { get; private set; }

        public IList<XElement> Any { get; private set; }

        public IList<XAttribute> AnyAttr { get; private set; }
    }

    internal class SegmentTimelineS
    {
        public SegmentTimelineS()
        {
            this.AnyAttr = new List<XAttribute>();
            this.R = ((uint)(0));
        }

        public uint? T { get; set; }

        public uint D { get; set; }

        public uint R { get; set; }

        public IList<XAttribute> AnyAttr { get; private set; }
    }

    internal class SegmentTemplate : MultipleSegmentBase
    {
        public string Media { get; set; }

        public string Index { get; set; }

        public string InitializationValue { get; set; }

        public string BitstreamSwitchingValue { get; set; }
    }

    internal class SegmentList : MultipleSegmentBase
    {
        public SegmentList()
        {
            this.SegmentURL = new List<SegmentURL>();
            this.Actuate = Actuate.OnRequest;
        }

        public IList<SegmentURL> SegmentURL { get; private set; }

        public string Href { get; set; }

        public Actuate Actuate { get; set; }
    }

    internal class SegmentURL
    {
        public SegmentURL()
        {
            this.AnyAttr = new List<XAttribute>();
            this.Any = new List<XElement>();
        }

        public string Media { get; set; }

        public string MediaRange { get; set; }

        public string Index { get; set; }

        public string IndexRange { get; set; }

        public IList<XAttribute> AnyAttr { get; private set; }

        public IList<XElement> Any { get; private set; }
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
        public AdaptationSet()
        {
            this.Representation = new List<Representation>();
            this.SegmentTemplate = new SegmentTemplate();
            this.SegmentList = new SegmentList();
            this.SegmentBase = new SegmentBase();
            this.BaseURL = new List<BaseURL>();
            this.ContentComponent = new List<ContentComponent>();
            this.Viewpoint = new List<Descriptor>();
            this.Rating = new List<Descriptor>();
            this.Role = new List<Descriptor>();
            this.Accessibility = new List<Descriptor>();
            this.Actuate = Actuate.OnRequest;
            this.SegmentAlignment = "false";
            this.SubsegmentAlignment = "false";
            this.SubsegmentStartsWithSAP = ((uint)(0));
        }

        public IList<Descriptor> Accessibility { get; private set; }

        public IList<Descriptor> Role { get; private set; }

        public IList<Descriptor> Rating { get; private set; }

        public IList<Descriptor> Viewpoint { get; private set; }

        public IList<ContentComponent> ContentComponent { get; private set; }

        public IList<BaseURL> BaseURL { get; private set; }

        public SegmentBase SegmentBase { get; set; }

        public SegmentList SegmentList { get; set; }

        public SegmentTemplate SegmentTemplate { get; set; }

        public IList<Representation> Representation { get; private set; }

        public string Href { get; set; }

        public Actuate Actuate { get; set; }

        public uint? Id { get; set; }

        public uint? Group { get; set; }

        public string Lang { get; set; }

        public string ContentType { get; set; }

        public string Par { get; set; }

        public uint? MinBandwidth { get; set; }

        public uint? MaxBandwidth { get; set; }

        public uint? MinWidth { get; set; }

        public uint? MaxWidth { get; set; }

        public uint? MinHeight { get; set; }

        public uint? MaxHeight { get; set; }

        public ConditionalUInt MinFrameRate { get; set; }

        public ConditionalUInt MaxFrameRate { get; set; }

        public string SegmentAlignment { get; set; }

        public string SubsegmentAlignment { get; set; }

        public uint SubsegmentStartsWithSAP { get; set; }

        public bool? BitstreamSwitching { get; set; }
    }

    internal class Period
    {
        public Period()
        {
            this.AnyAttr = new List<XAttribute>();
            this.Any = new List<XElement>();
            this.Subset = new List<Subset>();
            this.AdaptationSet = new List<AdaptationSet>();
            this.SegmentTemplate = new SegmentTemplate();
            this.SegmentList = new SegmentList();
            this.SegmentBase = new SegmentBase();
            this.BaseURL = new List<BaseURL>();
            this.Actuate = Actuate.OnRequest;
            this.BitstreamSwitching = false;
        }

        public IList<BaseURL> BaseURL { get; private set; }

        public SegmentBase SegmentBase { get; set; }

        public SegmentList SegmentList { get; set; }

        public SegmentTemplate SegmentTemplate { get; set; }

        public IList<AdaptationSet> AdaptationSet { get; private set; }

        public IList<Subset> Subset { get; private set; }

        public string Href { get; set; }

        public Actuate Actuate { get; set; }

        public string Id { get; set; }

        public TimeSpan? Start { get; set; }

        public TimeSpan? Duration { get; set; }

        public bool BitstreamSwitching { get; set; }

        public IList<XAttribute> AnyAttr { get; private set; }

        public IList<XElement> Any { get; private set; }
    }

    public enum Presentation
    {
        Static,
        Dynamic
    }
}
