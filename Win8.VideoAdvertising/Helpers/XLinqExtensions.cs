using System;

namespace System.Xml.Linq
{
    internal static class XLinqExtensions
    {
        public static bool GetBoolAttribute(this XElement source, string name, bool defaultValue)
        {
            var attrib = source.Attribute(name);
            return attrib == null ? defaultValue : bool.Parse(attrib.Value);
        }

        public static int GetIntAttribute(this XElement source, string name)
        {
            var attrib = source.Attribute(name);
            return attrib == null ? default(int) : int.Parse(attrib.Value);
        }

        public static long GetLongAttribute(this XElement source, string name)
        {
            var attrib = source.Attribute(name);
            return attrib == null ? default(long) : long.Parse(attrib.Value);
        }
    }
}
