using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// Provides helper methods to act on logs.
    /// </summary>
    public static class LoggingExtensions
    {
        /// <summary>
        /// Gets only the required data for a log. Typically this is called from a log itself in the GetData method to serve as a starting point for the result that will ultimately be returned.
        /// </summary>
        /// <param name="log">The log to get basic data for.</param>
        /// <returns>A dictionary of data for the required properties on the log.</returns>
        public static IDictionary<string, object> CreateBasicLogData(this ILog log)
        {
            var result = new Dictionary<string, object>();
            result.Add("TimeStamp", log.TimeStamp);
            result.Add("Type", log.Type);
            result.Add("LogId", log.Id);
            if (log.ExtraData != null)
            {
                foreach (var item in log.ExtraData)
                {
                    result.Add(item.Key, item.Value);
                }
            }
            return result;
        }
    }
}