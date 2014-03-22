using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
#if COMPRESSION
#if WINDOWS_PHONE7
using SharpCompress.Compressor.Deflate;
using SharpCompress.Compressor;
#else
using System.IO.Compression;
#endif
#endif
using System.Linq;
using System.Text;
using System.Xml;


namespace Microsoft.Media.AudienceInsight
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
        /// Gets the json string representation of the provided object
        /// </summary>
        /// <param name="value">The object to get the json string representation of</param>
        /// <returns>The json string representation of the provided object</returns>
        internal static string GetJsonValue(object value)
        {
            if (value.GetType() == typeof(bool))
                return (bool)value ? "1" : "0";
            else if (value.GetType() == typeof(TimeSpan))
                return ((TimeSpan)value).Ticks.ToString();
            else if (value.GetType() == typeof(DateTimeOffset))
                return ((DateTimeOffset)value).Ticks.ToString();
            else if (value.GetType() == typeof(Uri))
                return "\"" + ((Uri)value).OriginalString + "\"";
            else
                return "\"" + Convert.ToString(value, CultureInfo.InvariantCulture) + "\"";
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
        /// Creates a JsonObject from an IBatch, including all child logs.
        /// </summary>
        /// <param name="batch">The IBatch to create the JsonOBject from</param>
        /// <returns>A JsonObject representing the provided IBatch</returns>
        internal static string SerializeToJson(this IBatch batch)
        {
            List<string> batchProperties = new List<string>();

            foreach (var nvp in batch.GetData().Where(nvp => nvp.Value != null))
            {
                batchProperties.Add(nvp.Key + ":" + GetJsonValue(nvp.Value));
            }

            List<string> logStrings = new List<string>();

            foreach (var log in batch.Logs)
            {
                var logProperties = log.GetData()
                    .Where(nvp => nvp.Value != null)
                    .Select(nvp => nvp.Key + ":" + GetJsonValue(nvp.Value));

                logStrings.Add("{" + string.Join(",", logProperties) + "}");
            }

            var logsArrayString = "[" + string.Join(",", logStrings) +"]";

            batchProperties.Add(LogsArrayName + ":" + logsArrayString);

            var batchString = "{" + string.Join(",", batchProperties) + "}";

            return batchString;
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
                    keyValueStrings.Add(LogsArrayName + "[" + logNumber + "][" + nvp.Key + "]=" + Uri.EscapeDataString(nvp.Value.ToString()));
                }

                logNumber++;
            }

            return String.Join("&", keyValueStrings.ToArray());
        }


        // PoC
        //internal static JsonObject HttpQueryStringToJsonObject(this string httpQueryString)
        //{
        //    // Assumes property names with no index belong to parent batch

        //    JsonObject batch = new JsonObject();

        //    // use a dictionary here in case log data in the query string is not in order
        //    // allows log 3 to be created before log 1 (rather than using a List, etc)
        //    Dictionary<int, JsonObject> logs = new Dictionary<int, JsonObject>();

        //    string[] keyValuePairs = httpQueryString.Split('&');

        //    foreach (var kvp in keyValuePairs)
        //    {
        //        string[] keyAndValue = kvp.Split('=');

        //        var key = keyAndValue[0];
        //        var escapedValueString = keyAndValue[1];

        //        var unescapedValueString = Uri.UnescapeDataString(escapedValueString);

        //        if (key.StartsWith("logs["))
        //        {
        //            // add property to log
        //            int indexerMiddle = key.IndexOf("][");
                    
        //            string logNumberString = key.Substring("logs[".Length, indexerMiddle - "logs[".Length);
        //            int logNumber = int.Parse(logNumberString);

        //            string logPropertyName = key.Substring(indexerMiddle + 2, key.Length - (indexerMiddle + 2) - 1);

        //            if (!logs.ContainsKey(logNumber))
        //                logs.Add(logNumber, new JsonObject());

        //            logs[logNumber].Add(logPropertyName, JsonValue.CreateStringValue(unescapedValueString));
        //        }
        //        else
        //        {
        //            // add property to batch
        //            batch.Add(key, JsonValue.CreateStringValue(escapedValueString));
        //        }
        //    }

        //    JsonArray logsJson = new JsonArray();

        //    foreach (var key in logs.Keys)
        //        logsJson.Add(logs[key]);

        //    batch.Add("logs", logsJson);

        //    return batch;
        //}

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
#if COMPRESSION
        /// <summary>
        /// Serializes an IBatch to a XML and compresses it
        /// </summary>
        /// <param name="batch">The IBatch to serialize</param>
        /// <param name="stream">The stream to write the serialized and compressed data to</param>
        internal static void SerializeCompressedXml(this IBatch batch, Stream stream)
        {
            using (var gzipStream = new GZipStream(stream, CompressionMode.Compress, true))
            {
                batch.SerializeUncompressedXml(gzipStream);
                gzipStream.Flush();
            }
        }
#endif
        /// <summary>
        /// Serializes an IBatch to a JSON
        /// </summary>
        /// <param name="batch">The IBatch to serialize</param>
        /// <param name="stream">The stream to write the serialized data to</param>
        internal static void SerializeUncompressedJson(this IBatch batch, Stream stream)
        {
            TextWriter textWriter = new StreamWriter(stream);
            textWriter.Write(batch.SerializeToJson());
            textWriter.Flush();


            // Testing

            //var str = batch.ToHttpQueryString();
            //var jsonObject = str.HttpQueryStringToJsonObject();
            //var final = jsonObject.Stringify();
        }
#if COMPRESSION
        /// <summary>
        /// Serializes an IBatch to a JSON and compresses it
        /// </summary>
        /// <param name="batch">The IBatch to serialize</param>
        /// <param name="stream">The stream to write the serialized data to</param>
        internal static void SerializeCompressedJson(this IBatch batch, Stream stream)
        {
            using (var gzipStream = new GZipStream(stream, CompressionMode.Compress, true))
            {
                TextWriter textWriter = new StreamWriter(gzipStream);
                textWriter.Write(batch.SerializeToJson());
                textWriter.Flush();
                gzipStream.Flush();
            }
        }
#endif
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
            
            if (log.ExtraData != null)
            {
                foreach (var item in log.ExtraData)
                {
                    result.Add(item.Key, item.Value);
                }
            }

            if (!result.Keys.Contains("TimeStamp"))
                result.Add("TimeStamp", log.TimeStamp);

            if (!result.Keys.Contains("Type"))
                result.Add("Type", log.Type);

            if (!result.Keys.Contains("LogId"))
                result.Add("LogId", log.Id);

            return result;
        }
    }
}
