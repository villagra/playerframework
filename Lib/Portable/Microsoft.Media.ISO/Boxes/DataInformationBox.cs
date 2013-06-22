
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Specifies external tracks, if any. For IIS Smooth Streaming, it MUST contain a single entry with the self-contained flag set.
    /// </summary>
    public class DataInformationBox: Box
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DataInformationBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public DataInformationBox(long offset, long size)
            : base(offset, size, BoxType.Dinf)
        {
           
        }

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            reader.GotoEndOfBox(this.Offset, this.Size);            
        }
    }
}
