using System;
using System.Collections.Generic;
using System.Linq;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.Foundation;
#endif

namespace Microsoft.Media.Advertising
{
    /// <summary>
    /// Represents all the information necessary to render a VPAID ad.
    /// This will be used to find an appropriate VPAID plugin and then the VPAID plugin will optionally use it to play the ad.
    /// </summary>
    public interface ICreativeSource
    {
        /// <summary>
        /// Icons to be associated with the ad. Only applies to linear ads.
        /// </summary>
        IEnumerable<Icon> Icons { get; }

        /// <summary>
        /// The MIME type of the ad
        /// </summary>
        string MimeType { get; }

        /// <summary>
        /// the codec used to encode the file which can take values as specified by RFC 4281: http://tools.ietf.org/html/rfc4281
        /// </summary>
        string Codec { get; }

        /// <summary>
        /// The payload of the creative. This is usually a URL depending on the MediaSourceType but can also contain HTML.
        /// </summary>
        string MediaSource { get; }

        /// <summary>
        /// Additional information associated with the creative to help VPAID play the ad.
        /// </summary>
        string ExtraInfo { get; }

        /// <summary>
        /// The URL that should be launched if the ad is clicked. This is different from the URL used to track clicks.
        /// </summary>
        Uri ClickUrl { get; }

        /// <summary>
        /// Returns all click tracking events
        /// </summary>
        IEnumerable<string> ClickTracking { get; }

        /// <summary>
        /// Returns all tracking events
        /// </summary>
        IEnumerable<TrackingEvent> TrackingEvents { get; }

        /// <summary>
        /// The duration of the ad. Companion ads do not have durations. And durations for NonLinear ads are optional.
        /// </summary>
        TimeSpan? Duration { get; }

        /// <summary>
        /// Indicates what the MediaSource contains.
        /// </summary>
        MediaSourceEnum MediaSourceType { get; }
        
        /// <summary>
        /// Indicates whether or not the source is streaming vs. progressive
        /// </summary>
        bool IsStreaming { get; }

        /// <summary>
        /// The expanded dimensions of the creative.
        /// </summary>
        Size ExpandedDimensions { get; }

        /// <summary>
        /// The dimensions of the creative.
        /// </summary>
        Size Dimensions { get; }

        /// <summary>
        /// Indicates whether or not the creative can scale in size.
        /// </summary>
        bool IsScalable { get; }

        /// <summary>
        /// Indicates whether or not the aspect ratio should be maintained.
        /// </summary>
        bool MaintainAspectRatio { get; }

        /// <summary>
        /// Indicates how the creative is intended to be used.
        /// </summary>
        CreativeSourceType Type { get; }

        /// <summary>
        /// The ID of the Ad Creative
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The location (% or time) when the ad can be skipped
        /// </summary>
        FlexibleOffset SkippableOffset { get; }

        /// <summary>
        /// Gets the API Framework of the ad. This is usually "VPAID" or left empty.
        /// </summary>
        string ApiFramework { get; }
    }

    /// <summary>
    /// The possible values for how a creative is intended to be used.
    /// </summary>
    public enum CreativeSourceType
    {
        Linear,
        NonLinear
    }

    /// <summary>
    /// The various kinds of media sources.
    /// </summary>
    public enum MediaSourceEnum
    {
        Static,
        HTML,
        IFrame
    }
}
