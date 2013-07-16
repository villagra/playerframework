using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
#if SILVERLIGHT
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Globalization;
using System.ComponentModel;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// An plugin responsible for maintaining a playlist of media.
    /// The playlist items are automatically loaded into the MediaElement at the appropriate times.
    /// </summary>
    public class PlaylistPlugin : DependencyObject, IPlugin
    {
        /// <summary>
        /// Creates a new instance of PlaylistPlugin.
        /// </summary>
        public PlaylistPlugin()
        {
            SetValue(PlaylistProperty, new ObservableCollection<PlaylistItem>());
            StartupPlaylistItemIndex = 0;
        }

        /// <summary>
        /// Occurs when the CurrentPlaylistItem changes.
        /// </summary>
        public event EventHandler CurrentPlaylistItemChanged;

        /// <summary>
        /// Playlist DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty PlaylistProperty = DependencyProperty.Register("Playlist", typeof(ObservableCollection<PlaylistItem>), typeof(PlaylistPlugin), new PropertyMetadata(null, (d, e) => ((PlaylistPlugin)d).OnPlaylistChanged(e.OldValue as ObservableCollection<PlaylistItem>, e.NewValue as ObservableCollection<PlaylistItem>)));

        /// <summary>
        /// Gets a list of media items to play.
        /// </summary>
        public ObservableCollection<PlaylistItem> Playlist
        {
            get { return GetValue(PlaylistProperty) as ObservableCollection<PlaylistItem>; }
            set { SetValue(PlaylistProperty, value); }
        }

        void OnPlaylistChanged(ObservableCollection<PlaylistItem> oldPlaylist, ObservableCollection<PlaylistItem> newPlaylist)
        {
            if (oldPlaylist != null)
            {
                oldPlaylist.CollectionChanged -= playlist_CollectionChanged;
            }

            if (newPlaylist != null)
            {
                newPlaylist.CollectionChanged += playlist_CollectionChanged;
            }
        }

        void playlist_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshPreviousNextPlaylistItems();
        }

        /// <summary>
        /// AutoAdvance DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty AutoAdvanceProperty = DependencyProperty.Register("AutoAdvance", typeof(bool), typeof(PlaylistPlugin), new PropertyMetadata(true));

        /// <summary>
        /// Determines whether the next PlaylistItem should be automatically loaded when a PlaylistItem has ended. AutoPlay is still used to determine whether or not that next PlaylistItem should automatically start playing.
        /// </summary>
        public bool AutoAdvance
        {
            get
            {
                return (bool)GetValue(AutoAdvanceProperty);
            }
            set
            {
                SetValue(AutoAdvanceProperty, value);
            }
        }

        /// <summary>
        /// SkipBackThreshold DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty SkipBackThresholdProperty = DependencyProperty.Register("SkipBackThreshold", typeof(TimeSpan?), typeof(PlaylistPlugin), new PropertyMetadata(TimeSpan.FromSeconds(5)));

        /// <summary>
        /// The amount of time into the media after which a skip back operation is treated as a reset (vs. going to the previous playlist item).
        /// If not set, skip back will always reset the video intead of going to the previous playlist item.
        /// </summary>
        public TimeSpan? SkipBackThreshold
        {
            get
            {
                return (TimeSpan?)GetValue(SkipBackThresholdProperty);
            }
            set
            {
                SetValue(SkipBackThresholdProperty, value);
            }
        }

        /// <summary>
        /// CurrentPlaylistItem DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty CurrentPlaylistItemProperty = DependencyProperty.Register("CurrentPlaylistItem", typeof(PlaylistItem), typeof(PlaylistPlugin), new PropertyMetadata(null, (d, e) => ((PlaylistPlugin)d).OnCurrentPlaylistItemChanged(e.OldValue as PlaylistItem, e.NewValue as PlaylistItem)));

        /// <summary>
        /// Gets the current PlaylistItem.
        /// </summary>
        public PlaylistItem CurrentPlaylistItem
        {
            get { return GetValue(CurrentPlaylistItemProperty) as PlaylistItem; }
            set { SetValue(CurrentPlaylistItemProperty, value); }
        }

        void OnCurrentPlaylistItemChanged(PlaylistItem oldCurrentPlaylistItem, PlaylistItem newCurrentPlaylistItem)
        {
            RefreshPreviousNextPlaylistItems();

            UpdateMediaSource(MediaPlayer, newCurrentPlaylistItem);
            if (CurrentPlaylistItemChanged != null) CurrentPlaylistItemChanged(this, EventArgs.Empty);
        }

        private void RefreshPreviousNextPlaylistItems()
        {
            PreviousPlaylistItem = CurrentPlaylistItemIndex > 0 ? Playlist[CurrentPlaylistItemIndex - 1] : null;
            NextPlaylistItem = CurrentPlaylistItemIndex < Playlist.Count - 1 ? Playlist[CurrentPlaylistItemIndex + 1] : null;
        }

        /// <summary>
        /// PreviousPlaylistItem DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty PreviousPlaylistItemProperty = DependencyProperty.Register("PreviousPlaylistItem", typeof(PlaylistItem), typeof(PlaylistPlugin), null);

        /// <summary>
        /// Returns the PlaylistItem directly before the CurrentPlaylistItem (or null if none exist).
        /// </summary>
        public PlaylistItem PreviousPlaylistItem
        {
            get { return GetValue(PreviousPlaylistItemProperty) as PlaylistItem; }
            private set { SetValue(PreviousPlaylistItemProperty, value); }
        }

        /// <summary>
        /// NextPlaylistItem DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty NextPlaylistItemProperty = DependencyProperty.Register("NextPlaylistItem", typeof(PlaylistItem), typeof(PlaylistPlugin), null);

        /// <summary>
        /// Returns the PlaylistItem directly after the CurrentPlaylistItem (or null if none exist).
        /// </summary>
        public PlaylistItem NextPlaylistItem
        {
            get { return GetValue(NextPlaylistItemProperty) as PlaylistItem; }
            private set { SetValue(NextPlaylistItemProperty, value); }
        }

        void IPlugin.Load()
        {

        }

        void IPlugin.Update(IMediaSource mediaSource)
        {

        }

        void IPlugin.Unload()
        {
            CurrentPlaylistItem = null;
        }

        private MediaPlayer MediaPlayer;
        /// <summary>
        /// The current MediaPlayer
        /// </summary>
        MediaPlayer IPlugin.MediaPlayer
        {
            get { return MediaPlayer; }
            set
            {
                if (MediaPlayer != null)
                {
                    MediaPlayer.MediaEnded -= MediaPlayer_MediaEnded;
                    MediaPlayer.Initialized -= MediaPlayer_Initialized;
                    MediaPlayer.SkippingAhead -= MediaPlayer_SkippingAhead;
                    MediaPlayer.SkippingBack -= MediaPlayer_SkippingBack;
                }
                MediaPlayer = value;
                if (MediaPlayer != null)
                {
                    MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
                    MediaPlayer.Initialized += MediaPlayer_Initialized;
                    MediaPlayer.SkippingAhead += MediaPlayer_SkippingAhead;
                    MediaPlayer.SkippingBack += MediaPlayer_SkippingBack;
                }
            }
        }

        void MediaPlayer_Initialized(object sender, RoutedEventArgs e)
        {
            if (StartupPlaylistItemIndex.HasValue && Playlist != null && Playlist.Any() && StartupPlaylistItemIndex.Value < Playlist.Count)
            {
                var playlistItem = Playlist[StartupPlaylistItemIndex.Value];
                if (MediaPlayer.StartupPosition.HasValue) // apply the player's startup position to the playlistitem.
                {
                    playlistItem.StartupPosition = MediaPlayer.StartupPosition;
                }
                CurrentPlaylistItem = playlistItem;
            }
        }

        void MediaPlayer_SkippingBack(object sender, SkipRoutedEventArgs e)
        {
            if (e.Position == MediaPlayer.StartTime && (!SkipBackThreshold.HasValue || MediaPlayer.Position.Subtract(MediaPlayer.StartTime) < SkipBackThreshold.Value) && CurrentPlaylistItemIndex > 0)
            {
                GoToPreviousPlaylistItem();
                e.Canceled = true;
            }
        }

        void MediaPlayer_SkippingAhead(object sender, SkipRoutedEventArgs e)
        {
            if (e.Position == MediaPlayer.LivePosition.GetValueOrDefault(MediaPlayer.EndTime) && CurrentPlaylistItemIndex < Playlist.Count - 1)
            {
                GoToNextPlaylistItem();
                e.Canceled = true;
            }
        }

        /// <summary>
        /// Advances to the previous playlist item.
        /// </summary>
        public void GoToPreviousPlaylistItem()
        {
            CurrentPlaylistItem = Playlist[CurrentPlaylistItemIndex - 1];
        }

        /// <summary>
        /// Advances to the next playlist item.
        /// </summary>
        public void GoToNextPlaylistItem()
        {
            CurrentPlaylistItem = Playlist[CurrentPlaylistItemIndex + 1];
        }

        /// <summary>
        /// Gets or sets the index of the playlistitem that should start off with. Normally this is zero but can be set in scenarios where you are restoring the user back to the original state.
        /// </summary>
        public int? StartupPlaylistItemIndex { get; set; }

        /// <summary>
        /// Gets or sets the index of the new playlist item.
        /// </summary>
        public int CurrentPlaylistItemIndex
        {
            get { return Playlist.IndexOf(CurrentPlaylistItem); }
            set { CurrentPlaylistItem = Playlist[value]; }
        }

        void MediaPlayer_MediaEnded(object sender, MediaPlayerActionEventArgs e)
        {
            if (!MediaPlayer.IsLooping && AutoAdvance)
            {
                var index = CurrentPlaylistItemIndex;
                index++;
                if (index < Playlist.Count)
                {
                    CurrentPlaylistItemIndex = index;
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Updates the media source on the player. This is called internally when a new playlist item is selected.
        /// </summary>
        /// <param name="mediaPlayer">The MediaPlayer to load the media source (playlist item) into.</param>
        /// <param name="mediaSource">The new media source (this is usually a PlaylistItem object).</param>
        public static void UpdateMediaSource(MediaPlayer mediaPlayer, IMediaSource mediaSource)
        {
            if (mediaSource == null)
            {
#if SILVERLIGHT
                mediaPlayer.LicenseAcquirer = new LicenseAcquirer();
#else
                mediaPlayer.ProtectionManager = null;
                mediaPlayer.Stereo3DVideoPackingMode = Stereo3DVideoPackingMode.None;
                mediaPlayer.Stereo3DVideoRenderMode = Stereo3DVideoRenderMode.Mono;
#endif
                mediaPlayer.VisualMarkers.Clear();
                mediaPlayer.AvailableAudioStreams.Clear();
                mediaPlayer.AvailableCaptions.Clear();
                mediaPlayer.PosterSource = null;
                mediaPlayer.Close();
            }
            else
            {
                var playlistItem = mediaSource as PlaylistItem;

#if SILVERLIGHT
                mediaPlayer.LicenseAcquirer = mediaSource.LicenseAcquirer ?? new LicenseAcquirer();
#else
                mediaPlayer.ProtectionManager = mediaSource.ProtectionManager;
                mediaPlayer.Stereo3DVideoPackingMode = mediaSource.Stereo3DVideoPackingMode;
                mediaPlayer.Stereo3DVideoRenderMode = mediaSource.Stereo3DVideoRenderMode;
#endif
                mediaPlayer.PosterSource = mediaSource.PosterSource;
                mediaPlayer.AutoLoad = mediaSource.AutoLoad;
                mediaPlayer.AutoPlay = mediaSource.AutoPlay;
                mediaPlayer.StartupPosition = mediaSource.StartupPosition;
                mediaPlayer.VisualMarkers.Clear();
                foreach (var marker in mediaSource.VisualMarkers)
                {
                    mediaPlayer.VisualMarkers.Add(marker);
                }
                mediaPlayer.AvailableAudioStreams.Clear();
                foreach (var audioStream in mediaSource.AvailableAudioStreams)
                {
                    mediaPlayer.AvailableAudioStreams.Add(audioStream);
                }
                mediaPlayer.AvailableCaptions.Clear();
                foreach (var caption in mediaSource.AvailableCaptions)
                {
                    mediaPlayer.AvailableCaptions.Add(caption);
                }

                if (mediaSource.Source != null)
                {
                    mediaPlayer.Source = mediaSource.Source;
                }
#if NETFX_CORE
                else if (playlistItem != null && playlistItem.SourceStream != null)
                {
                    mediaPlayer.SetSource(playlistItem.SourceStream, playlistItem.MimeType);
                }
#else
                else if (playlistItem != null && playlistItem.SourceStream != null)
                {
                    mediaPlayer.SetSource(playlistItem.SourceStream);
                }
                else if (playlistItem != null && playlistItem.MediaStreamSource != null)
                {
                    mediaPlayer.SetSource(playlistItem.MediaStreamSource);
                }
#endif
                else
                {
                    mediaPlayer.Source = null;
                }
            }

            foreach (var plugin in mediaPlayer.Plugins)
            {
                plugin.Update(mediaSource);
            }
        }
    }

    /// <summary>
    ///  Helper class used to provide extension methods for the playlist plugin.
    /// </summary>
    public static class PlaylistPluginExtensions
    {
        /// <summary>
        /// A helper method to return the instance of the PlaylistPlugin associated with a MediaPlayer instance.
        /// </summary>
        /// <param name="source">The instance of MediaPlayer to return the associated PlaylistPlugin.</param>
        /// <returns>The instance of the PlaylistPlugin associated with the MediaPlayer</returns>
        public static PlaylistPlugin GetPlaylistPlugin(this MediaPlayer source)
        {
            return source.Plugins.OfType<PlaylistPlugin>().FirstOrDefault();
        }
    }
}
