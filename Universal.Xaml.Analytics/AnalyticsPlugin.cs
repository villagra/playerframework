using Microsoft.Media.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Threading;
#else
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Core;
#endif

namespace Microsoft.PlayerFramework.Analytics
{
    /// <summary>
    /// A plugin used to retrieve and log analytics data.
    /// </summary>
    public class AnalyticsPlugin : IPlugin
    {
        /// <summary>
        /// Gets the area key to be applied to all analytic related tracking events.
        /// Tracking events without this area will be ignored by the analytics plugin.
        /// </summary>
        public static string TrackingEventArea { get { return "Analytics"; } }

        /// <summary>
        /// Creates a new instance of AnalyticsPlugin.
        /// </summary>
        public AnalyticsPlugin()
        {
            AnalyticsConfig = new AnalyticsConfig();
            MediaData = new Dictionary<string, object>();
            SessionData = new Dictionary<string, object>();
        }

        /// <summary>
        /// Creates a new instance of AnalyticsPlugin.
        /// </summary>
        /// <param name="analyticsConfig">The required analytics config object used to control what kind of analytics data should be collected.</param>
        public AnalyticsPlugin(AnalyticsConfig analyticsConfig)
        {
            AnalyticsConfig = analyticsConfig ?? new AnalyticsConfig();
            MediaData = new Dictionary<string, object>();
            SessionData = new Dictionary<string, object>();
        }

        AnalyticsCollector collector;
        MediaPlayerAdapter playerMonitor;

        /// <summary>
        /// Gets or sets the a Dispatcher to be used to poll CPU and FPS metrics. Set to null if one is not needed.
        /// </summary>
#if SILVERLIGHT
        public Dispatcher Dispatcher { get; set; }
#else
        public CoreDispatcher Dispatcher { get; set; }
#endif

        /// <summary>
        /// Gets or sets an object responsible for providing environment or system level data to log.
        /// </summary>
        public IEnvironmentMonitor EnvironmentMonitor { get; set; }

        /// <summary>
        /// Gets or sets an object responsible for providing information about the edge server used to serve the media.
        /// </summary>
        public IEdgeServerMonitor EdgeServerMonitor { get; set; }

        /// <summary>
        /// Gets or sets an object responsible for providing additional information about adaptive streaming.
        /// </summary>
        public IAdaptiveMonitor AdaptiveMonitor { get; set; }

        /// <summary>
        /// Gets or sets the required configuration object to help determine what information to collect, aggregate and log.
        /// </summary>
        public AnalyticsConfig AnalyticsConfig { get; set; }

        /// <summary>
        /// Gets the analytics collector object used by the analytics plugin to collect data
        /// </summary>
        public AnalyticsCollector AnalyticsCollector { get { return collector; } }

        /// <summary>
        /// Gets a collection of data to add to each log only for the current media. (e.g. 'VideoId')
        /// </summary>
        public IDictionary<string, object> MediaData { get; private set; }

        /// <summary>
        /// Gets a collection of data to add to each log across all media. (e.g. 'AppId')
        /// </summary>
        public IDictionary<string, object> SessionData { get; private set; }

        private void AddAdditionalData(IDictionary<string, object> data)
        {
            if (data != null)
            {
                foreach (var item in data)
                {
                    collector.AddtionalData.Add(item);
                }
            }
        }

        private void RemoveAdditionalData(IDictionary<string, object> data)
        {
            if (data != null)
            {
                foreach (var item in data)
                {
                    collector.AddtionalData.Remove(item);
                }
            }
        }

        /// <summary>
        /// Sends a log to the LoggingService but stamps it with session and media specific data first.
        /// </summary>
        /// <param name="log">The log to send</param>
        public void Log(ILog log)
        {
            collector.SendLog(log);
        }

        /// <inheritdoc /> 
        public void Load()
        {
            // by default, we always add the collector as a logging source
            collector = new AnalyticsCollector(AnalyticsConfig);
            collector.Dispatcher = Dispatcher;
            // add session specific data
            AddAdditionalData(SessionData);
            LoggingService.Current.LoggingSources.Add(collector);

            // initialize the collector. The AnalyticsCollector relies on other objects to pass it info.
            playerMonitor = new MediaPlayerAdapter(MediaPlayer);
            AddAdditionalData(MediaData);
            collector.Attach(playerMonitor, AdaptiveMonitor, EnvironmentMonitor, EdgeServerMonitor);
        }

        /// <inheritdoc /> 
        public void Update(IMediaSource mediaSource)
        {
            if (collector.IsAttached)
            {
                collector.Detach();
                // remove media specific data
                RemoveAdditionalData(MediaData);
                MediaData = null;
            }

            if (mediaSource != null)
            {
                // add media specific data
                MediaData = Analytics.GetAdditionalData(mediaSource as DependencyObject);
                AddAdditionalData(MediaData);
                collector.Attach(playerMonitor, AdaptiveMonitor, EnvironmentMonitor, EdgeServerMonitor);
            }
        }

        /// <inheritdoc /> 
        public void Unload()
        {
            collector.Detach();
            // remove session specific data
            RemoveAdditionalData(SessionData);
            collector.Dispatcher = null;
            collector = null;
            playerMonitor.MediaPlayer = null;
            playerMonitor = null;
        }

        /// <inheritdoc /> 
        public MediaPlayer MediaPlayer { get; set; }
    }
}
