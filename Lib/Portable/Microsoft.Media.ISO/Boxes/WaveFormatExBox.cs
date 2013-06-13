using Microsoft.Media.ISO.Boxes.Codecs.Data;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Contains audio codec data as <see cref="WaveFormatEx"/>
    /// </summary>
    public class WaveFormatExBox: Box
    {
        /// <summary>
        /// Gets the audio codec data.
        /// </summary>
        public WaveFormatEx CodecData { get; private set; }

         /// <summary>
        /// Initializes a new instance of the <see cref="WaveFormatExBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        public WaveFormatExBox(long offset, long size)
            : base(offset, size, BoxType.Wfex)
        {

        }

         /// <summary>
        /// Reads the box properties from stream.
        /// </summary>
        /// <param name="reader">The binary reader with the stream.</param>
        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            this.CodecData = new WaveFormatEx(reader);
        }
    }
}
