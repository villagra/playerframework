using System.Net;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// Contains the result of the edge server detection operation
    /// </summary>
    public sealed class EdgeServerResult
    {
        internal EdgeServerResult(string edgeServer, string clientIP)
        {
            EdgeServer = edgeServer;
            ClientIP = clientIP;
        }

        /// <summary>
        /// Gets the edge server address. This could be a domain name or IP address.
        /// </summary>
        public string EdgeServer { get; private set; }

        /// <summary>
        /// Gets the Client IP as returned by the edge server dataclient.
        /// </summary>
        public string ClientIP { get; private set; }

        /// <summary>
        /// Gets an empty EdgeServerResult
        /// </summary>
        public static EdgeServerResult Empty
        {
            get { return new EdgeServerResult(IpNA, IpNA); }
        }

        /// <summary>
        /// Represents the default IP Address
        /// </summary>
        public static string IpNA
        {
            get { return "255.255.255.255"; }
        }
    }
}
