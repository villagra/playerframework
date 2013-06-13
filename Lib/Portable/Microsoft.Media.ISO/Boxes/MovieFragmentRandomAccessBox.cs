
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// The Movie Fragment Random Access Box allows the Movie Fragments to be located without scanning the entire file.
    /// </summary>
    public class MovieFragmentRandomAccessBox: Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovieFragmentRandomAccessBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public MovieFragmentRandomAccessBox(long offset, long size): base(offset, size, BoxType.Mfra)
        {

        }

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            ReadInnerBoxes(reader, BoxType.Tfra, BoxType.Mfro);
        }
    }
}
