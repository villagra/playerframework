
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Represents a movie fragment box: a high-level container for the metadata to describe a fragment.
    /// </summary>
    public class MovieFragmentBox: Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovieFragmentBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public MovieFragmentBox(long offset, long size)
            : base(offset, size, BoxType.Moof)
        {
        }

        
        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            ReadInnerBoxes(reader, BoxType.Mfhd, BoxType.Traf);            
        }
    }
}
