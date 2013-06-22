
namespace Microsoft.Media.ISO.Boxes
{
    public class ProtectedSchemeInformationBox : Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectedSchemeInformationBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public ProtectedSchemeInformationBox(long offset, long size)
            : base(offset, size, BoxType.Sinf)
        {
        }

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            ReadInnerBoxes(reader, BoxType.Frma, BoxType.Schm, BoxType.Schi, BoxType.Tenc);   
        }
    }
}
