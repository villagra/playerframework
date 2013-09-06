using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TempAudienceInsightDemoPFWP8.Resources;
using Microsoft.AudienceInsight;
using Microsoft.PlayerFramework.Analytics;
using Microsoft.PlayerFramework.Adaptive;
using Microsoft.PlayerFramework.Adaptive.Analytics;
using Microsoft.VideoAnalytics.VideoAdvertising;
using Microsoft.PlayerFramework.Advertising;
using Microsoft.VideoAdvertising;
using Microsoft.VideoAnalytics;

namespace TempAudienceInsightDemoPFWP8
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
            // Audience Insight config

            var batchingConfig = Microsoft.AudienceInsight.BatchingConfigFactory.Load(new Uri("/TempAudienceInsightDemoPFWP8;component/AudienceInsightConfig.xml", UriKind.Relative));

            var dataClient = (Microsoft.AudienceInsight.RESTDataClient)batchingConfig.BatchAgent;
            dataClient.AdditionalHttpHeaders.Add("Authorization-Token", "{2842C782-562E-4250-A1A2-F66D55B5EA15}");

            var batchinglogAgent = new Microsoft.AudienceInsight.BatchingLogAgent(batchingConfig);
            var aiLoggingTarget = new Microsoft.VideoAnalytics.AudienceInsight.AudienceInsightLoggingTarget(batchinglogAgent);

            Microsoft.VideoAnalytics.LoggingService.Current.LoggingTargets.Add(aiLoggingTarget);
            
            // Player Framework analytics config

            var analyticsConfig = Microsoft.VideoAnalytics.AnalyticsConfig.Load(new Uri("/TempAudienceInsightDemoPFWP8;component/AudienceInsightConfig.xml", UriKind.Relative));
            var analyticsPlugin = new AnalyticsPlugin(analyticsConfig);

            var adaptivePlugin = player.Plugins.OfType<AdaptivePlugin>().FirstOrDefault();

            analyticsPlugin.AdaptiveMonitor = new AdaptiveMonitor(adaptivePlugin.SSME);
            analyticsPlugin.EdgeServerMonitor = new Microsoft.VideoAnalytics.EdgeServerMonitor();

            player.Plugins.Add(analyticsPlugin);

            // Audience Insight ad tracking config

            AdvertisingLoggingSource.Initialize(player.GetAdHandlerPlugin().AdHandlers, analyticsPlugin.AnalyticsCollector);
        }
    }
}