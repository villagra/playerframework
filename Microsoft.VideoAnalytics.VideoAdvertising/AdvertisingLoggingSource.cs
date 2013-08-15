using Microsoft.VideoAdvertising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VideoAnalytics.VideoAdvertising
{
    public sealed class AdvertisingLoggingSource
    {
        AnalyticsCollector analyticsCollector;

        public AdvertisingLoggingSource(IList<IAdPayloadHandler> adHandlers, AnalyticsCollector analyticsCollector)
        {
            this.analyticsCollector = analyticsCollector;
            
            foreach (var adHandler in adHandlers)
                adHandler.AdTrackingEventOccurred += handler_AdTrackingEventOccurred;
        }

        void handler_AdTrackingEventOccurred(object sender, AdTrackingEventEventArgs e)
        {
            var adLog = new AdEventLog();
            adLog.TrackingType = e.TrackingType;
            
            // TODO - potentially include an ad identifier here

            if (analyticsCollector != null) analyticsCollector.SendLog(adLog);
            else LoggingService.Current.Log(adLog);
        }
    }

    public sealed class AdEventLog : ILog
    {
        public AdEventLog()
        {
            TimeStamp = DateTimeOffset.Now;
            Type = "AdEvent";
            Id = Guid.NewGuid();
            ExtraData = new Dictionary<string, object>();
        }
        
        /// <summary>
        /// The type of ad tracking event that occurred.
        /// </summary>
        public TrackingType TrackingType { get; set; }

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
            result.Add("TrackingType", TrackingType);
            return result;
        }

        /// <inheritdoc /> 
        public IDictionary<string, object> ExtraData { get; private set; }
    }
}
