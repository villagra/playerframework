using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Microsoft.Media.WebVTT
{
    public sealed class MediaMarkerManager
    {
        private readonly object _syncObject = new object();
        private readonly IList<MediaMarker> previouslyActiveMarkers = new List<MediaMarker>();

        private TimeSpan? previousPosition;
        private MediaMarkerCollection markers;

        /// <summary>
        /// Occurs when a marker start time is reached.
        /// </summary>
        public event EventHandler<MediaMarkerEventArgs> MarkerReached;

        /// <summary>
        /// Occurs when a marker end time has been reached.
        /// </summary>
        public event EventHandler<MediaMarkerEventArgs> MarkerLeft;

        public MediaMarkerManager()
        {
            Markers = new MediaMarkerCollection();
        }

        public void Clear()
        {
            lock (_syncObject)
            {
                previousPosition = null;
                // clean up any previously active markers that are no longer in the markers list or are out of range
                foreach (var marker in previouslyActiveMarkers.ToList())
                {
                    OnMarkerLeft(marker);
                    previouslyActiveMarkers.Remove(marker);
                }
                Markers.Clear();
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
                foreach (var marker in previouslyActiveMarkers.ToList().Where(i => !markers.Contains(i) || !i.IsActiveAtPosition(mediaPosition)))
                {
                    OnMarkerLeft(marker);
                    previouslyActiveMarkers.Remove(marker);
                }

                var activeMarkers = markers.WhereActiveAtPosition(mediaPosition).ToList();

                foreach (var marker in activeMarkers.Except(previouslyActiveMarkers.ToList()))
                {
                    OnMarkerReached(marker);
                    previouslyActiveMarkers.Add(marker);
                }

                previousPosition = mediaPosition;
            }
        }

        void CheckMarkerPosition(TimeSpan mediaPosition, MediaMarker marker)
        {
            lock (_syncObject)
            {
                bool isActive = marker.IsActiveAtPosition(mediaPosition);
                if (isActive && !previouslyActiveMarkers.Contains(marker))
                {
                    OnMarkerReached(marker);
                    previouslyActiveMarkers.Add(marker);
                }
            }
        }

        void OnMarkerReached(MediaMarker mediaMarker)
        {
            if (MarkerReached != null) MarkerReached(this, new MediaMarkerEventArgs(mediaMarker));
        }

        void OnMarkerLeft(MediaMarker mediaMarker)
        {
            if (MarkerLeft != null) MarkerLeft(this, new MediaMarkerEventArgs(mediaMarker));
        }

        public IList<MediaMarker> MediaMarkers
        {
            get { return markers; }
        }

        private MediaMarkerCollection Markers
        {
            get { return markers; }
            set
            {
                if (markers != null)
                {
                    markers.CollectionChanged -= Captions_CollectionChanged;
                }

                markers = value;

                if (markers != null)
                {
                    markers.CollectionChanged += Captions_CollectionChanged;
                }
            }
        }

        private void Captions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (previousPosition.HasValue)
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    CheckMarkerPositions(previousPosition.Value);
                }
                else
                {
                    if (e.NewItems != null)
                    {
                        foreach (var marker in e.NewItems.Cast<MediaMarker>())
                        {
                            CheckMarkerPosition(previousPosition.Value, marker);
                        }
                    }

                    if (e.OldItems != null)
                    {
                        foreach (var marker in e.OldItems.Cast<MediaMarker>())
                        {
                            CheckMarkerPosition(previousPosition.Value, marker);
                        }
                    }
                }
            }
        }
    }

#if SILVERLIGHT
    public sealed class MediaMarkerEventArgs : EventArgs
#else
    public sealed class MediaMarkerEventArgs
#endif
    {
        internal MediaMarkerEventArgs(MediaMarker marker)
        {
            Marker = marker;
        }

        public MediaMarker Marker { get; private set; }
    }
}
