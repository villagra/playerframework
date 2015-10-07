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
using System.Windows.Markup;
#else
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;
using Windows.UI.Xaml.Markup;
using Windows.Foundation;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Represents a time elapsed button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class TimeElapsedButtonBehavior : MediaToggleControlBehavior, IElementAwareMediaBehavior
    {
        string skipBackPointerOverStringFormat;

        /// <summary>
        /// Creates a new instance of TimeElapsedButtonBehavior.
        /// </summary>
        public TimeElapsedButtonBehavior()
        {
            Command = ViewModelCommandFactory.CreateSkipBackCommand();

            skipBackPointerOverStringFormat = MediaPlayer.GetResourceString("SkipBackPointerOverStringFormat");

            Label = MediaPlayer.GetResourceString("TimeElapsedButtonLabel");
        }

        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            BindingOperations.SetBinding(this, MediaToggleControlBehavior.SetContentProperty, new Binding() { Path = new PropertyPath("SkipBackInterval"), Source = newValue, Converter = new StringFormatConverter() { StringFormat = skipBackPointerOverStringFormat } });
            BindingOperations.SetBinding(this, MediaToggleControlBehavior.UnsetContentProperty, new Binding() { Path = new PropertyPath("Position"), Source = newValue, Converter = ViewModel != null ? ViewModel.TimeFormatConverter : null });
        }

        DependencyObject element;

        /// <inheritdoc /> 
        public DependencyObject Element
        {
            get { return element; }
            set
            {
                element = value;
                if (element == null)
                {
                    IsSet = false;
                }
                else
                {
#if !WINDOWS_PHONE
                    BindingOperations.SetBinding(this, MediaToggleControlBehavior.IsSetProperty, new Binding() { Path = new PropertyPath("IsPointerOver"), Source = element });
#endif
                }
            }
        }
    }

    /// <summary>
    /// Represents a duration button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class DurationButtonBehavior : MediaToggleControlBehavior, IElementAwareMediaBehavior
    {
        string skipAheadPointerOverStringFormat;

        /// <summary>
        /// Creates a new instance of DurationButtonBehavior.
        /// </summary>
        public DurationButtonBehavior()
        {
            Command = ViewModelCommandFactory.CreateSkipAheadCommand();

            skipAheadPointerOverStringFormat = MediaPlayer.GetResourceString("SkipAheadPointerOverStringFormat");

            Label = MediaPlayer.GetResourceString("DurationButtonLabel");
        }

        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            BindingOperations.SetBinding(this, MediaToggleControlBehavior.SetContentProperty, new Binding() { Path = new PropertyPath("SkipAheadInterval"), Source = newValue, Converter = new StringFormatConverter() { StringFormat = skipAheadPointerOverStringFormat } });
            BindingOperations.SetBinding(this, MediaToggleControlBehavior.UnsetContentProperty, new Binding() { Path = new PropertyPath("Duration"), Source = newValue, Converter = ViewModel != null ? ViewModel.TimeFormatConverter : null });
        }

        DependencyObject element;

        /// <inheritdoc /> 
        public DependencyObject Element
        {
            get { return element; }
            set
            {
                element = value;
                if (element == null)
                {
                    IsSet = false;
                }
                else
                {
#if !WINDOWS_PHONE
                    BindingOperations.SetBinding(this, MediaToggleControlBehavior.IsSetProperty, new Binding() { Path = new PropertyPath("IsPointerOver"), Source = element });
#endif
                }
            }
        }
    }

    /// <summary>
    /// Represents a time remaining button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class TimeRemainingButtonBehavior : MediaToggleControlBehavior, IElementAwareMediaBehavior
    {
        string skipAheadPointerOverStringFormat;

        /// <summary>
        /// Creates a new instance of TimeRemainingButtonBehavior.
        /// </summary>
        public TimeRemainingButtonBehavior()
        {
            Command = ViewModelCommandFactory.CreateSkipAheadCommand();

            skipAheadPointerOverStringFormat = MediaPlayer.GetResourceString("SkipAheadPointerOverStringFormat");

            Label = MediaPlayer.GetResourceString("TimeRemainingButtonLabel");
        }

        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            BindingOperations.SetBinding(this, MediaToggleControlBehavior.SetContentProperty, new Binding() { Path = new PropertyPath("SkipAheadInterval"), Source = newValue, Converter = new StringFormatConverter() { StringFormat = skipAheadPointerOverStringFormat } });
            BindingOperations.SetBinding(this, MediaToggleControlBehavior.UnsetContentProperty, new Binding() { Path = new PropertyPath("TimeRemaining"), Source = newValue, Converter = ViewModel != null ? ViewModel.TimeFormatConverter : null });
        }

        DependencyObject element;

        /// <inheritdoc /> 
        public DependencyObject Element
        {
            get { return element; }
            set
            {
                element = value;
                if (element == null)
                {
                    IsSet = false;
                }
                else
                {
#if !WINDOWS_PHONE
                    BindingOperations.SetBinding(this, MediaToggleControlBehavior.IsSetProperty, new Binding() { Path = new PropertyPath("IsPointerOver"), Source = element });
#endif
                }
            }
        }
    }

    /// <summary>
    /// Represents a total duration behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class TotalDurationBehavior : MediaControlBehavior
    {
        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            BindingOperations.SetBinding(this, MediaControlBehavior.ContentProperty, new Binding() { Path = new PropertyPath("Duration"), Source = newValue, Converter = ViewModel != null ? ViewModel.TimeFormatConverter : null });
        }
    }

    /// <summary>
    /// Represents a time elapsed behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class TimeElapsedBehavior : MediaControlBehavior
    {
        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            BindingOperations.SetBinding(this, MediaControlBehavior.ContentProperty, new Binding() { Path = new PropertyPath("Position"), Source = newValue, Converter = ViewModel != null ? ViewModel.TimeFormatConverter : null });
        }
    }

    /// <summary>
    /// Represents a time remaining behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class TimeRemainingBehavior : MediaControlBehavior
    {
        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            BindingOperations.SetBinding(this, MediaControlBehavior.ContentProperty, new Binding() { Path = new PropertyPath("TimeRemaining"), Source = newValue, Converter = ViewModel != null ? ViewModel.TimeFormatConverter : null });
        }
    }

    /// <summary>
    /// Represents a slow motion toggle button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class SlowMotionButtonBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Creates a new instance of SlowMotionButtonBehavior.
        /// </summary>
        public SlowMotionButtonBehavior()
        {
            Command = ViewModelCommandFactory.CreateSlowMotionCommand();
            Content = XamlReader.Load(MediaPlayer.GetResourceString("SlowMotionButtonContent"));
            Label = MediaPlayer.GetResourceString("SlowMotionButtonLabel");
        }
    }

    /// <summary>
    /// Represents a pause button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class PauseButtonBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Creates a new instance of PauseButtonBehavior.
        /// </summary>
        public PauseButtonBehavior()
        {
            Command = ViewModelCommandFactory.CreatePauseCommand();
            Label = MediaPlayer.GetResourceString("PauseButtonLabel");
            Content = XamlReader.Load(MediaPlayer.GetResourceString("PauseButtonContent"));
        }
    }

    /// <summary>
    /// Represents a pause button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class PlayButtonBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Creates a new instance of PlayButtonBehavior.
        /// </summary>
        public PlayButtonBehavior()
        {
            Command = ViewModelCommandFactory.CreatePlayResumeCommand();
            Label = MediaPlayer.GetResourceString("PlayButtonLabel");
            Content = XamlReader.Load(MediaPlayer.GetResourceString("PlayButtonContent"));
        }
    }

    /// <summary>
    /// Represents a stop button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class StopButtonBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Creates a new instance of StopButtonBehavior.
        /// </summary>
        public StopButtonBehavior()
        {
            Command = ViewModelCommandFactory.CreateStopCommand();
            Label = MediaPlayer.GetResourceString("StopButtonLabel");
            Content = XamlReader.Load(MediaPlayer.GetResourceString("StopButtonContent"));
        }
    }

    /// <summary>
    /// Represents a rewind button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class RewindButtonBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Creates a new instance of RewindButtonBehavior.
        /// </summary>
        public RewindButtonBehavior()
        {
            Command = ViewModelCommandFactory.CreateRewindCommand();
            Label = MediaPlayer.GetResourceString("RewindButtonLabel");
            Content = XamlReader.Load(MediaPlayer.GetResourceString("RewindButtonContent"));
        }
    }

    /// <summary>
    /// Represents a fast forward button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class FastForwardButtonBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Creates a new instance of FastForwardButtonBehavior.
        /// </summary>
        public FastForwardButtonBehavior()
        {
            Command = ViewModelCommandFactory.CreateFastForwardCommand();
            Label = MediaPlayer.GetResourceString("FastForwardButtonLabel");
            Content = XamlReader.Load(MediaPlayer.GetResourceString("FastForwardButtonContent"));
        }
    }

    /// <summary>
    /// Represents a replay button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class ReplayButtonBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Creates a new instance of ReplayButtonBehavior.
        /// </summary>
        public ReplayButtonBehavior()
        {
            Command = ViewModelCommandFactory.CreateReplayCommand();
            Label = MediaPlayer.GetResourceString("ReplayButtonLabel");
            Content = XamlReader.Load(MediaPlayer.GetResourceString("ReplayButtonContent"));
        }
    }

    /// <summary>
    /// Represents a mute button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class MuteButtonBehavior : MediaToggleControlBehavior
    {
        /// <summary>
        /// Creates a new instance of MuteButtonBehavior.
        /// </summary>
        public MuteButtonBehavior()
        {
            Command = ViewModelCommandFactory.CreateMuteCommand();
            SetLabel = MediaPlayer.GetResourceString("UnmuteButtonLabel");
            UnsetLabel = MediaPlayer.GetResourceString("MuteButtonLabel");
            SetContent = XamlReader.Load(MediaPlayer.GetResourceString("MuteButtonContent"));
            UnsetContent = XamlReader.Load(MediaPlayer.GetResourceString("UnmuteButtonContent"));
        }

        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            BindingOperations.SetBinding(this, MediaToggleControlBehavior.IsSetProperty, new Binding() { Path = new PropertyPath("IsMuted"), Source = newValue });
        }
    }

    /// <summary>
    /// Represents a play/pause toggle button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class PlayPauseButtonBehavior : MediaToggleControlBehavior
    {
        /// <summary>
        /// Creates a new instance of PlayPauseButtonBehavior.
        /// </summary>
        public PlayPauseButtonBehavior()
        {
            Command = ViewModelCommandFactory.CreatePlayPauseCommand();
            SetLabel = MediaPlayer.GetResourceString("PlayButtonLabel");
            UnsetLabel = MediaPlayer.GetResourceString("PauseButtonLabel");
            SetContent = MediaPlayer.GetResourceString("PlayButtonContent");
            UnsetContent = MediaPlayer.GetResourceString("PauseButtonContent");
            ContentConverter = new XamlConverter(); // instead of creating Xaml here, we will use a converter to convert the string each time it is re-assigned. This fixes a problem with old visual state changes being applied to the content when it switches.
        }

        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            BindingOperations.SetBinding(this, MediaToggleControlBehavior.IsSetProperty, new Binding() { Path = new PropertyPath("IsPlayResumeEnabled"), Source = newValue });
        }
    }

    /// <summary>
    /// Represents a fullscreen toggle button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class FullScreenButtonBehavior : MediaToggleControlBehavior
    {
        /// <summary>
        /// Creates a new instance of FullScreenButtonBehavior.
        /// </summary>
        public FullScreenButtonBehavior()
        {
            Command = ViewModelCommandFactory.CreateFullScreenCommand();
            SetLabel = MediaPlayer.GetResourceString("ExitFullScreenButtonLabel");
            UnsetLabel = MediaPlayer.GetResourceString("FullScreenButtonLabel");
            SetContent = XamlReader.Load(MediaPlayer.GetResourceString("ExitFullScreenButtonContent"));
            UnsetContent = XamlReader.Load(MediaPlayer.GetResourceString("FullScreenButtonContent"));
        }

        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            BindingOperations.SetBinding(this, MediaToggleControlBehavior.IsSetProperty, new Binding() { Path = new PropertyPath("IsFullScreen"), Source = newValue });
        }
    }

#if !WINDOWS80
    /// <summary>
    /// Represents a zoom toggle button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class ZoomButtonBehavior : MediaToggleControlBehavior
    {
        /// <summary>
        /// Creates a new instance of ZoomButtonBehavior.
        /// </summary>
        public ZoomButtonBehavior()
        {
            Command = ViewModelCommandFactory.CreateZoomCommand();
            SetLabel = MediaPlayer.GetResourceString("ZoomOutScreenButtonLabel");
            UnsetLabel = MediaPlayer.GetResourceString("ZoomInScreenButtonLabel");
            SetContent = XamlReader.Load(MediaPlayer.GetResourceString("ZoomOutScreenButtonContent"));
            UnsetContent = XamlReader.Load(MediaPlayer.GetResourceString("ZoomInScreenButtonContent"));
        }

        /// <inheritdoc /> 
        protected override void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            base.OnViewModelChanged(oldValue, newValue);

            BindingOperations.SetBinding(this, MediaToggleControlBehavior.IsSetProperty, new Binding() { Path = new PropertyPath("Zoom"), Source = newValue });
        }
    }
#endif

#if WINDOWS_UWP
    /// <summary>
    /// Represents a cast button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class CastButtonBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Creates a new instance of CastButtonBehavior.
        /// </summary>
        public CastButtonBehavior()
        {
            Label = MediaPlayer.GetResourceString("CastButtonLabel");
            Command = ViewModelCommandFactory.CreateCastCommand();
            Content = XamlReader.Load(MediaPlayer.GetResourceString("CastButtonContent"));
        }
    }
#endif

    /// <summary>
    /// Represents a caption selection button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class CaptionSelectionButtonBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Creates a new instance of CaptionSelectionButtonBehavior.
        /// </summary>
        public CaptionSelectionButtonBehavior()
        {
            Label = MediaPlayer.GetResourceString("CaptionSelectionButtonLabel");
            Command = ViewModelCommandFactory.CreateCaptionsCommand();
            Content = XamlReader.Load(MediaPlayer.GetResourceString("CaptionSelectionButtonContent"));
        }
    }

    /// <summary>
    /// Represents a go live button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class GoLiveButtonBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Creates a new instance of GoLiveButtonBehavior.
        /// </summary>
        public GoLiveButtonBehavior()
        {
            Label = MediaPlayer.GetResourceString("GoLiveButtonLabel");
            Command = ViewModelCommandFactory.CreateGoLiveCommand();
            Content = XamlReader.Load(MediaPlayer.GetResourceString("GoLiveButtonContent"));
        }
    }

    /// <summary>
    /// Represents an info button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class InfoButtonBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Creates a new instance of InfoButtonBehavior.
        /// </summary>
        public InfoButtonBehavior()
        {
            Label = MediaPlayer.GetResourceString("InfoButtonLabel");
            Command = ViewModelCommandFactory.CreateInfoCommand();
            Content = XamlReader.Load(MediaPlayer.GetResourceString("InfoButtonContent"));
        }
    }

    /// <summary>
    /// Represents an More button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class MoreButtonBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Creates a new instance of MoreButtonBehavior.
        /// </summary>
        public MoreButtonBehavior()
        {
            Label = MediaPlayer.GetResourceString("MoreButtonLabel");
            Command = ViewModelCommandFactory.CreateMoreCommand();
            Content = XamlReader.Load(MediaPlayer.GetResourceString("MoreButtonContent"));
        }
    }

    /// <summary>
    /// Represents an audio stream selection button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class AudioSelectionButtonBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Creates a new instance of AudioSelectionButtonBehavior.
        /// </summary>
        public AudioSelectionButtonBehavior()
        {
            Label = MediaPlayer.GetResourceString("AudioSelectionButtonLabel");
            Command = ViewModelCommandFactory.CreateAudioSelectionCommand();
            Content = XamlReader.Load(MediaPlayer.GetResourceString("AudioSelectionButtonContent"));
        }
    }

    /// <summary>
    /// Represents a skip previous button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class SkipPreviousButtonBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Creates a new instance of SkipBackButtonBehavior.
        /// </summary>
        public SkipPreviousButtonBehavior()
        {
            Label = MediaPlayer.GetResourceString("SkipPreviousButtonLabel");
            Command = ViewModelCommandFactory.CreateSkipPreviousCommand();
            Content = XamlReader.Load(MediaPlayer.GetResourceString("SkipPreviousButtonContent"));
        }
    }

    /// <summary>
    /// Represents a skip next button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class SkipNextButtonBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Creates a new instance of SkipNextButtonBehavior.
        /// </summary>
        public SkipNextButtonBehavior()
        {
            Label = MediaPlayer.GetResourceString("SkipNextButtonLabel");
            Command = ViewModelCommandFactory.CreateSkipNextCommand();
            Content = XamlReader.Load(MediaPlayer.GetResourceString("SkipNextButtonContent"));
        }
    }

    /// <summary>
    /// Represents a skip back button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class SkipBackButtonBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Creates a new instance of SkipBackButtonBehavior.
        /// </summary>
        public SkipBackButtonBehavior()
        {
            Label = MediaPlayer.GetResourceString("SkipBackButtonLabel");
            Command = ViewModelCommandFactory.CreateSkipBackCommand();
            Content = XamlReader.Load(MediaPlayer.GetResourceString("SkipBackButtonContent"));
        }
    }

    /// <summary>
    /// Represents a skip ahead button behavior that can be bound to MediaPlayer.InteractiveViewModel
    /// </summary>
    public class SkipAheadButtonBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Creates a new instance of SkipAheadButtonBehavior.
        /// </summary>
        public SkipAheadButtonBehavior()
        {
            Label = MediaPlayer.GetResourceString("SkipAheadButtonLabel");
            Command = ViewModelCommandFactory.CreateSkipAheadCommand();
            Content = XamlReader.Load(MediaPlayer.GetResourceString("SkipAheadButtonContent"));
        }
    }
}
