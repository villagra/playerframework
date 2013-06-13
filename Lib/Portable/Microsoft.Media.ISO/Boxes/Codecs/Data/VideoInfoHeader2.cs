using System;


namespace Microsoft.Media.ISO.Boxes.Codecs.Data
{
    /// <summary>
    /// Interlace flags
    /// </summary>
    [Flags]
    public enum InterlaceFlags
    {
        None = 0,
        IsInterlaced = 0x00000001,
        OneFieldPerSample = 0x00000002,
        Field1First = 0x00000004,
        Unused = 0x00000008,
        FieldPatternMask = 0x00000030,
        FieldPatField1Only = 0x00000000,
        FieldPatField2Only = 0x00000010,
        FieldPatBothRegular = 0x00000020,
        FieldPatBothIrregular = 0x00000030,
        DisplayModeMask = 0x000000c0,
        DisplayModeBobOnly = 0x00000000,
        DisplayModeWeaveOnly = 0x00000040,
        DisplayModeBobOrWeave = 0x00000080,
    }

    /// <summary>
    /// Copy protection flags
    /// </summary>
    [Flags]
    public enum CopyProtectFlags
    {
        None = 0,
        RestrictDuplication = 0x00000001
    }


    /// <summary>
    /// Control flags
    /// </summary>
    [Flags]
    public enum ControlFlags
    {
        None = 0,
        Used = 0x00000001,
        PadTo4x3 = 0x00000002,
        PadTo16x9 = 0x00000004,
    }

    /// <summary>
    /// Describes the bitmap and color information for a video image, including interlace, copy protection, and pixel aspect ratio information. 
    /// </summary>
    public class VideoInfoHeader2
    {

        /// <summary>
        /// A <see cref="Rectangle"/> structure that specifies what part of the source stream should be used to fill the destination buffer. 
        /// </summary>
        public Rectangle SourceRectangle { get; private set; }
        /// <summary>
        /// A <see cref="Rectangle"/> structure that specifies that specifies what part of the destination buffer should be used.
        /// </summary>
        public Rectangle TargetRectangle { get; private set; }
        /// <summary>
        /// The approximate data rate of the video stream, in bits per second. 
        /// </summary>
        public int BitRate { get; private set; }
        /// <summary>
        /// The data error rate of the video stream, in bits per second.
        /// </summary>
        public int BitErrorRate { get; private set; }
        /// <summary>
        /// The video frame's average display time, in 100-nanosecond units. 
        /// </summary>
        public long AverageTimePerFrame { get; private set; }
        /// <summary>
        /// Flags that specify how the video is interlaced. This member is a bit-wise combination of zero or more flags.
        /// </summary>
        public InterlaceFlags Interlace { get; private set; }
        /// <summary>
        /// Flag set with the <see cref="CopyProtectFlags.RestrictDuplication"/> to indicate that the duplication of the stream should be restricted. 
        /// If undefined, specify zero or else the connection will be rejected.
        /// </summary>
        public CopyProtectFlags CopyProtect { get; private set; }
        /// <summary>
        /// The X dimension of picture aspect ratio. For example, 16 for a 16-inch x 9-inch display.
        /// </summary>
        public int PictureAspectRatioX { get; private set; }
        /// <summary>
        /// The Y dimension of picture aspect ratio. For example, 9 for a 16-inch x 9-inch display.
        /// </summary>
        public int PictureAspectRatioY { get; private set; }
        /// <summary>
        /// Gets the control flags.
        /// </summary>
        public ControlFlags Control { get; private set; }
        /// <summary>
        /// Reserved for future use. Must be zero.
        /// </summary>
        public int Reserved2 { get; private set; }

        /// <summary>
        /// Contains color and dimension information for the video image bitmap.
        /// </summary>
        public BitmapInfonHeader BitmapInformationHeader { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoInfoHeader2"/> class.
        /// </summary>
        /// <param name="reader">The box binary reader.</param>
        public VideoInfoHeader2(BoxBinaryReader reader)
        {
            this.SourceRectangle = new Rectangle(reader);
            this.TargetRectangle = new Rectangle(reader);

            this.BitRate = reader.ReadInt32();
            this.BitErrorRate = reader.ReadInt32();
            this.AverageTimePerFrame = reader.ReadInt64();
            this.Interlace = (InterlaceFlags)reader.ReadInt32();
            this.CopyProtect = (CopyProtectFlags)reader.ReadInt32();
            this.PictureAspectRatioX = reader.ReadInt32();
            this.PictureAspectRatioY = reader.ReadInt32();
            this.Control = (ControlFlags)reader.ReadInt32();
            this.Reserved2 = reader.ReadInt32();

            this.BitmapInformationHeader = new BitmapInfonHeader(reader);
        }        
    }    
}
