using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Windows.Storage.Streams;

namespace Microsoft.Media.Advertising
{
    public static class MastModelFactory
    {
        static XNamespace ns = "http://openvideoplayer.sf.net/mast";

        public static MAST CreateFromMast(IInputStream stream)
        {
            return CreateFromMast(stream.AsStreamForRead());
        }

        internal static MAST CreateFromMast(Stream stream)
        {
            XDocument xDoc = XDocument.Load(stream);

            XElement root = xDoc.Element(ns + "MAST");

            var result = new MAST();
            result.Triggers.AddRange(CreateTriggers(root, "triggers"));
            return result;
        }

        private static IEnumerable<Trigger> CreateTriggers(XElement element, string elementName)
        {
            var triggersXml = element.Element(ns + elementName);
            if (triggersXml != null)
            {
                return triggersXml.Elements(ns + "trigger").Select(CreateTrigger);
            }
            else
            {
                return Enumerable.Empty<Trigger>();
            }
        }

        private static Trigger CreateTrigger(XElement triggerXml)
        {
            var trigger = new Trigger();
            trigger.Description = GetAttribute(triggerXml.Attribute("description"));
            trigger.Id = GetAttribute(triggerXml.Attribute("id"));
            trigger.StartConditions.AddRange(CreateConditions(triggerXml, "startConditions"));
            trigger.EndConditions.AddRange(CreateConditions(triggerXml, "endConditions"));
            trigger.Sources.AddRange(CreateSources(triggerXml, "sources"));
            return trigger;
        }

        private static IEnumerable<Condition> CreateConditions(XElement element, string elementName)
        {
            var conditionsXml = element.Element(ns + elementName);
            if (conditionsXml != null)
            {
                return conditionsXml.Elements(ns + "condition").Select(CreateCondition);
            }
            else
            {
                return Enumerable.Empty<Condition>();
            }
        }

        private static Condition CreateCondition(XElement conditionXml)
        {
            var condition = new Condition();
            condition.Conditions.AddRange(CreateConditions(conditionXml, "conditions"));

            condition.Name = GetAttribute(conditionXml.Attribute("name"));
            condition.Operator = CreateOperator(conditionXml.Attribute("operator"));
            condition.Type = CreateConditionType(conditionXml.Attribute("type"));
            condition.Value = GetAttribute(conditionXml.Attribute("value"));
            return condition;
        }

        static string GetAttribute(XAttribute attributeXml)
        {
            return attributeXml != null ? attributeXml.Value : null;
        }

        static Operator? CreateOperator(XAttribute operatorXml)
        {
            if (operatorXml != null)
            {
                return (Operator)Enum.Parse(typeof(Operator), operatorXml.Value, true);
            }
            else return null;
        }

        static ConditionType? CreateConditionType(XAttribute conditionTypeXml)
        {
            if (conditionTypeXml != null)
            {
                return (ConditionType)Enum.Parse(typeof(ConditionType), conditionTypeXml.Value, true);
            }
            else return null;
        }

        private static IEnumerable<Source> CreateSources(XElement element, string elementName)
        {
            var sourcesXml = element.Element(ns + elementName);
            if (sourcesXml != null)
            {
                return sourcesXml.Elements(ns + "source").Select(CreateSource);
            }
            else
            {
                return Enumerable.Empty<Source>();
            }
        }

        private static Source CreateSource(XElement sourceXml)
        {
            var source = new Source();
            source.Sources.AddRange(CreateSources(sourceXml, "sources"));
            source.Targets.AddRange(CreateTargets(sourceXml, "targets"));
            source.Format = GetAttribute(sourceXml.Attribute("format"));
            source.AltReference = GetAttribute(sourceXml.Attribute("altReference"));
            source.Uri = GetAttribute(sourceXml.Attribute("uri"));
            return source;
        }

        private static IEnumerable<Target> CreateTargets(XElement element, string elementName)
        {
            var targetsXml = element.Element(ns + elementName);
            if (targetsXml != null)
            {
                return targetsXml.Elements(ns + "target").Select(CreateTarget);
            }
            else
            {
                return Enumerable.Empty<Target>();
            }
        }

        private static Target CreateTarget(XElement targetXml)
        {
            var target = new Target();
            target.Targets.AddRange(targetXml.Elements(ns + "target").Select(CreateTarget));
            target.Region = GetAttribute(targetXml.Attribute("region"));
            target.Type = GetAttribute(targetXml.Attribute("type"));
            return target;
        }
    }
}
