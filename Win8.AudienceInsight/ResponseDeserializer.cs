using System;
using System.IO;
using System.Xml;

namespace Microsoft.AudienceInsight
{
    internal class ResponseDeserializer
    {
        public static LogBatchResult Deserialize(Stream stream)
        {
            // parse the results
            using (XmlReader reader = XmlReader.Create(new StreamReader(stream, System.Text.Encoding.UTF8)))
            {
                return Deserialize(reader);
            }
        }

        public static LogBatchResult Deserialize(XmlReader reader)
        {
            LogBatchResult result = new LogBatchResult();

            reader.GoToElement();
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "LoggingEnabled":
                        result.IsEnabled = Convert.ToBoolean(reader.ReadContentAsInt());
                        break;
                    case "QueuePollingIntervalSeconds":
                            result.QueuePollingInterval = TimeSpan.FromSeconds(reader.ReadContentAsInt());
                        break;
                    case "ServerTime":
                        result.ServerTime = new DateTimeOffset(reader.ReadContentAsLong(), TimeSpan.Zero);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            return result;
        }
    }
}
