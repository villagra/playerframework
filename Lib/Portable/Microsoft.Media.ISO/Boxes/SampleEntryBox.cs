
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Represents the base class for sample entries of <see cref="SampleDescriptionFullBox"/>.
    /// </summary>
    public abstract class SampleEntryBox : Box
    {
        private byte[] reserved;

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleEntryBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        /// <param name="boxType">Type of the box.</param>
        public SampleEntryBox(long offset, long size, BoxType boxType)
            : base(offset, size, boxType)
        {

        }

        /// <summary>
        /// An integer that contains the index of the data reference to use to retrieve data associated with samples that use this sample description. 
        /// Data references are stored in DataReference Boxes. The index ranges from 1 to the number of data references.
        /// </summary>            
        public uint DataReferenceIndex { get; set; }

        /// <summary>
        /// Reads the sample entry properties from stream.
        /// </summary>
        /// <param name="reader">The stream reader.</param>
        internal abstract void ReadSampleEntryPropertiesFromStream(BoxBinaryReader reader);

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            this.reserved = reader.ReadBytes(6);
            this.DataReferenceIndex = reader.ReadUInt16();

            ReadSampleEntryPropertiesFromStream(reader);
        }
    }
}
