using Microsoft.AudienceInsight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TempAudienceInsightDemo
{
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
