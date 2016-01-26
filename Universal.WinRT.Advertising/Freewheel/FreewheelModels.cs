using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Media.Advertising
{
    public sealed class FWAdResponse
    {
        private string _Version = string.Empty;
        public string Version
        {
            get { return _Version; }
            set { _Version = value; }
        }

        private string _CustomId = string.Empty;
        public string CustomId
        {
            get { return _CustomId; }
            set { _CustomId = value; }
        }

        public int? NetworkId { get; set; }

        private string _Diagnostic = string.Empty;
        public string Diagnostic
        {
            get { return _Diagnostic; }
            set { _Diagnostic = value; }
        }

        private string _CustomState = string.Empty;
        public string CustomState
        {
            get { return _CustomState; }
            set { _CustomState = value; }
        }

        private IList<FWParameter> _Parameters;
        public IList<FWParameter> Parameters
        {
            get { if (_Parameters == null) _Parameters = new List<FWParameter>(); return _Parameters; }
            private set { _Parameters = value; }
        }

        private IList<FWError> _Errors;
        public IList<FWError> Errors
        {
            get { if (_Errors == null) _Errors = new List<FWError>(); return _Errors; }
            private set { _Errors = value; }
        }

        private FWVisitor _Visitor;
        public FWVisitor Visitor
        {
            get { return _Visitor; }
            set { _Visitor = value; }
        }

        private IList<FWAd> _Ads;
        public IList<FWAd> Ads
        {
            get { if (_Ads == null) _Ads = new List<FWAd>(); return _Ads; }
            private set { _Ads = value; }
        }

        private IList<FWEventCallback> _EventCallbacks;
        public IList<FWEventCallback> EventCallbacks
        {
            get { if (_EventCallbacks == null) _EventCallbacks = new List<FWEventCallback>(); return _EventCallbacks; }
            private set { _EventCallbacks = value; }
        }

        private FWSiteSection _SiteSection;
        public FWSiteSection SiteSection
        {
            get { return _SiteSection; }
            set { _SiteSection = value; }
        }

        private string _RendererManifest = string.Empty;
        public string RendererManifest
        {
            get { return _RendererManifest; }
            set { _RendererManifest = value; }
        }

        private string _RendererManifestVersion = string.Empty;
        public string RendererManifestVersion
        {
            get { return _RendererManifestVersion; }
            set { _RendererManifestVersion = value; }
        }
    }

    public sealed class FWAd
    {
        private string _AdUnit = string.Empty;
        public string AdUnit
        {
            get { return _AdUnit; }
            set { _AdUnit = value; }
        }

        private string _Id = string.Empty;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private bool _NoPreload;
        public bool NoPreload
        {
            get { return _NoPreload; }
            set { _NoPreload = value; }
        }

        private bool _NoLoad;
        public bool NoLoad
        {
            get { return _NoLoad; }
            set { _NoLoad = value; }
        }

        private int _BundleId;
        public int BundleId
        {
            get { return _BundleId; }
            set { _BundleId = value; }
        }

        private IList<FWCreative> _Creatives;
        public IList<FWCreative> Creatives
        {
            get { if (_Creatives == null) _Creatives = new List<FWCreative>(); return _Creatives; }
            private set { _Creatives = value; }
        }
    }

    public sealed class FWParameter
    {
        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string Category { get; set; }
        public string Value { get; set; }
    }

    public sealed class FWCreative
    {
        private string _AdUnit = string.Empty;
        public string AdUnit
        {
            get { return _AdUnit; }
            set { _AdUnit = value; }
        }

        private FWBaseUnit _BaseUnit;
        public FWBaseUnit BaseUnit
        {
            get { return _BaseUnit; }
            set { _BaseUnit = value; }
        }

        public TimeSpan? Duration { get; set; }

        private string _Id = string.Empty;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private string _RedirectUrl = string.Empty;
        public string RedirectUrl
        {
            get { return _RedirectUrl; }
            set { _RedirectUrl = value; }
        }

        private IList<FWCreativeRendition> _CreativeRenditions;
        public IList<FWCreativeRendition> CreativeRenditions
        {
            get { if (_CreativeRenditions == null) _CreativeRenditions = new List<FWCreativeRendition>(); return _CreativeRenditions; }
            private set { _CreativeRenditions = value; }
        }

        private IList<FWParameter> _Parameters;
        public IList<FWParameter> Parameters
        {
            get { if (_Parameters == null) _Parameters = new List<FWParameter>(); return _Parameters; }
            private set { _Parameters = value; }
        }
    }

    public enum FWBaseUnit
    {
        still_image,
        still_image_click_to_content,
        linear_animation,
        linear_animation_click_to_content,
        fixed_size_interactive,
        fixed_size_interactive_click_to_content,
        video,
        video_click_to_content,
        text_ad,
        text_ad_click_to_content,
        text_ad_logo,
        text_ad_logo_click_to_content,
        expand_east_still_image,
        expand_west_still_image,
        expand_north_still_image,
        expand_south_still_image,
        expand_northwest_still_image,
        expand_northeast_still_image,
        expand_southwest_still_image,
        expand_southeast_still_image,
        expanding_interactive
    }

    public sealed class FWCreativeRendition
    {
        private FWCreativeApi _CreativeApi;
        public FWCreativeApi CreativeApi
        {
            get { return _CreativeApi; }
            set { _CreativeApi = value; }
        }

        private string _ContentType = string.Empty;
        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }

        private string _WrapperType = string.Empty;
        public string WrapperType
        {
            get { return _WrapperType; }
            set { _WrapperType = value; }
        }

        private string _WrapperUrl = string.Empty;
        public string WrapperUrl
        {
            get { return _WrapperUrl; }
            set { _WrapperUrl = value; }
        }

        private string _Id = string.Empty;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private int _Height;
        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        private int _Width;
        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        private string _AdReplicaId = string.Empty;
        public string AdReplicaId
        {
            get { return _AdReplicaId; }
            set { _AdReplicaId = value; }
        }

        private FWPreference _Preference;
        public FWPreference Preference
        {
            get { return _Preference; }
            set { _Preference = value; }
        }

        private FWAsset _Asset;
        public FWAsset Asset
        {
            get { return _Asset; }
            set { _Asset = value; }
        }

        private IList<FWAsset> _OtherAssets;
        public IList<FWAsset> OtherAssets
        {
            get { if (_OtherAssets == null) _OtherAssets = new List<FWAsset>(); return _OtherAssets; }
            private set { _OtherAssets = value; }
        }

        private IList<FWParameter> _Parameters;
        public IList<FWParameter> Parameters
        {
            get { if (_Parameters == null) _Parameters = new List<FWParameter>(); return _Parameters; }
            private set { _Parameters = value; }
        }
    }

    public enum FWPreference
    {
        StrongPreference = 10,
        Preference = 5,
        Neutral = 0,
        BiasAgainst = -5,
        StrongBiasAgainst = -10
    }

    public enum FWCreativeApi
    {
        none,
        vpaid,
        clickTag,
        youtube,
        freewheelOverlay
    }

    public sealed class FWAsset
    {
        private string _Content = string.Empty;
        public string Content
        {
            get { return _Content; }
            set { _Content = value; }
        }

        private string _Id = string.Empty;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private string _ContentType = string.Empty;
        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }

        private string _MimeType = string.Empty;
        public string MimeType
        {
            get { return _MimeType; }
            set { _MimeType = value; }
        }

        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Url = string.Empty;
        public string Url
        {
            get { return _Url; }
            set { _Url = value; }
        }
        public int Bytes { get; set; }
    }

    public sealed class FWSiteSection
    {
        internal FWSiteSection()
        {
            AdSlots = new List<FWAdSlot>();
        }

        private string _Id = string.Empty;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private string _CustomId = string.Empty;
        public string CustomId
        {
            get { return _CustomId; }
            set { _CustomId = value; }
        }

        private string _PageViewRandom = string.Empty;
        public string PageViewRandom
        {
            get { return _PageViewRandom; }
            set { _PageViewRandom = value; }
        }

        private FWVideoPlayer _VideoPlayer;
        public FWVideoPlayer VideoPlayer
        {
            get { return _VideoPlayer; }
            set { _VideoPlayer = value; }
        }

        private IList<FWAdSlot> _AdSlots;
        public IList<FWAdSlot> AdSlots
        {
            get { if (_AdSlots == null) _AdSlots = new List<FWAdSlot>(); return _AdSlots; }
            private set { _AdSlots = value; }
        }
    }

    public sealed class FWVideoPlayer
    {
        private FWVideoAsset _VideoAsset;
        public FWVideoAsset VideoAsset
        {
            get { return _VideoAsset; }
            set { _VideoAsset = value; }
        }

        private IList<FWAdSlot> _AdSlots;
        public IList<FWAdSlot> AdSlots
        {
            get { if (_AdSlots == null) _AdSlots = new List<FWAdSlot>(); return _AdSlots; }
            private set { _AdSlots = value; }
        }
    }

    public sealed class FWVideoAsset
    {
        private string _Id = string.Empty;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private string _CustomId = string.Empty;
        public string CustomId
        {
            get { return _CustomId; }
            set { _CustomId = value; }
        }

        private string _VideoPlayRandom = string.Empty;
        public string VideoPlayRandom
        {
            get { return _VideoPlayRandom; }
            set { _VideoPlayRandom = value; }
        }

        private IList<FWTemporalAdSlot> _AdSlots;
        public IList<FWTemporalAdSlot> AdSlots
        {
            get { if (_AdSlots == null) _AdSlots = new List<FWTemporalAdSlot>(); return _AdSlots; }
            private set { _AdSlots = value; }
        }

        private IList<FWEventCallback> _EventCallbacks;
        public IList<FWEventCallback> EventCallbacks
        {
            get { if (_EventCallbacks == null) _EventCallbacks = new List<FWEventCallback>(); return _EventCallbacks; }
            private set { _EventCallbacks = value; }
        }
    }

    public sealed class FWTemporalAdSlot : IFWAdSlot
    {
        public int? Height { get; set; }
        public int? Width { get; set; }

        private string _CompatibleDimensions = string.Empty;
        public string CompatibleDimensions
        {
            get { return _CompatibleDimensions; }
            set { _CompatibleDimensions = value; }
        }

        private string _Source = string.Empty;
        public string Source
        {
            get { return _Source; }
            set { _Source = value; }
        }

        private TimeSpan _MaxSlotDuration;
        public TimeSpan MaxSlotDuration
        {
            get { return _MaxSlotDuration; }
            set { _MaxSlotDuration = value; }
        }

        private string _TimePositionClass = string.Empty;
        public string TimePositionClass
        {
            get { return _TimePositionClass; }
            set { _TimePositionClass = value; }
        }

        private TimeSpan _TimePosition;
        public TimeSpan TimePosition
        {
            get { return _TimePosition; }
            set { _TimePosition = value; }
        }

        private int _TimePositionSequence;
        public int TimePositionSequence
        {
            get { return _TimePositionSequence; }
            set { _TimePositionSequence = value; }
        }

        private string _CustomId = string.Empty;
        public string CustomId
        {
            get { return _CustomId; }
            set { _CustomId = value; }
        }

        private string _AdUnit = string.Empty;
        public string AdUnit
        {
            get { return _AdUnit; }
            set { _AdUnit = value; }
        }

        private IList<FWAdReference> _SelectedAds;
        public IList<FWAdReference> SelectedAds
        {
            get { if (_SelectedAds == null) _SelectedAds = new List<FWAdReference>(); return _SelectedAds; }
            private set { _SelectedAds = value; }
        }

        private IList<FWEventCallback> _EventCallbacks;
        public IList<FWEventCallback> EventCallbacks
        {
            get { if (_EventCallbacks == null) _EventCallbacks = new List<FWEventCallback>(); return _EventCallbacks; }
            private set { _EventCallbacks = value; }
        }

        private IList<FWParameter> _Parameters;
        public IList<FWParameter> Parameters
        {
            get { if (_Parameters == null) _Parameters = new List<FWParameter>(); return _Parameters; }
            private set { _Parameters = value; }
        }
    }

    public sealed class FWNonTemporalAdSlot : IFWAdSlot
    {
        public int? Height { get; set; }
        public int? Width { get; set; }

        private string _CompatibleDimensions = string.Empty;
        public string CompatibleDimensions
        {
            get { return _CompatibleDimensions; }
            set { _CompatibleDimensions = value; }
        }

        private string _CustomId = string.Empty;
        public string CustomId
        {
            get { return _CustomId; }
            set { _CustomId = value; }
        }

        private string _AdUnit = string.Empty;
        public string AdUnit
        {
            get { return _AdUnit; }
            set { _AdUnit = value; }
        }

        private IList<FWAdReference> _SelectedAds;
        public IList<FWAdReference> SelectedAds
        {
            get { if (_SelectedAds == null) _SelectedAds = new List<FWAdReference>(); return _SelectedAds; }
            private set { _SelectedAds = value; }
        }

        private IList<FWParameter> _Parameters;
        public IList<FWParameter> Parameters
        {
            get { if (_Parameters == null) _Parameters = new List<FWParameter>(); return _Parameters; }
            private set { _Parameters = value; }
        }
    }

    public sealed class FWAdSlot : IFWAdSlot
    {
        public int? Height { get; set; }
        public int? Width { get; set; }

        private string _CompatibleDimensions = string.Empty;
        public string CompatibleDimensions
        {
            get { return _CompatibleDimensions; }
            set { _CompatibleDimensions = value; }
        }

        private string _CustomId = string.Empty;
        public string CustomId
        {
            get { return _CustomId; }
            set { _CustomId = value; }
        }

        private string _AdUnit = string.Empty;
        public string AdUnit
        {
            get { return _AdUnit; }
            set { _AdUnit = value; }
        }

        private IList<FWAdReference> _SelectedAds;
        public IList<FWAdReference> SelectedAds
        {
            get { if (_SelectedAds == null) _SelectedAds = new List<FWAdReference>(); return _SelectedAds; }
            private set { _SelectedAds = value; }
        }
    }

    public sealed class FWEventCallback
    {

        private static string _VideoView = "videoView";
        public static string VideoView
        {
            get { return _VideoView; }
            set { _VideoView = value; }
        }

        private static string _SlotImpression = "slotImpression";
        public static string SlotImpression
        {
            get { return _SlotImpression; }
            set { _SlotImpression = value; }
        }

        private static string _DefaultClick = "defaultClick";
        public static string DefaultClick
        {
            get { return _DefaultClick; }
            set { _DefaultClick = value; }
        }

        private static string _DefaultImpression = "defaultImpression";
        public static string DefaultImpression
        {
            get { return _DefaultImpression; }
            set { _DefaultImpression = value; }
        }

        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Url = string.Empty;
        public string Url
        {
            get { return _Url; }
            set { _Url = value; }
        }

        private FWCallbackType _Type;
        public FWCallbackType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        private bool _ShowBrowser;
        public bool ShowBrowser
        {
            get { return _ShowBrowser; }
            set { _ShowBrowser = value; }
        }

        private string _Use = string.Empty;
        public string Use
        {
            get { return _Use; }
            set { _Use = value; }
        }

        private IList<FWUrl> _TrackingUrls;
        public IList<FWUrl> TrackingUrls
        {
            get { if (_TrackingUrls == null) _TrackingUrls = new List<FWUrl>(); return _TrackingUrls; }
            private set { _TrackingUrls = value; }
        }

        public IEnumerable<string> GetUrls()
        {
            if (!string.IsNullOrEmpty(Url)) yield return Url;
            foreach (var url in TrackingUrls.Select(u => u.Value))
            {
                yield return url;
            }
        }
    }

    public sealed class FWUrl
    {
        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Value = string.Empty;
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
    }

    public enum FWCallbackType
    {
        Impression,
        Click,
        Action,
        Standard,
        Generic
    }

    public sealed class FWAdReference
    {
        private string _AdId = string.Empty;
        public string AdId
        {
            get { return _AdId; }
            set { _AdId = value; }
        }

        private string _CreativeId = string.Empty;
        public string CreativeId
        {
            get { return _CreativeId; }
            set { _CreativeId = value; }
        }

        private string _CreativeRenditionId = string.Empty;
        public string CreativeRenditionId
        {
            get { return _CreativeRenditionId; }
            set { _CreativeRenditionId = value; }
        }

        private string _ReplicaId = string.Empty;
        public string ReplicaId
        {
            get { return _ReplicaId; }
            set { _ReplicaId = value; }
        }

        private string _SlotEnv = string.Empty;
        public string SlotEnv
        {
            get { return _SlotEnv; }
            set { _SlotEnv = value; }
        }

        private string _SlotId = string.Empty;
        public string SlotId
        {
            get { return _SlotId; }
            set { _SlotId = value; }
        }

        private IList<FWEventCallback> _EventCallbacks;
        public IList<FWEventCallback> EventCallbacks
        {
            get { if (_EventCallbacks == null) _EventCallbacks = new List<FWEventCallback>(); return _EventCallbacks; }
            private set { _EventCallbacks = value; }
        }

        private IList<FWAdReference> _CompanionAds;
        public IList<FWAdReference> CompanionAds
        {
            get { if (_CompanionAds == null) _CompanionAds = new List<FWAdReference>(); return _CompanionAds; }
            private set { _CompanionAds = value; }
        }

        private IList<FWAdReference> _FallbackAds;
        public IList<FWAdReference> FallbackAds
        {
            get { if (_FallbackAds == null) _FallbackAds = new List<FWAdReference>(); return _FallbackAds; }
            private set { _FallbackAds = value; }
        }
    }

    public sealed class FWVisitor
    {
        private string _CustomId = string.Empty;
        public string CustomId
        {
            get { return _CustomId; }
            set { _CustomId = value; }
        }

        private IList<FWHttpHeader> _HttpHeaders;
        public IList<FWHttpHeader> HttpHeaders
        {
            get { if (_HttpHeaders == null) _HttpHeaders = new List<FWHttpHeader>(); return _HttpHeaders; }
            private set { _HttpHeaders = value; }
        }
        public FWState State { get; set; }
    }

    public sealed class FWState
    {
        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Value = string.Empty;
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
    }

    public sealed class FWHttpHeader
    {

        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public string Value { get; set; }
    }

    public sealed class FWError
    {
        private string _Severity = string.Empty;
        public string Severity
        {
            get { return _Severity; }
            set { _Severity = value; }
        }

        private string _Name = string.Empty;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        private string _Id = string.Empty;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        public string Context { get; set; }
    }

    public interface IFWAdSlot
    {
        IList<FWAdReference> SelectedAds { get; }
        string CustomId { get; set; }
        string AdUnit { get; set; }
        int? Height { get; set; }
        int? Width { get; set; }
        string CompatibleDimensions { get; set; }
    }

}
