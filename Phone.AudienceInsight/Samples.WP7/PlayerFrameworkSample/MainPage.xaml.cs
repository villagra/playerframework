using Microsoft.AudienceInsight;
using Microsoft.Phone.Controls;
using Microsoft.PlayerFramework.Adaptive;
using Microsoft.PlayerFramework.Adaptive.Analytics;
using Microsoft.PlayerFramework.Advertising;
using Microsoft.PlayerFramework.Analytics;
using Microsoft.VideoAnalytics;
using Microsoft.VideoAnalytics.AudienceInsight;
using Microsoft.VideoAnalytics.VideoAdvertising;
using System;
using System.Linq;
using System.Windows;

namespace PlayerFrameworkSample
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            player.Loaded += player_Loaded;
        }

        void player_Loaded(object sender, RoutedEventArgs e)
        {
            var configFileUrl = new Uri("/PlayerFrameworkSample;component/AudienceInsightConfig.xml", UriKind.Relative);

            // Audience Insight config

            var batchingConfig = BatchingConfigFactory.Load(configFileUrl);

            // Add custom header(s)

            var dataClient = (RESTDataClient)batchingConfig.BatchAgent;
            dataClient.AdditionalHttpHeaders.Add("Authorization-Token", "{2842C782-562E-4250-A1A2-F66D55B5EA15}");

            var batchinglogAgent = new BatchingLogAgent(batchingConfig);
            var aiLoggingTarget = new AudienceInsightLoggingTarget(batchinglogAgent);

            Microsoft.VideoAnalytics.LoggingService.Current.LoggingTargets.Add(aiLoggingTarget);

            // Player Framework analytics config

            var analyticsConfig = AnalyticsConfig.Load(configFileUrl);
            var analyticsPlugin = new AnalyticsPlugin(analyticsConfig);

            var adaptivePlugin = player.Plugins.OfType<AdaptivePlugin>().FirstOrDefault();

            analyticsPlugin.AdaptiveMonitor = new AdaptiveMonitor(adaptivePlugin.SSME);
            analyticsPlugin.EdgeServerMonitor = new EdgeServerMonitor();

            player.Plugins.Add(analyticsPlugin);

            // Audience Insight ad tracking config

            analyticsPlugin.AnalyticsCollector.LoggingSources.Add(new AdvertisingLoggingSource(player.GetAdHandlerPlugin().AdHandlerController));

            // -or-

            //LoggingService.Current.LoggingSources.Add(new AdvertisingLoggingSource(player.GetAdHandlerPlugin().AdHandlerController));
        }
    }
}