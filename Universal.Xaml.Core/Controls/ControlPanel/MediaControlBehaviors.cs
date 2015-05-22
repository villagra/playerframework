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
    /// <summary>
    /// Help extension class to help with media control behaviors.
    /// </summary>
    public static class MediaControlBehaviors
    {
        /// <summary>
        /// Creates a strongly typed media control behavior (IMediaControlBehavior) that can be assigned to a button via MediaControls.SetBehavior(button, behavior)
        /// </summary>
        /// <typeparam name="T">The behavior type. There is one per transport bar button.</typeparam>
        /// <param name="source">The instance of the media player to bind the behavior to.</param>
        /// <returns>A behavior that can be passed to the MediaControls.Behavior attached property</returns>
        public static T CreateMediaControlBehavior<T>(this MediaPlayer source) where T : MediaControlBehaviorBase, new()
        {
            var behavior = new T();
            BindingOperations.SetBinding(behavior, MediaControlBehaviorBase.ViewModelProperty, new Binding() { Path = new PropertyPath("InteractiveViewModel"), Source = source });
            return behavior;
        }
    }

    /// <summary>
    /// A static class used that defines an attached property that can help associate a media control behavior (IMediaControlBehavior) with a control (usually a button).
    /// This allows applications to create controls normally found in the transport bar that can live outside the player framework.
    /// </summary>
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
                        appbarButton.Label = "";
                        appbarButton.Icon = null;
                    }
                    else
                    {
                        var appbarToggleButton = buttonBase as AppBarToggleButton;
                        if (appbarToggleButton != null)
                        {
                            appbarToggleButton.Label = "";
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
                AutomationProperties.SetName(obj, "");
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
                        appbarButton.SetBinding(AppBarButton.LabelProperty, new Binding() { Path = new PropertyPath("Label"), Source = newValue, Converter = newValue.LabelConverter });
                        appbarButton.SetBinding(AppBarButton.IconProperty, new Binding() { Path = new PropertyPath("Content"), Source = newValue, Converter = newValue.ContentConverter });
                    }
                    else
                    {
                        var appbarToggleButton = buttonBase as AppBarToggleButton;
                        if (appbarToggleButton != null)
                        {
                            appbarToggleButton.SetBinding(AppBarToggleButton.LabelProperty, new Binding() { Path = new PropertyPath("Label"), Source = newValue, Converter = newValue.LabelConverter });
                            appbarToggleButton.SetBinding(AppBarToggleButton.IconProperty, new Binding() { Path = new PropertyPath("Content"), Source = newValue, Converter = newValue.ContentConverter });
                        }
                        else
                        {
                            contentControl.SetBinding(ContentControl.ContentProperty, new Binding() { Path = new PropertyPath("Content"), Source = newValue, Converter = newValue.ContentConverter });
                        }
                    }
#else
                    contentControl.SetBinding(ContentControl.ContentProperty, new Binding() { Path = new PropertyPath("Content"), Source = newValue, Converter = newValue.ContentConverter });
#endif
                }
                else
                {
                    var textBlock = obj as TextBlock;
                    if (textBlock != null)
                    {
                        textBlock.SetBinding(TextBlock.TextProperty, new Binding() { Path = new PropertyPath("Content"), Source = newValue, Converter = newValue.ContentConverter });
                    }
                }

                BindingOperations.SetBinding(obj, AutomationProperties.NameProperty, new Binding() { Path = new PropertyPath("Label"), Source = newValue, Converter = newValue.LabelConverter });
                if (GetIsToolTipEnabled(obj))
                {
                    BindingOperations.SetBinding(obj, ToolTipService.ToolTipProperty, new Binding() { Path = new PropertyPath("Label"), Source = newValue, Converter = newValue.LabelConverter });
                }
            }
        }

        /// <summary>
        /// Gets the behavior associated with the object.
        /// </summary>
        /// <param name="obj">The object to retrieve the behavior for.</param>
        /// <returns>The behavior associated with the object.</returns>
        public static IMediaControlBehavior GetBehavior(DependencyObject obj)
        {
            return obj.GetValue(BehaviorProperty) as IMediaControlBehavior;
        }

        /// <summary>
        /// Sets the behavior on an object.
        /// </summary>
        /// <param name="obj">The object to set the behavior on.</param>
        /// <param name="value">The behavior to be associated with the object.</param>
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

        /// <summary>
        /// Gets a flag indicating if tooltips shoudl be enabled on the object.
        /// </summary>
        /// <param name="obj">The object to retrieve the flag for.</param>
        /// <returns>The flag associated with the object.</returns>
        public static bool GetIsToolTipEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsToolTipEnabledProperty);
        }

        /// <summary>
        /// Sets a flag indicating if tooltips should be enabled on the object.
        /// </summary>
        /// <param name="obj">The object to set the flag on.</param>
        /// <param name="value">The flag to be associated with the object.</param>
        public static void SetIsToolTipEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsToolTipEnabledProperty, value);
        }
    }

    /// <summary>
    /// Represents an object that help define the behavior and content of a control associated with the player framework.
    /// </summary>
    public interface IMediaControlBehavior
    {
        /// <summary>
        /// Gets the command associated with the control.
        /// </summary>
        ICommand Command { get; }

        /// <summary>
        /// Gets the content for the control.
        /// </summary>
        object Content { get; }

        /// <summary>
        /// Gets the label associated with the control.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Gets a converter to be used when assigning the content property
        /// </summary>
        IValueConverter ContentConverter { get; }

        /// <summary>
        /// Gets a converter to be used when assigning the content property
        /// </summary>
        IValueConverter LabelConverter { get; }
    }

    /// <summary>
    /// Represents a special kind of behavior that has knowlege of the control it is associated with. This can be useful if the behavior needs to react to events on the control.
    /// </summary>
    public interface IElementAwareMediaBehavior : IMediaControlBehavior
    {
        /// <summary>
        /// The element the behavior has been associated with.
        /// </summary>
        DependencyObject Element { get; set; }
    }

    /// <summary>
    /// A base class that can help keep a ViewModelCommand object associated with the current IInteractiveViewModel.
    /// </summary>
    public abstract class MediaControlBehaviorBase : DependencyObject
    {
        ICommand command;

        /// <summary>
        /// Gets or sets the Command object (usually a ViewModelCommand).
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
        /// Identifies the ViewModel dependency property.
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
        /// This is usually an instance of the MediaPlayer's default InteractiveViewModel but could be a custom implementation to support unique interaction such as in the case of advertising.
        /// </summary>
        public IInteractiveViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as IInteractiveViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }
    }

    /// <summary>
    /// Represents a behavior that can be attached to a control to make it behave like the default instance of that control provided with the player framework.
    /// </summary>
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

        /// <summary>
        /// Identifies the ContentConverter dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentConverterProperty = DependencyProperty.Register("ContentConverter", typeof(IValueConverter), typeof(MediaControlBehavior), null);

        /// <summary>
        /// Gets or sets the ContentConverter of the element.
        /// </summary>
        public IValueConverter ContentConverter
        {
            get { return GetValue(ContentConverterProperty) as IValueConverter; }
            set { SetValue(ContentConverterProperty, value); }
        }

        /// <summary>
        /// Identifies the LabelConverter dependency property.
        /// </summary>
        public static readonly DependencyProperty LabelConverterProperty = DependencyProperty.Register("LabelConverter", typeof(IValueConverter), typeof(MediaControlBehavior), null);

        /// <summary>
        /// Gets or sets the LabelConverter of the element.
        /// </summary>
        public IValueConverter LabelConverter
        {
            get { return GetValue(LabelConverterProperty) as IValueConverter; }
            set { SetValue(LabelConverterProperty, value); }
        }
    }

    /// <summary>
    /// Represents a behavior that can be attached to a control to make it behave like the default instance of that control provided with the player framework.
    /// This particular MediaControlBehavior allows the content and label to change when the IsSet property changes and is useful for toggle button scenarios.
    /// </summary>
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
