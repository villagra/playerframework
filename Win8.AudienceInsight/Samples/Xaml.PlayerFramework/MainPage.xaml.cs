using Microsoft.AdaptiveStreaming.Analytics;
using Microsoft.AudienceInsight;
using Microsoft.PlayerFramework.Adaptive;
using Microsoft.PlayerFramework.Advertising;
using Microsoft.PlayerFramework.Analytics;
using Microsoft.VideoAnalytics.VideoAdvertising;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Xaml.PlayerFramework
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Audience Insight config

            var batchingConfig = await Microsoft.AudienceInsight.BatchingConfigFactory.Load(new Uri("ms-appx:///AudienceInsightConfig.xml"));

            var dataClient = (RESTDataClient)batchingConfig.BatchAgent;
            dataClient.AdditionalHttpHeaders.Add("Authorization-Token", "{2842C782-562E-4250-A1A2-F66D55B5EA15}");

            var batchinglogAgent = new Microsoft.AudienceInsight.BatchingLogAgent(batchingConfig);
            var aiLoggingTarget = new Microsoft.VideoAnalytics.AudienceInsight.AudienceInsightLoggingTarget(batchinglogAgent);

            Microsoft.VideoAnalytics.LoggingService.Current.LoggingTargets.Add(aiLoggingTarget);


            // Player Framework analytics config

            var analyticsConfig = await Microsoft.VideoAnalytics.AnalyticsConfig.Load(new Uri("ms-appx:///AudienceInsightConfig.xml"));
            var analyticsPlugin = new AnalyticsPlugin(analyticsConfig);

            var adaptivePlugin = player.Plugins.OfType<AdaptivePlugin>().FirstOrDefault();
            var adaptiveMonitorFactory = new AdaptiveMonitorFactory(adaptivePlugin.Manager);

            var edgeServerMonitor = new Microsoft.VideoAnalytics.EdgeServerMonitor();

            analyticsPlugin.AdaptiveMonitor = adaptiveMonitorFactory.AdaptiveMonitor;
            analyticsPlugin.EdgeServerMonitor = edgeServerMonitor;

            player.Plugins.Add(analyticsPlugin);


            // Audience Insight ad tracking config

            AdvertisingLoggingSource.Initialize(player.GetAdHandlerPlugin().AdHandlers, analyticsPlugin.AnalyticsCollector);
        }
    }
}
