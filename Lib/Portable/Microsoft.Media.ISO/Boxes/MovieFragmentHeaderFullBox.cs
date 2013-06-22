
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Movie Fragment header specifies the sequence number of the Movie Fragment in the file. 
    /// </summary>
    public class MovieFragmentHeaderFullBox : FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovieFragmentHeaderFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public MovieFragmentHeaderFullBox(long offset, long size)
            : base(offset, size, BoxType.Mfhd)
        {            
        }

        /// <summary>
        /// Gets or sets the sequence number. The sequence number is a 1-indexed ordinal: the Nth Movie Fragment in the file has sequence number N.
        /// </summary>
        /// <value>
        /// The sequence number is a 1-indexed ordinal: the Nth Movie Fragment in the file has sequence number N.
        /// </value>
        public uint SequenceNumber { get; private set; }


        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            this.SequenceNumber = reader.ReadUInt32();
        }
    }
}
