using System;
using System.Globalization;

namespace Microsoft.Media.ISO.Boxes.Codecs.Data
{
    /// <summary>
    /// The <see cref="WaveFormatEx"/> structure defines the format of waveform-audio data. Only format information common to all waveform-audio data formats is included in this structure.
    /// </summary>
    public sealed class WaveFormatEx
    {
        /// <summary>
        /// Waveform-audio format type.
        /// </summary>
        public Int16 FormatTag { get; set; }
        /// <summary>
        /// Number of channels in the waveform-audio data. Monaural data uses one channel and stereo data uses two channels. 
        /// </summary>
        public Int16 Channels { get; set; }
        /// <summary>
        /// Sample rate, in samples per second (hertz). 
        /// </summary>
        public Int32 SamplesPerSec { get; set; }
        /// <summary>
        /// Required average data-transfer rate, in bytes per second, for the format tag.
        /// </summary>
        public Int32 AvgBytesPerSec { get; set; }
        /// <summary>
        /// Block alignment, in bytes. The block alignment is the minimum atomic unit of data for the wFormatTag format type.
        /// </summary>
        public Int16 BlockAlign { get; set; }
        /// <summary>
        /// Bits per sample for the wFormatTag format type.
        /// </summary>
        public Int16 BitsPerSample { get; set; }
        /// <summary>
        /// Size, in bytes, of extra format information appended to the end of the <see cref="WaveFormatEx"/> structure.
        /// </summary>
        public Int16 Size { get; set; }
        /// <summary>
        /// Gets the extended data.
        /// </summary>
        public byte[] ExtendedData { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaveFormatEx"/> class.
        /// </summary>
        public WaveFormatEx()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WaveFormatEx"/> class.
        /// </summary>
        /// <param name="reader">The binary stream reader.</param>
        public WaveFormatEx(BoxBinaryReader reader)
        {
            this.FormatTag = reader.ReadInt16();
            this.Channels = reader.ReadInt16();
            this.SamplesPerSec = reader.ReadInt32();
            this.AvgBytesPerSec = reader.ReadInt32(); ;
            this.BlockAlign = reader.ReadInt16();
            this.BitsPerSample = reader.ReadInt16();
            this.Size = reader.ReadInt16();
            if (this.Size > 0)
            {
                this.ExtendedData = reader.ReadBytes(this.Size);
            }
        }

        public string CodecPrivateData
        {
            get
            {
                //string s = string.Format(CultureInfo.InvariantCulture, "{0:X4}", this.FormatTag).ToLittleEndian();
                //s += string.Format(CultureInfo.InvariantCulture, "{0:X4}", this.Channels).ToLittleEndian();
                //s += string.Format(CultureInfo.InvariantCulture, "{0:X8}", this.SamplesPerSec).ToLittleEndian();
                //s += string.Format(CultureInfo.InvariantCulture, "{0:X8}", this.AvgBytesPerSec).ToLittleEndian();
                //s += string.Format(CultureInfo.InvariantCulture, "{0:X4}", this.BlockAlign).ToLittleEndian();
                //s += string.Format(CultureInfo.InvariantCulture, "{0:X4}", this.BitsPerSample).ToLittleEndian();
                //s += string.Format(CultureInfo.InvariantCulture, "{0:X4}", this.Size).ToLittleEndian();
                //if (this.Size > 0)
                //{
                //    s += BitConverter.ToString(ExtendedData).Replace("-", "");
                //}
                //return s;

                return BitConverter.ToString(ExtendedData).Replace("-", "");
            }
        }
    }
}
