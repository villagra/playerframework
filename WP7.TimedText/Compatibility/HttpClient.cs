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
            request.AllowReadStreamBuffering = true;
            var response = await Task.Factory.FromAsync<WebResponse>(request.BeginGetResponse, request.EndGetResponse, null);
            return response.GetResponseStream();
        }

        public async Task<byte[]> GetByteArrayAsync(string address)
        {
            using (var stream = await GetStreamAsync(new Uri(address)))
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        public void Dispose()
        {
            // do nothing
        }
    }
}
