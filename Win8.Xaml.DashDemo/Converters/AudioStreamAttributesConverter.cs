using Microsoft.Media.AdaptiveStreaming;
using System;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace Microsoft.PlayerFramework.Xaml.DashDemo
{
    public class AudioStreamAttributesConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var stream = value as IManifestStream;
            return stream != null ? new AudioStreamAttributes(stream.SelectedTracks.First()) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class AudioStreamAttributes
    {
        IManifestTrack track;

        public AudioStreamAttributes(IManifestTrack track)
        {
            this.track = track;
        }

        public long Bitrate { get { return track == null ? 0 : GetLongAttribute("Bitrate"); } }
        public long SamplingRate { get { return track == null ? 0 : GetLongAttribute("SamplingRate"); } }
        public string FourCC { get { return track == null ? "NA" : GetStringAttribute("FourCC"); } }
        public long Channels { get { return track == null ? 0 : GetLongAttribute("Channels"); } }
        public long BitsPerSample { get { return track == null ? 0 : GetLongAttribute("BitsPerSample"); } }
        public long PacketSize { get { return track == null ? 0 : GetLongAttribute("PacketSize"); } }
        public string AudioTag { get { return track == null ? "NA" : GetStringAttribute("AudioTag"); } }
        public string CodecPrivateData { get { return track == null ? "NA" : GetStringAttribute("CodecPrivateData"); } }

        string GetStringAttribute(string key)
        {
            try
            {
                return track.GetAttribute(key);
            }
            catch (ArgumentException) { return "NA"; }
        }

        long GetLongAttribute(string key)
        {
            string value;
            try
            {
                value = track.GetAttribute(key);
            }
            catch (ArgumentException) { return 0; }

            long result;
            if (long.TryParse(value, out result))
            {
                return result;
            }
            else return 0;
        }
    }
}
