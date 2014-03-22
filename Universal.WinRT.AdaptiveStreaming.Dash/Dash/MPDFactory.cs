using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.Media.AdaptiveStreaming.Dash
{
    internal static class MPDFactory
    {
        public static MPD LoadMPD(XElement element)
        {
            var ns = element.GetDefaultNamespace().NamespaceName;
            var result = new MPD();

            result.Id = (string)element.Attribute("id");
            result.Profiles = (string)element.Attribute("profiles");
            result.Type = element.Attribute("type").GetEnum<Presentation>();
            result.AvailabilityStartTime = element.Attribute("availabilityStartTime").GetNullableDateTime();
            result.AvailabilityEndTime = element.Attribute("availabilityEndTime").GetNullableDateTime();
            result.MediaPresentationDuration = element.Attribute("mediaPresentationDuration").GetNullableDuration();
            result.MinimumUpdatePeriod = element.Attribute("minimumUpdatePeriod").GetNullableDuration();
            result.MinBufferTime = element.Attribute("minBufferTime").GetNullableDuration();
            result.TimeShiftBufferDepth = element.Attribute("timeShiftBufferDepth").GetNullableDuration();
            result.SuggestedPresentationDelay = element.Attribute("suggestedPresentationDelay").GetNullableDuration();
            result.MaxSegmentDuration = element.Attribute("maxSegmentDuration").GetNullableDuration();
            result.MaxSubsegmentDuration = element.Attribute("maxSubsegmentDuration").GetNullableDuration();
            result.AnyAttr.AddRange(element.Attributes());

            result.ProgramInformation.AddRange(element.Elements(XName.Get("ProgramInformation", ns)).Select(LoadProgramInformation));
            result.BaseURL.AddRange(element.Elements(XName.Get("BaseURL", ns)).Select(LoadBaseURL));
            result.Location.AddRange(element.Elements(XName.Get("Location", ns)).Select(e => e.Value));
            result.Period.AddRange(element.Elements(XName.Get("Period", ns)).Select(LoadPeriod));
            result.Metrics.AddRange(element.Elements(XName.Get("Metrics", ns)).Select(LoadMetrics));
            result.Any.AddRange(element.Elements());

            return result;
        }

        static ProgramInformation LoadProgramInformation(XElement element)
        {
            var ns = element.GetDefaultNamespace().NamespaceName;
            var result = new ProgramInformation();

            result.Lang = (string)element.Attribute("lang");
            result.MoreInformationURL = (string)element.Attribute("moreInformationURL");
            result.AnyAttr.AddRange(element.Attributes());

            result.Title = (string)element.Element("Title");
            result.Source = (string)element.Element("Source");
            result.Copyright = (string)element.Element("Copyright");
            result.Any.AddRange(element.Elements());

            return result;
        }

        static BaseURL LoadBaseURL(XElement element)
        {
            var result = new BaseURL();

            result.ServiceLocation = (string)element.Attribute("serviceLocation");
            result.ByteRange = (string)element.Attribute("byteRange");
            result.AnyAttr.AddRange(element.Attributes());

            result.Value = element.Value;

            return result;
        }

        static Period LoadPeriod(XElement element)
        {
            var ns = element.GetDefaultNamespace().NamespaceName;
            var result = new Period();

            result.Href = (string)element.Attribute("href");
            result.Actuate = element.Attribute("actuate").GetNullableEnum<Actuate>().GetValueOrDefault(result.Actuate);
            result.Id = (string)element.Attribute("id");
            result.Start = element.Attribute("start").GetNullableDuration();
            result.Duration = element.Attribute("duration").GetNullableDuration();
            result.BitstreamSwitching = element.Attribute("bitstreamSwitching").GetNullableBool().GetValueOrDefault(result.BitstreamSwitching);
            result.AnyAttr.AddRange(element.Attributes());

            result.BaseURL.AddRange(element.Elements(XName.Get("BaseURL", ns)).Select(LoadBaseURL));
            result.SegmentBase = element.Elements(XName.Get("SegmentBase", ns)).Select(LoadSegmentBase).SingleOrDefault();
            result.SegmentList = element.Elements(XName.Get("SegmentList", ns)).Select(LoadSegmentList).SingleOrDefault();
            result.SegmentTemplate = element.Elements(XName.Get("SegmentTemplate", ns)).Select(LoadSegmentTemplate).SingleOrDefault();
            result.AdaptationSet.AddRange(element.Elements(XName.Get("AdaptationSet", ns)).Select(LoadAdaptationSet));
            result.Subset.AddRange(element.Elements(XName.Get("Subset", ns)).Select(LoadSubset));
            result.Any.AddRange(element.Elements());

            return result;
        }

        static Metrics LoadMetrics(XElement element)
        {
            var ns = element.GetDefaultNamespace().NamespaceName;
            var result = new Metrics();

            result.MetricsValue = (string)element.Attribute("metrics");
            result.AnyAttr.AddRange(element.Attributes());

            result.Reporting.AddRange(element.Elements(XName.Get("Reporting", ns)).Select(LoadDescriptor));
            result.Range.AddRange(element.Elements(XName.Get("Range", ns)).Select(LoadRange));
            result.Any.AddRange(element.Elements());

            return result;
        }

        static Range LoadRange(XElement element)
        {
            var result = new Range();

            result.StartTime = element.Attribute("starttime").GetNullableDuration();
            result.Duration = element.Attribute("duration").GetNullableDuration();

            return result;
        }

        static Descriptor LoadDescriptor(XElement element)
        {
            var result = new Descriptor();

            result.Value = (string)element.Attribute("value");
            result.SchemeIdUri = (string)element.Attribute("schemeIdUri");
            result.AnyAttr.AddRange(element.Attributes());

            result.Any.AddRange(element.Elements());

            return result;
        }

        static SegmentBase LoadSegmentBase(XElement element)
        {
            var result = new SegmentBase();
            PopulateSegmentBase(element, result);
            return result;
        }

        static void PopulateSegmentBase(XElement element, SegmentBase result)
        {
            var ns = element.GetDefaultNamespace().NamespaceName;
            result.Timescale = element.Attribute("timescale").GetNullableUInt();
            result.PresentationTimeOffset = element.Attribute("presentationTimeOffset").GetNullableUInt();
            result.IndexRange = (string)element.Attribute("indexRange");
            result.IndexRangeExact = element.Attribute("indexRangeExact").GetNullableBool();
            result.AnyAttr.AddRange(element.Attributes());

            result.Initialization = element.Elements(XName.Get("Initialisation", ns)).Concat(element.Elements(XName.Get("Initialization", ns))).Select(LoadURL).SingleOrDefault();
            result.RepresentationIndex = element.Elements(XName.Get("RepresentationIndex", ns)).Select(LoadURL).SingleOrDefault();
            result.Any.AddRange(element.Elements());
        }

        static SegmentList LoadSegmentList(XElement element)
        {
            var ns = element.GetDefaultNamespace().NamespaceName;
            var result = new SegmentList();
            PopulateMultipleSegmentBase(element, result);

            result.Href = (string)element.Attribute("href");
            result.Actuate = element.Attribute("actuate").GetNullableEnum<Actuate>().GetValueOrDefault(result.Actuate);

            result.SegmentURL.AddRange(element.Elements(XName.Get("SegmentURL", ns)).Select(LoadSegmentURL));

            return result;
        }

        static void PopulateMultipleSegmentBase(XElement element, MultipleSegmentBase result)
        {
            var ns = element.GetDefaultNamespace().NamespaceName;
            PopulateSegmentBase(element, result);

            result.Duration = element.Attribute("duration").GetNullableUInt();
            result.StartNumber = element.Attribute("startNumber").GetNullableUInt();

            result.SegmentTimeline = element.Elements(XName.Get("SegmentTimeline", ns)).Select(LoadSegmentTimeline).SingleOrDefault();
            result.BitstreamSwitching = element.Elements(XName.Get("BitstreamSwitching", ns)).Select(LoadURL).SingleOrDefault();
        }

        static SegmentTimeline LoadSegmentTimeline(XElement element)
        {
            var ns = element.GetDefaultNamespace().NamespaceName;
            var result = new SegmentTimeline();

            result.AnyAttr.AddRange(element.Attributes());

            result.S.AddRange(element.Elements(XName.Get("S", ns)).Select(LoadSegmentTimelineS));
            result.Any.AddRange(element.Elements());

            return result;
        }

        static SegmentTimelineS LoadSegmentTimelineS(XElement element)
        {
            var ns = element.GetDefaultNamespace().NamespaceName;
            var result = new SegmentTimelineS();

            result.T = element.Attribute("t").GetNullableUInt();
            result.D = element.Attribute("d").GetUInt();
            result.R = element.Attribute("r").GetNullableUInt().GetValueOrDefault(result.R);
            result.AnyAttr.AddRange(element.Attributes());

            return result;
        }

        static SegmentTemplate LoadSegmentTemplate(XElement element)
        {
            var result = new SegmentTemplate();
            PopulateMultipleSegmentBase(element, result);

            result.Media = (string)element.Attribute("media");
            result.Index = (string)element.Attribute("index");
            result.InitializationValue = (string)(element.Attribute("initialisation") ?? element.Attribute("initialization"));
            result.BitstreamSwitchingValue = (string)element.Attribute("bitstreamSwitching");

            return result;
        }

        static SegmentURL LoadSegmentURL(XElement element)
        {
            var result = new SegmentURL();

            result.Media = (string)element.Attribute("media");
            result.MediaRange = (string)element.Attribute("mediaRange");
            result.Index = (string)element.Attribute("index");
            result.IndexRange = (string)element.Attribute("indexRange");
            result.AnyAttr.AddRange(element.Attributes());

            result.Any.AddRange(element.Elements());

            return result;
        }

        static URL LoadURL(XElement element)
        {
            var result = new URL();

            result.SourceURL = (string)element.Attribute("sourceURL");
            result.Range = (string)element.Attribute("range");
            result.AnyAttr.AddRange(element.Attributes());

            result.Any.AddRange(element.Elements());

            return result;
        }

        static Subset LoadSubset(XElement element)
        {
            var result = new Subset();

            result.Contains.AddRange(element.Attribute("contains").GetUIntVector());
            result.AnyAttr.AddRange(element.Attributes());

            return result;
        }

        static void PopulateRepresentationBase(XElement element, RepresentationBase result)
        {
            var ns = element.GetDefaultNamespace().NamespaceName;
            result.Profiles = (string)element.Attribute("profiles");
            result.Width = element.Attribute("width").GetNullableUInt();
            result.Height = element.Attribute("height").GetNullableUInt();
            result.Sar = (string)element.Attribute("sar");
            result.FrameRate = (string)element.Attribute("frameRate");
            result.AudioSamplingRate = (string)element.Attribute("audioSamplingRate");
            result.MimeType = (string)element.Attribute("mimeType");
            result.SegmentProfiles = (string)element.Attribute("segmentProfiles");
            result.Codecs = (string)element.Attribute("codecs");
            result.MaximumSAPPeriod = element.Attribute("maximumSAPPeriod").GetNullableDouble();
            result.StartWithSAP = element.Attribute("startWithSAP").GetNullableUInt();
            result.MaxPlayoutRate = element.Attribute("maxPlayoutRate").GetNullableDouble();
            result.CodingDependency = element.Attribute("codingDependency").GetNullableBool();
            result.ScanType = element.Attribute("scanType").GetNullableEnum<VideoScan>();
            result.AnyAttr.AddRange(element.Attributes());

            result.FramePacking.AddRange(element.Elements(XName.Get("FramePacking", ns)).Select(LoadDescriptor));
            result.AudioChannelConfiguration.AddRange(element.Elements(XName.Get("AudioChannelConfiguration", ns)).Select(LoadDescriptor));
            result.ContentProtection.AddRange(element.Elements(XName.Get("ContentProtection", ns)).Select(LoadDescriptor));
            result.Any.AddRange(element.Elements());
        }

        static AdaptationSet LoadAdaptationSet(XElement element)
        {
            var ns = element.GetDefaultNamespace().NamespaceName;
            var result = new AdaptationSet();
            PopulateRepresentationBase(element, result);

            result.Href = (string)element.Attribute("href");
            result.Actuate = element.Attribute("actuate").GetNullableEnum<Actuate>().GetValueOrDefault(result.Actuate);
            result.Id = element.Attribute("id").GetNullableUInt();
            result.Group = element.Attribute("group").GetNullableUInt();
            result.Lang = (string)element.Attribute("lang");
            result.ContentType = (string)element.Attribute("contentType");
            result.Par = (string)element.Attribute("par");
            result.MinBandwidth = element.Attribute("minBandwidth").GetNullableUInt();
            result.MaxBandwidth = element.Attribute("maxBandwidth").GetNullableUInt();
            result.MinWidth = element.Attribute("minWidth").GetNullableUInt();
            result.MaxWidth = element.Attribute("maxWidth").GetNullableUInt();
            result.MinHeight = element.Attribute("minHeight").GetNullableUInt();
            result.MaxHeight = element.Attribute("maxHeight").GetNullableUInt();
            result.MinFrameRate = element.Attribute("minFrameRate").GetConditionalUInt();
            result.MaxFrameRate = element.Attribute("maxFrameRate").GetConditionalUInt();
            result.SegmentAlignment = (string)element.Attribute("segmentAlignment") ?? result.SegmentAlignment;
            result.SubsegmentAlignment = (string)element.Attribute("subsegmentAlignment") ?? result.SubsegmentAlignment;
            result.SubsegmentStartsWithSAP = element.Attribute("subsegmentStartsWithSAP").GetNullableUInt().GetValueOrDefault(result.SubsegmentStartsWithSAP);
            result.BitstreamSwitching = element.Attribute("bitstreamSwitching").GetNullableBool();

            result.Accessibility.AddRange(element.Elements(XName.Get("Accessibility", ns)).Select(LoadDescriptor));
            result.Role.AddRange(element.Elements(XName.Get("Role", ns)).Select(LoadDescriptor));
            result.Rating.AddRange(element.Elements(XName.Get("Rating", ns)).Select(LoadDescriptor));
            result.Viewpoint.AddRange(element.Elements(XName.Get("Viewpoint", ns)).Select(LoadDescriptor));
            result.ContentComponent.AddRange(element.Elements(XName.Get("ContentComponent", ns)).Select(LoadContentComponent));
            result.BaseURL.AddRange(element.Elements(XName.Get("BaseURL", ns)).Select(LoadBaseURL));
            result.SegmentBase = element.Elements(XName.Get("SegmentBase", ns)).Select(LoadSegmentBase).SingleOrDefault();
            result.SegmentList = element.Elements(XName.Get("SegmentList", ns)).Select(LoadSegmentList).SingleOrDefault();
            result.SegmentTemplate = element.Elements(XName.Get("SegmentTemplate", ns)).Select(LoadSegmentTemplate).SingleOrDefault();
            result.Representation.AddRange(element.Elements(XName.Get("Representation", ns)).Select(LoadRepresentation));

            return result;
        }

        static ContentComponent LoadContentComponent(XElement element)
        {
            var ns = element.GetDefaultNamespace().NamespaceName;
            var result = new ContentComponent();

            result.Id = element.Attribute("id").GetNullableUInt();
            result.Lang = (string)element.Attribute("lang");
            result.ContentType = (string)element.Attribute("contentType");
            result.Par = (string)element.Attribute("par");
            result.AnyAttr.AddRange(element.Attributes());

            result.Accessibility.AddRange(element.Elements(XName.Get("Accessibility", ns)).Select(LoadDescriptor));
            result.Role.AddRange(element.Elements(XName.Get("Role", ns)).Select(LoadDescriptor));
            result.Rating.AddRange(element.Elements(XName.Get("Rating", ns)).Select(LoadDescriptor));
            result.Viewpoint.AddRange(element.Elements(XName.Get("Viewpoint", ns)).Select(LoadDescriptor));
            result.Any.AddRange(element.Elements());

            return result;
        }

        static Representation LoadRepresentation(XElement element)
        {
            var ns = element.GetDefaultNamespace().NamespaceName;
            var result = new Representation();
            PopulateRepresentationBase(element, result);

            result.Id = (string)element.Attribute("id");
            result.Bandwidth = element.Attribute("bandwidth").GetUInt();
            result.QualityRanking = element.Attribute("qualityRanking").GetNullableUInt();
            result.DependencyId.AddRange(element.Attribute("dependencyId").GetStringVector());
            result.MediaStreamStructureId.AddRange(element.Attribute("mediaStreamStructureId").GetStringVector());

            result.BaseURL.AddRange(element.Elements(XName.Get("BaseURL", ns)).Select(LoadBaseURL));
            result.SubRepresentation.AddRange(element.Elements(XName.Get("SubRepresentation", ns)).Select(LoadSubRepresentation));
            result.SegmentBase = element.Elements(XName.Get("SegmentBase", ns)).Select(LoadSegmentBase).SingleOrDefault();
            result.SegmentList = element.Elements(XName.Get("SegmentList", ns)).Select(LoadSegmentList).SingleOrDefault();
            result.SegmentTemplate = element.Elements(XName.Get("SegmentTemplate", ns)).Select(LoadSegmentTemplate).SingleOrDefault();

            return result;
        }

        static SubRepresentation LoadSubRepresentation(XElement element)
        {
            var result = new SubRepresentation();
            PopulateRepresentationBase(element, result);

            result.Level = element.Attribute("level").GetNullableUInt();
            result.DependencyLevel.AddRange(element.Attribute("dependencyLevel").GetUIntVector());
            result.Bandwidth = element.Attribute("bandwidth").GetNullableUInt();
            result.ContentComponent.AddRange(element.Attribute("contentComponent").GetStringVector());

            return result;
        }
    }
}
