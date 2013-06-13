
namespace Microsoft.Media.ISO.Boxes
{
    public class BinaryXmlFullBox : FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovieHeaderFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public BinaryXmlFullBox(long offset, long size)
            : base(offset, size, BoxType.Bxml)
        { }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            // TODO: Do we need to read this data?
            reader.GotoEndOfBox(this.Offset, this.Size);
        }
    }
}
