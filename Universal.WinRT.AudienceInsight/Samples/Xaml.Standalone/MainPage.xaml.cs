using Microsoft.AudienceInsight;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Xaml.Standalone
{
    public sealed partial class MainPage : Page
    {
        BatchingLogAgent batchingLogAgent;
        
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Audience Insight config
            
            var batchingConfig = await BatchingConfigFactory.Load(new Uri("ms-appx:///AudienceInsightConfig.xml"));

            var dataClient = (RESTDataClient)batchingConfig.BatchAgent;
            dataClient.AdditionalHttpHeaders.Add("Authorization-Token", "{2842C782-562E-4250-A1A2-F66D55B5EA15}");

            batchingLogAgent = new BatchingLogAgent(batchingConfig);
        }

        private void logButton_Click(object sender, RoutedEventArgs e)
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

    public class CustomLog : ILog
    {
        public CustomLog()
        {
            Type = "CustomLog";
            Id = Guid.NewGuid();
            ExtraData = new Dictionary<string, object>();
            TimeStamp = DateTimeOffset.Now;
        }

        public string CustomProperty { get; set; }
        public double CustomPropertyNumber { get; set; }
        public bool CustomPropertyBool { get; set; }

        public IDictionary<string, object> ExtraData { get; private set; }

        public IDictionary<string, object> GetData()
        {
            var data = this.CreateBasicLogData();
            data.Add("CustomProperty", CustomProperty);
            data.Add("CustomPropertyNumber", CustomPropertyNumber);
            data.Add("CustomPropertyBool", CustomPropertyBool);
            return data;
        }

        public Guid Id { get; private set; }
        public DateTimeOffset TimeStamp { get; set; }
        public string Type { get; private set; }
    }
}
