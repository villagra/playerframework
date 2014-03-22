using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Microsoft.Media.TimedText
{
    public class MediaMarkerManager<TMediaMarker> : IMarkerManager<TMediaMarker> where TMediaMarker : MediaMarker
    {
        private readonly object _syncObject = new object();

        protected TimeSpan? PreviousPosition { get; private set; }
        protected IList<TMediaMarker> PreviouslyActiveMarkers { get; private set; }

        /// <summary>
        /// Occurs when a marker start time is reached.
        /// </summary>
        public event Action<IMarkerManager<TMediaMarker>, TMediaMarker> MarkerReached;

        /// <summary>
        /// Occurs when a marker end time has been reached.
        /// </summary>
        public event Action<IMarkerManager<TMediaMarker>, TMediaMarker> MarkerLeft;

        public MediaMarkerManager()
        {
            PreviouslyActiveMarkers = new List<TMediaMarker>();
        }

        public void Clear()
        {
            lock (_syncObject)
            {
                PreviousPosition = null;
                PreviouslyActiveMarkers.Clear();
            }
        }

        /// <summary>
        /// Checks for markers whose position has been reached or left.
        /// </summary>
        public void CheckMarkerPositions(TimeSpan mediaPosition)
        {
            lock (_syncObject)
            {
                // clean up any previously active markers that are no longer in the markers list or are out of range
                foreach (var marker in PreviouslyActiveMarkers.ToList().Where(i => !markers.Contains(i) || !i.IsActiveAtPosition(mediaPosition)))
                {
                    OnMarkerLeft(marker);
                    PreviouslyActiveMarkers.Remove(marker);
                }

                var activeMarkers = markers.WhereActiveAtPosition(mediaPosition).ToList();

                foreach (var marker in activeMarkers.Except(PreviouslyActiveMarkers.ToList()))
                {
                    OnMarkerReached(marker);
                    PreviouslyActiveMarkers.Add(marker);
                }

                PreviousPosition = mediaPosition;
            }
        }

        void CheckMarkerPosition(TimeSpan mediaPosition, TMediaMarker marker)
        {
            lock (_syncObject)
            {
                bool isActive = marker.IsActiveAtPosition(mediaPosition);
                if (isActive && !PreviouslyActiveMarkers.Contains(marker))
                {
                    OnMarkerReached(marker);
                    PreviouslyActiveMarkers.Add(marker);
                }
            }
        }

        protected virtual void OnMarkerReached(TMediaMarker mediaMarker)
        {
            MarkerReached.IfNotNull(i => i(this, mediaMarker));
        }

        protected virtual void OnMarkerLeft(TMediaMarker mediaMarker)
        {
            MarkerLeft.IfNotNull(i => i(this, mediaMarker));
        }

        MediaMarkerCollection<TMediaMarker> markers;
        public MediaMarkerCollection<TMediaMarker> Markers
        {
            get { return markers; }
            set
            {
                if (markers != null)
                {
                    markers.CollectionChanged -= Captions_CollectionChanged;
                    markers.MarkerPositionChanged -= CaptionPositionChanged;
                }

                markers = value;

                if (markers != null)
                {
                    markers.CollectionChanged += Captions_CollectionChanged;
                    markers.MarkerPositionChanged += CaptionPositionChanged;
                }
            }
        }

        private void Captions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (PreviousPosition.HasValue)
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    CheckMarkerPositions(PreviousPosition.Value);
                }
                else
                {
                    if (e.NewItems != null)
                    {
                        e.NewItems.Cast<TMediaMarker>().ForEach(i => CheckMarkerPosition(PreviousPosition.Value, i));
                    }

                    if (e.OldItems != null)
                    {
                        e.OldItems.Cast<TMediaMarker>().ForEach(i => CheckMarkerPosition(PreviousPosition.Value, i));
                    }
                }
            }
        }

        private void CaptionPositionChanged(TMediaMarker captionRegion)
        {
            if (PreviousPosition.HasValue)
            {
                CheckMarkerPosition(PreviousPosition.Value, captionRegion);
            }
        }
    }
}
