using System;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.Foundation;
using Windows.UI.Input;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Represents a control that shows a visual indicator of the duration of the current media and current position.
    /// </summary>
    /// <remarks>
    /// The Timeline keeps track of the current position, start position, and end position. 
    /// </remarks>
    public partial class SeekableSlider : Slider
    {
        private bool pointerCaptured;
        private Action pointerReleaseAction;
        private bool ignoreValueChanged;
        private bool inboundValue;

        /// <summary>
        /// Instantiates a new instance of the SeekableSlider class.
        /// </summary>
        public SeekableSlider()
        {
            DefaultStyleKey = typeof(SeekableSlider);
            this.SizeChanged += SeekableSlider_SizeChanged;
        }

        /// <summary>
        /// Gets or sets whether the timeline is scrubbing.
        /// </summary>
        public bool IsScrubbing
        {
            get
            {
                return pointerCaptured || (Thumb != null && Thumb.IsDragging);
            }
        }

        /// <summary>
        /// Occurs when the user seeked.
        /// </summary>
        public event EventHandler<ValueRoutedEventArgs> Seeked;

        /// <summary>
        /// Occurs when the user begins scrubbing.
        /// </summary>
        public event EventHandler<ValueRoutedEventArgs> ScrubbingStarted;

        /// <summary>
        /// Occurs when the user scrubs.
        /// </summary>
        public event EventHandler<ValueRoutedEventArgs> Scrubbing;

        /// <summary>
        /// Occurs when the user completes scrubbing.
        /// </summary>
        public event EventHandler<ValueRoutedEventArgs> ScrubbingCompleted;

        /// <summary>
        /// Invokes the Seeked event.
        /// </summary>
        /// <param name="e">EventArgs used to provide info about the event</param>
        protected void OnSeeked(ValueRoutedEventArgs e)
        {
            if (Seeked != null)
            {
                ignoreValueChanged = true;
                try
                {
                    Seeked(this, e);
                }
                finally { ignoreValueChanged = false; }
            }
        }

        /// <summary>
        /// Invokes the ScrubbingCompleted event.
        /// </summary>
        /// <param name="e">EventArgs used to provide info about the event</param>
        protected void OnScrubbingCompleted(ValueRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "NotScrubbing", true);
            if (ScrubbingCompleted != null)
            {
                ignoreValueChanged = true;
                try
                {
                    ScrubbingCompleted(this, e);
                }
                finally { ignoreValueChanged = false; }
            }
        }

        /// <summary>
        /// Invokes the Scrubbing event.
        /// </summary>
        /// <param name="e">EventArgs used to provide info about the event</param>
        protected void OnScrubbing(ValueRoutedEventArgs e)
        {
            if (Scrubbing != null)
            {
                ignoreValueChanged = true;
                try
                {
                    Scrubbing(this, e);
                }
                finally { ignoreValueChanged = false; }
            }
        }

        /// <summary>
        /// Invokes the ScrubbingStarted event.
        /// </summary>
        /// <param name="e">EventArgs used to provide info about the event</param>
        protected void OnScrubbingStarted(ValueRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Scrubbing", true);
            if (ScrubbingStarted != null)
            {
                ignoreValueChanged = true;
                try
                {
                    ScrubbingStarted(this, e);
                }
                finally { ignoreValueChanged = false; }
            }
        }

        /// <inheritdoc /> 
        protected override void OnValueChanged(double oldValue, double newValue)
        {
            if (!ignoreValueChanged)
            {
                var value = Math.Min(newValue, Max);
                if (inboundValue || !IsEnabled)
                {
                    base.OnValueChanged(oldValue, value);
                }
                else
                {
                    var args = new ValueRoutedEventArgs(value);
                    if (IsScrubbing)
                    {
                        OnScrubbing(args);
                    }
                    else
                    {
                        OnSeeked(args);
                    }
                    if (!args.Canceled)
                    {
                        base.OnValueChanged(oldValue, value);
                    }
                    else
                    {
                        inboundValue = true;
                        try
                        {
                            Value = oldValue;
                        }
                        finally
                        {
                            inboundValue = false;
                        }
                    }
                }
            }
        }

        /// <inheritdoc /> 
        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            RestrictAvailability();
            RefreshStepFrequency();
        }

        /// <inheritdoc /> 
        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            RestrictAvailability();
            RefreshStepFrequency();
        }

        void SeekableSlider_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshStepFrequency();
        }

        void RefreshStepFrequency()
        {
#if !SILVERLIGHT
            var length = Orientation == Orientation.Horizontal ? RenderSize.Width : RenderSize.Height;
            if (length > 0)
            {
                StepFrequency = (Maximum - Minimum) / length;
            }
#endif
        }

        private void RestrictAvailability()
        {
            double range = Maximum - Minimum;

            // convert the available unit to a pixel width

            if (AvailableBar != null)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    if (Panel != null && Panel.ActualWidth > 0 && range > 0)
                    {
                        var thumbWidth = ThumbElement == null ? 0 : ThumbElement.ActualWidth;
                        // calculate the pixel width of the available bar, need to take into
                        // account the _horizontalThumb width, otherwise the _horizontalThumb position is calculated differently
                        // then the available bar (the _horizontalThumb position takes into account the _horizontalThumb width)
                        double availableRange = Max - Minimum;
                        double pixelValue = (availableRange / range) * (Panel.ActualWidth - thumbWidth);

                        // want the width of the available bar to be aligned with the right of the _horizontalThumb, this
                        // allows the maxposition indictor to be correctly positioned to the right of the _horizontalThumb
                        pixelValue += thumbWidth;

                        // make sure within range, Available can be negative when it first starts
                        pixelValue = Math.Min(Panel.ActualWidth, pixelValue);
                        pixelValue = Math.Max(0, pixelValue);

                        AvailableBar.Width = pixelValue;
                    }
                    else
                    {
                        AvailableBar.Width = Panel.ActualWidth;
                    }
                }
                else
                {
                    if (Panel != null && Panel.ActualHeight > 0 && range > 0)
                    {
                        var thumbHeight = ThumbElement == null ? 0 : ThumbElement.ActualHeight;
                        // calculate the pixel width of the available bar, need to take into
                        // account the Thumb width, otherwise the Thumb position is calculated differently
                        // then the available bar (the Thumb position takes into account the Thumb width)
                        double availableRange = Max - Minimum;
                        double pixelValue = (availableRange / range) * (Panel.ActualHeight - thumbHeight);

                        // want the width of the available bar to be aligned with the right of the Thumb, this
                        // allows the maxposition indictor to be correctly positioned to the right of the Thumb
                        pixelValue += thumbHeight;

                        // make sure within range, Available can be negative when it first starts
                        pixelValue = Math.Min(Panel.ActualHeight, pixelValue);
                        pixelValue = Math.Max(0, pixelValue);

                        AvailableBar.Height = pixelValue;
                    }
                    else
                    {
                        AvailableBar.Height = 0;
                    }
                }
            }
        }

#if SILVERLIGHT
        private double? GetHorizontalPanelMousePosition(MouseEventArgs e)
#else
        private double? GetHorizontalPanelMousePosition(PointerRoutedEventArgs e)
#endif
        {
            // take into account the scrubber _horizontalThumb size
            double thumbWidth = (ThumbElement == null) ? 0 : ThumbElement.ActualWidth;
            double panelWidth = Panel.ActualWidth - thumbWidth;

            if (panelWidth > 0)
            {
                double range = Maximum - Minimum;

                // calculate the new newValue based on mouse position
#if SILVERLIGHT
                Point mousePosition = e.GetPosition(Panel);
#else
                Point mousePosition = e.GetCurrentPoint(Panel).Position;
#endif
                double x = mousePosition.X - thumbWidth / 2;
                x = Math.Min(Math.Max(0, x), panelWidth);
                double value = (x * range) / panelWidth;

                // offset from the min newValue
                value += Minimum;

                return value;
            }
            else return null;
        }

#if SILVERLIGHT
        private double? GetVerticalPanelMousePosition(MouseEventArgs e)
#else
        private double? GetVerticalPanelMousePosition(PointerRoutedEventArgs e)
#endif
        {
            // take into account the scrubber _horizontalThumb size
            double thumbHeight = (ThumbElement == null) ? 0 : ThumbElement.ActualHeight;
            double panelHeight = Panel.ActualHeight - thumbHeight;

            if (panelHeight > 0)
            {
                double range = Maximum - Minimum;

                // calculate the new newValue based on mouse position
#if SILVERLIGHT
                Point mousePosition = e.GetPosition(Panel);
#else
                Point mousePosition = e.GetCurrentPoint(Panel).Position;
#endif
                double y = mousePosition.Y - thumbHeight / 2;
                y = Math.Min(Math.Max(0, y), panelHeight);
                double value = ((panelHeight - y) * range) / panelHeight;

                // offset from the min newValue
                value += Minimum;

                return value;
            }
            else return null;
        }
    }

    /// <summary>
    /// EventArgs class to return a double.
    /// </summary>
    public class ValueRoutedEventArgs : RoutedEventArgs
    {
        internal ValueRoutedEventArgs(double Value)
        {
            this.Value = Value;
        }

        /// <summary>
        /// The value associated with the event.
        /// </summary>
        public double Value { get; internal set; }

        /// <summary>
        /// The value associated with the event.
        /// </summary>
        public bool Canceled { get; set; }
    }
}