
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// This MUST appear as the last Box in the file, and allows the beginning of the Movie Fragment Random Access Box to be located by reading the file from the end. 
    /// The last 32 bits of the file contains the size of the Movie Fragment Random Access Box, so that it can be located by reading this value and seeking by the 
    /// number of bytes indicated from the end of the file.
    /// </summary>
    public class MovieFragmentRandomAccessOffsetFullBox: FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MovieFragmentRandomAccessOffsetFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public MovieFragmentRandomAccessOffsetFullBox(long offset, long size)
            : base(offset, size, BoxType.Mfro)
        {
        }

        /// <summary>
        /// An integer that gives the number of bytes of the enclosing 'mfra' box. 
        /// This field is placed at the end of the enclosing box to assist readers scanning from the end of the file in finding the 'mfra' box.
        /// </summary>
        public uint MfraReverseOffset { get; private set; }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            this.MfraReverseOffset = reader.ReadUInt32();
        }
    }
}
