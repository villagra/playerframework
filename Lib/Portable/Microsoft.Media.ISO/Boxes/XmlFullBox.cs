
namespace Microsoft.Media.ISO.Boxes
{
    public class XmlFullBox : FullBox
    {
        public string Xml { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieHeaderFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public XmlFullBox(long offset, long size)
            : base(offset, size, BoxType.Xml)
        { }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            Xml = reader.ReadNullTerminatedString();
        }
    }
}
