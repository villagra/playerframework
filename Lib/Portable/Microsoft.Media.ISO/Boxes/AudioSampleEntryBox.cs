using Microsoft.Media.ISO.Boxes.Codecs.Data;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    ///  Base class for audio sequence samples
    /// </summary>
    public class AudioSampleEntryBox : SampleEntryBox
    {
        private ushort reserved1;
        private ushort reserved2;
        private uint reserved3;
        private ushort predefined;
        private ushort reserved4;

        ///// <summary>
        ///// Initializes a new instance of the <see cref="AudioSampleEntryBox"/> class.
        ///// </summary>
        ///// <param name="offset">The offset in the stream where this box begins.</param>
        ///// <param name="size">The size of this box.</param>
        ///// <param name="boxType">Type of the box.</param>
        //public AudioSampleEntryBox(long offset, long size, BoxType boxType)
        //    : base(offset, size, boxType)
        //{

        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioSampleEntryBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public AudioSampleEntryBox(long offset, long size)
            : base(offset, size, BoxType.Soun)
        {

        }

        /// <summary>
        /// Either 1 (mono) or 2 (stereo).
        /// </summary>
        public ushort ChannelCount { get; private set; }

        /// <summary>
        /// Specifies the precision of each elementary sample in bits.
        /// </summary>
        public ushort SampleSize { get; private set; }

        /// <summary>
        /// The elementary sampling rate expressed as a 16.16 fixed-point number (hi.lo) in samples per second. If the sample rate exceeds 64k, 
        /// <see cref="SampleRate"/> is set to 0 and the information in the wave_format_ex_data field in the owma box is used to indicate this instead.
        /// </summary>
        public uint SampleRate { get; private set; }


        /// <summary>
        /// Gets the audio codec data.
        /// </summary>
        public AudioTrackCodecData AudioCodecData { get; private set; }


        /// <summary>
        /// Reads the sample entry properties from stream.
        /// </summary>
        /// <param name="reader">The stream reader.</param>
        internal override void ReadSampleEntryPropertiesFromStream(BoxBinaryReader reader)
        {
            this.reserved1 = reader.ReadUInt16();
            this.reserved2 = reader.ReadUInt16();
            this.reserved3 = reader.ReadUInt32();
            this.ChannelCount = reader.ReadUInt16();
            this.SampleSize = reader.ReadUInt16();
            this.predefined = reader.ReadUInt16();
            this.reserved4 = reader.ReadUInt16();
            this.SampleRate = reader.ReadUInt32();

            if (reader.PeekNextBoxType() != BoxType.Null)
            {
                ReadInnerBoxes(reader, BoxType.Esds, BoxType.Wfex, BoxType.Sinf, BoxType.Dec3, BoxType.Dac3, BoxType.Dmlp, BoxType.Ddts);
                this.AudioCodecData = GetAudioCodecDataFromInnerBoxes();
            }
            else
            {
                var waveFormatEx = new WaveFormatEx(reader);
                this.AudioCodecData = new AudioTrackCodecData(waveFormatEx);
            }
        }

        private AudioTrackCodecData GetAudioCodecDataFromInnerBoxes()
        {
            foreach (Box box in this.InnerBoxes)
            {
                switch (box.Type)
                {
                    case BoxType.Esds:
                        return new AudioTrackCodecData(this, box as ElementaryStreamDescriptorFullBox);
                    case BoxType.Wfex:
                        return new AudioTrackCodecData(((WaveFormatExBox)box).CodecData);
                    case BoxType.Dec3:
                    case BoxType.Dac3:
                    case BoxType.Dmlp:
                    case BoxType.Ddts:
                        // TODO: add support
                        break;
                    default:
                        break;
                }
            }
            return null;

        }
    }
}
