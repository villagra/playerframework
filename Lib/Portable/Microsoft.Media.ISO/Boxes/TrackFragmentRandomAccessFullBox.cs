using System.Collections.Generic;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Specifies where fragments of each track can be found. The data specifies byte offsets for locating the Movie Fragment where the samples are contained, 
    /// and information on how to locate the samples within the Movie Fragment.    
    /// </summary>
    /// <remarks>
    /// Given the Wire Format of one track per movie fragment and one track run per track, this means that <see cref="TrackFragmentRandomAccessEntry.TrafNumber"/>, 
    /// <see cref="TrackFragmentRandomAccessEntry.TrunNumber"/> and <see cref="TrackFragmentRandomAccessEntry.SampleNumber"/> in a Smooth Streaming presentation MUST be 1 for all entries.
    /// </remarks>
    public class TrackFragmentRandomAccessFullBox : FullBox
    {

        private int reserved;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackFragmentRandomAccessFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public TrackFragmentRandomAccessFullBox(long offset, long size): base(offset, size, BoxType.Tfra)
        {
            TrackFragmentRandomAccessEntries = new List<TrackFragmentRandomAccessEntry>();
        }        

        /// <summary>
        /// Gets the length size of sample num.
        /// </summary>
        public byte LengthSizeOfSampleNum { get; private set; }

        /// <summary>
        /// Indicates the length in bytes of the traf_number field minus one.
        /// </summary>
        public byte LengthSizeOfTrafNum { get; private set; }

        /// <summary>
        /// Indicates the length in bytes of the trun_number field minus one.
        /// </summary>
        public byte LengthSizeOfTrunNum { get; private set; }

        /// <summary>
        /// An integer that gives the number of the entries for this track. If this value is zero, it indicates that every sample is a random access point and no <see cref="TrackFragmentRandomAccessEntries"/> follows.
        /// </summary>
        public uint NumberOfEntry { get; private set; }

        /// <summary>
        /// An integer identifying the track ID.
        /// </summary>
        public uint TrackId { get; private set; }


        /// <summary>
        /// Gets or sets the tfra entries list.
        /// </summary>      
        public List<TrackFragmentRandomAccessEntry> TrackFragmentRandomAccessEntries { get; private set; }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            this.TrackId = reader.ReadUInt32();
            
            uint num = reader.ReadUInt32();
            this.reserved = ((int)(num >> 6)) & 0x3ffffff;
            this.LengthSizeOfTrafNum = (byte)((num >> 4) & 3);
            this.LengthSizeOfTrunNum = (byte)((num >> 2) & 3);
            this.LengthSizeOfSampleNum = (byte)(num & 3);

            this.NumberOfEntry = reader.ReadUInt32();

            for (int i = 0; i < this.NumberOfEntry; i++)
            {
                TrackFragmentRandomAccessEntry entry = new TrackFragmentRandomAccessEntry();
                
                if (base.Version == 1)
                {
                    entry.Time = reader.ReadUInt64();
                    entry.MoofOffset = reader.ReadUInt64();
                }
                else
                {
                    entry.Time = reader.ReadUInt32();
                    entry.MoofOffset = reader.ReadUInt32();
                }

                entry.TrafNumber = ReadIntValueFromBytes(reader, this.LengthSizeOfTrafNum);
                entry.TrunNumber = ReadIntValueFromBytes(reader, this.LengthSizeOfTrunNum);
                entry.SampleNumber = ReadIntValueFromBytes(reader, this.LengthSizeOfSampleNum);
                               
                this.TrackFragmentRandomAccessEntries.Add(entry);
            }
        }

        private uint ReadIntValueFromBytes(BoxBinaryReader reader, byte numberOfBytes)
        {
            switch (numberOfBytes)
            {
                case 0:
                    return reader.ReadByte();
                case 1:
                    return reader.ReadUInt16();
                case 2:
                    return reader.ReadUInt24();
                case 3:
                    return reader.ReadUInt32();
                default:
                    throw new BoxException("Specified number of bytes to parse an integer is not supported in the traf box.");
            }
        }


        /// <summary>
        /// Defines the information for each track entry.
        /// </summary>
        public class TrackFragmentRandomAccessEntry
        {           
            /// <summary>
            /// An integer that gives the offset of the 'moof' used in this entry. Offset is the byte-offset between the beginning of the file and the beginning of the 'moof'.
            /// </summary>
            public ulong MoofOffset { get; internal set; }
            /// <summary>
            /// Indicates the sample number that contains the random accessible sample. The number ranges from 1 in each 'trun'.
            /// </summary>
            public uint SampleNumber { get; internal set; }
            /// <summary>
            /// An integer that indicates the presentation time of the random access sample in units defined in the 'mdhd' of the associated track.
            /// </summary>
            public ulong Time { get; internal set; }
            /// <summary>
            /// Indicates the 'traf' number that contains the random accessible sample. The number ranges from 1 (the first traf is numbered 1) in each 'moof'.
            /// </summary>
            public uint TrafNumber { get; internal set; }
            /// <summary>
            /// Indicates the 'trun' number that contains the random accessible sample. The number ranges from 1 in each 'traf'.
            /// </summary>
            public uint TrunNumber { get; internal set; }
            /// <summary>
            /// An integer that indicates how large the sample is.
            /// </summary>
            public ulong SampleSize { get; set; }
        }
    }
}
