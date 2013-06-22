using System;

namespace Microsoft.Media.ISO.Boxes
{
    public class TrackEncryptionFullBox : FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackEncryptionFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        public TrackEncryptionFullBox(long offset, long size) 
            : base(offset, size, BoxType.Tenc)
        {
        }

        public uint DefaultIsEntrypted { get; private set; }
        public byte DefaultIVSize { get; private set; }
        public Guid DefaultKID { get; private set; }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            this.DefaultIsEntrypted = reader.ReadUInt24();
            this.DefaultIVSize = reader.ReadByte();
            this.DefaultKID = reader.ReadGuid();
        }
    }
}
