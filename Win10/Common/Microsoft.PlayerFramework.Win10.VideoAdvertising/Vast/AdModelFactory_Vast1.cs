using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.VideoAdvertising
{
    /// <summary>
    /// Adds support for VAST 1.0 wrapper ads only.
    /// </summary>
    public static partial class AdModelFactory
    {
        internal static async Task<AdDocumentPayload> CreateFromVast1(XElement vastRoot, int? maxRedirectDepth, bool allowMultipleAds)
        {
            var result = new AdDocumentPayload();
            result.Version = (string)vastRoot.Attribute("version");

            foreach (var vastAdPod in vastRoot.Elements("Ad").GroupBy(va => va.Attribute("sequence") != null ? 1 : int.MaxValue).OrderBy(vap => vap.Key))
            {
                var adPod = new AdPod();
                foreach (var vastAd in vastAdPod.OrderBy(va => ToNullableInt((string)va.Attribute("sequence")).GetValueOrDefault(0)))
                {
                    var ad = new Ad();
                    ad.Id = (string)vastAd.Attribute("id");

                    if (vastAd.Elements("InLine").Any())
                    {
                        throw new NotImplementedException();
                    }
                    else if (vastAd.Elements("Wrapper").Any())
                    {
                        Ad wrapper = new Ad();
                        var vastAdWrapper = vastAd.Element("Wrapper");

                        // parse the wrapper itself
                        wrapper.AdSystem = GetAdSystem(vastAdWrapper.Element("AdSystem"));
                        var error = (string)vastAdWrapper.Element("Error");
                        if (error == null) wrapper.Errors.Add(error);

                        var linearCreative = new CreativeLinear();

                        foreach (var trackingEvent in GetTrackingEvents(vastAdWrapper))
                            linearCreative.TrackingEvents.Add(trackingEvent);

                        LoadVideoClicks(vastAdWrapper, linearCreative);

                        wrapper.Creatives.Add(linearCreative);

                        var vastAdUri = GetUriValue(vastAdWrapper.Element("VASTAdTagURL"));
                        if (vastAdUri != null)
                        {
                            // load the stream from the web
                            using (var s = await Extensions.LoadStreamAsync(vastAdUri))
                            {
                                int? nextMaxRedirectDepth = maxRedirectDepth.HasValue ? maxRedirectDepth.Value - 1 : maxRedirectDepth;
                                var vastDoc = await CreateFromVast(s, nextMaxRedirectDepth, allowMultipleAds);

                                var firstAdPodInChild = vastDoc.AdPods.FirstOrDefault();

                                if (firstAdPodInChild != null)
                                {
                                    // merge tracking info from this wrapper to every ad in the first adpod in the child
                                    foreach (Ad inlineAd in firstAdPodInChild.Ads)
                                        MergeWrappedAd(wrapper, inlineAd);

                                    // add each ad from the first adpod in the child to the current adpod
                                    foreach (Ad inlineAd in firstAdPodInChild.Ads)
                                        adPod.Ads.Add(inlineAd);
                                }
                            }
                        }
                    }
                }
                result.AdPods.Add(adPod);
            }
            return result;
        }

    }
}
