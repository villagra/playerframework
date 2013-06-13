
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Specifies details of the codec and samples.
    /// </summary>
    public class MediaInformationBox: Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediaInformationBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public MediaInformationBox(long offset, long size)
            : base(offset, size, BoxType.Minf)
        {

        }

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            ReadInnerBoxes(reader, BoxType.Vmhd, BoxType.Smhd, BoxType.Nmhd, BoxType.Dinf, BoxType.Stbl, BoxType.Sthd);
        }
    }
}
