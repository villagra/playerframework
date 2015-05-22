using System;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Contains state information and event data associated with the SelectedAudioStreamChanged event.
    /// </summary>
    public class SelectedAudioStreamChangedEventArgs : RoutedPropertyChangedEventArgs<AudioStream>
    {
        internal SelectedAudioStreamChangedEventArgs(AudioStream oldValue, AudioStream newValue) : base(oldValue, newValue)
        { }

        /// <summary>
        /// Gets or sets whether the event handler took responsibility for modifying the selected audio stream.
        /// Setting to true will prevent the MediaPlayer from setting the AudioStreamIndex property automatically.
        /// </summary>
        public bool Handled { get; set; }
    }
}
