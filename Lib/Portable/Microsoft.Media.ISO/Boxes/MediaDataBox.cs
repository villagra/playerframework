
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// The media data box holds sample data. 
    /// The sample size metadata in traf and trun determines sample boundaries in the byte stream, and the duration metadata determines the composition time of each sample.
    /// </summary>
    public class MediaDataBox: Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaDataBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public MediaDataBox(long offset, long size)
            : base(offset, size, BoxType.Mdat)
        {

        }

        ///// <summary>
        ///// Gets or sets the stream with the data.
        ///// </summary>
        //public Stream DataStream { get; private set; }

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            //this.DataStream = reader.BaseStream;

            reader.GotoEndOfBox(this.Offset, this.Size);
        }
    }
}
