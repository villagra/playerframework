using System.Collections.Generic;

namespace Microsoft.Media.ISO.Boxes
{
    public class SampleToGroupFullBox : FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleToGroupFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        public SampleToGroupFullBox(long offset, long size)
            : base(offset, size, BoxType.Sbgp)
        {
            this.Entries = new List<SampleToGroupFullBoxEntry>();
        }

        public uint GroupingType { get; private set; }
        public uint EntryCount { get; private set; }
        public List<SampleToGroupFullBoxEntry> Entries { get; private set; }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            this.GroupingType = reader.ReadUInt32();
            this.EntryCount = reader.ReadUInt32();

            for (var i = 0; i < this.EntryCount; i++)
            {
                uint sampleCount = reader.ReadUInt32();
                uint groupDescriptionIndex = reader.ReadUInt32();

                this.Entries.Add(new SampleToGroupFullBoxEntry(sampleCount, groupDescriptionIndex));
            }
        }
    }

    public class SampleToGroupFullBoxEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleToGroupFullBoxEntry"/> class.
        /// </summary>
        /// <param name="sampleCount">The sample count.</param>
        /// <param name="groupDescriptionIndex">Index of the group description.</param>
        public SampleToGroupFullBoxEntry(uint sampleCount, uint groupDescriptionIndex)
        {
            this.SampleCount = sampleCount;
            this.GroupDescriptionIndex = groupDescriptionIndex;
        }

        public uint SampleCount { get; private set; }
        public uint GroupDescriptionIndex { get; private set; }
    }
}
