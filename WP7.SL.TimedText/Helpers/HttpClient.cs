using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace System.Net.Http
{
    /// <summary>
    /// Used for compatibility with Win8
    /// </summary>
    internal class HttpClient : IDisposable
    {
        public async Task<Stream> GetStreamAsync(Uri address)
        {
            var request = WebRequest.CreateHttp(address);
            var response = await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);
            var stream = ((HttpWebResponse)response).GetResponseStream();
            var result = new MemoryStream();
            await stream.CopyToAsync(result);
            result.Seek(0, SeekOrigin.Begin);
            return result;
        }

        public void Dispose()
        {
            // do nothing, just here for backward compatibility
        }
    }
}
