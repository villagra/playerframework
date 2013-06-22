
namespace Microsoft.Media.ISO.Boxes
{
    public class BitRateBox : Box
    {
        public uint BufferSizeDB { get; private set; }
        public uint MaxBitRate { get; private set; }
        public uint AvgBitRate { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="DataInformationBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public BitRateBox(long offset, long size)
            : base(offset, size, BoxType.Btrt)
        {

        }

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            BufferSizeDB = reader.ReadUInt32();
            MaxBitRate = reader.ReadUInt32();
            AvgBitRate = reader.ReadUInt32();
        }
    }
}
