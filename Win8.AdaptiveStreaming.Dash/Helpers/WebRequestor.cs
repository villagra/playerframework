using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.AdaptiveStreaming.Dash
{
    internal static class WebRequestor
    {
        public static async Task<long> GetFileSizeAsync(Uri uri)
        {
            using (var httpClient = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, uri))
                {
                    using (HttpResponseMessage response = await httpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return response.Content.Headers.ContentLength.GetValueOrDefault(0);
                        }
                        else
                        {
                            throw new WebRequestorFailure(response.StatusCode, response.Headers);
                        }
                    }
                }
            }
        }

        public static async Task<Stream> GetStreamRangeAsync(Uri uri, Range range)
        {
            using (var httpClient = new HttpClient())
            {
                if (range != null)
                {
                    if (range.From.HasValue) httpClient.AddRange((long)range.From.Value, (long)range.To.Value);
                    else if (range.To.HasValue) httpClient.AddRange((long)range.To.Value);
                }
                return (await httpClient.GetResponse(uri)).Stream;
            }
        }

        public static async Task<Stream> GetStreamRangeAsync(Uri uri, long from, long to)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.AddRange(from, to);
                return (await httpClient.GetResponse(uri)).Stream;
            }
        }

        public static async Task<Stream> GetStreamRangeNoSuffixAsync(Uri uri, long range, long fileSize)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.AddRange(range);
                if (range < 0)
                { 
                    return await GetStreamRangeAsync(uri, fileSize + range, fileSize);
                }
                else
                {
                    return (await httpClient.GetResponse(uri)).Stream;
                }
            }
        }

        public static async Task<Stream> GetStreamRangeAsync(Uri uri, long range)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.AddRange(range);
                return (await httpClient.GetResponse(uri)).Stream;
            }
        }

        public static async Task<WebRequestorResponse> GetResponseAsync(Uri uri)
        {
            using (var httpClient = new HttpClient())
            {
                return await httpClient.GetResponse(uri);
            }
        }

        public static async Task<WebRequestorResponse> GetResponseAsync(Uri uri, long from, long to)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.AddRange(from, to);
                return await httpClient.GetResponse(uri);
            }
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
