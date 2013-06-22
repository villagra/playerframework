
namespace Microsoft.Media.ISO.Boxes.Descriptors
{
    /// <summary>
    /// Defines the syncronization configuration descriptor 
    /// </summary>
    public class SyncronizationConfigurationDescriptor: Descriptor
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncronizationConfigurationDescriptor"/> class.
        /// </summary>
        /// <param name="headerSize">Size of the descriptor header.</param>
        /// <param name="bodySize">Size of the descriptor body.</param>
        public SyncronizationConfigurationDescriptor(uint headerSize, uint bodySize)
            : base(headerSize, bodySize, DescriptorTag.SL_CONFIG)
        {

        }

        /// <summary>
        /// Gets syncronization bit.
        /// </summary>
        public byte Predefined { get; private set; }

        /// <summary>
        /// Reads the descriptor properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadDescriptorPropertiesFromStream(BoxBinaryReader reader)
        {
            this.Predefined = reader.ReadByte();
        }
    }
}
