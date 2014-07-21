using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.Media.AdaptiveStreaming.Dash
{
    internal static class IListExtensions
    {
        public static void AddRange<T>(this IList<T> collection, IEnumerable<T> items)
        {
            if (collection is List<T>)
            {
                ((List<T>)collection).AddRange(items);
            }
            else
            {
                foreach (var item in items)
                {
                    collection.Add(item);
                }
            }
        }
    }

    internal static class XLinqExtensions
    {
        public static bool GetBool(this XAttribute attrib)
        {
            return bool.Parse(attrib.Value);
        }

        public static bool? GetNullableBool(this XAttribute attrib)
        {
            return attrib == null || string.IsNullOrEmpty(attrib.Value) ? new bool?() : bool.Parse(attrib.Value);
        }

        public static double? GetNullableDouble(this XAttribute attrib)
        {
            return attrib == null || string.IsNullOrEmpty(attrib.Value) ? new double?() : double.Parse(attrib.Value);
        }

        public static uint GetUInt(this XAttribute attrib)
        {
            return uint.Parse(attrib.Value);
        }

        public static uint? GetNullableUInt(this XAttribute attrib)
        {
            return attrib == null || string.IsNullOrEmpty(attrib.Value) ? new uint?() : uint.Parse(attrib.Value);
        }

        public static ulong GetULong(this XAttribute attrib)
        {
            return ulong.Parse(attrib.Value);
        }

        public static ulong? GetNullableULong(this XAttribute attrib)
        {
            return attrib == null || string.IsNullOrEmpty(attrib.Value) ? new ulong?() : ulong.Parse(attrib.Value);
        }

        public static DateTimeOffset? GetNullableDateTime(this XAttribute attrib)
        {
            return attrib == null || string.IsNullOrEmpty(attrib.Value) ? new DateTimeOffset?() : DateTimeOffset.Parse(attrib.Value);
        }

        public static TimeSpan? GetNullableDuration(this XAttribute attrib)
        {
            return attrib == null || string.IsNullOrEmpty(attrib.Value) ? new TimeSpan?() : System.Xml.XmlConvert.ToTimeSpan(attrib.Value);
        }

        public static T? GetNullableEnum<T>(this XAttribute attrib) where T : struct
        {
            return attrib == null || string.IsNullOrEmpty(attrib.Value) ? new T?() : (T)Enum.Parse(typeof(T), attrib.Value, true);
        }

        public static T GetEnum<T>(this XAttribute attrib) where T : struct
        {
            return (T)Enum.Parse(typeof(T), attrib.Value, true);
        }

        public static ConditionalUInt GetConditionalUInt(this XAttribute attrib)
        {
            if (attrib != null && string.IsNullOrEmpty(attrib.Value))
            {
                var value = attrib.Value;
                bool boolValue;
                if (bool.TryParse(value, out boolValue))
                {
                    return new ConditionalUInt() { BooleanValue = boolValue };
                }
                else
                {
                    uint numericValue;
                    if (uint.TryParse(value, out numericValue))
                    {
                        return new ConditionalUInt() { NumericValue = numericValue };
                    }
                }
            }
            return null;
        }

        public static IEnumerable<uint> GetUIntVector(this XAttribute attrib)
        {
            return attrib.GetStringVector().Select(uint.Parse);
        }

        public static IEnumerable<string> GetStringVector(this XAttribute attrib)
        {
            return attrib == null || string.IsNullOrEmpty(attrib.Value) ? Enumerable.Empty<string>() : attrib.Value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }

    public sealed class ConditionalUInt
    {
        public bool BooleanValue { get; set; }
        public uint NumericValue { get; set; }
    }
}
