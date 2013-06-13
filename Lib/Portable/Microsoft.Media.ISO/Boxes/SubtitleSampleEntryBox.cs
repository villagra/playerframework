
namespace Microsoft.Media.ISO.Boxes
{
    public class SubtitleSampleEntryBox : SampleEntryBox
    {
        public string Namespace { get; private set; }
        public string SchemaLocation { get; private set; }
        public string ImageMimeType { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtitleSampleEntryBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public SubtitleSampleEntryBox(long offset, long size)
            : base(offset, size, BoxType.Subt)
        { }

        /// <summary>
        /// Reads the sample entry properties from stream.
        /// </summary>
        /// <param name="reader">The stream reader.</param>
        internal override void ReadSampleEntryPropertiesFromStream(BoxBinaryReader reader)
        {
            this.Namespace = reader.ReadNullTerminatedString();
            this.SchemaLocation = reader.ReadNullTerminatedString();
            this.ImageMimeType = reader.ReadNullTerminatedString();

            ReadInnerBoxes(reader, BoxType.Btrt);
        }
    }
}
