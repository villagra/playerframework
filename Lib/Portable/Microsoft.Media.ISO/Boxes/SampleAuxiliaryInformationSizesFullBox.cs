
namespace Microsoft.Media.ISO.Boxes
{
    public class SampleAuxiliaryInformationSizesFullBox : FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleAuxiliaryInformationSizesFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        public SampleAuxiliaryInformationSizesFullBox(long offset, long size)
            : base(offset, size, BoxType.Saiz)
        {
        }

        public uint AuxInfoType { get; private set; }
        public uint AuxInfoTypeParameter { get; private set; }
        public byte DefaultSampleInfoSize { get; private set; }
        public uint SampleCount { get; private set; }
        public byte[] SampleInfoSize { get; private set; }
        
        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            if ((this.Flags & 1) != 0)
            {
                this.AuxInfoType = reader.ReadUInt32();
                this.AuxInfoTypeParameter = reader.ReadUInt32();
            }

            this.DefaultSampleInfoSize = reader.ReadByte();
            this.SampleCount = reader.ReadUInt32();

            if (this.DefaultSampleInfoSize == 0)
            {
                this.SampleInfoSize = reader.ReadBytes(System.Convert.ToInt32(this.SampleCount));
            }
        }
    }
}
