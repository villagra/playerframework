
using System.Collections.Generic;
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Specifies the decoding time to each sample.
    /// </summary>
    public class DecodingTimeToSampleFullBox : FullBox
    {
        /// <summary>
        /// An array of STTSRECORD structures.
        /// </summary>
        public List<DecodingTimeToSampleFullBoxEntry> Entries { get; private set; }

        /// <summary>
        /// The number of STTSRECORD entries
        /// </summary>
        public uint Count { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecodingTimeToSampleFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public DecodingTimeToSampleFullBox(long offset, long size)
            : base(offset, size, BoxType.Stts)
        {
            Entries = new List<DecodingTimeToSampleFullBoxEntry>();
        }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            Count = reader.ReadUInt32();
            for (int i = 0; i < Count; i++)
            {
                var entry = new DecodingTimeToSampleFullBoxEntry(reader.ReadUInt32(), reader.ReadUInt32());
                Entries.Add(entry);
            }
        }
    }

    public class DecodingTimeToSampleFullBoxEntry
    {
        internal DecodingTimeToSampleFullBoxEntry(uint sampleCount, uint sampleDelta)
        {
            SampleCount = sampleCount;
            SampleDelta = sampleDelta;
        }

        /// <summary>
        /// The number of consecutive samples that this STTSRECORD applies to
        /// </summary>
        public uint SampleCount { get; private set; }

        /// <summary>
        /// Sample duration
        /// </summary>
        public uint SampleDelta { get; private set; }
    }
}
