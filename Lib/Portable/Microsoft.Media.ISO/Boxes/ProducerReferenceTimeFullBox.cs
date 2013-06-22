
namespace Microsoft.Media.ISO.Boxes
{
    public class ProducerReferenceTimeFullBox : FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProducerReferenceTimeFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        public ProducerReferenceTimeFullBox(long offset, long size)
            : base(offset, size, BoxType.Prft)
        {
        }

        public uint ReferenceTrackID { get; private set; }
        public ulong NtpTimestamp { get; private set; }
        public ulong MediaTime { get; private set; }
        
        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            this.ReferenceTrackID = reader.ReadUInt32();
            this.NtpTimestamp = reader.ReadUInt64();

            if (this.Version == 0)
            {
                this.MediaTime = reader.ReadUInt32();
            }
            else
            {
                this.MediaTime = reader.ReadUInt64();
            }
        }
    }
}
