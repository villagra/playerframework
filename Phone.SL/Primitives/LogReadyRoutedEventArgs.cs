#define CODE_ANALYSIS

using System.Windows;
using System.Windows.Media;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Represents the method that will handle the MediaPlayer.LogReady event.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Justification="MediaElement compatibility")]
    public delegate void LogReadyRoutedEventHandler(object sender, LogReadyRoutedEventArgs e);

    /// <summary>
    /// Provides data for the MediaPlayer.LogReady event.
    /// </summary>
    public sealed class LogReadyRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the MediaPlayer.LogReadyRoutedEventArgs class while providing the necessary info about the event.
        /// </summary>
        public LogReadyRoutedEventArgs(string log, LogSource logSource)
        {
            Log = log;
            LogSource = logSource;
        }

        /// <summary>
        /// Gets an XML string in the Windows Media Log format that contains the fields listed in the LogFields section.
        /// </summary>
        public string Log { get; private set; }
        
        /// <summary>
        ///  Gets a value that indicates why the log was generated.
        ///  Returns one of the enumeration values that indicates why the log was generated.
        /// </summary>
        public LogSource LogSource { get; private set; }
    }
}
