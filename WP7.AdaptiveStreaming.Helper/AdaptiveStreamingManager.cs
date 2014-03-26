using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.Web.Media.SmoothStreaming;

namespace Microsoft.Media.AdaptiveStreaming.Helper
{
    public class AdaptiveStreamingManager
    {
        public SmoothStreamingMediaElement SSME { get; private set; }
        private IDownloaderPlugin downloaderPlugin;
        private bool isStartupBitrateActive;

        public AdaptiveStreamingManager()
        {
#if WINDOWS_PHONE
            AutoRestrictTracks = true;
#endif
        }

        public IDownloaderPlugin DownloaderPlugin
        {
            get { return downloaderPlugin; }
            set
            {
                downloaderPlugin = value;
                if (SSME != null)
                {
                    SSME.SmoothStreamingCache = new DownloaderPluginAdapter(downloaderPlugin);
                }
            }
        }

        /// <summary>
        /// Initializes the smooth streaming media element.
        /// </summary>
        /// <param name="ssme">The instance of the SmoothStreamingMediaElement to use</param>
        public void Initialize(SmoothStreamingMediaElement ssme)
        {
            SSME = ssme;
            if (downloaderPlugin != null)
            {
                SSME.SmoothStreamingCache = new DownloaderPluginAdapter(downloaderPlugin);
            }

#if !WINDOWS_PHONE
            _chunkDownloadManager = new ChunkDownloadManager(SSME);
            _chunkDownloadManager.DownloadCompleted += ChunkDownloadManager_ChunkDownloadCompleted;
            _chunkDownloadManager.RetryingDownload += ChunkDownloadManager_RetryingChunkDownload;
            _chunkDownloadManager.DownloadExceededMaximumRetries += ChunkDownloadManager_ChunkDownloadExceededMaximumRetryAttempts;
#endif
            WireSmoothEvents();
        }

        /// <summary>
        /// Uninitializes the smooth streaming media element.
        /// </summary>
        public void Uninitialize()
        {
            UnwireSmoothEvents();

#if !WINDOWS_PHONE
            _chunkDownloadManager.RetryingDownload -= ChunkDownloadManager_RetryingChunkDownload;
            _chunkDownloadManager.DownloadExceededMaximumRetries -= ChunkDownloadManager_ChunkDownloadExceededMaximumRetryAttempts;
            _chunkDownloadManager.DownloadCompleted -= ChunkDownloadManager_ChunkDownloadCompleted;
            _chunkDownloadManager.Cancel();
            _chunkDownloadManager.Dispose();
            _chunkDownloadManager = null;
#endif
            SSME = null;
        }

        void WireSmoothEvents()
        {
            SSME.ManifestReady += SSME_ManifestReady;
            SSME.PlaybackTrackChanged += SSME_PlaybackTrackChanged;
            SSME.LiveEventCompleted += SSME_LiveEventCompleted;
            SSME.CurrentStateChanged += SSME_CurrentStateChanged;
#if !WINDOWS_PHONE
            SSME.SizeChanged += SSME_SizeChanged;
#endif
        }

        void UnwireSmoothEvents()
        {
#if !WINDOWS_PHONE
            SSME.SizeChanged -= SSME_SizeChanged;
#endif
            SSME.ManifestReady -= SSME_ManifestReady;
            SSME.PlaybackTrackChanged -= SSME_PlaybackTrackChanged;
            SSME.LiveEventCompleted -= SSME_LiveEventCompleted;
            SSME.CurrentStateChanged -= SSME_CurrentStateChanged;
        }

        void SSME_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            switch (SSME.CurrentState)
            {
                case SmoothStreamingMediaElementState.Closed:
                    isStartupBitrateActive = false;
                    CloseManifest();
                    break;
                case SmoothStreamingMediaElementState.Playing:
                case SmoothStreamingMediaElementState.Paused:
                    if (isStartupBitrateActive)
                    {
                        isStartupBitrateActive = false;
                        foreach (var videoStream in CurrentSegment.SelectedStreams.Where(IsVideoStream))
                        {
                            var tracks = videoStream.AvailableTracks.ToList();
                            if (tracks.Any())
                            {
                                videoStream.SelectTracks(tracks, false);
                            }
                        }
                    }
                    break;
            }
        }

        void SSME_ManifestReady(object sender, EventArgs e)
        {
            // don't let tracks that are higher resolution than what can be displayed get selected.
#if WINDOWS_PHONE
            if (AutoRestrictTracks) RestrictTracks();
#else
            RestrictSize((uint)SSME.ActualWidth, (uint)SSME.ActualHeight);
#endif
            if (StartupBitrate.HasValue)
            {
                foreach (var videoStream in CurrentSegment.SelectedStreams.Where(IsVideoStream))
                {
                    var tracks = videoStream.AvailableTracks.OrderBy(o => Math.Abs((long)o.Bitrate - (long)StartupBitrate.Value)).Take(1).ToList();
                    if (tracks.Any())
                    {
                        videoStream.SelectTracks(tracks, false);
                        isStartupBitrateActive = true;
                    }
                }
            }

            if (ManifestReady != null) ManifestReady(this, EventArgs.Empty);

            OpenManifest();
        }

        void SSME_LiveEventCompleted(object sender, EventArgs e)
        {
            if (EndOfLive != null) EndOfLive(this, EventArgs.Empty);
        }

        void SSME_PlaybackTrackChanged(object sender, TrackChangedEventArgs e)
        {
            CurrentBitrate = e.NewTrack.Bitrate;
            MaxBitrate = e.HighestPlayableTrack.Bitrate;
            var size = e.NewTrack.GetSize();
            CurrentWidth = (uint)size.Width;
            CurrentHeight = (uint)size.Height;

            if (StateChanged != null) StateChanged(this, EventArgs.Empty);
        }

#if !WINDOWS_PHONE
        void SSME_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RestrictSize((uint)e.NewSize.Width, (uint)e.NewSize.Height);
        }

#endif

        private void OpenManifest()
        {
#if !WINDOWS_PHONE
            SSME.ManifestInfo.ChunkListChanged += ManifestInfo_ChunkListChanged;
#endif
            _streamSelectionManager = new StreamSelectionManager(SSME.ManifestInfo);
            _streamSelectionManager.StreamSelectionCompleted += StreamSelectionManager_StreamSelectionCompleted;
            _streamSelectionManager.StreamSelectionExceededMaximumRetries += StreamSelectionManager_StreamSelectionExceededMaximumRetries;
        }

        void CloseManifest()
        {
#if !WINDOWS_PHONE
            SSME.ManifestInfo.ChunkListChanged -= ManifestInfo_ChunkListChanged;
#endif
            if (_streamSelectionManager != null)
            {
                _streamSelectionManager.StreamSelectionCompleted -= StreamSelectionManager_StreamSelectionCompleted;
                _streamSelectionManager.StreamSelectionExceededMaximumRetries -= StreamSelectionManager_StreamSelectionExceededMaximumRetries;
                _streamSelectionManager = null;
            }
        }

        #region Helpers

        private const string TypeAttribute = "type";
        private const string SubTypeAttribute = "subtype";

        static readonly string[] AllowedCaptionStreamSubTypes = new string[] { "CAPT", "SUBT" };

        /// <summary>
        /// Gets the segment from the adaptive manifest that is currently active
        /// </summary>
        public SegmentInfo CurrentSegment
        {
            get
            {
                return SSME != null && SSME.ManifestInfo != null && SSME.CurrentSegmentIndex.HasValue
                                          ? SSME.ManifestInfo.Segments[SSME.CurrentSegmentIndex.Value]
                                          : null;
            }
        }

        public static bool IsCaptionStream(StreamInfo stream)
        {
            string type = stream.Attributes.GetEntryIgnoreCase(TypeAttribute);
            string subType = stream.Attributes.GetEntryIgnoreCase(SubTypeAttribute);

            return string.Equals(type, "text", StringComparison.InvariantCultureIgnoreCase) && AllowedCaptionStreamSubTypes.Any(i => string.Equals(i, subType, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool IsAudioStream(StreamInfo stream)
        {
            return stream.Type == MediaStreamType.Audio;
        }

        public static bool IsVideoStream(StreamInfo stream)
        {
            return stream.Type == MediaStreamType.Video;
        }

        #endregion

        #region Size Restriction

#if WINDOWS_PHONE
        static int MaxPixels;

        static AdaptiveStreamingManager()
        {
#if WINDOWS_PHONE7
            MaxPixels = 384000; // smooth streaming supports a max pixel count of 800x480 (384000)
#else
            double scale = (double)Application.Current.Host.Content.ScaleFactor / 100;
            int verticalSize = (int)Math.Ceiling(Application.Current.Host.Content.ActualHeight * scale);
            int horizontalSize = (int)Math.Ceiling(Application.Current.Host.Content.ActualWidth * scale);
            MaxPixels = verticalSize * horizontalSize;
#endif
        }

        /// <summary>
        /// Gets or sets whether to automatically restrict tracks unsuitable for platform and device. Sometimes the app knows best and should perform this duty itself.
        /// </summary>
        public bool AutoRestrictTracks { get; set; }

        /// <summary>
        /// Called from OnManifestReady to restrict video tracks. Windows Phone 7 has restrictions on which tracks are allowed. Override this function to change the default logic.
        /// </summary>
        protected virtual void RestrictTracks()
        {
            foreach (var videoStream in CurrentSegment.SelectedStreams.Where(vs => IsVideoStream(vs) && vs.AvailableTracks.Any()))
            {
                IEnumerable<TrackInfo> excludedTracks;
                if (IsMultiResolutionVideoSupported)
                {
                    excludedTracks = videoStream.AvailableTracks.Where(o => o.GetSize().Height * o.GetSize().Width > MaxPixels);

                    if (excludedTracks.Any())
                    {
                        // restrict tracks to non-excluded tracks
                        videoStream.RestrictTracks(videoStream.AvailableTracks.Except(excludedTracks).ToList());
                    }
                }
                else
                {
                    var supportedTracks = videoStream.AvailableTracks.Where(o => o.GetSize().Height * o.GetSize().Width <= MaxPixels).ToList();
                    if (!supportedTracks.Any())
                    {
                        // no tracks were found smaller than the max size, just pick the smallest one(s).
                        supportedTracks = videoStream.AvailableTracks.GroupBy(o => o.GetSize().Height * o.GetSize().Width).OrderBy(o => o.Key).First().ToList();
                    }
                    var trackGroups = supportedTracks.GroupBy(o => o.GetSize());
                    // pick the group with the most tracks at the same resolution, if there are more than one use the group that contains the highest bitate
                    var bestGroup = trackGroups.OrderByDescending(g => g.Count()).ThenByDescending(g => g.Max(t => t.Bitrate)).First().ToList();

                    if (bestGroup.Any())
                    {
                        videoStream.RestrictTracks(bestGroup);
                    }
                }
            }
        }

#if WINDOWS_PHONE70
        static bool IsMultiResolutionVideoSupported
        {
            get
            {
                return false;
            }
        }
#elif WINDOWS_PHONE7
        static bool IsMultiResolutionVideoSupported
        {
            get
            {
                return Microsoft.Phone.Info.MediaCapabilities.IsMultiResolutionVideoSupported;
            }
        }
#else
        // HACK: This only works in WP8 for some streams. Allowing users to set this option.
        /// <summary>
        /// Determines if multiple resolutions are supported. Default is false.
        /// </summary>
        public bool IsMultiResolutionVideoSupported { get; set; }
#endif
#endif

        void RestrictSize(uint width, uint height)
        {
            if (CurrentSegment != null)
            {
                foreach (var videoStream in CurrentSegment.AvailableStreams.Where(IsVideoStream))
                {
                    if (videoStream.AvailableTracks.Any())
                    {
                        var tracks = (from t in videoStream.AvailableTracks
                                      let size = t.GetSize()
                                      where size.Width <= width && size.Height <= height
                                      select t).ToList();
                        if (!tracks.Any())
                        {
                            // there are no tracks this small. Instead take the smallest ones (note: there can be more than one at the same size)
                            tracks = videoStream.AvailableTracks.GroupBy(t => t.GetSize().Width * t.GetSize().Height).OrderBy(g => g.Key).First().ToList();
                        }
                        if (tracks.Count != videoStream.SelectedTracks.Count)
                        {
                            videoStream.SelectTracks(tracks, false);
                        }
                    }
                }
            }
        }
        #endregion

        #region Data
        private StreamSelectionManager _streamSelectionManager;

        /// <summary>
        /// Occurs when a stream has been selected.
        /// </summary>
        public event Action<AdaptiveStreamingManager, StreamInfo> StreamSelected;

        /// <summary>
        /// Occurs when a stream has been unselected.
        /// </summary>
        public event Action<AdaptiveStreamingManager, StreamInfo> StreamUnselected;

        /// <summary>
        /// Occurs when the selection of a stream fails.
        /// </summary>
        public event Action<AdaptiveStreamingManager, IEnumerable<StreamInfo>, Exception> StreamSelectionFailed;

#if !WINDOWS_PHONE

        private ChunkDownloadManager _chunkDownloadManager;

        /// <summary>
        /// Occurs when data has been added to a stream.
        /// </summary>
        public event Action<AdaptiveStreamingManager, StreamInfo, ChunkInfo> StreamDataAdded;

        /// <summary>
        /// Occurs when data has been removed from a stream.
        /// </summary>
        public event Action<AdaptiveStreamingManager, StreamInfo, TimeSpan> StreamDataRemoved;

        /// <summary>
        /// Occurs when data has been downloaded and is ready for processing
        /// </summary>
        public event Action<AdaptiveStreamingManager, DataReceivedInfo> DataReceived;

        /// <summary>
        /// Occurs when the download of data from a stream fails.
        /// </summary>
        public event Action<AdaptiveStreamingManager, TrackInfo, ChunkInfo, Exception> DownloadStreamDataFailed;

        /// <summary>
        /// Occurs when the download of data from a stream completes.
        /// </summary>
        public event Action<AdaptiveStreamingManager, TrackInfo, ChunkInfo, Stream, StreamInfo> DownloadStreamDataCompleted;
#endif

        private void StreamSelectionManager_StreamSelectionCompleted(StreamSelectionManager streamSelectionManager, SegmentInfo segment, IEnumerable<StreamInfo> streams, StreamUpdatedListEventArgs e)
        {
            if (e.Error == null)
            {
                foreach (StreamUpdatedEventArgs update in e.StreamUpdatedEvents)
                {
                    if (update.Action == StreamUpdatedEventArgs.StreamUpdatedAction.StreamSelected)
                    {
                        var stream = update.Stream;
#if !WINDOWS_PHONE
                        if ((stream.GetStreamType().ToLower() == "binary" || stream.GetStreamType().ToLower() == "text") && stream.AvailableTracks.Any())
                        {
                            var track = stream.AvailableTracks.First();
                            DownloadStreamData(track);
                        }
#endif
                        if (StreamSelected != null) StreamSelected(this, stream);
                    }
                    else if (update.Action == StreamUpdatedEventArgs.StreamUpdatedAction.StreamDeselected)
                    {
                        var stream = update.Stream;
#if !WINDOWS_PHONE
                        if ((stream.GetStreamType().ToLower() == "binary" || stream.GetStreamType().ToLower() == "text") && stream.AvailableTracks.Any())
                        {
                            var track = stream.AvailableTracks.First();
                            CancelDownloadStreamData(track);
                        }
#endif
                        if (StreamUnselected != null) StreamUnselected(this, update.Stream);
                    }
                }
            }
            else
            {
                if (StreamSelectionFailed != null) StreamSelectionFailed(this, streams, e.Error);
            }
        }

        private void StreamSelectionManager_StreamSelectionExceededMaximumRetries(StreamSelectionManager streamSelectionManager, SegmentInfo segment, IEnumerable<StreamInfo> streams)
        {
            if (StreamSelectionFailed != null) StreamSelectionFailed(this, streams, new TimeoutException());
        }

        public void SetSegmentSelectedStreams(SegmentInfo segment, IEnumerable<StreamInfo> streams)
        {
            var mediaStreams = streams.ToList();

            if (_streamSelectionManager != null && segment != null)
            {
                _streamSelectionManager.EnqueueRequest(segment, mediaStreams);
            }
        }

        public void ModifySegmentSelectedStreams(SegmentInfo segment, IEnumerable<StreamInfo> streamsToAdd, IEnumerable<StreamInfo> streamsToRemove)
        {
            var mediaStreamsToAdd = streamsToAdd == null ? Enumerable.Empty<StreamInfo>() : streamsToAdd.ToList();
            var mediaStreamsToRemove = streamsToRemove == null ? Enumerable.Empty<StreamInfo>() : streamsToRemove.ToList();

            if (_streamSelectionManager != null && segment != null)
            {
                _streamSelectionManager.EnqueueRequest(segment, mediaStreamsToAdd, mediaStreamsToRemove);
            }
        }

#if !WINDOWS_PHONE
        /// <summary>
        /// Downloads all of the available data from the specified track.
        /// </summary>
        /// <param name="track">the track that contains the data to be downloaded.</param>
        public void DownloadStreamData(TrackInfo track)
        {
            if (track != null)
            {
                _chunkDownloadManager.AddRequests(track.Stream.ChunkList.Select(chunk => new Tuple<TrackInfo, TimeSpan>(track, chunk.TimeStamp)));
            }
        }

        public void CancelDownloadStreamData(TrackInfo track)
        {
            if (track != null)
            {
                _chunkDownloadManager.RemoveRequests(track);
            }
        }

        /// <summary>
        /// Downloads the chunk of data that is part of the specified track and has the specified timestamp id.
        /// </summary>
        /// <param name="track">the track that contains the data to be downloaded.</param>
        /// <param name="chunk">the chunk to be downloaded.</param>
        public void DownloadStreamData(TrackInfo track, ChunkInfo chunk)
        {
            if (track != null)
            {
                _chunkDownloadManager.AddRequest(track, chunk.TimeStamp);
            }
        }

        private void ChunkDownloadManager_ChunkDownloadCompleted(ChunkDownloadManager downloadManager, TrackInfo track, TimeSpan timestamp, ChunkResult result)
        {
            var dataChunk = track.Stream.ChunkList.First(i => i.TimeStamp == timestamp);

            if (result.Result == ChunkResult.ChunkResultState.Succeeded)
            {
                OnDownloadStreamDataCompleted(track, dataChunk, result.ChunkData, track.Stream);
                if (DownloadStreamDataCompleted != null) DownloadStreamDataCompleted(this, track, dataChunk, result.ChunkData, track.Stream);
            }
            else
            {
                if (DownloadStreamDataFailed != null) DownloadStreamDataFailed(this, track, dataChunk, result.Error);
            }
        }

        private void ChunkDownloadManager_RetryingChunkDownload(ChunkDownloadManager chunkDownloadManager, TrackInfo mediaTrack, TimeSpan chunkTimestamp)
        {
            //SendLogEntry(KnownLogEntryTypes.SmoothStreamingMediaElementDownloadRetry);
        }

        private void ChunkDownloadManager_ChunkDownloadExceededMaximumRetryAttempts(ChunkDownloadManager chunkDownloadManager, TrackInfo mediaTrack, TimeSpan chunkTimestamp)
        {
            //SendLogEntry(KnownLogEntryTypes.SmoothStreamingMediaElementDownloadExceededMaximumRetryAttempts, LogLevel.Error);

            if (DownloadStreamDataFailed != null) DownloadStreamDataFailed(this, mediaTrack, null, new TimeoutException());
        }

        private void ManifestInfo_ChunkListChanged(object sender, StreamUpdatedEventArgs e)
        {
            if (e.Action == StreamUpdatedEventArgs.StreamUpdatedAction.ChunkAdded)
            {
                if (e.Timestamp.HasValue)
                {
                    var dataChunk = e.Stream.ChunkList.First(i => i.TimeStamp == e.Timestamp);
                    OnStreamDataAdded(e.Stream, dataChunk);
                    if (StreamDataAdded != null) StreamDataAdded(this, e.Stream, dataChunk);
                }
            }
            else if (e.Action == StreamUpdatedEventArgs.StreamUpdatedAction.ChunkRemoved)
            {
                if (e.Timestamp.HasValue)
                {
                    if (StreamDataRemoved != null) StreamDataRemoved(this, e.Stream, e.Timestamp.Value);
                }
            }
        }

        protected virtual void OnStreamDataAdded(StreamInfo stream, ChunkInfo dataChunk)
        {
            if (stream.AvailableTracks.Count() > 0)
            {
                var track = stream.AvailableTracks.First();
                DownloadStreamData(track, dataChunk);
            }
        }

        protected virtual void OnDownloadStreamDataCompleted(TrackInfo track, ChunkInfo dataChunk, Stream streamData, StreamInfo stream)
        {
            if (DataReceived != null)
            {
                streamData.Seek(0, SeekOrigin.Begin);
                int length = (int)streamData.Length;
                var data = new byte[length];
                int count;
                int sum = 0;

                do
                {
                    count = streamData.Read(data, sum, length - sum);
                    sum += count;
                } while (count > 0 && sum < length);

                DataReceived(this, new DataReceivedInfo(data, dataChunk, track, stream));
            }
        }
#endif
        #endregion

        #region Live Playback
        public bool IsLive
        {
            get { return SSME.IsLive; }
        }

#if SILVERLIGHT
#pragma warning disable 0067
        public event EventHandler OutsideWindowEdge;
#pragma warning restore 0067
#else
        public event EventHandler<object> OutsideWindowEdge;
#endif

#if SILVERLIGHT
        public event EventHandler EndOfLive;
#else
        public event EventHandler<object> EndOfLive;
#endif

        public TimeSpan LivePosition { get { return TimeSpan.FromSeconds(SSME.LivePosition); } }

        public TimeSpan StartTime { get { return SSME.StartPosition; } }

        public TimeSpan EndTime { get { return SSME.EndPosition; } }
        #endregion

        #region Audio Streams
        public IList<StreamInfo> AvailableAudioStreams { get { return CurrentSegment.AvailableStreams.Where(IsAudioStream).ToList(); } }

        public StreamInfo SelectedAudioStream
        {
            get
            {
                return CurrentSegment.SelectedStreams.FirstOrDefault(IsAudioStream);
            }
            set
            {
                var oldAudioStream = SelectedAudioStream;
                var newAudioStream = value;

                var selectedStreams = CurrentSegment.SelectedStreams.ToList();
                if (oldAudioStream != null)
                {
                    selectedStreams.Remove(oldAudioStream);
                }
                if (newAudioStream != null)
                {
                    selectedStreams.Add(newAudioStream);
                }
                if (newAudioStream != oldAudioStream)
                {
                    CurrentSegment.SelectStreamsAsync(selectedStreams);
                }
            }
        }
        #endregion

        #region Caption Streams
        public IList<StreamInfo> AvailableCaptionStreams { get { return CurrentSegment.AvailableStreams.Where(IsCaptionStream).ToList(); } }

        public StreamInfo SelectedCaptionStream
        {
            get
            {
                return CurrentSegment.SelectedStreams.FirstOrDefault(IsCaptionStream);
            }
            set
            {
                var oldCaptionStream = SelectedCaptionStream;
                var newCaptionStream = value;

                var selectedStreams = new[] { newCaptionStream };
                var unSelectedStreams = new[] { oldCaptionStream };

                ModifySegmentSelectedStreams(CurrentSegment, selectedStreams, unSelectedStreams);
            }
        }
        #endregion

        #region Bitrates
#if SILVERLIGHT
        public event EventHandler ManifestReady;
#else
        public event EventHandler<object> ManifestReady;
#endif

        public ulong? StartupBitrate { get; set; }

        public ulong MaxBitrate { get; private set; }

        public ulong CurrentBitrate { get; private set; }

        public uint CurrentWidth { get; private set; }

        public uint CurrentHeight { get; private set; }

#if SILVERLIGHT
        public event EventHandler StateChanged;
#else
        public event EventHandler<object> StateChanged;
#endif
        #endregion
    }
}
