using Microsoft.Media.ISO;
using Microsoft.Media.ISO.Boxes;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Microsoft.AdaptiveStreaming.Dash.Smooth
{
    internal static class Extensions
    {
        public static Stream ToStream(this SmoothStreamingMedia media)
        {
            var memStream = new MemoryStream();
            var settings = new XmlWriterSettings()
            {
                Indent = true, 
                Encoding = System.Text.Encoding.Unicode
            };

            XmlWriter writer = XmlWriter.Create(memStream, settings);
            media.WriteTo(writer);
            writer.Flush();

            memStream.Seek(0, SeekOrigin.Begin);

            return memStream;
        }

        public static void WriteTo(this SmoothStreamingMedia media, XmlWriter writer)
        {
            // The SmoothStreamingMedia element
            writer.WriteStartElement("SmoothStreamingMedia");
            writer.WriteAttributeString("MajorVersion", "2");
            writer.WriteAttributeString("MinorVersion", "1");
            writer.WriteAttributeString("Duration", media.Duration.ToString());
            if (media.TimeScale.HasValue) writer.WriteAttributeString("TimeScale", media.TimeScale.ToString());
            if (media.IsLive) writer.WriteAttributeString("IsLive", "true");
            if (media.LookaheadCount.HasValue) writer.WriteAttributeString("LookaheadCount", media.LookaheadCount.ToString());
            if (media.DVRWindowLength.HasValue) writer.WriteAttributeString("DVRWindowLength", media.DVRWindowLength.ToString());

            if (media.Protection != null)
            {
                // The Protection element
                writer.WriteStartElement("Protection");

                // The ProtectionHeader element
                writer.WriteStartElement("ProtectionHeader");
                writer.WriteAttributeString("SystemID", media.Protection.ProtectionHeader.SystemID);
                writer.WriteValue(media.Protection.ProtectionHeader.Value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }

            // The StreamIndex elements
            foreach (var streamIndex in media.StreamIndex)
            {
                // The StreamIndex element
                writer.WriteStartElement("StreamIndex");
                writer.WriteAttributeString("Type", streamIndex.Type);
                if (streamIndex.Subtype != null) writer.WriteAttributeString("Subtype", streamIndex.Subtype);
                writer.WriteAttributeString("Name", streamIndex.Name);
                writer.WriteAttributeString("Chunks", streamIndex.Chunks.ToString());
                writer.WriteAttributeString("QualityLevels", streamIndex.QualityLevels.ToString());
                if (streamIndex.TimeScale != 10000000)
                {
                    writer.WriteAttributeString("TimeScale", streamIndex.TimeScale.ToString());
                }

                switch (streamIndex.Type)
                {
                    case "audio":
                        break;
                    case "video":
                        writer.WriteAttributeString("MaxWidth", streamIndex.MaxWidth.ToString());
                        writer.WriteAttributeString("MaxHeight", streamIndex.MaxHeight.ToString());
                        writer.WriteAttributeString("DisplayWidth", streamIndex.DisplayWidth.ToString());
                        writer.WriteAttributeString("DisplayHeight", streamIndex.DisplayHeight.ToString());
                        break;
                    case "text":
                        writer.WriteAttributeString("Subtype", "SUBT");
                        break;
                }

                // These values may be empty for some StreamIndexes
                if (!string.IsNullOrEmpty(streamIndex.Url)) writer.WriteAttributeString("Url", streamIndex.Url);
                if (!string.IsNullOrEmpty(streamIndex.Language)) writer.WriteAttributeString("Language", streamIndex.Language);

                foreach (var qualityLevel in streamIndex.QualityLevel)
                {
                    // The QualityLevel element
                    writer.WriteStartElement("QualityLevel");

                    writer.WriteAttributeString("Index", qualityLevel.Index.ToString());
                    writer.WriteAttributeString("Bitrate", qualityLevel.Bitrate.ToString());

                    if (!string.IsNullOrEmpty(qualityLevel.FourCC)) writer.WriteAttributeString("FourCC", qualityLevel.FourCC.ToString());

                    switch (streamIndex.Type)
                    {
                        case "audio":
                            writer.WriteAttributeString("SamplingRate", qualityLevel.SamplingRate.ToString());
                            writer.WriteAttributeString("Channels", qualityLevel.Channels.ToString());
                            writer.WriteAttributeString("BitsPerSample", qualityLevel.BitsPerSample.ToString());
                            writer.WriteAttributeString("PacketSize", qualityLevel.PacketSize.ToString());
                            writer.WriteAttributeString("AudioTag", qualityLevel.AudioTag.ToString());
                            break;
                        case "video":
                            writer.WriteAttributeString("MaxWidth", qualityLevel.MaxWidth.ToString());
                            writer.WriteAttributeString("MaxHeight", qualityLevel.MaxHeight.ToString());
                            break;
                        case "text":
                            break;
                    }

                    if (!string.IsNullOrEmpty(qualityLevel.CodecPrivateData)) writer.WriteAttributeString("CodecPrivateData", qualityLevel.CodecPrivateData);

                    // Close the QualityLevel attribute
                    writer.WriteEndElement();
                }

                foreach (var chunk in streamIndex.c)
                {
                    // The c element
                    writer.WriteStartElement("c");

                    if (chunk.t.HasValue) writer.WriteAttributeString("t", chunk.t.ToString());
                    writer.WriteAttributeString("d", chunk.d.ToString());

                    // Close the c element
                    writer.WriteEndElement();
                }

                // Close the StreamIndex element
                writer.WriteEndElement();
            }

            // Close the SmoothStreamingMedia element
            writer.WriteEndElement();
        }
    }
}
