using Microsoft.Media.Advertising;
using System;
using System.Linq;
using System.Collections.Generic;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.UI.Xaml.Media;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// Provides a default implementation of IAdPlayerFactoryPlugin that is capable of serving linear video ads or nonlinear image ads.
    /// </summary>
    public class AdPlayerFactoryPlugin : IAdPlayerFactoryPlugin
    {
        /// <summary>
        /// Gets the collection MIME types supported for video ads.
        /// </summary>
        public IList<string> SupportedVideoMimeTypes { get; private set; }

        /// <summary>
        /// Gets or sets at which the point at which ads can be skipped.
        /// Note: This is used only if VAST 3.0 does not specify this already.
        /// Can be in TimeSpan format or percentage.
        /// Default = null
        /// </summary>
        public FlexibleOffset SkippableOffset { get; set; }

        /// <summary>
        /// Gets or sets if the plugin is enabled.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a custom style to be used for the VpaidVideoAdPlayer
        /// </summary>
        public Style VpaidVideoAdPlayerStyle { get; set; }

        /// <summary>
        /// Gets or sets a custom style to be used for the VpaidImageAdPlayer
        /// </summary>
        public Style VpaidImageAdPlayerStyle { get; set; }

        /// <summary>
        /// Gets or sets a custom style to be used for the VpaidWebAdPlayer (For IFrame and HTML nonlinear resources)
        /// </summary>
        public Style VpaidWebAdPlayerStyle { get; set; }

        /// <summary>
        /// Creates a new instance of AdPlayerFactoryPlugin
        /// </summary>
        public AdPlayerFactoryPlugin()
        {
            SupportedVideoMimeTypes = new List<string>(new[] { "video/x-ms-wmv", "video/mp4" });
            IsEnabled = true;
        }

        /// <inheritdoc /> 
        public IVpaid GetPlayer(ICreativeSource creativeSource)
        {
            if (!IsEnabled || creativeSource.MimeType == null) return null;
            var skippableOffset = creativeSource.SkippableOffset ?? SkippableOffset;
            if (creativeSource.Type == CreativeSourceType.Linear)
            {
                if (creativeSource.ApiFramework == "VPAID")
                {
                    //switch (creativeSource.MimeType)
                    //{
                    //    case "application/x-javascript":
                    //        return new VpaidJavaScriptAdPlayer(skippableOffset, creativeSource.Duration, creativeSource.ClickUrl) { Style = VpaidWebAdPlayerStyle };
                    //}
                }
                else
                {
#if SILVERLIGHT
                    if (SupportedVideoMimeTypes.Contains(creativeSource.MimeType.ToLowerInvariant()))
#else
                    if (SupportedVideoMimeTypes.Contains(creativeSource.MimeType.ToLowerInvariant()) || CanPlayCodec(creativeSource.Codec))
#endif
                    {
                        return new VpaidVideoAdPlayer(skippableOffset, creativeSource.Duration, creativeSource.ClickUrl) 
                        { 
                            Style = VpaidVideoAdPlayerStyle,
#if NETFX_CORE
                            AudioCategory = MediaPlayer.AudioCategory
#endif
                        };
                    }
                }
            }
            else if (creativeSource.Type == CreativeSourceType.NonLinear)
            {
                switch (creativeSource.MediaSourceType)
                {
                    case MediaSourceEnum.Static:
#if SILVERLIGHT
                        if (creativeSource.MimeType.ToLowerInvariant().StartsWith("image/"))
#else
                        if (BitmapDecoder.GetDecoderInformationEnumerator().SelectMany(d => d.MimeTypes).Select(m => m.ToLowerInvariant()).Contains(creativeSource.MimeType.ToLowerInvariant()))
#endif
                        {
                            return new VpaidImageAdPlayer(skippableOffset, creativeSource.Duration, creativeSource.ClickUrl) { Style = VpaidImageAdPlayerStyle };
                        }
                        else return null;
                    case MediaSourceEnum.IFrame:
                        return new VpaidIFrameAdPlayer(skippableOffset, creativeSource.Duration, creativeSource.ClickUrl, creativeSource.Dimensions) { Style = VpaidWebAdPlayerStyle };
                    case MediaSourceEnum.HTML:
                        return new VpaidHtmlAdPlayer(skippableOffset, creativeSource.Duration, creativeSource.ClickUrl, creativeSource.Dimensions) { Style = VpaidWebAdPlayerStyle };
                }
            }
            return null;
        }
#if !SILVERLIGHT
        private bool CanPlayCodec(string codec)
        {
            if (!string.IsNullOrEmpty(codec))
            {
                var canPlayCodecResponse = MediaPlayer.CanPlayType(codec);
                return canPlayCodecResponse == MediaCanPlayResponse.Maybe || canPlayCodecResponse == MediaCanPlayResponse.Probably;
            }
            return false;
        }
#endif

        void IPlugin.Load() { }

        void IPlugin.Update(IMediaSource mediaSource) { }

        void IPlugin.Unload() { }

        /// <inheritdoc /> 
        public MediaPlayer MediaPlayer { get; set; }
    }
}
