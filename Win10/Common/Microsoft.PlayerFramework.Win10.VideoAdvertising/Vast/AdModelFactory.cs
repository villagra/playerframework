using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Threading;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace Microsoft.VideoAdvertising
{
    public static partial class AdModelFactory
    {
        public static AdDocumentPayload CreateAdDocumentPayload()
        {
            return new AdDocumentPayload();
        }

        public static AdPod CreateAdPod()
        {
            return new AdPod();
        }

        public static Ad CreateAd()
        {
            return new Ad();
        }

        public static CreativeLinear CreateCreativeLinear()
        {
            return new CreativeLinear();
        }

        public static CreativeNonLinears CreateCreativeNonLinears()
        {
            return new CreativeNonLinears();
        }

        public static CreativeCompanions CreateCreativeCompanions()
        {
            return new CreativeCompanions();
        }

        public static NonLinear CreateNonLinear()
        {
            return new NonLinear();
        }

        public static Companion CreateCompanion()
        {
            return new Companion();
        }

        public static MediaFile CreateMediaFile()
        {
            return new MediaFile();
        }

        public static IFrameResource CreateIFrameResource()
        {
            return new IFrameResource();
        }

        public static HtmlResource CreateHtmlResource()
        {
            return new HtmlResource();
        }

        public static StaticResource CreateStaticResource()
        {
            return new StaticResource();
        }

        public static TrackingEvent CreateTrackingEvent()
        {
            return new TrackingEvent();
        }

        public static Icon CreateIcon()
        {
            return new Icon();
        }

        public static Extension CreateExtension()
        {
            return new Extension();
        }

        public static AdSystem CreateAdSystem()
        {
            return new AdSystem();
        }

        public static Pricing CreatePricing()
        {
            return new Pricing();
        }

        /// <summary>
        /// Provides a fallback ad pod to use in the event that a wrapper does not return an ad.
        /// </summary>
        public static AdPod FallbackAdPod { get; set; }

        public static IAsyncOperation<AdDocumentPayload> LoadSource(Uri source, int? maxRedirectDepth, bool allowMultipleAds)
        {
            return AsyncInfo.Run(c => InternalLoadSource(source, maxRedirectDepth, allowMultipleAds, c));
        }

        static async Task<AdDocumentPayload> InternalLoadSource(Uri source, int? maxRedirectDepth, bool allowMultipleAds, CancellationToken cancellationToken)
        {
            using (var stream = await Extensions.LoadStreamAsync(source))
            {
                return await Task.Run(() => CreateFromVast(stream, maxRedirectDepth, allowMultipleAds), cancellationToken);
            }
        }

        public static IAsyncOperation<AdDocumentPayload> CreateFromVast(IInputStream stream, int? maxRedirectDepth, bool allowMultipleAds)
        {
            return AsyncInfo.Run(c => CreateFromVast(stream.AsStreamForRead(), maxRedirectDepth, allowMultipleAds));
        }

        internal static async Task<AdDocumentPayload> CreateFromVast(Stream stream, int? maxRedirectDepth, bool allowMultipleAds)
        {
            XDocument xDoc = XDocument.Load(stream);
            XElement vastRoot = xDoc.Element("VAST");
            if (vastRoot == null)
            {
                vastRoot = xDoc.Element("VideoAdServingTemplate");
                if (vastRoot == null) throw new NotImplementedException();
                return await CreateFromVast1(vastRoot, maxRedirectDepth, allowMultipleAds);
            }
            else
            {
                var result = new AdDocumentPayload();
                result.Version = (string)vastRoot.Attribute("version");
                result.Error = (string)vastRoot.Element("Error");

                var eligableAds = vastRoot.Elements("Ad");
                if (!allowMultipleAds)
                {
                    eligableAds = eligableAds.Where(va => string.IsNullOrEmpty((string)va.Attribute("sequence")));
                }
                foreach (var vastAdPod in eligableAds.GroupBy(va => va.Attribute("sequence") != null ? 1 : int.MaxValue).OrderBy(vap => vap.Key))
                {
                    var adPod = new AdPod();
                    foreach (var vastAd in vastAdPod.OrderBy(va => ToNullableInt((string)va.Attribute("sequence")).GetValueOrDefault(0)))
                    {
                        var ad = new Ad();
                        ad.Id = (string)vastAd.Attribute("id");

                        if (vastAd.Elements("InLine").Any())
                        {
                            var vastAdInline = vastAd.Element("InLine");

                            ad.AdSystem = GetAdSystem(vastAdInline.Element("AdSystem"));

                            ad.Advertiser = (string)vastAdInline.Element("Advertiser");
                            ad.Description = (string)vastAdInline.Element("Description");
                            var error = (string)vastAdInline.Element("Error");
                            if (error != null) ad.Errors.Add(error);
                            ad.Title = (string)vastAdInline.Element("AdTitle");
                            ad.Survey = GetUriValue(vastAdInline.Element("Survey"));

                            ad.Pricing = new Pricing();
                            var pricing = vastAdInline.Element("Pricing");
                            if (pricing != null)
                            {
                                ad.Pricing.Currency = (string)pricing.Attribute("currency");
                                ad.Pricing.Model = (PricingModel)Enum.Parse(typeof(PricingModel), (string)pricing.Attribute("model"), true);
                                ad.Pricing.Value = Convert.ToDouble((string)pricing);
                            }

                            foreach (var vastImpression in vastAdInline.Elements("Impression"))
                            {
                                ad.Impressions.Add((string)vastImpression);
                            }

                            if (vastAdInline.Elements("Extensions").Any())
                            {
                                foreach (var vastExtension in vastAdInline.Element("Extensions").Elements("Extension"))
                                {
                                    ad.Extensions.Add(new Extension()); // TODO
                                }
                            }

                            LoadCreatives(vastAdInline, ad);

                            adPod.Ads.Add(ad);
                        }
                        else if (vastAd.Elements("Wrapper").Any())
                        {
                            Ad wrapper = new Ad();
                            var vastAdWrapper = vastAd.Element("Wrapper");

                            // parse the wrapper itself
                            wrapper.AdSystem = GetAdSystem(vastAdWrapper.Element("AdSystem"));
                            var error = (string)vastAdWrapper.Element("Error");
                            if (error != null) wrapper.Errors.Add(error);

                            foreach (var vastImpression in vastAdWrapper.Elements("Impression"))
                            {
                                wrapper.Impressions.Add((string)vastImpression);
                            }

                            LoadCreatives(vastAdWrapper, wrapper);

                            if (vastAdWrapper.Elements("Extensions").Any())
                            {
                                foreach (var vastExtension in vastAdWrapper.Element("Extensions").Elements("Extension"))
                                {
                                    ad.Extensions.Add(new Extension()); // TODO
                                }
                            }

                            AdDocumentPayload wrappedVastDoc = null;
                            var vastAdUri = GetUriValue(vastAdWrapper.Element("VASTAdTagURI"));
                            if (vastAdUri != null && (!maxRedirectDepth.HasValue || maxRedirectDepth.Value > 0))
                            {
                                try
                                {
                                    // load the stream from the web
                                    using (var s = await Extensions.LoadStreamAsync(vastAdUri))
                                    {
                                        var newAllowMultipleAds = vastAdWrapper.GetBoolAttribute("allowMultipleAds", allowMultipleAds);
                                        var followAdditionalWrappers = vastAdWrapper.GetBoolAttribute("followAdditionalWrappers", true);
                                        int? nextMaxRedirectDepth = followAdditionalWrappers ? (maxRedirectDepth.HasValue ? maxRedirectDepth.Value - 1 : maxRedirectDepth) : 0;
                                        wrappedVastDoc = await CreateFromVast(s, nextMaxRedirectDepth, newAllowMultipleAds);
                                    }
                                }
                                catch { /* swallow */ }
                            }

                            AdPod wrappedAdPod = null;
                            if (wrappedVastDoc != null)
                            {
                                wrappedAdPod = wrappedVastDoc.AdPods.FirstOrDefault();
                            }

                            if (wrappedAdPod == null || !wrappedAdPod.Ads.Any())
                            {
                                // no ads were returned
                                var fallbackOnNoAd = vastAdWrapper.GetBoolAttribute("fallbackOnNoAd", true);
                                if (fallbackOnNoAd)
                                {
                                    wrappedAdPod = FallbackAdPod;
                                }
                            }

                            if (wrappedAdPod != null)
                            {
                                // merge tracking info from this wrapper to every ad in the first adpod in the child
                                foreach (Ad inlineAd in wrappedAdPod.Ads)
                                    MergeWrappedAd(wrapper, inlineAd);

                                // add each ad from the first adpod in the child to the current adpod
                                foreach (Ad inlineAd in wrappedAdPod.Ads)
                                    adPod.Ads.Add(inlineAd);
                            }
                        }
                    }
                    result.AdPods.Add(adPod);
                }
                return result;
            }
        }

        internal static void MergeWrappedAd(Ad source, Ad dest)
        {
            // carry over impressions
            MergeWrappedAdBeacons(source, dest);

            // carry over tracking events
            foreach (ICreative destCreative in dest.Creatives)
            {
                ICreative sourceCreative = FindMatchingCreative(dest, destCreative, source);
                if (sourceCreative != null)
                {
                    MergeWrappedCreative(sourceCreative, destCreative);
                }
            }
        }

        internal static void MergeWrappedAdBeacons(Ad source, Ad dest)
        {
            foreach (var impression in source.Impressions)
                dest.Impressions.Add(impression);

            foreach (var error in source.Errors)
                dest.Errors.Add(error);
        }

        internal static void MergeWrappedCreative(ICreative source, ICreative dest)
        {
            if (dest is CreativeLinear)
            {
                var linearWrapper = source as CreativeLinear;
                if (linearWrapper != null)
                {
                    var linear = (CreativeLinear)dest;

                    // apply TrackingEvents
                    foreach (var t in linearWrapper.TrackingEvents)
                        linear.TrackingEvents.Add(t);

                    foreach (var clicks in linearWrapper.ClickTracking)
                        linear.ClickTracking.Add(clicks);

                    foreach (var clicks in linearWrapper.CustomClick)
                        linear.CustomClick.Add(clicks);

                    // only add icons of unique program types since VAST 3.0 calls for conflicts to favor the deepest ad in the chain.
                    foreach (var icon in linearWrapper.Icons.Where(wi => !linear.Icons.Any(i => i.Program == wi.Program)))
                        linear.Icons.Add(icon);

                    // pass along the media files
                    foreach (var media in linearWrapper.MediaFiles)
                        linear.MediaFiles.Add(media);

                    // use the click through url from the wrapped doc
                    if (linearWrapper.ClickThrough != null)
                        linear.ClickThrough = linearWrapper.ClickThrough;

                    linear.Id = linearWrapper.Id;
                    linear.AdId = linearWrapper.AdId;
                    if (linear.AdParameters == null) linear.AdParameters = linearWrapper.AdParameters;
                    linear.Extensions.AddRange(linearWrapper.Extensions);
                    linear.SkipOffset = linearWrapper.SkipOffset;
                    linear.Duration = linearWrapper.Duration;
                }
            }
            else if (dest is CreativeNonLinears)
            {
                var nonLinearsWrapper = source as CreativeNonLinears;
                if (nonLinearsWrapper != null)
                {
                    var nonLinears = (CreativeNonLinears)dest;

                    // apply TrackingEvents
                    foreach (var t in nonLinearsWrapper.TrackingEvents)
                        nonLinears.TrackingEvents.Add(t);

                    // apply ClickTracking to nonlinear children
                    foreach (var nonLinearObj in nonLinears.NonLinears)
                    {
                        var nonlinearWrapperObj = nonLinearsWrapper.NonLinears.FirstOrDefault(nl => nl.Id == nonLinearObj.Id);
                        if (nonlinearWrapperObj == null)
                        {
                            nonlinearWrapperObj = nonLinearsWrapper.NonLinears.FirstOrDefault();
                        }
                        if (nonlinearWrapperObj != null)
                        {
                            foreach (var clicks in nonlinearWrapperObj.ClickTracking)
                                nonLinearObj.ClickTracking.Add(clicks);

                            nonLinearObj.MinSuggestedDuration = nonlinearWrapperObj.MinSuggestedDuration;
                        }
                    }
                }
            }
            else if (dest is CreativeCompanions)
            {
                var companions = (CreativeCompanions)dest;
                var companionsWrapper = (CreativeCompanions)source;

                foreach (var companionObj in companions.Companions.ToList()) // use .ToList() to allow it to be updated while being iterated
                {
                    var companionWrapperObj = companionsWrapper.Companions.FirstOrDefault(c => c.Id == companionObj.Id);
                    if (companionWrapperObj.Item != null)
                    {
                        // the companion is a filled out companion, add instead of trying to merge.
                        companions.Companions.Add(companionWrapperObj);
                    }
                    else
                    {
                        if (companionWrapperObj == null)
                        {
                            companionWrapperObj = companionsWrapper.Companions.FirstOrDefault(c => c.Width == companionObj.Width && c.Height == companionObj.Height);
                        }
                        if (companionWrapperObj != null)
                        {
                            // apply TrackingEvents
                            foreach (var t in companionWrapperObj.ViewTracking)
                                companionObj.ViewTracking.Add(t);

                            // apply ClickTracking
                            foreach (var clicks in companionWrapperObj.ClickTracking)
                                companionObj.ClickTracking.Add(clicks);
                        }
                    }
                }

            }
        }

        private static ICreative FindMatchingCreative(Ad ad, ICreative creative, Ad wrapperAd)
        {
            Type type = creative.GetType();

            var appropriateCreatives = ad.Creatives.Where(c => c.GetType() == type).ToList();
            var appropriateWrapperCreatives = wrapperAd.Creatives.Where(c => c.GetType() == type).ToList();

            int index = appropriateCreatives.IndexOf(creative);

            if (appropriateWrapperCreatives.Count > index)
                return appropriateWrapperCreatives.ElementAt(index);
            else
                return null;
        }

        internal static ICreative FindMatchingCreative(ICreative creative, Ad searchAd)
        {
            var type = creative.GetType();
            return searchAd.Creatives.Where(c => c.GetType() == type).FirstOrDefault();
        }

        private static void LoadCreatives(XElement adElement, Ad ad)
        {
            var vastCreatives = adElement.Element("Creatives");
            if (vastCreatives != null)
            {
                foreach (var vastCreative in vastCreatives.Elements("Creative"))
                {
                    var vastItem = vastCreative.Elements().FirstOrDefault();
                    if (vastItem != null)
                    {
                        ICreative creative = null;
                        switch (vastItem.Name.LocalName)
                        {
                            case "Linear":
                                var vastLinear = vastItem;
                                var linearCreative = new CreativeLinear();
                                creative = linearCreative;

                                foreach (var trackingEvent in GetTrackingEvents(vastLinear))
                                    linearCreative.TrackingEvents.Add(trackingEvent);

                                if (vastLinear.Elements("AdParameters").Any())
                                {
                                    var xmlEncoded = (bool?)vastLinear.Element("AdParameters").Attribute("xmlEncoded") ?? false;
                                    linearCreative.AdParameters = xmlEncoded ? XmlDecode((string)vastLinear.Element("AdParameters")) : (string)vastLinear.Element("AdParameters");
                                }

                                LoadVideoClicks(vastLinear, linearCreative);

                                if (vastLinear.Elements("CreativeExtensions").Any())
                                {
                                    foreach (var vastExtension in vastLinear.Element("CreativeExtensions").Elements("CreativeExtension"))
                                    {
                                        linearCreative.Extensions.Add(new Extension()); // TODO
                                    }
                                }

                                linearCreative.Duration = ToNullableTimeSpan((string)vastLinear.Element("Duration"));
                                linearCreative.SkipOffset = FlexibleOffset.Parse((string)vastLinear.Attribute("skipoffset"));

                                var vastIcons = vastLinear.Element("Icons");
                                if (vastIcons != null)
                                {
                                    foreach (var vastIcon in vastIcons.Elements("Icon"))
                                    {
                                        var icon = new Icon();

                                        if (vastIcon.Elements("IconClicks").Any())
                                        {
                                            var iconClicks = vastIcon.Element("IconClicks");

                                            icon.ClickThrough = GetUriValue(iconClicks.Element("IconClickThrough"));

                                            foreach (var clickTracking in iconClicks.Elements("IconClickTracking"))
                                            {
                                                icon.ClickTracking.Add((string)clickTracking);
                                            }
                                        }

                                        icon.ApiFramework = (string)vastIcon.Attribute("apiFramework") ?? string.Empty;
                                        icon.Duration = ToNullableTimeSpan((string)vastIcon.Attribute("duration")) ?? new TimeSpan?();
                                        icon.Height = (int?)vastIcon.Attribute("height");
                                        icon.Width = (int?)vastIcon.Attribute("width");
                                        icon.Offset = ToNullableTimeSpan((string)vastIcon.Attribute("offset")) ?? new TimeSpan?();
                                        icon.Program = (string)vastIcon.Attribute("program") ?? string.Empty;
                                        icon.XPosition = (string)vastIcon.Attribute("xPosition") ?? string.Empty;
                                        icon.YPosition = (string)vastIcon.Attribute("yPosition") ?? string.Empty;
                                        icon.Item = GetResources(vastIcon).FirstOrDefault();
                                        foreach (var viewTrackingUrl in vastIcon.Elements("IconViewTracking"))
                                        {
                                            icon.ViewTracking.Add((string)viewTrackingUrl);
                                        }
                                        linearCreative.Icons.Add(icon);
                                    }
                                }

                                if (vastLinear.Elements("MediaFiles").Any())
                                {
                                    foreach (var vastMedia in vastLinear.Element("MediaFiles").Elements("MediaFile"))
                                    {
                                        var mediaFile = new MediaFile();
                                        mediaFile.ApiFramework = (string)vastMedia.Attribute("apiFramework") ?? string.Empty;
                                        mediaFile.Bitrate = ToNullableLong((string)vastMedia.Attribute("bitrate"));
                                        mediaFile.Codec = (string)vastMedia.Attribute("codec") ?? string.Empty;
                                        mediaFile.Delivery = (MediaFileDelivery)Enum.Parse(typeof(MediaFileDelivery), (string)vastMedia.Attribute("delivery"), true);
                                        mediaFile.Height = (int?)vastMedia.Attribute("height") ?? 0;
                                        mediaFile.Width = (int?)vastMedia.Attribute("width") ?? 0;
                                        mediaFile.Id = (string)vastMedia.Attribute("id");
                                        mediaFile.MaintainAspectRatio = (bool?)vastMedia.Attribute("maintainAspectRatio") ?? false;
                                        mediaFile.MaxBitrate = (long?)vastMedia.Attribute("maxBitrate") ?? 0;
                                        mediaFile.MinBitrate = (long?)vastMedia.Attribute("minBitrate") ?? 0;
                                        mediaFile.Scalable = (bool?)vastMedia.Attribute("scalable") ?? false;
                                        mediaFile.Type = (string)vastMedia.Attribute("type");
                                        mediaFile.Value = GetUriValue(vastMedia);
                                        linearCreative.MediaFiles.Add(mediaFile);
                                    }
                                }
                                break;
                            case "CompanionAds":
                                var vastCompanions = vastItem;
                                creative = LoadCompanions(vastCompanions);
                                break;
                            case "NonLinearAds":
                                var vastNonLinears = vastItem;
                                var nonLinearCreative = new CreativeNonLinears();
                                creative = nonLinearCreative;

                                foreach (var trackingEvent in GetTrackingEvents(vastNonLinears))
                                    nonLinearCreative.TrackingEvents.Add(trackingEvent);

                                foreach (var vastNonLinear in vastNonLinears.Elements("NonLinear"))
                                {
                                    var nonLinear = new NonLinear();

                                    if (vastNonLinear.Elements("AdParameters").Any())
                                    {
                                        var xmlEncoded = (bool?)vastNonLinear.Element("AdParameters").Attribute("xmlEncoded") ?? false;
                                        nonLinear.AdParameters = xmlEncoded ? XmlDecode((string)vastNonLinear.Element("AdParameters")) : (string)vastNonLinear.Element("AdParameters");
                                    };
                                    nonLinear.ApiFramework = (string)vastNonLinear.Attribute("apiFramework");
                                    nonLinear.ClickThrough = GetUriValue(vastNonLinear.Element("NonLinearClickThrough"));
                                    foreach (var vastTracking in vastNonLinear.Elements("NonLinearClickTracking"))
                                    {
                                        nonLinear.ClickTracking.Add((string)vastTracking);
                                    }

                                    if (vastNonLinear.Elements("CreativeExtensions").Any())
                                    {
                                        foreach (var vastExtension in vastNonLinear.Element("CreativeExtensions").Elements("CreativeExtension"))
                                        {
                                            nonLinear.Extensions.Add(new Extension()); // TODO
                                        }
                                    }
                                    nonLinear.ExpandedHeight = (int?)vastNonLinear.Attribute("expandedHeight");
                                    nonLinear.ExpandedWidth = (int?)vastNonLinear.Attribute("expandedWidth");
                                    nonLinear.Height = (int?)vastNonLinear.Attribute("height") ?? 0;
                                    nonLinear.Width = (int?)vastNonLinear.Attribute("width") ?? 0;
                                    nonLinear.MaintainAspectRatio = (bool?)vastNonLinear.Attribute("maintainAspectRatio") ?? false;
                                    nonLinear.MinSuggestedDuration = ToNullableTimeSpan((string)vastNonLinear.Attribute("minSuggestedDuration")) ?? new TimeSpan?();
                                    nonLinear.Scalable = (bool?)vastNonLinear.Attribute("scalable") ?? false;
                                    nonLinear.Item = GetResources(vastNonLinear).FirstOrDefault();
                                    nonLinearCreative.NonLinears.Add(nonLinear);
                                }
                                break;
                        }
                        if (creative != null)
                        {
                            creative.AdId = (string)vastCreative.Attribute("AdID");
                            creative.Id = (string)vastCreative.Attribute("id");
                            creative.Sequence = ToNullableInt((string)vastCreative.Attribute("sequence"));
                            ad.Creatives.Add(creative);
                        }
                    }
                }
            }
        }

        private static IEnumerable<IResource> GetResources(XElement vastNonLinear)
        {
            foreach (var vastStaticResource in vastNonLinear.Elements("StaticResource"))
            {
                yield return new StaticResource()
                {
                    CreativeType = (string)vastStaticResource.Attribute("creativeType") ?? string.Empty,
                    Value = GetUriValue(vastStaticResource)
                };
            }

            foreach (var vastHtmlResource in vastNonLinear.Elements("HTMLResource"))
            {
                var xmlEncoded = (bool?)vastHtmlResource.Attribute("xmlEncoded") ?? false;
                yield return new HtmlResource()
                {
                    Value = (xmlEncoded ? XmlDecode((string)vastHtmlResource) : (string)vastHtmlResource) ?? string.Empty
                };
            }

            foreach (var vastIFrameResource in vastNonLinear.Elements("IFrameResource"))
            {
                yield return new IFrameResource()
                {
                    Value = GetUriValue(vastIFrameResource)
                };
            }
        }

        private static void LoadVideoClicks(XElement vastLinear, CreativeLinear linearCreative)
        {
            if (vastLinear.Elements("VideoClicks").Any())
            {
                var videoClicks = vastLinear.Element("VideoClicks");

                linearCreative.ClickThrough = GetUriValue(videoClicks.Element("ClickThrough"));

                foreach (var clickTracking in videoClicks.Elements("ClickTracking"))
                {
                    linearCreative.ClickTracking.Add((string)clickTracking);
                }

                foreach (var customClick in videoClicks.Elements("CustomClick"))
                {
                    linearCreative.ClickTracking.Add((string)customClick);
                }
            }
        }

        internal static CreativeCompanions CreateCompanionsFromVast(Stream vastCreativeXmlStream)
        {
            XDocument xDoc = XDocument.Load(vastCreativeXmlStream);
            return LoadCompanions(xDoc.Root);
        }

        private static CreativeCompanions LoadCompanions(XElement vastCompanions)
        {
            var companionCreative = new CreativeCompanions();
            companionCreative.Required = (vastCompanions.Attribute("required") != null) ? (CompanionAdsRequired)Enum.Parse(typeof(CompanionAdsRequired), (string)vastCompanions.Attribute("required"), true) : CompanionAdsRequired.None;

            foreach (var vastCompanion in vastCompanions.Elements("Companion"))
            {
                var companion = new Companion();

                foreach (var trackingEvent in GetTrackingEvents(vastCompanion).Where(t => t.Type == TrackingType.CreativeView))
                    companion.ViewTracking.Add(trackingEvent.Value);

                if (vastCompanion.Elements("AdParameters").Any())
                {
                    var xmlEncoded = (bool?)vastCompanion.Element("AdParameters").Attribute("xmlEncoded") ?? false;
                    companion.AdParameters = xmlEncoded ? XmlDecode((string)vastCompanion.Element("AdParameters")) : (string)vastCompanion.Element("AdParameters");
                }
                companion.AdSlotId = (string)vastCompanion.Attribute("adSlotId") ?? string.Empty;
                companion.AltText = (string)vastCompanion.Element("AltText") ?? string.Empty;
                companion.ApiFramework = (string)vastCompanion.Attribute("apiFramework") ?? string.Empty;
                companion.AssetHeight = (int?)vastCompanion.Attribute("assetHeight");
                companion.AssetWidth = (int?)vastCompanion.Attribute("assetWidth");
                companion.ClickThrough = GetUriValue(vastCompanion.Element("CompanionClickThrough"));
                // TODO: this is not in the schema
                //foreach (var vastTracking in vastCompanion.CompanionClickTracking)
                //{
                //    companion.ClickTracking.Add(new Uri(vastTracking));    
                //}
                if (vastCompanion.Elements("CreativeExtensions").Any())
                {
                    foreach (var vastExtension in vastCompanion.Element("CreativeExtensions").Elements("CreativeExtension"))
                    {
                        companion.Extensions.Add(new Extension()); // TODO
                    }
                }
                companion.ExpandedHeight = (int?)vastCompanion.Attribute("expandedHeight");
                companion.ExpandedWidth = (int?)vastCompanion.Attribute("expandedWidth");
                companion.Height = (int?)vastCompanion.Attribute("height") ?? 0;
                companion.Width = (int?)vastCompanion.Attribute("width") ?? 0;
                companion.Item = GetResources(vastCompanion).FirstOrDefault();
                companionCreative.Companions.Add(companion);
            }
            return companionCreative;
        }

        private static AdSystem GetAdSystem(XElement element)
        {
            var adSystem = new AdSystem();
            if (element != null)
            {
                adSystem.Version = (string)element.Attribute("version");
                adSystem.Value = (string)element;
            }
            return adSystem;
        }

        private static Uri GetUriValue(XElement element)
        {
            if (element == null || string.IsNullOrWhiteSpace(element.Value))
            {
                return null;
            }
            else
            {
                return new Uri(element.Value);
            }
        }

        private static IEnumerable<TrackingEvent> GetTrackingEvents(XElement element)
        {
            if (element != null)
            {
                var vastTrackingEvents = element.Element("TrackingEvents");
                if (vastTrackingEvents != null)
                {
                    foreach (var vastTracking in vastTrackingEvents.Elements("Tracking"))
                    {
                        var eventName = (string)vastTracking.Attribute("event");
                        var trackingEvent = GetTrackingEvent(eventName, (string)vastTracking);
                        if (trackingEvent != null)
                        {
                            if (trackingEvent.Type == TrackingType.Progress)
                            {
                                trackingEvent.Offset = FlexibleOffset.Parse((string)vastTracking.Attribute("offset"));
                            }

                            yield return trackingEvent;
                        }
                    }
                }
            }
        }

        public static TrackingEvent GetTrackingEvent(string eventName, string url)
        {
            TrackingType trackingType;
            if (Enum.TryParse<TrackingType>(eventName, true, out trackingType))
            {
                return new TrackingEvent()
                {
                    Type = trackingType,
                    Value = url
                };
            }
            else return null;
        }

        private static string XmlDecode(string value)
        {
            return System.Net.WebUtility.HtmlDecode(value);
        }

        private static DateTime? ToNullableDateTime(string value)
        {
            DateTime result;
            if (DateTime.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        private static bool? ToNullableBool(string value)
        {
            bool result;
            if (bool.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        private static int? ToNullableInt(string value)
        {
            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        private static long? ToNullableLong(string value)
        {
            long result;
            if (long.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        private static TimeSpan? ToNullableTimeSpan(string value)
        {
            TimeSpan result;
            if (TimeSpan.TryParse(value, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        private static TimeSpan? ToNullableTimeSpan(DateTime? value)
        {
            if (value.HasValue)
            {
                if (value.Value < DateTime.Today)
                    return value.Value.Subtract(new DateTime());
                else
                    return value.Value.Subtract(DateTime.Today);
            }
            else
            {
                return new TimeSpan?();
            }
        }

    }
}
