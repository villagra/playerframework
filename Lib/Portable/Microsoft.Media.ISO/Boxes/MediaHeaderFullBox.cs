
using System;
using System.Text;
namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Specifies metadata about the track independent of its codec or type.
    /// </summary>
    public class MediaHeaderFullBox : FullBox
    {
        /// <summary>
        /// Gets the timescale for the media in the track.
        /// </summary>
        public uint Timescale { get; private set; }

        /// <summary>
        /// Gets the timescale for the duration of the media in the track.
        /// </summary>
        public ulong Duration { get; private set; }

        /// <summary>
        /// Gets the ISO-639-2 three character language code for the track.
        /// </summary>
        public string Language { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaHeaderFullBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public MediaHeaderFullBox(long offset, long size)
            : base(offset, size, BoxType.Mdhd)
        { }

        /// <summary>
        /// Reads the full box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadFullBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            if (this.Version == 1)
            {
                reader.BaseStream.Seek(16, System.IO.SeekOrigin.Current);
                this.Timescale = reader.ReadUInt32();
                this.Duration = reader.ReadUInt64();
            }
            else
            {
                reader.BaseStream.Seek(8, System.IO.SeekOrigin.Current);
                this.Timescale = reader.ReadUInt32();
                this.Duration = reader.ReadUInt32();
            }

            this.Language = reader.ReadUInt16PackedCharacters();

            reader.GotoEndOfBox(Offset, Size);
        }
    }
}
