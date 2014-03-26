using Microsoft.Web.Media.SmoothStreaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Microsoft.Media.AdaptiveStreaming.Helper
{
    public static class SmoothStreamingExtensions
    {
        private const string NameAttribute = "name";
        private const string LanguageAttribute = "language";
        private const string TypeAttribute = "type";
        private const string HeightAttribute = "height";
        private const string WidthAttribute = "width";
        private const string MaxHeightAttribute = "maxheight";
        private const string MaxWidthAttribute = "maxwidth";

        public static string GetName(this StreamInfo stream)
        {
            return stream.Attributes.GetEntryIgnoreCase(NameAttribute);
        }

        public static string GetLanguage(this StreamInfo stream)
        {
            return stream.Attributes.GetEntryIgnoreCase(LanguageAttribute);
        }

        public static string GetStreamType(this StreamInfo stream)
        {
            return stream.Attributes.GetEntryIgnoreCase(TypeAttribute);
        }

        public static Size GetSize(this TrackInfo trackInfo)
        {
            string heightStr = trackInfo.Attributes.GetEntryIgnoreCase(MaxHeightAttribute) ?? trackInfo.Attributes.GetEntryIgnoreCase(HeightAttribute);
            string widthStr = trackInfo.Attributes.GetEntryIgnoreCase(MaxWidthAttribute) ?? trackInfo.Attributes.GetEntryIgnoreCase(WidthAttribute);
            double height, width;
            return double.TryParse(heightStr, out height)
                   && double.TryParse(widthStr, out width)
                       ? new Size(width, height)
                       : Size.Empty;
        }

        public static int GetWidth(this TrackInfo track)
        {
            return int.Parse(track.Attributes["MaxWidth"]);
        }

        public static TValue GetEntryIgnoreCase<TValue>(this IDictionary<string, TValue> dictionary, string key)
        {
            return dictionary.GetEntryIgnoreCase(key, default(TValue));
        }

        public static TValue GetEntryIgnoreCase<TValue>(this IDictionary<string, TValue> dictionary, string key, TValue defaultValue)
        {
            key = dictionary.Keys.Where(i => i.Equals(key, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            return !key.IsNullOrWhiteSpace()
                       ? dictionary[key]
                       : defaultValue;
        }

        static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
    }
}
