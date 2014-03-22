using System;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
#if SILVERLIGHT
#else
using Windows.Storage.Streams;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace Microsoft.Media.Advertising
{
    public static class VmapFactory
    {
        static XNamespace ns = "http://www.iab.net/vmap-1.0";

#if SILVERLIGHT
        public static async Task<Vmap> LoadSource(Uri source, CancellationToken cancellationToken)
#else
        public static IAsyncOperation<Vmap> LoadSource(Uri source)
        {
            return AsyncInfo.Run(c => InternalLoadSource(source, c));
        }

        static async Task<Vmap> InternalLoadSource(Uri source, CancellationToken cancellationToken)
#endif
        {
            using (var stream = await Extensions.LoadStreamAsync(source))
            {
#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
                return await TaskEx.Run(() => CreateFromVmap(stream), cancellationToken);
#else
                return await Task.Run(() => CreateFromVmap(stream), cancellationToken);
#endif
            }
        }

#if SILVERLIGHT
        public static Vmap CreateFromVmap(Stream stream)
#else
        public static Vmap CreateFromMVmap(IInputStream stream)
        {
            return CreateFromVmap(stream.AsStreamForRead());
        }

        internal static Vmap CreateFromVmap(Stream stream)
#endif
        {
            XDocument xDoc = XDocument.Load(stream);
            return LoadVmap(xDoc.Root);
        }

        static Vmap LoadVmap(XElement vmapXml)
        {
            var version = (string)vmapXml.Attribute("version");
            if (version != "1.0") throw new ArgumentException("VMAP version not supported", "vmapXml");
            if (vmapXml.Name != ns + "VMAP") throw new ArgumentException("Invalid VMAP xml", "vmapXml");

            var result = new Vmap();

            result.Version = version;
            foreach (var adBreakXml in vmapXml.Elements(ns + "AdBreak"))
            {
                result.AdBreaks.Add(LoadAdBreak(adBreakXml));
            }
            var extensionsXml = vmapXml.Element(ns + "Extensions");
            if (extensionsXml != null)
            {
                foreach (var extensionXml in extensionsXml.Elements(ns + "Extension"))
                {
                    result.Extensions.Add(LoadExtension(extensionXml));
                }
            }

            return result;
        }

        static VmapAdBreak LoadAdBreak(XElement adBreakXml)
        {
            var result = new VmapAdBreak();

            result.BreakId = (string)adBreakXml.Attribute("breakId");
            result.BreakType = (string)adBreakXml.Attribute("breakType");
            result.TimeOffset = (string)adBreakXml.Attribute("timeOffset");
            var adSourceXml = adBreakXml.Element(ns + "AdSource");
            if (adSourceXml != null)
            {
                result.AdSource = LoadAdSource(adSourceXml);
            }
            var trackingEventsXml = adBreakXml.Element(ns + "TrackingEvents");
            if (trackingEventsXml != null)
            {
                foreach (var trackingEventXml in trackingEventsXml.Elements(ns + "Tracking"))
                {
                    result.TrackingEvents.Add(LoadTrackingEvent(trackingEventXml));
                }
            }
            var extensionsXml = adBreakXml.Element(ns + "Extensions");
            if (extensionsXml != null)
            {
                foreach (var extensionXml in extensionsXml.Elements(ns + "Extension"))
                {
                    result.Extensions.Add(LoadExtension(extensionXml));
                }
            }

            return result;
        }

        static VmapAdSource LoadAdSource(XElement adSourceXml)
        {
            var result = new VmapAdSource();

            result.Id = (string)adSourceXml.Attribute("id") ?? string.Empty;
            result.AllowMultipleAds = adSourceXml.GetBoolAttribute("allowMultipleAds", true);
            result.FollowsRedirect = adSourceXml.GetBoolAttribute("followRedirects", true);
            var vastDataXml = adSourceXml.Element(ns + "VASTAdData") ?? adSourceXml.Element(ns + "VASTData"); // HACK: VASTData is a typeo in VMAP spec so it may be phased out eventually.
            if (vastDataXml != null)
            {
                var innerVastDataXml = vastDataXml.Elements().FirstOrDefault();
                if (innerVastDataXml != null)
                {
                    result.VastData = innerVastDataXml.ToString() ?? string.Empty;
                }
            }

            var customAdDataXml = adSourceXml.Element(ns + "CustomAdData");
            if (customAdDataXml != null)
            {
                result.CustomAdData = customAdDataXml.Value ?? string.Empty;
                result.CustomAdDataTemplateType = (string)customAdDataXml.Attribute("templateType") ?? string.Empty;
            }

            var adTagXml = adSourceXml.Element(ns + "AdTagURI");
            if (adTagXml != null)
            {
                if (!string.IsNullOrEmpty(adTagXml.Value))
                {
                    result.AdTag = new Uri(adTagXml.Value);
                }
                result.AdTagTemplateType = (string)adTagXml.Attribute("templateType") ?? string.Empty;
            }

            return result;
        }

        static VmapTrackingEvent LoadTrackingEvent(XElement trackingXml)
        {
            var result = new VmapTrackingEvent();

            if (!string.IsNullOrEmpty(trackingXml.Value))
            {
                result.TrackingUri = new Uri(trackingXml.Value);
            }

            VmapTrackingEventType type;
#if WINDOWS_PHONE
            if (EnumEx.TryParse<VmapTrackingEventType>((string)trackingXml.Attribute("type"), true, out type))
#else
            if (Enum.TryParse((string)trackingXml.Attribute("type"), out type))
#endif
            {
                result.EventType = type;
            }

            return result;
        }

        static VmapExtension LoadExtension(XElement extensionXml)
        {
            var result = new VmapExtension();

            var innerExtensionXml = extensionXml.Elements().FirstOrDefault();
            if (innerExtensionXml != null)
            {
                result.Xml = innerExtensionXml.ToString();
            }

            result.Type = (string)extensionXml.Attribute("type");

            return result;
        }

        // error codes are currently not used
        static readonly Dictionary<VmapErrorId, VmapError> VmapErrors;
        static VmapFactory()
        {
            VmapErrors = new Dictionary<VmapErrorId, VmapError>();
            VmapErrors.Add(VmapErrorId.Undefined, new VmapError() { Code = 900, Message = "Undefined error" });
            VmapErrors.Add(VmapErrorId.VMAPSchema, new VmapError() { Code = 1000, Message = "VMAP schema error" });
            VmapErrors.Add(VmapErrorId.VMAPVersion, new VmapError() { Code = 1001, Message = "VMAP version of response not supported" });
            VmapErrors.Add(VmapErrorId.VMAPParsing, new VmapError() { Code = 1002, Message = "VMAP parsing error" });
            VmapErrors.Add(VmapErrorId.AdBreakType, new VmapError() { Code = 1003, Message = "AdBreak type not supported" });
            VmapErrors.Add(VmapErrorId.AdResponseGeneral, new VmapError() { Code = 1004, Message = "General ad response document error" });
            VmapErrors.Add(VmapErrorId.AdResponseTemplate, new VmapError() { Code = 1005, Message = "Ad response template type not supported" });
            VmapErrors.Add(VmapErrorId.AdResponseDocument, new VmapError() { Code = 1006, Message = "Ad response document extraction or parsing error" });
            VmapErrors.Add(VmapErrorId.AdResponseTimeout, new VmapError() { Code = 1007, Message = "Ad response document retrieval timeout" });
            VmapErrors.Add(VmapErrorId.AdResponseRetieval, new VmapError() { Code = 1008, Message = "Ad response document retrieval error (e.g., HTTP server responded with error code)" });
        }

        const string ErrorCodeMacro = "[ERROR_CODE]";
        const string ErrorMessageMacro = "[ERROR_MESSAGE]";

        internal static string GetErrorText(string errorText, VmapErrorId errorId)
        {
            var vmapError = VmapErrors[errorId];
            return errorText.Replace(ErrorCodeMacro, vmapError.Code.ToString()).Replace(ErrorMessageMacro, vmapError.Message);
        }
        
        class VmapError
        {
            public int Code { get; set; }
            public string Message { get; set; }
        }
    }

    internal enum VmapErrorId
    {
        Undefined,
        VMAPSchema,
        VMAPVersion,
        VMAPParsing,
        AdBreakType,
        AdResponseGeneral,
        AdResponseTemplate,
        AdResponseDocument,
        AdResponseTimeout,
        AdResponseRetieval,
    }
}
