
using System;
using System.Text;
using System.Collections.Generic;
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// The stsz box specifies the size of each sample in a sample table. An stsz atom must contain one and only one stsz box.
    /// </summary>
    public class SampleSizeBox : FullBox
    {
        /// <summary>
        /// If all samples have the same size, this field is set with that constant size; otherwise it is 0
        /// </summary>
        public uint ConstantSize { get; private set; }

        /// <summary>
        /// The number of entries in SizeTable
        /// </summary>
        public uint SizeCount { get; private set; }

        /// <summary>
        /// A table of sample sizes; if ConstantSize is 0, this table is empty
        /// </summary>
        public List<uint> SizeTable { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleSizeBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public SampleSizeBox(long offset, long size)
            : base(offset, size, BoxType.Stsz)
        {
            SizeTable = new List<uint>();
        }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            ConstantSize = reader.ReadUInt32();
            SizeCount = reader.ReadUInt32();
            if (ConstantSize == 0)
            {
                for (int i = 0; i < SizeCount; i++)
                {
                    SizeTable.Add(reader.ReadUInt32());
                }
            }
        }
    }
}
