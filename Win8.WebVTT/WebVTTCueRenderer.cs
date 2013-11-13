using System;
#if SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;
#else
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.WebVTT
{
    public sealed class WebVTTCueRenderer
    {
        public event EventHandler<NodeRenderingEventArgs> NodeRendering;

        public double OutlineWidth { get; set; }

        public Brush OutlineBrush { get; set; }

        public Brush InnerBrush { get; set; }

        public BoxElement GetRenderedCue(WebVTTCue cue)
        {
            var alignment = cue.Settings.Alignment;

            var result = new BoxElement();

            if (OutlineWidth > 0)
            {
                // create adjacent textblocks
                var textBlockOutline1 = GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left);
                textBlockOutline1.Margin = new Thickness(OutlineWidth);
                textBlockOutline1.RenderTransform = new TranslateTransform() { X = -OutlineWidth, Y = 0 };
                textBlockOutline1.Foreground = OutlineBrush;
                var textBlockOutline2 = GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Top);
                textBlockOutline2.Margin = new Thickness(OutlineWidth);
                textBlockOutline2.RenderTransform = new TranslateTransform() { X = 0, Y = -OutlineWidth };
                textBlockOutline2.Foreground = OutlineBrush;
                var textBlockOutline3 = GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Right);
                textBlockOutline3.Margin = new Thickness(OutlineWidth);
                textBlockOutline3.RenderTransform = new TranslateTransform() { X = OutlineWidth, Y = 0 };
                textBlockOutline3.Foreground = OutlineBrush;
                var textBlockOutline4 = GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Bottom);
                textBlockOutline4.Margin = new Thickness(OutlineWidth);
                textBlockOutline4.RenderTransform = new TranslateTransform() { X = 0, Y = OutlineWidth };
                textBlockOutline4.Foreground = OutlineBrush;

                // create diagonal textblocks
                var textBlockOutline5 = GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.TopLeft);
                textBlockOutline5.Margin = new Thickness(OutlineWidth);
                textBlockOutline5.RenderTransform = new TranslateTransform() { X = -OutlineWidth, Y = -OutlineWidth };
                textBlockOutline5.Foreground = OutlineBrush;
                var textBlockOutline6 = GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.TopRight);
                textBlockOutline6.Margin = new Thickness(OutlineWidth);
                textBlockOutline6.RenderTransform = new TranslateTransform() { X = OutlineWidth, Y = -OutlineWidth };
                textBlockOutline6.Foreground = OutlineBrush;
                var textBlockOutline7 = GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.BottomRight);
                textBlockOutline7.Margin = new Thickness(OutlineWidth);
                textBlockOutline7.RenderTransform = new TranslateTransform() { X = OutlineWidth, Y = OutlineWidth };
                textBlockOutline7.Foreground = OutlineBrush;
                var textBlockOutline8 = GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.BottomLeft);
                textBlockOutline8.Margin = new Thickness(OutlineWidth);
                textBlockOutline8.RenderTransform = new TranslateTransform() { X = -OutlineWidth, Y = OutlineWidth };
                textBlockOutline8.Foreground = OutlineBrush;

                result.AddBlock(textBlockOutline1);
                result.AddBlock(textBlockOutline2);
                result.AddBlock(textBlockOutline3);
                result.AddBlock(textBlockOutline4);
                result.AddBlock(textBlockOutline5);
                result.AddBlock(textBlockOutline6);
                result.AddBlock(textBlockOutline7);
                result.AddBlock(textBlockOutline8);
            }

            var textBlock = GetRenderedCueTextBlock(cue, alignment, InnerBrush, TextPosition.Center);
            textBlock.Margin = new Thickness(OutlineWidth);
            textBlock.Foreground = InnerBrush;
            result.AddBlock(textBlock);

            return result;
        }

        private TextBlock GetRenderedCueTextBlock(WebVTTCue cue, WebVTTAlignment alignment, Brush brush, TextPosition textPosition)
        {
            var content = cue.Content;
            var result = new TextBlock();
            result.TextWrapping = TextWrapping.Wrap;

            switch (alignment)
            {
                case WebVTTAlignment.Middle:
                    result.TextAlignment = TextAlignment.Center;
                    break;
                case WebVTTAlignment.Left:
                    result.TextAlignment = TextAlignment.Left;
                    break;
                case WebVTTAlignment.Right:
                    result.TextAlignment = TextAlignment.Right;
                    break;
                case WebVTTAlignment.Start:
                    result.TextAlignment = TextAlignment.Left;
                    break;
                case WebVTTAlignment.End:
                    result.TextAlignment = TextAlignment.Right;
                    break;
            }

            CreateInlines(cue, content, result.Inlines, brush, textPosition);

            return result;
        }

        private void CreateInlines(WebVTTCue cue, IWebVTTInternalNode node, InlineCollection inlines, Brush brush, TextPosition textPosition)
        {
            foreach (var child in node.Nodes)
            {
                var inline = CreateInline(child, brush);
                if (inline != null)
                {
                    inlines.Add(inline);
                    if (inline is Span && child is IWebVTTInternalNode)
                    {
                        CreateInlines(cue, (IWebVTTInternalNode)child, ((Span)inline).Inlines, brush, textPosition);
                    }
                    if (NodeRendering != null) NodeRendering(this, new NodeRenderingEventArgs(cue, child, inline, textPosition));
                }
            }
        }

        private Inline CreateInline(IWebVTTNode node, Brush brush = null)
        {
            if (node is WebVTTTextNode)
            {
                var c = (WebVTTTextNode)node;
                return new Run() { Text = c.Text };
            }
            else if (node is WebVTTClassNode)
            {
                return new Span();
            }
            else if (node is WebVTTVoiceNode)
            {
                return new Span();
            }
            else if (node is WebVTTLanguageNode)
            {
                return new Span();
            }
            else if (node is WebVTTBoldNode)
            {
                return new Bold();
            }
            else if (node is WebVTTItalicNode)
            {
                return new Italic();
            }
            else if (node is WebVTTUnderlineNode)
            {
                return new Underline();
            }
            else if (node is WebVTTRubyNode)
            {
                return new Span();
            }
            else if (node is WebVTTRubyTextNode)
            {
                // TODO: ruby annotations
            }
            else if (node is WebVTTTimestampNode)
            {
                var span = new Span();
                if (brush != null) span.Foreground = brush;
                return span;
            }
            return null;
        }
    }

#if SILVERLIGHT
    public sealed class NodeRenderingEventArgs : EventArgs
#else
    public sealed class NodeRenderingEventArgs
#endif
    {
        public NodeRenderingEventArgs(WebVTTCue cue, IWebVTTNode node, Inline inline, TextPosition textPosition)
        {
            Cue = cue;
            Node = node;
            Inline = inline;
            TextPosition = textPosition;
        }

        public WebVTTCue Cue { get; private set; }
        public IWebVTTNode Node { get; private set; }
        public Inline Inline { get; private set; }

        /// <summary>
        /// Gets the text position
        /// </summary>
        public TextPosition TextPosition { get; private set; }
    }

}
