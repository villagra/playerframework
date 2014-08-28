using System;
using System.Windows;
using Microsoft.Web.Media.SmoothStreaming;
using System.Collections.Generic;
using Microsoft.Media.AdaptiveStreaming.Helper;

namespace Microsoft.PlayerFramework.Adaptive
{
    /// <summary>
    /// Provides an optional plugin to connect the SmoothStreamingMediaElement to the MediaPlayer
    /// </summary>
    public class AdaptivePlugin : IMediaPlugin
    {
        /// <summary>
        /// Gets the AdaptiveStreamingManager instance used to broker communication between the SmoothStreamingMediaElement and the MediaPlayer
        /// </summary>
        public AdaptiveStreamingManager Manager { get; private set; }

        IMediaElement mediaElement;
        bool InManifestReady;

        /// <summary>
        /// Creates a new instance of AdaptivePlugin.
        /// </summary>
        public AdaptivePlugin()
        {
            Manager = new AdaptiveStreamingManager();
        }

#if WINDOWS_PHONE
        /// <summary>
        /// Gets or sets whether to automatically restrict tracks unsuitable for platform and device. Sometimes the app knows best and should perform this duty itself.
        /// </summary>
        public bool AutoRestrictTracks
        {
            get { return Manager.AutoRestrictTracks; }
            set { Manager.AutoRestrictTracks = value; }
        }
#endif
        
#if !WINDOWS_PHONE
        /// <summary>
        /// Gets or sets whether in-stream text tracks should be used as captions. Default = false.
        /// </summary>
        public bool InstreamCaptionsEnabled { get; set; }
#endif

        /// <summary>
        /// Gets or sets the startup bitrate to be used. This is useful for starting at a higher quality when you know the user has a good connection.
        /// </summary>
        public ulong? StartupBitrate
        {
            get { return Manager.StartupBitrate; }
            set { Manager.StartupBitrate = value; }
        }

        /// <summary>
        /// Gets or set the DownloaderPlugin to be used.
        /// This is a class that allows you to intercept each request to the Smooth Streaming SDK before it tries to process the data.
        /// Note: IDownloaderPlugin just adapts ISmoothStreamingCache to use the async/await pattern.
        /// </summary>
        public IDownloaderPlugin DownloaderPlugin
        {
            get { return Manager.DownloaderPlugin; }
            set { Manager.DownloaderPlugin = value; }
        }

        /// <inheritdoc />
        public void Load()
        {
            MediaPlayer.SupportedPlaybackRates = new List<double>(new[] { -8.0, -4.0, 0.5, 0, 1, 4.0, 8.0 });
            MediaPlayer.BufferingTime = (TimeSpan)GetDefaultValue(SmoothStreamingMediaElement.BufferingTimeProperty);
            Manager.Initialize(SSME);

            Manager.ManifestReady += Manager_ManifestReady;
            Manager.StateChanged += Manager_StateChanged;
            Manager.EndOfLive += Manager_EndOfLive;
            Manager.OutsideWindowEdge += Manager_OutsideWindowEdge;
#if !WINDOWS_PHONE
            Manager.DataReceived += Manager_DataReceived;
            MediaPlayer.SelectedCaptionChanged += MediaPlayer_SelectedCaptionChanged;
#endif
            MediaPlayer.SelectedAudioStreamChanged += MediaPlayer_SelectedAudioStreamChanged;
            MediaPlayer.UpdateCompleted += MediaPlayer_UpdateCompleted;
#if WINDOWS_PHONE
            MediaPlayer.MediaLoading += MediaPlayer_MediaLoading;
            MediaPlayer.MediaClosed += MediaPlayer_MediaClosed;
#endif
        }

        /// <inheritdoc />
        public void Update(IMediaSource mediaSource)
        {

        }

        /// <inheritdoc />
        public void Unload()
        {
            Manager.ManifestReady -= Manager_ManifestReady;
            Manager.StateChanged -= Manager_StateChanged;
            Manager.EndOfLive -= Manager_EndOfLive;
            Manager.OutsideWindowEdge -= Manager_OutsideWindowEdge;
#if !WINDOWS_PHONE
            Manager.DataReceived -= Manager_DataReceived;
            MediaPlayer.SelectedCaptionChanged -= MediaPlayer_SelectedCaptionChanged;
#endif
            MediaPlayer.SelectedAudioStreamChanged -= MediaPlayer_SelectedAudioStreamChanged;
            MediaPlayer.UpdateCompleted -= MediaPlayer_UpdateCompleted;
#if WINDOWS_PHONE
            MediaPlayer.MediaLoading -= MediaPlayer_MediaLoading;
            MediaPlayer.MediaClosed -= MediaPlayer_MediaClosed;
#endif
            Manager.Uninitialize();

            if (mediaElement is IDisposable) ((IDisposable)mediaElement).Dispose();
            mediaElement = null;
        }

        void Manager_ManifestReady(object sender, EventArgs e)
        {
            MediaPlayer.IsLive = SSME.IsLive;
            InManifestReady = true;
            try
            {
                MediaPlayer.AvailableAudioStreams.Clear();

                foreach (var audioStream in Manager.AvailableAudioStreams)
                {
                    var wrapper = new AudioStreamWrapper(audioStream);
                    MediaPlayer.AvailableAudioStreams.Add(wrapper);
                    if (audioStream == Manager.SelectedAudioStream)
                    {
                        MediaPlayer.SelectedAudioStream = wrapper;
                    }
                }
                
#if !WINDOWS_PHONE
                if (InstreamCaptionsEnabled)
                {
                    MediaPlayer.AvailableCaptions.Clear();
                    foreach (var captionStream in Manager.AvailableCaptionStreams)
                    {
                        var wrapper = new CaptionStreamWrapper(captionStream);
                        MediaPlayer.AvailableCaptions.Add(wrapper);
                        if (captionStream == Manager.SelectedCaptionStream)
                        {
                            MediaPlayer.SelectedCaption = wrapper;
                        }
                    }
                }
#endif
            }
            finally
            {
                InManifestReady = false;
            }
        }

        void Manager_OutsideWindowEdge(object sender, EventArgs e)
        {
            // scenarios:
            // 1) FF past LivePosition
            // 2) Paused or in slow motion and StartTime caught up with current position
            if (MediaPlayer.AdvertisingState != AdvertisingState.Loading && MediaPlayer.AdvertisingState != AdvertisingState.Linear)
            {
                MediaPlayer.PlayResume();
            }
        }

        void Manager_EndOfLive(object sender, EventArgs e)
        {
            MediaPlayer.Dispatcher.BeginInvoke(() =>
            {
                if (MediaPlayer != null)
                {
                    MediaPlayer.LivePosition = null;
                    MediaPlayer.IsLive = false;
                }
            });
        }

        void Manager_StateChanged(object sender, EventArgs e)
        {
            MediaPlayer.SignalStrength = (double)Manager.CurrentBitrate / Manager.MaxBitrate;
            MediaPlayer.MediaQuality = Manager.CurrentHeight >= 720 ? MediaQuality.HighDefinition : MediaQuality.StandardDefinition;
        }
        
#if !WINDOWS_PHONE
        void Manager_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (InstreamCaptionsEnabled)
            {
                var captionWrapper = MediaPlayer.SelectedCaption as CaptionStreamWrapper;
                if (captionWrapper != null && captionWrapper.AdaptiveCaptionStream.UniqueId == e.Stream.UniqueId)
                {
                    MediaPlayer.SelectedCaption.AugmentPayload(e.Data, new TimeSpan(e.StartTime), new TimeSpan(e.EndTime));
                }
            }
        }
#endif

#if WINDOWS_PHONE
        void MediaPlayer_MediaLoading(object sender, MediaPlayerDeferrableEventArgs e)
        {
            if (Manager.DownloaderPlugin is ILifetimeAwareDownloaderPlugin)
            {
                var args = e as MediaLoadingEventArgs;
                ((ILifetimeAwareDownloaderPlugin)Manager.DownloaderPlugin).OnOpenMedia(args.Source);
            }
        }

        void MediaPlayer_MediaClosed(object sender, RoutedEventArgs e)
        {
            if (Manager.DownloaderPlugin is ILifetimeAwareDownloaderPlugin)
            {
                ((ILifetimeAwareDownloaderPlugin)Manager.DownloaderPlugin).OnCloseMedia(MediaPlayer.Source);
            }
        }
#endif

        void MediaPlayer_UpdateCompleted(object sender, RoutedEventArgs e)
        {
            if (SSME.SmoothStreamingSource != null)
            {
                MediaPlayer.StartTime = Manager.StartTime;
                MediaPlayer.LivePosition = Manager.IsLive ? Manager.LivePosition : (TimeSpan?)null;
                MediaPlayer.EndTime = Manager.EndTime;
            }
#if WINDOWS_PHONE
            var smoothMediaElement = mediaElement as SmoothStreamingMediaElementWrapper;
            if (smoothMediaElement != null)
            {
                smoothMediaElement.EvaluateMarkers();
            }
#endif
        }
        
#if !WINDOWS_PHONE
        void MediaPlayer_SelectedCaptionChanged(object sender, RoutedPropertyChangedEventArgs<Caption> e)
        {
            if (InstreamCaptionsEnabled)
            {
                if (SSME.SmoothStreamingSource != null)
                {
                    if (!InManifestReady)
                    {
                        var CaptionStreamWrapper = e.NewValue as CaptionStreamWrapper;
                        if (e.NewValue == null || CaptionStreamWrapper != null)
                        {
                            var newCaptionStream = CaptionStreamWrapper != null ? CaptionStreamWrapper.AdaptiveCaptionStream : null;
                            if (Manager.SelectedCaptionStream != newCaptionStream)
                            {
                                Manager.SelectedCaptionStream = newCaptionStream;
                            }
                        }
                    }
                }
            }
        }
#endif

        void MediaPlayer_SelectedAudioStreamChanged(object sender, SelectedAudioStreamChangedEventArgs e)
        {
            if (SSME.SmoothStreamingSource != null)
            {
                if (!InManifestReady)
                {
                    var newAudioStream = e.NewValue is AudioStreamWrapper ? ((AudioStreamWrapper)e.NewValue).AdaptiveAudioStream : null;
                    Manager.SelectedAudioStream = newAudioStream;
                }
                e.Handled = true;
            }
        }

        /// <inheritdoc />
        public MediaPlayer MediaPlayer { get; set; }

        /// <summary>
        /// Gets the underlying SmoothStreamingMediaElement being used by this plugin.
        /// </summary>
        public SmoothStreamingMediaElement SSME { get { return mediaElement as SmoothStreamingMediaElement; } }

        /// <inheritdoc />
        public virtual IMediaElement MediaElement
        {
            get
            {
                return mediaElement = mediaElement ?? new SmoothStreamingMediaElementWrapper();
            }
            protected set
            {
                mediaElement = value;
            }
        }

        static object GetDefaultValue(DependencyProperty dp)
        {
            return dp.GetMetadata(typeof(SmoothStreamingMediaElement)).DefaultValue;
        }
    }
}
