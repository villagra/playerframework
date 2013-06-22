using System;

namespace Microsoft.Media.ISO
{
    /// <summary>
    /// Utility conversion class.
    /// </summary>
    internal static class Converter
    {
        private static readonly DateTime offsetDate = new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Convert an offset of seconds to the corresponding UTC datetime.
        /// </summary>
        /// <param name="numberOfSeconds">The number of seconds.</param>
        public static DateTime SecondsOffsetToDateTimeUtc(ulong numberOfSeconds)
        {
            return offsetDate.AddSeconds(numberOfSeconds);
        }
    }
}
