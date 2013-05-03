using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
#if SILVERLIGHT
#else
using System.Net.Http.Headers;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
#endif

namespace Microsoft.VideoAdvertising
{
    public static class Extensions
    {
        static Extensions()
        {
#if WINDOWS_PHONE
            var userAgentMask = "Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone {0}; Trident/6.0; IEMobile/10.0; Touch; {1}; {2})";
            DefaultUserAgent = string.Format(userAgentMask, Environment.OSVersion.Version, Microsoft.Phone.Info.DeviceStatus.DeviceManufacturer, Microsoft.Phone.Info.DeviceStatus.DeviceName);
#else
            DefaultUserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
#endif
        }

        public static string DefaultUserAgent { get; set; }

        internal static async Task<Stream> LoadStreamAsync(Uri source)
        {
#if NETFX_CORE || (WINDOWS_PHONE && !WINDOWS_PHONE7)
            switch (source.Scheme.ToLowerInvariant())
            {
                case "ms-appx":
                case "ms-appdata":
                    var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(source);
                    return await file.OpenStreamForReadAsync();
                default:
                    return await DownloadStreamAsync(source);
            }
#else
            return await DownloadStreamAsync(source);
#endif
        }

        internal static async Task PingAsync(Uri source)
        {
#if SILVERLIGHT
            using (var client = new HttpClient())
            {
                client.UserAgent = DefaultUserAgent;
                await client.GetStreamAsync(source);
            }
#else
            using (var client = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, source))
                {
                    if (DefaultUserAgent != null)
                    {
                        request.Headers.UserAgent.ParseAdd(DefaultUserAgent);
                    }
                    using (var response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }
            }
#endif
        }

        internal static async Task<Stream> DownloadStreamAsync(Uri source)
        {
#if SILVERLIGHT
            using (var client = new HttpClient())
            {
                client.UserAgent = DefaultUserAgent;
                return await client.GetStreamAsync(source);
            }
#else
            using (var client = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, source))
                {
                    if (DefaultUserAgent != null)
                    {
                        request.Headers.UserAgent.ParseAdd(DefaultUserAgent);
                    }
                    using (var response = await client.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        {
                            var result = new MemoryStream();
                            await stream.CopyToAsync(result);
                            result.Seek(0, SeekOrigin.Begin);
                            return result;
                        }
                    }
                }
            }
#endif
        }

        internal static Stream ToStream(this string source)
        {
            var result = new MemoryStream();
            var sw = new StreamWriter(result);
            sw.Write(source);
            result.Position = 0;
            return result;
        }

        internal static Task<TResult> Cast<TSource, TResult>(this Task<TSource> task)
            where TSource : TResult
            where TResult : class
        {
            return task.ContinueWith(t => t.Result as TResult);
        }

        internal static string SectionsReplace(this string source, string start, string end, string replace)
        {
            StringBuilder sb = new StringBuilder(512);
            int startIndex = 0;
            while (true)
            {
                int s = source.IndexOf(start, startIndex);
                if (s >= 0)
                {
                    int e = source.IndexOf(end, s + start.Length);
                    if (e >= 0)
                    {
                        sb.Append(source.Substring(startIndex, s - startIndex));
                        sb.Append(replace);
                        startIndex = e + end.Length;
                    }
                    else break;
                }
                else break;
            }
            if (startIndex == 0) return source; // optimization
            sb.Append(source.Substring(startIndex));
            return sb.ToString();
        }

        internal static string SectionReplace(this string source, string start, string end, string replace)
        {
            StringBuilder sb = new StringBuilder(512);
            int s = source.IndexOf(start);
            if (s >= 0)
            {
                s += start.Length;
                int e = source.IndexOf(end, s);
                if (e > s)
                {
                    sb.Append(source.Substring(0, s));
                    sb.Append(replace);
                    sb.Append(source.Substring(e));
                    return sb.ToString();
                }
            }
            return source;
        }
    }
}
