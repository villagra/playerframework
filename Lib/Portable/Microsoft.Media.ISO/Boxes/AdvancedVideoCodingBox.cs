using System.Collections.Generic;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Advanced Video Coding decoding configuration.
    /// </summary>
    public class AdvancedVideoCodingBox: Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdvancedVideoCodingBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public AdvancedVideoCodingBox(long offset, long size)
            : base(offset, size, BoxType.Avcc)
        {
            this.SequenceParameters = new List<byte[]>();
            this.PictureParameters = new List<byte[]>();
        }

        /// <summary>
        /// Bit map indicating which profiles this stream is compatible, as defined in the AVC specification [ISO/IEC 14496-10 | ITU Recommendation H.264]. 
        /// </summary>
        public byte AvcCompatibleProfiles { get; private set; }
        /// <summary>
        /// Level code defined in Annex A of the AVC specification [ISO/IEC 14496-10 | ITU Recommendation H.264]. 
        /// </summary>
        public byte AvcLevelIndication { get; private set; }
        /// <summary>
        /// Profile code defined in Annex A of the AVC specification [ISO/IEC 14496-10 | ITU Recommendation H.264]. 
        /// </summary>
        public byte AvcProfileIndication { get; private set; }
        /// <summary>
        /// Gets the configuration version.
        /// </summary>
        public byte ConfigurationVersion { get; private set; }
        /// <summary>
        /// Length in bytes of the NALUnitLength field in an AVC video sample of the associated stream. 
        /// </summary>
        public byte NaluLengthSize { get; private set; }
        /// <summary>
        /// Picture parameter set NAL Unit, as specified in ISO/IEC 14496-10. 
        /// Picture parameter sets shall occur in order of ascending parameter set identifier with gaps being allowed. 
        /// </summary>
        public List<byte[]> PictureParameters { get; private set; }
        /// <summary>
        /// Gets the raw bytes.
        /// </summary>
        public byte[] RawBytes { get; private set; }
        /// <summary>
        /// Sequence parameter set NAL Unit, as specified in ISO/IEC 14496-10. 
        /// Sequence parameter sets shall occur in order of ascending parameter set identifier with gaps being allowed. 
        /// </summary>
        public List<byte[]> SequenceParameters { get; private set; }


        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            var offset = reader.Offset;

            this.RawBytes = reader.ReadBytes((int)this.Size);
            reader.GotoPosition(offset);

            this.ConfigurationVersion = reader.ReadByte();
            this.AvcProfileIndication = reader.ReadByte();
            this.AvcCompatibleProfiles = reader.ReadByte();
            this.AvcLevelIndication = reader.ReadByte();            
            this.NaluLengthSize = (byte)(1 + (reader.ReadByte() & 3));

            var numSequenceParameters = (byte)(reader.ReadByte() & 0x1f);            
            for (uint i = 0; i < numSequenceParameters; i++)
            {
                var length = reader.ReadUInt16();
                this.SequenceParameters.Add(reader.ReadBytes(length));
            }

            var numPictureParameters = reader.ReadByte();            
            for (uint j = 0; j < numPictureParameters; j++)
            {
                var length = reader.ReadUInt16();
                this.PictureParameters.Add(reader.ReadBytes(length));
            }
        }
    }
}
