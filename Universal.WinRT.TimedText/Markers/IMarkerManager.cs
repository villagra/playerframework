using System;

namespace Microsoft.Media.TimedText
{
    public interface IMarkerManager<TMediaMarker> where TMediaMarker : MediaMarker
    {
        MediaMarkerCollection<TMediaMarker> Markers { get; set; }

        void Clear();

        /// <summary>
        /// Occurs when a marker start time is reached.
        /// </summary>
        event Action<IMarkerManager<TMediaMarker>, TMediaMarker> MarkerReached;

        /// <summary>
        /// Occurs when a marker end time has been reached.
        /// </summary>
        event Action<IMarkerManager<TMediaMarker>, TMediaMarker> MarkerLeft;

        void CheckMarkerPositions(TimeSpan mediaPosition);
    }
}
