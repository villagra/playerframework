
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Specifies a sample type to which an encryption or content protection has been applied. Protection is specified by encapsulating 
    /// the <see cref="SampleEntryBox"/> exactly as it would appear for an unprotected version of the track as a child of the <see cref="ProtectedSampleEntryBox"/>. 
    /// </summary>
    public class ProtectedSampleEntryBox : SampleEntryBox
    {
        /// <summary>
        /// Gets or sets the original sample entry data.
        /// </summary>
        public SampleEntryBox OriginalSampleEntryData { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectedSampleEntryBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        /// <param name="type">The box type.</param>
        public ProtectedSampleEntryBox(long offset, long size, BoxType type) : base(offset, size, type)
        {
        }

        /// <summary>
        /// Reads the sample entry properties from stream.
        /// </summary>
        /// <param name="reader">The stream reader.</param>
        internal override void ReadSampleEntryPropertiesFromStream(BoxBinaryReader reader)
        {           
            switch(this.Type)
            {
                case BoxType.Enca:
                    this.OriginalSampleEntryData = new AudioSampleEntryBox(reader.Offset, this.Size - (reader.Offset - this.Offset));                    
                    break;
                case BoxType.Encv:
                    this.OriginalSampleEntryData = new VisualSampleEntryBox(reader.Offset, this.Size - (reader.Offset - this.Offset));
                    break;
            }

            this.OriginalSampleEntryData.ReadSampleEntryPropertiesFromStream(reader);
            
            ReadInnerBoxes(reader, BoxType.Sinf);
        }
    }
}
