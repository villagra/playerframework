using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Net.Http;
using System.Threading;
#if SILVERLIGHT
#else
using Windows.Storage.Streams;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace Microsoft.Media.Advertising
{
    public static class FreeWheelFactory
    {
#if SILVERLIGHT
        public static async Task<FWAdResponse> LoadSource(Uri source, CancellationToken cancellationToken)
#else
        public static IAsyncOperation<FWAdResponse> LoadSource(Uri source)
        {
            return AsyncInfo.Run(c => InternalLoadSource(source, c));
        }

        static async Task<FWAdResponse> InternalLoadSource(Uri source, CancellationToken cancellationToken)
#endif
        {
            using (var stream = await Extensions.LoadStreamAsync(source))
            {
#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
                return await TaskEx.Run(() => CreateFromSmartXml(stream), cancellationToken);
#else
                return await Task.Run(() => CreateFromSmartXml(stream), cancellationToken);
#endif
            }
        }

#if SILVERLIGHT
        public static FWAdResponse CreateFromSmartXml(Stream stream)
#else
        public static FWAdResponse CreateFromSmartXml(IInputStream stream)
        {
            var resp = CreateFromSmartXml(stream.AsStreamForRead());
            return resp;
        }

        internal static FWAdResponse CreateFromSmartXml(Stream stream)
#endif
        {
            XDocument xDoc = XDocument.Load(stream);

            var root = xDoc.Root;
            if (root == null || root.Name != "adResponse") throw new ArgumentException("Smart XML root 'adResponse' expected");

            var resp = LoadAdResponse(root);
            return resp;
        }

        internal static FWAdResponse LoadAdResponse(XElement element)
        {
            var result = new FWAdResponse();
            if (element.Attribute("version") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("version").Value))
                result.Version = element.Attribute("version").Value;
            if (element.Attribute("customId") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("customId").Value))
                result.CustomId = element.Attribute("customId").Value;
            if (element.Attribute("networkId") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("networkId").Value))
            {
                int val;
                if (int.TryParse(element.Attribute("networkId").Value, out val))
                    result.NetworkId = element.GetIntAttribute("networkId");
            }
            if (element.Attribute("diagnostic") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("diagnostic").Value))
                result.Diagnostic = element.Attribute("diagnostic").Value;
            if (element.Attribute("customState") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("customState").Value))
                result.CustomState = element.Attribute("customState").Value;

            var rendererManifestXml = element.Element("rendererManifest");
            if (rendererManifestXml != null)
            {
                result.RendererManifest = rendererManifestXml.Value;
                if (rendererManifestXml.Attribute("version") != null &&
                    !string.IsNullOrWhiteSpace(rendererManifestXml.Attribute("version").Value))
                    result.RendererManifestVersion = rendererManifestXml.Attribute("version").Value;
            }

            var visitorXml = element.Element("visitor");
            if (visitorXml != null)
            {
                result.Visitor = LoadVisitor(visitorXml);
            }

            var siteSectionXml = element.Element("siteSection");
            if (siteSectionXml != null)
            {
                result.SiteSection = LoadSiteSection(siteSectionXml);
            }

            var adsXml = element.Element("ads");
            if (adsXml != null)
            {
                foreach (var adXml in adsXml.Elements("ad"))
                {
                    result.Ads.Add(LoadAd(adXml));
                }
            }

            var parametersXml = element.Element("parameters");
            if (parametersXml != null)
            {
                foreach (var parameterXml in parametersXml.Elements("parameter"))
                {
                    result.Parameters.Add(LoadParameter(parameterXml));
                }
            }

            var errorsXml = element.Element("errors");
            if (errorsXml != null)
            {
                foreach (var errorXml in errorsXml.Elements("error"))
                {
                    result.Errors.Add(LoadError(errorXml));
                }
            }

            var eventsXml = element.Element("eventCallbacks");
            if (eventsXml != null)
            {
                foreach (var eventXml in eventsXml.Elements("eventCallback"))
                {
                    result.EventCallbacks.Add(LoadEventCallback(eventXml));
                }
            }

            return result;
        }

        internal static FWEventCallback LoadEventCallback(XElement element)
        {
            var result = new FWEventCallback();
            if (element.Attribute("url") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("url").Value))
                result.Url = element.Attribute("url").Value;
            if (element.Attribute("name") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("name").Value))
                result.Name = element.Attribute("name").Value;
            if (element.Attribute("type") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("type").Value))
            {
                FWCallbackType val;
                if (Enum.TryParse<FWCallbackType>(element.Attribute("type").Value, true, out val))
                    result.Type = val;
            }
            result.ShowBrowser = element.GetBoolAttribute("showBrowser", false);
            if (element.Attribute("use") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("use").Value))
                result.Use = element.Attribute("use").Value;
            var trackingUrlsXml = element.Element("trackingURLs");
            if (trackingUrlsXml != null)
            {
                foreach (var trackingUrlXml in trackingUrlsXml.Elements("url"))
                {
                    result.TrackingUrls.Add(LoadUrl(trackingUrlXml));
                }
            }

            return result;
        }

        internal static FWUrl LoadUrl(XElement element)
        {
            var result = new FWUrl();
            if (element.Attribute("name") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("name").Value))
                result.Name = element.Attribute("name").Value;
            if (element.Attribute("value") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("value").Value))
                result.Value = element.Attribute("value").Value;
            return result;
        }

        internal static FWError LoadError(XElement element)
        {
            var result = new FWError();
            if (element.Attribute("id") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("id").Value))
                result.Id = element.Attribute("id").Value;
            if (element.Attribute("severity") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("severity").Value))
                result.Severity = element.Attribute("severity").Value;
            if (element.Attribute("name") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("name").Value))
                result.Name = element.Attribute("name").Value;
            if (element.Attribute("context") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("context").Value))
                result.Context = element.Attribute("context").Value;
            return result;
        }

        internal static FWParameter LoadParameter(XElement element)
        {
            var result = new FWParameter();
            if (element.Attribute("name") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("name").Value))
                result.Name = element.Attribute("name").Value;
            if (element.Attribute("category") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("category").Value))
                result.Category = element.Attribute("category").Value;
            if (element.Attribute("value") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("value").Value))
                result.Value = element.Attribute("value").Value;
            return result;
        }

        internal static FWVisitor LoadVisitor(XElement element)
        {
            var result = new FWVisitor();
            if (element.Attribute("customId") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("customId").Value))
                result.CustomId = element.Attribute("customId").Value;

            var headersXml = element.Element("httpHeaders");
            if (headersXml != null)
            {
                foreach (var headerXml in headersXml.Elements("httpHeader"))
                {
                    result.HttpHeaders.Add(LoadHttpHeader(headerXml));
                }
            }

            var stateXml = element.Element("state");
            if (stateXml != null)
            {
                var entryXml = stateXml.Element("entry");
                if (entryXml != null)
                {
                    result.State = LoadState(entryXml);
                }
            }

            return result;
        }

        internal static FWHttpHeader LoadHttpHeader(XElement element)
        {
            var result = new FWHttpHeader();
            if (element.Attribute("name") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("name").Value))
                result.Name = element.Attribute("name").Value;
            if (element.Attribute("value") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("value").Value))
                result.Value = element.Attribute("value").Value;

            return result;
        }

        internal static FWState LoadState(XElement element)
        {
            var result = new FWState();
            if (element.Attribute("name") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("name").Value))
                result.Name = element.Attribute("name").Value;
            if (element.Attribute("value") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("value").Value))
                result.Value = element.Attribute("value").Value;

            return result;
        }

        internal static FWAd LoadAd(XElement element)
        {
            var result = new FWAd();
            if (element.Attribute("adId") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("adId").Value))
                result.Id = element.Attribute("adId").Value;
            if (element.Attribute("adUnit") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("adUnit").Value))
                result.AdUnit = element.Attribute("adUnit").Value;
            result.BundleId = element.GetIntAttribute("bundleId");
            result.NoLoad = element.GetBoolAttribute("noLoad", false);
            result.NoPreload = element.GetBoolAttribute("noPreload", false);

            var creativesXml = element.Element("creatives");
            if (creativesXml != null)
            {
                foreach (var creativeXml in creativesXml.Elements("creative"))
                {
                    result.Creatives.Add(LoadCreative(creativeXml));
                }
            }

            return result;
        }

        internal static FWCreative LoadCreative(XElement element)
        {
            var result = new FWCreative();

            if (element.Attribute("redirectUrl") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("redirectUrl").Value))
                result.RedirectUrl = element.Attribute("redirectUrl").Value;
            if (element.Attribute("adUnit") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("adUnit").Value))
                result.AdUnit = element.Attribute("adUnit").Value;
            if (element.Attribute("baseUnit") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("baseUnit").Value))
            {
                FWBaseUnit val;
                if (Enum.TryParse<FWBaseUnit>(element.Attribute("baseUnit").Value.Replace("-", "_"), true, out val))
                    result.BaseUnit = val;
            }
            if (element.Attribute("duration") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("duration").Value))
            {
                int val;
                if (int.TryParse(element.Attribute("duration").Value, out val))
                    result.Duration = TimeSpan.FromSeconds(val);
            }
            if (element.Attribute("creativeId") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("creativeId").Value))
                result.Id = element.Attribute("creativeId").Value;

            var creativeRenditionsXml = element.Element("creativeRenditions");
            if (creativeRenditionsXml != null)
            {
                foreach (var creativeRenditionXml in creativeRenditionsXml.Elements("creativeRendition"))
                {
                    result.CreativeRenditions.Add(LoadCreativeRendition(creativeRenditionXml));
                }
            }

            var parametersXml = element.Element("parameters");
            if (parametersXml != null)
            {
                foreach (var parameterXml in parametersXml.Elements("parameter"))
                {
                    result.Parameters.Add(LoadParameter(parameterXml));
                }
            }

            return result;
        }

        internal static FWCreativeRendition LoadCreativeRendition(XElement element)
        {
            var result = new FWCreativeRendition();

            if (element.Attribute("contentType") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("contentType").Value))
                result.ContentType = element.Attribute("contentType").Value;
            if (element.Attribute("creativeRenditionId") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("creativeRenditionId").Value))
                result.Id = element.Attribute("creativeRenditionId").Value;
            if (element.Attribute("wrapperType") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("wrapperType").Value))
                result.WrapperType = element.Attribute("wrapperType").Value;
            if (element.Attribute("wrapperUrl") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("wrapperUrl").Value))
                result.WrapperUrl = element.Attribute("wrapperUrl").Value;
            if (element.Attribute("adReplicaId") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("adReplicaId").Value))
                result.AdReplicaId = element.Attribute("adReplicaId").Value;
            result.Width = element.GetIntAttribute("width");
            result.Height = element.GetIntAttribute("height");
            if (element.Attribute("preference") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("preference").Value))
            {
                FWPreference val;
                if (Enum.TryParse<FWPreference>(element.Attribute("preference").Value, true, out val))
                    result.Preference = val;
            }
            if (element.Attribute("creativeApi") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("creativeApi").Value))
            {
                FWCreativeApi val;
                if (Enum.TryParse<FWCreativeApi>(element.Attribute("creativeApi").Value, true, out val))
                    result.CreativeApi = val;
            }
            var otherAssetsXml = element.Element("otherAsset");
            if (otherAssetsXml != null)
            {
                foreach (var otherAssetXml in otherAssetsXml.Elements("asset"))
                {
                    result.OtherAssets.Add(LoadAsset(otherAssetXml));
                }
            }

            var assetXml = element.Element("asset");
            if (assetXml != null)
            {
                result.Asset = LoadAsset(assetXml);
            }

            var parametersXml = element.Element("parameters");
            if (parametersXml != null)
            {
                foreach (var parameterXml in parametersXml.Elements("parameter"))
                {
                    result.Parameters.Add(LoadParameter(parameterXml));
                }
            }

            return result;
        }

        internal static FWAsset LoadAsset(XElement element)
        {
            var result = new FWAsset();

            result.Bytes = element.GetIntAttribute("bytes");
            if (element.Attribute("contentType") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("contentType").Value))
                result.ContentType = element.Attribute("contentType").Value;
            if (element.Attribute("id") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("id").Value))
                result.Id = element.Attribute("id").Value;
            if (element.Attribute("name") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("name").Value))
                result.Name = element.Attribute("name").Value;
            if (element.Attribute("url") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("url").Value))
                result.Url = element.Attribute("url").Value;
            if (element.Attribute("mimeType") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("mimeType").Value))
                result.MimeType = element.Attribute("mimeType").Value;
            if (element.Attribute("content") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("content").Value))
                result.Content = element.Attribute("content").Value;

            return result;
        }

        internal static FWSiteSection LoadSiteSection(XElement element)
        {
            var result = new FWSiteSection();
            if (element.Attribute("id") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("id").Value))
                result.Id = element.Attribute("id").Value;
            if (element.Attribute("customId") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("customId").Value))
                result.CustomId = element.Attribute("customId").Value;
            if (element.Attribute("pageviewRandom") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("pageviewRandom").Value))
                result.PageViewRandom = element.Attribute("CustomId").Value;
            result.VideoPlayer = LoadVideoPlayer(element.Element("videoPlayer"));

            var adSlotsXml = element.Element("adSlots");
            if (adSlotsXml != null)
            {
                foreach (var adSlotXml in adSlotsXml.Elements("adSlot"))
                {
                    result.AdSlots.Add(LoadAdSlot(adSlotXml));
                }
            }

            return result;
        }

        internal static FWVideoPlayer LoadVideoPlayer(XElement element)
        {
            var result = new FWVideoPlayer();

            var videoAssetXml = element.Element("videoAsset");
            if (videoAssetXml != null)
            {
                result.VideoAsset = LoadVideoAsset(videoAssetXml);
            }

            var adSlotsXml = element.Element("adSlots");
            if (adSlotsXml != null)
            {
                foreach (var adSlotXml in adSlotsXml.Elements("adSlot"))
                {
                    result.AdSlots.Add(LoadAdSlot(adSlotXml));
                }
            }

            return result;
        }

        internal static FWVideoAsset LoadVideoAsset(XElement element)
        {
            var result = new FWVideoAsset();
            if (element.Attribute("id") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("id").Value))
                result.Id = element.Attribute("id").Value;
            if (element.Attribute("customId") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("customId").Value))
                result.CustomId = element.Attribute("customId").Value;
            if (element.Attribute("videoPlayRandom") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("videoPlayRandom").Value))
                result.VideoPlayRandom = element.Attribute("videoPlayRandom").Value;

            var adSlotsXml = element.Element("adSlots");
            if (adSlotsXml != null)
            {
                foreach (var adSlotXml in adSlotsXml.Elements("temporalAdSlot"))
                {
                    result.AdSlots.Add(LoadTemporalAdSlot(adSlotXml));
                }
            }

            var eventCallbacksXml = element.Element("eventCallbacks");
            if (eventCallbacksXml != null)
            {
                foreach (var eventCallbackXml in eventCallbacksXml.Elements("eventCallback"))
                {
                    result.EventCallbacks.Add(LoadEventCallback(eventCallbackXml));
                }
            }

            return result;
        }

        internal static FWNonTemporalAdSlot LoadNonTemporalAdSlot(XElement element)
        {
            var result = new FWNonTemporalAdSlot();
            if (element.Attribute("height") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("height").Value))
            {
                int val;
                if (int.TryParse(element.Attribute("height").Value, out val))
                    result.Height = element.GetIntAttribute("height");
            }
            if (element.Attribute("width") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("width").Value))
            {
                int val;
                if (int.TryParse(element.Attribute("width").Value, out val))
                    result.Width = val;
            }
            if (element.Attribute("compatibleDimensions") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("compatibleDimensions").Value))
                result.CompatibleDimensions = element.Attribute("compatibleDimensions").Value;
            if (element.Attribute("adUnit") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("adUnit").Value))
                result.AdUnit = element.Attribute("adUnit").Value;
            if (element.Attribute("customId") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("customId").Value))
                result.CustomId = element.Attribute("customId").Value;

            var adsXml = element.Element("selectedAds");
            if (adsXml != null)
            {
                foreach (var adReferenceXml in adsXml.Elements("adReference"))
                {
                    result.SelectedAds.Add(LoadAdReference(adReferenceXml));
                }
            }

            var parametersXml = element.Element("parameters");
            if (parametersXml != null)
            {
                foreach (var parameterXml in parametersXml.Elements("parameter"))
                {
                    result.Parameters.Add(LoadParameter(parameterXml));
                }
            }

            return result;
        }

        internal static FWTemporalAdSlot LoadTemporalAdSlot(XElement element)
        {
            var result = new FWTemporalAdSlot();
            if (element.Attribute("height") != null && 
                !string.IsNullOrWhiteSpace(element.Attribute("height").Value))
            {
                int val;
                if (int.TryParse(element.Attribute("height").Value, out val))
                    result.Height = element.GetIntAttribute("height");
            }
            if (element.Attribute("width") != null && 
                !string.IsNullOrWhiteSpace(element.Attribute("width").Value))
            {
                int val;
                if (int.TryParse(element.Attribute("width").Value, out val))
                    result.Width = val;
            }
            if (element.Attribute("compatibleDimensions") != null && 
                !string.IsNullOrWhiteSpace(element.Attribute("compatibleDimensions").Value))
                result.CompatibleDimensions = element.Attribute("compatibleDimensions").Value;
            if (element.Attribute("adUnit") != null && 
                !string.IsNullOrWhiteSpace(element.Attribute("adUnit").Value))
                result.AdUnit = element.Attribute("adUnit").Value;
            if (element.Attribute("customId") != null && 
                !string.IsNullOrWhiteSpace(element.Attribute("customId").Value))
                result.CustomId = element.Attribute("customId").Value;
            if (element.Attribute("source") != null && 
                !string.IsNullOrWhiteSpace(element.Attribute("source").Value))
                result.Source = element.Attribute("source").Value;
            if (element.Attribute("maxSlotDuration") != null && 
                !string.IsNullOrWhiteSpace(element.Attribute("maxSlotDuration").Value))
            {
                int val;
                if (int.TryParse(element.Attribute("maxSlotDuration").Value, out val))
                    result.MaxSlotDuration = TimeSpan.FromSeconds(val);
            }
            if (element.Attribute("timePosition") != null && 
                !string.IsNullOrWhiteSpace(element.Attribute("timePosition").Value))
            {
                int val;
                if (int.TryParse(element.Attribute("timePosition").Value, out val))
                    result.TimePosition = TimeSpan.FromSeconds(val);
            }
            result.TimePositionSequence = element.GetIntAttribute("timePositionSequence");
            if (element.Attribute("timePositionClass") != null && 
                !string.IsNullOrWhiteSpace(element.Attribute("timePositionClass").Value))
                result.TimePositionClass = element.Attribute("timePositionClass").Value;

            var adsXml = element.Element("selectedAds");
            if (adsXml != null)
            {
                foreach (var adReferenceXml in adsXml.Elements("adReference"))
                {
                    result.SelectedAds.Add(LoadAdReference(adReferenceXml));
                }
            }

            var eventCallbacksXml = element.Element("eventCallbacks");
            if (eventCallbacksXml != null)
            {
                foreach (var eventCallbackXml in eventCallbacksXml.Elements("eventCallback"))
                {
                    result.EventCallbacks.Add(LoadEventCallback(eventCallbackXml));
                }
            }

            var parametersXml = element.Element("parameters");
            if (parametersXml != null)
            {
                foreach (var parameterXml in parametersXml.Elements("parameter"))
                {
                    result.Parameters.Add(LoadParameter(parameterXml));
                }
            }

            return result;
        }

        internal static FWAdReference LoadAdReference(XElement element)
        {
            var result = new FWAdReference();
            if (element.Attribute("creativeId") != null && 
                !string.IsNullOrWhiteSpace(element.Attribute("creativeId").Value))
                result.CreativeId = element.Attribute("creativeId").Value;
            if (element.Attribute("creativeRenditionId") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("creativeRenditionId").Value))
                result.CreativeRenditionId = element.Attribute("creativeRenditionId").Value;
            if (element.Attribute("adId") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("adId").Value))
                result.AdId = element.Attribute("adId").Value;
            if (element.Attribute("replicaId") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("replicaId").Value))
                result.ReplicaId = element.Attribute("replicaId").Value;
            if (element.Attribute("adSlotEnv") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("adSlotEnv").Value))
                result.SlotEnv = element.Attribute("adSlotEnv").Value;
            if (element.Attribute("slotCustomId") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("slotCustomId").Value))
                result.SlotId = element.Attribute("slotCustomId").Value;

            var eventCallbacksXml = element.Element("eventCallbacks");
            if (eventCallbacksXml != null)
            {
                foreach (var eventCallbackXml in eventCallbacksXml.Elements("eventCallback"))
                {
                    result.EventCallbacks.Add(LoadEventCallback(eventCallbackXml));
                }
            }

            var companionAdsXaml = element.Element("companionAds");
            if (companionAdsXaml != null)
            {
                foreach (var adReferenceXml in companionAdsXaml.Elements("adReference"))
                {
                    result.CompanionAds.Add(LoadAdReference(adReferenceXml));
                }
            }

            var fallbackAdsXaml = element.Element("fallbackAds");
            if (fallbackAdsXaml != null)
            {
                foreach (var adReferenceXml in fallbackAdsXaml.Elements("adReference"))
                {
                    result.FallbackAds.Add(LoadAdReference(adReferenceXml));
                }
            }

            return result;
        }

        internal static FWAdSlot LoadAdSlot(XElement element)
        {
            var result = new FWAdSlot();
            if (element.Attribute("height") != null && 
                !string.IsNullOrWhiteSpace(element.Attribute("height").Value))
            {
                int val;
                if (int.TryParse(element.Attribute("height").Value, out val))
                    result.Height = element.GetIntAttribute("height");
            }
            if (element.Attribute("width") != null && 
                !string.IsNullOrWhiteSpace(element.Attribute("width").Value))
            {
                int val;
                if (int.TryParse(element.Attribute("width").Value, out val))
                    result.Width = val;
            }
            if (element.Attribute("compatibleDimensions") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("compatibleDimensions").Value))
                result.CompatibleDimensions = element.Attribute("compatibleDimensions").Value;
            if (element.Attribute("adUnit") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("adUnit").Value))
                result.AdUnit = element.Attribute("adUnit").Value;
            if (element.Attribute("customId") != null &&
                !string.IsNullOrWhiteSpace(element.Attribute("customId").Value))
                result.CustomId = element.Attribute("customId").Value;

            var adsXml = element.Element("selectedAds");
            if (adsXml != null)
            {
                foreach (var adReferenceXml in adsXml.Elements("adReference"))
                {
                    result.SelectedAds.Add(LoadAdReference(adReferenceXml));
                }
            }

            return result;
        }

#if NETFX_CORE
        /// <summary>
        /// Creates an AdDocumentPayload from a SmartXML ad slot.
        /// </summary>
        /// <param name="adSlot">The FWTemporalAdSlot object defining the ad payload</param>
        /// <param name="adResponse">The entire SmartXML document (required for cross referencing)</param>
        /// <returns>An AdDocumentPayload object that can be played by the AdHandlerPlugin.</returns>
        public static IAsyncOperation<AdDocumentPayload> GetAdDocumentPayload(FWTemporalAdSlot adSlot, FWAdResponse adResponse)
        {
            return AsyncInfo.Run(c => GetAdDocumentPayload(adSlot, adResponse, c));
        }
#endif

        /// <summary>
        /// Creates an AdDocumentPayload from a SmartXML ad slot.
        /// </summary>
        /// <param name="adSlot">The FWTemporalAdSlot object defining the ad payload</param>
        /// <param name="adResponse">The entire SmartXML document (required for cross referencing)</param>
        /// <returns>An AdDocumentPayload object that can be played by the AdHandlerPlugin.</returns>
#if NETFX_CORE
        internal static async Task<AdDocumentPayload> GetAdDocumentPayload(FWTemporalAdSlot adSlot, FWAdResponse adResponse, CancellationToken c)
#else
        public static async Task<AdDocumentPayload> GetAdDocumentPayload(FWTemporalAdSlot adSlot, FWAdResponse adResponse, CancellationToken c)
#endif
        {
            var payload = new AdDocumentPayload();
            var adPod = new AdPod();
            payload.AdPods.Add(adPod);
            foreach (var adReference in adSlot.SelectedAds)
            {
                var ad = await CreateAd(adResponse, adReference);
                adPod.Ads.Add(ad);

                foreach (var fallbackAdReference in adReference.FallbackAds)
                {
                    var fallbackAd = await CreateAd(adResponse, fallbackAdReference);
                    ad.FallbackAds.Add(fallbackAd);
                }
            }
            return payload;
        }

        private static async Task<Ad> CreateAd(FWAdResponse adResponse, FWAdReference adReference)
        {
            var ad = adResponse.Ads.FirstOrDefault(a => a.Id == adReference.AdId);
            if (ad != null)
            {
                var linearCreative = await CreateLinearAd(ad, adReference);

                foreach (var companionAdReference in adReference.CompanionAds)
                {
                    var companionAd = adResponse.Ads.FirstOrDefault(a => a.Id == companionAdReference.AdId);
                    var companionCreative = CreateCompanionAds(companionAd, companionAdReference);
                    companionCreative.Sequence = 1;
                    linearCreative.Creatives.Add(companionCreative);
                }

                return linearCreative;
            }
            return null;
        }

        public static CreativeCompanions CreateCompanionAds(FWAd source, FWAdReference reference)
        {
            var result = new CreativeCompanions();

            foreach (var creative in source.Creatives)
            {
                var companion = new Companion();
                result.Companions.Add(companion);
                companion.AdSlotId = reference.SlotId ?? string.Empty;

                var rendition = creative.CreativeRenditions.OrderByDescending(cr => cr.Preference).FirstOrDefault(c => c.Asset.MimeType.ToLowerInvariant().StartsWith("image/"));
                if (rendition != null)
                {
                    var asset = rendition.Asset;
                    if (asset != null)
                    {
                        companion.Item = new StaticResource()
                        {
                            Value = new Uri(asset.Url),
                            CreativeType = asset.MimeType
                        };
                        companion.Width = rendition.Width;
                        companion.Height = rendition.Height;
                        companion.Id = asset.Id ?? string.Empty;
                    }
                }

                var allCallbacks = reference.EventCallbacks;
                foreach (var url in allCallbacks.Where(ec => ec.Type == FWCallbackType.Click && !ec.ShowBrowser).SelectMany(ec => ec.GetUrls()))
                {
                    companion.ClickTracking.Add(url);
                }

                var clickUrl = allCallbacks.Where(ec => ec.Type == FWCallbackType.Click && ec.ShowBrowser).SelectMany(ec => ec.GetUrls()).FirstOrDefault();
                if (clickUrl != null)
                {
                    companion.ClickThrough = new Uri(clickUrl);
                }
            }
            return result;
        }

        static async Task<Ad> CreateLinearAd(FWAd source, FWAdReference reference)
        {
            var ad = new Ad();

            var allCallbacks = reference.EventCallbacks;
            foreach (var url in allCallbacks.Where(ec => ec.Type == FWCallbackType.Impression && ec.Name == FWEventCallback.DefaultImpression).SelectMany(ec => ec.GetUrls()))
            {
                ad.Impressions.Add(url);
            }

            int index = 0;
            IEnumerable<FWCreative> creatives = source.Creatives;
            if (reference.CreativeId != null)
            {
                creatives = creatives.Where(c => c.Id == reference.CreativeId);
            }
            foreach (var creative in creatives)
            {
                index++;
                var wrappedAds = new List<Ad>();
                var linear = new CreativeLinear();
                linear.AdParameters = string.Join("&", creative.Parameters.Select(p => Uri.EscapeDataString(p.Name) + "=" + Uri.EscapeDataString(p.Value)));
                linear.Duration = creative.Duration;
                linear.Sequence = index;

                IEnumerable<FWCreativeRendition> creativeRenditions = creative.CreativeRenditions;
                if (reference.CreativeRenditionId != null)
                {
                    creativeRenditions = creativeRenditions.Where(cr => cr.Id == reference.CreativeRenditionId).DefaultIfEmpty(creativeRenditions);
                    if (reference.ReplicaId != null)
                    {
                        creativeRenditions = creativeRenditions.Where(cr => cr.AdReplicaId == reference.ReplicaId).DefaultIfEmpty(creativeRenditions);
                    }
                }

                foreach (var rendition in creativeRenditions)
                {
                    if (!string.IsNullOrEmpty(rendition.WrapperType))
                    {
                        switch (rendition.WrapperType.ToLowerInvariant())
                        {
                            case "external/vast-2":
                                try
                                {
                                    var vastAdUri = new Uri(rendition.WrapperUrl);
                                    // load the stream from the web
                                    using (var s = await Extensions.LoadStreamAsync(vastAdUri))
                                    {
                                        var wrappedVastDoc = await AdModelFactory.CreateFromVast(s, null, true);
                                        if (wrappedVastDoc != null)
                                        {
                                            // use the first ad
                                            var wrappedAd = wrappedVastDoc.AdPods.SelectMany(pod => pod.Ads).FirstOrDefault();
                                            if (wrappedAd != null)
                                            {
                                                wrappedAds.Add(wrappedAd);
                                            }
                                        }
                                    }
                                }
                                catch { /* swallow */ }
                                break;
                        }
                    }
                    else
                    {
                        // TODO: FreeWheel assets can contain Content instead of Url. This could be supported someday; for now it is ignored.
                        if (rendition.Asset != null)
                        {
                            var mediaFile = CreateMediaFile(creative, rendition, rendition.Asset);
                            if (mediaFile != null)
                            {
                                mediaFile.Ranking = (int)rendition.Preference + 1; // add one to indicate this is preferred over "OtherAssets"
                                linear.MediaFiles.Add(mediaFile);
                            }
                        }

                        foreach (var asset in rendition.OtherAssets)
                        {
                            var mediaFile = CreateMediaFile(creative, rendition, asset);
                            if (mediaFile != null)
                            {
                                mediaFile.Ranking = (int)rendition.Preference;
                                linear.MediaFiles.Add(mediaFile);
                            }
                        }
                    }
                }

                // generate callback urls from one base url
                foreach (var eventCallback in allCallbacks.Where(ec => ec.Type == FWCallbackType.Impression))
                {
                    foreach (var url in eventCallback.GetUrls())
                    {
                        switch (eventCallback.Name.ToLower())
                        {
                            case "start":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.Start, Value = url });
                                break;
                            case "firstquartile":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.FirstQuartile, Value = url });
                                break;
                            case "midpoint":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.Midpoint, Value = url });
                                break;
                            case "thirdquartile":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.ThirdQuartile, Value = url });
                                break;
                            case "complete":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.Complete, Value = url });
                                break;
                        }
                    }
                }

                // generate callback urls from one base url
                foreach (var eventCallback in allCallbacks.Where(ec => ec.Type == FWCallbackType.Standard))
                {
                    foreach (var url in eventCallback.GetUrls())
                    {
                        switch (eventCallback.Name.Replace("-", "").ToLower())
                        {
                            case "_creativeview":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.CreativeView, Value = url });
                                break;
                            case "_mute":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.Mute, Value = url });
                                break;
                            case "_unmute":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.Unmute, Value = url });
                                break;
                            case "_pause":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.Pause, Value = url });
                                break;
                            case "_rewind":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.Rewind, Value = url });
                                break;
                            case "_resume":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.Resume, Value = url });
                                break;
                            case "_fullscreen":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.Fullscreen, Value = url });
                                break;
                            case "_exitfullscreen":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.ExitFullscreen, Value = url });
                                break;
                            case "_expand":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.Expand, Value = url });
                                break;
                            case "_collapse":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.Collapse, Value = url });
                                break;
                            case "_acceptinvitation":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.AcceptInvitation, Value = url });
                                break;
                            case "_close":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.Close, Value = url });
                                break;
                            case "_skip":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.Skip, Value = url });
                                break;
                            case "_progress":
                                linear.TrackingEvents.Add(new TrackingEvent() { Type = TrackingType.Progress, Value = url });
                                break;
                        }
                    }
                }

                foreach (var url in allCallbacks.Where(ec => ec.Type == FWCallbackType.Click && !ec.ShowBrowser).SelectMany(ec => ec.GetUrls()))
                {
                    linear.ClickTracking.Add(url);
                }

                var clickUrl = allCallbacks.Where(ec => ec.Type == FWCallbackType.Click && ec.ShowBrowser).SelectMany(ec => ec.GetUrls()).FirstOrDefault();
                if (clickUrl != null)
                {
                    linear.ClickThrough = new Uri(clickUrl);
                }

                // generate callback urls from one base url ONLY when the callback does not already exist
                foreach (var eventCallback in allCallbacks.Where(ec => ec.Type == FWCallbackType.Generic))
                {
                    foreach (var url in eventCallback.GetUrls())
                    {
                        var baseUrl = url + string.Format("&metr={0}", FreeWheelFactory.GetSupportedMetrics());

                        // quartile events
                        var quartileUrl = baseUrl + "&ct=[LASTQUARTILE]&et=i"; // [LASTQUARTILE] will get replaced by the VPAID controller
                        AddSecondaryCallback(linear.TrackingEvents, new TrackingEvent() { Type = TrackingType.FirstQuartile, Value = quartileUrl + "&cn=firstQuartile" });
                        AddSecondaryCallback(linear.TrackingEvents, new TrackingEvent() { Type = TrackingType.Midpoint, Value = quartileUrl + "&cn=midPoint" });
                        AddSecondaryCallback(linear.TrackingEvents, new TrackingEvent() { Type = TrackingType.ThirdQuartile, Value = quartileUrl + "&cn=thirdQuartile" });
                        AddSecondaryCallback(linear.TrackingEvents, new TrackingEvent() { Type = TrackingType.Complete, Value = quartileUrl + "&cn=complete" });

                        // advanced metrics
                        var advancedUrl = baseUrl + "&et=s";
                        AddSecondaryCallback(linear.TrackingEvents, new TrackingEvent() { Type = TrackingType.Mute, Value = advancedUrl + "&cn=_mute" });
                        AddSecondaryCallback(linear.TrackingEvents, new TrackingEvent() { Type = TrackingType.Unmute, Value = advancedUrl + "&cn=_un-mute" });
                        AddSecondaryCallback(linear.TrackingEvents, new TrackingEvent() { Type = TrackingType.Collapse, Value = advancedUrl + "&cn=_collapse" });
                        AddSecondaryCallback(linear.TrackingEvents, new TrackingEvent() { Type = TrackingType.Expand, Value = advancedUrl + "&cn=_expand" });
                        AddSecondaryCallback(linear.TrackingEvents, new TrackingEvent() { Type = TrackingType.Pause, Value = advancedUrl + "&cn=_pause" });
                        AddSecondaryCallback(linear.TrackingEvents, new TrackingEvent() { Type = TrackingType.Resume, Value = advancedUrl + "&cn=_resume" });
                        AddSecondaryCallback(linear.TrackingEvents, new TrackingEvent() { Type = TrackingType.Rewind, Value = advancedUrl + "&cn=_rewind" });
                        AddSecondaryCallback(linear.TrackingEvents, new TrackingEvent() { Type = TrackingType.AcceptInvitation, Value = advancedUrl + "&cn=_accept-invitation" });
                        AddSecondaryCallback(linear.TrackingEvents, new TrackingEvent() { Type = TrackingType.Close, Value = advancedUrl + "&cn=_close" });
                        //AddSecondaryCallback(linear.TrackingEvents, new TrackingEvent() { Type = TrackingType.Minimize, Value = advancedUrl + "&cn=_minimize" });
                    }
                }

                ad.Creatives.Add(linear);

                foreach (var wrappedAd in wrappedAds)
                {
                    AdModelFactory.MergeWrappedAdBeacons(wrappedAd, ad);
                    var wrappedCreative = AdModelFactory.FindMatchingCreative(linear, wrappedAd);
                    AdModelFactory.MergeWrappedCreative(wrappedCreative, linear);
                }
            }

            return ad;
        }

        /// <summary>
        /// Adds a new tracking event ONLY if the tracking event doesn't already exist.
        /// </summary>
        /// <param name="trackingEvents">The list of tracking events to conditionaly add to</param>
        /// <param name="trackingEvent">The new tracking event to conditionally ad</param>
        private static void AddSecondaryCallback(IList<TrackingEvent> trackingEvents, TrackingEvent trackingEvent)
        {
            if (!trackingEvents.Any(t => t.Type == trackingEvent.Type))
            {
                trackingEvents.Add(trackingEvent);
            }
        }

        private static MediaFile CreateMediaFile(FWCreative creative, FWCreativeRendition rendition, FWAsset asset)
        {
            Uri uri;
            if (Uri.TryCreate(asset.Url, UriKind.RelativeOrAbsolute, out uri))
            {
                var mediaFile = new MediaFile();

                mediaFile.Value = uri;
                mediaFile.Width = rendition.Width;
                mediaFile.Height = rendition.Height;
                mediaFile.Codec = rendition.ContentType ?? asset.ContentType;
                mediaFile.Type = asset.MimeType;
                mediaFile.Id = asset.Id;
                mediaFile.Bitrate = (long)(asset.Bytes / creative.Duration.GetValueOrDefault(TimeSpan.FromSeconds(30)).TotalSeconds / 1000 * 8);
                if (rendition.CreativeApi != FWCreativeApi.none)
                {
                    mediaFile.ApiFramework = rendition.CreativeApi.ToString().ToUpperInvariant();
                }

                return mediaFile;
            }
            else return null;
        }

        public static IEnumerable<ICompanionSource> GetNonTemporalCompanions(FWAdResponse adResponse)
        {
            foreach (var adSlot in adResponse.SiteSection.AdSlots.Concat(adResponse.SiteSection.VideoPlayer.AdSlots))
            {
                foreach (var adReference in adSlot.SelectedAds)
                {
                    var companionAd = adResponse.Ads.FirstOrDefault(a => a.Id == adReference.AdId);
                    var companionCreatives = FreeWheelFactory.CreateCompanionAds(companionAd, adReference);
                    foreach (var companionCreative in companionCreatives.Companions)
                    {
                        yield return companionCreative;
                    }
                }
            }
        }

        public static int GetSupportedMetrics()
        {
            // everything but minimize is supported
            return quartile + midPoint + complete + mute + collapse + pause_resume + rewind + accept_invitation + close;
        }

        const int quartile = 1;
        const int midPoint = 2;
        const int complete = 4;
        const int mute = 8;
        const int collapse = 16;
        const int pause_resume = 32;
        const int rewind = 64;
        const int accept_invitation = 128;
        const int close = 256;
        const int minimize = 512;
    }
}
