using System.Windows;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Represents methods that will handle various routed events related to timeline markers.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void TimelineMarkerRoutedEventHandler(object sender, TimelineMarkerRoutedEventArgs e);

    /// <summary>
    /// Provides event data for the System.Windows.Controls.MediaElement.MarkerReached event.
    /// </summary>
    public sealed class TimelineMarkerRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets the System.Windows.Media.TimelineMarker that triggered this event.
        /// </summary>
        public System.Windows.Media.TimelineMarker Marker { get; set; }
    }
}
