using Microsoft.Media.Analytics;
using System;
using Windows.UI.Xaml;

namespace Microsoft.PlayerFramework.Analytics
{
    /// <summary>
    /// Represents a helper class responsible for logging unhandled exceptions
    /// </summary>
    public class ErrorLogger : IDisposable
    {
        /// <summary>
        /// Gets or sets whether unhandled exceptions should be prevented. If true, this will catch the exception and not allow it to bubble up.
        /// </summary>
        public bool PreventUnhandledErrors { get; set; }

        /// <summary>
        /// Gets or sets whether exceptions should be trunacted and if so, to what number of characters.
        /// </summary>
        public int? MaxErrorLength { get; set; }

        /// <summary>
        /// Creates a new instance of ErrorLogger
        /// </summary>
        public ErrorLogger()
        {
            PreventUnhandledErrors = false;
            MaxErrorLength = 2048;
            Application.Current.UnhandledException += App_UnhandledException;
        }

        void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // log UEs
            LogError(e.Exception, "UnhandledException");
            e.Handled = e.Handled || PreventUnhandledErrors;
        }

        /// <summary>
        /// Logs an exception
        /// </summary>
        /// <param name="error">The exception object to log</param>
        /// <param name="applicationArea">The area of the application the error occurred in.</param>
        public void LogError(Exception error, string applicationArea)
        {
            var log = new ErrorLog(error.ToString(), applicationArea);
            log.MaxErrorLength = MaxErrorLength;
            LoggingService.Current.Log(log);
        }

        /// <inheritdoc /> 
        public void Dispose()
        {
            Application.Current.UnhandledException += App_UnhandledException;
        }
    }
}
