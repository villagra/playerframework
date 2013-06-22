
namespace Microsoft.Media.ISO.Boxes
{
    public class SampleAuxiliaryInformationOffsetsFullBox : FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleAuxiliaryInformationOffsetsFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        public SampleAuxiliaryInformationOffsetsFullBox(long offset, long size)
            : base(offset, size, BoxType.Saio)
        {
        }

        public uint AuxInfoType { get; private set; }
        public uint AuxInfoTypeParameter { get; private set; }
        public uint EntryCount { get; private set; }
        public ulong[] Offsets { get; private set; }
        
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

            this.EntryCount = reader.ReadUInt32();
            this.Offsets = new ulong[this.EntryCount];

            for (var i = 0; i < this.EntryCount; i++)
            {
                if (this.Version == 0)
                {
                    this.Offsets[i] = reader.ReadUInt32();
                }
                else
                {
                    this.Offsets[i] = reader.ReadUInt64();
                }
            }
        }
    }
}
