using System;

namespace Microsoft.Media.Advertising
{
    public interface IClipAdPayload
    {
        Uri MediaSource { get; set; }
        string MimeType { get; set; }
        Uri ClickThrough { get; set; }
    }
}
