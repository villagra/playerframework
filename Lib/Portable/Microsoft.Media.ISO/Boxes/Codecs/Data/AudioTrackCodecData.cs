using System;
using System.Linq;
using Microsoft.Media.ISO.Boxes.Descriptors;
using System.Globalization;

namespace Microsoft.Media.ISO.Boxes.Codecs.Data
{
    /// <summary>
    /// Holds audio codec data needed to populate a smooth streaming manifest.
    /// </summary>
    public class AudioTrackCodecData
    {
        /// <summary>
        /// Gets the track four codec code.
        /// </summary>
        public string FourCodecCode { get; private set; }
        // <summary>
        /// Gets the track codec private data.
        /// </summary>
        public string CodecPrivateData
        {
            get
            {
                return WaveFormatEx.CodecPrivateData;
            }
        }
        /// <summary>
        /// Gets the WaveFormatEx structure used to create the CodePrivateData
        /// </summary>
        public WaveFormatEx WaveFormatEx { get; private set; }
        /// <summary>
        /// Gets the audio track tag.
        /// </summary>
        public ushort AudioTag { get; private set; }
        /// <summary>
        /// Gets the audio track size of the packet.
        /// </summary>       
        public ushort PacketSize { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioTrackCodecData"/> class.
        /// </summary>
        /// <param name="waveFormatEx">The wave format ex.</param>
        public AudioTrackCodecData(WaveFormatEx waveFormatEx)
        {
            if (waveFormatEx != null)
            {
                this.PacketSize = (ushort)waveFormatEx.BlockAlign;
                this.AudioTag = (ushort)waveFormatEx.FormatTag;
                this.FourCodecCode = MapAudioTagToFourCodecCode(this.AudioTag);
                WaveFormatEx = waveFormatEx;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioTrackCodecData"/> class.
        /// </summary>
        /// <param name="esds">The esds box.</param>
        public AudioTrackCodecData(AudioSampleEntryBox ase, ElementaryStreamDescriptorFullBox esds)
        {
            if (esds != null && esds.StreamDescriptor != null)
            {
                var decoderConfiguration = esds.StreamDescriptor.SubDescriptors.SingleOrDefault(d => d.Tag == DescriptorTag.DECODER_CONFIG) as DecoderConfigurationDescriptor;
                if (decoderConfiguration != null)
                {
                    var decoderSpecificInformation = decoderConfiguration.SubDescriptors.SingleOrDefault(d => d.Tag == DescriptorTag.DECODER_SPECIFIC_INFO) as DecoderSpecificInformationDescriptor;

                    switch (decoderConfiguration.ObjectTypeIndication)
                    {
                        case DecoderConfigurationDescriptor.DecoderObjectTypes.MPEG1_AUDIO:
                            this.PacketSize = 1;
                            this.AudioTag = 0x55;
                            break;
                        case DecoderConfigurationDescriptor.DecoderObjectTypes.MPEG2_AAC_AUDIO_SSRP:
                        case DecoderConfigurationDescriptor.DecoderObjectTypes.MPEG2_AAC_AUDIO_LC:
                        case DecoderConfigurationDescriptor.DecoderObjectTypes.MPEG2_AAC_AUDIO_MAIN:
                        case DecoderConfigurationDescriptor.DecoderObjectTypes.MPEG4_AUDIO:
                        case DecoderConfigurationDescriptor.DecoderObjectTypes.MPEG2_PART3_AUDIO:
                            this.PacketSize = 4;
                            if (decoderSpecificInformation != null && (decoderSpecificInformation.Information[0] == 0x05 || decoderSpecificInformation.Information[0] == 0x1D))
                                this.AudioTag = 0x1610; //HE-AAC
                            else
                                this.AudioTag = 0xFF; //AAC-LC

                            break;
                        default:
                            break;
                    }

                    this.FourCodecCode = MapAudioTagToFourCodecCode(this.AudioTag);

                    WaveFormatEx = new WaveFormatEx();
                    WaveFormatEx.FormatTag = (short)this.AudioTag;
                    WaveFormatEx.Channels = (short)ase.ChannelCount;
                    WaveFormatEx.SamplesPerSec = (int)ase.SampleRate;
                    WaveFormatEx.BitsPerSample = (short)ase.SampleSize;
                    WaveFormatEx.BlockAlign = 8;
                    WaveFormatEx.AvgBytesPerSec = WaveFormatEx.SamplesPerSec * WaveFormatEx.Channels * WaveFormatEx.BitsPerSample / WaveFormatEx.BlockAlign;

                    if (decoderSpecificInformation != null)
                    {
                        WaveFormatEx.Size = (short)decoderSpecificInformation.Information.Length;
                        WaveFormatEx.ExtendedData = decoderSpecificInformation.Information;
                    }
                }
            }
        }

        private string MapAudioTagToFourCodecCode(ushort fourCC)
        {
            switch (fourCC)
            {
                case 0x1:
                case 0x162:
                case 0x161:
                case 0x160:
                    return "WMAP";
                case 0xFF:
                    return "AACL";
                case 0x55:
                    return "MPEG";
                case 0x1610:
                    return "AACH";
                default:
                    return "";
            }
        }

    }
}
