using System.Collections.Generic;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Contains the sample specific encryption data. It is used when the sample data in the track or fragment is encrypted.  
    /// The box is mandatory for Track Fragment Boxes or Sample Table Boxes that refer to sample data for 
    /// tracks containing encrypted data. It SHOULD be omitted for unencrypted content.
    /// </summary>
    /// <remarks>
    /// Currently only 0x1 flag value is supported: Override TrackEncryptionBox parameters
    /// If set, this flag implies that the SampleEncryptionBox specifies the <see cref="AlgorithmId"/>, 
    /// <see cref="SampleIdentifierSize"/>, and <see cref="KeyId"/> parameters.  If not present, 
    /// then the default values from the <see cref="TrackEncryptionBox"/> should be used for this fragment and 
    /// only the <see cref="SampleCount"/> and <see cref="SampleIdentifiers"/> are present in the <see cref="SampleEncryptionFullBox"/>.
    /// Guid("A2394F52-5A9B-4f14-A244-6C427C648DF4")
    /// </remarks>
    public class SampleEncryptionFullBox : FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleEncryptionFullBox"/> class. 
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public SampleEncryptionFullBox(long offset, long size)
            : base(offset, size, BoxType.Senc)
        { }

        public Sample[] samples;

        /// <summary>
        /// Gets the list of sample entries in the Sample Encryption Box.
        /// </summary>
        public IEnumerable<Sample> Samples { get { return samples; } }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            var sampleCount = reader.ReadUInt32();
            this.samples = new Sample[sampleCount];

            for (int i = 0; i < sampleCount; i++)
            {
                // TODO: Figure out the iv_size to read this value correctly.
                ulong initializationVector = reader.ReadUInt64();
                SubSample[] subSamples;

                if ((this.Flags & 0x02) == 0x02)
                {
                    var subSampleCount = reader.ReadUInt16();
                    subSamples = new SubSample[subSampleCount];

                    for (int j = 0; j < subSampleCount; j++)
                    {
                        subSamples[j] = new SubSample(
                            reader.ReadUInt16(),
                            reader.ReadUInt32());
                    }
                }
                else
                {
                    subSamples = new SubSample[0];
                }

                this.samples[i] = new Sample(initializationVector, subSamples);
            }
        }

        public class Sample
        {
            private SubSample[] subSamples;

            public ulong InitializationVector { get; private set; }
            public IEnumerable<SubSample> SubSamples { get { return subSamples; } }

            public Sample(ulong initializationVector, SubSample[] subSamples)
            {
                this.InitializationVector = initializationVector;
                this.subSamples = subSamples;
            }
        }

        public class SubSample
        {
            public ushort ClearLength { get; private set; }
            public uint EncryptedLength { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="SubSample"/> class.
            /// </summary>
            /// <param name="clearLength">Length of the sample in clear form.</param>
            /// <param name="encryptedLength">Length of the sample in encrypted form.</param>
            public SubSample(ushort clearLength, uint encryptedLength)
            {
                this.ClearLength = clearLength;
                this.EncryptedLength = encryptedLength;
            }
        }
    }
}
