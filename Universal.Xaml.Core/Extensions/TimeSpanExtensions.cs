using System;

namespace Microsoft.PlayerFramework
{
    internal static class TimeSpanExtensions
    {
        public static TimeSpan Min(TimeSpan timeSpan1, TimeSpan timeSpan2)
        {
            return timeSpan1 < timeSpan2 ? timeSpan1 : timeSpan2;
        }

        public static TimeSpan Max(TimeSpan timeSpan1, TimeSpan timeSpan2)
        {
            return timeSpan1 > timeSpan2 ? timeSpan1 : timeSpan2;
        }
    }
}
