using System;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.Foundation;
#endif

namespace Microsoft.VideoAdvertising
{
    public interface IPlayer
    {
        int CurrentBitrate { get; set; }
        // HACK: setters required to work from WinJS
        // i.e. avoids "read-only" exceptions when setting properties on the MediaPlayerAdapterBridge class in the Advertising Plugin
        Size Dimensions { get; set; }
        bool IsFullScreen { get; set; }
        double Volume { get; set; }
        bool IsMuted { get; set; }
        TimeSpan CurrentPosition { get; }

#if SILVERLIGHT
        event EventHandler FullscreenChanged;
        event EventHandler DimensionsChanged;
        event EventHandler VolumeChanged;
        event EventHandler IsMutedChanged;
#else
        event EventHandler<object> FullscreenChanged;
        event EventHandler<object> DimensionsChanged;
        event EventHandler<object> VolumeChanged;
        event EventHandler<object> IsMutedChanged;
#endif
    }
}
