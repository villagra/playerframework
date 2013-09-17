using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Data;
#else
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Represents a play/pause toggle button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class PlayPauseButton : MediaPlayerButton
    {
        /// <summary>
        /// Creates a new instance of PlayPauseButton.
        /// </summary>
        public PlayPauseButton()
        {
            this.DefaultStyleKey = typeof(PlayPauseButton);

            Command = ViewModelCommandFactory.CreatePlayPauseCommand();

            SelectedName = MediaPlayer.GetResourceString("PlayButtonLabel");
            UnselectedName = MediaPlayer.GetResourceString("PauseButtonLabel");
        }

        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            this.SetBinding(MediaPlayerButton.IsSelectedProperty, new Binding() { Path = new PropertyPath("IsPlayResumeEnabled"), Source = ViewModel });
        }
    }

    /// <summary>
    /// Represents a fullscreen toggle button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class FullScreenButton : MediaPlayerButton
    {
        /// <summary>
        /// Creates a new instance of FullScreenButton.
        /// </summary>
        public FullScreenButton()
        {
            this.DefaultStyleKey = typeof(FullScreenButton);

            Command = ViewModelCommandFactory.CreateFullScreenCommand();

            SelectedName = MediaPlayer.GetResourceString("ExitFullScreenButtonLabel");
            UnselectedName = MediaPlayer.GetResourceString("FullScreenButtonLabel");
        }

        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            this.SetBinding(MediaPlayerButton.IsSelectedProperty, new Binding() { Path = new PropertyPath("IsFullScreen"), Source = ViewModel });
        }
    }

    /// <summary>
    /// Represents a mute toggle button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class MuteButton : MediaPlayerButton
    {
        /// <summary>
        /// Creates a new instance of MuteButton.
        /// </summary>
        public MuteButton()
        {
            this.DefaultStyleKey = typeof(MuteButton);

            Command = ViewModelCommandFactory.CreateMuteCommand();

            SelectedName = MediaPlayer.GetResourceString("UnmuteButtonLabel");
            UnselectedName = MediaPlayer.GetResourceString("MuteButtonLabel");
        }

        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            this.SetBinding(MediaPlayerButton.IsSelectedProperty, new Binding() { Path = new PropertyPath("IsMuted"), Source = ViewModel });
        }
    }

    /// <summary>
    /// Represents a slow motion toggle button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class SlowMotionButton : MediaPlayerToggleButton
    {
        /// <summary>
        /// Creates a new instance of SlowMotionButton.
        /// </summary>
        public SlowMotionButton()
        {
            this.DefaultStyleKey = typeof(SlowMotionButton);

            Command = ViewModelCommandFactory.CreateSlowMotionCommand();

            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("SlowMotionButtonLabel"));
        }

        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            this.SetBinding(ToggleButton.IsCheckedProperty, new Binding() { Path = new PropertyPath("IsSlowMotion"), Source = ViewModel });
        }
    }

    /// <summary>
    /// Represents a play button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class PlayButton : MediaPlayerButton
    {
        /// <summary>
        /// Creates a new instance of PlayButton.
        /// </summary>
        public PlayButton()
        {
            this.DefaultStyleKey = typeof(PlayButton);

            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("PlayButtonLabel"));
            Command = ViewModelCommandFactory.CreatePlayResumeCommand();
        }
    }

    /// <summary>
    /// Represents a pause button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class PauseButton : MediaPlayerButton
    {
        /// <summary>
        /// Creates a new instance of PauseButton.
        /// </summary>
        public PauseButton()
        {
            this.DefaultStyleKey = typeof(PauseButton);

            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("PauseButtonLabel"));
            Command = ViewModelCommandFactory.CreatePauseCommand();
        }
    }

    /// <summary>
    /// Represents a caption selection button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class CaptionSelectionButton : MediaPlayerButton
    {
        /// <summary>
        /// Creates a new instance of CaptionSelectionButton.
        /// </summary>
        public CaptionSelectionButton()
        {
            this.DefaultStyleKey = typeof(CaptionSelectionButton);

            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("CaptionSelectionButtonLabel"));
            Command = ViewModelCommandFactory.CreateCaptionsCommand();
        }
    }

    /// <summary>
    /// Represents a go live button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class GoLiveButton : MediaPlayerButton
    {
        /// <summary>
        /// Creates a new instance of GoLiveButton.
        /// </summary>
        public GoLiveButton()
        {
            this.DefaultStyleKey = typeof(GoLiveButton);

            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("GoLiveButtonLabel"));
            Command = ViewModelCommandFactory.CreateGoLiveCommand();
        }
    }

    /// <summary>
    /// Represents an audio stream selection button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class AudioSelectionButton : MediaPlayerButton
    {
        /// <summary>
        /// Creates a new instance of AudioSelectionButton.
        /// </summary>
        public AudioSelectionButton()
        {
            this.DefaultStyleKey = typeof(AudioSelectionButton);

            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("AudioSelectionButtonLabel"));
            Command = ViewModelCommandFactory.CreateAudioSelectionCommand();
        }
    }

    /// <summary>
    /// Represents a skip back button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class SkipBackButton : MediaPlayerButton
    {
        /// <summary>
        /// Creates a new instance of SkipBackButton.
        /// </summary>
        public SkipBackButton()
        {
            this.DefaultStyleKey = typeof(SkipBackButton);
            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("SkipBackButtonLabel"));

            Command = ViewModelCommandFactory.CreateSkipBackCommand();
        }
    }

    /// <summary>
    /// Represents a skip ahead button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class SkipAheadButton : MediaPlayerButton
    {
        /// <summary>
        /// Creates a new instance of SkipAheadButton.
        /// </summary>
        public SkipAheadButton()
        {
            this.DefaultStyleKey = typeof(SkipAheadButton);

            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("SkipAheadButtonLabel"));

            Command = ViewModelCommandFactory.CreateSkipAheadCommand();
        }
    }

    /// <summary>
    /// Represents a time elapsed + skip back button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class TimeElapsedButton : SkipBackButton
    {
        string skipBackPointerOverStringFormat;

        /// <summary>
        /// Creates a new instance of TimeElapsedButton.
        /// </summary>
        public TimeElapsedButton()
        {
            this.DefaultStyleKey = typeof(TimeElapsedButton);

            skipBackPointerOverStringFormat = MediaPlayer.GetResourceString("SkipBackPointerOverStringFormat");

            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("TimeElapsedButtonLabel"));
        }

        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            this.SetBinding(MediaPlayerButton.ContentHoverProperty, new Binding() { Path = new PropertyPath("SkipBackInterval"), Source = ViewModel, Converter = new StringFormatConverter() { StringFormat = skipBackPointerOverStringFormat } });
            this.SetBinding(MediaPlayerButton.ContentUnhoverProperty, new Binding() { Path = new PropertyPath("Position"), Source = ViewModel, Converter = ViewModel != null ? ViewModel.TimeFormatConverter : null });
        }
    }

    /// <summary>
    /// Represents a total duration + skip ahead button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class DurationButton : SkipAheadButton
    {
        string skipAheadPointerOverStringFormat;

        /// <summary>
        /// Creates a new instance of DurationButton.
        /// </summary>
        public DurationButton()
        {
            this.DefaultStyleKey = typeof(DurationButton);

            skipAheadPointerOverStringFormat = MediaPlayer.GetResourceString("SkipAheadPointerOverStringFormat");

            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("DurationButtonLabel"));
        }

        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            this.SetBinding(MediaPlayerButton.ContentHoverProperty, new Binding() { Path = new PropertyPath("SkipAheadInterval"), Source = ViewModel, Converter = new StringFormatConverter() { StringFormat = skipAheadPointerOverStringFormat } });
            this.SetBinding(MediaPlayerButton.ContentUnhoverProperty, new Binding() { Path = new PropertyPath("Duration"), Source = ViewModel, Converter = ViewModel != null ? ViewModel.TimeFormatConverter : null });
        }
    }

    /// <summary>
    /// Represents a time remaining + skip ahead button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class TimeRemainingButton : SkipAheadButton
    {
        string skipAheadPointerOverStringFormat;

        /// <summary>
        /// Creates a new instance of TimeRemainingButton.
        /// </summary>
        public TimeRemainingButton()
        {
            this.DefaultStyleKey = typeof(TimeRemainingButton);

            skipAheadPointerOverStringFormat = MediaPlayer.GetResourceString("SkipAheadPointerOverStringFormat");

            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("TimeRemainingButtonLabel"));
        }

        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            this.SetBinding(MediaPlayerButton.ContentHoverProperty, new Binding() { Path = new PropertyPath("SkipAheadInterval"), Source = ViewModel, Converter = new StringFormatConverter() { StringFormat = skipAheadPointerOverStringFormat } });
            this.SetBinding(MediaPlayerButton.ContentUnhoverProperty, new Binding() { Path = new PropertyPath("TimeRemaining"), Source = ViewModel, Converter = ViewModel != null ? ViewModel.TimeFormatConverter : null });
        }
    }

    /// <summary>
    /// Represents a total duration textblock that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class TotalDuration : MediaPlayerControl
    {
        /// <summary>
        /// Creates a new instance of TotalDuration.
        /// </summary>
        public TotalDuration()
        {
            this.DefaultStyleKey = typeof(TotalDuration);
        }

        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            this.SetBinding(ContentControl.ContentProperty, new Binding() { Path = new PropertyPath("Duration"), Source = ViewModel, Converter = ViewModel != null ? ViewModel.TimeFormatConverter : null });
        }
    }

    /// <summary>
    /// Represents a time elapsed textblock that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class TimeElapsed : MediaPlayerControl
    {
        /// <summary>
        /// Creates a new instance of TimeElapsed.
        /// </summary>
        public TimeElapsed()
        {
            this.DefaultStyleKey = typeof(TimeElapsed);
        }

        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            this.SetBinding(ContentControl.ContentProperty, new Binding() { Path = new PropertyPath("Position"), Source = ViewModel, Converter = ViewModel != null ? ViewModel.TimeFormatConverter : null });
        }
    }

    /// <summary>
    /// Represents a time remaining textblock that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class TimeRemaining : MediaPlayerControl
    {
        /// <summary>
        /// Creates a new instance of TimeRemaining.
        /// </summary>
        public TimeRemaining()
        {
            this.DefaultStyleKey = typeof(TimeRemaining);
        }

        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            this.SetBinding(ContentControl.ContentProperty, new Binding() { Path = new PropertyPath("TimeRemaining"), Source = ViewModel, Converter = ViewModel != null ? ViewModel.TimeFormatConverter : null });
        }
    }

    /// <summary>
    /// Represents a skip to previous marker/playlist item button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class SkipPreviousButton : MediaPlayerButton
    {
        /// <summary>
        /// Creates a new instance of SkipPreviousButton.
        /// </summary>
        public SkipPreviousButton()
        {
            this.DefaultStyleKey = typeof(SkipPreviousButton);

            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("SkipPreviousButtonLabel"));
            Command = ViewModelCommandFactory.CreateSkipPreviousCommand();
        }
    }

    /// <summary>
    /// Represents a skip to next marker/playlist item button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class SkipNextButton : MediaPlayerButton
    {
        /// <summary>
        /// Creates a new instance of SkipNextButton.
        /// </summary>
        public SkipNextButton()
        {
            this.DefaultStyleKey = typeof(SkipNextButton);

            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("SkipNextButtonLabel"));
            Command = ViewModelCommandFactory.CreateSkipNextCommand();
        }
    }

    /// <summary>
    /// Represents a stop button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class StopButton : MediaPlayerButton
    {
        /// <summary>
        /// Creates a new instance of StopButton.
        /// </summary>
        public StopButton()
        {
            this.DefaultStyleKey = typeof(StopButton);

            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("StopButtonLabel"));
            Command = ViewModelCommandFactory.CreateStopCommand();
        }
    }

    /// <summary>
    /// Represents a rewind button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class RewindButton : MediaPlayerButton
    {
        /// <summary>
        /// Creates a new instance of RewindButton.
        /// </summary>
        public RewindButton()
        {
            this.DefaultStyleKey = typeof(RewindButton);

            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("RewindButtonLabel"));
            Command = ViewModelCommandFactory.CreateRewindCommand();
        }
    }

    /// <summary>
    /// Represents a fast forward button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class FastForwardButton : MediaPlayerButton
    {
        /// <summary>
        /// Creates a new instance of FastForwardButton.
        /// </summary>
        public FastForwardButton()
        {
            this.DefaultStyleKey = typeof(FastForwardButton);

            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("FastForwardButtonLabel"));
            Command = ViewModelCommandFactory.CreateFastForwardCommand();
        }
    }

    /// <summary>
    /// Represents a replay button that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class ReplayButton : MediaPlayerButton
    {
        /// <summary>
        /// Creates a new instance of ReplayButton.
        /// </summary>
        public ReplayButton()
        {
            this.DefaultStyleKey = typeof(ReplayButton);

            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("ReplayButtonLabel"));
            Command = ViewModelCommandFactory.CreateReplayCommand();
        }
    }
}
