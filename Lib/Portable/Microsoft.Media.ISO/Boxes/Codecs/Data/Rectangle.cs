
namespace Microsoft.Media.ISO.Boxes.Codecs.Data
{
    /// <summary>
    /// Defines a rectangle
    /// </summary>
    public class Rectangle
    {
        /// <summary>
        /// The x-coordinate of the upper-left corner of the rectangle.
        /// </summary>       
        public uint Left { get; set; }
        /// <summary>
        /// The y-coordinate of the upper-left corner of the rectangle.
        /// </summary>      
        public uint Top { get; set; }

        /// <summary>
        /// The x-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public uint Right { get; set; }
        /// <summary>
        /// The y-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public uint Bottom { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rectangle"/> class.
        /// </summary>
        /// <param name="reader">The box binary reader.</param>
        public Rectangle(BoxBinaryReader reader)
        {
            this.Left = reader.ReadUInt32();
            this.Top = reader.ReadUInt32();
            this.Right = reader.ReadUInt32();
            this.Bottom = reader.ReadUInt32();
        }
    }
}
