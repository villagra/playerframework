
namespace Microsoft.Media.ISO.Boxes
{
    public class AssetInformationFullBox : FullBox
    {
        public string ProfileVersion { get; private set; }
        public string Apid { get; private set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MovieHeaderFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public AssetInformationFullBox(long offset, long size)
            : base(offset, size, BoxType.Ainf)
        { }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            ProfileVersion = reader.ReadString(4);      // Is this actually a string?
            Apid = reader.ReadNullTerminatedString();

            // TODO: Do we need to read in the other_boxes?
            reader.GotoEndOfBox(this.Offset, this.Size);
        }
    }
}
