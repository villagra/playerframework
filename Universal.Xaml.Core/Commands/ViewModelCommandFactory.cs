using System;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Represents a static class used to create commands that can be bound to media player buttons
    /// </summary>
    public static class ViewModelCommandFactory
    {
        /// <summary>
        /// Creates a command used to bind to a pause button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreatePauseCommand()
        {
            return new ViewModelCommand(
                vm => vm.Pause(),
                vm => vm.IsPauseEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsPauseEnabledChanged -= eh, (vm, eh) => vm.IsPauseEnabledChanged += eh)
                );
        }

        /// <summary>
        /// Creates a command used to bind to a play button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreatePlayResumeCommand()
        {
            return new ViewModelCommand(
                vm => vm.PlayResume(),
                vm => vm.IsPlayResumeEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsPlayResumeEnabledChanged -= eh, (vm, eh) => vm.IsPlayResumeEnabledChanged += eh)
                );
        }

        /// <summary>
        /// Creates a command used to bind to a stop button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateStopCommand()
        {
            return new ViewModelCommand(
                vm => vm.Stop(),
                vm => vm.IsStopEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsStopEnabledChanged -= eh, (vm, eh) => vm.IsStopEnabledChanged += eh)
                );
        }

        /// <summary>
        /// Creates a command used to bind to a replay button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateReplayCommand()
        {
            return new ViewModelCommand(
                vm => vm.Replay(),
                vm => vm.IsReplayEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsReplayEnabledChanged -= eh, (vm, eh) => vm.IsReplayEnabledChanged += eh)
                );
        }

        /// <summary>
        /// Creates a command used to bind to a rewind button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateRewindCommand()
        {
            return new ViewModelCommand(
                vm => vm.DecreasePlaybackRate(),
                vm => vm.IsRewindEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsRewindEnabledChanged -= eh, (vm, eh) => vm.IsRewindEnabledChanged += eh)
                );
        }

        /// <summary>
        /// Creates a command used to bind to a fast forward button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateFastForwardCommand()
        {
            return new ViewModelCommand(
                vm => vm.IncreasePlaybackRate(),
                vm => vm.IsFastForwardEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsFastForwardEnabledChanged -= eh, (vm, eh) => vm.IsFastForwardEnabledChanged += eh)
                );
        }

        /// <summary>
        /// Creates a command used to bind to a slow motion button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateSlowMotionCommand()
        {
            return new ViewModelCommand(
                vm => vm.IsSlowMotion = !vm.IsSlowMotion,
                vm => vm.IsSlowMotionEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsSlowMotionEnabledChanged -= eh, (vm, eh) => vm.IsSlowMotionEnabledChanged += eh)
                );
        }

        /// <summary>
        /// Creates a command used to bind to a volume slider. Note: the volume value is expected to be passed in as a CommandParameter.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateVolumeCommand()
        {
            return new ViewModelCommand<double>(
                (vm, volume) => vm.Volume = volume,
                (vm, volume) => true
                );
        }

        /// <summary>
        /// Creates a command used to bind to a mute button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateMuteCommand()
        {
            return new ViewModelCommand(
                vm => vm.IsMuted = !vm.IsMuted,
                vm => true
                );
        }

        /// <summary>
        /// Creates a command used to bind to a caption selection button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateCaptionsCommand()
        {
            return new ViewModelCommand(
                vm => vm.InvokeCaptionSelection(),
                vm => vm.IsCaptionSelectionEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsCaptionSelectionEnabledChanged -= eh, (vm, eh) => vm.IsCaptionSelectionEnabledChanged += eh)
                );
        }

        /// <summary>
        /// Creates a command used to bind to a audio stream selection button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateAudioSelectionCommand()
        {
            return new ViewModelCommand(
                vm => vm.InvokeAudioSelection(),
                vm => vm.IsAudioSelectionEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsAudioSelectionEnabledChanged -= eh, (vm, eh) => vm.IsAudioSelectionEnabledChanged += eh)
                );
        }

        /// <summary>
        /// Creates a command used to bind to a fullscreen button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateFullScreenCommand()
        {
            return new ViewModelCommand(
                vm => vm.IsFullScreen = !vm.IsFullScreen,
                vm => vm.IsFullScreenEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsFullScreenEnabledChanged -= eh, (vm, eh) => vm.IsFullScreenEnabledChanged += eh)
                );
        }

#if !WINDOWS80
        /// <summary>
        /// Creates a command used to bind to a zoom button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateZoomCommand()
        {
            return new ViewModelCommand(
                vm => vm.Zoom = !vm.Zoom,
                vm => vm.IsZoomEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsZoomEnabledChanged -= eh, (vm, eh) => vm.IsZoomEnabledChanged += eh)
                );
        }
#endif

#if WINDOWS_UWP
        /// <summary>
        /// Creates a command used to bind to a zoom button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateCastCommand()
        {
            return new ViewModelCommand(
                vm => vm.InvokeCast(),
                vm => vm.IsCastingEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsCastingEnabledChanged -= eh, (vm, eh) => vm.IsCastingEnabledChanged += eh)
                );
        }
#endif

        /// <summary>
        /// Creates a command used to bind to a seek button. Note: the new position is expected to be passed in as a CommandParameter.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateSeekCommand()
        {
            return new ViewModelCommand<TimeSpan>(
                (vm, position) => { bool canceled; vm.Seek(position, out canceled); },
                (vm, position) => vm.IsSeekEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsSeekEnabledChanged -= eh, (vm, eh) => vm.IsSeekEnabledChanged += eh)
                );
        }

        /// <summary>
        /// Creates a command used to bind to a skip previous marker/playlist item button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateSkipPreviousCommand()
        {
            return new ViewModelCommand<VisualMarker>(
                (vm, position) => vm.SkipPrevious(),
                (vm, position) => vm.IsSkipPreviousEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsSkipPreviousEnabledChanged -= eh, (vm, eh) => vm.IsSkipPreviousEnabledChanged += eh)
                );
        }

        /// <summary>
        /// Creates a command used to bind to a skip next marker/playlist item button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateSkipNextCommand()
        {
            return new ViewModelCommand<VisualMarker>(
                (vm, position) => vm.SkipNext(),
                (vm, position) => vm.IsSkipNextEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsSkipNextEnabledChanged -= eh, (vm, eh) => vm.IsSkipNextEnabledChanged += eh)
                );
        }

        /// <summary>
        /// Creates a command used to bind to a skip back button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateSkipBackCommand()
        {
            return new ViewModelCommand(
                vm => vm.SkipBack(),
                vm => vm.IsSkipBackEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsSkipBackEnabledChanged -= eh, (vm, eh) => vm.IsSkipBackEnabledChanged += eh)
                );
        }

        /// <summary>
        /// Creates a command used to bind to a skip ahead button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateSkipAheadCommand()
        {
            return new ViewModelCommand(
                vm => vm.SkipAhead(),
                vm => vm.IsSkipAheadEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsSkipAheadEnabledChanged -= eh, (vm, eh) => vm.IsSkipAheadEnabledChanged += eh)
                );
        }

        /// <summary>
        /// Creates a command used to bind to a go live button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateGoLiveCommand()
        {
            return new ViewModelCommand(
                vm => vm.GoLive(),
                vm => vm.IsGoLiveEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsGoLiveEnabledChanged -= eh, (vm, eh) => vm.IsGoLiveEnabledChanged += eh)
                );
        }

        /// <summary>
        /// Creates a command used to bind to an info button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateInfoCommand()
        {
            return new ViewModelCommand(
                vm => vm.InvokeInfo(),
                vm => vm.IsInfoEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsInfoEnabledChanged -= eh, (vm, eh) => vm.IsInfoEnabledChanged += eh)
                );
        }

        /// <summary>
        /// Creates a command used to bind to an more button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreateMoreCommand()
        {
            return new ViewModelCommand(
                vm => vm.InvokeMore(),
                vm => vm.IsMoreEnabled,
                new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsMoreEnabledChanged -= eh, (vm, eh) => vm.IsMoreEnabledChanged += eh)
                );
        }

        /// <summary>
        /// Creates a command used to bind to a play/pause button.
        /// </summary>
        /// <returns>A special ICommand object expected to be wired to a ViewModel.</returns>
        public static ViewModelCommand CreatePlayPauseCommand()
        {
            return new ViewModelCommand(
                vm =>
                {
                    if (vm.IsPlayResumeEnabled)
                    {
                        vm.PlayResume();
                    }
                    else
                    {
                        vm.Pause();
                    }
                },
            vm => vm.IsPauseEnabled || vm.IsPlayResumeEnabled,
            new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsPlayResumeEnabledChanged -= eh, (vm, eh) => vm.IsPlayResumeEnabledChanged += eh),
            new HandlerReference<IInteractiveViewModel, RoutedEventHandler>((vm, eh) => vm.IsPauseEnabledChanged -= eh, (vm, eh) => vm.IsPauseEnabledChanged += eh)
            );
        }
    }
}
