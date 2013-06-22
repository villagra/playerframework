using System;
using System.Collections.Generic;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// This box describes which samples can be omitted without compromising the decoder's ability to correctly render the output. 
    /// It MAY be omitted for Smooth Streaming, which implies the following default semantic: the first sample is seekable, remaining samples 
    /// are non-seekable (as their dependency on other samples is not known), and no samples are disposable (since the dependency of other samples on any given sample is unknown).
    /// </summary>
    public class IndependentAndDisposableSamplesFullBox : FullBox
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="IndependentAndDisposableSamplesFullBox"/> class.
        /// </summary>
        //// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public IndependentAndDisposableSamplesFullBox(long offset, long size)
            : base(offset, size, BoxType.Sdtp)
        {
            this.Samples = new List<Sample>();
        }

        /// <summary>
        /// Gets the entries table of the samples descriptions.
        /// </summary>
        public List<Sample> Samples { get; private set; }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            int capacity = ((int)Size) - 12;
            for (int i = 0; i < capacity; i++)
            {
                var sample = new Sample();

                byte num = reader.ReadByte();
                sample.SampleDependsOn = (num & 0x30) >> 4;
                sample.SampleIsDependedOn = (num & 12) >> 2;
                sample.SampleHasRedundancy = num & 3;
                this.Samples.Add(sample);
            }
        }

        /// <summary>
        /// Describes each of the samples.
        /// </summary>
        public class Sample
        {
            /// <summary>
            /// This flag takes one of the following four values:
            ///     0: the dependency of this sample is unknown;
            ///     1: this sample does depend on others (not an I picture);
            ///     2: this sample does not depend on others (I picture);
            ///     3: reserved
            /// </summary>
            public int SampleDependsOn { get; internal set; }


            /// <summary>
            /// This flag takes one of the following three values:
            ///     0: it is unknown whether there is redundant coding in this sample;
            ///     1: there is redundant coding in this sample;
            ///     2: there is no redundant coding
            /// </summary>
            public int SampleHasRedundancy { get; internal set; }


            /// <summary>
            /// This flag takes one of the following four values:
            ///     0: the dependency of other samples on this sample is unknown;
            ///     1: other samples depend on this one (not disposable);
            ///     2: no other sample depends on this one (disposable);
            ///     3: reserved
            /// </summary>
            public int SampleIsDependedOn { get; internal set; }
        }
    }
}
