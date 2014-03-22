using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.PlayerFramework.TimedText
{
    internal static class Extensions
    {
        public static async Task<Stream> LoadToStream(this Uri source)
        {
#if NETFX_CORE || (WINDOWS_PHONE && !WINDOWS_PHONE7)
            switch (source.Scheme.ToLowerInvariant())
            {
                case "ms-appx":
                case "ms-appdata":
                    var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(source);
                    return await file.OpenStreamForReadAsync();
                default:
                    using (var client = new HttpClient())
                    {
                        using (var stream = await client.GetStreamAsync(source))
                        {
                            var result = new MemoryStream();
                            await stream.CopyToAsync(result);
                            result.Seek(0, SeekOrigin.Begin);
                            return result;
                        }
                    }
            }
#else
            using (var client = new HttpClient())
            {
                return await client.GetStreamAsync(source);
            }
#endif
        }

        public static async Task<string> LoadToString(this Uri source)
        {
            using (var stream = await source.LoadToStream())
            {
                return new StreamReader(stream).ReadToEnd();
            }
        }
    }
}
