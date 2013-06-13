
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// FullBox is a another box base type that contains version information and a set of flags.
    /// </summary>
    public abstract class FullBox : Box
    {
        /// <summary>
        /// Gets or sets the version of the box.
        /// </summary>
        public uint Version { get; private set; }

        /// <summary>
        /// Gets or sets the map of flags. See <see cref="TrackFragmentHeaderFullBox"/> for documentation on a flags usage scenario.
        /// </summary>
        public uint Flags { get; private set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="FullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        /// <param name="boxType">Type of the box.</param>
        public FullBox(long offset, long size, BoxType boxType)
            : base(offset, size, boxType)
        {
        }


        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            uint versionAndFlags = reader.ReadUInt32();
            this.Version = (versionAndFlags >> 0x18) & 0xff;
            this.Flags = versionAndFlags & 0xffffff;

            ReadFullBoxPropertiesFromStream(reader);
        }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected abstract void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader);
    }
}
