
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Specifies the type of track.
    /// </summary>
    public class HandlerReferenceFullBox: FullBox
    {
        private uint predefined;
        private uint[] reserved = new uint[3];

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerReferenceFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public HandlerReferenceFullBox(long offset, long size)
            : base(offset, size, BoxType.Hdlr)
        {

        }


        /// <summary>
        /// When present in a media box, an integer containing either one of the following values or a value from a derived specification:
        ///     'vide': Video track
        ///     'soun': Audio track
        ///     'hint': Hint track
        /// </summary>
        public string HandlerType { get; private set; }

        /// <summary>
        /// A null-terminated string in UTF-8 characters which gives a human-readable name for the track type (for debugging and inspection purposes).
        /// </summary>
        public string Name { get; private set; }
      

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            predefined = reader.ReadUInt32();
            this.HandlerType = new string(reader.ReadChars(4));
            
            reserved[0] = reader.ReadUInt32();
            reserved[1] = reader.ReadUInt32();
            reserved[2] = reader.ReadUInt32();

            this.Name = reader.ReadNullTerminatedString();
        }
    }
}
