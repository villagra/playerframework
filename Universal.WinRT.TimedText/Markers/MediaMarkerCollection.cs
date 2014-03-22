using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Microsoft.Media.TimedText
{
    public class MediaMarkerCollection<TMediaMarker> : OrderedObservableCollection<TMediaMarker> where TMediaMarker : MediaMarker
    {
        public event Action<TMediaMarker> MarkerPositionChanged;
        readonly IList<TMediaMarker> items = new List<TMediaMarker>();

        bool _suspendCollectionChangedEvents = false;

        public new void Add(TMediaMarker mediaMarker)
        {
            if (mediaMarker is CaptionElement)
            {
                var comparable = new CaptionMarkerComparable(mediaMarker.Begin, (mediaMarker as CaptionElement).Index, true);
                Add(mediaMarker, comparable);
            }
            else
            {
                var comparable = new MediaMarkerComparable(mediaMarker.Begin);
                Add(mediaMarker, comparable);
            }
        }

        public IEnumerable<TMediaMarker> WhereActiveAtPosition(TimeSpan mediaPosition, TimeSpan? searchAfter = null)
        {
            var startIndex = 0;

            if (searchAfter.HasValue)
            {
                var comparer = new MediaMarkerComparable(searchAfter.Value);
                startIndex = SearchForInsertIndex(comparer);
            }

            return this.Skip(startIndex)
                .TakeWhile(i => i.Begin <= mediaPosition)
                .Where(i => i.IsActiveAtPosition(mediaPosition))
                .ToList();
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var item in items.ToList())
                {
                    OnItemRemoved(item);
                }
                foreach (var item in this)
                {
                    OnItemAdded(item);
                }
            }
            else
            {
                if (e.NewItems != null)
                {
                    e.NewItems.Cast<TMediaMarker>().ForEach(OnItemAdded);
                }

                if (e.OldItems != null)
                {
                    e.OldItems.Cast<TMediaMarker>().ForEach(OnItemRemoved);
                }
            }

            if (!_suspendCollectionChangedEvents)
            {
                base.OnCollectionChanged(e);
            }
        }

        private void OnItemAdded(TMediaMarker item)
        {
            item.PositionChanged += Item_PositionChanged;
            items.Add(item);
        }

        private void OnItemRemoved(TMediaMarker item)
        {
            item.PositionChanged -= Item_PositionChanged;
            items.Remove(item);
        }

        private void Item_PositionChanged(MediaMarker item)
        {
            var tItem = item as TMediaMarker;

            if (tItem != null)
            {
                _suspendCollectionChangedEvents = true;
                Remove(tItem);
                Add(tItem);
                _suspendCollectionChangedEvents = false;
                MarkerPositionChanged.IfNotNull(i => i(tItem));
            }
        }

        private class CaptionMarkerComparable : IComparable<TMediaMarker>
        {
            private TimeSpan _searchTime;
            int _index;
            bool _compareBeginTime;

            public CaptionMarkerComparable(TimeSpan searchTime, int index, bool compareBeginTime)
            {
                _searchTime = searchTime;
                _index = index;
                _compareBeginTime = compareBeginTime;
            }

            public int CompareTo(TMediaMarker other)
            {
                int ret = 0;
                if (_compareBeginTime)
                {
                    ret = _searchTime.CompareTo(other.Begin);
                }
                else
                {
                    ret = _searchTime.CompareTo(other.End);
                }

                if (ret == 0)
                {
                    if (other is CaptionElement && _index != int.MinValue)
                    {
                        var i = (other as CaptionElement).Index;
                        if (i != _index)
                        {
                            return _index.CompareTo(i);
                        }
                    }
                }
                return ret;
            }
        }


        private class MediaMarkerComparable : IComparable<TMediaMarker>
        {
            private TimeSpan _searchTime;

            public MediaMarkerComparable(TimeSpan searchTime)
            {
                _searchTime = searchTime;
            }

            public int CompareTo(TMediaMarker other)
            {
                return _searchTime.CompareTo(other.Begin);
            }
        }
    }
}
