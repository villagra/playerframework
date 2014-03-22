using System;
using System.Linq;
using Windows.Media;
using Windows.UI.Xaml;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A plugin used to connect the Windows 8.1 system media transport controls with the current media.
    /// </summary>
    public class SystemTransportControlsPlugin : PluginBase
    {
        /// <summary>
        /// Gets the instance of the SystemMediaTransportControls being used.
        /// </summary>
        public SystemMediaTransportControls SystemControls { get; private set; }

        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                SystemControls = SystemMediaTransportControls.GetForCurrentView();
                SystemControls.ButtonPressed += SystemControls_ButtonPressed;
                RefreshFastForwardState();
                RefreshRewindState();
                RefreshStopState();
                RefreshPlayState();
                RefreshPauseState();
                RefreshNextState();
                RefreshPreviousState();
                RefreshPlaybackStatus();

                if (PlaylistPlugin != null)
                {
                    PlaylistPlugin.Playlist.CollectionChanged += Playlist_CollectionChanged;
                    PlaylistPlugin.CurrentPlaylistItemChanged += PlaylistPlugin_CurrentPlaylistItemChanged;
                }
                WireEvents(MediaPlayer.InteractiveViewModel);
                MediaPlayer.InteractiveViewModelChanged += MediaPlayer_InteractiveViewModelChanged;
                return true;
            }
            else return false;
        }

        /// <inheritdoc /> 
        protected override void OnDeactivate()
        {
            if (PlaylistPlugin != null)
            {
                PlaylistPlugin.Playlist.CollectionChanged -= Playlist_CollectionChanged;
                PlaylistPlugin.CurrentPlaylistItemChanged -= PlaylistPlugin_CurrentPlaylistItemChanged;
            }

            IsNextTrackEnabled = false;
            IsPreviousTrackEnabled = false;

            MediaPlayer.InteractiveViewModelChanged -= MediaPlayer_InteractiveViewModelChanged;
            UnwireEvents(MediaPlayer.InteractiveViewModel);

            SystemControls.ButtonPressed -= SystemControls_ButtonPressed;
            SystemControls = null;
        }

        void WireEvents(IInteractiveViewModel viewModel)
        {
            if (viewModel != null)
            {
                viewModel.CurrentStateChanged += Player_CurrentStateChanged;
                viewModel.IsStopEnabledChanged += Player_IsStopEnabledChanged;
                viewModel.IsRewindEnabledChanged += Player_IsRewindEnabledChanged;
                viewModel.IsFastForwardEnabledChanged += Player_IsFastForwardEnabledChanged;
                viewModel.IsPlayResumeEnabledChanged += Player_IsPlayResumeEnabledChanged;
                viewModel.IsPauseEnabledChanged += Player_IsPauseEnabledChanged;
                viewModel.IsSkipNextEnabledChanged += Player_IsSkipNextEnabledChanged;
                viewModel.IsSkipPreviousEnabledChanged += Player_IsSkipPreviousEnabledChanged;
            }
        }

        void UnwireEvents(IInteractiveViewModel viewModel)
        {
            if (viewModel != null)
            {
                viewModel.CurrentStateChanged -= Player_CurrentStateChanged;
                viewModel.IsStopEnabledChanged -= Player_IsStopEnabledChanged;
                viewModel.IsRewindEnabledChanged -= Player_IsRewindEnabledChanged;
                viewModel.IsFastForwardEnabledChanged -= Player_IsFastForwardEnabledChanged;
                viewModel.IsPlayResumeEnabledChanged -= Player_IsPlayResumeEnabledChanged;
                viewModel.IsPauseEnabledChanged -= Player_IsPauseEnabledChanged;
                viewModel.IsSkipNextEnabledChanged -= Player_IsSkipNextEnabledChanged;
                viewModel.IsSkipPreviousEnabledChanged -= Player_IsSkipPreviousEnabledChanged;
            }
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
                    SystemControls.IsPreviousEnabled = isPreviousTrackEnabled;
                }
            }
        }

        bool isNextTrackEnabled;
        bool IsNextTrackEnabled
        {
            get { return isNextTrackEnabled; }
            set
            {
                if (isNextTrackEnabled != value)
                {
                    isNextTrackEnabled = value;
                    SystemControls.IsNextEnabled = isNextTrackEnabled;
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
            RefreshNextState();
            RefreshPreviousState();
        }

        void Playlist_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RefreshNextState();
            RefreshPreviousState();
        }

        PlaylistPlugin PlaylistPlugin
        {
            get
            {
                return MediaPlayer.Plugins.OfType<PlaylistPlugin>().FirstOrDefault();
            }
        }

        void Player_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            RefreshPlaybackStatus();
        }

        private void RefreshPlaybackStatus()
        {
            switch (MediaPlayer.InteractiveViewModel.CurrentState)
            {
                case Windows.UI.Xaml.Media.MediaElementState.Paused:
                    SystemControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                case Windows.UI.Xaml.Media.MediaElementState.Playing:
                    SystemControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                    break;
                case Windows.UI.Xaml.Media.MediaElementState.Closed:
                    SystemControls.PlaybackStatus = MediaPlaybackStatus.Closed;
                    break;
                case Windows.UI.Xaml.Media.MediaElementState.Stopped:
                    SystemControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    break;
                case Windows.UI.Xaml.Media.MediaElementState.Opening:
                    SystemControls.PlaybackStatus = MediaPlaybackStatus.Changing;
                    break;
            }
        }

        void Player_IsSkipPreviousEnabledChanged(object sender, RoutedEventArgs e)
        {
            RefreshPreviousState();
        }

        private void RefreshPreviousState()
        {
            IsPreviousTrackEnabled = MediaPlayer.InteractiveViewModel != null && MediaPlayer.InteractiveViewModel.IsSkipPreviousEnabled && PreviousTrackExists;
        }

        void Player_IsSkipNextEnabledChanged(object sender, RoutedEventArgs e)
        {
            RefreshNextState();
        }

        private void RefreshNextState()
        {
            IsNextTrackEnabled = MediaPlayer.InteractiveViewModel != null && MediaPlayer.InteractiveViewModel.IsSkipNextEnabled && NextTrackExists;
        }

        void Player_IsPauseEnabledChanged(object sender, RoutedEventArgs e)
        {
            RefreshPauseState();
        }

        private void RefreshPauseState()
        {
            SystemControls.IsPauseEnabled = MediaPlayer.InteractiveViewModel != null;
        }

        void Player_IsPlayResumeEnabledChanged(object sender, RoutedEventArgs e)
        {
            RefreshPlayState();
        }

        private void RefreshPlayState()
        {
            SystemControls.IsPlayEnabled = MediaPlayer.InteractiveViewModel != null;
        }

        void Player_IsFastForwardEnabledChanged(object sender, RoutedEventArgs e)
        {
            RefreshFastForwardState();
        }

        private void RefreshFastForwardState()
        {
            SystemControls.IsFastForwardEnabled = MediaPlayer.InteractiveViewModel != null && MediaPlayer.InteractiveViewModel.IsFastForwardEnabled;
        }

        void Player_IsRewindEnabledChanged(object sender, RoutedEventArgs e)
        {
            RefreshRewindState();
        }

        private void RefreshRewindState()
        {
            SystemControls.IsRewindEnabled = MediaPlayer.InteractiveViewModel != null && MediaPlayer.InteractiveViewModel.IsRewindEnabled;
        }

        void Player_IsStopEnabledChanged(object sender, RoutedEventArgs e)
        {
            RefreshStopState();
        }

        private void RefreshStopState()
        {
            SystemControls.IsStopEnabled = MediaPlayer.InteractiveViewModel != null && MediaPlayer.InteractiveViewModel.IsStopEnabled;
        }

        void MediaPlayer_InteractiveViewModelChanged(object sender, RoutedPropertyChangedEventArgs<IInteractiveViewModel> e)
        {
            if (e.OldValue != null)
            {
                UnwireEvents(e.OldValue);
            }

            RefreshFastForwardState();
            RefreshRewindState();
            RefreshStopState();
            RefreshPlayState();
            RefreshPauseState();
            RefreshNextState();
            RefreshPreviousState();
            RefreshPlaybackStatus();

            if (e.NewValue != null)
            {
                WireEvents(e.NewValue);
            }
        }

        async void SystemControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Pause:
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
                    break;
                case SystemMediaTransportControlsButton.Play:
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
                    break;
                case SystemMediaTransportControlsButton.Stop:
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
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        if (base.IsActive)
                        {
                            if (isPreviousTrackEnabled && PlaylistPlugin != null)
                            {
                                PlaylistPlugin.GoToPreviousPlaylistItem();
                            }
                        }
                    });
                    break;
                case SystemMediaTransportControlsButton.Next:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        if (base.IsActive)
                        {
                            if (IsNextTrackEnabled && PlaylistPlugin != null)
                            {
                                PlaylistPlugin.GoToNextPlaylistItem();
                            }
                        }
                    });
                    break;
                case SystemMediaTransportControlsButton.Rewind:
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
                    break;
                case SystemMediaTransportControlsButton.FastForward:
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
                    break;
            }
        }
    }
}
