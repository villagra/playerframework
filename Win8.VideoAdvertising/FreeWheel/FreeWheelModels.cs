using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VideoAdvertising
{
    public sealed class FWAdResponse
    {
        internal FWAdResponse()
        {
            Errors = new List<FWError>();
            Ads = new List<FWAd>();
            EventCallbacks = new List<FWEventCallback>();
            Parameters = new List<FWParameter>();
        }

        public string Version { get; set; }
        public string CustomId { get; set; }
        public int? NetworkId { get; set; }
        public string Diagnostic { get; set; }
        public string CustomState { get; set; }
        public IList<FWParameter> Parameters { get; private set; }
        public IList<FWError> Errors { get; private set; }
        public FWVisitor Visitor { get; set; }
        public IList<FWAd> Ads { get; private set; }
        public IList<FWEventCallback> EventCallbacks { get; private set; }
        public FWSiteSection SiteSection { get; set; }
        public string RendererManifest { get; set; }
        public string RendererManifestVersion { get; set; }
    }

    public sealed class FWAd
    {
        internal FWAd()
        {
            Creatives = new List<FWCreative>();
        }

        public string AdUnit { get; set; }
        public string Id { get; set; }
        public bool NoPreload { get; set; }
        public bool NoLoad { get; set; }
        public int BundleId { get; set; }
        public IList<FWCreative> Creatives { get; private set; }
    }

    public sealed class FWParameter
    {
        internal FWParameter()
        { }

        public string Name { get; set; }
        public string Category { get; set; }
    }

    public sealed class FWCreative
    {
        internal FWCreative()
        {
            CreativeRenditions = new List<FWCreativeRendition>();
            Parameters = new List<FWParameter>();
        }

        public string AdUnit { get; set; }
        public FWBaseUnit BaseUnit { get; set; }
        public TimeSpan? Duration { get; set; }
        public string Id { get; set; }
        public string RedirectUrl { get; set; }
        public IList<FWCreativeRendition> CreativeRenditions { get; private set; }
        public IList<FWParameter> Parameters { get; private set; }
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
        internal FWCreativeRendition()
        {
            OtherAssets = new List<FWAsset>();
            Parameters = new List<FWParameter>();
        }

        public FWCreativeApi CreativeApi { get; set; }
        public string ContentType { get; set; }
        public string WrapperType { get; set; }
        public string WrapperUrl { get; set; }
        public string Id { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public string AdReplicaId { get; set; }
        public FWPreference Preference { get; set; }
        public FWAsset Asset { get; set; }
        public IList<FWAsset> OtherAssets { get; private set; }
        public IList<FWParameter> Parameters { get; private set; }
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
        internal FWAsset()
        { }

        public string Content { get; set; }
        public string Id { get; set; }
        public string ContentType { get; set; }
        public string MimeType { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public int Bytes { get; set; }
    }

    public sealed class FWSiteSection
    {
        internal FWSiteSection()
        {
            AdSlots = new List<FWAdSlot>();
        }

        public string Id { get; set; }
        public string CustomId { get; set; }
        public string PageViewRandom { get; set; }
        public FWVideoPlayer VideoPlayer { get; set; }
        public IList<FWAdSlot> AdSlots { get; set; }
    }

    public sealed class FWVideoPlayer
    {
        internal FWVideoPlayer()
        {
            AdSlots = new List<FWAdSlot>();
        }

        public FWVideoAsset VideoAsset { get; set; }
        public IList<FWAdSlot> AdSlots { get; private set; }
    }

    public sealed class FWVideoAsset
    {
        internal FWVideoAsset()
        {
            AdSlots = new List<FWTemporalAdSlot>();
            EventCallbacks = new List<FWEventCallback>();
        }

        public string Id { get; set; }
        public string CustomId { get; set; }
        public string VideoPlayRandom { get; set; }
        public IList<FWTemporalAdSlot> AdSlots { get; private set; }
        public IList<FWEventCallback> EventCallbacks { get; private set; }
    }

    public sealed class FWTemporalAdSlot : IFWAdSlot
    {
        internal FWTemporalAdSlot()
        {
            SelectedAds = new List<FWAdReference>();
            EventCallbacks = new List<FWEventCallback>();
            Parameters = new List<FWParameter>();
        }

        public int? Height { get; set; }
        public int? Width { get; set; }
        public string CompatibleDimensions { get; set; }
        public string Source { get; set; }
        public TimeSpan MaxSlotDuration { get; set; }
        public string TimePositionClass { get; set; }
        public TimeSpan TimePosition { get; set; }
        public int TimePositionSequence { get; set; }

        public string CustomId { get; set; }
        public string AdUnit { get; set; }
        public IList<FWAdReference> SelectedAds { get; private set; }
        public IList<FWEventCallback> EventCallbacks { get; private set; }
        public IList<FWParameter> Parameters { get; private set; }
    }

    public sealed class FWNonTemporalAdSlot : IFWAdSlot
    {
        internal FWNonTemporalAdSlot()
        {
            SelectedAds = new List<FWAdReference>();
            Parameters = new List<FWParameter>();
        }

        public int? Height { get; set; }
        public int? Width { get; set; }
        public string CompatibleDimensions { get; set; }
        public string CustomId { get; set; }
        public string AdUnit { get; set; }
        public IList<FWAdReference> SelectedAds { get; private set; }
        public IList<FWParameter> Parameters { get; private set; }
    }

    public sealed class FWAdSlot : IFWAdSlot
    {
        internal FWAdSlot()
        {
            SelectedAds = new List<FWAdReference>();
        }

        public int? Height { get; set; }
        public int? Width { get; set; }
        public string CompatibleDimensions { get; set; }
        public string CustomId { get; set; }
        public string AdUnit { get; set; }
        public IList<FWAdReference> SelectedAds { get; private set; }
    }

    public sealed class FWEventCallback
    {
        public static string VideoView { get; private set; }
        public static string SlotImpression { get; private set; }
        public static string DefaultClick { get; private set; }
        public static string DefaultImpression { get; private set; }

        static FWEventCallback()
        {
            VideoView = "videoView";
            SlotImpression = "slotImpression";
            DefaultClick = "defaultClick";
            DefaultImpression = "defaultImpression";
        }

        internal FWEventCallback()
        {
            TrackingUrls = new List<FWUrl>();
        }

        public string Name { get; set; }
        public string Url { get; set; }
        public FWCallbackType Type { get; set; }
        public bool ShowBrowser { get; set; }
        public string Use { get; set; }
        public IList<FWUrl> TrackingUrls { get; private set; }

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
        internal FWUrl()
        { }

        public string Name { get; set; }
        public string Value { get; set; }
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
        internal FWAdReference()
        {
            EventCallbacks = new List<FWEventCallback>();
            CompanionAds = new List<FWAdReference>();
            FallbackAds = new List<FWAdReference>();
        }

        public string AdId { get; set; }
        public string CreativeId { get; set; }
        public string CreativeRenditionId { get; set; }
        public string ReplicaId { get; set; }
        public string SlotEnv { get; set; }
        public string SlotId { get; set; }
        public IList<FWEventCallback> EventCallbacks { get; private set; }
        public IList<FWAdReference> CompanionAds { get; private set; }
        public IList<FWAdReference> FallbackAds { get; private set; }
    }

    public sealed class FWVisitor
    {
        internal FWVisitor()
        {
            HttpHeaders = new List<FWHttpHeader>();
        }

        public string CustomId { get; set; }
        public IList<FWHttpHeader> HttpHeaders { get; private set; }
        public FWState State { get; set; }
    }

    public sealed class FWState
    {
        internal FWState()
        { }

        public string Name { get; set; }
        public string Value { get; set; }
    }

    public sealed class FWHttpHeader
    {
        internal FWHttpHeader()
        { }

        public string Name { get; set; }
        public string Value { get; set; }
    }

    public sealed class FWError
    {
        internal FWError()
        { }

        public string Severity { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
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
