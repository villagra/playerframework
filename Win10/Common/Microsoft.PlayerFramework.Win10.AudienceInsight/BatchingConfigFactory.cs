using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
#if SILVERLIGHT
#else
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

namespace Microsoft.AudienceInsight
{
    /// <summary>
    /// Provides a helper class used to deserialize xml into a BatchingConfig object.
    /// </summary>
    public static class BatchingConfigFactory
    {
        /// <summary>
        /// Deserializes Xml into a BatchingConfig object.
        /// </summary>
        /// <param name="source">The source URI of the config file.</param>
        /// <returns>An awaitable result.</returns>
        public static IAsyncOperation<BatchingConfig> Load(Uri source)
        {
            return AsyncInfo.Run(c => InternalLoad(source));
        }

        internal static async Task<BatchingConfig> InternalLoad(Uri source)
        {
            var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(source);
            using (var stream = await file.OpenStreamForReadAsync())
            {
                return Load(XmlReader.Create(stream));
            }
        }

        internal static BatchingConfig Load(XmlReader reader)
        {
            BatchingConfig result = null;
            IBatchAgent batchAgent = null;

            reader.GoToElement();
            if (reader.LocalName != "Configuration")
                throw new Exception("Invalid config Xml");
            reader.ReadStartElement();
            if (!reader.IsEmptyElement)
            {
                while (reader.GoToSibling())
                {
                    switch (reader.LocalName)
                    {
                        case "BatchingConfig":
                            result = BatchingConfig.Load(reader);
                            break;
                        case "Service":
                            batchAgent = BatchAgentFactory.Load(reader);
                            break;
                        default:
                            reader.Skip();
                            break;
                    }
                }
                reader.ReadEndElement();
            }
            else
                reader.Skip();

            if (result == null || batchAgent == null)
                throw new Exception("Invalid Configuraiton");
            result.BatchAgent = batchAgent;
            return result;
        }
    }
}
