using System;

namespace Microsoft.Media.ISO.Boxes.Codecs.Data
{
    /// <summary>
    /// Holds video codec data needed to populate a smooth streaming manifest.
    /// </summary>
    public class VideoTrackCodecData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VideoTrackCodecData"/> class.
        /// </summary>
        /// <param name="videoInfoHeader">The video info header.</param>
        public VideoTrackCodecData(VideoInfoHeader2 videoInfoHeader)
        {
            this.Bitrate = (uint)videoInfoHeader.BitRate;
            this.DisplayWidth = (uint)videoInfoHeader.PictureAspectRatioX;
            this.DisplayHeight = (uint)videoInfoHeader.PictureAspectRatioX;
            this.FourCodecCode = videoInfoHeader.BitmapInformationHeader.Compression;
            this.CodecPrivateData = BitConverter.ToString(videoInfoHeader.BitmapInformationHeader.CodecPrivateData).Replace("-", "");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoTrackCodecData"/> class.
        /// </summary>
        /// <param name="avcc">The <see cref="AdvancedVideoCodingBox"/> box.</param>
        /// <param name="width">The track width.</param>
        /// <param name="height">The track height.</param>
        public VideoTrackCodecData(AdvancedVideoCodingBox avcc, ushort width, ushort height)
        {
            this.FourCodecCode = "H264";
            this.DisplayWidth = width;
            this.DisplayHeight = height;
            this.Bitrate = 0;
            this.CodecPrivateData = string.Format("00000001{0}00000001{1}", BitConverter.ToString(avcc.SequenceParameters[0]).Replace("-", ""), 
                                                                            BitConverter.ToString(avcc.PictureParameters[0]).Replace("-", ""));                
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoTrackCodecData"/> class.
        /// </summary>
        /// <param name="dvc1">The <see cref="DigitalVideoCodingBox"/>.</param>
        /// <param name="width">The track width.</param>
        /// <param name="height">The track height.</param>
        public VideoTrackCodecData(DigitalVideoCodingBox dvc1, ushort width, ushort height)
        {
            this.FourCodecCode = "WVC1";
            this.DisplayWidth = width;
            this.DisplayHeight = height;
            //this.Bitrate = 1800000;
            this.CodecPrivateData = BitConverter.ToString(dvc1.CodecPrivateData).Replace("-", "");

        }

 
        /// <summary>
        /// Gets the video track display height.
        /// </summary>
        public uint DisplayHeight { get; private set; }
        /// <summary>
        /// Gets the video track display width.
        /// </summary>
        public uint DisplayWidth { get; private set; }
        /// <summary>
        /// Gets the track codec private data.
        /// </summary>
        public string CodecPrivateData { get; private set; }
        /// <summary>
        /// Gets the track bitrate.
        /// </summary>
        public uint Bitrate { get; private set; }
        /// <summary>
        /// Gets the track four codec code.
        /// </summary>
        public string FourCodecCode { get; private set; }


    }
}
