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
    /// Responsible for indicating which quality data should be tracked and aggregated. Used to filter data that will not be used at the earliest possible moment to improve performance.
    /// </summary>
    public sealed class QualityConfig
    {
        /// <summary>
        /// Gets or sets if this metric should be recorded.
        /// </summary>
        public bool DroppedFrames { get; set; }

        /// <summary>
        /// Gets or sets if this metric should be recorded.
        /// </summary>
        public bool RenderedFrames { get; set; }

        /// <summary>
        /// Gets or sets if this metric should be recorded.
        /// </summary>
        public bool ProcessCpuLoad { get; set; }

        /// <summary>
        /// Gets or sets if this metric should be recorded.
        /// </summary>
        public bool SystemCpuLoad { get; set; }

        /// <summary>
        /// Gets or sets if this metric should be recorded.
        /// </summary>
        public bool Bitrate { get; set; }

        /// <summary>
        /// Gets or sets if this metric should be recorded.
        /// </summary>
        public bool BitrateMax { get; set; }

        /// <summary>
        /// Gets or sets if this metric should be recorded.
        /// </summary>
        public bool BitrateMaxDuration { get; set; }

        /// <summary>
        /// Gets or sets if this metric should be recorded.
        /// </summary>
        public bool PerceivedBandwidth { get; set; }

        /// <summary>
        /// Gets or sets if this metric should be recorded.
        /// </summary>
        public bool VideoBufferSize { get; set; }

        /// <summary>
        /// Gets or sets if this metric should be recorded.
        /// </summary>
        public bool AudioBufferSize { get; set; }

        /// <summary>
        /// Gets or sets if this metric should be recorded.
        /// </summary>
        public bool Buffering { get; set; }

        /// <summary>
        /// Gets or sets if this metric should be recorded.
        /// </summary>
        public bool BitrateChangeCount { get; set; }

        /// <summary>
        /// Gets or sets if this metric should be recorded.
        /// </summary>
        public bool DvrOperationCount { get; set; }

        /// <summary>
        /// Gets or sets if this metric should be recorded.
        /// </summary>
        public bool FullScreenChangeCount { get; set; }

        /// <summary>
        /// Gets or sets if this metric should be recorded.
        /// </summary>
        public bool DownloadErrorCount { get; set; }

        /// <summary>
        /// Gets or sets if this metric should be recorded.
        /// </summary>
        public bool VideoDownloadLatency { get; set; }

        /// <summary>
        /// Gets or sets if this metric should be recorded.
        /// </summary>
        public bool AudioDownloadLatency { get; set; }

        /// <summary>
        /// Creates a new instance of QualityConfig.
        /// </summary>
        public QualityConfig()
        {
#if NETFX_CORE
            DroppedFrames = false;
            RenderedFrames = false;
            ProcessCpuLoad = false;
            SystemCpuLoad = false;
#else
            DroppedFrames = true;
            RenderedFrames = true;
            ProcessCpuLoad = true;
            SystemCpuLoad = true;
#endif
            Bitrate = true;
            BitrateMax = true;
            BitrateMaxDuration = true;
            PerceivedBandwidth = true;
            VideoBufferSize = true;
            AudioBufferSize = true;
            Buffering = true;
            BitrateChangeCount = true;
            VideoDownloadLatency = true;
            AudioDownloadLatency = true;
            DvrOperationCount = true;
            FullScreenChangeCount = true;
            DownloadErrorCount = true;
        }

        internal static QualityConfig Load(XmlReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            QualityConfig result = new QualityConfig();

            reader.GoToElement();
            if (!reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                while (reader.GoToSibling())
                {
                    switch (reader.LocalName)
                    {
                        case "DroppedFrames":
                            result.DroppedFrames = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "RenderedFrames":
                            result.RenderedFrames = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "ProcessCPULoad":
                            result.ProcessCpuLoad = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "SystemCPULoad":
                            result.SystemCpuLoad = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "Bitrate":
                            result.Bitrate = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "BitrateMax":
                            result.BitrateMax = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "BitrateMaxDuration":
                            result.BitrateMaxDuration = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "PerceivedBandwidth":
                            result.PerceivedBandwidth = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "VideoBufferSize":
                            result.VideoBufferSize = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "AudioBufferSize":
                            result.AudioBufferSize = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "Buffering":
                            result.Buffering = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "BitrateChangeCount":
                            result.BitrateChangeCount = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "VideoDownloadLatency":
                            result.VideoDownloadLatency = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "AudioDownloadLatency":
                            result.AudioDownloadLatency = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "DvrOperationCount":
                            result.DvrOperationCount = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "FullScreenChangeCount":
                            result.FullScreenChangeCount = Convert.ToBoolean(reader.ReadElementContentAsInt());
                            break;
                        case "HttpErrorCount":
                            result.DownloadErrorCount = Convert.ToBoolean(reader.ReadElementContentAsInt());
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

            return result;
        }
    }
}
