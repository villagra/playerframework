
namespace Microsoft.Media.ISO.Boxes.Descriptors
{
    /// <summary>
    /// Defines specific decoder information.
    /// </summary>
    public class DecoderSpecificInformationDescriptor: Descriptor
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DecoderSpecificInformationDescriptor"/> class.
        /// </summary>
        /// <param name="headerSize">Size of the descriptor header.</param>
        /// <param name="bodySize">Size of the descriptor body.</param>
        public DecoderSpecificInformationDescriptor(uint headerSize, uint bodySize)
            : base(headerSize, bodySize, DescriptorTag.DECODER_SPECIFIC_INFO)
        {

        }

        /// <summary>
        /// Gets the decoder specific information.
        /// </summary>
        public byte[] Information { get; private set; }

        /// <summary>
        /// Reads the descriptor properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadDescriptorPropertiesFromStream(BoxBinaryReader reader)
        {
            this.Information = reader.ReadBytes((int)this.PayloadSize);
        }
    }
}
