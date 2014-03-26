#define CODE_ANALYSIS

using System.Windows;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Represents the method that will handle the System.Windows.Controls.MediaElement.RateChanged.
    /// </summary>
    /// <param name="sender">The instance of MediaPlayer that raised the event.</param>
    /// <param name="e">The EventArgs containing information about the new rate.</param>
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Justification = "MediaElement compatibility")]
    public delegate void RateChangedRoutedEventHandler(object sender, RateChangedRoutedEventArgs e);

    /// <summary>
    /// Provides data for the MediaPlayer.RateChanged event.
    /// </summary>
    public sealed class RateChangedRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instances of the MediaPlayer.RateChangedRoutedEventArgs class with the new rate.
        /// </summary>
        public RateChangedRoutedEventArgs(double newRate)
        {
            NewRate = newRate;
        }

        /// <summary>
        /// Gets the new rate
        /// </summary>
        public double NewRate { get; private set; }
    }
}
