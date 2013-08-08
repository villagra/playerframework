using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using Windows.Data.Json;

namespace Microsoft.AudienceInsight
{
    /// <summary>
    /// Provides helper methods to act on logs.
    /// </summary>
    public static class LoggingExtensions
    {
        internal const string BatchNodeName = "batch";
        internal const string LogNodeName = "log";
        internal const string LogsArrayName = "logs";

        internal static string SerializeValue(object value)
        {
            if (value.GetType() == typeof(bool))
                return Convert.ToInt32((bool)value).ToString();
            else if (value.GetType() == typeof(TimeSpan))
                return ((TimeSpan)value).Ticks.ToString();
            else if (value.GetType() == typeof(DateTimeOffset))
                return ((DateTimeOffset)value).Ticks.ToString();
            else if (value.GetType() == typeof(Uri))
                return ((Uri)value).OriginalString;
            else
                return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        internal static JsonValue GetJsonValue(object value)
        {
            if (value.GetType() == typeof(bool))
                return JsonValue.CreateNumberValue(Convert.ToInt32((bool)value));
            else if (value.GetType() == typeof(TimeSpan))
                return JsonValue.CreateStringValue(((TimeSpan)value).Ticks.ToString());
            else if (value.GetType() == typeof(DateTimeOffset))
                return JsonValue.CreateStringValue(((DateTimeOffset)value).Ticks.ToString());
            else if (value.GetType() == typeof(Uri))
                return JsonValue.CreateStringValue(((Uri)value).OriginalString);
            else
                return JsonValue.CreateStringValue(Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Helper method for default serialization
        /// </summary>
        internal static void Serialize(this IBatch batch, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement(BatchNodeName);
            batch.SerializeData(xmlWriter);
            foreach (var log in batch.Logs)
            {
                xmlWriter.WriteStartElement(LogNodeName);
                log.SerializeData(xmlWriter);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();

            xmlWriter.Flush();
        }


        /// <summary>
        /// Serializes the current log to Xml
        /// </summary>
        /// <param name="log">The log to serialize</param>
        /// <param name="xmlWriter">The XmlWriter object you would like to store the log's values in</param>
        internal static void SerializeData(this ILog log, XmlWriter xmlWriter)
        {
            // write all named value pairs
            foreach (var nvp in log.GetData().Where(nvp => nvp.Value != null))
            {
                xmlWriter.WriteAttributeString(nvp.Key, SerializeValue(nvp.Value));
            }
        }


        internal static JsonObject CreateJsonObject(ILog log)
        {
            JsonObject jsonObject = new JsonObject();

            // write all named value pairs
            foreach (var nvp in log.GetData().Where(nvp => nvp.Value != null))
            {
                jsonObject[nvp.Key] = GetJsonValue(nvp.Value);
            }

            return jsonObject;
        }

        internal static JsonObject ToJsonObject(this IBatch batch)
        {
            JsonObject batchJson = CreateJsonObject(batch);
            JsonArray logsJson = new JsonArray();

            foreach (var log in batch.Logs)
                logsJson.Add(CreateJsonObject(log));

            batchJson[LogsArrayName] = logsJson;

            return batchJson;
        }

        internal static string ToHttpQueryString(this IBatch batch)
        {
            List<string> keyValueStrings = new List<string>();

            foreach (var nvp in batch.GetData().Where(nvp => nvp.Value != null))
            {
                keyValueStrings.Add(nvp.Key + "=" + Uri.EscapeUriString(nvp.Value.ToString()));
            }

            int logNumber = 0;

            foreach (var log in batch.Logs)
            {
                foreach (var nvp in log.GetData().Where(nvp => nvp.Value != null))
                {
                    keyValueStrings.Add(LogsArrayName + "[" + logNumber + "][" + nvp.Key + "]=" + Uri.EscapeUriString(nvp.Value.ToString()));
                }

                logNumber++;
            }

            return String.Join("&", keyValueStrings.ToArray());
        }

        internal static void SerializeUncompressed(this IBatch batch, Stream stream)
        {
            //StreamWriter outputStream = new StreamWriter(stream, System.Text.Encoding.UTF8);
            XmlWriter xmlWriter = XmlWriter.Create(stream);
            batch.Serialize(xmlWriter);
        }

        internal static void SerializeCompressed(this IBatch batch, Stream stream)
        {
            //using (var zipStream = new DeflateStream(stream, CompressionMode.Compress, true))
            using (var zipStream = new Ionic.Zlib.ZlibStream(stream, Ionic.Zlib.CompressionMode.Compress, true))
            {
                batch.SerializeUncompressed(zipStream);
                zipStream.Flush();
            }
        }

        internal static void SerializeUncompressedJson(this IBatch batch, Stream stream)
        {
            JsonObject batchJson = batch.ToJsonObject();
            TextWriter textWriter = new StreamWriter(stream);
            textWriter.Write(batchJson.Stringify());
            textWriter.Flush();
        }

        internal static void SerializeCompressedJson(this IBatch batch, Stream stream)
        {
            using (var zipStream = new Ionic.Zlib.ZlibStream(stream, Ionic.Zlib.CompressionMode.Compress, true))
            {
                JsonObject batchJson = batch.ToJsonObject();
                TextWriter textWriter = new StreamWriter(zipStream);
                textWriter.Write(batchJson.Stringify());
                textWriter.Flush();
                zipStream.Flush();
            }
        }

        internal static void SerializeHttpQueryString(this IBatch batch, Stream stream)
        {
            TextWriter textWriter = new StreamWriter(stream);
            textWriter.Write(batch.ToHttpQueryString());
            textWriter.Flush();
        }

        /// <summary>
        /// Gets only the required data for a log. Typically this is called from a log itself in the GetData method to serve as a starting point for the result that will ultimately be returned.
        /// </summary>
        /// <param name="log">The log to get basic data for.</param>
        /// <returns>A dictionary of data for the required properties on the log.</returns>
        public static IDictionary<string, object> CreateBasicLogData(this ILog log)
        {
            var result = new Dictionary<string, object>();
            result.Add("TimeStamp", log.TimeStamp);
            result.Add("Type", log.Type);
            result.Add("LogId", log.Id);
            if (log.ExtraData != null)
            {
                foreach (var item in log.ExtraData)
                {
                    result.Add(item.Key, item.Value);
                }
            }
            return result;
        }
    }
}
