using System;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Captures the key metadata for the entire movie.
    /// </summary>
    /// <remarks>
    /// matrix: provides a transformation matrix for the video; (u,v,w) are restricted here to (0,0,1), hex values (0,0,0x40000000).
    /// </remarks>
    public class MovieHeaderFullBox: FullBox
    {
        private byte[] reserved1 = new byte[2];
        private byte[] reserved2 = new byte[8];
        private uint[] matrix = new uint[9];
        private byte[] predefined = new byte[0x18];        

        /// <summary>
        /// Initializes a new instance of the <see cref="MovieHeaderFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public MovieHeaderFullBox(long offset, long size): base(offset, size, BoxType.Mvhd)
        {

        }

        /// <summary>
        /// The creation time of the presentation (in seconds since midnight, Jan. 1, 1904, in UTC time).
        /// </summary>
        public DateTime CreationTimeUtc { get; private set; }

        /// <summary>
        /// The most recent time the presentation was modified (in seconds since midnight, Jan. 1, 1904, in UTC time).
        /// </summary>
        public DateTime ModificationTimeUtc { get; private set; }

        /// <summary>
        /// An integer that specifies the time-scale for the entire presentation. This is the number of time units that pass in one second.
        /// </summary>
        public uint TimeScale { get; private set; }

        /// <summary>
        /// An integer that declares the length of the presentation (in the indicated timescale).
        /// </summary>
        public ulong Duration { get; private set; }

        /// <summary>
        /// A non-zero integer that indicates a value to use for the track ID of the next track to be added to this presentation. Zero is not a valid track ID value. 
        /// </summary>
        public uint NextTrackId { get; private set; }

        /// <summary>
        /// A fixed-point 16.16 number that indicates the preferred rate to play the presentation. 1.0 (0x00010000) is normal forward playback
        /// </summary>
        public uint Rate { get; private set; }

        /// <summary>
        /// A fixed-point 8.8 number that indicates the preferred playback volume. 1.0 (0x0100) is full volume.
        /// </summary>
        public ushort Volume { get; private set; }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {          
            if (base.Version == 0)
            {
                this.CreationTimeUtc = Converter.SecondsOffsetToDateTimeUtc(reader.ReadUInt32());
                this.ModificationTimeUtc = Converter.SecondsOffsetToDateTimeUtc(reader.ReadUInt32());
                this.TimeScale = reader.ReadUInt32();
                this.Duration = reader.ReadUInt32();
            }
            else
            {
                this.CreationTimeUtc = Converter.SecondsOffsetToDateTimeUtc(reader.ReadUInt64());
                this.ModificationTimeUtc = Converter.SecondsOffsetToDateTimeUtc(reader.ReadUInt64());
                this.TimeScale = reader.ReadUInt32();
                this.Duration = reader.ReadUInt64();
            }

            this.Rate = reader.ReadUInt32();
            this.Volume = reader.ReadUInt16();
            reader.Read(reserved1, 0, reserved1.Length);
            reader.Read(reserved2, 0, reserved2.Length);
            for (int i = 0; i < 9; i++)
            {
                matrix[i] = reader.ReadUInt32();
            }
            reader.Read(predefined, 0, predefined.Length);
            this.NextTrackId = reader.ReadUInt32();

        }

        
    }
}
