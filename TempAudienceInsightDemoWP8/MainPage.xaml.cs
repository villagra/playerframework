using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using TempAudienceInsightDemoWP8.Resources;
using Microsoft.AudienceInsight;
using TempAudienceInsightDemo;

namespace TempAudienceInsightDemoWP8
{
    public partial class MainPage : PhoneApplicationPage
    {
        BatchingLogAgent batchingLogAgent;
        
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

            var batchingConfig = Microsoft.AudienceInsight.BatchingConfigFactory.Load(new Uri("/TempAudienceInsightDemoWP8;component/AudienceInsightConfig.xml", UriKind.Relative));

            var dataClient = (RESTDataClient)batchingConfig.BatchAgent;
            dataClient.AdditionalHttpHeaders.Add("Authorization-Token", "{2842C782-562E-4250-A1A2-F66D55B5EA15}");

            batchingLogAgent = new Microsoft.AudienceInsight.BatchingLogAgent(batchingConfig);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var customLog = new CustomLog()
            {
                CustomProperty = "testing",
                CustomPropertyNumber = 3.14159,
                CustomPropertyBool = true
            };

            batchingLogAgent.LogEntry(customLog);
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