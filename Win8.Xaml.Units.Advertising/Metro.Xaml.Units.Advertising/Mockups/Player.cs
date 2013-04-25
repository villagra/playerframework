using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VideoAdvertising;
using Windows.Foundation;

namespace Microsoft.PlayerFramework.Units.Advertising.Mockups
{
    public class Player : IPlayer
    {
        public int CurrentBitrate { get; set; }

        Size dimensions;
        public Size Dimensions
        {
            get { return dimensions; }
            set
            {
                dimensions = value;
                if (DimensionsChanged != null) DimensionsChanged(this, EventArgs.Empty);
            }
        }

        bool isFullScreen;
        public bool IsFullScreen
        {
            get { return isFullScreen; }
            set
            {
                isFullScreen = value;
                if (FullscreenChanged != null) FullscreenChanged(this, EventArgs.Empty);
            }
        }

        double volume;
        public double Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                if (VolumeChanged != null) VolumeChanged(this, EventArgs.Empty);
            }
        }

        bool isMuted;
        public bool IsMuted
        {
            get { return isMuted; }
            set
            {
                isMuted = value;
                if (IsMutedChanged != null) IsMutedChanged(this, EventArgs.Empty);
            }
        }

        public TimeSpan CurrentPosition
        {
            get { return TimeSpan.Zero; }
        }

        public event EventHandler<object> FullscreenChanged;

        public event EventHandler<object> DimensionsChanged;

        public event EventHandler<object> VolumeChanged;

        public event EventHandler<object> IsMutedChanged;
    }
}
