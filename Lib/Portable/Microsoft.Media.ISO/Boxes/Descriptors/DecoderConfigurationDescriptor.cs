
namespace Microsoft.Media.ISO.Boxes.Descriptors
{
    /// <summary>
    /// Descriptor information for this stream decoder
    /// </summary>
    public class DecoderConfigurationDescriptor : Descriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecoderConfigurationDescriptor"/> class.
        /// </summary>
        /// <param name="headerSize">Size of the descriptor header.</param>
        /// <param name="bodySize">Size of the descriptor body.</param>
        public DecoderConfigurationDescriptor(uint headerSize, uint bodySize)
            : base(headerSize, bodySize, DescriptorTag.DECODER_CONFIG)
        {

        }


        /// <summary>
        /// Gets the average bitrate.
        /// </summary>
        public uint AverageBitrate { get; private set; }
        /// <summary>
        /// Gets the size of the buffer.
        /// </summary>
        public uint BufferSize { get; private set; }
        /// <summary>
        /// Gets the max bitrate.
        /// </summary>
        public uint MaxBitrate { get; private set; }
        /// <summary>
        /// Gets the object type indication.
        /// </summary>
        public DecoderObjectTypes ObjectTypeIndication { get; private set; }
        /// <summary>
        /// Gets the type of the stream.
        /// </summary>
        public byte StreamType { get; private set; }
        /// <summary>
        /// Gets a value indicating whether this is an up stream.
        /// </summary>
        public bool UpStream { get; private set; }


        /// <summary>
        /// Reads the descriptor properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadDescriptorPropertiesFromStream(BoxBinaryReader reader)
        {

            var initialOffset = reader.Offset;

            this.ObjectTypeIndication = (DecoderObjectTypes)reader.ReadByte();
            byte num = reader.ReadByte();
            this.StreamType = (byte)((num >> 2) & 0x3f);
            this.UpStream = (num & 2) != 0;
            this.BufferSize = reader.ReadUInt24();
            this.MaxBitrate = reader.ReadUInt32();
            this.AverageBitrate = reader.ReadUInt32();

            ReadSubDescriptors(reader, initialOffset);
        }


        /// <summary>
        /// Enumerates the several decoder object types
        /// </summary>
        public enum DecoderObjectTypes
        {
            /// <summary>
            /// Visual ISO/IEC 10918-1
            /// </summary>
            JPEG = 0x6c,
            /// <summary>
            /// Audio ISO/IEC 11172-3
            /// </summary>
            MPEG1_AUDIO = 0x6b,
            /// <summary>
            /// Visual ISO/IEC 11172-2
            /// </summary>
            MPEG1_VISUAL = 0x6a,
            /// <summary>
            /// Audio ISO/IEC 13818-7 LowComplexity Profile
            /// </summary>
            MPEG2_AAC_AUDIO_LC = 0x67,
            /// <summary>
            /// Audio ISO/IEC 13818-7 Main Profile
            /// </summary>
            MPEG2_AAC_AUDIO_MAIN = 0x66,
            /// <summary>
            /// Audio ISO/IEC 13818-7 Scaleable Sampling Rate Profile
            /// </summary>
            MPEG2_AAC_AUDIO_SSRP = 0x68,
            /// <summary>
            /// Audio ISO/IEC 13818-3
            /// </summary>
            MPEG2_PART3_AUDIO = 0x69,
            /// <summary>
            /// Visual ISO/IEC 13818-2 422 Profile
            /// </summary>
            MPEG2_VISUAL_422 = 0x65,
            /// <summary>
            /// Visual ISO/IEC 13818-2 High Profile
            /// </summary>
            MPEG2_VISUAL_HIGH = 0x64,
            /// <summary>
            /// Visual ISO/IEC 13818-2 Main Profile
            /// </summary>
            MPEG2_VISUAL_MAIN = 0x61,
            /// <summary>
            /// Visual ISO/IEC 13818-2 Simple Profile
            /// </summary>
            MPEG2_VISUAL_SIMPLE = 0x60,
            /// <summary>
            /// Visual ISO/IEC 13818-2 SNR Profile
            /// </summary>
            MPEG2_VISUAL_SNR = 0x62,
            /// <summary>
            /// Visual ISO/IEC 13818-2 Spatial Profile
            /// </summary>
            MPEG2_VISUAL_SPATIAL = 0x63,
            /// <summary>
            /// Audio ISO/IEC 14496-3
            /// </summary>
            MPEG4_AUDIO = 0x40,
            /// <summary>
            /// Systems ISO/IEC 14496-1
            /// </summary>
            MPEG4_SYSTEM = 1,
            /// <summary>
            /// Systems ISO/IEC 14496-1
            /// </summary>
            MPEG4_SYSTEM_COR = 2,
            /// <summary>
            /// Text Stream
            /// </summary>
            MPEG4_TEXT = 8,
            /// <summary>
            /// Visual ISO/IEC 14496-2
            /// </summary>
            MPEG4_VISUAL = 0x20
        }
    }
}
