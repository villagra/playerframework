using System;
using System.Collections.Generic;

namespace System.Linq
{
    internal static class MyLinqExtensions
    {
        public static double WeightedAverage<T>(this IEnumerable<T> source, Func<T, double> valueSelector, Func<T, double> weightSelector)
        {
            double total = 0;
            double sum = 0;
            foreach (var item in source)
            {
                double value = valueSelector(item);
                double weight = weightSelector(item);
                total += value * weight;
                sum += weight;
            }
            return total / sum;
        }
    }
}