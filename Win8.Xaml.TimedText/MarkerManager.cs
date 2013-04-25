using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.TimedText;
using System.Collections.Specialized;

namespace Microsoft.PlayerFramework.TimedText
{
    public class MarkerManager<TMediaMarker> : IMarkerManager<TMediaMarker> where TMediaMarker : MediaMarker
    {
        // TODO: seeking backwards should clear all active markers
        // TOOD: seeking forward will still cause MarkerReached to fire, these should be ignored if not in active range
        // TODO: reversing/rewinding will not fire MarkerReached events. All active markers should be cleared when this occurs or they could get orphaned.
        // TODO: optimization: consolidate closely spaced events into the same marker event – this ensures paired hide event and the show event that occur at GOP boundaries are processed during the same MarkerReached event / UI update cycle – which effectively eliminates any blinking of the caption and other more annoying problems.
        // TODO: all MarkerManager instances could share the same MarkerReached event handlers and dictionaries to speed up lookup. Not sure if the perf impact is worth effort but theoretically would be faster.

        const string MarkerTypeBegin = "BeginMarker";
        const string MarkerTypeEnd = "EndMarker";
        readonly Dictionary<string, Tuple<TMediaMarker, TimelineMarker>> EligableMarkers = new Dictionary<string, Tuple<TMediaMarker, TimelineMarker>>();
        readonly Dictionary<string, Tuple<TMediaMarker, TimelineMarker>> ActiveMarkers = new Dictionary<string, Tuple<TMediaMarker, TimelineMarker>>();

        TimeSpan lastPosition;

        MediaPlayer mediaPlayer;
        public MarkerManager(MediaPlayer MediaPlayer)
        {
            mediaPlayer = MediaPlayer;
            mediaPlayer.MarkerReached += mediaPlayer_MarkerReached;
        }
        
        public void CheckMarkerPositions(TimeSpan mediaPosition, bool seeking = false)
        {
            lastPosition = mediaPosition;
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

                    foreach (var marker in markers)
                    {
                        AddMarker(marker);
                    }
                }
            }
        }

        private void AddMarker(TMediaMarker marker)
        {
            var timelineMarker = new TimelineMarker()
            {
                Time = marker.Begin,
                Type = MarkerTypeBegin,
                Text = marker.Id
            };
            EligableMarkers.Add(marker.Id, Tuple.Create(marker, timelineMarker));
            mediaPlayer.Markers.Add(timelineMarker);

            if (marker.Begin <= lastPosition && marker.End > lastPosition)
            {
                OnMarkerReached(timelineMarker);
            }
        }

        private void RemoveMarker(TMediaMarker marker)
        {
            if (EligableMarkers.ContainsKey(marker.Id))
            {
                EligableMarkers.Remove(marker.Id);
                mediaPlayer.Markers.Remove(mediaPlayer.Markers.FirstOrDefault(t => t.Type == MarkerTypeBegin && t.Text == marker.Id));
            }
            if (ActiveMarkers.ContainsKey(marker.Id))
            {
                RemoveEndMarker(marker);
            }
        }

        private void RemoveEndMarker(TMediaMarker marker)
        {
            ActiveMarkers.Remove(marker.Id);
            mediaPlayer.Markers.Remove(mediaPlayer.Markers.FirstOrDefault(t => t.Type == MarkerTypeEnd && t.Text == marker.Id));
            MarkerLeft.IfNotNull(i => i(this, marker));
        }
        
        private void Captions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                e.NewItems.Cast<TMediaMarker>().ForEach(AddMarker);
            }

            if (e.OldItems != null)
            {
                e.OldItems.Cast<TMediaMarker>().ForEach(RemoveMarker);
            }
        }

        private void CaptionPositionChanged(TMediaMarker marker)
        {
            RemoveMarker(marker);
            AddMarker(marker);
        }

        void mediaPlayer_MarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
        {
            OnMarkerReached(e.Marker);
        }

        private void OnMarkerReached(TimelineMarker timelineMarker)
        {
            if (timelineMarker.Type == MarkerTypeBegin)
            {
                if (EligableMarkers.ContainsKey(timelineMarker.Text))
                {
                    var marker = EligableMarkers[timelineMarker.Text].Item1;
                    MarkerReached.IfNotNull(i => i(this, marker, mediaPlayer.PlaybackRate != 1));

                    // add a new marker for the end time
                    var endTime = marker.End;
                    if (endTime > mediaPlayer.NaturalDuration.TimeSpan)
                    {
                        endTime = mediaPlayer.NaturalDuration.TimeSpan;
                    }
                    var endTimelineMarker = new TimelineMarker()
                    {
                        Time = endTime,
                        Type = MarkerTypeEnd,
                        Text = marker.Id
                    };
                    mediaPlayer.Markers.Add(endTimelineMarker);
                    ActiveMarkers.Add(marker.Id, Tuple.Create(marker, endTimelineMarker));
                }
            }
            else if (timelineMarker.Type == MarkerTypeEnd)
            {
                if (ActiveMarkers.ContainsKey(timelineMarker.Text))
                {
                    var marker = ActiveMarkers[timelineMarker.Text].Item1;
                    ActiveMarkers.Remove(marker.Id);
                    mediaPlayer.Markers.Remove(mediaPlayer.Markers.FirstOrDefault(t => t.Type == MarkerTypeEnd && t.Text == marker.Id));
                    MarkerLeft.IfNotNull(i => i(this, marker));
                }
            }
        }

        public void Clear()
        {
            foreach (var key in EligableMarkers.Keys)
            {
                mediaPlayer.Markers.Remove(mediaPlayer.Markers.FirstOrDefault(t => t.Type == MarkerTypeBegin && t.Text == key));
            }
            EligableMarkers.Clear();

            foreach (var key in ActiveMarkers.Keys)
            {
                mediaPlayer.Markers.Remove(mediaPlayer.Markers.FirstOrDefault(t => t.Type == MarkerTypeEnd && t.Text == key));
            }
            ActiveMarkers.Clear();
        }

        public event Action<IMarkerManager<TMediaMarker>, TMediaMarker, bool> MarkerReached;

        public event Action<IMarkerManager<TMediaMarker>, TMediaMarker> MarkerLeft;
    }
}
