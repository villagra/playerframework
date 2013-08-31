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

namespace TempAudienceInsightDemoPFWP8
{
    public partial class MainPage : PhoneApplicationPage
    {
        // TODO
        //AdvertisingLoggingSource advertisingLoggingSource;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            // Audience Insight config

            var batchingConfig = Microsoft.AudienceInsight.BatchingConfigFactory.Load(new Uri("/TempAudienceInsightDemoWP8;component/AudienceInsightConfig.xml", UriKind.Relative));

            var dataClient = (RESTDataClient)batchingConfig.BatchAgent;
            dataClient.AdditionalHttpHeaders.Add("Authorization-Token", "{2842C782-562E-4250-A1A2-F66D55B5EA15}");

            var batchinglogAgent = new Microsoft.AudienceInsight.BatchingLogAgent(batchingConfig);
            var aiLoggingTarget = new Microsoft.VideoAnalytics.AudienceInsight.AudienceInsightLoggingTarget(batchinglogAgent);

            Microsoft.VideoAnalytics.LoggingService.Current.LoggingTargets.Add(aiLoggingTarget);


            // Player Framework analytics config

            var analyticsConfig = Microsoft.VideoAnalytics.AnalyticsConfig.Load(new Uri("/TempAudienceInsightDemoWP8;component/AudienceInsightConfig.xml", UriKind.Relative));
            var analyticsPlugin = new AnalyticsPlugin(analyticsConfig);

            // TODO - this is Win8 code - translate to WP8
            //var adaptivePlugin = player.Plugins.OfType<AdaptivePlugin>().FirstOrDefault();
            //var adaptiveMonitorFactory = new AdaptiveMonitorFactory(adaptivePlugin.Manager);

            //var edgeServerMonitor = new Microsoft.VideoAnalytics.EdgeServerMonitor();

            //analyticsPlugin.AdaptiveMonitor = adaptiveMonitorFactory.AdaptiveMonitor;
            //analyticsPlugin.EdgeServerMonitor = edgeServerMonitor;

            player.Plugins.Add(analyticsPlugin);


            // Audience Insight ad tracking config

            //var analyticsCollector = player.Plugins.OfType<AnalyticsPlugin>().First().AnalyticsCollector;
            //advertisingLoggingSource = new AdvertisingLoggingSource(player.GetAdHandlerPlugin().AdHandlers, analyticsCollector);
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}