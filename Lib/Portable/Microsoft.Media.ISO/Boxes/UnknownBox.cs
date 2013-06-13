using System;

namespace Microsoft.Media.ISO.Boxes
{
    /// <summary>
    /// Placeholder for unknown boxes that can be ignored.
    /// </summary>
    public class UnknownBox : Box
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownBox"/> class.
        /// </summary>
        /// <param name="offset">The offset in the stream where this box begins.</param>
        /// <param name="size">The size of this box.</param>
        /// <param name="boxType">Type of the box.</param>
        public UnknownBox(long offset, long size)
            : base(offset, size, BoxType.Unknown)
        {
        }

        protected override void ReadBoxPropertiesFromStream(BoxBinaryReader reader)
        {
            reader.GotoEndOfBox(this.Offset, this.Size);
        }
    }
}
