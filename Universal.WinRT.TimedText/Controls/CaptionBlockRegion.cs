using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
#else
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace Microsoft.Media.TimedText
{
    public sealed class CaptionBlockRegion : Control
    {
        /// <summary>
        /// the minimum text rows to display
        /// </summary>
        private const uint MinimumTextRows = 2;

        /// <summary>
        /// The padding between lines as a percentage of the font size
        /// </summary>
        private const double LinePadding = 0.5;

        /// <summary>
        /// Padding to add to regions to insure that they don't get cut off
        /// </summary>
        private const double RegionPadding = 50.0;

        private TimeSpan _mediaPosition;
        private IMarkerManager<TimedTextElement> _captionManager;
        private IDictionary<CaptionElement, UIElement> _activeElements;

        private Grid LayoutRoot;
        private Border CaptionsBorder;
        private Grid CaptionsRoot;
        private StackPanel CaptionsPanel;

        private bool isTemplateApplied;

#if SILVERLIGHT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            LayoutRoot = base.GetTemplateChild("LayoutRoot") as Grid;
            CaptionsBorder = base.GetTemplateChild("CaptionsBorder") as Border;
            CaptionsRoot = base.GetTemplateChild("CaptionsRoot") as Grid;
            CaptionsPanel = base.GetTemplateChild("captionsPanel") as StackPanel;

            isTemplateApplied = true;

            UpdateSize();
        }

        #region CaptionRegion
        public static readonly DependencyProperty CaptionRegionProperty = DependencyProperty.Register("CaptionRegion", typeof(CaptionRegion), typeof(CaptionBlockRegion), new PropertyMetadata(null, OnCaptionRegionPropertyChanged));

        public CaptionRegion CaptionRegion
        {
            get { return (CaptionRegion)GetValue(CaptionRegionProperty); }
            set { SetValue(CaptionRegionProperty, value); }
        }

        private static void OnCaptionRegionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var captionBlockRegion = d as CaptionBlockRegion;
            captionBlockRegion.IfNotNull(i => i.OnCaptionRegionChanged());
        }

        private void OnCaptionRegionChanged()
        {
            ApplyRegionStyles();
        }
        #endregion

        const int MaxOverflow = 1080;

        public CaptionBlockRegion()
        {
            this.DefaultStyleKey = typeof(CaptionBlockRegion);

            _activeElements = new Dictionary<CaptionElement, UIElement>();

            this.Loaded += (s, e) => RedrawActiveCaptions();
            this.SizeChanged += (s, e) => UpdateSize();
        }

        public IMarkerManager<TimedTextElement> CaptionManager
        {
            get { return _captionManager; }
            set
            {
                if (_captionManager != null)
                {
                    _captionManager.MarkerLeft -= _captionManager_MarkerLeft;
                    _captionManager.MarkerReached -= _captionManager_MarkerReached;
                }
                _captionManager = value;
                if (_captionManager != null)
                {
                    _captionManager.MarkerLeft += _captionManager_MarkerLeft;
                    _captionManager.MarkerReached += _captionManager_MarkerReached;
                }
            }
        }

        void _captionManager_MarkerLeft(IMarkerManager<TimedTextElement> sender, TimedTextElement marker)
        {
            _mediaPosition = marker.End;
            HideCaption(marker);
        }

        void _captionManager_MarkerReached(IMarkerManager<TimedTextElement> sender, TimedTextElement marker)
        {
            _mediaPosition = marker.Begin;
            ShowCaption(marker);
        }

        public void UpdateAnimations(TimeSpan mediaPosition)
        {
            _mediaPosition = mediaPosition;

#if HACK_XAMLTYPEINFO
            var children = CaptionRegion.Children as MediaMarkerCollection<TimedTextElement>;
#else
            var children = CaptionRegion.Children;
#endif
            children.WhereActiveAtPosition(mediaPosition)
                                  .Where(i => i.HasAnimations)
                                  .ForEach(HideCaption)
                                  .ForEach(ShowCaption);
        }

        private void ShowCaption(TimedTextElement timedTextElement)
        {
            var caption = timedTextElement as CaptionElement;
            if (caption != null)
            {
                caption.CalculateCurrentStyle(_mediaPosition);
                var uiElement = RenderElement(caption);
                if (uiElement != null)
                {
                    if (_activeElements.ContainsKey(caption))
                    {
                        HideCaption(timedTextElement);
                    }
                    _activeElements.Add(caption, uiElement);
                    CaptionsPanel.Children.Clear();
                    _activeElements.OrderBy(i => i.Key.Index)
                                   .ForEach(i => CaptionsPanel.Children.Add(i.Value));
                }
            }
        }

        private void HideCaption(TimedTextElement timedTextElement)
        {
            var caption = timedTextElement as CaptionElement;

            if (caption != null && _activeElements.ContainsKey(caption))
            {
                CaptionsPanel.Children.Remove(_activeElements[caption]);
                _activeElements.Remove(caption);
            }
        }

        private void RedrawActiveCaptions()
        {
            var activeCaptions = _activeElements.Keys.ToList();
            activeCaptions.ForEach(HideCaption);
            activeCaptions.ForEach(ShowCaption);
        }

        /// <summary>
        /// Update the size of the caption region and caption border based on 
        /// the size declared in the TTML and the size of the text.
        /// </summary>
        private void UpdateSize()
        {
            if (isTemplateApplied)
            {
                var width = this.GetEffectiveWidth();
                var height = this.GetEffectiveHeight();

                if (width != 0 && height != 0 && CaptionRegion != null)
                {
                    Origin origin = CaptionRegion.CurrentStyle.Origin;
                    Extent extent = CaptionRegion.CurrentStyle.Extent;

                    var extentHeight = extent.Height.ToPixelLength(height);
                    double pixelFontSize = CaptionRegion.CurrentStyle.FontSize.ToPixelLength(extentHeight);

                    var pixelPadding = pixelFontSize * LinePadding; 
                    double minimumCaptionRegionHeight = pixelFontSize * MinimumTextRows + (pixelPadding * (MinimumTextRows + 1));

                    double regionHeight = extent.Height.ToPixelLength(height);
                    
                    regionHeight = RegionPadding + Math.Max(regionHeight, minimumCaptionRegionHeight);

                    double regionWidth = extent.Width.ToPixelLength(width);

                    CaptionsBorder.Width = regionWidth < 0 ? width : regionWidth;
                    CaptionsBorder.Height = regionHeight < 0 ? height : regionHeight;
                    CaptionsBorder.VerticalAlignment = regionHeight < 0
                        ? VerticalAlignment.Bottom
                        : VerticalAlignment.Top;

                    CaptionsBorder.Margin = new Thickness
                    {
                        Left = origin.Left.ToPixelLength(width),
                        Top =  Math.Min(origin.Top.ToPixelLength(height), height - regionHeight)
                    };

                    ApplyRegionStyles();
                    RedrawActiveCaptions();
                }
            }
        }

        const int BRUSH_CACHE_CAPACITY = 50;

        // captions will usually use a handful of colors, we'll cache them to reduce resource usage.
        static Dictionary<Color, CachedBrush> cachedBrushes = new Dictionary<Color, CachedBrush>();

        class CachedBrush
        {
            public CachedBrush(Brush Brush)
            {
                this.Brush = Brush;
                LastUse = DateTime.Now;
            }

            public Brush Brush { get; private set; }
            public DateTime LastUse { get; set; }
        }

        static Brush GetCachedBrush(Color src)
        {
            if (cachedBrushes.ContainsKey(src))
            {
                var result = cachedBrushes[src];
                result.LastUse = DateTime.Now;
                return result.Brush;
            }
            else
            {
                Brush brush = new SolidColorBrush(src);
                if (cachedBrushes.Count >= BRUSH_CACHE_CAPACITY)
                {
                    var oldestUsedBrush = cachedBrushes.OrderBy(b => b.Value.LastUse).First();
                    cachedBrushes.Remove(oldestUsedBrush.Key);
                }
                cachedBrushes.Add(src, new CachedBrush(brush));
                return brush;
            }
        }

        private void ApplyRegionStyles()
        {
            if (isTemplateApplied)
            {
                var effectiveSize = this.GetEffectiveSize();
                var fontSize = CaptionRegion.CurrentStyle.FontSize.ToPixelLength(effectiveSize.Height);

                Canvas.SetZIndex(this, CaptionRegion.CurrentStyle.ZIndex);
                FontSize = fontSize > 0 ? fontSize : FontSize;
#if SILVERLIGHT
                FontFamily = CaptionRegion.CurrentStyle.FontFamily;
#else
                FontFamily = CaptionRegion.CurrentStyle.FontFamily.WindowsFontFamily;
#endif
                Foreground = GetCachedBrush(CaptionRegion.CurrentStyle.Color);
                CaptionsBorder.Background = GetCachedBrush(CaptionRegion.CurrentStyle.BackgroundColor);
                CaptionsBorder.Padding = CaptionRegion.CurrentStyle.Padding.ToThickness(effectiveSize);
                LayoutRoot.Visibility = CaptionRegion.CurrentStyle.Display == Visibility.Collapsed
                                        ? Visibility.Collapsed
                                        : CaptionRegion.CurrentStyle.Visibility;

                switch (CaptionRegion.CurrentStyle.DisplayAlign)
                {
                    case DisplayAlign.Center:
                        CaptionsPanel.VerticalAlignment = VerticalAlignment.Center;
                        break;
                    case DisplayAlign.Before:
                        CaptionsPanel.VerticalAlignment = VerticalAlignment.Top;
                        break;
                    case DisplayAlign.After:
                        CaptionsPanel.VerticalAlignment = VerticalAlignment.Bottom;
                        break;
                }

                if (CaptionRegion.CurrentStyle.Overflow == Overflow.Visible)
                {
                    // we're going to do this with margins since no default Silverlight panels support overflow and alignment. Could also be done with a custom panel.
                    switch (CaptionRegion.CurrentStyle.DisplayAlign)
                    {
                        case DisplayAlign.Center:
                            CaptionsPanel.Margin = new Thickness(0, -MaxOverflow, 0, -MaxOverflow);
                            break;
                        case DisplayAlign.Before:
                            CaptionsPanel.Margin = new Thickness(0, 0, 0, -MaxOverflow);
                            break;
                        case DisplayAlign.After:
                            CaptionsPanel.Margin = new Thickness(0, -MaxOverflow, 0, 0);
                            break;
                    }
                    switch (CaptionRegion.CurrentStyle.TextAlign)
                    {
                        case TextAlignment.Center:
                            CaptionsPanel.Margin = new Thickness(-MaxOverflow, CaptionsPanel.Margin.Top, -MaxOverflow, CaptionsPanel.Margin.Bottom);
                            break;
                        case TextAlignment.Right:
                            CaptionsPanel.Margin = new Thickness(-MaxOverflow, CaptionsPanel.Margin.Top, 0, CaptionsPanel.Margin.Bottom);
                            break;
                        default:
                            CaptionsPanel.Margin = new Thickness(0, CaptionsPanel.Margin.Top, -MaxOverflow, CaptionsPanel.Margin.Bottom);
                            break;
                    }
                }
                else
                {
                    CaptionsPanel.Margin = new Thickness();
                }
            }
        }

        private UIElement RenderElement(CaptionElement element)
        {
            Panel parent = null;
            UIElement result = null;

            try
            {
                parent = new StackPanel()
                {
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                var bgImage = element.CurrentStyle.BackgroundImage;
                if (!string.IsNullOrEmpty(bgImage))
                {
                    var border = new Border() { Background = GetCachedBrush(element.CurrentStyle.BackgroundColor) };
                    border.Child = parent;
                    result = border;
                    SetBackgroundImageAsync(bgImage, parent, element.CurrentStyle.BackgroundImageHorizontal, element.CurrentStyle.BackgroundImageVertical);
                }
                else
                {
                    result = parent;
                    parent.Background = GetCachedBrush(element.CurrentStyle.BackgroundColor);
                }

                if (element.CurrentStyle.IsOriginSpecified || element.CurrentStyle.IsExtentSpecified)
                {
                    // set position and size (origin and extent)
                    var width = this.GetEffectiveWidth();
                    var height = this.GetEffectiveHeight();
                    if (element.CurrentStyle.IsOriginSpecified)
                    {
                        var origin = element.CurrentStyle.Origin;
                        parent.Margin = new Thickness
                        {
                            Left = origin.Left.ToPixelLength(width),
                            Top = origin.Top.ToPixelLength(height)
                        };
                    }
                    if (element.CurrentStyle.IsExtentSpecified)
                    {
                        var extent = CaptionRegion.CurrentStyle.Extent;
                        var extentWidth = extent.Width.ToPixelLength(width);
                        var extentHeight = extent.Height.ToPixelLength(height);
                        if (extentWidth > 0) parent.Width = extentWidth;
                        if (extentHeight > 0) parent.Height = extentHeight;
                    }
                }

                var textAlignment = element.CurrentStyle.TextAlign;

                double offset = 0;
                Panel p = NewPanel(parent, ref offset, element, textAlignment);

                RenderElementRecurse(parent, ref p, element, ref offset, textAlignment);

#if !WINDOWS_PHONE && !SILVERLIGHT3 && NOTUSED
                foreach (StackPanel stack in parent.Children)
                {
                    double baseline = 0;
                    foreach (Border b in stack.Children)
                    {
                        TextBlock tb = b.Child as TextBlock;
                        if (tb != null && tb.BaselineOffset > baseline)
                        {
                            baseline = tb.ActualHeight - tb.BaselineOffset;
                        }
                    }
                    foreach (Border b in stack.Children)
                    {
                        TextBlock tb = b.Child as TextBlock;
                        if (tb != null)
                        {
                            tb.Margin = new Thickness(0, 0, 0, baseline - (tb.ActualHeight - tb.BaselineOffset));
                        }
                    }
                }
#endif
            }
            catch (Exception)
            {
                //TODO: Respond to errors
            }

            return result;
        }

        async void SetBackgroundImageAsync(string imageId, Panel control, PositionLength horizontal, PositionLength vertical)
        {
            byte[] imgBytes = null;
            if (imageId.StartsWith("#"))
            {
                var img = CaptionRegion.TunneledData[imageId.Substring(1)];
                if (img != null && img.Encoding == "Base64")
                {
                    imgBytes = System.Convert.FromBase64String(img.Data);
                }
            }
            else
            {
                using (var httpClient = new HttpClient())
                {
                    imgBytes = await httpClient.GetByteArrayAsync(imageId);
                }
            }
            if (imgBytes != null)
            {
                await SetBackgroundImageBytesAsync(imgBytes, control, horizontal, vertical);
            }
        }

        static async Task SetBackgroundImageBytesAsync(byte[] imageBytes, Panel control, PositionLength horizontal, PositionLength vertical)
        {
            var bmpImage = await ByteArrayToImageAsync(imageBytes);
            var imageBrush = new ImageBrush() { ImageSource = bmpImage, Stretch = Stretch.None };
            if (horizontal != null)
            {
                switch (horizontal.Unit)
                {
                    case PositionLengthUnit.CenterAlign:
                        imageBrush.AlignmentX = AlignmentX.Center;
                        break;
                    case PositionLengthUnit.NearAlign:
                        imageBrush.AlignmentX = AlignmentX.Left;
                        break;
                    case PositionLengthUnit.FarAlign:
                        imageBrush.AlignmentX = AlignmentX.Right;
                        break;
                    case PositionLengthUnit.Absolute:
                        imageBrush.AlignmentX = AlignmentX.Left;
                        imageBrush.Transform = new TranslateTransform() { X = horizontal.Value };
                        break;
                    case PositionLengthUnit.Percentage:
                        imageBrush.AlignmentX = AlignmentX.Left;
                        imageBrush.RelativeTransform = new TranslateTransform() { X = horizontal.Value };
                        break;
                }
            }
            if (vertical != null)
            {
                switch (vertical.Unit)
                {
                    case PositionLengthUnit.CenterAlign:
                        imageBrush.AlignmentY = AlignmentY.Center;
                        break;
                    case PositionLengthUnit.NearAlign:
                        imageBrush.AlignmentY = AlignmentY.Top;
                        break;
                    case PositionLengthUnit.FarAlign:
                        imageBrush.AlignmentY = AlignmentY.Bottom;
                        break;
                    case PositionLengthUnit.Absolute:
                        imageBrush.AlignmentY = AlignmentY.Top;
                        var transform = imageBrush.Transform as TranslateTransform;
                        if (transform == null)
                        {
                            transform = new TranslateTransform();
                            imageBrush.Transform = transform;
                        }
                        transform.Y = vertical.Value;
                        break;
                    case PositionLengthUnit.Percentage:
                        imageBrush.AlignmentY = AlignmentY.Top;
                        var relativeTransform = imageBrush.RelativeTransform as TranslateTransform;
                        if (relativeTransform == null)
                        {
                            relativeTransform = new TranslateTransform();
                            imageBrush.RelativeTransform = relativeTransform;
                        }
                        relativeTransform.Y = vertical.Value;
                        break;
                }
            }
            control.Background = imageBrush;
        }

#if SILVERLIGHT
        static async Task<BitmapImage> ByteArrayToImageAsync(byte[] imageBytes)
        {
            using (var stream = new MemoryStream())
            {
                BitmapImage image = new BitmapImage();
                await stream.WriteAsync(imageBytes, 0, imageBytes.Length);
                await stream.FlushAsync();
                stream.Seek(0, SeekOrigin.Begin);
                image.SetSource(stream);
                return image;
            }
        }
#else
        static async Task<BitmapImage> ByteArrayToImageAsync(byte[] imageBytes)
        {
            using (var stream = new InMemoryRandomAccessStream())
            {
                BitmapImage image = new BitmapImage();
                await stream.WriteAsync(imageBytes.AsBuffer());
                stream.Seek(0);
                image.SetSource(stream);
                return image;
            }
        }
#endif

        private void RenderElementRecurse(Panel parent, ref Panel p, CaptionElement element, ref double offset, TextAlignment align)
        {
            if (element.IsActiveAtPosition(_mediaPosition) && element.CurrentStyle.Display == Visibility.Visible)
            {
                if (element.CaptionElementType == TimedTextElementType.Text)
                {
                    var text = element.Content != null ? element.Content.ToString() : string.Empty;
                    offset = WrapElement(parent, ref p, offset, text, element, align);
                }
                else if (element.CaptionElementType == TimedTextElementType.Container)
                {
#if HACK_XAMLTYPEINFO
                    var children = element.Children as MediaMarkerCollection<TimedTextElement>;
#else
                    var children = element.Children;
#endif

                    foreach (CaptionElement child in children)
                    {
                        RenderElementRecurse(parent, ref p, child, ref offset, align);
                    }
                }
                else if (element.CaptionElementType == TimedTextElementType.LineBreak)
                {
                    p = NewPanel(parent, ref offset, element, align);
                }
            }
        }

        private double WrapElement(Panel parent, ref Panel p, double offset, string text, CaptionElement element, TextAlignment align, bool directionApplied = false)
        {
            if (text == null || text == "") return offset;

            var effectiveSize = this.GetEffectiveSize();
            var style = element.CurrentStyle;
            var panelSize = style.Extent.ToPixelSize(effectiveSize);
            double panelWidth = panelSize.Width;
            double panelHeight = panelSize.Height;

            if (style.Direction == FlowDirection.RightToLeft && !directionApplied)
            {
                text = new string(text.ToCharArray().Reverse().ToArray());
            }

            double height = style.FontSize.Unit == LengthUnit.PixelProportional || style.FontSize.Unit == LengthUnit.Cell ? effectiveSize.Height : panelHeight;
            TextBlock textblock = GetStyledTextblock(style, panelWidth, height, false);
            SetContent(textblock, text);

            Border border = new Border();
            border.Background = GetCachedBrush(style.BackgroundColor);
            FrameworkElement contentElement;

            double outlineWidth = style.OutlineWidth.ToPixelLength(effectiveSize.Height);

            if (outlineWidth > 0)
            {
                switch (style.TextStyle)
                {
                    case TextStyle.Default:
                        contentElement = AddOutlineTextStyle(text, style, panelWidth, height, textblock, outlineWidth);
                        break;

                    case TextStyle.DepressedEdge:
                        contentElement = AddDepressedEdgeTextStyle(text, style, panelWidth, height, textblock, outlineWidth);
                        break;

                    case TextStyle.DropShadow:
                        contentElement = AddDropShadowTextStyle(text, style, panelWidth, height, textblock, outlineWidth);
                        break;

                    case TextStyle.None:
                        contentElement = textblock;
                        break;

                    case TextStyle.Outline:
                        contentElement = AddOutlineTextStyle(text, style, panelWidth, height, textblock, outlineWidth);
                        break;

                    case TextStyle.RaisedEdge:
                        contentElement = AddRaisedEdgeTextStyle(text, style, panelWidth, height, textblock, outlineWidth);
                        break;

                    default:
                        contentElement = AddOutlineTextStyle(text, style, panelWidth, height, textblock, outlineWidth);
                        break;
                }

            }
            else
            {
                contentElement = textblock;
            }

            contentElement.Opacity = System.Convert.ToDouble(style.Color.A) / 255.0;
            border.Opacity = System.Convert.ToDouble(style.Color.A) / 255.0;
            border.Child = contentElement;
            p.Children.Add(border);

            string head = text;
            string tail = string.Empty;
            double elementWidth = textblock.GetEffectiveWidth();
            if (offset + elementWidth > panelSize.Width && style.WrapOption == TextWrapping.Wrap)
            {
                if (text.Length > 0 && text.IndexOf(' ') < 0)
                {
                    if (offset != 0 && elementWidth < panelSize.Width)
                    {
                        p.Children.Remove(border);
                        p = NewPanel(parent, ref offset, element, align);
                        return WrapElement(parent, ref p, 0, text, element, align, true);
                    }
                    int idx = text.Length - 1;
                    head = text.Substring(0, idx);
                    tail = text.Substring(idx);
                    SetAllContent(contentElement, head);
                    while (offset + textblock.GetEffectiveWidth() > panelSize.Width)
                    {
                        idx--;
                        head = text.Substring(0, idx);
                        tail = text.Substring(idx);
                        SetAllContent(contentElement, head);
                        p.UpdateLayout();
                    }
                    p = NewPanel(parent, ref offset, element, align);
                    return WrapElement(parent, ref p, offset, tail, element, align, true);
                }
                while (offset + textblock.GetEffectiveWidth() > panelSize.Width)
                {
                    int idx = head.LastIndexOf(' ');
                    if (idx < 0)
                    {
                        SetAllContent(contentElement, text);
                        return 0;
                    }
                    else
                    {
                        tail = text.Substring(idx + 1);
                        head = text.Substring(0, idx);
                    }
                    SetAllContent(contentElement, head);
                }
                p = NewPanel(parent, ref offset, element, align);
                return WrapElement(parent, ref p, offset, tail, element, align, true);
            }
            else
            {
                offset += elementWidth;
                return offset;
            }
        }

        /// <summary>
        /// Add raised edge text blocks to the lower left in front of the original textblock
        /// </summary>
        /// <param name="text">the text</param>
        /// <param name="style">the style</param>
        /// <param name="panelWidth">the panel width</param>
        /// <param name="height">the height</param>
        /// <param name="textblock">the original text block</param>
        /// <param name="outlineWidth">the outline width</param>
        /// <returns>a new Grid</returns>
        private FrameworkElement AddRaisedEdgeTextStyle(string text, TimedTextStyle style, double panelWidth, double height, TextBlock textblock, double outlineWidth)
        {
            var textBlocks = new TextBlock[]
            {
                GetStyledTextblock(style, panelWidth, height, true),
                GetStyledTextblock(style, panelWidth, height, true),
                GetStyledTextblock(style, panelWidth, height, true)
            };

            // trailing edge
            // right
            textBlocks[0].RenderTransform = new TranslateTransform() { X = outlineWidth, Y = 0 };
            // bottom
            textBlocks[1].RenderTransform = new TranslateTransform() { X = 0, Y = outlineWidth };
            // bottom right
            textBlocks[2].RenderTransform = new TranslateTransform() { X = outlineWidth, Y = outlineWidth };

            var grid = new Grid();

            // The cache mode composites all of the elements together so the 
            // opacity is applied to the whole grid instead of the individual 
            // elements.
            grid.CacheMode = new BitmapCache();


            foreach (var item in textBlocks)
            {
                SetContent(item, text);
                grid.Children.Add(item);
            }

            grid.Children.Add(textblock);

            return grid;
        }

        /// <summary>
        /// Add drop shadow text blocks to the lower right
        /// </summary>
        /// <param name="text">the text</param>
        /// <param name="style">the style</param>
        /// <param name="panelWidth">the panel width</param>
        /// <param name="height">the height</param>
        /// <param name="textblock">the original text block</param>
        /// <param name="outlineWidth">the outline width</param>
        /// <returns>a new Grid</returns>
        private FrameworkElement AddDropShadowTextStyle(string text, TimedTextStyle style, double panelWidth, double height, TextBlock textblock, double outlineWidth)
        {
            var textBlocks = new TextBlock[]
            {
                GetStyledTextblock(style, panelWidth, height, true),
                GetStyledTextblock(style, panelWidth, height, true),
                GetStyledTextblock(style, panelWidth, height, true)
            };

            // right
            textBlocks[0].RenderTransform = new TranslateTransform() { X = outlineWidth, Y = 0 };
            // bottom
            textBlocks[1].RenderTransform = new TranslateTransform() { X = 0, Y = outlineWidth};
            // bottom right
            textBlocks[2].RenderTransform = new TranslateTransform() { X = outlineWidth, Y = outlineWidth};

            var grid = new Grid();

            // The cache mode composites all of the elements together so the 
            // opacity is applied to the whole grid instead of the individual 
            // elements.
            grid.CacheMode = new BitmapCache();

            foreach (var item in textBlocks)
            {
                item.Opacity = 0.5;
                SetContent(item, text);
                grid.Children.Add(item);
            }

            grid.Children.Add(textblock);

            return grid;
        }

        /// <summary>
        /// Depressed edge text has a dark leading edge
        /// </summary>
        /// <param name="text">the text</param>
        /// <param name="style">the timed text style</param>
        /// <param name="panelWidth">the panel width</param>
        /// <param name="height">the height</param>
        /// <param name="textblock">the original TextBlock</param>
        /// <param name="outlineWidth">the outline width</param>
        /// <returns>a new Grid</returns>
        private FrameworkElement AddDepressedEdgeTextStyle(string text, TimedTextStyle style, double panelWidth, double height, TextBlock textblock, double outlineWidth)
        {
            var textBlocks = new TextBlock[]
            {
                GetStyledTextblock(style, panelWidth, height, true),
                GetStyledTextblock(style, panelWidth, height, true),
                GetStyledTextblock(style, panelWidth, height, true)
            };

            
            // top left
            textBlocks[0].RenderTransform = new TranslateTransform() { X = -outlineWidth, Y = -outlineWidth };
            // top
            textBlocks[1].RenderTransform = new TranslateTransform() { X = 0, Y = -outlineWidth };
            // left
            textBlocks[2].RenderTransform = new TranslateTransform() { X = -outlineWidth, Y = 0};

            var grid = new Grid();

            // The cache mode composites all of the elements together so the 
            // opacity is applied to the whole grid instead of the individual 
            // elements.
            grid.CacheMode = new BitmapCache();


            foreach (var item in textBlocks)
            {
                SetContent(item, text);
                grid.Children.Add(item);
            }

            grid.Children.Add(textblock);

            return grid;
        }

        /// <summary>
        /// Add outlined text style
        /// </summary>
        /// <param name="text">the text</param>
        /// <param name="style">the timed text style</param>
        /// <param name="panelWidth">the panel width</param>
        /// <param name="height">the height</param>
        /// <param name="textblock">the base text block</param>
        /// <param name="outlineWidth">the outline width</param>
        /// <returns>a Grid</returns>
        private FrameworkElement AddOutlineTextStyle(string text, TimedTextStyle style, double panelWidth, double height, TextBlock textblock, double outlineWidth)
        {
            FrameworkElement contentElement;
            Grid cnv = new Grid();

            // The cache mode composites all of the elements together so the 
            // opacity is applied to the whole grid instead of the individual 
            // elements.
            cnv.CacheMode = new BitmapCache();
            
            // do outline image up and to left
            TextBlock tb2 = GetStyledTextblock(style, panelWidth, height, true);
            SetContent(tb2, text);
            cnv.Children.Add(tb2);
            tb2.RenderTransform = new TranslateTransform() { X = -outlineWidth, Y = -outlineWidth };
            // do outline image left
            tb2 = GetStyledTextblock(style, panelWidth, height, true);
            SetContent(tb2, text);
            cnv.Children.Add(tb2);
            tb2.RenderTransform = new TranslateTransform() { X = -outlineWidth, Y = 0 };

            // do outline image down and to right
            tb2 = GetStyledTextblock(style, panelWidth, height, true);
            SetContent(tb2, text);
            cnv.Children.Add(tb2);
            tb2.RenderTransform = new TranslateTransform() { X = outlineWidth, Y = outlineWidth };
            // do outline image to right
            tb2 = GetStyledTextblock(style, panelWidth, height, true);
            SetContent(tb2, text);
            cnv.Children.Add(tb2);
            tb2.RenderTransform = new TranslateTransform() { X = outlineWidth, Y = 0 };

            // do outline image up and to right
            tb2 = GetStyledTextblock(style, panelWidth, height, true);
            SetContent(tb2, text);
            cnv.Children.Add(tb2);
            tb2.RenderTransform = new TranslateTransform() { X = outlineWidth, Y = -outlineWidth };
            // do outline image up
            tb2 = GetStyledTextblock(style, panelWidth, height, true);
            SetContent(tb2, text);
            cnv.Children.Add(tb2);
            tb2.RenderTransform = new TranslateTransform() { X = 0, Y = -outlineWidth };

            // do outline image down and to left
            tb2 = GetStyledTextblock(style, panelWidth, height, true);
            SetContent(tb2, text);
            cnv.Children.Add(tb2);
            tb2.RenderTransform = new TranslateTransform() { X = -outlineWidth, Y = outlineWidth };
            // do outline image down
            tb2 = GetStyledTextblock(style, panelWidth, height, true);
            SetContent(tb2, text);
            cnv.Children.Add(tb2);
            tb2.RenderTransform = new TranslateTransform() { X = 0, Y = outlineWidth };

            // add the main text
            cnv.Children.Add(textblock);

            // add the border
            contentElement = cnv;
            return contentElement;
        }

        private TextBlock GetStyledTextblock(TimedTextStyle style, double width, double height, bool fOutline)
        {
            TextBlock textblock = new TextBlock();
            //textblock.Width = width;
            textblock.FontStyle = style.FontStyle;
            textblock.FontWeight = FontWeightConverter.Convert(style.FontWeight);
            textblock.VerticalAlignment = VerticalAlignment.Bottom;

#if !WINDOWS_PHONE7
            if (style.FontFamily.Source == "_Smallcaps")
            {
                Typography.SetCapitals(textblock, FontCapitals.SmallCaps);
            }
            else
#endif
            {
#if SILVERLIGHT
                textblock.FontFamily = style.FontFamily;
#else
                textblock.FontFamily = style.FontFamily.WindowsFontFamily;
#endif
            }
            if (!double.IsNaN(height) && height != 0)
            {
                textblock.FontSize = Math.Round(style.FontSize.ToPixelLength(height));
            }
            textblock.Foreground = GetCachedBrush(fOutline ? style.OutlineColor : Color.FromArgb(255, style.Color.R, style.Color.G, style.Color.B));

            textblock.Opacity = style.Visibility == Visibility.Visible
                                    ? style.Opacity
                                    : 0;
            //textblock.TextWrapping = style.WrapOption;
            textblock.TextAlignment = style.TextAlign;
            return textblock;
        }

        private Panel NewPanel(Panel parent, ref double offset, CaptionElement element, TextAlignment align)
        {
            var p = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            switch (align)
            {
                case TextAlignment.Center:
                    p.HorizontalAlignment = HorizontalAlignment.Center;
                    break;
                case TextAlignment.Right:
                    p.HorizontalAlignment = HorizontalAlignment.Right;
                    break;
                case TextAlignment.Left:
                    p.HorizontalAlignment = HorizontalAlignment.Left;
                    break;
                case TextAlignment.Justify:
                    p.HorizontalAlignment = HorizontalAlignment.Stretch;
                    break;
            }
            parent.Children.Add(p);
            offset = 0;
            return p;
        }

        private void SetAllContent(FrameworkElement contentElement, string text)
        {
            if (contentElement is TextBlock)
            {
                SetContent((TextBlock)contentElement, text);
            }
            else if (contentElement is Panel)
            {
                foreach (var textblock in ((Panel)contentElement).Children.OfType<TextBlock>())
                {
                    SetContent(textblock, text);
                }
            }
        }

        private void SetContent(TextBlock textblock, string text)
        {
            textblock.Text = text;
            textblock.UpdateLayout();
        }
    }
}
