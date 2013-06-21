using Microsoft.VideoAnalytics;
using Microsoft.Web.Media.Diagnostics;
using Microsoft.Web.Media.SmoothStreaming;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
#if SILVERLIGHT
#else
using System.Threading.Tasks;
using Windows.System.Threading;
#endif

namespace Microsoft.PlayerFramework.Adaptive.Analytics
{
    public sealed class AdaptiveMonitor : IAdaptiveMonitor, IDisposable
    {
        static readonly object traceLock = new object();

        SmoothStreamingMediaElement ssme;

        public AdaptiveMonitor(SmoothStreamingMediaElement ssme)
            : this(ssme, "/Microsoft.PlayerFramework.Adaptive.Analytics;component/TracingConfig.xml")
        {
        }

        public AdaptiveMonitor(SmoothStreamingMediaElement ssme, string tracingConfigPath)
        {
            this.ssme = ssme;
            ssme.PlaybackTrackChanged += ssme_PlaybackTrackChanged;
            ssme.ManifestReady += ssme_ManifestReady;

            Tracing.Initialize();
            using (var reader = System.Xml.XmlReader.Create(tracingConfigPath))
            {
                Tracing.ReadTraceConfig(reader);
            }
        }

        void ssme_ManifestReady(object sender, EventArgs e)
        {
            var segment = ssme.ManifestInfo.Segments.FirstOrDefault();
            if (segment != null)
            {
                var videoStream = segment.AvailableStreams.FirstOrDefault(s => s.Type == MediaStreamType.Video);
                if (videoStream != null)
                {
                    MaxBitrate = (uint)videoStream.AvailableTracks.Max(t => t.Bitrate);
                    MinBitrate = (uint)videoStream.AvailableTracks.Min(t => t.Bitrate);
                }
            }
        }

        void ssme_PlaybackTrackChanged(object sender, TrackChangedEventArgs e)
        {
            if (e.NewTrack.Stream.Type == System.Windows.Media.MediaStreamType.Video)
            {
                CurrentBitrate = (uint)e.NewTrack.Bitrate;
                if (CurrentBitrateChanged != null)
                {
                    CurrentBitrateChanged(this, new CurrentBitrateChangedEventArgs());
                }
            }
        }

        public void Refresh()
        {
            TraceEntry[] entries;
            lock (traceLock)
            {
                entries = Tracing.GetTraceEntries(true) ?? new TraceEntry[] { };
            }

            foreach (var entry in entries)
            {
                if (entry.TraceLevel == TraceLevel.Warning || entry.TraceLevel == TraceLevel.Fatal
                    || entry.TraceLevel == TraceLevel.Error || entry.TraceLevel == TraceLevel.Shutdown)
                {
                    if (entry.TraceArea == TraceArea.BufferingEngine)
                    {
                        if (entry.MethodName == "HandleDownloadError")
                        {
                            if (ChunkFailure != null)
                            {
                                var args = GetChunkError(entry);
                                if (args != null)
                                {
                                    ChunkFailure(this, args);
                                }
                            }
                        }
                    }
                }
                else
                {
                    switch (entry.TraceArea)
                    {
                        case TraceArea.Heuristics:
                            if (entry.MethodName == "GetPerceivedBandwidth")
                            {
                                PerceivedBandwidth = GetPerceivedBandwidth(entry);
                            }
                            break;
                        case TraceArea.Test:
                            if (ChunkDownloaded != null)
                            {
                                try
                                {
                                    // this trace message sometimes had bad data in it
                                    string[] s = entry.Text.Split(' ');
                                    if (s.Length == 17 && (s[5] == "V" || s[5] == "A"))
                                    {
                                        ChunkDownloadedEventArgs result = new ChunkDownloadedEventArgs();

                                        TimeSpan downloadTime;
                                        if (TimeSpan.TryParse(s[2], CultureInfo.CurrentCulture, out downloadTime))
                                        {
                                            result.DownloadTimeMs = (uint)downloadTime.TotalMilliseconds;
                                        }
                                        result.Bitrate = uint.Parse(s[10]);
                                        result.ChunkId = int.Parse(s[9]);
                                        result.ByteCount = uint.Parse(s[7]);
                                        result.StreamType = s[5].ToLowerInvariant();
                                        // TODO: result.PerceivedBandwidth = 0;
                                        // TODO: result.StartTime = 0;
                                        ChunkDownloaded(this, result);
                                    }
                                }
                                catch { /* ignore this exception, must have been a bad trace message */ }
                            }
                            break;
                        case TraceArea.BufferingEngine:
                            if (entry.MethodName == "AddChunkToCache")
                            {
                                var bufferSizeChange = GetBufferSize(entry);
                                if (bufferSizeChange.StreamType == "video")
                                {
                                    VideoBufferSize = bufferSizeChange.Size;
                                }
                                else if (bufferSizeChange.StreamType == "audio")
                                {
                                    AudioBufferSize = bufferSizeChange.Size;
                                }
                            }
                            break;
                    }
                }
            }
        }

        static BufferSizeResult GetBufferSize(TraceEntry entry)
        {
            string streamType = "";
            uint bufferSize = 0;

            const string regexString = "Added (?<d2>.*) chunk with duration (?<d1>.*) to cache. Size including active chunk (?<v>.*) ms";
            Regex regex = new Regex(regexString);
            if (regex.IsMatch(entry.Text))
            {
                ChunkFailureEventArgs result = new ChunkFailureEventArgs();
                var matches = regex.Matches(entry.Text);
                var valueCapture = matches[0].Groups["v"].Captures;
                if (valueCapture.Count > 0)
                {
                    uint.TryParse(valueCapture[0].Value, NumberStyles.None, CultureInfo.CurrentCulture, out bufferSize);
                }
                var data2Capture = matches[0].Groups["d2"].Captures;
                if (data2Capture.Count > 0)
                {
                    streamType = data2Capture[0].Value.ToLowerInvariant();
                }
            }
            return new BufferSizeResult(streamType, bufferSize);
        }

        static uint GetPerceivedBandwidth(TraceEntry entry)
        {
            const string regexString = "NetworkHeuristicsModule - Perceived bandwidth using .* sliding windows and .* method is (?<v>.*) bytes/sec \\[(?<d1>.*)\\]";
            Regex regex = new Regex(regexString);
            if (regex.IsMatch(entry.Text))
            {
                ChunkFailureEventArgs result = new ChunkFailureEventArgs();
                var matches = regex.Matches(entry.Text);
                var valueCapture = matches[0].Groups["v"].Captures;
                if (valueCapture.Count > 0)
                {
                    uint value;
                    if (uint.TryParse(valueCapture[0].Value, NumberStyles.None, CultureInfo.CurrentCulture, out value))
                    {
                        return value;
                    }
                }
            }
            return 0;
        }

        static ChunkFailureEventArgs GetChunkError(TraceEntry entry)
        {
            const string regexString = "Download error for (?<d1>.*) chunk id (?<d2>.*) startTime (?<d3>.*) timeout = .*";
            Regex regex = new Regex(regexString);
            if (regex.IsMatch(entry.Text))
            {
                string streamType = string.Empty;
                int chunkId = 0;
                ulong startTime = 0;

                var matches = regex.Matches(entry.Text);
                var data1Capture = matches[0].Groups["d1"].Captures;
                if (data1Capture.Count > 0)
                {
                    streamType = data1Capture[0].Value;
                }
                var data2Capture = matches[0].Groups["d2"].Captures;
                if (data2Capture.Count > 0)
                {
                    int value;
                    if (int.TryParse(data2Capture[0].Value, out value))
                    {
                        chunkId = value;
                    }
                }
                var data3Capture = matches[0].Groups["d3"].Captures;
                if (data3Capture.Count > 0)
                {
                    ulong value;
                    if (ulong.TryParse(data2Capture[0].Value, out value))
                    {
                        startTime = value;
                    }
                }

                ChunkFailureEventArgs result = new ChunkFailureEventArgs();
                result.ChunkId = string.Format("{0}/{1}", streamType, chunkId);
                return result;
            }
            return null;
        }

        public uint CurrentBitrate { get; private set; }

        public uint AudioBufferSize { get; private set; }

        public uint VideoBufferSize { get; private set; }

        public uint PerceivedBandwidth { get; private set; }

        public uint MaxBitrate { get; private set; }

        public uint MinBitrate { get; private set; }

        public event EventHandler<CurrentBitrateChangedEventArgs> CurrentBitrateChanged;

        public event EventHandler<ChunkDownloadedEventArgs> ChunkDownloaded;

        public event EventHandler<ChunkFailureEventArgs> ChunkFailure;

        public void Dispose()
        {
            Tracing.Shutdown();
            ssme.ManifestReady -= ssme_ManifestReady;
            ssme.PlaybackTrackChanged -= ssme_PlaybackTrackChanged;
            ssme = null;
        }
    }

    internal class BufferSizeResult
    {
        public BufferSizeResult(string streamType, uint size)
        {
            StreamType = streamType;
            Size = size;
        }

        public string StreamType { get; private set; }
        public uint Size { get; private set; }
    }
}
