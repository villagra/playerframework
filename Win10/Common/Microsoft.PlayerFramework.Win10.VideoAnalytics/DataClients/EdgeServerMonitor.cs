using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// Responsible for hitting another url to retrieve information about the edge server and client IP address.
    /// Request is built and response is parsed according to rules defined in EdgeServerRules
    /// </summary>
    public sealed class EdgeServerMonitor : IEdgeServerMonitor
    {
        /// <summary>
        /// Gets a collection of EdgeServerRules
        /// </summary>
        public IEnumerable<EdgeServerRules> EdgeServerRuleCollection { get; private set; }

        /// <summary>
        /// Creates a new instance of EdgeServerMonitor.
        /// </summary>
        public EdgeServerMonitor()
        {
            EdgeServerRuleCollection = new List<EdgeServerRules>();
        }

        /// <summary>
        /// Creates a new instance of EdgeServerMonitor.
        /// </summary>
        /// <param name="config">The required configuration object for the monitor</param>
        public EdgeServerMonitor(EdgeServerConfig config)
        {
            EdgeServerRuleCollection = config.EdgeServerRulesCollection;
        }
        /// <inheritdoc /> 
        public IAsyncOperation<EdgeServerResult> GetEdgeServerAsync(Uri currentStreamUri)
        {
            return AsyncInfo.Run(c => GetEdgeServerAsync(currentStreamUri, c));
        }
        
        internal async Task<EdgeServerResult> GetEdgeServerAsync(Uri currentStreamUri, CancellationToken cancellationToken)
        {
            if (EdgeServerRuleCollection != null)
            {
                var edgeServerRules = EdgeServerRuleCollection.FirstOrDefault(ai => ai.Domain != null && currentStreamUri.Host.EndsWith(ai.Domain, StringComparison.OrdinalIgnoreCase));
                // fallback on the address rules without a domain
                if (edgeServerRules == null)
                {
                    edgeServerRules = EdgeServerRuleCollection.FirstOrDefault(ai => ai.Domain == null);
                }
                if (edgeServerRules != null)
                {
                    Uri ipRequestUri;
                    if (currentStreamUri != null)
                        ipRequestUri = new Uri(string.Format(CultureInfo.InvariantCulture, edgeServerRules.EdgeResolverUrlPattern, currentStreamUri.Host, currentStreamUri.Port), UriKind.Absolute);
                    else
                        ipRequestUri = new Uri(edgeServerRules.EdgeResolverUrlPattern, UriKind.Absolute);

					// use the client networking stack so we can read headers
					var request = WebRequest.CreateHttp(ipRequestUri);
					for (int i = 0; i < edgeServerRules.EdgeResolverHeaders.Count; i = i + 2)
                    {
                        string key = edgeServerRules.EdgeResolverHeaders[i];
                        string value = edgeServerRules.EdgeResolverHeaders[i + 1];
                        request.Headers[key] = value;
                    }
                    request.Method = "GET";
                    var response = await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);
                    cancellationToken.ThrowIfCancellationRequested();

                    string result;
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }
                    string thingToParse;

                    // get the edge server
                    string edgeServer = "";
                    thingToParse = result;
                    if (edgeServerRules.EdgeHeader != null)
                    {
                        if (response.SupportsHeaders)
                            thingToParse = response.Headers[edgeServerRules.EdgeHeader].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    }
                    if (edgeServerRules.EdgeRegex != null && thingToParse != null)
                    {
                        Regex regex = new Regex(edgeServerRules.EdgeRegex);
                        if (regex.IsMatch(thingToParse))
                        {
                            var matches = regex.Matches(thingToParse);
                            edgeServer = matches[0].Value;
                        }
                    }

                    // get the client IP
                    string clientIP = EdgeServerResult.IpNA;
                    thingToParse = result;
                    if (edgeServerRules.ClientIPHeader != null)
                    {
                        if (response.SupportsHeaders)
                            thingToParse = response.Headers[edgeServerRules.ClientIPHeader];
                    }
                    if (edgeServerRules.ClientIPRegex != null && thingToParse != null)
                    {
                        Regex regex = new Regex(edgeServerRules.ClientIPRegex);
                        if (regex.IsMatch(thingToParse))
                        {
                            var matches = regex.Matches(thingToParse);
                            clientIP = matches[0].Value;
                        }
                    }

                    return new EdgeServerResult(edgeServer, clientIP);
                }
            }
            return null;
        }
    }
}
