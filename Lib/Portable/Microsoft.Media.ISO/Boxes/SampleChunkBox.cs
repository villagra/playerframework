
using System;
using System.Text;
using System.Collections.Generic;
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// The stsc box defines the sample-to-chunk mapping in the sample table of a media track. An stbl box must contain one and only one stsc box.
    /// </summary>
    public class SampleChunkBox : FullBox
    {
        /// <summary>
        /// The number of STSCRECORD entries.
        /// </summary>
        public uint Count { get; private set; }

        /// <summary>
        /// An array of STSCRECORD structures.
        /// </summary>
        public List<SampleChunkBoxEntry> Entries { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleChunkBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public SampleChunkBox(long offset, long size)
            : base(offset, size, BoxType.Stsc)
        {
            Entries = new List<SampleChunkBoxEntry>();
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
                var entry = new SampleChunkBoxEntry(reader.ReadUInt32(), reader.ReadUInt32(), reader.ReadUInt32());
                Entries.Add(entry);
            }
        }
    }

    public class SampleChunkBoxEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleChunkBoxEntry"/> class.
        /// </summary>
        /// <param name="firstChunk">The first chunk that this record applies to.</param>
        /// <param name="samplesPerChunk">The number of consecutive samples that this record applies to.</param>
        /// <param name="sampleDescIndex">The sample description that describes this sequence of chunks.</param>
        internal SampleChunkBoxEntry(uint firstChunk, uint samplesPerChunk, uint sampleDescIndex)
        {
            this.FirstChunk = firstChunk;
            this.SamplesPerChunk = samplesPerChunk;
            this.SampleDescIndex = sampleDescIndex;
        }

        public uint FirstChunk { get; private set; }
        public uint SamplesPerChunk { get; private set; }
        public uint SampleDescIndex { get; private set; }
    }
}
