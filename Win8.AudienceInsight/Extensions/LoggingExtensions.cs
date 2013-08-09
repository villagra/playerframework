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

        /// <summary>
        /// Gets the string representation of the provided object
        /// </summary>
        /// <param name="value">The object to get the string representation of</param>
        /// <returns>A string representation of the provided object</returns>
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

        /// <summary>
        /// Gets the JsonValue representation of the provided object
        /// </summary>
        /// <param name="value">The object to get the JsonValue representation of</param>
        /// <returns>The JsonValue representation of the provided object</returns>
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

        /// <summary>
        /// Creates a JsonObject from an ILog
        /// </summary>
        /// <param name="log">The log to create the JsonObject from</param>
        /// <returns>A JsonObject representing the ILog</returns>
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

        /// <summary>
        /// Creates a JsonObject from an IBatch, including all child logs.
        /// </summary>
        /// <param name="batch">The IBatch to create the JsonOBject from</param>
        /// <returns>A JsonObject representing the provided IBatch</returns>
        internal static JsonObject ToJsonObject(this IBatch batch)
        {
            JsonObject batchJson = CreateJsonObject(batch);
            JsonArray logsJson = new JsonArray();

            foreach (var log in batch.Logs)
                logsJson.Add(CreateJsonObject(log));

            batchJson[LogsArrayName] = logsJson;

            return batchJson;
        }

        /// <summary>
        /// Converts an IBatch to an HTTP query string
        /// </summary>
        /// <param name="batch">The IBatch to create the HTTP query string from</param>
        /// <returns></returns>
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

        /// <summary>
        /// Serializes an IBatch to a XML
        /// </summary>
        /// <param name="batch">The IBatch to serialize</param>
        /// <param name="stream">The stream to write the serialized data to</param>
        internal static void SerializeUncompressedXml(this IBatch batch, Stream stream)
        {
            XmlWriter xmlWriter = XmlWriter.Create(stream);
            batch.Serialize(xmlWriter);
        }

        /// <summary>
        /// Serializes an IBatch to a XML and compresses it
        /// </summary>
        /// <param name="batch">The IBatch to serialize</param>
        /// <param name="stream">The stream to write the serialized and compressed data to</param>
        internal static void SerializeCompressedXml(this IBatch batch, Stream stream)
        {
            using (var zipStream = new Ionic.Zlib.ZlibStream(stream, Ionic.Zlib.CompressionMode.Compress, true))
            {
                batch.SerializeUncompressedXml(zipStream);
                zipStream.Flush();
            }
        }

        /// <summary>
        /// Serializes an IBatch to a JSON
        /// </summary>
        /// <param name="batch">The IBatch to serialize</param>
        /// <param name="stream">The stream to write the serialized data to</param>
        internal static void SerializeUncompressedJson(this IBatch batch, Stream stream)
        {
            JsonObject batchJson = batch.ToJsonObject();
            TextWriter textWriter = new StreamWriter(stream);
            textWriter.Write(batchJson.Stringify());
            textWriter.Flush();
        }

        /// <summary>
        /// Serializes an IBatch to a JSON and compresses it
        /// </summary>
        /// <param name="batch">The IBatch to serialize</param>
        /// <param name="stream">The stream to write the serialized data to</param>
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

        /// <summary>
        /// Serializes an IBatch to an HTTP query string and writes it to the provided stream
        /// </summary>
        /// <param name="batch">The IBatch to serialize</param>
        /// <param name="stream">The stream to write the serialized data to</param>
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
