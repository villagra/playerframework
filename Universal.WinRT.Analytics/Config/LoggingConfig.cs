using System;
using System.IO;
using System.Xml;
#if SILVERLIGHT
#else
using Windows.Storage.Streams;
#endif

namespace Microsoft.Media.Analytics
{
    /// <summary>
    /// Responsible for providing additional configuration data for the logging service.
    /// </summary>
    public sealed class LoggingConfig
    {
        internal static LoggingConfig Load(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            LoggingConfig result = new LoggingConfig();

            reader.GoToElement();
            if (!reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                while (reader.GoToSibling())
                {
                    switch (reader.LocalName)
                    {
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
}
