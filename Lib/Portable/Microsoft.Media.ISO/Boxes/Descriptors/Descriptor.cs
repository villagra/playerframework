using System.Collections.Generic;

namespace Microsoft.Media.ISO.Boxes.Descriptors
{
    /// <summary>
    /// Defines the base class for track descriptors.
    /// </summary>
    public abstract class Descriptor
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Descriptor"/> class.
        /// </summary>
        /// <param name="headerSize">Size of the descriptor header.</param>
        /// <param name="payloadSize">Size of the descriptor body.</param>
        /// <param name="tag">The descriptor tag.</param>
        public Descriptor(uint headerSize, uint payloadSize, DescriptorTag tag)
        {
            this.HeaderSize = headerSize;
            this.PayloadSize = payloadSize;
            this.Tag = tag;
            this.SubDescriptors = new List<Descriptor>();
        }

        /// <summary>
        /// Gets the size of the header.
        /// </summary>
        public uint HeaderSize { get; private set; }

        /// <summary>
        /// Gets the size of the payload.
        /// </summary>
        public uint PayloadSize { get; private set; }        

        /// <summary>
        /// Gets the descriptor tag.
        /// </summary>
        public DescriptorTag Tag { get; private set; }

        /// <summary>
        /// Gets the list of sub-descriptors.
        /// </summary>
        public List<Descriptor> SubDescriptors { get; private set; }

        /// <summary>
        /// Reads the descriptor properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected abstract void ReadDescriptorPropertiesFromStream(BoxBinaryReader reader);

        /// <summary>
        /// Reads the sub descriptors from the stream.
        /// </summary>
        /// <param name="reader">The binary stream reader.</param>
        /// <param name="initialOffset">The initial offset.</param>
        protected void ReadSubDescriptors(BoxBinaryReader reader, long initialOffset)
        {
            while (this.PayloadSize - (long)(reader.Offset - initialOffset) > 0)
            {
                this.SubDescriptors.Add(Descriptor.GetNextDescriptorFromStream(reader));
            }
        }

        /// <summary>
        /// Read the next descriptor from the stream.
        /// </summary>
        /// <param name="reader">The stream reader.</param>
        public static Descriptor GetNextDescriptorFromStream(BoxBinaryReader reader)
        {
            if (reader == null || reader.AtEndOfStream) return null;

            Descriptor descriptor = null;

            var offset = reader.Offset;
            var tag = (DescriptorTag)reader.ReadByte();


            uint payloadSize = 0;
            uint headerSize = 1;
            uint maxHeaderSize = 4;
            byte num;

            do
            {
                headerSize++;
                num = reader.ReadByte();
                payloadSize = (payloadSize << 7) + ((uint)(num & 0x7f));
            }
            while ((--maxHeaderSize != 0) && ((num & 0x80) != 0));

            switch (tag)
            {
                case DescriptorTag.ES:
                    descriptor = new ElementaryStreamDescriptor(headerSize, payloadSize);
                    break;
                case DescriptorTag.DECODER_CONFIG:
                    descriptor = new DecoderConfigurationDescriptor(headerSize, payloadSize);
                    break;
                case DescriptorTag.DECODER_SPECIFIC_INFO:
                    descriptor = new DecoderSpecificInformationDescriptor(headerSize, payloadSize);
                    break;
                case DescriptorTag.SL_CONFIG:
                    descriptor = new SyncronizationConfigurationDescriptor(headerSize, payloadSize);
                    break;
                default:                    
                    break;
            }

            if (descriptor != null)
                descriptor.ReadDescriptorPropertiesFromStream(reader);

            if (reader.Offset != offset + descriptor.PayloadSize + descriptor.HeaderSize)
                throw new BoxException(string.Format("The descriptor {0} was not totally read from the stream", tag));

            return descriptor;
        }
    }
}
