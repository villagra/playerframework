using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Microsoft.PlayerFramework.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TrackingPage : Microsoft.PlayerFramework.Samples.Common.LayoutAwarePage
    {
        public TrackingPage()
        {
            this.InitializeComponent();

            var positionTrackingPlugin = new PositionTrackingPlugin();
            player.Plugins.Add(positionTrackingPlugin);
            positionTrackingPlugin.EventTracked += trackingPlugin_EventTracked;

            positionTrackingPlugin.TrackingEvents.Add(new PositionTrackingEvent() { PositionPercentage = 0, Data = "PositionTrackingEvent: 0%" });
            positionTrackingPlugin.TrackingEvents.Add(new PositionTrackingEvent() { PositionPercentage = .25, Data = "PositionTrackingEvent: 25%" });
            positionTrackingPlugin.TrackingEvents.Add(new PositionTrackingEvent() { PositionPercentage = .50, Data = "PositionTrackingEvent: 50%" });
            positionTrackingPlugin.TrackingEvents.Add(new PositionTrackingEvent() { PositionPercentage = .75, Data = "PositionTrackingEvent: 75%" });
            positionTrackingPlugin.TrackingEvents.Add(new PositionTrackingEvent() { PositionPercentage = 1, Data = "PositionTrackingEvent: 100%" });

            positionTrackingPlugin.TrackingEvents.Add(new PositionTrackingEvent() { Position = TimeSpan.FromSeconds(5), Data = "PositionTrackingEvent: 5 seconds" });
            positionTrackingPlugin.TrackingEvents.Add(new PositionTrackingEvent() { Position = TimeSpan.FromSeconds(15), Data = "PositionTrackingEvent: 15 seconds" });
            positionTrackingPlugin.TrackingEvents.Add(new PositionTrackingEvent() { Position = TimeSpan.FromSeconds(25), Data = "PositionTrackingEvent: 25 seconds" });

            var playTimeTrackingPlugin = new PlayTimeTrackingPlugin();
            player.Plugins.Add(playTimeTrackingPlugin);
            playTimeTrackingPlugin.EventTracked += trackingPlugin_EventTracked;

            playTimeTrackingPlugin.TrackingEvents.Add(new PlayTimeTrackingEvent() { PlayTimePercentage = 0, Data = "PlayTimeTrackingEvent: 0%" });
            playTimeTrackingPlugin.TrackingEvents.Add(new PlayTimeTrackingEvent() { PlayTimePercentage = .50, Data = "PlayTimeTrackingEvent: 50%" });

            playTimeTrackingPlugin.TrackingEvents.Add(new PlayTimeTrackingEvent() { PlayTime = TimeSpan.FromSeconds(5), Data = "PlayTimeTrackingEvent: 5 seconds" });
            playTimeTrackingPlugin.TrackingEvents.Add(new PlayTimeTrackingEvent() { PlayTime = TimeSpan.FromSeconds(15), Data = "PlayTimeTrackingEvent: 15 seconds" });
            playTimeTrackingPlugin.TrackingEvents.Add(new PlayTimeTrackingEvent() { PlayTime = TimeSpan.FromSeconds(45), Data = "PlayTimeTrackingEvent: 45 seconds" });

            PlayTimeEventList.ItemsSource = playTimeTrackingPlugin.TrackingEvents;
            PositionEventList.ItemsSource = positionTrackingPlugin.TrackingEvents;
            ResultsEventList.ItemsSource = new ObservableCollection<EventTrackedEventArgs>();
        }

        void trackingPlugin_EventTracked(object sender, EventTrackedEventArgs e)
        {
            var trackedEventList = ResultsEventList.ItemsSource as ObservableCollection<EventTrackedEventArgs>;
            trackedEventList.Add(e);
            Debug.WriteLine(string.Format("{1} - tracked: {0}", e.TrackingEvent.Data, e.Timestamp));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            player.Dispose();
            base.OnNavigatedFrom(e);
        }
    }
}
