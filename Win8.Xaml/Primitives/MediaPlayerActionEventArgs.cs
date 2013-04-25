using System;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Represents the method that will handle a MediaPlayer action.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void MediaPlayerActionEventHandler(object sender, MediaPlayerActionEventArgs e);

    /// <summary>
    /// Contains state information and event data associated with a MediaPlayer action event.
    /// </summary>
    public class MediaPlayerActionEventArgs : RoutedEventArgs
    {
        internal MediaPlayerActionEventArgs() { }

        /// <summary>
        /// Gets or sets whether the event was already handled.
        /// </summary>
        public bool Handled { get; set; }
    }

    /// <summary>
    /// Represents the method that will handle a MediaPlayer action associated with a position.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void MediaPlayerPositionActionEventHandler(object sender, MediaPlayerPositionActionEventArgs e);

    /// <summary>
    /// Contains state information and event data associated with a MediaPlayer position action event.
    /// </summary>
    public class MediaPlayerPositionActionEventArgs : MediaPlayerActionEventArgs
    {
        internal MediaPlayerPositionActionEventArgs(TimeSpan position)
        {
            Position = position;
        }

        /// <summary>
        /// Gets the position associated with the event.
        /// </summary>
        public TimeSpan Position { get; internal set; }
    }

    /// <summary>
    /// Represents the method that will handle a MediaPlayer action associated with a marker.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void MediaPlayerMarkerActionEventHandler(object sender, MediaPlayerMarkerActionEventArgs e);

    /// <summary>
    /// Contains state information and event data associated with a MediaPlayer marker action event.
    /// </summary>
    public class MediaPlayerMarkerActionEventArgs : MediaPlayerActionEventArgs
    {
        internal MediaPlayerMarkerActionEventArgs(VisualMarker marker)
        {
            Marker = marker;
        }

        /// <summary>
        /// Gets the marker associated with the event.
        /// </summary>
        public VisualMarker Marker { get; internal set; }
    }

    internal static class MediaPlayerActionEventExtensions
    {
        public static bool Notify(this MediaPlayerActionEventHandler handler, object sender)
        {
            if (handler != null)
            {
                var e = new MediaPlayerActionEventArgs();
                handler(sender, e);
                return e.Handled;
            }
            else
            {
                return true;
            }
        }
    }
}
