using System;
using System.Linq;
using Windows.Media;
using Windows.UI.Xaml;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A plugin used to connect the Windows 8 media controls with the current media.
    /// </summary>
    public class MediaControlPlugin: PluginBase
    {
        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            MediaControl.PlayPauseTogglePressed += MediaControl_PlayPauseTogglePressed;
            MediaControl.PlayPressed += MediaControl_PlayPressed;
            MediaControl.PausePressed += MediaControl_PausePressed;
            MediaControl.StopPressed += MediaControl_StopPressed;
            MediaControl.SoundLevelChanged += MediaControl_SoundLevelChanged;
            MediaControl.RewindPressed += MediaControl_RewindPressed;
            MediaControl.FastForwardPressed += MediaControl_FastForwardPressed;
            RefreshTrackButtonStates();

            if (PlaylistPlugin != null)
            {
                PlaylistPlugin.Playlist.CollectionChanged += Playlist_CollectionChanged;
                PlaylistPlugin.CurrentPlaylistItemChanged += PlaylistPlugin_CurrentPlaylistItemChanged;
            }
            if (MediaPlayer.InteractiveViewModel != null)
            {
                MediaPlayer.InteractiveViewModel.IsPlayResumeEnabledChanged += InteractiveViewModel_IsPlayResumeEnabledChanged;
            }
            MediaPlayer.InteractiveViewModelChanged += MediaPlayer_InteractiveViewModelChanged;

            return true;
        }

        /// <inheritdoc /> 
        protected override void OnDeactivate()
        {
            if (PlaylistPlugin != null)
            {
                PlaylistPlugin.Playlist.CollectionChanged -= Playlist_CollectionChanged;
                PlaylistPlugin.CurrentPlaylistItemChanged -= PlaylistPlugin_CurrentPlaylistItemChanged;
            }

            MediaControl.PlayPauseTogglePressed -= MediaControl_PlayPauseTogglePressed;
            MediaControl.PlayPressed -= MediaControl_PlayPressed;
            MediaControl.PausePressed -= MediaControl_PausePressed;
            MediaControl.StopPressed -= MediaControl_StopPressed;
            MediaControl.SoundLevelChanged -= MediaControl_SoundLevelChanged;
            MediaControl.RewindPressed -= MediaControl_RewindPressed;
            MediaControl.FastForwardPressed -= MediaControl_FastForwardPressed;
            IsNextTrackEnabled = false;
            IsPreviousTrackEnabled = false;

            MediaPlayer.InteractiveViewModelChanged -= MediaPlayer_InteractiveViewModelChanged;
            if (MediaPlayer.InteractiveViewModel != null)
            {
                MediaPlayer.InteractiveViewModel.IsPlayResumeEnabledChanged -= InteractiveViewModel_IsPlayResumeEnabledChanged;
            }
        }

        /// <summary>
        /// Refreshes the next and previous track buttons based on whether a next or previous track exists.
        /// </summary>
        protected void RefreshTrackButtonStates()
        {
            IsNextTrackEnabled = NextTrackExists;
            IsPreviousTrackEnabled = PreviousTrackExists;
        }

        bool isPreviousTrackEnabled;
        bool IsPreviousTrackEnabled
        {
            get { return isPreviousTrackEnabled; }
            set
            {
                if (isPreviousTrackEnabled != value)
                {
                    isPreviousTrackEnabled = value;
                    if (isPreviousTrackEnabled)
                    {
                        MediaControl.PreviousTrackPressed += MediaControl_PreviousTrackPressed;
                    }
                    else
                    {
                        MediaControl.PreviousTrackPressed -= MediaControl_PreviousTrackPressed;
                    }
                }
            }
        }

        bool isNextTrackEnabled;
        bool IsNextTrackEnabled
        {
            get { return isNextTrackEnabled; }
            set {
                if (isNextTrackEnabled != value)
                {
                    isNextTrackEnabled = value;
                    if (isNextTrackEnabled)
                    {
                        MediaControl.NextTrackPressed += MediaControl_NextTrackPressed;
                    }
                    else
                    {
                        MediaControl.NextTrackPressed -= MediaControl_NextTrackPressed;
                    }
                }
            }
        }

        /// <summary>
        /// Gets if a next track currently exists in the playlist.
        /// </summary>
        protected virtual bool NextTrackExists
        {
            get 
            {
                return PlaylistPlugin != null && PlaylistPlugin.NextPlaylistItem != null;
            }
        }

        /// <summary>
        /// Gets if a previous track currently exists in the playlist.
        /// </summary>
        protected virtual bool PreviousTrackExists
        {
            get
            {
                return PlaylistPlugin != null && PlaylistPlugin.PreviousPlaylistItem != null;
            }
        }

        void PlaylistPlugin_CurrentPlaylistItemChanged(object sender, EventArgs e)
        {
            RefreshTrackButtonStates();
        }

        void Playlist_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshTrackButtonStates();
        }

        PlaylistPlugin PlaylistPlugin
        {
            get
            {
                return MediaPlayer.Plugins.OfType<PlaylistPlugin>().FirstOrDefault();
            }
        }

        void MediaPlayer_InteractiveViewModelChanged(object sender, RoutedPropertyChangedEventArgs<IInteractiveViewModel> e)
        {
            if (e.OldValue != null)
            {
                e.OldValue.IsPlayResumeEnabledChanged -= InteractiveViewModel_IsPlayResumeEnabledChanged;
            }

            if (e.NewValue != null)
            {
                e.NewValue.IsPlayResumeEnabledChanged += InteractiveViewModel_IsPlayResumeEnabledChanged;
            }
        }

        void InteractiveViewModel_IsPlayResumeEnabledChanged(object sender, RoutedEventArgs e)
        {
            MediaControl.IsPlaying = !MediaPlayer.InteractiveViewModel.IsPlayResumeEnabled;
        }

        async void MediaControl_FastForwardPressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (base.IsActive)
                {
                    if (MediaPlayer.InteractiveViewModel.IsFastForwardEnabled)
                    {
                        MediaPlayer.InteractiveViewModel.IncreasePlaybackRate();
                    }
                }
            });
        }

        async void MediaControl_RewindPressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (base.IsActive)
                {
                    if (MediaPlayer.InteractiveViewModel.IsRewindEnabled)
                    {
                        MediaPlayer.InteractiveViewModel.DecreasePlaybackRate();
                    }
                }
            });
        }

        void MediaControl_SoundLevelChanged(object sender, object e)
        {
            // do nothing
        }

        async void MediaControl_NextTrackPressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (base.IsActive)
                {
                    PlaylistPlugin.GoToNextPlaylistItem();
                }
            });
        }

        async void MediaControl_PreviousTrackPressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (base.IsActive)
                {
                    PlaylistPlugin.GoToPreviousPlaylistItem();
                }
            });
        }

        async void MediaControl_StopPressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (base.IsActive)
                {
                    if (MediaPlayer.InteractiveViewModel.IsStopEnabled)
                    {
                        MediaPlayer.InteractiveViewModel.Stop();
                    }
                }
            });
        }

        async void MediaControl_PausePressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (base.IsActive)
                {
                    if (MediaPlayer.InteractiveViewModel.IsPauseEnabled)
                    {
                        MediaPlayer.InteractiveViewModel.Pause();
                    }
                }
            });
        }

        async void MediaControl_PlayPressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (base.IsActive)
                {
                    if (MediaPlayer.InteractiveViewModel.IsPlayResumeEnabled)
                    {
                        MediaPlayer.InteractiveViewModel.PlayResume();
                    }
                }
            });
        }

        async void MediaControl_PlayPauseTogglePressed(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (base.IsActive)
                {
                    if (MediaPlayer.InteractiveViewModel.IsPlayResumeEnabled)
                    {
                        MediaPlayer.InteractiveViewModel.PlayResume();
                    }
                    else if (MediaPlayer.InteractiveViewModel.IsPauseEnabled)
                    {
                        MediaPlayer.InteractiveViewModel.Pause();
                    }
                }
            });
        }
    }
}
