
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// The Movie box is a top-level container for the Movie's global metadata.
    /// </summary>
    public class MovieBox: Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovieBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public MovieBox(long offset, long size) : base(offset, size, BoxType.Moov)
        {

        }

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            ReadInnerBoxes(reader, BoxType.Mvhd, BoxType.Uuid, BoxType.Trak, BoxType.Mvex, BoxType.Ainf, BoxType.Meta, BoxType.Free, BoxType.Pssh, BoxType.Iods);
        }
    }
}
