
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Describes high-level metadata for a track.
    /// </summary>
    public class TrackBox: Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public TrackBox(long offset, long size):base(offset, size, BoxType.Trak)
        {

        }

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            ReadInnerBoxes(reader, BoxType.Uuid, BoxType.Tkhd, BoxType.Tref, BoxType.Mdia);            
        }
    }
}
