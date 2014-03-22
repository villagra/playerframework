using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Threading.Tasks;
#if SILVERLIGHT
#else
using Windows.Storage.Streams;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// This is the main configuration object used by the diagnostic component.
    /// An instance of this object is required for the AnalyticsCollector class to instantiate.
    /// </summary>
    public sealed class AnalyticsConfig
    {
        /// <summary>
        /// Responsible for indicating which quality data should be tracked and aggregated. Used to filter data that will not be used at the earliest possible moment to improve performance.
        /// Set to null to track all quality data.
        /// </summary>
        public QualityConfig QualityConfig { get; set; }

        /// <summary>
        /// Responsible for determining how logging data should be collected.
        /// </summary>
        public LoggingConfig LoggingConfig { get; set; }

        /// <summary>
        /// Indicates that specifics should be tracked about download errors and reported via the AnalyticsCollector.ReportAggregatedData event.
        /// </summary>
        public bool TrackDownloadErrors { get; set; }

        /// <summary>
        /// Indicates that quality data should be sampled, aggregated and reported via the AnalyticsCollector.ReportAggregatedData event.
        /// </summary>
        public bool TrackQuality { get; set; }

        /// <summary>
        /// Indicates the latency in KBps that needs to be reached before a LatencyAlert is generated.
        /// </summary>
        public double? LatencyAlertThreshold { get; set; }

        /// <summary>
        /// Indicates the interval that quality data should be generated. Default is every 30 seconds.
        /// </summary>
        public TimeSpan AggregationInterval { get; set; }
        
        /// <summary>
        /// This is the polling interval used to retrieve info that needs to be polled (e.g. FPS, CPU, and various adaptive streaming information)
        /// Default value is 2 seconds.
        /// </summary>
        public TimeSpan PollingInterval { get; set; }

        /// <summary>
        /// Contains additional data to add to every log.
        /// </summary>
        public IDictionary<string, string> AdditionalData { get; private set; }

        /// <summary>
        /// Creates a new instance of AnalyticsConfig.
        /// </summary>
        public AnalyticsConfig()
        {
            SetDefaultSettings();
        }

        void SetDefaultSettings()
        {
            TrackQuality = false;
            TrackDownloadErrors = false;
            AggregationInterval = TimeSpan.Zero;
            PollingInterval = TimeSpan.Zero;
            LatencyAlertThreshold = null;
            QualityConfig = new QualityConfig();
            AdditionalData = new Dictionary<string, string>();
        }

#if !SILVERLIGHT
        /// <summary>
        /// Deserializes Xml into an AnalyticsConfig object.
        /// </summary>
        /// <param name="source">The source URI of the config file.</param>
        /// <returns>An awaitable result.</returns>
        public static IAsyncOperation<AnalyticsConfig> Load(Uri source)
        {
            return AsyncInfo.Run(c => InternalLoad(source));
        }

        internal static async Task<AnalyticsConfig> InternalLoad(Uri source)
        {
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(source);
            using (var stream = await file.OpenStreamForReadAsync())
            {
                return Load(XmlReader.Create(stream));
            }
        }
#else

        /// <summary>
        /// Deserializes Xml into an AnalyticsConfig object.
        /// </summary>
        /// <param name="source">The source URI of the config file.</param>
        /// <returns>An awaitable result.</returns>
        public static AnalyticsConfig Load(Uri source)
        {
            using (var stream = System.Windows.Application.GetResourceStream(source).Stream)
            {
                return Load(XmlReader.Create(stream));
            }
        }
#endif

        /// <summary>
        /// Creates an instance of the main diagnostic config object from an XmlReader
        /// </summary>
        internal static AnalyticsConfig Load(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            var result = new AnalyticsConfig();
            result.LatencyAlertThreshold = null;

            reader.GoToElement();
            reader.ReadStartElement();
            if (!reader.IsEmptyElement)
            {
                while (reader.GoToSibling())
                {
                    switch (reader.LocalName)
                    {
                        case "AdditionalData":
                            result.AdditionalData = reader.ReadElementContentAsString().Split(',').Select(kvp => kvp.Split('=')).ToDictionary(kvp => kvp[0], kvp => kvp[1]);
                            break;
                        case "Logging":
                            if (!reader.IsEmptyElement)
                            {
                                result.LoggingConfig = LoggingConfig.Load(reader);
                            }
                            else reader.Skip();
                            break;
                        case "Diagnostics":
                            if (!reader.IsEmptyElement)
                            {
                                reader.ReadStartElement();
                                while (reader.GoToSibling())
                                {
                                    switch (reader.LocalName)
                                    {
                                        case "TrackQuality":
                                            result.TrackQuality = Convert.ToBoolean(reader.ReadElementContentAsInt());
                                            break;
                                        case "TrackDownloadErrors":
                                            result.TrackDownloadErrors = Convert.ToBoolean(reader.ReadElementContentAsInt());
                                            break;
                                        case "AggregationIntervalMilliseconds":
                                            result.AggregationInterval = TimeSpan.FromMilliseconds(reader.ReadElementContentAsInt());
                                            break;
                                        case "PollingMilliseconds":
                                            result.PollingInterval = TimeSpan.FromMilliseconds(reader.ReadElementContentAsInt());
                                            break;
                                        case "LatencyAlertThreshold":
                                            result.LatencyAlertThreshold = reader.ReadElementContentAsDouble();
                                            break;
                                        case "QualityTracking":
                                            result.QualityConfig = QualityConfig.Load(reader);
                                            break;
                                        default:
                                            reader.Skip();
                                            break;
                                    }
                                }
                                reader.ReadEndElement();
                            }
                            else
                                reader.Skip();
                            break;

                        default:
                            reader.Skip();
                            break;
                    }
                }
                reader.ReadEndElement();
            }
            else
                reader.Skip();

            return result;
        }


    }

}
