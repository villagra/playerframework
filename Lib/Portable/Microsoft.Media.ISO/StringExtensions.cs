using System;
using System.Linq;
using System.Text;

namespace Microsoft.Media.ISO
{
    /// <summary>
    /// Extension methods for strings
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Converts a string of hex characters from Big Endian to Little Endian
        /// </summary>
        /// <param name="value">Big Endian hex string</param>
        /// <returns>Little Endian hex string</returns>
        public static string ToLittleEndian(this string value)
        {
            char[] bigEndianChars = value.ToCharArray();
            var result = new StringBuilder();
            for (int i = bigEndianChars.Length - 2; i >= 0; i -= 2)
            {
                result.Append(bigEndianChars[i]);
                result.Append(bigEndianChars[i + 1]);
            }
            return result.ToString();
        }
    }
}
