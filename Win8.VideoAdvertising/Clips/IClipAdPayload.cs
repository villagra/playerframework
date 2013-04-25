using System;

namespace Microsoft.VideoAdvertising
{
    public interface IClipAdPayload
    {
        Uri MediaSource { get; set; }
        string MimeType { get; set; }
        Uri ClickThrough { get; set; }
    }
}
