using Microsoft.AudienceInsight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public IDictionary<string, object> ExtraData { get; private set; }

        public IDictionary<string, object> GetData()
        {
            var data = new Dictionary<string, object>();
            data.Add("CustomProperty", CustomProperty);
            return data;
        }

        public Guid Id { get; private set; }
        public DateTimeOffset TimeStamp { get; set; }
        public string Type { get; private set; }
    }
}
