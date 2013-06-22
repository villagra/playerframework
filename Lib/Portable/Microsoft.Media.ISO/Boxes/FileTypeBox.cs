using System.Collections.Generic;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Specifies the sub-type and intended use of the MP4 file, and high-level attributes.
    /// </summary>
    public class FileTypeBox: Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileTypeBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public FileTypeBox(long offset, long size)
            : base(offset, size, BoxType.Ftyp)
        {
            this.CompatibleBrands = new List<string>();
        }

        /// <summary>
        /// Gets or sets the brand identifier.
        /// </summary>
        public string MajorBrand { get; private set; }


        /// <summary>
        /// An informative integer for the minor version of the major brand.
        /// </summary>
        public uint MinorVersion { get; private set; }


        /// <summary>
        /// Represents the list of compatible brands.
        /// </summary>
        public List<string> CompatibleBrands { get; private set; }

        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        /// <remarks>
        /// The CFF format makes the following changes.
        /// * MajorBrand will be set to "ccff"
        /// * MinorVersion will be set to 0x00000000
        /// * CompatibleBrands will have at least one additional brand encoded with "iso6"
        /// </remarks>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            this.MajorBrand = reader.ReadString(4);
            this.MinorVersion = reader.ReadUInt32();
            
            while (reader.Offset < this.Offset + this.Size)
            {
                this.CompatibleBrands.Add(reader.ReadString(4));
            }
        }
    }
}
