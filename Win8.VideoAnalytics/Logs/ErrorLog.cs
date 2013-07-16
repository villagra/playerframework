using System;
using System.Collections.Generic;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// A log generated when an application level error (unhandled exception) occurs.
    /// </summary>
    public sealed class ErrorLog : ILog
    {
        /// <summary>
        /// Creates a new instance of ErrorLog.
        /// </summary>
        /// <param name="error">The error message that occurred. This may contain a stacktrace.</param>
        /// <param name="applicationArea">The area of the application that the error occurred in.</param>
        public ErrorLog(string error, string applicationArea)
        {
            Id = Guid.NewGuid();
            TimeStamp = DateTimeOffset.Now;
            Type = LogTypes.ApplicationError;

            Error = error;
            ApplicationArea = applicationArea;
            ExtraData = new Dictionary<string, object>();
        }

        /// <inheritdoc /> 
        public IDictionary<string, object> ExtraData { get; private set; }

        /// <inheritdoc /> 
        public Guid Id { get; private set; }

        /// <inheritdoc /> 
        public DateTimeOffset TimeStamp { get; set; }

        /// <inheritdoc /> 
        public string Type { get; private set; }

        /// <inheritdoc /> 
        public IDictionary<string, object> GetData()
        {
            var result = this.CreateBasicLogData();
            var errorString = Error;
            if (MaxErrorLength.HasValue)
            {
                errorString = errorString.Substring(0, MaxErrorLength.Value);
            }
            result.Add("Message", errorString);
            result.Add("ApplicationArea", ApplicationArea);
            return result;
        }

        /// <summary>
        /// Gets the error message that occurred. Note: This may contain a stacktrace or anything else important.
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// The area of the application that the error occurred in. Typically this is set to 'UnhandledException'.
        /// </summary>
        public string ApplicationArea { get; private set; }

        /// <summary>
        /// The maximum length of the error message to be sent to the server. Set to null if you do not want to truncate.
        /// </summary>
        public int? MaxErrorLength { get; set; }
    }
}
