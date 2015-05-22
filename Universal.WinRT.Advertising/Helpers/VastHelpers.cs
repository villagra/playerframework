using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Media.Advertising
{
    public static class VastHelpers
    {
        /// <summary>
        /// Replaced with a random 8-­‐digit number.
        /// </summary>
        const string Macro_CacheBusting = "[CACHEBUSTING]";

        public static void FireTracking(string beaconUrl)
        {
            if (beaconUrl != null)
            {
                var url = GetMacroUrl(beaconUrl);
                AdTracking.Current.FireTracking(url);
            }
        }

        private static string GetMacroUrl(string url)
        {
            return url
                .Replace(Macro_CacheBusting, GetCacheBuster());
        }

        private static Random rnd = new Random();
        private static string GetCacheBuster()
        {
            return rnd.Next(100000000).ToString();
        }
    }
}
