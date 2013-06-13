
using System;
using System.Text;
using System.Collections.Generic;
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// The stco and co64 boxes define chunk offsets for each chunk in a sample table. Each sample table must contain one and only one box of either the stco or co64 type.
    /// </summary>
    public class SampleChunkOffsetBox : FullBox
    {
        /// <summary>
        /// The number of offsets in the Offsets table
        /// </summary>
        public uint OffsetCount { get; private set; }

        /// <summary>
        /// A table of absolute chunk offsets within the file
        /// </summary>
        public List<uint> Offsets { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleChunkOffsetBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public SampleChunkOffsetBox(long offset, long size)
            : base(offset, size, BoxType.Stco)
        {
            Offsets = new List<uint>();
        }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            OffsetCount = reader.ReadUInt32();
            for (int i = 0; i < OffsetCount; i++)
            {
                Offsets.Add(reader.ReadUInt32());
            }
        }
    }
}
