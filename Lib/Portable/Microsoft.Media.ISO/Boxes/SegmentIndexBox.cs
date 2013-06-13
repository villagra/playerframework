using Microsoft.Media.ISO.Boxes.Codecs.Data;
using System.Collections.Generic;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    ///  Base class for audio sequence samples
    /// </summary>
    public class SegmentIndexBox : FullBox
    {
        public uint ReferenceId { get; private set; }
        public uint Timescale { get; private set; }
        public ulong EarliestPresentationTime { get; private set; }
        public ulong FirstOffset { get; private set; }
        public ushort Reserved { get; private set; }
        public ushort ReferenceCount { get; private set; }
        public List<Subsegment> Subsegments { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataInformationBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public SegmentIndexBox(long offset, long size)
            : base(offset, size, BoxType.Sidx)
        {

        }

        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            this.ReferenceId = reader.ReadUInt32();
            this.Timescale = reader.ReadUInt32();

            if (this.Version == 1)
            {
                this.EarliestPresentationTime = reader.ReadUInt64();
                this.FirstOffset = reader.ReadUInt64();
            }
            else
            {
                this.EarliestPresentationTime = reader.ReadUInt32();
                this.FirstOffset = reader.ReadUInt32();
            }

            this.Reserved = reader.ReadUInt16();
            this.ReferenceCount = reader.ReadUInt16();

            Subsegments = new List<Subsegment>();
            for (int i = 0; i < this.ReferenceCount; i++)
            {
                var subsegment = new Subsegment();

                uint referenceNum = reader.ReadUInt32();
                subsegment.ReferenceType = System.Convert.ToBoolean(referenceNum >> 31); // 1 bit
                subsegment.ReferencedSize = (referenceNum << 1) >> 1; // 31 bits

                subsegment.Duration = reader.ReadUInt32();

                uint sapNum = reader.ReadUInt32();
                subsegment.StartsWithSAP = System.Convert.ToBoolean(sapNum >> 31); // 1 bit
                subsegment.SAPType = System.Convert.ToUInt16((sapNum << 1) >> 29); // 3 bits
                subsegment.SAPDeltaTime = (sapNum << 4) >> 4; // 28 bits

                Subsegments.Add(subsegment);
            }

            reader.GotoEndOfBox(Offset, Size);
        }
    }

    public class Subsegment
    {
        internal Subsegment()
        { }

        public uint Duration { get; internal set; }
        public bool ReferenceType { get; internal set; }
        public uint ReferencedSize { get; internal set; }
        public bool StartsWithSAP { get; internal set; }
        public ushort SAPType { get; internal set; }
        public uint SAPDeltaTime { get; internal set; }
    }
}
