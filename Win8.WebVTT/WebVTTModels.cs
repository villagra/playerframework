using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WebVTT
{
    public sealed class WebVTTDocument
    {
        public WebVTTDocument()
        {
            Cues = new List<WebVTTCue>();
        }

        public IList<WebVTTCue> Cues { get; private set; }
    }

    public sealed class WebVTTCue
    {
        public WebVTTCue()
        {
            Settings = new WebVTTCueSettings();
        }

        public string StyleClass { get; set; }
        public TimeSpan Begin { get; set; }
        public TimeSpan End { get; set; }
        public IWebVTTContent Content { get; set; }
        public WebVTTCueSettings Settings { get; set; }
    }

    public sealed class WebVTTCueSettings
    {
        public WebVTTCueSettings()
        {
            WritingMode = WebVTTWritingMode.Horizontal;
            Alignment = WebVTTAlignment.Middle;
            TextPosition = 50;
            Size = 100;
            SnapToLines = true;
        }

        public bool SnapToLines { get; set; }
        public WebVTTWritingMode WritingMode { get; set; }
        public int? LinePosition { get; set; }
        public int TextPosition { get; set; }
        public int Size { get; set; }
        public WebVTTAlignment Alignment { get; set; }
    }

    public enum WebVTTAlignment
    {
        Start,
        Middle,
        End,
        Left,
        Right
    }

    public enum WebVTTWritingMode
    {
        Horizontal,
        VerticalGrowingLeft,
        VerticalGrowingRight
    }

    internal sealed class WebVTTContent : WebVTTInternalNode, IWebVTTContent
    {
        WebVTTParagraphDirection? paragraphDirection;
        public WebVTTParagraphDirection ParagraphDirection
        {
            get
            {
                if (!paragraphDirection.HasValue)
                {
                    paragraphDirection = GetDirection(GetAllText());
                }
                return paragraphDirection.Value;
            }
        }

        //Determine direction
        //http://www.unicode.org/reports/tr9/
        //P2. In each paragraph, find the first character of type L, AL, or R.
        //If a character is found in P2 and it is of type AL or R, then set the paragraph embedding level to one; otherwise, set it to zero.
        //If the paragraph embedding level determined in the previous step is even (the paragraph direction is left-to-right), let direction be 'ltr', otherwise, let it be 'rtl'.
        static WebVTTParagraphDirection GetDirection(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                int codepoint = s[i];
                if (UnicodeExtensions.IsTypeB(codepoint))
                {
                    break;
                }
                if (UnicodeExtensions.IsLCat(codepoint))
                {
                    return WebVTTParagraphDirection.LeftToRight;
                }
                else if (UnicodeExtensions.IsRandALCat(codepoint))
                {
                    return WebVTTParagraphDirection.RightToLeft;
                }
            }
            return WebVTTParagraphDirection.LeftToRight;
        }
    }

    internal class WebVTTNode : IWebVTTNode
    {
        public WebVTTNode() { }
    }

    internal class WebVTTInternalNode : WebVTTNode, IWebVTTInternalNode
    {
        public WebVTTInternalNode()
        {
            Nodes = new List<IWebVTTNode>();
        }

        public IList<IWebVTTNode> Nodes { get; private set; }

        internal string GetAllText()
        {
            var sb = new StringBuilder();
            foreach (var node in Nodes)
            {
                if (node is WebVTTInternalNode)
                {
                    sb.Append(((WebVTTInternalNode)node).GetAllText());
                }
                else if (node is WebVTTTextNode)
                {
                    sb.Append(((WebVTTTextNode)node).Text);
                }
            }
            return sb.ToString();
        }
    }

    internal class WebVTTLeafNode : WebVTTNode, IWebVTTLeafNode
    {
        public WebVTTLeafNode() { }
    }

    internal sealed class WebVTTTextNode : WebVTTLeafNode, IWebVTTTextNode
    {
        public string Text { get; set; }
    }

    internal class WebVTTSpanNode : WebVTTInternalNode, IWebVTTSpanNode
    {
        public WebVTTSpanNode()
        {
            Classes = new List<string>();
        }

        public IList<string> Classes { get; set; }
    }

    internal sealed class WebVTTTimestampNode : WebVTTSpanNode, IWebVTTTimestampNode
    {
        public TimeSpan Timestamp { get; set; }
    }

    internal sealed class WebVTTClassNode : WebVTTSpanNode, IWebVTTClassNode
    { }

    internal sealed class WebVTTItalicNode : WebVTTSpanNode, IWebVTTItalicNode
    { }

    internal sealed class WebVTTBoldNode : WebVTTSpanNode, IWebVTTBoldNode
    { }

    internal sealed class WebVTTUnderlineNode : WebVTTSpanNode, IWebVTTUnderlineNode
    { }

    internal sealed class WebVTTRubyNode : WebVTTSpanNode, IWebVTTRubyNode
    { }

    internal sealed class WebVTTRubyTextNode : WebVTTSpanNode, IWebVTTRubyTextNode
    { }

    internal sealed class WebVTTVoiceNode : WebVTTSpanNode, IWebVTTVoiceNode
    {
        public string Name { get; set; }
    }

    internal sealed class WebVTTLanguageNode : WebVTTSpanNode, IWebVTTLanguageNode
    {
        public string Language { get; set; }
    }
}
