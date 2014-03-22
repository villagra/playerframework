using System;
using System.Collections.Generic;

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// A log generated when the application starts.
    /// </summary>
    public sealed class AppStartLog : ILog
    {
        /// <summary>
        /// Creates a new instance of AppStartLog.
        /// </summary>
        /// <param name="startupParam">The ID of the track</param>
        public AppStartLog(string startupParam)
        {
            TimeStamp = DateTimeOffset.Now;
            Type = LogTypes.ApplicationSessionStart;
            Id = Guid.NewGuid();
            StartupParam = startupParam;
            ExtraData = new Dictionary<string, object>();
        }

        /// <inheritdoc /> 
        public IDictionary<string, object> ExtraData { get; private set; }

        /// <inheritdoc /> 
        public Guid Id { get; set; }

        /// <inheritdoc /> 
        public DateTimeOffset TimeStamp { get; set; }

        /// <inheritdoc /> 
        public string Type { get; private set; }

        /// <inheritdoc /> 
        public IDictionary<string, object> GetData()
        {
            var result = this.CreateBasicLogData();
            result.Add("StartupParam", StartupParam);
            return result;
        }

        /// <summary>
        /// Gets additional info about how the app started
        /// </summary>
        public string StartupParam { get; private set; }
    }
}
