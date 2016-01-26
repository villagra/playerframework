using System;
using System.Collections.Generic;

namespace Microsoft.Media.Advertising
{
    public sealed class AdDocumentPayload
    {
        private IList<AdPod> _AdPods;
        public IList<AdPod> AdPods
        {
            get { if (_AdPods == null) _AdPods = new List<AdPod>(); return _AdPods; }
            private set { _AdPods = value; }
        }

        private string _Version = string.Empty;
        public string Version
        {
            get { return _Version; }
            set { _Version = value; }
        }

        private string _Error = string.Empty;
        public string Error
        {
            get { return _Error; }
            set { _Error = value; }
        }
    }

    public sealed class AdPod
    {
        private IList<Ad> _Ads;
        public IList<Ad> Ads
        {
            get { if (_Ads == null) _Ads = new List<Ad>(); return _Ads; }
            private set { _Ads = value; }
        }
    }

    public sealed class Ad
    {
        private string _Id = string.Empty;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private AdSystem _AdSystem;
        public AdSystem AdSystem
        {
            get { return _AdSystem; }
            set { _AdSystem = value; }
        }

        private IList<string> _Impressions;
        public IList<string> Impressions
        {
            get { if (_Impressions == null) _Impressions = new List<string>(); return _Impressions; }
            private set { _Impressions = value; }
        }

        private IList<ICreative> _Creatives;
        public IList<ICreative> Creatives
        {
            get { if (_Creatives == null) _Creatives = new List<ICreative>(); return _Creatives; }
            private set { _Creatives = value; }
        }

        private IList<Extension> _Extensions;
        public IList<Extension> Extensions
        {
            get { if (_Extensions == null) _Extensions = new List<Extension>(); return _Extensions; }
            private set { _Extensions = value; }
        }

        private string _Title = string.Empty;
        public string Title
        {
            get { return _Title; }
            set { _Title = value; }
        }

        private string _Description = string.Empty;
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        private string _Advertiser = string.Empty;
        public string Advertiser
        {
            get { return _Advertiser; }
            set { _Advertiser = value; }
        }

        private Pricing _Pricing;
        public Pricing Pricing
        {
            get { return _Pricing; }
            set { _Pricing = value; }
        }

        private Uri _Survey;
        public Uri Survey
        {
            get { return _Survey; }
            set { _Survey = value; }
        }

        private IList<string> _Errors;
        public IList<string> Errors
        {
            get { if (_Errors == null) _Errors = new List<string>(); return _Errors; }
            private set { _Errors = value; }
        }

        private IList<Ad> _FallbackAds;
        public IList<Ad> FallbackAds
        {
            get { if (_FallbackAds == null) _FallbackAds = new List<Ad>(); return _FallbackAds; }
            private set { _FallbackAds = value; }
        }
    }

    public interface ICreative
    {
        string Id { get; set; }

        int? Sequence { get; set; }

        string AdId { get; set; }
    }

    public sealed class CreativeLinear : ICreative
    {
        private string _Id = string.Empty;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private string _AdId = string.Empty;
        public string AdId
        {
            get { return _AdId; }
            set { _AdId = value; }
        }

        public int? Sequence { get; set; }

        public TimeSpan? Duration { get; set; }

        private string _AdParameters = string.Empty;
        public string AdParameters
        {
            get { return _AdParameters; }
            set { _AdParameters = value; }
        }

        private Uri _ClickThrough;
        public Uri ClickThrough
        {
            get { return _ClickThrough; }
            set { _ClickThrough = value; }
        }

        private IList<MediaFile> _MediaFiles;
        public IList<MediaFile> MediaFiles
        {
            get { if (_MediaFiles == null) _MediaFiles = new List<MediaFile>(); return _MediaFiles; }
            private set { _MediaFiles = value; }
        }

        public FlexibleOffset SkipOffset { get; set; }

        private IList<object> _Extensions;
        public IList<object> Extensions
        {
            get { if (_Extensions == null) _Extensions = new List<object>(); return _Extensions; }
            private set { _Extensions = value; }
        }

        private IList<Icon> _Icons;
        public IList<Icon> Icons
        {
            get { if (_Icons == null) _Icons = new List<Icon>(); return _Icons; }
            private set { _Icons = value; }
        }

        private IList<string> _ClickTracking;
        public IList<string> ClickTracking
        {
            get { if (_ClickTracking == null) _ClickTracking = new List<string>(); return _ClickTracking; }
            private set { _ClickTracking = value; }
        }

        private IList<Uri> _CustomClick;
        public IList<Uri> CustomClick
        {
            get { if (_CustomClick == null) _CustomClick = new List<Uri>(); return _CustomClick; }
            private set { _CustomClick = value; }
        }

        private IList<TrackingEvent> _TrackingEvents;
        public IList<TrackingEvent> TrackingEvents
        {
            get { if (_TrackingEvents == null) _TrackingEvents = new List<TrackingEvent>(); return _TrackingEvents; }
            private set { _TrackingEvents = value; }
        }
    }

    public sealed class CreativeNonLinears : ICreative
    {
        private string _Id = string.Empty;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private string _AdId = string.Empty;
        public string AdId
        {
            get { return _AdId; }
            set { _AdId = value; }
        }

        public int? Sequence { get; set; }

        private IList<NonLinear> _NonLinears;
        public IList<NonLinear> NonLinears
        {
            get { if (_NonLinears == null) _NonLinears = new List<NonLinear>(); return _NonLinears; }
            private set { _NonLinears = value; }
        }

        private IList<TrackingEvent> _TrackingEvents;
        public IList<TrackingEvent> TrackingEvents
        {
            get { if (_TrackingEvents == null) _TrackingEvents = new List<TrackingEvent>(); return _TrackingEvents; }
            private set { _TrackingEvents = value; }
        }
    }

    public sealed class CreativeCompanions : ICreative
    {
        private string _Id = string.Empty;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public int? Sequence { get; set; }

        private string _AdId = string.Empty;
        public string AdId
        {
            get { return _AdId; }
            set { _AdId = value; }
        }

        private IList<Companion> _Companions;
        public IList<Companion> Companions
        {
            get { if (_Companions == null) _Companions = new List<Companion>(); return _Companions; }
            private set { _Companions = value; }
        }

        public CompanionAdsRequired Required { get; set; }
    }

    public sealed class NonLinear
    {
        private IResource _Item;
        public IResource Item
        {
            get { return _Item; }
            set { _Item = value; }
        }

        private Uri _ClickThrough;
        public Uri ClickThrough
        {
            get { return _ClickThrough; }
            set { _ClickThrough = value; }
        }

        private string _AdParameters = string.Empty;
        public string AdParameters
        {
            get { return _AdParameters; }
            set { _AdParameters = value; }
        }

        private IList<object> _Extensions;
        public IList<object> Extensions
        {
            get { if (_Extensions == null) _Extensions = new List<object>(); return _Extensions; }
            private set { _Extensions = value; }
        }

        private Uri _Value;
        public Uri Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        private int _Width;
        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        private int _Height;
        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        public int? ExpandedWidth { get; set; }

        public int? ExpandedHeight { get; set; }

        public bool? Scalable { get; set; }

        public bool? MaintainAspectRatio { get; set; }

        public TimeSpan? MinSuggestedDuration { get; set; }        

        private string _ApiFramework = string.Empty;
        public string ApiFramework
        {
            get { return _ApiFramework; }
            set { _ApiFramework = value; }
        }

        private IList<string> _ClickTracking;
        public IList<string> ClickTracking
        {
            get { if (_ClickTracking == null) _ClickTracking = new List<string>(); return _ClickTracking; }
            private set { _ClickTracking = value; }
        }

        private string _Id = string.Empty;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
    }

    public sealed class Companion : ICompanionSource
    {
        private IResource _Item;
        public IResource Item
        {
            get { return _Item; }
            set { _Item = value; }
        }

        private IList<object> _Extensions;
        public IList<object> Extensions
        {
            get { if (_Extensions == null) _Extensions = new List<object>(); return _Extensions; }
            private set { _Extensions = value; }
        }

        private Uri _ClickThrough;
        public Uri ClickThrough
        {
            get { return _ClickThrough; }
            set { _ClickThrough = value; }
        }

        private string _AltText = string.Empty;
        public string AltText
        {
            get { return _AltText; }
            set { _AltText = value; }
        }

        private string _AdParameters = string.Empty;
        public string AdParameters
        {
            get { return _AdParameters; }
            set { _AdParameters = value; }
        }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public int? AssetWidth { get; set; }

        public int? AssetHeight { get; set; }

        public int? ExpandedWidth { get; set; }

        public int? ExpandedHeight { get; set; }

        private string _ApiFramework = string.Empty;
        public string ApiFramework
        {
            get { return _ApiFramework; }
            set { _ApiFramework = value; }
        }

        private string _AdSlotId = string.Empty;
        public string AdSlotId
        {
            get { return _AdSlotId; }
            set { _AdSlotId = value; }
        }

        private string _Id = string.Empty;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private IList<string> _ViewTracking;
        public IList<string> ViewTracking
        {
            get { if (_ViewTracking == null) _ViewTracking = new List<string>(); return _ViewTracking; }
            private set { _ViewTracking = value; }
        }

        private IList<string> _ClickTracking;
        public IList<string> ClickTracking
        {
            get { if (_ClickTracking == null) _ClickTracking = new List<string>(); return _ClickTracking; }
            private set { _ClickTracking = value; }
        }

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
        #region Required
        private string _Id = string.Empty;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private MediaFileDelivery _Delivery;
        public MediaFileDelivery Delivery
        {
            get { return _Delivery; }
            set { _Delivery = value; }
        }

        private string _Type = string.Empty;
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public Uri Value { get; set; }

        private int _Width;
        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        private int _Height;
        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }
        #endregion

        #region Optional
        public long? Bitrate { get; set; }

        public long? MinBitrate { get; set; }

        public long? MaxBitrate { get; set; }

        public bool? Scalable { get; set; }

        public bool? MaintainAspectRatio { get; set; }

        private string _ApiFramework = string.Empty;
        public string ApiFramework
        {
            get { return _ApiFramework; }
            set { _ApiFramework = value; }
        }

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

        private Uri _Value;
        public Uri Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
    }

    public sealed class HtmlResource : IResource
    {
        internal HtmlResource()
        { }

        private string _Value = string.Empty;
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
    }

    public sealed class StaticResource : IResource
    {
        internal StaticResource()
        { }

        private string _CreativeType = string.Empty;
        public string CreativeType
        {
            get { return _CreativeType; }
            set { _CreativeType = value; }
        }

        private Uri _Value;
        public Uri Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
    }

    public interface IResource
    { }

    public sealed class TrackingEvent
    {
        internal TrackingEvent()
        { }

        private TrackingType _Type;
        public TrackingType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private FlexibleOffset _Offset;
        public FlexibleOffset Offset
        {
            get { return _Offset; }
            set { _Offset = value; }
        }

        private string _Value = string.Empty;
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
    }

    public sealed class Icon
    {
        private IResource _Item;
        public IResource Item
        {
            get { return _Item; }
            set { _Item = value; }
        }

        private IList<string> _ClickTracking;
        public IList<string> ClickTracking
        {
            get { if (_ClickTracking == null) _ClickTracking = new List<string>(); return _ClickTracking; }
            private set { _ClickTracking = value; }
        }

        private Uri _ClickThrough;
        public Uri ClickThrough
        {
            get { return _ClickThrough; }
            set { _ClickThrough = value; }
        }

        private IList<string> _ViewTracking;
        public IList<string> ViewTracking
        {
            get { if (_ViewTracking == null) _ViewTracking = new List<string>(); return _ViewTracking; }
            private set { _ViewTracking = value; }
        }

        private string _Program = string.Empty;
        public string Program
        {
            get { return _Program; }
            set { _Program = value; }
        }

        public int? Width { get; set; }

        public int? Height { get; set; }

        private string _XPosition = string.Empty;  // allows "left", "right" or a pixel value
        public string XPosition
        {
            get { return _XPosition; }
            set { _XPosition = value; }
        }

        private string _YPosition = string.Empty;  // allows "top", "bottom", or a pixel value
        public string YPosition
        {
            get { return _YPosition; }
            set { _YPosition = value; }
        }

        public TimeSpan? Offset { get; set; }

        public TimeSpan? Duration { get; set; }

        private string _ApiFramework = string.Empty;
        public string ApiFramework
        {
            get { return _ApiFramework; }
            set { _ApiFramework = value; }
        }
    }

    public sealed class Extension
    {
        internal Extension()
        { }

        private string _Type = string.Empty;
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private string _Value = string.Empty;
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
    }

    public sealed class AdSystem
    {
        internal AdSystem()
        { }

        private string _Version = string.Empty;
        public string Version
        {
            get { return _Version; }
            set { _Version = value; }
        }

        private string _Value = string.Empty;
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
    }

    public sealed class Pricing
    {
        internal Pricing()
        { }

        private PricingModel _Model;
        public PricingModel Model
        {
            get { return _Model; }
            set { _Model = value; }
        }

        private string _Currency = string.Empty;
        public string Currency
        {
            get { return _Currency; }
            set { _Currency = value; }
        }

        private double _Value;
        public double Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
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
        private bool _IsAbsolute;
        public bool IsAbsolute
        {
            get { return _IsAbsolute; }
            set { _IsAbsolute = value; }
        }

        /// <summary>
        /// Gets or sets the relative offset as a percentage (0-1).
        /// </summary>
        private double _RelativeOffset;
        public double RelativeOffset
        {
            get { return _RelativeOffset; }
            set { _RelativeOffset = value; }
        }

        /// <summary>
        /// Gets or sets the absolute offset as a TimeSpan
        /// </summary>
        private TimeSpan _AbsoluteOffset;
        public TimeSpan AbsoluteOffset
        {
            get { return _AbsoluteOffset; }
            set { _AbsoluteOffset = value; }
        }
    }
}
