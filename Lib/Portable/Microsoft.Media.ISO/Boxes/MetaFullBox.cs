
namespace Microsoft.Media.ISO.Boxes
{
    public class MetaFullBox : FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovieFragmentBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public MetaFullBox(long offset, long size)
            : base(offset, size, BoxType.Meta)
        {
        }

        
        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            ReadInnerBoxes(reader, BoxType.Hdlr, BoxType.Iloc, BoxType.Xml, BoxType.Bxml, BoxType.Idat);            
        }
    }
}
