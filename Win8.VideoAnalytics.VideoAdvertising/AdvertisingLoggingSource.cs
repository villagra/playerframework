using Microsoft.VideoAdvertising;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VideoAnalytics.VideoAdvertising
{
    public sealed class AdvertisingLoggingSource : ILoggingSource
    {
        public event EventHandler<LogEventArgs> LogCreated;

        public AdvertisingLoggingSource(AdHandlerController adHandlerController)
        {
            adHandlerController.AdTrackingEventOccurred += adHandlerController_AdTrackingEventOccurred;
        }

        void adHandlerController_AdTrackingEventOccurred(object sender, AdTrackingEventEventArgs e)
        {
            if (LogCreated != null)
            {
                var adLog = CreateAdLog(e);

                LogCreated(this, new LogEventArgs(adLog));
            } 
        }

        AdEventLog CreateAdLog(AdTrackingEventEventArgs e)
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

            return adLog;
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
