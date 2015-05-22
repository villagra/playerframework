using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Browser;
using System.Threading.Tasks;

namespace Microsoft.Media.AdaptiveStreaming.Dash
{
    internal static class WebRequestor
    {
        public static async Task<long> GetFileSizeAsync(Uri uri)
        {
            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(uri);
            request.Method = "HEAD";
            using (var response = await request.GetResponseAsync())
            {
                long contentLength;
                long.TryParse(response.Headers["Content-Length"], out contentLength);
                return contentLength;
            }
        }

        public static async Task<Stream> GetStreamRangeAsync(Uri uri, Range range)
        {
            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(uri);
            request.Method = "GET";
            if (range != null)
            {
                if (range.From.HasValue)
                {
                    request.AddRange((long)range.From.Value, (long)range.To.Value);
                }
                else if (range.To.HasValue)
                {
                    request.AddRange((long)range.To.Value);
                }
            }
            using (var response = await request.GetResponseAsync())
            {
                return GetStream(response);
            }
        }

        public static async Task<Stream> GetStreamRangeAsync(Uri uri, long from, long to)
        {
            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(uri);
            request.Method = "GET";
            request.AddRange(from, to);
            using (var response = await request.GetResponseAsync())
            {
                return GetStream(response);
            }
        }

        public static async Task<Stream> GetStreamRangeNoSuffixAsync(Uri uri, long range, long fileSize)
        {
            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(uri);
            request.Method = "GET";
            if (range < 0)
            {
                request.AddRange(fileSize + range, fileSize);
            }
            else
            {
                request.AddRange(range);
            }
            using (var response = await request.GetResponseAsync())
            {
                return GetStream(response);
            }
        }

        public static async Task<Stream> GetStreamRangeAsync(Uri uri, long range)
        {
            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(uri);
            request.Method = "GET";
            request.AddRange(range);
            using (var response = await request.GetResponseAsync())
            {
                return GetStream(response);
            }
        }
        
        public static async Task<WebRequestorResponse> GetResponseAsync(Uri uri)
        {
            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(uri);
            request.Method = "GET";
            using (var response = await request.GetResponseAsync())
            {
                return GetWebRequestorResponse(response);
            }
        }

        public static async Task<WebRequestorResponse> GetResponseAsync(Uri uri, long from, long to)
        {
            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(uri);
            request.Method = "GET";
            request.AddRange(from, to);
            using (var response = await request.GetResponseAsync())
            {
                return GetWebRequestorResponse(response);
            }
        }

        private static WebRequestorResponse GetWebRequestorResponse(HttpWebResponse result)
        {
            return new WebRequestorResponse(GetStream(result), result.StatusCode, result.Headers, result.ContentType);
        }

        private static Stream GetStream(HttpWebResponse result)
        {
            var memoryStream = new MemoryStream();
            result.GetResponseStream().CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        public class Range
        {
            public Range(ulong? from, ulong? to)
            {
                From = from;
                To = to;
            }

            public ulong? From { get; set; }
            public ulong? To { get; set; }

            public static Range FromString(string rangeString)
            {
                if (string.IsNullOrEmpty(rangeString)) return null;
                var range = rangeString.Split('-').Select(r => ulong.Parse(r)).ToArray();
                return new Range(range[0], range[1]);
            }
        }
    }
}
