using System;
using System.Xml;

namespace Microsoft.AudienceInsight
{
    internal class BatchAgentFactory
    {
        public static IBatchAgent Load(XmlReader reader)
        {
            IBatchAgent result = null;

            reader.GoToElement();

            if (!reader.IsEmptyElement)
            {
                string url = null;
                SerializationFormat serializationFormat = SerializationFormat.Unknown;
                bool isRelative = false;
                bool isWCF = true;
                bool binaryBinding = false;
                bool compress = false;
                int maxOpenTimeSeconds = 5;
                int maxSendTimeSeconds = 5;
                int maxRecvTimeSeconds = 5;
                int serviceVersion = 3;

                reader.ReadStartElement();
                while (reader.GoToSibling())
                {
                    switch (reader.LocalName)
                    {
                        case "Url":
                            url = reader.ReadElementContentAsString();
                            break;
                        case "WCF":
                            isWCF = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "relative":
                            isRelative = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "binaryBinding":
                            binaryBinding = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "maxOpenTimeSeconds":
                            maxOpenTimeSeconds = reader.ReadElementContentAsInt();
                            break;
                        case "maxSendTimeSeconds":
                            maxSendTimeSeconds = reader.ReadElementContentAsInt();
                            break;
                        case "maxRecvTimeSeconds":
                            maxRecvTimeSeconds = reader.ReadElementContentAsInt();
                            break;
                        case "version":
                            serviceVersion = reader.ReadElementContentAsInt();
                            break;
                        case "compression":
                            compress = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "serializationFormat":

                            var value = reader.ReadElementContentAsString();

                            if (value.Equals("xml", StringComparison.OrdinalIgnoreCase))
                                serializationFormat = SerializationFormat.Xml;
                            else if (value.Equals("json", StringComparison.OrdinalIgnoreCase))
                                serializationFormat = SerializationFormat.Json;
                            else if (value.Equals("httpquerystring", StringComparison.OrdinalIgnoreCase))
                                serializationFormat = SerializationFormat.HttpQueryString;

                            break;
                        default:
                            reader.Skip();
                            break;
                    }
                }
                reader.ReadEndElement();

                // actually create the Uri
                Uri serviceUri = null;
                if (url != null)
                {
                    if (isRelative)
                        throw new NotImplementedException();
                    else
                        serviceUri = new Uri(url);
                }

                if (isWCF)
                {
                    throw new NotImplementedException();
                    //TODO: result = new WCFDataClient(serviceUri, binaryBinding, maxOpenTimeSeconds, maxSendTimeSeconds, maxRecvTimeSeconds, serviceVersion);
                }
                else
                {
                    result = new RESTDataClient(serviceUri, maxSendTimeSeconds + maxRecvTimeSeconds, compress, serializationFormat, serviceVersion);
                }
            }
            else
                reader.Skip();

            return result;
        }
    }
}
