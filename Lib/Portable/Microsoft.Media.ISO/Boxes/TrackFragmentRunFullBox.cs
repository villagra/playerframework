using System.Collections.Generic;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// This box describes the samples in the track fragment. If this fragment uses samples of varying size, the sample-size-present flag is set and sample 
    /// size appears in the sample_size field for each sample. If this fragment uses samples of varying duration, the sample-duration-present flag is set and 
    /// sample duration appears in the sample_duration field for each sample.
    /// The following flags are defined for this box:
    ///	    0x000001. (data-offset-present).
    ///     0x000004 (first-sample-flags-present). Overrides the default flags for the first sample only. This makes it possible to record a group of frames where the first is a key and the rest are difference frames, without supplying explicit flags for every sample. If this flag and field are used, sample-flags shall not be present.
    ///     0x000100 (sample-duration-present). Indicates that each sample has its own duration, otherwise the default is used.
    ///     0x000200 (sample-size-present). Each sample has its own size, otherwise the default is used.
    ///     0x000400 (sample-flags-present). Each sample has its own flags, otherwise the default is used.
    ///     0x000800 (sample-composition-time-offsets-present). Each sample has a composition time offset (e.g., as used for I/P/B video in MPEG).
    /// </summary>
    public class TrackFragmentRunFullBox: FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackFragmentRunFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="size">The size.</param>
        public TrackFragmentRunFullBox(long offset, long size):base(offset, size, BoxType.Trun)
        {
            Samples = new List<Sample>();
        }

        /// <summary>
        /// Added to the implicit or explicit data offset established in the track fragment header.
        /// </summary>
        public int DataOffset { get; private set; }
        
        /// <summary>
        /// Specifies the dependency and redundancy information for the first sample. 
        /// For a video track, the first sample in a fragment MUST be a seekable I-frame, and its sample_depends_on flag MUST be 2.
        /// </summary>
        public uint FirstSampleFlags { get; private set; }
        
        /// <summary>
        /// The number of samples being added in this fragment; also the number of rows in the <see cref="Samples"/> table (the rows can be empty).
        /// </summary>
        public uint SampleCount { get; private set; }


        /// <summary>
        /// Gets or sets the list of samples.
        /// </summary>      
        public List<Sample> Samples { get; private set; }


        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            this.SampleCount = reader.ReadUInt32();

            if ((this.Flags & 1) != 0)
            {
                this.DataOffset = (int)reader.ReadUInt32();
            }

            if ((this.Flags & 4) != 0)
            {
                this.FirstSampleFlags = reader.ReadUInt32();
            }

            
            for (int i = 0; i < this.SampleCount; i++)
            {
                var sample = new Sample();

                if ((base.Flags & 0x100) != 0)
                {
                    sample.SampleDuration = reader.ReadUInt32();
                }
                if ((base.Flags & 0x200) != 0)
                {
                    sample.SampleSize = reader.ReadUInt32();
                }
                if ((base.Flags & 0x400) != 0)
                {
                    sample.SampleFlags = reader.ReadUInt32();
                }
                if ((base.Flags & 0x800) != 0)
                {
                    sample.SampleCompositionTimeOffset = reader.ReadUInt32();
                }
            }
        }

        /// <summary>
        /// Defines information for each sample flags
        /// </summary>
        public class Sample
        {
            /// <summary>
            /// Specifies the offset between the decode time and composition time.
            /// </summary>            
            public uint SampleCompositionTimeOffset { get; internal set; }
            /// <summary>
            /// Gets or sets the duration of the sample.
            /// </summary>            
            public uint SampleDuration { get; internal set; }
            /// <summary>
            /// Specifies the dependency and redundancy information for each sample. For B-frames and P-frames, the sample_depends_on flag MUST be 1 and the sample_is_depended_on 
            /// SHOULD be set to 1 if no B-frames depend on this sample (and 2 otherwise), but MAY be set to 0 if this information cannot be reliably determined.
            /// </summary>
            public uint SampleFlags { get; internal set; }
            /// <summary>
            /// Gets or sets the size of the sample.
            /// </summary>           
            public uint SampleSize { get; internal set; }
        }
    }
}
