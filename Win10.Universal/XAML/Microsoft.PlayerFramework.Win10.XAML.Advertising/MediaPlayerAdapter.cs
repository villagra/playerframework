using System;
using System.Linq;
using Microsoft.Media.Advertising;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.Graphics.Display;

namespace Microsoft.PlayerFramework.Advertising
{
    internal class MediaPlayerAdapter : IPlayer, IDisposable
    {
        public event EventHandler<object> FullscreenChanged;
        public event EventHandler<object> DimensionsChanged;
        public event EventHandler<object> VolumeChanged;
        public event EventHandler<object> IsMutedChanged;

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

        void MediaPlayer_VolumeChanged(object sender, RoutedEventArgs e)
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
                var scale = (double)(int)DisplayInformation.GetForCurrentView().ResolutionScale / 100;
                return new Size(Math.Round(MediaPlayer.ActualWidth * scale), Math.Round(MediaPlayer.ActualHeight * scale));
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
