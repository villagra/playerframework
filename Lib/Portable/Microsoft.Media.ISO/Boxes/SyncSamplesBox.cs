
using System.Collections.Generic;
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// The optional stss box specifies which samples within a sample table are sync samples. Sync samples are defined as samples that are safe to seek to. If the track is a video track, sync samples are the keyframes/intraframes that do not rely on any data from any other frames. An stbl box can contain at most one stss box. If the stbl box doesn’t contain an stss box, all samples in the track are treated as sync samples.
    /// 
    /// SHOULD contain no entries.
    /// </summary>
    public class SyncSamplesBox: FullBox
    {
        /// <summary>
        /// A table of sample numbers that are also sync samples; the table is sorted in ascending order of sample numbers.
        /// </summary>
        public List<uint> SyncTable { get; private set; }

        /// <summary>
        /// The number of entries in SyncTable
        /// </summary>
        public uint SyncCount { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncSamplesBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public SyncSamplesBox(long offset, long size)
            : base(offset, size, BoxType.Stss)
        {
            SyncTable = new List<uint>();
        }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            SyncCount = reader.ReadUInt32();
            for (int i = 0; i < SyncCount; i++)
            {
                SyncTable.Add(reader.ReadUInt32());
            }
        }
    }
}
