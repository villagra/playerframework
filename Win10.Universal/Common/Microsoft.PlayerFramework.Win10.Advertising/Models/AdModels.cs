using System;
using System.Collections.Generic;

namespace Microsoft.Media.Advertising
{
    public sealed class AdDocumentPayload
    {
        internal AdDocumentPayload()
        {
            AdPods = new List<AdPod>();
        }

        public IList<AdPod> AdPods { get; private set; }

        public string Version { get; set; }

        public string Error { get; set; }
    }

    public sealed class AdPod
    {
        internal AdPod()
        {
            Ads = new List<Ad>();
        }

        public IList<Ad> Ads { get; private set; }
    }

    public sealed class Ad
    {
        internal Ad()
        {
            Impressions = new List<string>();
            Errors = new List<string>();
            Creatives = new List<ICreative>();
            Extensions = new List<Extension>();
            FallbackAds = new List<Ad>();
        }

        public string Id { get; set; }

        public AdSystem AdSystem { get; set; }

        public IList<string> Impressions { get; private set; }

        public IList<ICreative> Creatives { get; private set; }

        public IList<Extension> Extensions { get; private set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Advertiser { get; set; }

        public Pricing Pricing { get; set; }

        public Uri Survey { get; set; }

        public IList<string> Errors { get; set; }

        public IList<Ad> FallbackAds { get; set; } // Not part of VAST 3.0 spec; added to support additional feature.
    }

    public interface ICreative
    {
        string Id { get; set; }

        int? Sequence { get; set; }

        string AdId { get; set; }
    }

    public sealed class CreativeLinear : ICreative
    {
        internal CreativeLinear()
        {
            MediaFiles = new List<MediaFile>();
            Extensions = new List<object>();
            Icons = new List<Icon>();
            ClickTracking = new List<string>();
            CustomClick = new List<Uri>();
            TrackingEvents = new List<TrackingEvent>();
            AdParameters = string.Empty;
            Id = string.Empty;
            AdId = string.Empty;
        }

        public string Id { get; set; }

        public int? Sequence { get; set; }

        public string AdId { get; set; }

        public TimeSpan? Duration { get; set; }

        public string AdParameters { get; set; }

        public Uri ClickThrough { get; set; }

        public IList<MediaFile> MediaFiles { get; private set; }

        public FlexibleOffset SkipOffset { get; set; }

        public IList<object> Extensions { get; private set; }

        public IList<Icon> Icons { get; private set; }

        public IList<string> ClickTracking { get; private set; }

        public IList<Uri> CustomClick { get; private set; }

        public IList<TrackingEvent> TrackingEvents { get; private set; }
    }

    public sealed class CreativeNonLinears : ICreative
    {
        internal CreativeNonLinears()
        {
            NonLinears = new List<NonLinear>();
            TrackingEvents = new List<TrackingEvent>();
            Id = string.Empty;
            AdId = string.Empty;
        }

        public string Id { get; set; }

        public int? Sequence { get; set; }

        public string AdId { get; set; }

        public IList<NonLinear> NonLinears { get; private set; }

        public IList<TrackingEvent> TrackingEvents { get; private set; }
    }

    public sealed class CreativeCompanions : ICreative
    {
        internal CreativeCompanions()
        {
            Companions = new List<Companion>();
            Id = string.Empty;
            AdId = string.Empty;
        }

        public string Id { get; set; }

        public int? Sequence { get; set; }

        public string AdId { get; set; }

        public IList<Companion> Companions { get; private set; }

        public CompanionAdsRequired Required { get; set; }
    }

    public sealed class NonLinear
    {
        internal NonLinear()
        {
            Extensions = new List<object>();
            ClickTracking = new List<string>();
            AdParameters = string.Empty;
            ApiFramework = string.Empty;
            Id = string.Empty;
        }

        public IResource Item { get; set; }

        public Uri ClickThrough { get; set; }

        public string AdParameters { get; set; }

        public IList<object> Extensions { get; private set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int? ExpandedWidth { get; set; }

        public int? ExpandedHeight { get; set; }

        public bool? Scalable { get; set; }

        public bool? MaintainAspectRatio { get; set; }

        public TimeSpan? MinSuggestedDuration { get; set; }

        public string ApiFramework { get; set; }

        public IList<string> ClickTracking { get; set; }

        public string Id { get; set; }
    }

    public sealed class Companion : ICompanionSource
    {
        internal Companion()
        {
            Extensions = new List<object>();
            ClickTracking = new List<string>();
            ViewTracking = new List<string>();
            Id = string.Empty;
            AdParameters = string.Empty;
            AltText = string.Empty;
            AdSlotId = string.Empty;
            ApiFramework = string.Empty;
        }

        public IResource Item { get; set; }

        public IList<object> Extensions { get; private set; }

        public Uri ClickThrough { get; set; }

        public string AltText { get; set; }

        public string AdParameters { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public int? AssetWidth { get; set; }

        public int? AssetHeight { get; set; }

        public int? ExpandedWidth { get; set; }

        public int? ExpandedHeight { get; set; }

        public string ApiFramework { get; set; }

        public string AdSlotId { get; set; }

        public string Id { get; set; }

        public IList<string> ViewTracking { get; private set; }

        public IList<string> ClickTracking { get; private set; }

        public string MimeType
        {
            get { return Item is StaticResource ? ((StaticResource)Item).CreativeType : null; }
        }

        public string Content
        {
            get
            {
                if (Item is StaticResource)
                {
                    return ((StaticResource)Item).Value.OriginalString;
                }
                else if (Item is IFrameResource)
                {
                    return ((IFrameResource)Item).Value.OriginalString;
                }
                else if (Item is HtmlResource)
                {
                    return ((HtmlResource)Item).Value;
                }
                else throw new NotImplementedException();
            }
        }

        public CompanionType Type
        {
            get
            {
                if (Item is StaticResource)
                {
                    return CompanionType.Static;
                }
                else if (Item is IFrameResource)
                {
                    return CompanionType.IFrame;
                }
                else if (Item is HtmlResource)
                {
                    return CompanionType.Html;
                }
                else throw new NotImplementedException();
            }
        }
    }

    public sealed class MediaFile
    {
        internal MediaFile()
        {
            Id = string.Empty;
            Type = string.Empty;
            ApiFramework = string.Empty;
            Codec = string.Empty;
        }

        #region Required
        public string Id { get; set; }

        public MediaFileDelivery Delivery { get; set; }

        public string Type { get; set; }

        public Uri Value { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
        #endregion

        #region Optional
        public long? Bitrate { get; set; }

        public long? MinBitrate { get; set; }

        public long? MaxBitrate { get; set; }

        public bool? Scalable { get; set; }

        public bool? MaintainAspectRatio { get; set; }

        public string ApiFramework { get; set; }

        public string Codec { get; set; }

        /// <summary>
        /// Added to provide additional preference selection. Not part of the VAST spec. Higher numbers indicate preference over lower ones.
        /// </summary>
        public int Ranking { get; set; }
        #endregion
    }

    public sealed class IFrameResource : IResource
    {
        internal IFrameResource()
        { }

        public Uri Value { get; set; }
    }

    public sealed class HtmlResource : IResource
    {
        internal HtmlResource()
        { }

        public string Value { get; set; }
    }

    public sealed class StaticResource : IResource
    {
        internal StaticResource()
        { }

        public string CreativeType { get; set; }

        public Uri Value { get; set; }
    }

    public interface IResource
    { }

    public sealed class TrackingEvent
    {
        internal TrackingEvent()
        { }

        public TrackingType Type { get; set; }

        public FlexibleOffset Offset { get; set; }

        public string Value { get; set; }
    }

    public sealed class Icon
    {
        internal Icon()
        {
            ClickTracking = new List<string>();
            ViewTracking = new List<string>();
            ApiFramework = string.Empty;
            XPosition = string.Empty;
            YPosition = string.Empty;
            Program = string.Empty;
        }

        public IResource Item { get; set; }

        public IList<string> ClickTracking { get; private set; }

        public Uri ClickThrough { get; set; }

        public IList<string> ViewTracking { get; private set; }

        public string Program { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public string XPosition { get; set; }  // allows "left", "right" or a pixel value

        public string YPosition { get; set; }  // allows "top", "bottom", or a pixel value

        public TimeSpan? Offset { get; set; }

        public TimeSpan? Duration { get; set; }

        public string ApiFramework { get; set; }
    }

    public sealed class Extension
    {
        internal Extension()
        { }

        public string Type { get; set; }

        public string Value { get; set; }
    }

    public sealed class AdSystem
    {
        internal AdSystem()
        { }

        public string Version { get; set; }

        public string Value { get; set; }
    }

    public sealed class Pricing
    {
        internal Pricing()
        { }

        public PricingModel Model { get; set; }

        public string Currency { get; set; }

        public double Value { get; set; }
    }

    public enum PricingModel
    {
        Cpc,
        Cpm,
        Cpe,
        Cpv,
    }

    public enum CompanionAdsRequired
    {
        None,
        Any,
        All,
    }

    public enum MediaFileDelivery
    {
        Streaming,
        Progressive,
    }

    public enum TrackingType
    {
        CreativeView,
        Start,
        FirstQuartile,
        Midpoint,
        ThirdQuartile,
        Complete,
        Mute,
        Unmute,
        Pause,
        Rewind,
        Resume,
        Fullscreen,
        ExitFullscreen,
        Expand,
        Collapse,
        AcceptInvitation,
        Close,
        Skip,
        Progress,
    }

    public sealed class FlexibleOffset
    {
        internal FlexibleOffset()
        { }

        public static FlexibleOffset FromTimeSpan(TimeSpan absoluteOffset)
        {
            return new FlexibleOffset()
            {
                IsAbsolute = true,
                AbsoluteOffset = absoluteOffset
            };
        }

        public static FlexibleOffset FromPercent(double relativeOffset)
        {
            return new FlexibleOffset()
            {
                IsAbsolute = false,
                RelativeOffset = relativeOffset
            };
        }

        public static FlexibleOffset Parse(string SkippableOffset)
        {
            if (SkippableOffset == null) return null;
            if (SkippableOffset.EndsWith("%"))
            {
                double percent;
                if (double.TryParse(SkippableOffset.Substring(0, SkippableOffset.Length - 1), out percent))
                {
                    return new FlexibleOffset()
                    {
                        RelativeOffset = percent / 100,
                        IsAbsolute = false
                    };
                }
                else return null;
            }
            else
            {
                TimeSpan position;
                if (TimeSpan.TryParse(SkippableOffset, out position))
                {
                    return new FlexibleOffset()
                    {
                        AbsoluteOffset = position,
                        IsAbsolute = true
                    };
                }
                else return null;
            }
        }

        /// <summary>
        /// Gets or set whether or not the offset is absolute.
        /// </summary>
        public bool IsAbsolute { get; set; }

        /// <summary>
        /// Gets or sets the relative offset as a percentage (0-1).
        /// </summary>
        public double RelativeOffset { get; set; }

        /// <summary>
        /// Gets or sets the absolute offset as a TimeSpan
        /// </summary>
        public TimeSpan AbsoluteOffset { get; set; }
    }
}
