
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Specifies the composition time to each sample. SHOULD contain no entries.
    /// </summary>
    public class CompositionTimeToSampleFullBox: FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositionTimeToSampleFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public CompositionTimeToSampleFullBox(long offset, long size)
            : base(offset, size, BoxType.Ctts)
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
