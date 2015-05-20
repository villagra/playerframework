using System;
using Windows.Foundation;

namespace Microsoft.Media.Advertising
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
        event EventHandler<object> FullscreenChanged;
        event EventHandler<object> DimensionsChanged;
        event EventHandler<object> VolumeChanged;
        event EventHandler<object> IsMutedChanged;
    }
}
