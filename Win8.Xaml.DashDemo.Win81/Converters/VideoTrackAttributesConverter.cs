using Microsoft.Media.AdaptiveStreaming;
using System;
using Windows.UI.Xaml.Data;

namespace Microsoft.PlayerFramework.Xaml.DashDemo
{
    public class VideoTrackAttributesConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var track = value as IManifestTrack;
            return new VideoTrackAttributes(track);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class VideoTrackAttributes
    {
        IManifestTrack track;

        public VideoTrackAttributes(IManifestTrack track)
        {
            this.track = track;
        }

        public long Bitrate { get { return track == null ? 0 : track.Bitrate; } }
        public long MaxWidth { get { return track == null ? 0 : track.MaxWidth; } }
        public long MaxHeight { get { return track == null ? 0 : track.MaxHeight; } }
        public string FourCC { get { return track == null ? "NA" : track.GetAttribute("FourCC"); } }
        public long Index { get { return track == null ? 0 : track.TrackIndex; } }
        public string CodecPrivateData { get { return track == null ? "NA" : track.GetAttribute("CodecPrivateData"); } }
    }
}
