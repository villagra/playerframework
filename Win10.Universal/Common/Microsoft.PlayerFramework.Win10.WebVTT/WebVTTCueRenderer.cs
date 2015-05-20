using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Microsoft.Media.WebVTT
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

        public FontFamily FontFamily { get; set; }

        public FontCapitals FontCapitals { get; set; }

        public WebVTTCueRenderer()
        {
            this.FontFamily = new FontFamily("Arial");
            this.FontCapitals = FontCapitals.Normal;
        }

        public BoxElement GetRenderedCue(WebVTTCue cue)
        {
            var alignment = cue.Settings.Alignment;

            var result = new BoxElement();

            if (Windows.Media.ClosedCaptioning.ClosedCaptionProperties.FontEffect != Windows.Media.ClosedCaptioning.ClosedCaptionEdgeEffect.Default)
            {
                switch (Windows.Media.ClosedCaptioning.ClosedCaptionProperties.FontEffect)
                {
                    case Windows.Media.ClosedCaptioning.ClosedCaptionEdgeEffect.Depressed:
                        return GetDepressedEdgeBox(cue);
                    case Windows.Media.ClosedCaptioning.ClosedCaptionEdgeEffect.DropShadow:
                        return GetDropShadowBox(cue);
                    case Windows.Media.ClosedCaptioning.ClosedCaptionEdgeEffect.Raised:
                        return GetRaisedEdgeBox(cue);
                    case Windows.Media.ClosedCaptioning.ClosedCaptionEdgeEffect.Uniform:
                        return GetOutlineBox(cue);
                }
            }

            var box = new BoxElement();
            box.AddBlock(GetRenderedCueTextBlock(cue, alignment, InnerBrush, TextPosition.Center));
            return box;
        }

        private BoxElement GetDepressedEdgeBox(WebVTTCue cue)
        {
            var box = new BoxElement();

            var alignment = cue.Settings.Alignment;

            // top left
            box.AddBlock(GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left,
                new TranslateTransform() { X = -OutlineWidth, Y = -OutlineWidth }, OutlineWidth));
            // top
            box.AddBlock(GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left,
                new TranslateTransform() { X = 0, Y = -OutlineWidth }, OutlineWidth));
            // left
            box.AddBlock(GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left,
                new TranslateTransform() { X = -OutlineWidth, Y = 0 }, OutlineWidth));

            box.AddBlock(GetRenderedCueTextBlock(cue, alignment, InnerBrush, TextPosition.Center, null, OutlineWidth));

            return box;
        }

        private BoxElement GetRaisedEdgeBox(WebVTTCue cue)
        {
            var box = new BoxElement();

            var alignment = cue.Settings.Alignment;

            // right
            box.AddBlock(GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left,
                new TranslateTransform() { X = OutlineWidth, Y = 0 }, OutlineWidth));
            // bottom
            box.AddBlock(GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left,
                new TranslateTransform() { X = 0, Y = OutlineWidth }, OutlineWidth));
            // bottom right
            box.AddBlock(GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left,
                new TranslateTransform() { X = OutlineWidth, Y = OutlineWidth }, OutlineWidth));

            box.AddBlock(GetRenderedCueTextBlock(cue, alignment, InnerBrush, TextPosition.Center, null, OutlineWidth));

            return box;
        }

        private BoxElement GetDropShadowBox(WebVTTCue cue)
        {
            var box = new BoxElement();

            var alignment = cue.Settings.Alignment;

            // right
            box.AddBlock(GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left,
                new TranslateTransform() { X = OutlineWidth, Y = 0 }, OutlineWidth));
            // bottom
            box.AddBlock(GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left,
                new TranslateTransform() { X = 0, Y = OutlineWidth }, OutlineWidth));
            // bottom right
            box.AddBlock(GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left,
                new TranslateTransform() { X = OutlineWidth, Y = OutlineWidth }, OutlineWidth));

            box.AddBlock(GetRenderedCueTextBlock(cue, alignment, InnerBrush, TextPosition.Center, null, OutlineWidth));

            return box;
        }

        private BoxElement GetOutlineBox(WebVTTCue cue)
        {
            var box = new BoxElement();

            var alignment = cue.Settings.Alignment;

            if (OutlineWidth > 0)
            {
                // create adjacent textblocks
                box.AddBlock(GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left,
                    new TranslateTransform() { X = -OutlineWidth, Y = 0 }, OutlineWidth));
                box.AddBlock(GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left,
                    new TranslateTransform() { X = 0, Y = -OutlineWidth }, OutlineWidth));
                box.AddBlock(GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left,
                    new TranslateTransform() { X = OutlineWidth, Y = 0 }, OutlineWidth));
                box.AddBlock(GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left,
                    new TranslateTransform() { X = 0, Y = OutlineWidth }, OutlineWidth));

                // create diagonal textblocks
                box.AddBlock(GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left,
                    new TranslateTransform() { X = -OutlineWidth, Y = -OutlineWidth }, OutlineWidth));
                box.AddBlock(GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left,
                    new TranslateTransform() { X = OutlineWidth, Y = -OutlineWidth }, OutlineWidth));
                box.AddBlock(GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left,
                    new TranslateTransform() { X = OutlineWidth, Y = OutlineWidth }, OutlineWidth));
                box.AddBlock(GetRenderedCueTextBlock(cue, alignment, OutlineBrush, TextPosition.Left,
                    new TranslateTransform() { X = -OutlineWidth, Y = OutlineWidth }, OutlineWidth));
            }

            box.AddBlock(GetRenderedCueTextBlock(cue, alignment, InnerBrush, TextPosition.Center, null, OutlineWidth));

            return box;
        }

        private TextBlock GetRenderedCueTextBlock(WebVTTCue cue, WebVTTAlignment alignment, Brush brush, TextPosition textPosition, Transform transform = null, double margin = 0)
        {
            var content = cue.Content;
            
            var result = new TextBlock
            {
                FontFamily = this.FontFamily,
                TextWrapping = TextWrapping.Wrap,
                RenderTransform = transform,
                Foreground = brush
            };

            result.Margin = new Thickness(margin);

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

    public sealed class NodeRenderingEventArgs
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
