using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VideoAdvertising;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
#if !WINDOWS_PHONE
using System.Windows.Browser;
#endif
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Media;
using Windows.System;
#endif

namespace Microsoft.PlayerFramework.Advertising
{
#if MEF
    [System.ComponentModel.Composition.PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [System.ComponentModel.Composition.Export(typeof(IPlugin))]
#endif
    /// <summary>
    /// The main player framework plugin to handle ads. Ads can come from various scheduler plugins or be called directly.
    /// </summary>
    public class AdHandlerPlugin : AdHandlerBase, IPlugin
    {
        public AdHandlerPlugin()
        {
            AutoLoadAdPlayerFactoryPlugin = true;
        }

        int preferredBitrate;
        /// <summary>
        /// the preferred bitrate for ads (in bps NOT kbps).
        /// </summary>
        public int PreferredBitrate
        {
            get { return preferredBitrate; }
            set
            {
                preferredBitrate = value;
                if (Player != null)
                {
                    Player.CurrentBitrate = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the AdPlayerFactoryPlugin should be automatically added to the plugin collection. Set to false if you are providing your own.
        /// </summary>
        public bool AutoLoadAdPlayerFactoryPlugin { get; set; }

        /// <inheritdoc /> 
        protected override void UnloadPlayer(IVpaid adPlayer)
        {
            base.UnloadPlayer(adPlayer);
            var uiElement = adPlayer as UIElement;
            MediaPlayer.RemoveInteractiveElement(uiElement);
        }

        /// <inheritdoc /> 
        protected override void LoadPlayer(IVpaid adPlayer)
        {
            // set visibility to support preloading. MediaElement won't work unless it is contained in a visible parent
            AdContainer.Visibility = Visibility.Visible;
            base.LoadPlayer(adPlayer);
            var uiElement = adPlayer as UIElement;
            MediaPlayer.AddInteractiveElement(uiElement);
        }

        /// <inheritdoc /> 
        protected override IVpaid GetPlayer(ICreativeSource creativeSource)
        {
            IVpaid result = null;
            // look for ad player factories in the plugin collection. Try each one until you find a player
            foreach (var factory in MediaPlayer.Plugins.OfType<IAdPlayerFactoryPlugin>())
            {
                var player = factory.GetPlayer(creativeSource);
                if (player != null)
                {
                    result = player;
                    break;
                }
            }
            return result;
        }

        /// <inheritdoc /> 
        protected override FrameworkElement GetCompanionContainer(ICompanionSource source)
        {
            FrameworkElement container = null;
            if (!string.IsNullOrEmpty(source.AdSlotId))
            {
                container = MediaPlayer.Containers.OfType<FrameworkElement>().FirstOrDefault(f => f.Name == source.AdSlotId);
            }
            if (container == null && source.Width.HasValue && source.Height.HasValue)
            {
                container = MediaPlayer.Containers.OfType<FrameworkElement>().FirstOrDefault(f => f.Width == source.Width && f.Height == source.Height);
            }
            return container;
        }

#if WINDOWS_PHONE7
        MediaState playerState;

        /// <inheritdoc /> 
        protected override void SetAdvertisingState(AdState adState)
#else
        /// <inheritdoc /> 
        protected override void SetAdvertisingState(AdState adState)
#endif
        {
            var newValue = ConvertAdState(adState);
            var oldValue = MediaPlayer.AdvertisingState;

            if (newValue != oldValue)
            {
                if (MediaPlayer.PlayerState == PlayerState.Started)
                {
                    // pause the MediaPlayer if we're playing a linear ad or loading an ad, resume if the opposite
                    if ((newValue == AdvertisingState.Loading || newValue == AdvertisingState.Linear) && (oldValue == AdvertisingState.None || oldValue == AdvertisingState.NonLinear))
                    {
#if WINDOWS_PHONE7
                        playerState = MediaPlayer.GetMediaState();
                        MediaPlayer.Close();
#else
                        MediaPlayer.Pause();
#endif
                    }
                    else if ((oldValue == AdvertisingState.Loading || oldValue == AdvertisingState.Linear) && (newValue == AdvertisingState.None || newValue == AdvertisingState.NonLinear))
                    {
#if WINDOWS_PHONE7
                        MediaPlayer.RestoreMediaState(playerState);
#else
                        MediaPlayer.Play();
#endif
                    }
                }

                if (newValue == AdvertisingState.Loading)
                {
                    AdContainer.Visibility = Visibility.Visible;
                }
                else if (newValue == AdvertisingState.None)
                {
                    AdContainer.Visibility = Visibility.Collapsed;
                }

                // let the MediaPlayer update its visualstate
                MediaPlayer.AdvertisingState = newValue;
            }

            switch (newValue)
            {
                case AdvertisingState.Linear:
                    MediaPlayer.InteractiveViewModel = new VpaidLinearAdViewModel(ActiveAdPlayer, MediaPlayer);
                    break;
                case AdvertisingState.NonLinear:
                    MediaPlayer.InteractiveViewModel = new VpaidNonLinearAdViewModel(ActiveAdPlayer, MediaPlayer);
                    break;
                default:
                    MediaPlayer.InteractiveViewModel = MediaPlayer.DefaultInteractiveViewModel;
                    break;
            }
        }

        static AdvertisingState ConvertAdState(AdState adState)
        {
            switch (adState)
            {
                case AdState.Linear: return AdvertisingState.Linear;
                case AdState.NonLinear: return AdvertisingState.NonLinear;
                case AdState.Loading: return AdvertisingState.Loading;
                case AdState.None: return AdvertisingState.None;
                default: throw new NotImplementedException();
            }
        }

        void MediaPlayer_PlayerStateChanged(object sender, RoutedPropertyChangedEventArgs<PlayerState> e)
        {
            // hide the main media container for pre-rolls. Revert back once the media has started
            var mediaContainer = MediaPlayer.Containers.OfType<FrameworkElement>().FirstOrDefault(f => f.Name == MediaPlayerTemplateParts.MediaContainer);
            if (mediaContainer != null)
            {
                if (e.NewValue == PlayerState.Loaded && MediaPlayer.AutoPlay)
                {
#if WINDOWS_PHONE
                    mediaContainer.Opacity = 0;
#else
                    mediaContainer.Visibility = Visibility.Collapsed;
#endif
                }
                else if (e.NewValue != PlayerState.Opened && e.NewValue != PlayerState.Starting)
                {
#if WINDOWS_PHONE
                    mediaContainer.Opacity = 1;
#else
                    mediaContainer.Visibility = Visibility.Visible;
#endif
                }
            }
        }

        void MediaPlayer_MediaClosed(object sender, RoutedEventArgs e)
        {
            // always close all active ads when the media is closed
            var task = CancelActiveAds();
            UnloadCompanions();
        }

        void IPlugin.Load()
        {
            base.WireController();
            Player = new MediaPlayerAdapter(MediaPlayer) { CurrentBitrate = PreferredBitrate };
            AdContainer = MediaPlayer.Containers.OfType<Panel>().FirstOrDefault(f => f.Name == MediaPlayerTemplateParts.AdvertisingContainer);

            // look for adhandler in the plugin collection first
            foreach (var handler in MediaPlayer.Plugins.OfType<IAdPayloadHandler>())
            {
                AdHandlers.Add(handler);
            }

            MediaPlayer.PlayerStateChanged += MediaPlayer_PlayerStateChanged;
            MediaPlayer.MediaClosed += MediaPlayer_MediaClosed;

#if !MEF
            if (AutoLoadAdPlayerFactoryPlugin)
            {
                MediaPlayer.Plugins.Add(new AdPlayerFactoryPlugin());
            }
#endif
        }

        void IPlugin.Update(IMediaSource mediaSource)
        {
            // do nothing
        }

        void IPlugin.Unload()
        {
            MediaPlayer.MediaClosed -= MediaPlayer_MediaClosed;
            MediaPlayer.PlayerStateChanged -= MediaPlayer_PlayerStateChanged;
            base.UnwireController();
        }

        /// <inheritdoc /> 
        public MediaPlayer MediaPlayer { get; set; }
    }
}
