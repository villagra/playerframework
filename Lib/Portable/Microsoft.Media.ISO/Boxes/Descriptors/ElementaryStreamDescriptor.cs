using System.Text;

namespace Microsoft.Media.ISO.Boxes.Descriptors
{
    /// <summary>
    /// Defines the stream descriptors used on an <see cref="ElementaryStreamDescriptorFullBox"/>
    /// </summary>
    public class ElementaryStreamDescriptor : Descriptor
    {
        /// <summary>
        /// Gets the descriptor identifier on which this descriptor depends on.
        /// </summary>
        public ushort DependsOn { get; private set; }
        /// <summary>
        /// Gets the elementary stream id.
        /// </summary>
        public ushort ElementaryStreamId { get; private set; }
        /// <summary>
        /// Gets the desctiptor flags.
        /// </summary>
        public uint Flags { get; private set; }
        /// <summary>
        /// Gets the ocr elementary stream id.
        /// </summary>
        public ushort OcrElementaryStreamId { get; private set; }
        /// <summary>
        /// Gets the stream priority.
        /// </summary>
        public byte StreamPriority { get; private set; }
        
        /// <summary>
        /// Gets the URL.
        /// </summary>
        public string Url { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ElementaryStreamDescriptor"/> class.
        /// </summary>
        /// <param name="headerSize">Size of the descriptor header.</param>
        /// <param name="bodySize">Size of the descriptor body.</param>
        public ElementaryStreamDescriptor(uint headerSize, uint bodySize)
            : base(headerSize, bodySize, DescriptorTag.ES)
        {            
        }

        /// <summary>
        /// Reads the descriptor properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadDescriptorPropertiesFromStream(BoxBinaryReader reader)
        {
            var initialOffset = reader.Offset;

            this.ElementaryStreamId = reader.ReadUInt16();
            byte num = reader.ReadByte();
            this.Flags = (uint)((num >> 5) & 7);
            this.StreamPriority = (byte)(num & 0x1f);

            if ((this.Flags & 1) != 0)
            {
                this.DependsOn = reader.ReadUInt16();
            }
            else
            {
                this.DependsOn = 0;
            }

            if ((this.Flags & 2) != 0)
            {
                byte count = reader.ReadByte();
                if (count != 0)
                {
                    byte[] buffer = new byte[count + 1];
                    reader.Read(buffer, 0, count);
                    buffer[count] = 0;
                    this.Url = Encoding.UTF8.GetString(buffer, 0, count);
                }
            }

            if ((this.Flags & 2) != 0)
            {
                this.OcrElementaryStreamId = reader.ReadUInt16();
            }
            else
            {
                this.OcrElementaryStreamId = 0;
            }

            ReadSubDescriptors(reader, initialOffset);
        }
    }
}
