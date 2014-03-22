using Microsoft.Media.AdaptiveStreaming;
using Microsoft.Media.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Media.AdaptiveStreaming.Analytics
{
    public sealed class AdaptiveMonitor : IAdaptiveMonitor
    {
        readonly SortedSet<BitrateLogEntry> bitrateLog;
        uint currentBitrate;
        IManifestTrack selectedTrack;
        IAdaptiveSource source;

        public AdaptiveMonitor()
        {
            bitrateLog = new SortedSet<BitrateLogEntry>();
        }

        #region IAdaptiveMonitor
        public uint CurrentBitrate
        {
            get { return currentBitrate; }
            set
            {
                if (currentBitrate != value)
                {
                    currentBitrate = value;
                    if (CurrentBitrateChanged != null) CurrentBitrateChanged(this, new CurrentBitrateChangedEventArgs());
                }
            }
        }

        public uint AudioBufferSize { get; private set; }

        public uint VideoBufferSize { get; private set; }

        public uint PerceivedBandwidth { get; private set; }

        public event EventHandler<CurrentBitrateChangedEventArgs> CurrentBitrateChanged;

        public event EventHandler<ChunkDownloadedEventArgs> ChunkDownloaded;

        public event EventHandler<ChunkFailureEventArgs> ChunkFailure;

        public void Refresh()
        {
            // we don't need to poll
        }
        #endregion
        
        public void UpdatePosition(TimeSpan position)
        {
            UpdateSelectedTrack(position);
            if (selectedTrack != null)
            {
                CurrentBitrate = selectedTrack.Bitrate;
            }
            else
            {
                CurrentBitrate = 0;
            }
        }

        public IAdaptiveSource Source
        {
            get { return source; }
            set
            {
                if (source != null)
                {
                    source.AdaptiveSourceStatusUpdatedEvent -= source_AdaptiveSourceStatusUpdatedEvent;
                    source.ManifestReadyEvent -= source_ManifestReadyEvent;
                    lock (bitrateLog)
                    {
                        bitrateLog.Clear();
                    }
                }
                source = value;
                if (source != null)
                {
                    source.AdaptiveSourceStatusUpdatedEvent += source_AdaptiveSourceStatusUpdatedEvent;
                    source.ManifestReadyEvent += source_ManifestReadyEvent;
                }
            }
        }

        void source_ManifestReadyEvent(AdaptiveSource sender, ManifestReadyEventArgs args)
        {
            var videoStream = VideoStream;
            var bitrates = videoStream.AvailableTracks.Select(t => t.Bitrate).ToList();
            MinBitrate = bitrates.Min();
            MaxBitrate = bitrates.Max();
        }

        public uint MaxBitrate { get; private set; }

        public uint MinBitrate { get; private set; }

        bool IsOpen
        {
            get { return source != null; }
        }

        bool IsManifestReady
        {
            get { return IsOpen && source.Manifest != null; }
        }

        IManifestStream VideoStream
        {
            get
            {
                return IsManifestReady ? source.Manifest.SelectedStreams.FirstOrDefault(s => s.Type == MediaStreamType.Video) : null;
            }
        }

        void source_AdaptiveSourceStatusUpdatedEvent(AdaptiveSource sender, AdaptiveSourceStatusUpdatedEventArgs args)
        {
            OnStatusUpdated(args);
        }

        void OnStatusUpdated(AdaptiveSourceStatusUpdatedEventArgs args)
        {
            switch (args.UpdateType)
            {
                case AdaptiveSourceStatusUpdateType.NextChunkHttpInvalid:
                case AdaptiveSourceStatusUpdateType.ChunkConnectHttpInvalid:
                    if (ChunkFailure != null)
                    {
                        var failureEventArgs = new ChunkFailureEventArgs();
                        failureEventArgs.HttpResponse = args.HttpResponse;
                        failureEventArgs.ChunkId = args.AdditionalInfo;
                        ChunkFailure(this, failureEventArgs);
                    }
                    break;
                case AdaptiveSourceStatusUpdateType.BitrateChanged:
                    var videoStream = VideoStream;
                    if (videoStream != null)
                    {
                        var bitrateInfo = args.AdditionalInfo.Split(';');
                        var bitrate = uint.Parse(bitrateInfo[0]);
                        var timeStamp = long.Parse(bitrateInfo[1]);
                        var selectedTrack = videoStream.SelectedTracks.FirstOrDefault(t => t.Bitrate == bitrate);
                        if (selectedTrack != null)
                        {
                            lock (bitrateLog) // make this is thread safe since we'll be accessing it from the UI thread in .RefreshState
                            {
                                bitrateLog.Add(new BitrateLogEntry(timeStamp, selectedTrack));
                            }
                        }
                    }
                    break;
                case AdaptiveSourceStatusUpdateType.ChunkDownloaded:
                    var addtionalInfo = args.AdditionalInfo.Split(';');
                    var chunkIndex = int.Parse(addtionalInfo[0]);
                    var url = addtionalInfo[1];
                    var mediaStreamType = (Microsoft.Media.AdaptiveStreaming.MediaStreamType)int.Parse(addtionalInfo[2]);
                    var chunkStartTimeHns = ulong.Parse(addtionalInfo[3]);
                    var chunkDurationns = ulong.Parse(addtionalInfo[4]);
                    var chunkBitrate = uint.Parse(addtionalInfo[5]);
                    var chunkByteCount = uint.Parse(addtionalInfo[6]);
                    var downloadRequestTimeMs = uint.Parse(addtionalInfo[7]);
                    var downloadCompletedTimeMs = uint.Parse(addtionalInfo[8]);
                    var chunkPerceivedBandwidth = uint.Parse(addtionalInfo[9]);
                    var avgPerceivedBandwidth = uint.Parse(addtionalInfo[10]);
                    var bufferLevelAtRequested90kHz = uint.Parse(addtionalInfo[11]);
                    var bufferLevelAtCompleted90kHz = uint.Parse(addtionalInfo[12]);
                    var responseHeaders = addtionalInfo[13];

                    // update properties
                    PerceivedBandwidth = avgPerceivedBandwidth;
                    switch (mediaStreamType)
                    {
                        case MediaStreamType.Audio:
                            AudioBufferSize = bufferLevelAtCompleted90kHz;
                            break;
                        case MediaStreamType.Video:
                            VideoBufferSize = bufferLevelAtCompleted90kHz;
                            break;
                    }

                    if (ChunkDownloaded != null) // notify that a new chunk has downloaded
                    {
                        var chunkInfo = new ChunkDownloadedEventArgs();
                        chunkInfo.ChunkId = chunkIndex;
                        chunkInfo.DownloadTimeMs = downloadCompletedTimeMs - downloadRequestTimeMs;
                        chunkInfo.StartTime = chunkStartTimeHns;
                        chunkInfo.StreamType = mediaStreamType.ToString().ToLowerInvariant();
                        chunkInfo.PerceivedBandwidth = chunkPerceivedBandwidth;
                        chunkInfo.Bitrate = chunkBitrate;
                        chunkInfo.ByteCount = chunkByteCount;
                        ChunkDownloaded(this, chunkInfo);
                    }
                    break;
            }
        }

        void UpdateSelectedTrack(TimeSpan position)
        {
            lock (bitrateLog) // make this is thread safe since the collection can be updated on a background thread.
            {
                foreach (var item in bitrateLog.ToList())
                {
                    if (item.TimeStamp <= position.Ticks)
                    {
                        bitrateLog.Remove(item);
                        selectedTrack = item.Track;
                    }
                    else break;
                }
            }
        }

        class BitrateLogEntry : IComparable<BitrateLogEntry>
        {
            public BitrateLogEntry(long timeStamp, IManifestTrack track)
            {
                TimeStamp = timeStamp;
                Track = track;
            }

            public long TimeStamp { get; private set; }
            public IManifestTrack Track { get; private set; }

            public int CompareTo(BitrateLogEntry other)
            {
                return TimeStamp.CompareTo(other.TimeStamp);
            }
        }
    }
}
