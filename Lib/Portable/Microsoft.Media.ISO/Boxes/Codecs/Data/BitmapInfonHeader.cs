
namespace Microsoft.Media.ISO.Boxes.Codecs.Data
{
    /// <summary>
    /// Contains information about the dimensions and color format of a device-independent bitmap (DIB).
    /// </summary>
    public class BitmapInfonHeader
    {
        /// <summary>
        /// Specifies the number of bytes required by the structure. This value does not include the size of the color table or the size of the color masks, if they are appended to the end of structure. 
        /// </summary>
        public int Size { get; private set; }
        /// <summary>
        /// Specifies the width of the bitmap, in pixels.
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// Specifies the height of the bitmap, in pixels.
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// Specifies the number of planes for the target device. This value must be set to 1. 
        /// </summary>
        public short Planes { get; private set; }
        /// <summary>
        /// Specifies the number of bits per pixel (bpp). For uncompressed formats, this value is the average number of bits per pixel. For compressed formats, 
        /// this value is the implied bit depth of the uncompressed image, after the image has been decoded. 
        /// </summary>
        public short BitCount { get; private set; }
        /// <summary>
        /// Specifies the type of compression for a compressed bottom-up bitmap (top-down DIBs cannot be compressed). 
        /// </summary>
        public string Compression { get; private set; }
        /// <summary>
        /// Specifies the size, in bytes, of the image. This can be set to 0 for uncompressed RGB bitmaps. 
        /// </summary>
        public int ImageSize { get; private set; }
        /// <summary>
        /// Specifies the horizontal resolution, in pixels per meter, of the target device for the bitmap. 
        /// </summary>
        public int XPixelsPerMeter { get; private set; }
        /// <summary>
        /// Specifies the vertical resolution, in pixels per meter, of the target device for the bitmap. 
        /// </summary>
        public int YPixelsPerMeter { get; private set; }
        /// <summary>
        /// Specifies the number of color indices in the color table that are actually used by the bitmap. See Remarks for more information. 
        /// </summary>
        public int ColorsUsed { get; private set; }
        /// <summary>
        /// Specifies the number of color indices that are considered important for displaying the bitmap. If this value is zero, all colors are important.
        /// </summary>
        public int ColorsImportant { get; private set; }

        /// <summary>
        /// Gets or sets the codec private data.
        /// </summary>
        public byte[] CodecPrivateData { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitmapInfonHeader"/> class.
        /// </summary>
        /// <param name="reader">The box binary reader.</param>
        public BitmapInfonHeader(BoxBinaryReader reader)
        {
            var initialOffset = reader.Offset;

            this.Size = reader.ReadInt32();
            this.Width = reader.ReadInt32();
            this.Height = reader.ReadInt32();
            this.Planes = reader.ReadInt16();
            this.BitCount = reader.ReadInt16();
            this.Compression = reader.ReadString(4);
            this.ImageSize = reader.ReadInt32();
            this.XPixelsPerMeter = reader.ReadInt32();
            this.YPixelsPerMeter = reader.ReadInt32();
            this.ColorsUsed = reader.ReadInt32();
            this.ColorsImportant = reader.ReadInt32();

            this.CodecPrivateData = reader.ReadBytes(this.Size - (int)(reader.Offset - initialOffset));
        }
    }
}
