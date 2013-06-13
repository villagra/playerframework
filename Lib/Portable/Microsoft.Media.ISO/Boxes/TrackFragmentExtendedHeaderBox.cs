using System;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// This box MUST be present for a LIVE streaming coming from an encoder and SHOULD be omitted otherwise. 
    /// It specified the fragment’s duration, in timescale increments for the track, and its absolute 
    /// starting offset in timescale increments for the track from a reference point specified in the encoder manifest.
    /// </summary>
    /// <remarks>
    /// Guid("6D1D9B05-42D5-44E6-80E2-141DAFF757B2")
    /// </remarks>
    public class TrackFragmentExtendedHeaderBox : Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackFragmentExtendedHeaderBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public TrackFragmentExtendedHeaderBox(long offset, long size)
            : base(offset, size, BoxType.Uuid)
        {
        }

        /// <summary>
        /// Gets or sets the fragment absolute time as a 64 bit value.
        /// </summary>
        /// <value>
        /// Indicates the absolute time of the fragment's first sample in units defined in the 
        /// <see cref="MovieFragmentHeaderFullBox"/> of the associated track.
        /// </value>
        public uint FragmentAbsoluteTime { get; private set; }


        /// <summary>
        /// Gets or sets the duration of the fragment.
        /// </summary>
        /// <value>
        /// Indicates the duration of the entire fragment.
        /// </value>
        public uint FragmentDuration { get; private set; }


        /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
