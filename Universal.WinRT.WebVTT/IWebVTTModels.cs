using System;
using System.Collections.Generic;

namespace Microsoft.Media.WebVTT
{
    public interface IWebVTTNode
    {
    }

    public interface IWebVTTInternalNode : IWebVTTNode
    {
        IList<IWebVTTNode> Nodes { get; }
    }

    public interface IWebVTTLeafNode : IWebVTTNode
    {
    }

    public interface IWebVTTContent : IWebVTTInternalNode
    {
        WebVTTParagraphDirection ParagraphDirection { get; }
    }

    public interface IWebVTTSpanNode : IWebVTTInternalNode
    {
        IList<string> Classes { get; }
    }

    public interface IWebVTTTextNode : IWebVTTLeafNode
    {
        string Text { get; }
    }

    public interface IWebVTTTimestampNode : IWebVTTSpanNode
    {
        TimeSpan Timestamp { get; }
    }

    public interface IWebVTTClassNode : IWebVTTSpanNode
    { }

    public interface IWebVTTItalicNode : IWebVTTSpanNode
    { }

    public interface IWebVTTBoldNode : IWebVTTSpanNode
    { }

    public interface IWebVTTUnderlineNode : IWebVTTSpanNode
    { }

    public interface IWebVTTRubyNode : IWebVTTSpanNode
    { }

    public interface IWebVTTRubyTextNode : IWebVTTSpanNode
    { }

    public interface IWebVTTVoiceNode : IWebVTTSpanNode
    {
        string Name { get; }
    }

    public interface IWebVTTLanguageNode : IWebVTTSpanNode
    {
        string Language { get; }
    }

    public enum WebVTTParagraphDirection
    {
        LeftToRight,
        RightToLeft
    }
}
