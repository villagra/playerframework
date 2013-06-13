using Microsoft.Media.ISO.Boxes.Codecs.Data;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Contains video codec data as <see cref="VideoInfoHeader2"/>.
    /// </summary>
    public class DigitalVideoCodingBox : Box
    {
        private byte flags;

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalVideoCodingBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public DigitalVideoCodingBox(long offset, long size)
            : base(offset, size, BoxType.Dvc1)
        {

        }
      
        /// <summary>
        /// Gets the profile.
        /// </summary>
        public int Profile { get; private set; }

        /// <summary>
        /// Gets the sequence level.
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// Gets the advanced profile sequence level.
        /// </summary>
        public int AdvancedProfileLevel { get; private set; }

        /// <summary>
        /// Gets the codec private data.
        /// </summary>
        public byte[] CodecPrivateData { get; private set; }

        /// <summary>
        /// Gets the video framerate.
        /// </summary>
        public uint Framerate { get; private set; }

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            var num = reader.ReadByte();

            this.Profile = (num >> 6);
            this.Level = (num & 0xF) >> 1;
            this.AdvancedProfileLevel = reader.ReadByte() >> 5;
            this.flags = reader.ReadByte(); //no_interlace, no_multiple_seq, no_multiple_entry, no_slice_code, no_bframe            
            this.Framerate = reader.ReadUInt32();
            this.CodecPrivateData = reader.ReadBytes((int)(this.Size - (reader.Offset - this.Offset)));
        }
        
    }
}
