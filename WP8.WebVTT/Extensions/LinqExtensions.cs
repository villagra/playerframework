using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Media.WebVTT
{
    internal static class LinqExtensions
    {
        public static int RemoveAll<T>(this List<T> collection, Predicate<T> match)
        {
            int count = 0;
            foreach (var item in collection.Where(match.Invoke).ToList())
            {
                collection.Remove(item);
                count++;
            }
            return count;
        }
    }
}
