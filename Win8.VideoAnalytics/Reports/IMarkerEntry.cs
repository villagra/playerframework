using System;
using System.Collections.Generic;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// An interface that a log can implement to control how aggregation calculates start and end time.
    /// </summary>
    public interface IMarkerEntry : ILog
    {
        /// <summary>
        /// Gets whether or not playback has started
        /// </summary>
        bool IsPlaying { get; }
    }
}
