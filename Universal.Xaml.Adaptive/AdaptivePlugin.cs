using Microsoft.Media.AdaptiveStreaming.Helper;
using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Media;
using Windows.UI.Xaml;
using Windows.Graphics.Display;

namespace Microsoft.PlayerFramework.Adaptive
{
    /// <summary>
    /// Represents a plugin that can be used to automatically intialize the smooth streaming SDK.
    /// </summary>
    public partial class AdaptivePlugin : IPlugin
    {
        bool isLoaded;
        bool autoRestrictSize;
        const string downloaderPluginHttpScheme = "ms-sstr:";
        const string downloaderPluginHttpsScheme = "ms-sstrs:";

        /// <summary>
        /// Creates a new instance of AdaptivePlugin.
        /// </summary>
        public AdaptivePlugin()
        {
            Manager = new AdaptiveStreamingManager();
            SchemeHandlers = new List<SchemeHandler>();
            SchemeHandlers.Add(new SchemeHandler(downloaderPluginHttpScheme));
            SchemeHandlers.Add(new SchemeHandler(downloaderPluginHttpsScheme));
            ByteStreamHandlers = new List<ByteStreamHandler>();
            ByteStreamHandlers.Add(new ByteStreamHandler(".ism", "text/xml"));
            ByteStreamHandlers.Add(new ByteStreamHandler(".ism", "application/vnd.ms-sstr+xml"));
            AutoRestrictSize = true;
            AutoSchemeDownloaderPlugin = true;
        }

        /// <summary>
        /// Gets or sets whether to automatically prevent video tracks greater than the size of the player from ever being chosen.
        /// </summary>
        public bool AutoRestrictSize
        {
            get { return autoRestrictSize; }
            set
            {
                if (autoRestrictSize != value)
                {
                    autoRestrictSize = value;
                    if (isLoaded)
                    {
                        UpdateMaxSize(new Size(MediaPlayer.ActualWidth, MediaPlayer.ActualHeight));
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum bitrate that can be used for video. This is useful for limiting the bitrate under metered network situations.
        /// Note: additional tracks will also be unselected based on the size of the player if AutoRestrictSize = false.
        /// </summary>
        public uint? MaxBitrate
        {
            get { return Manager.MaxBitrate; }
            set { Manager.MaxBitrate = value; }
        }

        /// <summary>
        /// Gets or sets the Minimum bitrate that can be used for video. This is useful for limiting the bitrate so quality never dips below a certain amount.
        /// Note: additional tracks will also be unselected based on the size of the player if AutoRestrictSize = false.
        /// </summary>
        public uint? MinBitrate
        {
            get { return Manager.MinBitrate; }
            set { Manager.MinBitrate = value; }
        }

        /// <summary>
        /// Gets or sets the startup bitrate to be used. This is useful for starting at a higher quality when you know the user has a good connection.
        /// </summary>
        public uint? StartupBitrate
        {
            get { return Manager.StartupBitrate; }
            set { Manager.StartupBitrate = value; }
        }

        /// <summary>
        /// Gets or sets whether media URL schemes should automatically use the custom scheme 'ms-sstr:' in order to automatically invoke the DownloaderPlugin. Default true.
        /// </summary>
        public bool AutoSchemeDownloaderPlugin { get; set; }

        /// <summary>
        /// Gets the instance of the WinRT AdaptiveStreamingManager class used to communicate with the Smooth Streaming SDK.
        /// </summary>
        public AdaptiveStreamingManager Manager { get; private set; }

        /// <summary>
        /// Gets or sets the list of byte stream handlers that should be registered with the MediaExtensionManager for the Smooth Streaming SDK.
        /// </summary>
        public IList<ByteStreamHandler> ByteStreamHandlers { get; set; }

        /// <summary>
        /// Gets or sets whether in-stream text tracks should be used as captions. Default = false.
        /// </summary>
        public bool InstreamCaptionsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the list of scheme handlers that should be registered with the MediaExtensionManager for the Smooth Streaming SDK.
        /// </summary>
        public IList<SchemeHandler> SchemeHandlers { get; set; }

        void manager_OutsideWindowEdge(object sender, object e)
        {
            // scenarios:
            // 1) FF past LivePosition
            // 2) Paused or in slow motion and StartTime caught up with current position
            if (MediaPlayer.AdvertisingState != AdvertisingState.Loading && MediaPlayer.AdvertisingState != AdvertisingState.Linear)
            {
                MediaPlayer.PlayResume();
            }
        }

        void manager_EndOfLive(object sender, object e)
        {
            MediaPlayer.LivePosition = null;
            MediaPlayer.IsLive = false;
        }

        void manager_TimesChanged(object sender, object e)
        {
            MediaPlayer.StartTime = new TimeSpan(Manager.StartTime);
            MediaPlayer.LivePosition = Manager.IsLive ? new TimeSpan(Manager.LivePosition) : (TimeSpan?)null;
            MediaPlayer.EndTime = new TimeSpan(Manager.EndTime);
        }

        void manager_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (InstreamCaptionsEnabled)
            {
                var captionWrapper = MediaPlayer.SelectedCaption as CaptionStreamWrapper;
                if (captionWrapper != null && captionWrapper.AdaptiveCaptionStream.ManifestStream.Name == e.Stream.Name)
                {
                    MediaPlayer.SelectedCaption.AugmentPayload(e.Data, new TimeSpan(e.StartTime), new TimeSpan(e.EndTime));
                }
            }
        }

        void manager_StateChanged(object sender, object e)
        {
            UpdateQuality();
        }

        /// <summary>
        /// Called when the selected video track has changed. Used to update the signal strength and media quality properties on the MediaPlayer.
        /// </summary>
        protected virtual void UpdateQuality()
        {
            MediaPlayer.SignalStrength = Math.Min(1.0, Math.Max(0.0, (double)Manager.CurrentBitrate / Manager.HighestBitrate));
            MediaPlayer.MediaQuality = Manager.CurrentHeight >= 720 ? MediaQuality.HighDefinition : MediaQuality.StandardDefinition;
        }

        void manager_ManifestReady(object sender, object e)
        {
            MediaPlayer.IsLive = Manager.IsLive;

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
        }
        
        void MediaPlayer_SelectedCaptionChanged(object sender, RoutedPropertyChangedEventArgs<Caption> e)
        {
            if (InstreamCaptionsEnabled)
            {
                if (Manager.IsOpen)
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

        void MediaPlayer_SelectedAudioStreamChanged(object sender, SelectedAudioStreamChangedEventArgs e)
        {
            if (Manager.IsOpen)
            {
                var audioStreamWrapper = e.NewValue as AudioStreamWrapper;
                if (e.NewValue == null || audioStreamWrapper != null)
                {
                    var newAudioStream = audioStreamWrapper != null ? audioStreamWrapper.AdaptiveAudioStream : null;
                    if (Manager.SelectedAudioStream != newAudioStream)
                    {
                        Manager.SelectedAudioStream = newAudioStream;
                    }
                    e.Handled = true;
                }
            }
        }

        void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            Manager.MediaReady();
        }

        void MediaPlayer_UpdateCompleted(object sender, RoutedEventArgs e)
        {
            Manager.RefreshState(MediaPlayer.Position);
        }

        void MediaPlayer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateMaxSize(e.NewSize);
        }

        private void UpdateMaxSize(Size size)
        {
            if (AutoRestrictSize)
            {
#if WINDOWS_PHONE_APP
                var scale = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
#elif WINDOWS80
                var scale = (double)(int)DisplayProperties.ResolutionScale / 100;
#else
                var scale = (double)(int)DisplayInformation.GetForCurrentView().ResolutionScale / 100;
#endif
                Manager.MaxSize = new Size(Math.Round(size.Width * scale), Math.Round(size.Height * scale));
            }
            else
            {
                Manager.MaxSize = new Size?();
            }
        }

        void MediaPlayer_MediaLoading(object sender, MediaPlayerDeferrableEventArgs e)
        {
            var mediaLoadingEventArgs = e as MediaLoadingEventArgs;

            if (DownloaderPlugin != null && AutoSchemeDownloaderPlugin)
            {
                var src = mediaLoadingEventArgs.Source;
                var newSrc = new Uri(src.OriginalString.Replace("http:", downloaderPluginHttpScheme).Replace("https:", downloaderPluginHttpsScheme));
                mediaLoadingEventArgs.Source = newSrc;
            }

            if (!Manager.IsInitialized) // only do this the first time
            {
                Manager.Initialize(MediaPlayer.MediaExtensionManager);
                foreach (var handler in ByteStreamHandlers)
                {
                    Manager.RegisterByteStreamHandler(handler.FileExtension, handler.MimeType);
                }
                foreach (var handler in SchemeHandlers)
                {
                    Manager.RegisterSchemeHandler(handler.Scheme);
                }
            }

            Manager.SourceUri = mediaLoadingEventArgs.Source;
        }

        void IPlugin.Load()
        {
            UpdateMaxSize(new Size(MediaPlayer.ActualWidth, MediaPlayer.ActualHeight));

            Manager.DataReceived += manager_DataReceived;
            Manager.StateChanged += manager_StateChanged;
            Manager.ManifestReady += manager_ManifestReady;
            Manager.TimesChanged += manager_TimesChanged;
            Manager.EndOfLive += manager_EndOfLive;
            Manager.OutsideWindowEdge += manager_OutsideWindowEdge;
            MediaPlayer.SelectedAudioStreamChanged += MediaPlayer_SelectedAudioStreamChanged;
            MediaPlayer.SelectedCaptionChanged += MediaPlayer_SelectedCaptionChanged;
            MediaPlayer.SizeChanged += MediaPlayer_SizeChanged;
            MediaPlayer.MediaLoading += MediaPlayer_MediaLoading;
            MediaPlayer.UpdateCompleted += MediaPlayer_UpdateCompleted;
            MediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            isLoaded = true;
        }

        void IPlugin.Update(IMediaSource mediaSource)
        {
        }

        void IPlugin.Unload()
        {
            MediaPlayer.MediaLoading -= MediaPlayer_MediaLoading;
            MediaPlayer.SelectedAudioStreamChanged -= MediaPlayer_SelectedAudioStreamChanged;
            MediaPlayer.SelectedCaptionChanged -= MediaPlayer_SelectedCaptionChanged;
            MediaPlayer.SizeChanged -= MediaPlayer_SizeChanged;
            MediaPlayer.UpdateCompleted -= MediaPlayer_UpdateCompleted;
            Manager.DataReceived -= manager_DataReceived;
            Manager.StateChanged -= manager_StateChanged;
            Manager.ManifestReady -= manager_ManifestReady;
            Manager.TimesChanged -= manager_TimesChanged;
            Manager.EndOfLive -= manager_EndOfLive;
            Manager.OutsideWindowEdge -= manager_OutsideWindowEdge;
            MediaPlayer.MediaOpened -= MediaPlayer_MediaOpened;
            Manager.Uninitialize();
            isLoaded = false;
        }

        private MediaPlayer MediaPlayer;

        MediaPlayer IPlugin.MediaPlayer
        {
            get { return MediaPlayer; }
            set { MediaPlayer = value; }
        }

        /// <summary>
        /// Gets or set the DownloaderPlugin to be used.
        /// This is a class that allows you to intercept each request to the Smooth Streaming SDK before it tries to process the data.
        /// </summary>
        public Microsoft.Media.AdaptiveStreaming.IDownloaderPlugin DownloaderPlugin
        {
            get { return Manager.DownloaderPlugin; }
            set { Manager.DownloaderPlugin = value; }
        }
    }

    /// <summary>
    /// Provides a way to store data associated with MediaExtensionManager.RegisterByteStreamHandler
    /// </summary>
    public sealed class ByteStreamHandler
    {
        /// <summary>
        /// Creates a new instance of ByteStreamHandler.
        /// </summary>
        public ByteStreamHandler() { }

        /// <summary>
        /// Creates and initializes a new instance of ByteStreamHandler.
        /// </summary>
        /// <param name="fileExtension">The file name extension that is registered for this byte-stream handler.</param>
        /// <param name="mimeType">The MIME type that is registered for this byte-stream handler.</param>
        public ByteStreamHandler(string fileExtension, string mimeType)
        {
            FileExtension = fileExtension;
            MimeType = mimeType;
        }

        /// <summary>
        /// The file name extension that is registered for this byte-stream handler.
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// The MIME type that is registered for this byte-stream handler.
        /// </summary>
        public string MimeType { get; set; }
    }

    /// <summary>
    /// Provides a way to store data associated with MediaExtensionManager.RegisterSchemeHandler
    /// </summary>
    public sealed class SchemeHandler
    {
        /// <summary>
        /// Creates a new instance of ByteStreamHandler.
        /// </summary>
        public SchemeHandler() { }

        /// <summary>
        /// Creates and initializes a new instance of ByteStreamHandler.
        /// </summary>
        /// <param name="scheme">The URL scheme that will be recognized to invoke the scheme handler. For example, myscheme://.</param>
        public SchemeHandler(string scheme)
        {
            Scheme = scheme;
        }

        /// <summary>
        /// The URL scheme that will be recognized to invoke the scheme handler. For example, myscheme://.
        /// </summary>
        public string Scheme { get; set; }
    }
}
