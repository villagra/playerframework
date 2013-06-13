
namespace Microsoft.Media.ISO.Boxes
{
    public class SchemeInformationBox : Box
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemeInformationBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public SchemeInformationBox(long offset, long size)
            : base(offset, size, BoxType.Schi)
        { }

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            ReadInnerBoxes(reader, BoxType.Tenc);
        }
    }
}
