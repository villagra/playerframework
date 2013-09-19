using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace MediaRSS
{
    public class MRssMediaFactory
    {
        public static async Task<IEnumerable<MediaItem>> Load(Uri source)
        {
            string xml;
            switch (source.Scheme.ToLowerInvariant())
            {
                case "ms-appx":
                case "ms-appdata":
                    var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(source);
                    using (var stream = await file.OpenStreamForReadAsync())
                    {
                        xml = new StreamReader(stream).ReadToEnd();
                    }
                    break;
                default:
                    var client = new HttpClient();
                    xml = await client.GetStringAsync(source);
                    break;
            }

            XDocument doc = XDocument.Parse(xml);
            return doc.Element("rss").Element("channel").Elements("item").Select(i => MRssMediaFactory.FromMRss(i));
        }

        static MediaItem FromMRss(XElement el)
        {
            XNamespace mrss = "http://search.yahoo.com/mrss/";

            MediaItem result = new MediaItem();
            if (el.Element("title") != null)
            {
                result.Title = el.Element("title").Value;
            }
            if (el.Element("description") != null)
            {
                result.Description = el.Element("description").Value;
            }
            var media = el.Element(mrss + "content");
            result.Source = new Uri(media.Attribute("url").Value, UriKind.Absolute);
            if (media.Attribute("duration") != null)
            {
                result.Duration = TimeSpan.FromSeconds(double.Parse(media.Attribute("duration").Value));
            }
            var thumb = media.Element(mrss + "thumbnail");
            if (thumb != null)
            {
                result.Thumbnail = new Uri(thumb.Attribute("url").Value, UriKind.Absolute);
            }
            return result;
        }
    }
}
