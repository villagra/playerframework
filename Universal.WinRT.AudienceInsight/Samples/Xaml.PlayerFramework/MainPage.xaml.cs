using Microsoft.AdaptiveStreaming.Analytics;
using Microsoft.AudienceInsight;
using Microsoft.PlayerFramework.Adaptive;
using Microsoft.PlayerFramework.Advertising;
using Microsoft.PlayerFramework.Analytics;
using Microsoft.VideoAnalytics;
using Microsoft.VideoAnalytics.AudienceInsight;
using Microsoft.VideoAnalytics.VideoAdvertising;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Xaml.PlayerFramework
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            player.Loaded += player_Loaded;
        }

        async void player_Loaded(object sender, RoutedEventArgs e)
        {
            var configFileUrl = new Uri("ms-appx:///AudienceInsightConfig.xml");
            
            // Audience Insight config

            var batchingConfig = await BatchingConfigFactory.Load(configFileUrl);

            var dataClient = (RESTDataClient)batchingConfig.BatchAgent;
            dataClient.AdditionalHttpHeaders.Add("Authorization-Token", "{2842C782-562E-4250-A1A2-F66D55B5EA15}");

            var batchinglogAgent = new BatchingLogAgent(batchingConfig);
            var aiLoggingTarget = new AudienceInsightLoggingTarget(batchinglogAgent);

            Microsoft.VideoAnalytics.LoggingService.Current.LoggingTargets.Add(aiLoggingTarget);

            // Player Framework analytics config

            var analyticsConfig = await AnalyticsConfig.Load(configFileUrl);
            var analyticsPlugin = new AnalyticsPlugin(analyticsConfig);

            var adaptivePlugin = player.Plugins.OfType<AdaptivePlugin>().FirstOrDefault();
            var adaptiveMonitorFactory = new AdaptiveMonitorFactory(adaptivePlugin.Manager);

            var edgeServerMonitor = new EdgeServerMonitor();

            analyticsPlugin.AdaptiveMonitor = adaptiveMonitorFactory.AdaptiveMonitor;
            analyticsPlugin.EdgeServerMonitor = edgeServerMonitor;

            player.Plugins.Add(analyticsPlugin);

            // Audience Insight ad tracking config

            analyticsPlugin.AnalyticsCollector.LoggingSources.Add(new AdvertisingLoggingSource(player.GetAdHandlerPlugin().AdHandlerController));

            // -or-

            //LoggingService.Current.LoggingSources.Add(new AdvertisingLoggingSource(player.GetAdHandlerPlugin().AdHandlerController));
        
        }
    }
}
