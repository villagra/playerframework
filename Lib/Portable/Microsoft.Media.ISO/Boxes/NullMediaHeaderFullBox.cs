
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Placeholder for a media header. It MUST be present if describing a text, marker, or script-stream track.
    /// </summary>
    public class NullMediaHeaderFullBox: FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullMediaHeaderFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public NullMediaHeaderFullBox(long offset, long size)
            : base(offset, size, BoxType.Nmhd)
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
