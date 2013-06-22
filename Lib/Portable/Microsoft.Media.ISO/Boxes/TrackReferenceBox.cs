using System.Collections.Generic;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Specifies the track's references to other tracks. This box SHOULD appear only for video tracks that have a corresponding chapter 
    /// track (which is specified as a non-enabled text track) and/or a corresponding script stream track.
    /// </summary>
    public class TrackReferenceBox: Box
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackReferenceBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public TrackReferenceBox(long offset, long size)
            : base(offset, size, BoxType.Tref)
        {
            TrackIds = new List<uint>();
        }

        /// <summary>
        /// List of referenced track identifiers.
        /// </summary>
        public List<uint> TrackIds { get; private set; }


        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            long num = this.Size - 8;
            while (num >= 4)
            {
                this.TrackIds.Add(reader.ReadUInt32());
                num -= 4;
            }
        }
    }
}
