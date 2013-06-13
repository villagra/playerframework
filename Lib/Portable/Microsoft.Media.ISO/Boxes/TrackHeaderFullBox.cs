using System;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Describes high-level metadata for a track.
    /// </summary>
    public class TrackHeaderFullBox: FullBox
    {
        private uint reserved1;
        private byte[] reserved2 = new byte[8];
        private ushort reserved3;
        private uint[] matrix = new uint[9];

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackHeaderFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public TrackHeaderFullBox(long offset, long size):base(offset, size, BoxType.Tkhd)
        {

        }
        
        /// <summary>
        /// Declares the creation time of this track (in seconds since midnight, Jan. 1, 1904, in UTC time).
        /// </summary>
        public DateTime CreationTime { get; private set; }

        /// <summary>
        /// An integer that declares the most recent time the track was modified (in seconds since midnight, Jan. 1, 1904, in UTC time).
        /// </summary>
        public DateTime ModificationTime { get; private set; }

        /// <summary>
        /// An integer that indicates the duration of this track (in the timescale indicated in the Movie Header Box). 
        /// </summary>
        public ulong Duration { get; private set; }

        /// <summary>
        /// Specify the track's visual presentation size as fixed-point 16.16 values. Width and height for a track with no video MUST be 0.
        /// </summary>
        public uint Height { get; private set; }

        /// <summary>
        /// Specify the track's visual presentation size as fixed-point 16.16 values. Width and height for a track with no video MUST be 0.
        /// </summary>
        public uint Width { get; private set; }

        /// <summary>
        /// Specifies the front-to-back ordering of video tracks. Tracks with lower numbers are closer to the viewer. 0 is the normal value, -1 would be in front of track 0, and so on.
        /// </summary>
        public ushort Layer { get; private set; }

        /// <summary>
        /// An integer that specifies a group or collection of tracks. If this field is 0, there is no information on possible relations to other tracks. 
        /// If this field is not 0, it should be the same for tracks that contain alternate data for one another and different for tracks belonging to different groups. 
        /// Only one track within an alternate group should be played or streamed at any one time, and it must be distinguishable from other tracks in the group via attributes     
        /// such as bit rate, codec, language, packet size, and so on. A group may have only one member.
        /// </summary>
        public ushort AlternateGroup { get; private set; }        

        /// <summary>
        /// An integer that uniquely identifies this track over the entire lifetime of this presentation. Track IDs are never reused and cannot be zero.
        /// </summary>
        public uint TrackId { get; private set; }

        /// <summary>
        /// A fixed 8.8 value specifying the track's relative audio volume. Full volume is 1.0 (0x0100) and is the normal value. 
        /// Its value is irrelevant for a track that contains no audio. Tracks may be composed by combining them according to their volume, and then use 
        /// the overall Movie Header Box volume setting. More complex audio composition (e.g., MPEG-4 BIFS) may also be used.
        /// </summary>
        public ushort Volume { get; private set; }

        


        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            
            if (this.Version == 0)
            {
                this.CreationTime = Converter.SecondsOffsetToDateTimeUtc(reader.ReadUInt32());
                this.ModificationTime = Converter.SecondsOffsetToDateTimeUtc(reader.ReadUInt32());
                this.TrackId = reader.ReadUInt32();
                reserved1 = reader.ReadUInt32();
                this.Duration = reader.ReadUInt32();
            }
            else
            {
                this.CreationTime = Converter.SecondsOffsetToDateTimeUtc(reader.ReadUInt64());
                this.ModificationTime = Converter.SecondsOffsetToDateTimeUtc(reader.ReadUInt64());
                this.TrackId = reader.ReadUInt32();
                reserved1 = reader.ReadUInt32();
                this.Duration = reader.ReadUInt64();
            }
            reader.Read(reserved2, 0, 8);
            this.Layer = reader.ReadUInt16();
            this.AlternateGroup = reader.ReadUInt16();
            this.Volume = reader.ReadUInt16();
            reserved3 = reader.ReadUInt16();
            for (int i = 0; i < 9; i++)
            {
                matrix[i] = reader.ReadUInt32();
            }
            this.Width = reader.ReadUInt32();
            this.Height = reader.ReadUInt32();
        }
    }
}
