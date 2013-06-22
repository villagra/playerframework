using System.Text;
using Microsoft.Media.ISO.Boxes.Codecs.Data;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Base class for visual sequence samples
    /// </summary>
    public class VisualSampleEntryBox : SampleEntryBox
    {
        private ushort predefined1;
        private byte[] predefined2;
        private ushort predefined3;
        private uint reserved2;
        private ushort reserved1;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="VisualSampleEntryBox"/> class.
        ///// </summary>
        ///// <param name="offset">The offset in the stream where this box begins.</param>
        ///// <param name="size">The size of this box.</param>
        ///// <param name="boxType">Type of the box.</param>
        //public VisualSampleEntryBox(long offset, long size, BoxType boxType)
        //    : base(offset, size, boxType)
        //{

        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualSampleEntryBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public VisualSampleEntryBox(long offset, long size)
            : base(offset, size, BoxType.Vide)
        {

        }

        /// <summary>
        /// A name, for informative purposes. It is formatted in a fixed 32-byte field, with the first byte set to the number of bytes to be displayed, 
        /// followed by the number of bytes of displayable data, and then padding to complete 32 bytes total (including the size byte). The field may be set to 0.
        /// </summary>        
        public string CompressorName { get; private set; }


        /// <summary>
        /// Takes one of the following values:
        /// 	0x0018: Images are in color with no alpha
        /// </summary>
        public ushort Depth { get; private set; }

        /// <summary>
        /// Indicates how many frames of compressed video are stored in each sample. The default is 1 (one frame per sample). It may be more than 1 for multiple frames per sample.
        /// </summary>
        public ushort FrameCount { get; private set; }

        /// <summary>
        /// The maximum visual height of the stream are described by this sample description, in pixels.
        /// </summary>
        public ushort Height { get; private set; }

        /// <summary>
        /// Gives the horizontal resolution of the image in pixels-per-inch, as a fixed 16.16 number.
        /// </summary>
        public uint HorizResolution { get; private set; }

        /// <summary>
        /// Gives the vertical resolution of the image in pixels-per-inch, as a fixed 16.16 number.
        /// </summary>
        public uint VertResolution { get; private set; }

        /// <summary>
        /// The maximum visual width of the stream are described by this sample description, in pixels.
        /// </summary>
        public ushort Width { get; private set; }

        /// <summary>
        /// Gets the codec initialization data.
        /// </summary>
        public VideoTrackCodecData VideoCodecData { get; private set; }

        /// <summary>
        /// Reads the sample entry properties from stream.
        /// </summary>
        /// <param name="reader">The stream reader.</param>
        internal override void ReadSampleEntryPropertiesFromStream(BoxBinaryReader reader)
        {
            predefined1 = reader.ReadUInt16();
            reserved1 = reader.ReadUInt16();
            predefined2 = reader.ReadBytes(12);
            this.Width = reader.ReadUInt16();
            this.Height = reader.ReadUInt16();
            this.HorizResolution = reader.ReadUInt32();
            this.VertResolution = reader.ReadUInt32();
            reserved2 = reader.ReadUInt32();
            this.FrameCount = reader.ReadUInt16();

            byte[] buffer = new byte[0x20];
            reader.Read(buffer, 0, 0x20);
            int count = buffer[0];
            if (count < 0x20)
            {
                this.CompressorName = Encoding.UTF8.GetString(buffer, 1, count);
            }
            this.Depth = reader.ReadUInt16();
            predefined3 = reader.ReadUInt16();

            if (reader.PeekNextBoxType() != BoxType.Null)
            {
                ReadInnerBoxes(reader, BoxType.Esds, BoxType.Avcc, BoxType.Avc1, BoxType.Dvc1, BoxType.Btrt, BoxType.Sinf);
                this.VideoCodecData = GetVideoCodecDataFromInnerBoxes();
            }
            else
            {
                var videoInfoHeader = new VideoInfoHeader2(reader);
                this.VideoCodecData = new VideoTrackCodecData(videoInfoHeader);
            }

        }

        private VideoTrackCodecData GetVideoCodecDataFromInnerBoxes()
        {
            foreach (Box box in this.InnerBoxes)
            {
                switch (box.Type)
                {
                    case BoxType.Esds:
                        //return new AudioTrackCodecData(box as ElementaryStreamDescriptorFullBox);
                        break;
                    case BoxType.Avcc:
                        return new VideoTrackCodecData(((AdvancedVideoCodingBox)box), this.Width, this.Height);
                    case BoxType.Dvc1:
                        return new VideoTrackCodecData(((DigitalVideoCodingBox)box), this.Width, this.Height);
                    default:
                        break;
                }
            }
            return null;

        }

    }    
}
