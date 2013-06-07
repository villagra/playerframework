using System;
using System.Collections.Generic;

namespace Microsoft.VideoAdvertising
{
    public interface ICompanionSource
    {
        Uri ClickThrough { get; }

        string AltText { get; }

        string AdParameters { get; }

        int? Width { get; }

        int? Height { get; }

        int? AssetWidth { get; }

        int? AssetHeight { get; }

        int? ExpandedWidth { get; }

        int? ExpandedHeight { get; }

        string ApiFramework { get; }

        // HACK: setter required for JS consumption
        string AdSlotId { get; set; }

        string Id { get; }

        IList<string> ClickTracking { get; }

        IList<string> ViewTracking { get; }

        string MimeType { get; }

        string Content { get; }

        CompanionType Type { get; }
    }

    public enum CompanionType
    {
        Static,
        Html,
        IFrame
    }
}
