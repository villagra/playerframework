using System;
using System.Linq;
using Windows.UI.Xaml.Media;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Provides extension methods to help save and restore the state of a playing MediaPlayer object. This is useful for Suspend/Resume.
    /// </summary>
    public static class MediaPlayerExtensions
    {
        /// <summary>
        /// Gets the state of the player so it can be restored later.
        /// </summary>
        /// <param name="player">The instance of the MediaPlayer to get the state from.</param>
        /// <returns>The state of the player to preserve.</returns>
        public static MediaPlayerState GetPlayerState(this MediaPlayer player)
        {
            var result = new MediaPlayerState();
            result.Position = player.Position;
            result.IsPaused = player.InteractiveViewModel.CurrentState == MediaElementState.Paused;

            var playlistPlugin = player.Plugins.OfType<PlaylistPlugin>().FirstOrDefault();
            if (playlistPlugin != null)
            {
                result.PlaylistItemIndex = playlistPlugin.CurrentPlaylistItemIndex;
            }
            return result;
        }

        /// <summary>
        /// Restores the state of the player to the correct playlistitem (if using playlists) and postion within the media.
        /// </summary>
        /// <param name="player">The instance of the MediaPlayer being restored.</param>
        /// <param name="state">The state of the player to restore.</param>
        public static void RestorePlayerState(this MediaPlayer player, MediaPlayerState state)
        {
            if (state == null) throw new ArgumentNullException("state");
            player.StartupPosition = state.Position;
            player.AutoPlay = !state.IsPaused;
            player.AutoLoad = true; // force to true

            var playlistPlugin = player.Plugins.OfType<PlaylistPlugin>().FirstOrDefault();
            if (playlistPlugin != null)
            {
                playlistPlugin.StartupPlaylistItemIndex = state.PlaylistItemIndex;
            }
        }
    }

    /// <summary>
    /// Represents the state of the player.
    /// </summary>
    public sealed class MediaPlayerState
    {
        /// <summary>
        /// Gets or sets a flag indicating if the player was paused.
        /// </summary>
        public bool IsPaused { get; set; }

        /// <summary>
        /// Gets or sets the position of the player.
        /// </summary>
        public TimeSpan Position { get; set; }

        /// <summary>
        /// Gets or sets the index of the current playlistitem.
        /// </summary>
        public int PlaylistItemIndex { get; set; }
    }
}
