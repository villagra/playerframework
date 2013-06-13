
namespace Microsoft.Media.ISO.Boxes
{

    /// <summary>
    /// This box indicates that the file is Fragmented and that parsers should look for Movie Fragments.
    /// </summary>
    public class MovieExtendsBox: Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovieExtendsBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public MovieExtendsBox(long offset, long size)
            : base(offset, size, BoxType.Mvex)
        {

        }

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            reader.GotoEndOfBox(this.Offset, this.Size);
        }
    }
}
