using System;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Specifies a header needed by a Content Protection System to play back the content. The header’s format is specified by the System to it is targeted, and is considered opaque.
    /// Receivers that process such presentations MUST match the SystemID field in this box to the SystemID(s) of the System(s) they support, and select one of the Protection 
    /// System-Specific Header Boxes for a single playback session.
    /// </summary>
    public class ProtectionSystemSpecificHeaderFullBox: FullBox
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectionSystemSpecificHeaderFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public ProtectionSystemSpecificHeaderFullBox(long offset, long size)
            : base(offset, size, BoxType.Pssh)
        {

        }

        /// <summary>
        /// Specifies the Content Protection System that the data box is targeted to, as a UUID for the target System. 
        /// Supported values:
        ///     9A04F079-9840-4286-AB92-E65BE0885F95 – PlayReady
        /// </summary>
        public Guid SystemId { get; private set; }


        /// <summary>
        /// Specifies the size of the Data field in bytes.
        /// </summary>
        public uint DataSize { get; private set; }

        /// <summary>
        /// Specifies raw header data, in a format specific to the system.
        /// </summary>
        public byte[] Data { get; private set; }



        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            this.SystemId = reader.ReadGuid();
            if (string.Compare(this.SystemId.ToString(), "9A04F079-9840-4286-AB92-E65BE0885F95", StringComparison.OrdinalIgnoreCase) != 0)
            {
                throw new BoxException(string.Format("The specified protection system id \"{0}\" is not supported", this.SystemId));
            }

            this.DataSize = reader.ReadUInt32();
            this.Data = reader.ReadBytes(System.Convert.ToInt32(this.DataSize));
        }
    }
}
