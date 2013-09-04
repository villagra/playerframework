using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.AudienceInsight;
using TempAudienceInsightDemo;

namespace TempAudienceInsightDemoWP7
{
    public partial class MainPage : PhoneApplicationPage
    {
        BatchingLogAgent batchingLogAgent;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var batchingConfig = Microsoft.AudienceInsight.BatchingConfigFactory.Load(new Uri("/TempAudienceInsightDemoWP7;component/AudienceInsightConfig.xml", UriKind.Relative));

            var dataClient = (RESTDataClient)batchingConfig.BatchAgent;
            dataClient.AdditionalHttpHeaders.Add("Authorization-Token", "{2842C782-562E-4250-A1A2-F66D55B5EA15}");

            batchingLogAgent = new Microsoft.AudienceInsight.BatchingLogAgent(batchingConfig);

            base.OnNavigatedTo(e);
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
    }
}