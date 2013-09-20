using Microsoft.AudienceInsight;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Windows;

namespace StandaloneSample
{
    public partial class MainPage : PhoneApplicationPage
    {
        BatchingLogAgent batchingLogAgent;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // Audience Insight config
            
            var batchingConfig = Microsoft.AudienceInsight.BatchingConfigFactory.Load(new Uri("/StandaloneSample;component/AudienceInsightConfig.xml", UriKind.Relative));

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