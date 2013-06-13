
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Specifies video track composition (unused but required by the format). It MUST be present if and only if describing a video track.
    /// </summary>
    public class VideoMediaHeaderFullBox: FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoMediaHeaderFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public VideoMediaHeaderFullBox(long offset, long size) : base(offset, size, BoxType.Vmhd)
        {
        }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            reader.GotoEndOfBox(this.Offset, this.Size);
        }
    }
}
