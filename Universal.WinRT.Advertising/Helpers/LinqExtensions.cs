using System;
using System.Collections.Generic;

namespace System.Linq
{
    internal static class LinqExtensions
    {
        public static IEnumerable<T> DefaultIfEmpty<T>(this IEnumerable<T> source, IEnumerable<T> defaultValue)
        {
            if (!source.Any())
            {
                return defaultValue;
            }
            else
            {
                return source;
            }
        }
    }
}
