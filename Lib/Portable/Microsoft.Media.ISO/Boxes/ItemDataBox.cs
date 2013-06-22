
namespace Microsoft.Media.ISO.Boxes
{
    public class ItemDataBox : Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataInformationBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public ItemDataBox(long offset, long size)
            : base(offset, size, BoxType.Idat)
        {
           
        }

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            // TODO: Do we need this information?
            reader.GotoEndOfBox(this.Offset, this.Size);            
        }
    }
}
