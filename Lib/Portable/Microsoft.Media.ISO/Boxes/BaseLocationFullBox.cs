
namespace Microsoft.Media.ISO.Boxes
{
    public class BaseLocationFullBox : FullBox
    {
        private byte[] reserved = new byte[512];

        public string BaseLocation { get; private set; }
        public string PurchaseLocation { get; private set; }
        

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieHeaderFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public BaseLocationFullBox(long offset, long size)
            : base(offset, size, BoxType.Bloc)
        { }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            BaseLocation = reader.ReadString(256);
            PurchaseLocation = reader.ReadString(256);
            reader.Read(reserved, 0, 512);
        }
    }
}
