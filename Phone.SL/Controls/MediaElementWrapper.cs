using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Wraps the MediaElement to allow it to adhere to the IMediaElement interface.
    /// IMediaElement is used to allow the SmoothStreamingMediaElement or other custom MediaElements to be used by MediaPlayer
    /// </summary>
    public class MediaElementWrapper : ContentControl, IMediaElement
    {
        readonly TaskCompletionSource<object> templateAppliedTaskSource;

        /// <summary>
        /// The MediaElement being wrapped
        /// </summary>
        MediaElement mediaElement;

        /// <summary>
        /// The underlying MediaElement being wrapped
        /// </summary>
        protected MediaElement MediaElement
        {
            get { return mediaElement; }
            private set
            {
                if (mediaElement != null)
                {
                    mediaElement.CurrentStateChanged -= mediaElement_CurrentStateChanged;
                    mediaElement.LogReady -= mediaElement_LogReady;
#if !WINDOWS_PHONE
                    mediaElement.RateChanged -= mediaElement_RateChanged;
#endif
                }

                mediaElement = value;

                if (mediaElement != null)
                {
                    mediaElement.CurrentStateChanged += mediaElement_CurrentStateChanged;
                    mediaElement.LogReady += mediaElement_LogReady;
#if !WINDOWS_PHONE
                    mediaElement.RateChanged += mediaElement_RateChanged;
#endif
                }
            }
        }

        /// <summary>
        /// Creates a new instance of the MediaElementWrapper class.
        /// </summary>
        public MediaElementWrapper()
        {
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            this.VerticalContentAlignment = VerticalAlignment.Stretch;
            MediaElement = new MediaElement();
#if WINDOWS_PHONE
            MediaElement.MarkerReached += MediaElement_MarkerReached;
#endif
            this.Content = MediaElement;
            templateAppliedTaskSource = new TaskCompletionSource<object>();
        }

        /// <inheritdoc /> 
        public Task TemplateAppliedTask { get { return templateAppliedTaskSource.Task; } }

        /// <inheritdoc /> 
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            templateAppliedTaskSource.TrySetResult(null);
        }

        /// <inheritdoc /> 
        public event RoutedEventHandler BufferingProgressChanged
        {
            add
            {
                MediaElement.BufferingProgressChanged += value;
            }
            remove
            {
                MediaElement.BufferingProgressChanged -= value;
            }
        }

        /// <inheritdoc /> 
        public event RoutedEventHandler CurrentStateChanged;

        /// <inheritdoc /> 
        public event RoutedEventHandler DownloadProgressChanged
        {
            add
            {
                MediaElement.DownloadProgressChanged += value;
            }
            remove
            {
                MediaElement.DownloadProgressChanged -= value;
            }
        }

        void mediaElement_LogReady(object sender, System.Windows.Media.LogReadyRoutedEventArgs e)
        {
            if (LogReady != null) LogReady(this, new LogReadyRoutedEventArgs(e.Log, e.LogSource));
        }

        void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (CurrentStateChanged != null) CurrentStateChanged(sender, e);
        }

        /// <inheritdoc /> 
        public event LogReadyRoutedEventHandler LogReady;


#if WINDOWS_PHONE
        void MediaElement_MarkerReached(object sender, System.Windows.Media.TimelineMarkerRoutedEventArgs e)
        {
            if (MarkerReached != null) MarkerReached(this, new TimelineMarkerRoutedEventArgs() { Marker = e.Marker });
        }

        /// <inheritdoc /> 
        public event TimelineMarkerRoutedEventHandler MarkerReached;
#else   
        /// <inheritdoc /> 
        public event TimelineMarkerRoutedEventHandler MarkerReached
        {
            add
            {
                MediaElement.MarkerReached += value;
            }
            remove
            {
                MediaElement.MarkerReached -= value;
            }
        }
#endif


        /// <inheritdoc /> 
        public event RoutedEventHandler MediaEnded
        {
            add
            {
                MediaElement.MediaEnded += value;
            }
            remove
            {
                MediaElement.MediaEnded -= value;
            }
        }

        /// <inheritdoc /> 
        public event EventHandler<ExceptionRoutedEventArgs> MediaFailed
        {
            add
            {
                MediaElement.MediaFailed += value;
            }
            remove
            {
                MediaElement.MediaFailed -= value;
            }
        }

        /// <inheritdoc /> 
        public event RoutedEventHandler MediaOpened
        {
            add
            {
                MediaElement.MediaOpened += value;
            }
            remove
            {
                MediaElement.MediaOpened -= value;
            }
        }

#if !WINDOWS_PHONE
        void mediaElement_RateChanged(object sender, System.Windows.Media.RateChangedRoutedEventArgs e)
        {
            if (RateChanged != null) RateChanged(this, new RateChangedRoutedEventArgs(e.NewRate));
        }

        /// <inheritdoc /> 
        public event RateChangedRoutedEventHandler RateChanged;
        
        /// <inheritdoc /> 
        public System.Collections.Generic.Dictionary<string, string> Attributes
        {
            get { return MediaElement.Attributes; }
        }

        /// <inheritdoc /> 
        public bool IsDecodingOnGPU
        {
            get { return MediaElement.IsDecodingOnGPU; }
        }

        /// <inheritdoc /> 
        public double PlaybackRate
        {
            get { return MediaElement.PlaybackRate; }
            set { MediaElement.PlaybackRate = value; }
        }
#endif

        /// <inheritdoc /> 
        public void Pause()
        {
            MediaElement.Pause();
        }

        /// <inheritdoc /> 
        public void Play()
        {
            MediaElement.Play();
        }

        /// <inheritdoc /> 
        public void RequestLog()
        {
            MediaElement.RequestLog();
        }

        /// <inheritdoc /> 
        public void SetSource(System.IO.Stream stream)
        {
            MediaElement.SetSource(stream);
        }

        /// <inheritdoc /> 
        public void SetSource(MediaStreamSource mediaStreamSource)
        {
            MediaElement.SetSource(mediaStreamSource);
        }

        /// <inheritdoc /> 
        public void Stop()
        {
            MediaElement.Stop();
        }

        /// <inheritdoc /> 
        public int AudioStreamCount
        {
            get { return MediaElement.AudioStreamCount; }
        }

        /// <inheritdoc /> 
        public int? AudioStreamIndex
        {
            get { return MediaElement.AudioStreamIndex; }
            set { MediaElement.AudioStreamIndex = value; }
        }

        /// <inheritdoc /> 
        public bool AutoPlay
        {
            get { return MediaElement.AutoPlay; }
            set { MediaElement.AutoPlay = value; }
        }

        /// <inheritdoc /> 
        public double Balance
        {
            get { return MediaElement.Balance; }
            set { MediaElement.Balance = value; }
        }

        /// <inheritdoc /> 
        public double BufferingProgress
        {
            get { return MediaElement.BufferingProgress; }
        }

        /// <inheritdoc /> 
        public TimeSpan BufferingTime
        {
            get { return MediaElement.BufferingTime; }
            set { MediaElement.BufferingTime = value; }
        }

        /// <inheritdoc /> 
        public bool CanPause
        {
            get { return MediaElement.CanPause; }
        }

        /// <inheritdoc /> 
        public bool CanSeek
        {
            get { return MediaElement.CanSeek; }
        }

        /// <inheritdoc /> 
        public MediaElementState CurrentState
        {
            get { return MediaElement.CurrentState; }
        }

        /// <inheritdoc /> 
        public double DownloadProgress
        {
            get { return MediaElement.DownloadProgress; }
        }

        /// <inheritdoc /> 
        public double DownloadProgressOffset
        {
            get { return MediaElement.DownloadProgressOffset; }
        }

        /// <inheritdoc /> 
        public double DroppedFramesPerSecond
        {
            get { return MediaElement.DroppedFramesPerSecond; }
        }

        /// <inheritdoc /> 
        public bool IsMuted
        {
            get { return MediaElement.IsMuted; }
            set { MediaElement.IsMuted = value; }
        }

        /// <inheritdoc /> 
        public LicenseAcquirer LicenseAcquirer
        {
            get { return MediaElement.LicenseAcquirer; }
            set { MediaElement.LicenseAcquirer = value; }
        }

        /// <inheritdoc /> 
        public TimelineMarkerCollection Markers
        {
            get { return MediaElement.Markers; }
        }

        /// <inheritdoc /> 
        public Duration NaturalDuration
        {
            get { return MediaElement.NaturalDuration; }
        }

        /// <inheritdoc /> 
        public int NaturalVideoHeight
        {
            get { return MediaElement.NaturalVideoHeight; }
        }

        /// <inheritdoc /> 
        public int NaturalVideoWidth
        {
            get { return MediaElement.NaturalVideoWidth; }
        }

        /// <inheritdoc /> 
        public TimeSpan Position
        {
            get { return MediaElement.Position; }
            set { MediaElement.Position = value; }
        }

        /// <inheritdoc /> 
        public double RenderedFramesPerSecond
        {
            get { return MediaElement.RenderedFramesPerSecond; }
        }

        /// <inheritdoc /> 
        public Uri Source
        {
            get { return MediaElement.Source; }
            set
            {
                MediaElement.Source = value;
                if (value == null)  // hack: MediaElement doesn't raise CurrentStateChanged on its own
                {
                    if (CurrentStateChanged != null) CurrentStateChanged(this, new RoutedEventArgs());
                }
            }
        }

        /// <inheritdoc /> 
        public Stretch Stretch
        {
            get { return MediaElement.Stretch; }
            set { MediaElement.Stretch = value; }
        }

        /// <inheritdoc /> 
        public double Volume
        {
            get { return MediaElement.Volume; }
            set { MediaElement.Volume = value; }
        }

        #region Position
        /// <summary>
        /// Identifies the Position dependency property.
        /// </summary>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(TimeSpan), typeof(MediaElementWrapper), new PropertyMetadata(TimeSpan.Zero));

        #endregion
    }
}
