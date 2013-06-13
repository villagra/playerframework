
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Contains a table of per-sample metadata. For Fragmented MP4 and IIS Smooth Streaming, the boxes inside the Sample Table Box MUST NOT contain any sample entries. 
    /// Instead, sample metadata is specified with each fragment.
    /// </summary>
    public class SampleTableBox: Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleTableBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public SampleTableBox(long offset, long size)
            : base(offset, size, BoxType.Stbl)
        {

        }

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            ReadInnerBoxes(reader, BoxType.Stts, BoxType.Stss, BoxType.Stsc, BoxType.Stsz, BoxType.Stco, BoxType.Ctts, BoxType.Stsd, BoxType.Btrt, BoxType.Subs, BoxType.Free);
        }
    }
}
