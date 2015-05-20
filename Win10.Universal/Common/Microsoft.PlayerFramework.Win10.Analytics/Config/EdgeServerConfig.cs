using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// Defines the rules used to retrieve info about the edge server and client IP.
    /// </summary>
    public sealed class EdgeServerConfig
    {
        /// <summary>
        /// Creates a new instance of EdgeServerConfig.
        /// </summary>
        public EdgeServerConfig()
        {
            EdgeServerRulesCollection = new List<EdgeServerRules>();
        }

        /// <summary>
        /// Gets a collection of rules for how to determine what edge server is serving the media.
        /// </summary>
        public IList<EdgeServerRules> EdgeServerRulesCollection { get; private set; }

        /// <summary>
        /// Deserializes a config xml file containing the rules into an instance of EdgeServerConfig
        /// </summary>
        /// <param name="source">The source URI for the config file</param>
        /// <returns>An awaitable EdgeServerConfig</returns>
        public static IAsyncOperation<EdgeServerConfig> Load(Uri source)
        {
            return AsyncInfo.Run(c => InternalLoad(source));
        }

        internal static async Task<EdgeServerConfig> InternalLoad(Uri source)
        {
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(source);
            using (var stream = await file.OpenStreamForReadAsync())
            {
                return Load(XmlReader.Create(stream));
            }
        }

        internal static EdgeServerConfig Load(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            var result = new EdgeServerConfig();

            reader.GoToElement();
            reader.ReadStartElement();
            if (!reader.IsEmptyElement)
            {
                while (reader.GoToSibling())
                {
                    switch (reader.LocalName)
                    {
                        case "EdgeServerRules":
                            if (!reader.IsEmptyElement)
                            {
                                result.EdgeServerRulesCollection.Add(EdgeServerRules.Load(reader));
                            }
                            else
                                reader.Skip();
                            break;

                        default:
                            reader.Skip();
                            break;
                    }
                }
                reader.ReadEndElement();
            }
            else
                reader.Skip();

            return result;
        }
    }

    /// <summary>
    /// Represents the rules for one type of edge server.
    /// </summary>
    public sealed class EdgeServerRules
    {
        /// <summary>
        /// Creates a new instance of EdgeServerRules.
        /// </summary>
        public EdgeServerRules()
        {
            EdgeResolverHeaders = new List<string>();
        }

        /// <summary>
        /// Serves as a key to know which rule to select based on the current stream's domain. Rule is selected via the expression: currentStreamUri.Host.EndsWith(ai.Domain)
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Provides the Url for the request. If baseUri is supplied to EdgeServerDataClient, it will server as a pattern and resolve to an absolute url via the expression: string.Format(EdgeResolverUrlPattern, baseUri.Host, baseUri.Port)
        /// </summary>
        public string EdgeResolverUrlPattern { get; set; }

        /// <summary>
        /// An array of key value pairs to include in the request header. Array is expected to contain an even number of elements.
        /// </summary>
        public IList<string> EdgeResolverHeaders { get; private set; }

        /// <summary>
        /// Supplies the regular expression to be used to find the client IP in either the response body or response header. Set to null to ignore.
        /// </summary>
        public string ClientIPRegex { get; set; }

        /// <summary>
        /// Setting this will indicate that the specified response header includes the client IP (as opposed to the response body).
        /// </summary>
        public string ClientIPHeader { get; set; }

        /// <summary>
        /// Supplies the regular expression to be used to find the edge server in either the response body or response header. Set to null to ignore.
        /// </summary>
        public string EdgeRegex { get; set; }

        /// <summary>
        /// Setting this will indicate that the specified response header includes the edge server (as opposed to the response body).
        /// </summary>
        public string EdgeHeader { get; set; }

        /// <summary>
        /// Loads a new instance of the EdgeServerRules class from Xml.
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        internal static EdgeServerRules Load(XmlReader reader)
        {
            if (reader == null) throw new ArgumentNullException("reader");

            var result = new EdgeServerRules();

            reader.GoToElement();
            if (!reader.IsEmptyElement)
            {
                result.Domain = reader.GetAttribute("Domain");
                reader.ReadStartElement();
                while (reader.GoToSibling())
                {
                    switch (reader.LocalName)
                    {
                        case "EdgeResolverUrlPattern":
                            result.EdgeResolverUrlPattern = reader.ReadElementContentAsString();
                            break;
                        case "EdgeRegEx":
                            result.EdgeRegex = reader.ReadElementContentAsString();
                            break;
                        case "ClientIpRegEx":
                            result.ClientIPRegex = reader.ReadElementContentAsString();
                            break;
                        case "ClientIpHeader":
                            result.ClientIPHeader = reader.ReadElementContentAsString();
                            break;
                        case "EdgeResolverHeaders":
                            result.EdgeResolverHeaders = reader.ReadElementContentAsString().Split(',');
                            break;
                        case "EdgeHeader":
                            result.EdgeHeader = reader.ReadElementContentAsString();
                            break;
                        default:
                            reader.Skip();
                            break;
                    }
                }
                reader.ReadEndElement();
            }
            else reader.Skip();

            return result;
        }
    }
}
