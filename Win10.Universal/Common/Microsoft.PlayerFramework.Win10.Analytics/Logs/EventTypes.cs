using System;

namespace Microsoft.Media.Analytics
{
    internal static class EventTypes
    {
        public const string CpuLoad = "CpuLoad";
        public const string FramesPerSecond = "FramesPerSecond";
        public const string PerceivedBandwidth = "PerceivedBandwidth";
        public const string BitrateChanged = "BitrateChanged";
        public const string BufferSize = "BufferSize";
        public const string StreamEvent = "StreamEvent";
        public const string StreamLoaded = "StreamLoaded";
        public const string StreamFailed = "StreamFailed";
        public const string ClipEvent = "ClipEvent";
        public const string DvrOperation = "DvrOperation";
        public const string BufferingStateChanged = "BufferingStateChanged";
        public const string FullScreenChanged = "FullScreenChanged";
        public const string CaptionTrackChanged = "CaptionTrackSelect";
        public const string AudioTrackChanged = "AudioTrackSelect";
        public const string ChunkDownload = "ChunkDownload";
        public const string DownloadError = "DownloadError";
        public const string LatencyAlert = "LatencyAlert";
        public const string PlayTimeReached = "PlayTimeReached";
        public const string PlayTimePercentageReached = "PlayTimePercentageReached";
        public const string PositionReached = "PositionReached";
        public const string PositionPercentageReached = "PositionPercentageReached";
    }
}
