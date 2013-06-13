
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Specifies the size and compression scheme. Since the Sample Description box happens on a per-track basis, it MUST NOT contain entries of more than 
    /// one type (audio, video, text, hint, and so on.)
    /// Notes:
    ///     Tracks using a WMA audio codec SHOULD use entries of type 'owma'.
    ///     Tracks using a VC-1 video codec SHOULD use entries of type 'ovc1'.
    /// </summary>
    public class SampleDescriptionFullBox : FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleDescriptionFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public SampleDescriptionFullBox(long offset, long size)
            : base(offset, size, BoxType.Stsd)
        {
        }

        /// <summary>
        /// Specifies the number of sample entries.
        /// </summary>
        public uint EntryCount { get; set; }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            this.EntryCount = reader.ReadUInt32();

            ReadInnerBoxes(reader, BoxType.Enca, BoxType.Encs, BoxType.Enct, BoxType.Encv, BoxType.Vide, BoxType.Soun, BoxType.Esds, BoxType.Subt);
        }
    }
}
