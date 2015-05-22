using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.Media.AudienceInsight
{
    /// <summary>
    /// Stores the rules for how to map key value pairs.
    /// Also implements methods to do the mappings.
    /// Also implements IFilter and uses these rules to tell the LoggingService which logs to filter out before they are passed onto the BatchingLogAgent.
    /// </summary>
    public sealed class MappingRules : ILogFilter
    {
        /// <summary>
        /// Raised when there is an error mapping data.
        /// </summary>
        public event EventHandler<BatchingErrorEventArgs> MappingError;

        /// <summary>
        /// Creates a new instance of MappingRules.
        /// </summary>
        public MappingRules()
        {
            LogMappingsRules = new List<LogMappings>();
        }

        /// <summary>
        /// Uses the rules to determine which logs to filter.
        /// </summary>
        /// <param name="log">The log that is going to be logged.</param>
        /// <returns>True indicates the log should be logged and not filtered.</returns>
        public bool IncludeLog(ILog log)
        {
            return LogMappingsRules.Any(l => l.Type == log.Type);
        }

        /// <summary>
        /// A dictionary of mappings.
        /// </summary>
        public IList<LogMappings> LogMappingsRules { get; private set; }

        /// <summary>
        /// Attempts to map a dictionary using the LogMappings
        /// </summary>
        /// <param name="log">The log to map</param>
        /// <returns>A dictionary of mapped key value pairs</returns>
        public IEnumerable<IDictionary<string, object>> Map(ILog log)
        {
            if (LogMappingsRules.Any(l => l.Type == log.Type))
            {
                var data = log.GetData();
                foreach (var typePolicy in LogMappingsRules.Where(l => l.Type == log.Type))
                {
                    bool skipPolicy = false;
                    if (typePolicy.Conditions != null)
                    {
                        // filtering based on condition is also available, a log must have a param set to a specific value. This is used to exclude the Snapshot quality logs
                        foreach (var condition in typePolicy.Conditions)
                        {
                            if (!data.ContainsKey(condition.Key)) skipPolicy = true;
                            else
                            {
                                var serializedValue = LoggingExtensions.SerializeValue(data[condition.Key]);
                                if (condition.Value == "*")
                                {
                                    skipPolicy = string.IsNullOrEmpty(serializedValue);
                                }
                                else
                                {
                                    skipPolicy = serializedValue != condition.Value;
                                }
                            }
                            if (skipPolicy) break;
                        }
                    }

                    if (!skipPolicy)
                    {
                        bool errorOccurred = false;
                        var output = new Dictionary<string, object>();
                        foreach (var policy in typePolicy.Values)
                        {
                            if (data.ContainsKey(policy.OriginalKey))
                            {
                                object value = data[policy.OriginalKey];
                                if (value != null)
                                {
                                    if (policy.OriginalKey == LogAttributes.Type)
                                    {
                                        if (typePolicy.SerializedId != null)
                                            output.Add(policy.NewKey, typePolicy.SerializedId);
                                    }
                                    else
                                    {
                                        output.Add(policy.NewKey, value);
                                    }
                                }
                                else if (!policy.Optional)
                                {
                                    // policy not found for the log value
                                    OnMappingException(new Exception(string.Format("Log property '{0}' must be set.", policy.OriginalKey)));
                                    errorOccurred = true;
                                }
                            }
                            else
                            {
                                // policy not found for the log value
                                OnMappingException(new Exception(string.Format("Log property '{0}' not found.", policy.OriginalKey)));
                                errorOccurred = true;
                            }
                        }
                        if (errorOccurred)
                            output.Add("err", "1");

                        yield return output;
                    }
                }
            }
            else
            {
                // policy not found for the log type
                OnMappingException(new Exception(string.Format("Mapping Rule not found for log type '{0}'", log.Type)));
            }
        }

        void OnMappingException(Exception ex)
        {
            if (MappingError != null) MappingError(this, new BatchingErrorEventArgs(ex));
        }

        /// <summary>
        /// Helper method to deserialize mapping rules form xml
        /// </summary>
        internal static MappingRules Load(XmlReader reader)
        {
            MappingRules result = new MappingRules();
            reader.ReadStartElement();
            while (reader.GoToSibling())
            {
                switch (reader.LocalName)
                {
                    case "Log":
                        var mapping = LogMappings.Load(reader);
                        result.LogMappingsRules.Add(mapping);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
            reader.ReadEndElement();

            return result;
        }
    }

    /// <summary>
    /// Contains the mapping rules for an individual log type
    /// </summary>
    public sealed class LogMappings
    {
        /// <summary>
        /// Gets or sets the type of log the mapping applies to.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the new type ID for the log.
        /// </summary>
        public string SerializedId { get; set; }

        /// <summary>
        /// Gets a dictionary of condiditions for which to apply the mapping rule.
        /// </summary>
        public IDictionary<string, string> Conditions { get; private set; }

        internal static LogMappings Load(XmlReader reader)
        {
            LogMappings result = new LogMappings();

            while (reader.MoveToNextAttribute())
            {
                switch (reader.Name)
                {
                    case "Type":
                        result.Type = reader.Value;
                        break;
                    case "Id":
                        result.SerializedId = reader.Value;
                        break;
                    default:
                        if (result.Conditions == null)
                            result.Conditions = new Dictionary<string, string>();
                        result.Conditions.Add(reader.Name, reader.Value);
                        break;
                }
            }

            if (!reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                result.Values = LoadValues(reader).ToList();
                reader.ReadEndElement();
            }
            else
            {
                result.Values = Enumerable.Empty<KeyValuePairMapping>().ToList();
                reader.Skip(); // no children exist, just move on
            }

            return result;
        }

        /// <summary>
        /// Gets a collection of rules for the key value pairs that can be included/mapped with a log.
        /// </summary>
        public IList<KeyValuePairMapping> Values { get; set; }

        internal static IEnumerable<KeyValuePairMapping> LoadValues(XmlReader reader)
        {
            while (reader.GoToSibling())
            {
                switch (reader.LocalName)
                {
                    case "Value":
                        yield return KeyValuePairMapping.Load(reader);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
        }
    }

    /// <summary>
    /// The mapping rule for an individual key value pair
    /// </summary>
    public sealed class KeyValuePairMapping
    {
        /// <summary>
        /// Gets or sets the original value ID.
        /// </summary>
        public string OriginalKey { get; set; }

        /// <summary>
        /// Gets or sets the new value ID to use in the serialized result.
        /// </summary>
        public string NewKey { get; set; }

        /// <summary>
        /// Gets or sets if the mapping is optional or if the entire log should not be sent if the key value pair is missing.
        /// </summary>
        public bool Optional { get; set; }

        internal static KeyValuePairMapping Load(XmlReader reader)
        {
            KeyValuePairMapping result = new KeyValuePairMapping();
            result.OriginalKey = reader.GetAttribute("Name");
            result.NewKey = reader.GetAttribute("Id");
            result.Optional = Convert.ToBoolean(Convert.ToInt32(reader.GetAttribute("Optional")));

            // advance the xml reader before departing
            reader.Skip();

            return result;
        }
    }

}
