using System;
using System.Windows.Input;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Provides a set of commands that can be used to control the PlaylistPlugin. Useful for binding in Xaml
    /// </summary>
    public class PlaylistCommands : DependencyObject
    {
        /// <summary>
        /// Creates a new instance of the PlaylistCommands class.
        /// </summary>
        public PlaylistCommands()
        {
            previousPlaylistItemCommand = new DelegateCommand(() =>
            {
                PlaylistPlugin.GoToPreviousPlaylistItem();
            });
            nextPlaylistItemCommand = new DelegateCommand(() =>
            {
                PlaylistPlugin.GoToNextPlaylistItem();
            });
        }

        ICommand previousPlaylistItemCommand;
        /// <summary>
        /// Gets a command to return to the previous playlist item.
        /// </summary>
        public ICommand PreviousPlaylistItemCommand
        {
            get { return previousPlaylistItemCommand; }
        }

        ICommand nextPlaylistItemCommand;
        /// <summary>
        /// Gets a command to advance to the next playlist item.
        /// </summary>
        public ICommand NextPlaylistItemCommand
        {
            get { return nextPlaylistItemCommand; }
        }

        /// <summary>
        /// Identifies the PlaylistPlugin dependency property.
        /// </summary>
        public static readonly DependencyProperty PlaylistPluginProperty = DependencyProperty.Register("PlaylistPlugin", typeof(PlaylistPlugin), typeof(PlaylistCommands), null);
        /// <summary>
        /// Gets or sets the instance of the PlaylistPlugin that the commands should operate on.
        /// </summary>
        public PlaylistPlugin PlaylistPlugin
        {
            get { return (PlaylistPlugin)GetValue(PlaylistPluginProperty); }
            set { SetValue(PlaylistPluginProperty, value); }
        }
    }
}
