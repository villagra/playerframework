using System;
using System.Linq;
using System.Windows.Media;
using Microsoft.Web.Media.SmoothStreaming;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.PlayerFramework.Adaptive
{
    /// <summary>
    /// Provides a wrapper around the SmoothStreamingMediaElement to adapt it to the IMediaElement interface.
    /// </summary>
    public class SmoothStreamingMediaElementWrapper : SmoothStreamingMediaElement, IMediaElement, IDisposable
    {
        TaskCompletionSource<object> templateAppliedTaskSource;
        TimeSpan? pendingSeek;
        bool seekInProgress;
        TimelineMarkerCollection markers;
        event LogReadyRoutedEventHandler logReady;
        TimeSpan? lastMarkerCheckTime;

        /// <summary>
        /// Creates a new instance of SmoothStreamingMediaElementWrapper
        /// </summary>
        public SmoothStreamingMediaElementWrapper()
        {
            markers = new TimelineMarkerCollection();
            base.EnableGPUAcceleration = true;
            base.LogReady += SmoothStreamingMediaElement_LogReady;
            base.SeekCompleted += SmoothStreamingMediaElement_SeekCompleted;
            templateAppliedTaskSource = new TaskCompletionSource<object>();
            base.MediaOpened += SmoothStreamingMediaElementWrapper_MediaOpened;
        }

        void SmoothStreamingMediaElementWrapper_MediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            lastMarkerCheckTime = null;
            markers.Clear();
        }

        /// <inheritdoc /> 
        public Task TemplateAppliedTask { get { return templateAppliedTaskSource.Task; } }

        /// <inheritdoc /> 
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            templateAppliedTaskSource.TrySetResult(null);
        }

        void SmoothStreamingMediaElement_LogReady(object sender, SSMELogReadyRoutedEventArgs e)
        {
            if (logReady != null) logReady(this, new LogReadyRoutedEventArgs(e.Log, e.LogSource));
        }

        /// <inheritdoc /> 
        MediaElementState IMediaElement.CurrentState
        {
            get
            {
                return ConvertState(base.CurrentState);
            }
        }

        internal static MediaElementState ConvertState(SmoothStreamingMediaElementState state)
        {
            if (state != SmoothStreamingMediaElementState.ClipPlaying)
            {
                return (MediaElementState)Enum.Parse(typeof(MediaElementState), state.ToString(), true);
            }
            else
            {
                return MediaElementState.Playing;
            }
        }

        /// <inheritdoc /> 
        event LogReadyRoutedEventHandler IMediaElement.LogReady
        {
            add { logReady += value; }
            remove { logReady -= value; }
        }

        /// <inheritdoc /> 
        void IMediaElement.SetSource(MediaStreamSource mediaStreamSource)
        {
            throw new System.NotImplementedException();
        }

#if !WINDOWS_PHONE
        event RateChangedRoutedEventHandler rateChanged;
        /// <inheritdoc /> 
        event RateChangedRoutedEventHandler IMediaElement.RateChanged
        {
            add { rateChanged += value; }
            remove { rateChanged -= value; }
        }

        double playbackRate;
        /// <inheritdoc /> 
        double IMediaElement.PlaybackRate
        {
            get
            {
                return base.PlaybackRate.GetValueOrDefault(1.0);
            }
            set
            {
                if (value == 0)
                {
                    base.Pause();
                }
                else if (base.SupportedPlaybackRates.Contains(value))
                {
                    if (playbackRate == 0)
                    {
                        base.Play();
                    }
                    base.SetPlaybackRate(value);
                }
                playbackRate = value;
                if (rateChanged != null) rateChanged(this, new RateChangedRoutedEventArgs(value));
            }
        }
#endif

        /// <inheritdoc /> 
        Uri IMediaElement.Source
        {
            get { return base.SmoothStreamingSource ?? base.Source; }
            set
            {
                if (value != null)
                {
                    if (IsSmoothStreaming(value))
                    {
                        base.SmoothStreamingSource = value;
                    }
                    else
                    {
                        base.Source = value;
                    }
                }
                else
                {
                    base.SmoothStreamingSource = null;
                    base.Source = null;
                    seekInProgress = false;
                    pendingSeek = null;
                }
            }
        }

        /// <inheritdoc /> 
        TimeSpan IMediaElement.Position
        {
            get { return base.Position; }
            set
            {
                // reset on seek to prevent MarkerReached from firing.
                lastMarkerCheckTime = null;
                if (base.SmoothStreamingSource != null)
                {
                    if (!seekInProgress)
                    {
                        seekInProgress = true;
                        base.Position = value;
                    }
                    else
                    {
                        pendingSeek = value;
                    }
                }
                else
                {
                    base.Position = value;                
                }
            }
        }

        void SmoothStreamingMediaElement_SeekCompleted(object sender, SeekCompletedEventArgs e)
        {
            if (pendingSeek.HasValue)
            {
                base.Position = pendingSeek.Value;
                pendingSeek = null;
            }
            else
            {
                seekInProgress = false;
            }
        }

        protected virtual bool IsSmoothStreaming(Uri source)
        {
            return (source.OriginalString.IndexOf("/manifest", StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private IMediaElement Interface
        {
            get { return this as IMediaElement; }
        }

#if WINDOWS_PHONE
        /// <inheritdoc /> 
        public event TimelineMarkerRoutedEventHandler MarkerReached;

        /// <inheritdoc /> 
        public TimelineMarkerCollection Markers
        {
            get { return markers; }
        }

        internal void EvaluateMarkers()
        {
            var now = base.Position;
            if (lastMarkerCheckTime.HasValue)
            {
                foreach (var marker in markers.ToList())
                {
                    if (marker.Time <= now && marker.Time > lastMarkerCheckTime.Value)
                    {
                        if (MarkerReached != null) MarkerReached(this, new TimelineMarkerRoutedEventArgs() { Marker = marker });
                    }
                }
            }
            lastMarkerCheckTime = now;
        }
#endif

        void IDisposable.Dispose()
        {
            markers = null;
            base.LogReady -= SmoothStreamingMediaElement_LogReady;
            base.SeekCompleted -= SmoothStreamingMediaElement_SeekCompleted;
            templateAppliedTaskSource = null;
            base.MediaOpened -= SmoothStreamingMediaElementWrapper_MediaOpened;
            base.Dispose();
        }
    }
}
