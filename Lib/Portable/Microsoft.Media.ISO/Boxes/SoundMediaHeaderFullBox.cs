
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Specifies audio track composition. It MUST be present if and only if describing an audio track.
    /// </summary>
    public class SoundMediaHeaderFullBox: FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoundMediaHeaderFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public SoundMediaHeaderFullBox(long offset, long size)
            : base(offset, size, BoxType.Smhd)
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
