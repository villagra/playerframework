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
            adLog.CurrentPosition = e.CurrentPosition;

            var creativeSource = e.CreativeSource;

            if (creativeSource != null)
            {
                adLog.MediaSource = creativeSource.MediaSource;
                adLog.CreativeExtraInfo = creativeSource.ExtraInfo;
                adLog.MediaSourceType = creativeSource.MediaSourceType;
                adLog.CreativeSourceType = creativeSource.Type;
                adLog.CreativeId = creativeSource.Id;
            }

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

        /// <inheritdoc /> 
        public IDictionary<string, object> GetData()
        {
            var result = this.CreateBasicLogData();
            result.Add("TrackingType", TrackingType);
            result.Add("MediaSource", MediaSource);
            result.Add("MediaSourceType", MediaSourceType);
            result.Add("CreativeExtraInfo", CreativeExtraInfo);
            result.Add("CreativeSourceType", CreativeSourceType);
            result.Add("CreativeId", CreativeId);
            result.Add("CurrentPosition", CurrentPosition);
            return result;
        }

        /// <inheritdoc /> 
        public Guid Id { get; private set; }

        /// <inheritdoc /> 
        public DateTimeOffset TimeStamp { get; set; }

        /// <inheritdoc /> 
        public string Type { get; private set; }

        /// <inheritdoc /> 
        public IDictionary<string, object> ExtraData { get; private set; }

        /// <summary>
        /// The type of ad tracking event that occurred.
        /// </summary>
        public TrackingType TrackingType { get; set; }

        /// <summary>
        /// The payload of the creative. This is usually a URL depending on the MediaSourceType but can also contain HTML.
        /// </summary>
        public string MediaSource { get; set; }

        /// <summary>
        /// Indicates what the MediaSource contains.
        /// </summary>
        public MediaSourceEnum MediaSourceType { get; set; }

        /// <summary>
        /// Additional information associated with the creative.
        /// </summary>
        public string CreativeExtraInfo { get; set; }

        /// <summary>
        /// Indicates how the creative is intended to be used.
        /// </summary>
        public CreativeSourceType CreativeSourceType { get; set; }

        /// <summary>
        /// The ID of the Ad Creative
        /// </summary>
        public string CreativeId { get; set; }

        /// <summary>
        /// The playback position of the main content.
        /// </summary>
        public TimeSpan CurrentPosition { get; set; }
    }
}
