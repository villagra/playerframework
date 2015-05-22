using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Media.WebVTT
{
    internal static class StringExtensions
    {
        public static string[] Split(this string source, char[] separator, int count)
        {
            var result = new List<string>();
            var sb = new StringBuilder();
            foreach (var c in source.ToCharArray())
            {
                if (result.Count < count && separator.Contains(c))
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }
            result.Add(sb.ToString());
            return result.ToArray();
        }
    }
}
