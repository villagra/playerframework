using System;
using System.ComponentModel;
using System.Windows.Input;
#if SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Automation;
using System.Windows.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Data;
#endif

namespace Microsoft.PlayerFramework
{
    public static class MediaControlBehaviors
    {
        public static T CreateMediaControlBehavior<T>(this MediaPlayer source) where T : MediaControlBehaviorBase, new()
        {
            var behavior = new T();
            BindingOperations.SetBinding(behavior, MediaControlBehaviorBase.ViewModelProperty, new Binding() { Path = new PropertyPath("InteractiveViewModel"), Source = source });
            return behavior;
        }
    }

    public static class MediaControls
    {
        /// <summary>
        /// Identifies the Behavior dependency property.
        /// </summary>
        public static readonly DependencyProperty BehaviorProperty = DependencyProperty.RegisterAttached("Behavior", typeof(IMediaControlBehavior), typeof(MediaControls), new PropertyMetadata(null, (d, e) => MediaControls.OnBehaviorChanged(d, e.OldValue as IMediaControlBehavior, e.NewValue as IMediaControlBehavior)));

        static void OnBehaviorChanged(DependencyObject obj, IMediaControlBehavior oldValue, IMediaControlBehavior newValue)
        {
            if (oldValue != null)
            {
                var contentControl = obj as ContentControl;
                if (contentControl != null)
                {
                    var buttonBase = obj as ButtonBase;
                    if (buttonBase != null)
                    {
                        buttonBase.Command = null;
                    }
                    
#if !SILVERLIGHT && !WINDOWS80
                    var appbarButton = buttonBase as AppBarButton;
                    if (appbarButton != null)
                    {
                        appbarButton.Label = null;
                        appbarButton.Icon = null;
                    }
                    else
                    {
                        var appbarToggleButton = buttonBase as AppBarToggleButton;
                        if (appbarToggleButton != null)
                        {
                            appbarToggleButton.Label = null;
                            appbarToggleButton.Icon = null;
                        }
                        else
                        {
                            contentControl.Content = null;
                        }
                    }
#else
                    contentControl.Content = null;
#endif
                }
                else
                {
                    var textBlock = obj as TextBlock;
                    if (textBlock != null)
                    {
                        textBlock.Text = null;
                    }
                }
                AutomationProperties.SetName(obj, null);
                ToolTipService.SetToolTip(obj, null);
            }

            if (oldValue is IElementAwareMediaBehavior)
            {
                ((IElementAwareMediaBehavior)oldValue).Element = null;
            }
            if (newValue is IElementAwareMediaBehavior)
            {
                ((IElementAwareMediaBehavior)newValue).Element = obj;
            }

            if (newValue != null)
            {
                var contentControl = obj as ContentControl;
                if (contentControl != null)
                {
                    var buttonBase = obj as ButtonBase;
                    if (buttonBase != null)
                    {
                        buttonBase.Command = newValue.Command ?? buttonBase.Command;
                    }
#if !SILVERLIGHT && !WINDOWS80
                    var appbarButton = buttonBase as AppBarButton;
                    if (appbarButton != null)
                    {
                        appbarButton.SetBinding(AppBarButton.LabelProperty, new Binding() { Path = new PropertyPath("Label"), Source = newValue });
                        appbarButton.SetBinding(AppBarButton.IconProperty, new Binding() { Path = new PropertyPath("Content"), Source = newValue });
                    }
                    else
                    {
                        var appbarToggleButton = buttonBase as AppBarToggleButton;
                        if (appbarToggleButton != null)
                        {
                            appbarToggleButton.SetBinding(AppBarToggleButton.LabelProperty, new Binding() { Path = new PropertyPath("Label"), Source = newValue });
                            appbarToggleButton.SetBinding(AppBarToggleButton.IconProperty, new Binding() { Path = new PropertyPath("Content"), Source = newValue });
                        }
                        else
                        {
                            contentControl.SetBinding(ContentControl.ContentProperty, new Binding() { Path = new PropertyPath("Content"), Source = newValue });
                        }
                    }
#else
                    contentControl.SetBinding(ContentControl.ContentProperty, new Binding() { Path = new PropertyPath("Content"), Source = newValue });
#endif
                }
                else
                {
                    var textBlock = obj as TextBlock;
                    if (textBlock != null)
                    {
                        textBlock.SetBinding(TextBlock.TextProperty, new Binding() { Path = new PropertyPath("Content"), Source = newValue });
                    }
                }

                BindingOperations.SetBinding(obj, AutomationProperties.NameProperty, new Binding() { Path = new PropertyPath("Label"), Source = newValue });
                if (GetIsToolTipEnabled(obj))
                {
                    BindingOperations.SetBinding(obj, ToolTipService.ToolTipProperty, new Binding() { Path = new PropertyPath("Label"), Source = newValue });
                }
            }
        }

        public static IMediaControlBehavior GetBehavior(DependencyObject obj)
        {
            return obj.GetValue(BehaviorProperty) as IMediaControlBehavior;
        }

        public static void SetBehavior(DependencyObject obj, IMediaControlBehavior value)
        {
            obj.SetValue(BehaviorProperty, value);
        }

        /// <summary>
        /// Identifies the IsToolTipEnabled dependency property.
        /// </summary>
        public static readonly DependencyProperty IsToolTipEnabledProperty = DependencyProperty.RegisterAttached("IsToolTipEnabled", typeof(bool), typeof(MediaControls), new PropertyMetadata(false, (d, e) => MediaControls.OnIsToolTipEnabledChanged(d, (bool)e.OldValue, (bool)e.NewValue)));

        static void OnIsToolTipEnabledChanged(DependencyObject obj, bool oldValue, bool newValue)
        {
            if (newValue)
            {
                var behavior = GetBehavior(obj);
                if (behavior != null)
                {
                    BindingOperations.SetBinding(obj, ToolTipService.ToolTipProperty, new Binding() { Path = new PropertyPath("Label"), Source = behavior });
                }
            }
            else
            {
                ToolTipService.SetToolTip(obj, null);
            }
        }

        public static bool GetIsToolTipEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsToolTipEnabledProperty);
        }

        public static void SetIsToolTipEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsToolTipEnabledProperty, value);
        }
    }

    public interface IMediaControlBehavior
    {
        ICommand Command { get; }

        object Content { get; }

        string Label { get; }
    }

    public interface IElementAwareMediaBehavior : IMediaControlBehavior
    {
        DependencyObject Element { get; set; }
    }

    public abstract class MediaControlBehaviorBase : DependencyObject
    {
        ICommand command;

        /// <summary>
        /// Gets or sets the ViewModelCommand associated with the button.
        /// </summary>
        public ICommand Command
        {
            get { return command; }
            set
            {
                if (command != null)
                {
                    var vmCommand = command as ViewModelCommand;
                    if (vmCommand != null) vmCommand.ViewModel = null;
                }
                command = value;
                if (command != null)
                {
                    var vmCommand = command as ViewModelCommand;
                    if (vmCommand != null) vmCommand.ViewModel = ViewModel;
                }
            }
        }

        /// <summary>
        /// Identifies the MediaPlayer dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(IInteractiveViewModel), typeof(MediaControlBehaviorBase), new PropertyMetadata(null, (d, e) => ((MediaControlBehaviorBase)d).OnViewModelChanged(e.OldValue as IInteractiveViewModel, e.NewValue as IInteractiveViewModel)));

        /// <summary>
        /// Provides notification that the view model has changed.
        /// </summary>
        /// <param name="oldValue">The old view model. Note: this could be null.</param>
        /// <param name="newValue">The new view model. Note: this could be null.</param>
        protected virtual void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            var vmCommand = command as ViewModelCommand;
            if (vmCommand != null) vmCommand.ViewModel = newValue;
        }

        /// <summary>
        /// The InteractiveMediaPlayer object used to provide state updates and serve user interaction requests.
        /// This is usually an instance of the MediaPlayer but could be a custom implementation to support unique interaction such as in the case of advertising.
        /// </summary>
        public IInteractiveViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as IInteractiveViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }
    }

    public class MediaControlBehavior : MediaControlBehaviorBase, IMediaControlBehavior
    {
        /// <summary>
        /// Identifies the Content dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", typeof(object), typeof(MediaControlBehavior), null);

        /// <summary>
        /// Gets or sets the content of the element.
        /// </summary>
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Identifies the Label dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(MediaControlBehavior), null);

        /// <summary>
        /// Gets or sets the label of the element.
        /// </summary>
        public string Label
        {
            get { return GetValue(LabelProperty) as string; }
            set { SetValue(LabelProperty, value); }
        }
    }

    public class MediaToggleControlBehavior : MediaControlBehavior
    {
        /// <summary>
        /// Identifies the Value dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSetProperty = DependencyProperty.Register("IsSet", typeof(bool), typeof(MediaToggleControlBehavior), new PropertyMetadata(false, OnIsSetChanged));

        static void OnIsSetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as MediaToggleControlBehavior;
            var newValue = (bool)e.NewValue;
            if (newValue)
            {
                behavior.Content = behavior.SetContent ?? behavior.Content;
                behavior.Label = behavior.SetLabel ?? behavior.Label;
            }
            else
            {
                behavior.Content = behavior.UnsetContent ?? behavior.Content;
                behavior.Label = behavior.UnsetLabel ?? behavior.Label;
            }
        }

        /// <summary>
        /// Gets or sets whether the control is in an on or off state. This can impact UI aspects of the control.
        /// </summary>
        public bool IsSet
        {
            get { return (bool)GetValue(IsSetProperty); }
            set { SetValue(IsSetProperty, value); }
        }

        #region Content
        /// <summary>
        /// Identifies the SetContent dependency property.
        /// </summary>
        public static readonly DependencyProperty SetContentProperty = DependencyProperty.Register("SetContent", typeof(object), typeof(MediaToggleControlBehavior), new PropertyMetadata(null, OnSetContentChanged));

        static void OnSetContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as MediaToggleControlBehavior;
            if (behavior.IsSet)
            {
                behavior.Content = e.NewValue ?? behavior.Content;
            }
        }

        /// <summary>
        /// Gets or sets the contents of the control when in a 'set' state (IsSet=true).
        /// </summary>
        public object SetContent
        {
            get { return GetValue(SetContentProperty); }
            set { SetValue(SetContentProperty, value); }
        }

        /// <summary>
        /// Identifies the UnsetContent dependency property.
        /// </summary>
        public static readonly DependencyProperty UnsetContentProperty = DependencyProperty.Register("UnsetContent", typeof(object), typeof(MediaToggleControlBehavior), new PropertyMetadata(null, OnUnsetContentChanged));

        static void OnUnsetContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as MediaToggleControlBehavior;
            if (!behavior.IsSet)
            {
                behavior.Content = e.NewValue ?? behavior.Content;
            }
        }

        /// <summary>
        /// Gets or sets the control of the control when in an 'unset' state (IsSet=false).
        /// </summary>
        public object UnsetContent
        {
            get { return GetValue(UnsetContentProperty); }
            set { SetValue(UnsetContentProperty, value); }
        }

        #endregion

        #region Label
        /// <summary>
        /// Identifies the SetLabel dependency property.
        /// </summary>
        public static readonly DependencyProperty SetLabelProperty = DependencyProperty.Register("SetLabel", typeof(string), typeof(MediaToggleControlBehavior), new PropertyMetadata(null, OnSetLabelChanged));

        static void OnSetLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as MediaToggleControlBehavior;
            var newValue = e.NewValue as string;
            if (behavior.IsSet)
            {
                behavior.Label = newValue ?? behavior.Label;
            }
        }

        /// <summary>
        /// Gets or sets the label of the control when in a 'set' state (IsSet=true).
        /// </summary>
        public string SetLabel
        {
            get { return GetValue(SetLabelProperty) as string; }
            set { SetValue(SetLabelProperty, value); }
        }

        /// <summary>
        /// Identifies the UnsetLabel dependency property.
        /// </summary>
        public static readonly DependencyProperty UnselectedLabelProperty = DependencyProperty.Register("UnsetLabel", typeof(string), typeof(MediaToggleControlBehavior), new PropertyMetadata(null, OnUnsetLabelChanged));

        static void OnUnsetLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as MediaToggleControlBehavior;
            var newValue = e.NewValue as string;
            if (!behavior.IsSet)
            {
                behavior.Label = newValue ?? behavior.Label;
            }
        }

        /// <summary>
        /// Gets or sets the label of the control when in an 'unset' state (IsSet=false).
        /// </summary>
        public string UnsetLabel
        {
            get { return GetValue(UnselectedLabelProperty) as string; }
            set { SetValue(UnselectedLabelProperty, value); }
        }
        #endregion
    }
}
