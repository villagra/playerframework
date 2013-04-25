using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
#if SILVERLIGHT
#else
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace Microsoft.VideoAdvertising
{
    public sealed class ClipAdPayloadHandler : IAdPayloadHandler
    {
        public static string AdType { get { return "clip"; } }
        static string[] SupportedTypeIds = new[] { AdType };
        public string[] SupportedTypes { get { return SupportedTypeIds; } }

        readonly DocumentAdPayloadHandler adHandlerBase;

        public ClipAdPayloadHandler()
        {
            adHandlerBase = new DocumentAdPayloadHandler();
            adHandlerBase.LoadPlayer += adHandlerBase_LoadPlayer;
            adHandlerBase.UnloadPlayer += adHandlerBase_UnloadPlayer;
            adHandlerBase.ActivateAdUnit += adHandlerBase_ActivateAdUnit;
            adHandlerBase.DeactivateAdUnit += adHandlerBase_DeactivateAdUnit;
            adHandlerBase.AdFailure += adHandlerBase_AdFailure;
        }

        void adHandlerBase_AdFailure(object sender, AdFailureEventArgs e)
        {
            if (AdFailure != null) AdFailure(this, e);
        }

        void adHandlerBase_DeactivateAdUnit(object sender, DeactivateAdUnitEventArgs e)
        {
            if (DeactivateAdUnit != null) DeactivateAdUnit(this, e);
        }

        void adHandlerBase_ActivateAdUnit(object sender, ActivateAdUnitEventArgs e)
        {
            if (ActivateAdUnit != null) ActivateAdUnit(this, e);
        }

        void adHandlerBase_UnloadPlayer(object sender, UnloadPlayerEventArgs e)
        {
            if (UnloadPlayer != null) UnloadPlayer(this, e);
        }

        void adHandlerBase_LoadPlayer(object sender, LoadPlayerEventArgs e)
        {
            if (LoadPlayer != null) LoadPlayer(this, e);
        }

        public event EventHandler<LoadPlayerEventArgs> LoadPlayer;
        public event EventHandler<UnloadPlayerEventArgs> UnloadPlayer;
        public event EventHandler<ActivateAdUnitEventArgs> ActivateAdUnit;
        public event EventHandler<DeactivateAdUnitEventArgs> DeactivateAdUnit;
        public event EventHandler<AdFailureEventArgs> AdFailure;

        public IPlayer Player
        {
            get { return adHandlerBase.Player; }
            set { adHandlerBase.Player = value; }
        }

#if SILVERLIGHT
        public async Task PreloadAdAsync(IAdSource adSource, CancellationToken cancellationToken)
#else
        public IAsyncAction PreloadAdAsync(IAdSource adSource)
        {
            return AsyncInfo.Run(c => PreloadAdAsync(adSource, c));
        }

        internal async Task PreloadAdAsync(IAdSource adSource, CancellationToken cancellationToken)
#endif
        {
            ProcessPayload(adSource);
            await adHandlerBase.PreloadAdAsync(adSource, cancellationToken);
        }

#if SILVERLIGHT
        public async Task PlayAdAsync(IAdSource adSource, TimeSpan? startTimeout, CancellationToken cancellationToken, IProgress<AdStatus> progress)
#else
        public IAsyncActionWithProgress<AdStatus> PlayAdAsync(IAdSource adSource, TimeSpan? startTimeout)
        {
            return AsyncInfo.Run<AdStatus>((c, p) => PlayAdAsync(adSource, startTimeout, c, p));
        }

        internal async Task PlayAdAsync(IAdSource adSource, TimeSpan? startTimeout, CancellationToken cancellationToken, IProgress<AdStatus> progress)
#endif
        {
            ProcessPayload(adSource);
            if (!cancellationToken.IsCancellationRequested)
            {
                await adHandlerBase.PlayAdAsync(adSource, startTimeout, cancellationToken, progress);
            }
        }

        static void ProcessPayload(IAdSource adSource)
        {
            if (adSource.Payload is IClipAdPayload)
            {
                adSource.Payload = CreateDocument((IClipAdPayload)adSource.Payload);
            }
            if (!(adSource.Payload is AdDocumentPayload))
            {
                throw new ArgumentException("adSource must contain a payload of type Stream", "adPayload");
            }
        }

        private static AdDocumentPayload CreateDocument(IClipAdPayload mediaSource)
        {
            var adDocument = new AdDocumentPayload();
            var adPod = new AdPod();
            var ad = new Ad();
            var linearCreative = new CreativeLinear();
            linearCreative.ClickThrough = mediaSource.ClickThrough;
            linearCreative.MediaFiles.Add(new MediaFile() { Type = mediaSource.MimeType, Value = mediaSource.MediaSource });
            ad.Creatives.Add(linearCreative);
            adPod.Ads.Add(ad);
            adDocument.AdPods.Add(adPod);
            return adDocument;
        }

#if SILVERLIGHT
        public Task<bool> CancelAd(bool force)
        {
            return adHandlerBase.CancelAd(force);
        }
#else
        public IAsyncOperation<bool> CancelAd(bool force)
        {
            return adHandlerBase.CancelAd(force);
        }
#endif
    }
}
