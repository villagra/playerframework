using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
#if SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
#endif

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Microsoft.WebVTT
{
    public sealed class WebVTTPanel : Control
    {
        private WebVTTCueRenderer renderer;
        private IList<WebVTTCue> activeCues;
        private Panel captionPanel;

        private Size? currentSize;
        private double currentFontSize;

        private readonly List<Animation> animations = new List<Animation>();
        private readonly IDictionary<WebVTTCue, BoxElement> activeElements = new Dictionary<WebVTTCue, BoxElement>();

        public event EventHandler<NodeRenderingEventArgs> NodeRendering;

        public WebVTTPanel()
        {
            this.DefaultStyleKey = typeof(WebVTTPanel);
            renderer = new WebVTTCueRenderer();
            renderer.OutlineBrush = OutlineBrush;
            renderer.OutlineWidth = OutlineWidth;
            renderer.NodeRendering += renderer_NodeRendering;
            var _activeCues = new ObservableCollection<WebVTTCue>();
            _activeCues.CollectionChanged += _activeCues_CollectionChanged;
            activeCues = _activeCues;
            this.SizeChanged += this_SizeChanged;
        }

        public void Clear()
        {
            activeCues.Clear();
            // this will also cause all animations and elements to get removed
        }

        void _activeCues_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                var oldCues = e.OldItems.Cast<WebVTTCue>().ToList();
                animations.RemoveAll(a => oldCues.Contains(a.Cue));
                foreach (var cue in oldCues)
                {
                    var element = activeElements[cue];
                    activeElements.Remove(cue);
                    captionPanel.Children.Remove(element);
                }
            }
            if (e.NewItems != null)
            {
                var newCues = e.NewItems.Cast<WebVTTCue>().ToList();
                foreach (var cue in newCues)
                {
                    BoxElement element = renderer.GetRenderedCue(cue);
                    if (BoxStyle != null) element.Style = BoxStyle;
                    element.FontSize = currentFontSize;
                    WebVTTLayoutPanel.SetAlignment(element, GetPanelAlignment(cue.Settings.Alignment));
                    WebVTTLayoutPanel.SetDirection(element, GetPanelAlignment(cue.Settings.WritingMode, cue.Content));
                    WebVTTLayoutPanel.SetLinePosition(element, cue.Settings.LinePosition);
                    WebVTTLayoutPanel.SetOrientation(element, GetPanelOrientation(cue.Settings.WritingMode));
                    WebVTTLayoutPanel.SetPosition(element, cue.Settings.TextPosition);
                    WebVTTLayoutPanel.SetSize(element, cue.Settings.Size);
                    WebVTTLayoutPanel.SetSnapToLines(element, cue.Settings.SnapToLines);

                    activeElements.Add(cue, element);
                    captionPanel.Children.Add(element);
                }
            }
        }

        static Orientation GetPanelOrientation(WebVTTWritingMode writingMode)
        {
            switch (writingMode)
            {
                case WebVTTWritingMode.Horizontal:
                    return Orientation.Horizontal;
                default:
                    return Orientation.Vertical;
            }
        }

        static PanelContentDirection GetPanelAlignment(WebVTTWritingMode writingMode, IWebVTTContent content)
        {
            switch (writingMode)
            {
                case WebVTTWritingMode.Horizontal:
                    switch (content.ParagraphDirection)
                    {
                        case WebVTTParagraphDirection.LeftToRight:
                            return PanelContentDirection.LeftToRight;
                        default: //case WebVTTParagraphDirection.RightToLeft:
                            return PanelContentDirection.RightToLeft;
                    }
                case WebVTTWritingMode.VerticalGrowingLeft:
                    return PanelContentDirection.RightToLeft;
                default: //case WebVTTWritingMode.VerticalGrowingRight:
                    return PanelContentDirection.LeftToRight;
            }
        }

        static PanelContentAlignment GetPanelAlignment(WebVTTAlignment alignment)
        {
            switch (alignment)
            {
                case WebVTTAlignment.End:
                    return PanelContentAlignment.End;
                case WebVTTAlignment.Left:
                    return PanelContentAlignment.Left;
                case WebVTTAlignment.Middle:
                    return PanelContentAlignment.Middle;
                case WebVTTAlignment.Right:
                    return PanelContentAlignment.Right;
                default: // case WebVTTAlignment.Start:
                    return PanelContentAlignment.Start;
            }
        }

        public void UpdatePosition(TimeSpan time)
        {
            foreach (var animation in animations)
            {
                animation.IsEnabled = (animation.TimeStamp <= time);
            }
        }

        void renderer_NodeRendering(object sender, NodeRenderingEventArgs e)
        {
            if (NodeRendering != null) NodeRendering(this, e);

            var timestampNode = e.Node as WebVTTTimestampNode;
            if (timestampNode != null)
            {
                var animation = new Animation(e.Cue, timestampNode.Timestamp);
                var previousForeground = e.Inline.Foreground;
                animation.OnEnable = () => e.Inline.Foreground = previousForeground;
                animation.OnDisable = () => e.Inline.Foreground = new SolidColorBrush(Colors.Transparent);
                animation.OnDisable(); // force the state to be consistent with animation.IsEnabled
                animations.Add(animation);
            }
        }

        public IList<WebVTTCue> ActiveCues
        {
            get { return activeCues; }
            set { activeCues = value; }
        }

        void this_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            currentSize = e.NewSize;
            UpdateFontSize(e.NewSize, FontSizePercent);
        }

        private void UpdateFontSize(Size size, double fontSizePercent)
        {
            currentFontSize = size.Height * fontSizePercent / 100;
            foreach (var box in activeElements)
            {
                box.Value.FontSize = currentFontSize;
            }
            if (captionPanel != null)
            {
                captionPanel.InvalidateMeasure();
                captionPanel.InvalidateArrange();
            }
        }

#if SILVERLIGHT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            captionPanel = GetTemplateChild("CaptionsPanel") as Panel;
        }

        static readonly DependencyProperty boxStyleProperty = DependencyProperty.Register("BoxStyle", typeof(Style), typeof(WebVTTPanel), new PropertyMetadata(null, (d, e) => ((WebVTTPanel)d).OnBoxStyleChanged(e.NewValue as Style)));
        public static DependencyProperty BoxStyleProperty { get { return boxStyleProperty; } }

        public Style BoxStyle
        {
            get { return GetValue(boxStyleProperty) as Style; }
            set { SetValue(boxStyleProperty, value); }
        }

        void OnBoxStyleChanged(Style newValue)
        {
            foreach (var element in activeElements)
            {
                element.Value.Style = newValue;
            }
        }

        static readonly DependencyProperty outlineWidthProperty = DependencyProperty.Register("OutlineWidth", typeof(double), typeof(WebVTTPanel), new PropertyMetadata(1.0, (d, e) => ((WebVTTPanel)d).OnOutlineWidthChanged((double)e.NewValue)));
        public static DependencyProperty OutlineWidthProperty { get { return outlineWidthProperty; } }

        public double OutlineWidth
        {
            get { return (double)GetValue(outlineWidthProperty); }
            set { SetValue(outlineWidthProperty, value); }
        }

        void OnOutlineWidthChanged(double newValue)
        {
            renderer.OutlineWidth = newValue;
        }

        static readonly DependencyProperty outlineBrushProperty = DependencyProperty.Register("OutlineBrush", typeof(Brush), typeof(WebVTTPanel), new PropertyMetadata(new SolidColorBrush(Colors.Black), (d, e) => ((WebVTTPanel)d).OnOutlineBrushChanged(e.NewValue as Brush)));
        public static DependencyProperty OutlineBrushProperty { get { return outlineBrushProperty; } }

        public Brush OutlineBrush
        {
            get { return GetValue(outlineBrushProperty) as Brush; }
            set { SetValue(outlineBrushProperty, value); }
        }

        void OnOutlineBrushChanged(Brush newValue)
        {
            renderer.OutlineBrush = newValue;
        }

        static readonly DependencyProperty innerBrushProperty = DependencyProperty.Register("InnerBrush", typeof(Brush), typeof(WebVTTPanel), new PropertyMetadata(new SolidColorBrush(Colors.White), (d, e) => ((WebVTTPanel)d).OnInnerBrushChanged(e.NewValue as Brush)));
        public static DependencyProperty InnerBrushProperty { get { return innerBrushProperty; } }

        public Brush InnerBrush
        {
            get { return GetValue(innerBrushProperty) as Brush; }
            set { SetValue(innerBrushProperty, value); }
        }

        void OnInnerBrushChanged(Brush newValue)
        {
            renderer.InnerBrush = newValue;
        }

        static readonly DependencyProperty paddingPercentProperty = DependencyProperty.Register("PaddingPercent", typeof(Thickness), typeof(WebVTTPanel), new PropertyMetadata(new Thickness(0, 0, 0, 10)));
        public static DependencyProperty PaddingPercentProperty { get { return paddingPercentProperty; } }

        public Thickness PaddingPercent
        {
            get { return (Thickness)GetValue(paddingPercentProperty); }
            set { SetValue(paddingPercentProperty, value); }
        }

        static readonly DependencyProperty fontSizePercentProperty = DependencyProperty.Register("FontSizePercent", typeof(double), typeof(WebVTTPanel), new PropertyMetadata(5.0, (d, e) => ((WebVTTPanel)d).OnFontSizePercentChanged((double)e.NewValue)));
        public static DependencyProperty FontSizePercentProperty { get { return fontSizePercentProperty; } }

        void OnFontSizePercentChanged(double newValue)
        {
            if (currentSize.HasValue)
            {
                UpdateFontSize(currentSize.Value, newValue);
            }
        }

        public double FontSizePercent
        {
            get { return (double)GetValue(fontSizePercentProperty); }
            set { SetValue(fontSizePercentProperty, value); }
        }

        private class Animation
        {
            public Animation(WebVTTCue cue, TimeSpan timeStamp)
            {
                Cue = cue;
                TimeStamp = timeStamp;
            }

            public WebVTTCue Cue { get; private set; }
            public TimeSpan TimeStamp { get; private set; }
            internal Action OnEnable { get; set; }
            internal Action OnDisable { get; set; }

            bool isEnabled;
            public bool IsEnabled
            {
                get { return isEnabled; }
                set
                {
                    if (isEnabled != value)
                    {
                        isEnabled = value;
                        if (isEnabled) OnEnable();
                        else OnDisable();
                    }
                }
            }
        }
    }
}
