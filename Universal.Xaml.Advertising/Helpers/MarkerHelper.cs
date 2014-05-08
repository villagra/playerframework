using System;
using System.Collections.Generic;
using System.Linq;
#if SILVERLIGHT
using System.Windows.Media;
using System.Windows.Threading;
#else
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework.Advertising
{
    internal sealed class MarkerHelper
    {
        readonly List<TimelineMarker> reachedMarkers = new List<TimelineMarker>();
        readonly DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(250) };
        DateTime startTime;

        public TimelineMarkerCollection Markers { get; private set; }

        bool isPaused = true;
        DateTime pauseTime;

        public event EventHandler<MarkerReachedEventArgs> MarkerReached;

        public MarkerHelper()
        {
            Markers = new TimelineMarkerCollection();
        }

#if SILVERLIGHT
        void timer_Tick(object sender, EventArgs e)
#else
        void timer_Tick(object sender, object e)
#endif
        {
            foreach (var marker in Markers.Except(reachedMarkers).Where(m => m.Time < Position).ToList())
            {
                reachedMarkers.Add(marker);
                if (MarkerReached != null) MarkerReached(this, new MarkerReachedEventArgs(marker));
            }
        }

        public TimeSpan Position { get { return CurrentTime.Subtract(startTime); } }

        DateTime CurrentTime
        {
            get
            {
                return isPaused ? pauseTime : DateTime.Now;
            }
        }

        public void Start()
        {
            if (isPaused)
            {
                isPaused = false;
                startTime = DateTime.Now;
                timer.Tick += timer_Tick;
                timer.Start();
            }
        }

        public void Resume()
        {
            if (isPaused)
            {
                startTime = DateTime.Now.Subtract(Position);
                isPaused = false;
                timer.Tick += timer_Tick;
                timer.Start();
            }
        }

        public void Stop()
        {
            if (!isPaused)
            {
                pauseTime = DateTime.Now;
                isPaused = true;
                timer.Tick -= timer_Tick;
                timer.Stop();
            }
        }
    }

    internal sealed class MarkerReachedEventArgs
#if SILVERLIGHT
        : EventArgs
#endif
    {
        internal MarkerReachedEventArgs(TimelineMarker marker)
        {
            Marker = marker;
        }

        public TimelineMarker Marker { get; private set; }
    }
}
