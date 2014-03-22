using System;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A panel used to position the children items in a linear path based on relative coordinates (defined by attached properties).
    /// </summary>
    public class PositionedItemsPanel : Panel
    {
        // dependency property notification
        private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PositionedItemsPanel)d).InvalidateArrange();
        }

        #region Minimum
        /// <summary>
        /// Minimum DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(PositionedItemsPanel), new PropertyMetadata(0.0, OnDependencyPropertyChanged));

        /// <summary>
        /// Gets or sets the minimum position of the items.
        /// </summary>
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        #endregion

        #region Maximum
        /// <summary>
        /// Maximum DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(PositionedItemsPanel), new PropertyMetadata(100.0, OnDependencyPropertyChanged));

        /// <summary>
        /// Gets or sets the maximum position of the items.
        /// </summary>
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        #endregion

        #region MaxPosition
        /// <summary>
        /// MaxPosition DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty MaxPositionProperty = DependencyProperty.Register("MaxPosition", typeof(double?), typeof(PositionedItemsPanel), new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// Gets or sets the max position.
        /// </summary>
        public double? MaxPosition
        {
            get { return (double?)GetValue(MaxPositionProperty); }
            set { SetValue(MaxPositionProperty, value); }
        }
        #endregion

        #region MinPosition
        /// <summary>
        /// MinPosition DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty MinPositionProperty = DependencyProperty.Register("MinPosition", typeof(double?), typeof(PositionedItemsPanel), new PropertyMetadata(null, OnDependencyPropertyChanged));

        /// <summary>
        /// Gets or sets the min position.
        /// </summary>
        public double? MinPosition
        {
            get { return (double?)GetValue(MinPositionProperty); }
            set { SetValue(MinPositionProperty, value); }
        }
        #endregion

        #region DisplayAllItems
        /// <summary>
        /// DisplayAllItemsProperty DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty DisplayAllItemsProperty = DependencyProperty.Register("DisplayAllItems", typeof(bool), typeof(PositionedItemsPanel), new PropertyMetadata(true, OnDependencyPropertyChanged));

        /// <summary>
        /// Gets or sets whether items outside the MinPosition and MaxPosition are displayed
        /// </summary>
        public bool DisplayAllItems
        {
            get { return (bool)GetValue(DisplayAllItemsProperty); }
            set { SetValue(DisplayAllItemsProperty, value); }
        }
        #endregion

        /// <inheritdoc /> 
        protected override Size MeasureOverride(Size availableSize)
        {
            // this is the first pass in the layout, each control updates
            // its DesiredSize property, which is used later in ArrangeOverride
            foreach (var child in Children)
            {
                child.Measure(availableSize);
            }
            return base.MeasureOverride(availableSize);
        }

        /// <inheritdoc /> 
        protected override Size ArrangeOverride(Size finalSize)
        {
            // this is the bounds where items are visisble
            double startPosition = MinPosition.HasValue && !DisplayAllItems ? MinPosition.Value : Minimum;
            double endPosition = MaxPosition.HasValue && !DisplayAllItems ? MaxPosition.Value : Maximum;

            // go through each marker control and layout on the timeline
            foreach (UIElement childControl in Children)
            {
                DependencyObject child;
                if (childControl is ContentPresenter)
                {
                    ContentPresenter presenter = childControl as ContentPresenter;
                    child = presenter.Content as DependencyObject;
                }
                else
                {
                    child = childControl;
                }

                double childPosition = child != null ? GetPosition(child) : (double)PositionProperty.GetMetadata(typeof(PositionedItemsPanel)).DefaultValue;

                // make sure the child is within the range
                if (childPosition < startPosition || childPosition > endPosition)
                {
                    // don't display the marker
                    childControl.Arrange(new Rect(0, 0, 0, 0));
                }
                else
                {
                    double relativePosition = (childPosition - Minimum) / (Maximum - Minimum);

                    // calculate the top position, center the item vertically
                    double top = (finalSize.Height - childControl.DesiredSize.Height) / 2;

                    // calculate the left position, first get the pixel position
                    double left = relativePosition * finalSize.Width;

                    // next adjust the position so the center of the control
                    // note that the control can overhang the left or right side of the timeline
                    left -= (childControl.DesiredSize.Width / 2);

                    // display the marker
                    childControl.Arrange(new Rect(left, top, childControl.DesiredSize.Width, childControl.DesiredSize.Height));
                }
            }

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Position AttachedProperty definition.
        /// </summary>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached("Position", typeof(double), typeof(PositionedItemsPanel), new PropertyMetadata(0.0));

        /// <summary>
        /// Sets the position on an item.
        /// </summary>
        /// <param name="obj">The object to set the position on.</param>
        /// <param name="propertyValue">The position of the object.</param>
        public static void SetPosition(DependencyObject obj, double propertyValue)
        {
            obj.SetValue(PositionProperty, propertyValue);
        }

        /// <summary>
        /// Gets the position on an item.
        /// </summary>
        /// <param name="obj">The object to retrieve the position from.</param>
        /// <returns></returns>
        public static double GetPosition(DependencyObject obj)
        {
            return (double)obj.GetValue(PositionProperty);
        }
    }
}
