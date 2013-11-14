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

        /// <summary>
        /// Text rendering event handler
        /// </summary>
        public event EventHandler<CaptionTextEventArgs> TextRendering;

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
                var textBlockOutline1 = GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left, 
                    new TranslateTransform() { X = -OutlineWidth, Y = 0 });
                textBlockOutline1.Margin = new Thickness(OutlineWidth);
                var textBlockOutline2 = GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Top, 
                    new TranslateTransform { X = 0, Y = -OutlineWidth });
                textBlockOutline2.Margin = new Thickness(OutlineWidth);
                var textBlockOutline3 = GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Right, 
                    new TranslateTransform { X = OutlineWidth, Y = 0 });
                textBlockOutline3.Margin = new Thickness(OutlineWidth);
                var textBlockOutline4 = GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Bottom, 
                    new TranslateTransform { X = 0, Y = OutlineWidth });
                textBlockOutline4.Margin = new Thickness(OutlineWidth);

                // create diagonal textblocks
                var textBlockOutline5 = GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.TopLeft, 
                    new TranslateTransform { X = -OutlineWidth, Y = -OutlineWidth });
                textBlockOutline5.Margin = new Thickness(OutlineWidth);
                var textBlockOutline6 = GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.TopRight, 
                    new TranslateTransform { X = OutlineWidth, Y = -OutlineWidth });
                textBlockOutline6.Margin = new Thickness(OutlineWidth);
                var textBlockOutline7 = GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.BottomRight, 
                    new TranslateTransform { X = OutlineWidth, Y = OutlineWidth });
                textBlockOutline7.Margin = new Thickness(OutlineWidth);
                var textBlockOutline8 = GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.BottomLeft, 
                    new TranslateTransform { X = -OutlineWidth, Y = OutlineWidth });
                textBlockOutline8.Margin = new Thickness(OutlineWidth);

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
            result.AddBlock(textBlock);

            return result;
        }

        private TextBlock GetRenderedCueTextBlock(WebVTTCue cue, WebVTTAlignment alignment, Brush brush, TextPosition textPosition, Transform transform = null)
        {
            var content = cue.Content;
            
            var result = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                RenderTransform = transform,
                Foreground = brush
            };

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

            CreateInlines(cue, content, result.Inlines, brush);

            // If a Text Rendering event handler has been added, call it.
            if (this.TextRendering != null)
            {
                this.TextRendering(this, new CaptionTextEventArgs(result, textPosition));
            }

            return result;
        }

        private void CreateInlines(WebVTTCue cue, IWebVTTInternalNode node, InlineCollection inlines, Brush brush)
        {
            foreach (var child in node.Nodes)
            {
                var inline = CreateInline(child, brush);
                if (inline != null)
                {
                    inlines.Add(inline);
                    if (inline is Span && child is IWebVTTInternalNode)
                    {
                        CreateInlines(cue, (IWebVTTInternalNode)child, ((Span)inline).Inlines, brush);
                    }
                    if (NodeRendering != null) NodeRendering(this, new NodeRenderingEventArgs(cue, child, inline));
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
        public NodeRenderingEventArgs(WebVTTCue cue, IWebVTTNode node, Inline inline)
        {
            Cue = cue;
            Node = node;
            Inline = inline;
        }

        public WebVTTCue Cue { get; private set; }
        public IWebVTTNode Node { get; private set; }
        public Inline Inline { get; private set; }
    }

}
