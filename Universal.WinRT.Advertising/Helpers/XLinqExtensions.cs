using System;

namespace System.Xml.Linq
{
    internal static class XLinqExtensions
    {
        public static bool GetBoolAttribute(this XElement source, string name, bool defaultValue)
        {
            var attrib = source.Attribute(name);
            bool val = default(bool);
            if (attrib != null && !string.IsNullOrWhiteSpace(attrib.Value))
                bool.TryParse(attrib.Value, out val);

            return val;
        }

        public static int GetIntAttribute(this XElement source, string name)
        {
            var attrib = source.Attribute(name);
            int val = default(int);
            if (attrib != null && !string.IsNullOrWhiteSpace(attrib.Value))
                int.TryParse(attrib.Value, out val);

            return val;
        }

        public static long GetLongAttribute(this XElement source, string name)
        {
            var attrib = source.Attribute(name);
            long val = default(long);
            if (attrib != null && !string.IsNullOrWhiteSpace(attrib.Value))
                long.TryParse(attrib.Value, out val);

            return val;
        }
    }
}
