using System;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Microsoft.WebVTT
{
    public sealed class WebVTTLayoutPanel : Panel
    {
        private double lineSize;

        static readonly DependencyProperty paddingPercentProperty;
        static readonly DependencyProperty directionProperty;
        static readonly DependencyProperty orientationProperty;
        static readonly DependencyProperty alignmentProperty;
        static readonly DependencyProperty snapToLinesProperty;
        static readonly DependencyProperty positionProperty;
        static readonly DependencyProperty linePositionProperty;
        static readonly DependencyProperty sizeProperty;

        static WebVTTLayoutPanel()
        {
            paddingPercentProperty = DependencyProperty.Register("PaddingPercent", typeof(Thickness), typeof(WebVTTLayoutPanel), new PropertyMetadata(new Thickness(), PaddingPropertyChanged));
            directionProperty = DependencyProperty.RegisterAttached("Direction", typeof(PanelContentDirection), typeof(WebVTTLayoutPanel), new PropertyMetadata(PanelContentDirection.LeftToRight));
            orientationProperty = DependencyProperty.RegisterAttached("Orientation", typeof(Orientation), typeof(WebVTTLayoutPanel), new PropertyMetadata(Orientation.Horizontal));
            alignmentProperty = DependencyProperty.RegisterAttached("Alignment", typeof(PanelContentAlignment), typeof(WebVTTLayoutPanel), new PropertyMetadata(PanelContentAlignment.Middle));
            snapToLinesProperty = DependencyProperty.RegisterAttached("SnapToLines", typeof(bool), typeof(WebVTTLayoutPanel), new PropertyMetadata(true));
            positionProperty = DependencyProperty.RegisterAttached("Position", typeof(int), typeof(WebVTTLayoutPanel), new PropertyMetadata(1));
            linePositionProperty = DependencyProperty.RegisterAttached("LinePosition", typeof(int?), typeof(WebVTTLayoutPanel), new PropertyMetadata(new int?()));
            sizeProperty = DependencyProperty.RegisterAttached("Size", typeof(int), typeof(WebVTTLayoutPanel), new PropertyMetadata(1));
        }

        public static DependencyProperty PaddingPercentProperty { get { return paddingPercentProperty; } }

        static void PaddingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WebVTTLayoutPanel)d).PaddingChanged((Thickness)e.NewValue);
        }

        void PaddingChanged(Thickness newValue)
        {
            InvalidateMeasure();
            InvalidateArrange();
        }

        public Thickness PaddingPercent
        {
            get { return (Thickness)GetValue(paddingPercentProperty); }
            set { SetValue(paddingPercentProperty, value); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            bool isFirst = true;

            foreach (var child in Children)
            {
                PanelContentDirection direction = GetDirection(child);
                int size = GetSize(child);
                int position = GetPosition(child);
                Orientation orientation = GetOrientation(child);
                bool snapToLines = GetSnapToLines(child);
                //int? linePosition = GetLinePosition(child);
                PanelContentAlignment alignment = GetAlignment(child);

                bool isHorizontal = orientation == Orientation.Horizontal;
                bool ltr = direction == PanelContentDirection.LeftToRight;

                if (isFirst)
                {
                    child.Measure(new Size(double.MaxValue, double.MaxValue));
                    lineSize = isHorizontal ? child.DesiredSize.Height : child.DesiredSize.Width;
                    isFirst = false;
                }

                int maxSize = GetMaxSize(alignment, position, ltr);
                int actualSize = Math.Min(size, maxSize);

                int directionalPosition = GetPosition(ltr, isHorizontal, alignment, actualSize, position);
                //int orthagonalPosition = snapToLines ? 0 : linePosition.GetValueOrDefault(0);

                if (snapToLines)
                {
                    int maxPosition = isHorizontal ? (int)PaddingPercent.Left : (int)PaddingPercent.Top;
                    directionalPosition = Math.Max(directionalPosition, maxPosition);
                    int maxActualSize = 100 - (isHorizontal ? (int)PaddingPercent.Right : (int)PaddingPercent.Bottom) - directionalPosition;
                    actualSize = Math.Min(actualSize, maxActualSize);
                }
                //int x = isHorizontal ? directionalPosition : orthagonalPosition;
                //int y = !isHorizontal ? directionalPosition : orthagonalPosition;

                if (isHorizontal)
                {
                    double width = availableSize.Width * actualSize / 100;
                    child.Measure(new Size(width, double.MaxValue));
                }
                else
                {
                    double height = availableSize.Height * actualSize / 100;
                    child.Measure(new Size(double.MaxValue, height));
                }
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double topEdge = finalSize.Height * PaddingPercent.Top / 100;
            double bottomEdge = finalSize.Height - finalSize.Height * PaddingPercent.Bottom / 100;
            double leftEdge = finalSize.Width * PaddingPercent.Left / 100;
            double rightEdge = finalSize.Width - finalSize.Width * PaddingPercent.Right / 100;

            double stackingHeight = bottomEdge;
            double stackingRtlWidth = rightEdge;
            double stackingLtrWidth = leftEdge;

            foreach (var child in Children)
            {
                PanelContentDirection direction = GetDirection(child);
                int size = GetSize(child);
                int position = GetPosition(child);
                Orientation orientation = GetOrientation(child);
                bool snapToLines = GetSnapToLines(child);
                int? linePosition = GetLinePosition(child);
                PanelContentAlignment alignment = GetAlignment(child);

                bool isHorizontal = orientation == Orientation.Horizontal;
                bool ltr = direction == PanelContentDirection.LeftToRight;

                int maxSize = GetMaxSize(alignment, position, ltr);
                int actualSize = Math.Min(size, maxSize);

                int directionalPosition = GetPosition(ltr, isHorizontal, alignment, actualSize, position);
                int orthagonalPosition = snapToLines ? 0 : linePosition.GetValueOrDefault(0);

                if (snapToLines)
                {
                    int maxPosition = isHorizontal ? (int)PaddingPercent.Left : (int)PaddingPercent.Top;
                    directionalPosition = Math.Max(directionalPosition, maxPosition);
                    int maxActualSize = 100 - (isHorizontal ? (int)PaddingPercent.Right : (int)PaddingPercent.Bottom) - directionalPosition;
                    actualSize = Math.Min(actualSize, maxActualSize);
                }

                int x = isHorizontal ? directionalPosition : orthagonalPosition;
                int y = !isHorizontal ? directionalPosition : orthagonalPosition;

                double left = finalSize.Width * x / 100;
                double top = finalSize.Height * y / 100;
                double width, height;

                if (isHorizontal)
                {
                    width = finalSize.Width * actualSize / 100;
                    height = Math.Min(finalSize.Height, child.DesiredSize.Height);
                }
                else
                {
                    height = finalSize.Height * actualSize / 100;
                    width = Math.Min(finalSize.Width, child.DesiredSize.Width);
                }

                if (snapToLines)
                {
                    if (!linePosition.HasValue)
                    {
                        if (isHorizontal)
                        {
                            top = stackingHeight - height;
                            stackingHeight = top;
                        }
                        else if (ltr)
                        {
                            left = stackingLtrWidth;
                            stackingLtrWidth = left + width;
                        }
                        else
                        {
                            left = stackingRtlWidth - width;
                            stackingRtlWidth = left;
                        }
                    }
                    else if (linePosition.Value >= 0)
                    {
                        if (isHorizontal)
                        {
                            top = lineSize * linePosition.Value + topEdge;
                        }
                        else if (ltr)
                        {
                            left = lineSize * linePosition.Value + leftEdge;
                        }
                        else
                        {
                            left = rightEdge - lineSize * (linePosition.Value + 1);
                        }
                    }
                    else // negative linePosition
                    {
                        if (isHorizontal)
                        {
                            top = bottomEdge + lineSize * linePosition.Value;
                        }
                        else if (ltr)
                        {
                            left = rightEdge - lineSize * (-linePosition.Value + 1);
                        }
                        else
                        {
                            left = lineSize * -linePosition.Value + leftEdge;
                        }
                    }
                }
                child.Arrange(new Rect(left, top, width, height));
            }
            return base.ArrangeOverride(finalSize);
        }

        private static int GetPosition(bool ltr, bool isHorizontal, PanelContentAlignment alignment, int length, int position)
        {
            int result = 0;
            if (isHorizontal && ltr && (alignment == PanelContentAlignment.Start || alignment == PanelContentAlignment.Left))
            {
                result = position;
            }
            else if (isHorizontal && !ltr && (alignment == PanelContentAlignment.End || alignment == PanelContentAlignment.Left))
            {
                result = 100 - position;
            }
            else if (isHorizontal && ltr && (alignment == PanelContentAlignment.End || alignment == PanelContentAlignment.Right))
            {
                result = position - length;
            }
            else if (isHorizontal && !ltr && (alignment == PanelContentAlignment.Start|| alignment == PanelContentAlignment.Right))
            {
                result = (100 - position) - length;
            }
            else if (!isHorizontal && (alignment == PanelContentAlignment.Start || alignment == PanelContentAlignment.Left))
            {
                result = position;
            }
            else if (!isHorizontal && (alignment == PanelContentAlignment.End || alignment == PanelContentAlignment.Right))
            {
                result = position - length;
            }
            else if (isHorizontal && alignment == PanelContentAlignment.Middle && ltr)
            {
                result = position - length / 2;
            }
            else if (isHorizontal && alignment == PanelContentAlignment.Middle && !ltr)
            {
                result = (100 - position) - length / 2;
            }
            else if (!isHorizontal && alignment == PanelContentAlignment.Middle)
            {
                result = position - length / 2;
            }
            return result;
        }

        private static int GetMaxSize(PanelContentAlignment alignment, int position, bool ltr)
        {
            if (alignment == PanelContentAlignment.Middle)
            {
                //If the text track cue alignment is middle, the text track cue text position is less than or equal to 50
                //Let maximum size be the text track cue text position multiplied by two.
                //If the text track cue alignment is middle, the text track cue text position is greater than  50
                //Let maximum size be the result of subtracting text track cue text position from 100 and then multiplying the result by two.
                return position <= 50 ? position * 2 : (100 - position) * 2;
            }
            else if (
                (alignment == PanelContentAlignment.Start && ltr) ||
                (alignment == PanelContentAlignment.End && !ltr) ||
                (alignment == PanelContentAlignment.Left))
            {
                return 100 - position;
            }
            else
            {
                return position;
            }
        }

        #region Direction

        public static DependencyProperty DirectionProperty { get { return directionProperty; } }

        public static void SetDirection(DependencyObject obj, PanelContentDirection value)
        {
            obj.SetValue(DirectionProperty, value);
        }

        public static PanelContentDirection GetDirection(DependencyObject obj)
        {
            return (PanelContentDirection)obj.GetValue(DirectionProperty);
        }
        #endregion

        #region Orientation
        public static DependencyProperty OrientationProperty { get { return orientationProperty; } }

        public static void SetOrientation(DependencyObject obj, Orientation value)
        {
            obj.SetValue(OrientationProperty, value);
        }

        public static Orientation GetOrientation(DependencyObject obj)
        {
            return (Orientation)obj.GetValue(OrientationProperty);
        }
        #endregion

        #region SnapToLines
        public static DependencyProperty SnapToLinesProperty { get { return snapToLinesProperty; } }

        public static void SetSnapToLines(DependencyObject obj, bool value)
        {
            obj.SetValue(SnapToLinesProperty, value);
        }

        public static bool GetSnapToLines(DependencyObject obj)
        {
            return (bool)obj.GetValue(SnapToLinesProperty);
        }
        #endregion

        #region Position
        public static DependencyProperty PositionProperty { get { return positionProperty; } }

        public static void SetPosition(DependencyObject obj, int value)
        {
            obj.SetValue(PositionProperty, value);
        }

        public static int GetPosition(DependencyObject obj)
        {
            return (int)obj.GetValue(PositionProperty);
        }
        #endregion

        #region LinePosition
        public static DependencyProperty LinePositionProperty { get { return linePositionProperty; } }

        public static void SetLinePosition(DependencyObject obj, int? value)
        {
            obj.SetValue(LinePositionProperty, value);
        }

        public static int? GetLinePosition(DependencyObject obj)
        {
            return obj.GetValue(LinePositionProperty) as int?;
        }
        #endregion

        #region Size
        public static DependencyProperty SizeProperty { get { return sizeProperty; } }

        public static void SetSize(DependencyObject obj, int value)
        {
            obj.SetValue(SizeProperty, value);
        }

        public static int GetSize(DependencyObject obj)
        {
            return (int)obj.GetValue(SizeProperty);
        }
        #endregion

        #region Alignment
        public static DependencyProperty AlignmentProperty { get { return alignmentProperty; } }

        public static void SetAlignment(DependencyObject obj, PanelContentAlignment value)
        {
            obj.SetValue(AlignmentProperty, value);
        }

        public static PanelContentAlignment GetAlignment(DependencyObject obj)
        {
            return (PanelContentAlignment)obj.GetValue(AlignmentProperty);
        }
        #endregion
    }

    public enum PanelContentDirection
    {
        LeftToRight,
        RightToLeft
    }

    public enum PanelContentAlignment
    {
        Left,
        Start,
        Middle,
        End,
        Right
    }
}
