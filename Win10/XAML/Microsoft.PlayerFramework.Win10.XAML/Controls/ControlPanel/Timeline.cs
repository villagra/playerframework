using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace Microsoft.PlayerFramework
{
    internal static class TimelineTemplateParts
    {
        public const string PositionedItemsControl = "PositionedItemsControl";
        public const string DownloadProgressBarElement = "DownloadProgressBar";
        public const string ProgressSliderElement = "ProgressSlider";
    }

    /// <summary>
    /// Provides a Timeline control that can be easily bound to an InteractiveViewModel (e.g. MediaPlayer.InteractiveViewModel)
    /// </summary>
    [TemplatePart(Name = TimelineTemplateParts.DownloadProgressBarElement, Type = typeof(ProgressBar))]
    [TemplatePart(Name = TimelineTemplateParts.ProgressSliderElement, Type = typeof(SeekableSlider))]
    [TemplatePart(Name = TimelineTemplateParts.PositionedItemsControl, Type = typeof(PositionedItemsControl))]
    public class Timeline : Control
    {
        /// <summary>
        /// The download progress bar for non-adaptive video.
        /// </summary>
        protected ProgressBar DownloadProgressBarElement { get; private set; }
        /// <summary>
        /// The timeline.
        /// </summary>
        protected SeekableSlider ProgressSliderElement { get; private set; }
        /// <summary>
        /// The marker container.
        /// </summary>
        protected PositionedItemsControl PositionedItemsControl { get; private set; }

        /// <summary>
        /// Creates a new instance of Timeline
        /// </summary>
        public Timeline()
        {
            this.DefaultStyleKey = typeof(Timeline);
            AutomationProperties.SetName(this, MediaPlayer.GetResourceString("TimelineLabel"));
        }

        /// <inheritdoc /> 
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ProgressSliderElement != null)
            {
                // unwire existing event handlers if template was already applied
                UnwireProgressSliderEvents();
            }

            if (PositionedItemsControl != null)
            {
                PositionedItemsControl.ItemLoaded -= PositionedItemsControl_ItemLoaded;
                PositionedItemsControl.ItemUnloaded -= PositionedItemsControl_ItemUnloaded;
            }

            PositionedItemsControl = GetTemplateChild(TimelineTemplateParts.PositionedItemsControl) as PositionedItemsControl;
            DownloadProgressBarElement = GetTemplateChild(TimelineTemplateParts.DownloadProgressBarElement) as ProgressBar;
            ProgressSliderElement = GetTemplateChild(TimelineTemplateParts.ProgressSliderElement) as SeekableSlider;


            if (ProgressSliderElement != null)
            {
                ProgressSliderElement.Style = SliderStyle;
                // wire up events to bubble
                WireProgressSliderEvents();
            }

            InitializeProgressSlider();

            if (PositionedItemsControl != null)
            {
                PositionedItemsControl.ItemLoaded += PositionedItemsControl_ItemLoaded;
                PositionedItemsControl.ItemUnloaded += PositionedItemsControl_ItemUnloaded;
            }
        }

        private void UnwireProgressSliderEvents()
        {
            ProgressSliderElement.Seeked -= ProgressSliderElement_Seeked;
            ProgressSliderElement.ScrubbingStarted -= ProgressSliderElement_ScrubbingStarted;
            ProgressSliderElement.Scrubbing -= ProgressSliderElement_Scrubbing;
            ProgressSliderElement.ScrubbingCompleted -= ProgressSliderElement_ScrubbingCompleted;
        }

        private void WireProgressSliderEvents()
        {
            ProgressSliderElement.Seeked += ProgressSliderElement_Seeked;
            ProgressSliderElement.ScrubbingStarted += ProgressSliderElement_ScrubbingStarted;
            ProgressSliderElement.Scrubbing += ProgressSliderElement_Scrubbing;
            ProgressSliderElement.ScrubbingCompleted += ProgressSliderElement_ScrubbingCompleted;
        }

        void PositionedItemsControl_ItemUnloaded(object sender, FrameworkElementEventArgs args)
        {
            if (args.Element is ButtonBase)
            {
                ((ButtonBase)args.Element).Click -= Timeline_Click;
            }
        }

        void PositionedItemsControl_ItemLoaded(object sender, FrameworkElementEventArgs args)
        {
            if (args.Element is ButtonBase)
            {
                ((ButtonBase)args.Element).Click += Timeline_Click;
            }
        }

        void Timeline_Click(object sender, RoutedEventArgs e)
        {
            var marker = ((ButtonBase)sender).DataContext as VisualMarker;
            bool canceled = false;
            ViewModel.Seek(marker.Time, out canceled);
        }

        void ProgressSliderElement_Seeked(object sender, ValueRoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                bool canceled = false;
                ViewModel.Seek(TimeSpan.FromSeconds(e.Value), out canceled);
                e.Canceled = canceled;
            }
        }

        void ProgressSliderElement_Scrubbing(object sender, ValueRoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                var vm = ViewModel; // hold onto this in case the ViewModel changes because of this action. This way we can ensure we're calling the same one.
                bool canceled = false;
                vm.Scrub(TimeSpan.FromSeconds(e.Value), out canceled);
                if (canceled)
                {
                    ProgressSliderElement.CancelScrub();
                    vm.CompleteScrub(TimeSpan.FromSeconds(e.Value), ref canceled);
                }
                e.Canceled = canceled;
            }
        }

        void ProgressSliderElement_ScrubbingCompleted(object sender, ValueRoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                bool canceled = false;
                ViewModel.CompleteScrub(TimeSpan.FromSeconds(e.Value), ref canceled);
                e.Canceled = canceled;
            }
        }

        void ProgressSliderElement_ScrubbingStarted(object sender, ValueRoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                var vm = ViewModel; // hold onto this in case the ViewModel changes because of this action. This way we can ensure we're calling the same one.
                bool canceled = false;
                vm.StartScrub(TimeSpan.FromSeconds(e.Value), out canceled);
                if (canceled)
                {
                    ProgressSliderElement.CancelScrub();
                    vm.CompleteScrub(TimeSpan.FromSeconds(e.Value), ref canceled);
                }
                e.Canceled = canceled;
            }
        }

        /// <summary>
        /// Identifies the MediaPlayer dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(IInteractiveViewModel), typeof(Timeline), new PropertyMetadata(null, (d, e) => ((Timeline)d).OnViewModelChanged(e.OldValue as IInteractiveViewModel, e.NewValue as IInteractiveViewModel)));

        void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            InitializeProgressSlider();
        }

        private void InitializeProgressSlider()
        {
            if (ProgressSliderElement != null)
            {
                UnwireProgressSliderEvents();

                if (ViewModel != null)
                {
                    ProgressSliderElement.SetBinding(SeekableSlider.IsEnabledProperty, new Binding() { Path = new PropertyPath("IsScrubbingEnabled"), Source = ViewModel });
                    ProgressSliderElement.SetBinding(SeekableSlider.ActualValueProperty, new Binding() { Path = new PropertyPath("Position.TotalSeconds"), Source = ViewModel });
                    ProgressSliderElement.SetBinding(SeekableSlider.MinimumProperty, new Binding() { Path = new PropertyPath("StartTime.TotalSeconds"), Source = ViewModel });
                    ProgressSliderElement.SetBinding(SeekableSlider.MaximumProperty, new Binding() { Path = new PropertyPath("EndTime.TotalSeconds"), Source = ViewModel });
                    ProgressSliderElement.SetBinding(SeekableSlider.MaxValueProperty, new Binding() { Path = new PropertyPath("MaxPosition.TotalSeconds"), Source = ViewModel });
                }
                else
                {
                    ProgressSliderElement.ClearValue(SeekableSlider.IsEnabledProperty);
                    ProgressSliderElement.ClearValue(SeekableSlider.ActualValueProperty);
                    ProgressSliderElement.ClearValue(SeekableSlider.MinimumProperty);
                    ProgressSliderElement.ClearValue(SeekableSlider.MaximumProperty);
                    ProgressSliderElement.ClearValue(SeekableSlider.MaxValueProperty);
                }

                WireProgressSliderEvents();
            }
        }

        /// <summary>
        /// The InteractiveMediaPlayer object used to provide state updates and serve user interaction requests.
        /// This property is usually bound to MediaPlayer.InteractiveViewModel but could be a custom implementation to support unique interaction such as in the case of advertising.
        /// </summary>
        public IInteractiveViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as IInteractiveViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }

        /// <summary>
        /// Identifies the MediaPlayer dependency property.
        /// </summary>
        public static readonly DependencyProperty SliderStyleProperty = DependencyProperty.Register("SliderStyle", typeof(Style), typeof(Timeline), new PropertyMetadata(null, (d, e) => ((Timeline)d).OnSliderStyleChanged(e.NewValue as Style)));

        void OnSliderStyleChanged(Style newValue)
        {
            if (ProgressSliderElement != null)
            {
                ProgressSliderElement.Style = newValue;
            }
        }

        /// <summary>
        /// The InteractiveMediaPlayer object used to provide state updates and serve user interaction requests.
        /// This is usually an instance of the MediaPlayer but could be a custom implementation to support unique interaction such as in the case of advertising.
        /// </summary>
        public Style SliderStyle
        {
            get { return GetValue(SliderStyleProperty) as Style; }
            set { SetValue(SliderStyleProperty, value); }
        }

        /// <summary>
        /// ThumbnailContent DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty ThumbnailContentProperty = DependencyProperty.Register("ThumbnailContent", typeof(UIElement), typeof(Timeline), null);

        /// <summary>
        /// Gets or sets the UIElement to display as the thumbnail
        /// </summary>
        public UIElement ThumbnailContent
        {
            get { return GetValue(ThumbnailContentProperty) as UIElement; }
            set { SetValue(ThumbnailContentProperty, value); }
        }

        /// <summary>
        /// ThumbnailVisibility DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty ThumbnailVisibilityProperty = DependencyProperty.Register("ThumbnailVisibility", typeof(Visibility), typeof(Timeline), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or sets if the thumbnail is visible
        /// </summary>
        public Visibility ThumbnailVisibility
        {
            get { return (Visibility)GetValue(ThumbnailVisibilityProperty); }
            set { SetValue(ThumbnailVisibilityProperty, value); }
        }
    }
}
