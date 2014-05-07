using System;
using System.Collections;
using System.Collections.Generic;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using System.ComponentModel;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.PlayerFramework
{
    [TemplatePart(Name = SeekableSliderTemplateParts.HorizontalTemplate, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = SeekableSliderTemplateParts.HorizontalThumb, Type = typeof(Thumb))]
    [TemplatePart(Name = SeekableSliderTemplateParts.HorizontalAvailableBar, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = SeekableSliderTemplateParts.VerticalTemplate, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = SeekableSliderTemplateParts.VerticalThumb, Type = typeof(Thumb))]
    [TemplatePart(Name = SeekableSliderTemplateParts.VerticalAvailableBar, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = TimelineVisualStates.ScrubbingStates.IsScrubbing, GroupName = TimelineVisualStates.GroupNames.ScrubbingStates)]
    [TemplateVisualState(Name = TimelineVisualStates.ScrubbingStates.IsNotScrubbing, GroupName = TimelineVisualStates.GroupNames.ScrubbingStates)]
    public partial class SeekableSlider
    {
        // template controls
        /// <summary>
        /// The bar used to restrict the available area that can be scrubbed to.
        /// </summary>
        protected FrameworkElement AvailableBar { get; private set; }

        /// <summary>
        /// The Panel used to host the control.
        /// </summary>
        protected FrameworkElement Panel { get; private set; }

        /// <summary>
        /// The thumb used to allow the user to seek.
        /// </summary>
        protected Thumb Thumb { get; private set; }

        /// <summary>
        /// The thumb as an element instead of a Thumb. Needed for WP or custom templates that don't use a thumb control
        /// </summary>
        public FrameworkElement ThumbElement { get; private set; }

        #region Template Children

        /// <inheritdoc /> 
#if SILVERLIGHT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            UninitializeTemplateChildren();
            GetTemplateChildren();
            InitializeTemplateChildren();
        }

        private void GetTemplateChildren()
        {
            if (Orientation == Orientation.Horizontal)
            {
                Panel = GetTemplateChild(SeekableSliderTemplateParts.HorizontalTemplate) as FrameworkElement;
                Thumb = GetTemplateChild(SeekableSliderTemplateParts.HorizontalThumb) as Thumb;
                ThumbElement = Thumb ?? GetTemplateChild(SeekableSliderTemplateParts.HorizontalThumbElement) as FrameworkElement;
                AvailableBar = GetTemplateChild(SeekableSliderTemplateParts.HorizontalAvailableBar) as FrameworkElement;
            }
            else
            {
                Panel = GetTemplateChild(SeekableSliderTemplateParts.VerticalTemplate) as FrameworkElement;
                Thumb = GetTemplateChild(SeekableSliderTemplateParts.VerticalThumb) as Thumb;
                ThumbElement = Thumb ?? GetTemplateChild(SeekableSliderTemplateParts.VerticalThumbElement) as FrameworkElement;
                AvailableBar = GetTemplateChild(SeekableSliderTemplateParts.VerticalAvailableBar) as FrameworkElement;
            }
        }

        private void InitializeTemplateChildren()
        {
            if (AvailableBar != null)
            {
#if SILVERLIGHT
                AvailableBar.MouseLeftButtonDown += bar_PointerPressed;
                AvailableBar.MouseLeftButtonUp += bar_PointerReleased;
                AvailableBar.MouseMove += bar_PointerMoved;
#else
                AvailableBar.PointerPressed += bar_PointerPressed;
                AvailableBar.PointerReleased += bar_PointerReleased;
                AvailableBar.PointerMoved += bar_PointerMoved;
#endif
            }
            if (Panel != null)
            {
                Panel.SizeChanged += PanelSizeChanged;
            }

            if (Thumb != null)
            {
                Thumb.DragStarted += ThumbDragStarted;
                Thumb.DragDelta += ThumbDragDelta;
                Thumb.DragCompleted += ThumbDragCompleted;
            }
        }

        private void UninitializeTemplateChildren()
        {
            // main container
            if (AvailableBar != null)
            {
#if SILVERLIGHT
                AvailableBar.MouseLeftButtonDown -= bar_PointerPressed;
                AvailableBar.MouseLeftButtonUp -= bar_PointerReleased;
                AvailableBar.MouseMove -= bar_PointerMoved;
#else
                AvailableBar.PointerPressed -= bar_PointerPressed;
                AvailableBar.PointerReleased -= bar_PointerReleased;
                AvailableBar.PointerMoved -= bar_PointerMoved;
#endif
            }
            if (Panel != null)
            {
                Panel.SizeChanged -= PanelSizeChanged;
            }

            // thumb
            if (Thumb != null)
            {
                Thumb.DragStarted -= ThumbDragStarted;
                Thumb.DragDelta -= ThumbDragDelta;
                Thumb.DragCompleted -= ThumbDragCompleted;
            }
        }

        #endregion

        #region Dependency Properties

        #region ActualValue
        /// <summary>
        /// ActualValue DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty ActualValueProperty = DependencyProperty.Register("ActualValue", typeof(double), typeof(SeekableSlider), new PropertyMetadata(0.0, (d, e) => ((SeekableSlider)d).OnActualValueChanged((double)e.NewValue)));

        void OnActualValueChanged(double newValue)
        {
            if (!IsScrubbing && !inboundValue)
            {
                inboundValue = true;
                try
                {
                    Value = newValue;
                }
                finally
                {
                    inboundValue = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the actual value of the slider to be able to maintain the value of the slider while the user is scrubbing.
        /// </summary>
        public double ActualValue
        {
            get { return (double)GetValue(ActualValueProperty); }
            set { SetValue(ActualValueProperty, value); }
        }

        #endregion

        #region MaxValue
        /// <summary>
        /// MaxValue DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register("MaxValue", typeof(double), typeof(SeekableSlider), new PropertyMetadata(double.NaN, (d, e) => ((SeekableSlider)d).OnMaxValueChanged()));

        void OnMaxValueChanged()
        {
            RestrictAvailability();
        }

        /// <summary>
        /// Gets or sets the max position of the timeline.
        /// </summary>
        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        double Max
        {
            get
            {
                return double.IsNaN(MaxValue) ? Maximum : MaxValue;
            }
        }

        #endregion

        #region SliderThumbStyle
        /// <summary>
        /// SliderThumbStyle DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty SliderThumbStyleProperty = DependencyProperty.Register("SliderThumbStyle", typeof(Style), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the style for the slider thumb
        /// </summary>
        public Style SliderThumbStyle
        {
            get { return GetValue(SliderThumbStyleProperty) as Style; }
            set { SetValue(SliderThumbStyleProperty, value); }
        }

        #endregion

        #region ThumbnailContent
        /// <summary>
        /// ThumbnailContent DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty ThumbnailContentProperty = DependencyProperty.Register("ThumbnailContent", typeof(UIElement), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the UIElement to display behind the control
        /// </summary>
        public UIElement ThumbnailContent
        {
            get { return GetValue(ThumbnailContentProperty) as UIElement; }
            set { SetValue(ThumbnailContentProperty, value); }
        }

        #endregion

        #region ThumbnailVisibility
        /// <summary>
        /// ThumbnailVisibility DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty ThumbnailVisibilityProperty = DependencyProperty.Register("ThumbnailVisibility", typeof(Visibility), typeof(SeekableSlider), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Gets or sets if the thumbnail is visible
        /// </summary>
        public Visibility ThumbnailVisibility
        {
            get { return (Visibility)GetValue(ThumbnailVisibilityProperty); }
            set { SetValue(ThumbnailVisibilityProperty, value); }
        }

        #endregion

        #region HorizontalBackgroundContent
        /// <summary>
        /// HorizontalBackgroundContent DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty HorizontalBackgroundContentProperty = DependencyProperty.Register("HorizontalBackgroundContent", typeof(UIElement), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the UIElement to display behind the control
        /// </summary>
        public UIElement HorizontalBackgroundContent
        {
            get { return GetValue(HorizontalBackgroundContentProperty) as UIElement; }
            set { SetValue(HorizontalBackgroundContentProperty, value); }
        }

        #endregion

        #region HorizontalForegroundContent
        /// <summary>
        /// HorizontalForegroundContent DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty HorizontalForegroundContentProperty = DependencyProperty.Register("HorizontalForegroundContent", typeof(UIElement), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the UIElement to display in the foreground
        /// </summary>
        public UIElement HorizontalForegroundContent
        {
            get { return GetValue(HorizontalForegroundContentProperty) as UIElement; }
            set { SetValue(HorizontalForegroundContentProperty, value); }
        }

        #endregion

        #region VerticalBackgroundContent
        /// <summary>
        /// VerticalBackgroundContent DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty VerticalBackgroundContentProperty = DependencyProperty.Register("VerticalBackgroundContent", typeof(UIElement), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the UIElement to display behind the control
        /// </summary>
        public UIElement VerticalBackgroundContent
        {
            get { return GetValue(VerticalBackgroundContentProperty) as UIElement; }
            set { SetValue(VerticalBackgroundContentProperty, value); }
        }

        #endregion

        #region VerticalForegroundContent
        /// <summary>
        /// VerticalForegroundContent DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty VerticalForegroundContentProperty = DependencyProperty.Register("VerticalForegroundContent", typeof(UIElement), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the UIElement to display in the foreground
        /// </summary>
        public UIElement VerticalForegroundContent
        {
            get { return GetValue(VerticalForegroundContentProperty) as UIElement; }
            set { SetValue(VerticalForegroundContentProperty, value); }
        }

        #endregion

        #region SliderTrackDecreasePressedBackground
        /// <summary>
        /// SliderTrackDecreasePressedBackground DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty SliderTrackDecreasePressedBackgroundProperty = DependencyProperty.Register("SliderTrackDecreasePressedBackground", typeof(Brush), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the Brush to display in the foreground
        /// </summary>
        public Brush SliderTrackDecreasePressedBackground
        {
            get { return GetValue(SliderTrackDecreasePressedBackgroundProperty) as Brush; }
            set { SetValue(SliderTrackDecreasePressedBackgroundProperty, value); }
        }

        #endregion

        #region SliderTrackPressedBackground
        /// <summary>
        /// SliderTrackPressedBackground DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty SliderTrackPressedBackgroundProperty = DependencyProperty.Register("SliderTrackPressedBackground", typeof(Brush), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the Brush to display in the foreground
        /// </summary>
        public Brush SliderTrackPressedBackground
        {
            get { return GetValue(SliderTrackPressedBackgroundProperty) as Brush; }
            set { SetValue(SliderTrackPressedBackgroundProperty, value); }
        }

        #endregion

        #region SliderThumbPressedBackground
        /// <summary>
        /// SliderThumbPressedBackground DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty SliderThumbPressedBackgroundProperty = DependencyProperty.Register("SliderThumbPressedBackground", typeof(Brush), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the Brush to display in the foreground
        /// </summary>
        public Brush SliderThumbPressedBackground
        {
            get { return GetValue(SliderThumbPressedBackgroundProperty) as Brush; }
            set { SetValue(SliderThumbPressedBackgroundProperty, value); }
        }

        #endregion

        #region SliderThumbPressedBorder
        /// <summary>
        /// SliderThumbPressedBorder DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty SliderThumbPressedBorderProperty = DependencyProperty.Register("SliderThumbPressedBorder", typeof(Brush), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the Brush to display in the foreground
        /// </summary>
        public Brush SliderThumbPressedBorder
        {
            get { return GetValue(SliderThumbPressedBorderProperty) as Brush; }
            set { SetValue(SliderThumbPressedBorderProperty, value); }
        }

        #endregion

        #region SliderDisabledBorder
        /// <summary>
        /// SliderDisabledBorder DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty SliderDisabledBorderProperty = DependencyProperty.Register("SliderDisabledBorder", typeof(Brush), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the Brush to display in the foreground
        /// </summary>
        public Brush SliderDisabledBorder
        {
            get { return GetValue(SliderDisabledBorderProperty) as Brush; }
            set { SetValue(SliderDisabledBorderProperty, value); }
        }

        #endregion

        #region SliderTrackDecreaseDisabledBackground
        /// <summary>
        /// SliderTrackDecreaseDisabledBackground DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty SliderTrackDecreaseDisabledBackgroundProperty = DependencyProperty.Register("SliderTrackDecreaseDisabledBackground", typeof(Brush), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the Brush to display in the foreground
        /// </summary>
        public Brush SliderTrackDecreaseDisabledBackground
        {
            get { return GetValue(SliderTrackDecreaseDisabledBackgroundProperty) as Brush; }
            set { SetValue(SliderTrackDecreaseDisabledBackgroundProperty, value); }
        }

        #endregion

        #region SliderTrackDisabledBackground
        /// <summary>
        /// SliderTrackDisabledBackground DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty SliderTrackDisabledBackgroundProperty = DependencyProperty.Register("SliderTrackDisabledBackground", typeof(Brush), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the Brush to display in the foreground
        /// </summary>
        public Brush SliderTrackDisabledBackground
        {
            get { return GetValue(SliderTrackDisabledBackgroundProperty) as Brush; }
            set { SetValue(SliderTrackDisabledBackgroundProperty, value); }
        }

        #endregion

        #region SliderThumbDisabledBackground
        /// <summary>
        /// SliderThumbDisabledBackground DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty SliderThumbDisabledBackgroundProperty = DependencyProperty.Register("SliderThumbDisabledBackground", typeof(Brush), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the Brush to display in the foreground
        /// </summary>
        public Brush SliderThumbDisabledBackground
        {
            get { return GetValue(SliderThumbDisabledBackgroundProperty) as Brush; }
            set { SetValue(SliderThumbDisabledBackgroundProperty, value); }
        }

        #endregion

        #region SliderTrackDecreasePointerOverBackground
        /// <summary>
        /// SliderTrackDecreasePointerOverBackground DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty SliderTrackDecreasePointerOverBackgroundProperty = DependencyProperty.Register("SliderTrackDecreasePointerOverBackground", typeof(Brush), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the Brush to display in the foreground
        /// </summary>
        public Brush SliderTrackDecreasePointerOverBackground
        {
            get { return GetValue(SliderTrackDecreasePointerOverBackgroundProperty) as Brush; }
            set { SetValue(SliderTrackDecreasePointerOverBackgroundProperty, value); }
        }

        #endregion

        #region SliderTrackPointerOverBackground
        /// <summary>
        /// SliderTrackPointerOverBackground DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty SliderTrackPointerOverBackgroundProperty = DependencyProperty.Register("SliderTrackPointerOverBackground", typeof(Brush), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the Brush to display in the foreground
        /// </summary>
        public Brush SliderTrackPointerOverBackground
        {
            get { return GetValue(SliderTrackPointerOverBackgroundProperty) as Brush; }
            set { SetValue(SliderTrackPointerOverBackgroundProperty, value); }
        }

        #endregion

        #region SliderThumbPointerOverBackground
        /// <summary>
        /// SliderThumbPointerOverBackground DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty SliderThumbPointerOverBackgroundProperty = DependencyProperty.Register("SliderThumbPointerOverBackground", typeof(Brush), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the Brush to display in the foreground
        /// </summary>
        public Brush SliderThumbPointerOverBackground
        {
            get { return GetValue(SliderThumbPointerOverBackgroundProperty) as Brush; }
            set { SetValue(SliderThumbPointerOverBackgroundProperty, value); }
        }

        #endregion

        #region SliderThumbPointerOverBorder
        /// <summary>
        /// SliderThumbPointerOverBorder DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty SliderThumbPointerOverBorderProperty = DependencyProperty.Register("SliderThumbPointerOverBorder", typeof(Brush), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the Brush to display in the foreground
        /// </summary>
        public Brush SliderThumbPointerOverBorder
        {
            get { return GetValue(SliderThumbPointerOverBorderProperty) as Brush; }
            set { SetValue(SliderThumbPointerOverBorderProperty, value); }
        }

        #endregion

        #region SliderThumbBackground
        /// <summary>
        /// SliderThumbPointerOverBorder DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty SliderThumbBackgroundProperty = DependencyProperty.Register("SliderThumbBackground", typeof(Brush), typeof(SeekableSlider), null);

        /// <summary>
        /// Gets or sets the Brush to display in the foreground
        /// </summary>
        public Brush SliderThumbBackground
        {
            get { return GetValue(SliderThumbBackgroundProperty) as Brush; }
            set { SetValue(SliderThumbBackgroundProperty, value); }
        }

        #endregion

        #endregion

        #region Event Handlers

        private void PanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // bar size changed, update available bar
            RestrictAvailability();
        }

        private void ThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            UpdateScrubbingVisualState();
            OnScrubbingStarted(new ValueRoutedEventArgs(Value));
        }

        private void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (Thumb.IsDragging)
            {
                if (Value > Max) Value = Max;
            }
        }

        private void ThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (!e.Canceled)
            {
                if (Value > Max) Value = Max;
                UpdateScrubbingVisualState();
                OnScrubbingCompleted(new ValueRoutedEventArgs(Value));
            }
        }

#if SILVERLIGHT
        private void bar_PointerPressed(object sender, MouseButtonEventArgs e)
#else
        private void bar_PointerPressed(object sender, PointerRoutedEventArgs e)
#endif
        {
#if SILVERLIGHT
            pointerCaptured = ((FrameworkElement)sender).CaptureMouse();
#else
            pointerCaptured = ((FrameworkElement)sender).CapturePointer(e.Pointer);
#endif
            UpdateScrubbingVisualState();
            if (pointerCaptured)
            {
#if SILVERLIGHT
                pointerReleaseAction = () => ((FrameworkElement)sender).ReleaseMouseCapture();
#else
                pointerReleaseAction = () => ((FrameworkElement)sender).ReleasePointerCapture(e.Pointer);
#endif
                double? newValue = null;
                if (Orientation == Orientation.Horizontal) newValue = GetHorizontalPanelMousePosition(e);
                else newValue = GetVerticalPanelMousePosition(e);

                if (newValue.HasValue)
                {
                    var value = Math.Min(newValue.Value, Max);
                    var args = new ValueRoutedEventArgs(value);
                    OnScrubbingStarted(args);
                    if (!args.Canceled)
                    {
                        Value = value;
                    }
                }
            }
            e.Handled = true;
        }

#if SILVERLIGHT
        private void bar_PointerReleased(object sender, MouseButtonEventArgs e)
#else
        private void bar_PointerReleased(object sender, PointerRoutedEventArgs e)
#endif
        {
            if (pointerCaptured)
            {
                double? newValue = null;
                if (Orientation == Orientation.Horizontal) newValue = GetHorizontalPanelMousePosition(e);
                else newValue = GetVerticalPanelMousePosition(e);

                if (newValue.HasValue)
                {
                    Value = Math.Min(newValue.Value, Max);
                }

                if (pointerCaptured)
                {
                    OnScrubbingCompleted(new ValueRoutedEventArgs(Value));
                    pointerReleaseAction();
                    pointerReleaseAction = null;
                    pointerCaptured = false;
                    UpdateScrubbingVisualState();
                }
            }
            e.Handled = true;
        }

#if SILVERLIGHT
        private void bar_PointerMoved(object sender, MouseEventArgs e)
#else
        private void bar_PointerMoved(object sender, PointerRoutedEventArgs e)
#endif
        {
            if (pointerCaptured)
            {
                double? newValue = null;
                if (Orientation == Orientation.Horizontal) newValue = GetHorizontalPanelMousePosition(e);
                else newValue = GetVerticalPanelMousePosition(e);

                if (newValue.HasValue)
                {
                    Value = Math.Min(newValue.Value, Max);
                }
            }
#if !SILVERLIGHT
            e.Handled = true;
#endif
        }

        #endregion

        #region Misc
        private void UpdateScrubbingVisualState()
        {
            var state = IsScrubbing ? TimelineVisualStates.ScrubbingStates.IsScrubbing : TimelineVisualStates.ScrubbingStates.IsNotScrubbing;
            this.GoToVisualState(state);
        }

        /// <summary>
        /// Cancels the active scrub
        /// </summary>
        public void CancelScrub()
        {
            if (pointerCaptured)
            {
                pointerReleaseAction();
                pointerReleaseAction = null;
                pointerCaptured = false;
            }
            else if (Thumb.IsDragging)
            {
                Thumb.CancelDrag();
#if SILVERLIGHT
                Thumb.ReleaseMouseCapture();
#else
                Thumb.ReleasePointerCaptures();
#endif
            }
            UpdateScrubbingVisualState();
        }
        #endregion
    }
}