
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Contains media-specific metadata for the track.
    /// </summary>
    public class MediaBox: Box
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public MediaBox(long offset, long size)
            : base(offset, size, BoxType.Mdia)
        {
           
        }

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {            
            ReadInnerBoxes(reader, BoxType.Mdhd, BoxType.Hdlr, BoxType.Minf);
        }
    }
}
