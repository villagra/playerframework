using Microsoft.PlayerFramework.Samples.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Microsoft.PlayerFramework.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TrackingPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public TrackingPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

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
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            Windows.Graphics.Display.DisplayInformation.AutoRotationPreferences = Windows.Graphics.Display.DisplayOrientations.Landscape;
            var noawait = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            player.Dispose();
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
