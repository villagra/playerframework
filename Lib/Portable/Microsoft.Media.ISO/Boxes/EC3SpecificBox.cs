
namespace Microsoft.Media.ISO.Boxes
{
    public class EC3SpecificBox : FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EC3SpecificBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public EC3SpecificBox(long offset, long size)
            : base(offset, size, BoxType.Dec3)
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
