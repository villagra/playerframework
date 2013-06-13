
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// The IIS wire format uses one track per fragment, either a video fragment or an audio fragment.
    /// </summary>
    public class TrackFragmentBox : Box
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackFragmentBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public TrackFragmentBox(long offset, long size)
            : base(offset, size, BoxType.Traf)
        {
        }

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            ReadInnerBoxes(reader, BoxType.Uuid, BoxType.Tfhd, BoxType.Trun, BoxType.Sdtp, BoxType.Trik, BoxType.Subs, BoxType.Tfdt, BoxType.Senc, BoxType.Saio, BoxType.Saiz, BoxType.Avcn);
        }
    }
}
