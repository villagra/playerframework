using System;
using System.Linq;
using Microsoft.Media.Advertising;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
#if !WINDOWS_PHONE
using System.Windows.Browser;
#endif
#else
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.Graphics.Display;
#endif

namespace Microsoft.PlayerFramework.Advertising
{
    internal class MediaPlayerAdapter : IPlayer, IDisposable
    {
#if SILVERLIGHT
        public event EventHandler FullscreenChanged;
        public event EventHandler DimensionsChanged;
        public event EventHandler VolumeChanged;
        public event EventHandler IsMutedChanged;
#else
        public event EventHandler<object> FullscreenChanged;
        public event EventHandler<object> DimensionsChanged;
        public event EventHandler<object> VolumeChanged;
        public event EventHandler<object> IsMutedChanged;
#endif

        protected MediaPlayer MediaPlayer { get; private set; }

        public MediaPlayerAdapter(MediaPlayer mediaPlayer)
        {
            MediaPlayer = mediaPlayer;
            MediaPlayer.VolumeChanged += MediaPlayer_VolumeChanged;
            MediaPlayer.IsFullScreenChanged += MediaPlayer_IsFullScreenChanged;
            MediaPlayer.IsMutedChanged += MediaPlayer_IsMutedChanged;
            MediaPlayer.SizeChanged += MediaPlayer_SizeChanged;
        }

        void MediaPlayer_IsMutedChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (IsMutedChanged != null) IsMutedChanged(this, EventArgs.Empty);
        }

#if SILVERLIGHT
        void MediaPlayer_VolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
#else
        void MediaPlayer_VolumeChanged(object sender, RoutedEventArgs e)
#endif
        {
            if (VolumeChanged != null) VolumeChanged(this, EventArgs.Empty);
        }

        void MediaPlayer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DimensionsChanged != null) DimensionsChanged(this, EventArgs.Empty);
        }

        void MediaPlayer_IsFullScreenChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (FullscreenChanged != null) FullscreenChanged(this, EventArgs.Empty);
        }

        public int CurrentBitrate { get; set; }

        public double Volume
        {
            get
            {
                return MediaPlayer.Volume;
            }
            set
            {
                MediaPlayer.Volume = value;
            }
        }

        public bool IsMuted
        {
            get
            {
                return MediaPlayer.IsMuted;
            }
            set
            {
                MediaPlayer.IsMuted = value;
            }
        }

        public bool IsFullScreen
        {
            get
            {
                return MediaPlayer.IsFullScreen;
            }
            set
            {
                MediaPlayer.IsFullScreen = value;
            }
        }

        public Size Dimensions
        {
            get
            {
#if NETFX_CORE
                double scale = 1.0;
#if !WINDOWS80
                switch (DisplayInformation.GetForCurrentView().ResolutionScale)
                {
                    case ResolutionScale.Scale120Percent:
                        scale = 1.2;
                        break;
                    case ResolutionScale.Scale150Percent:
                        scale = 1.5;
                        break;
                    case ResolutionScale.Scale160Percent:
                        scale = 1.6;
                        break;
                    case ResolutionScale.Scale225Percent:
                        scale = 2.25;
                        break;
#else
                switch (DisplayProperties.ResolutionScale)
                {
#endif
                    case ResolutionScale.Scale180Percent:
                        scale = 1.8;
                        break;
                    case ResolutionScale.Scale140Percent:
                        scale = 1.4;
                        break;
                }
                return new Size(Math.Round(MediaPlayer.ActualWidth * scale), Math.Round(MediaPlayer.ActualHeight * scale));
#elif WINDOWS_PHONE && !WINDOWS_PHONE7
                double scale = (double)Application.Current.Host.Content.ScaleFactor / 100;
                int w = (int)Math.Ceiling(MediaPlayer.ActualWidth * scale);
                int h = (int)Math.Ceiling(MediaPlayer.ActualHeight * scale);
                return new Size(w, h);
#else
                return new Size(MediaPlayer.ActualWidth, MediaPlayer.ActualHeight);
#endif
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        public TimeSpan CurrentPosition
        {
            get
            {
                return MediaPlayer.Position;
            }
        }

        public void Dispose()
        {
            MediaPlayer.VolumeChanged -= MediaPlayer_VolumeChanged;
            MediaPlayer.IsFullScreenChanged -= MediaPlayer_IsFullScreenChanged;
            MediaPlayer.IsMutedChanged -= MediaPlayer_IsMutedChanged;
            MediaPlayer.SizeChanged -= MediaPlayer_SizeChanged;
            MediaPlayer = null;
        }
    }
}
