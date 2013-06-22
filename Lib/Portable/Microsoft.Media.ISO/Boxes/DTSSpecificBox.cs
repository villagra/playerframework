
namespace Microsoft.Media.ISO.Boxes
{
    public class DTSSpecificBox : FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DTSSpecificBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public DTSSpecificBox(long offset, long size)
            : base(offset, size, BoxType.Ddts)
        { }
        
        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            reader.GotoEndOfBox(this.Offset, this.Size);
        }
    }
}
