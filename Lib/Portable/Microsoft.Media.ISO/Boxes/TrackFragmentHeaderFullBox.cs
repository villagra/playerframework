
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// The track fragment header describes a single track fragment contained in the movie fragment. 
    /// IIS Smooth Streaming will only use one track per fragment. The <see cref="TrackId"/> field MUST 
    /// match the track Id for the track in the Track Header Box.
    /// </summary>
    public class TrackFragmentHeaderFullBox: FullBox
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackFragmentHeaderFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public TrackFragmentHeaderFullBox(long offset, long size): base(offset, size, BoxType.Tfhd)
        {
        }

        /// <summary>
        /// Gets or sets the track id.
        /// </summary>       
        public uint TrackId { get; private set; }

        /// <summary>
        /// Specifies the offset of the data field in the fragment’s mdat box, from the beginning of the moof box.
        /// </summary>
        public ulong BaseDataOffset { get; private set; }

        /// <summary>
        /// Contains an index into the Sample Description table ('stsd') for this track. 
        /// The Track Extends Box ('trex') specifies a default sample description index. This field is needed only when 
        /// the track contains multiple sample types, and only for track fragments composed of samples that are not of 
        /// the default sample type
        /// </summary>
        public uint SampleDescriptionIndex { get; private set; }

        /// <summary>
        /// Specifies the difference in decode time (in units of the timescale value of the mdhd box in the trak box with 
        /// the same <see cref="TrackId"/> as this tfhd box , usually 100 ns) between each sample.
        /// This field should be set for video tracks with a fixed frame rate. When the <see cref="DefaultSampleDuration"/> is 
        /// used, samples typically vary in size, so a per-sample sample_size is set in the Track Run box ('trun'), 
        /// and the <see cref="DefaultSampleSize"/> field is omitted.
        /// </summary> 
        public uint DefaultSampleDuration { get; private set; }

        /// <summary>
        /// Specifies the size of each sample in bytes. This field should be set for audio tracks using a 
        /// fixed-size-per-sample encoding. When the <see cref="DefaultSampleSize"/> is used, samples typically vary in 
        /// duration, so a per-sample sample_size is set in the Track Run box ('trun'), and the <see cref="DefaultSampleSize"/> 
        /// field is omitted.
        /// </summary>
        public uint DefaultSampleSize { get; private set; }

        /// <summary>
        /// The sample flags field in sample fragments. is coded as a 32-bit value. 
        /// The following flags are defined in the tf_flags:
        /// 0x000001 (base-data-offset-present). Indicates the definition of the <see cref="BaseDataOffset"/> field. 
        ///     This provides an explicit anchor for the data offsets in each track run (see below). If not provided, 
        ///     the base-data-offset for the first track in the movie fragment is the position of the first byte of 
        ///     the enclosing Movie Fragment Box, and for second and subsequent track fragments, the default is the 
        ///     end of the data defined by the preceding fragment. Fragments 'inheriting' their offset in this way 
        ///     must use the same data-reference (i.e., the data for these tracks must be in the same file). MUST be 1.
        /// 0x000002 (sample-description-index-present). Indicates the presence of this field, which overrides, in this fragment, 
        ///     the default set up in the Track Extends Box. For most common tracks, this field is 0 and the 
        ///     sample-description-index is omitted.
        /// 0x000008 (default-sample-duration-present). MUST be set to 0 if the <see cref="DefaultSampleDuration"/> is omitted.
        /// 0x000010 (default-sample-size-present). MUST be set to 0 if the <see cref="DefaultSampleSize"/> is omitted.
        /// 0x000020 (default-sample-flags-present). MUST be set to 0 if the <see cref="DefaultSampleFlags"/> is omitted.
        /// 0x010000 (duration-is-empty). Indicates that the duration provided in either <see cref="DefaultSampleDuration"/>, 
        ///     or by the default-duration in the Track Extends Box, is empty (i.e., there are no samples for this time interval). 
        ///     It is an error to make a presentation that has both edit lists in the Movie Box, and empty-duration fragments. 
        ///     MUST be 0 for Smooth Streaming.
        /// </summary>       
        public uint DefaultSampleFlags { get; private set; }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            this.TrackId = reader.ReadUInt32();
            if ((base.Flags & 1) != 0)
            {
                this.BaseDataOffset = reader.ReadUInt64();
            }
            if ((base.Flags & 2) != 0)
            {
                this.SampleDescriptionIndex = reader.ReadUInt32();
            }
            if ((base.Flags & 8) != 0)
            {
                this.DefaultSampleDuration = reader.ReadUInt32();
            }
            if ((base.Flags & 0x10) != 0)
            {
                this.DefaultSampleSize = reader.ReadUInt32();
            }
            if ((base.Flags & 0x20) != 0)
            {
                this.DefaultSampleFlags = reader.ReadUInt32();
            }
        }
    }
}
