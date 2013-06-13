
namespace Microsoft.Media.ISO.Boxes
{
    public class TrackFragmentBaseMediaDecodeTimeFullBox : FullBox
    {
        /// <summary>
        /// Gets a value equal to the sum o fthe decode durations of all earlier samples
        /// in the media which is in the media's timescale. This does not include the 
        /// samples added in the enclosing track fragment.
        /// </summary>
        public long BaseMediaDecodeTime { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackFragmentBaseMediaDecodeTimeFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public TrackFragmentBaseMediaDecodeTimeFullBox(long offset, long size)
            : base(offset, size, BoxType.Tfdt)
        { }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            switch (this.Version)
            {
                case 1:
                    this.BaseMediaDecodeTime = reader.ReadInt64();
                    break;
                case 0:
                    this.BaseMediaDecodeTime = reader.ReadInt32();
                    break;
            }
        }
    }
}
