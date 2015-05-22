using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Media.WebVTT
{
    internal sealed class MediaMarkerCollection : OrderedObservableCollection<MediaMarker>
    {
        public new void Add(MediaMarker mediaMarker)
        {
            var comparable = new MediaMarkerComparable(mediaMarker.Begin);
            Add(mediaMarker, comparable);
        }

        public IEnumerable<MediaMarker> WhereActiveAtPosition(TimeSpan mediaPosition, TimeSpan? searchAfter = null)
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

        private class MediaMarkerComparable : IComparable<MediaMarker>
        {
            private TimeSpan _searchTime;

            public MediaMarkerComparable(TimeSpan searchTime)
            {
                _searchTime = searchTime;
            }

            public int CompareTo(MediaMarker other)
            {
                return _searchTime.CompareTo(other.Begin);
            }
        }
    }
}
