using System;
using System.Collections.Generic;
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
    public sealed partial class SuspendResumePage : Microsoft.PlayerFramework.Samples.Common.LayoutAwarePage
    {
        const string playerStateKey = "mediaPlayerState";
        bool teardown;

        public SuspendResumePage()
        {
            this.InitializeComponent();
        }

        void App_Resuming(object sender, object e)
        {
            // resume playback on resume. Simulate the user clicking the button to makes sure ads are handled (vs. player.PlayResume()).
            player.InteractiveViewModel.PlayResume();
        }

        void App_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            // pause playback on suspend. Simulate the user clicking the button to makes sure ads are handled (vs. player.Pause()).
            player.InteractiveViewModel.Pause();
        }

        protected override void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            base.LoadState(navigationParameter, pageState);

            if (pageState != null)
            {
                var playerState = pageState[playerStateKey] as MediaPlayerState;
                if (playerState != null)
                {
                    player.RestorePlayerState(playerState);
                }
            }
        }

        protected override void SaveState(Dictionary<string, object> pageState)
        {
            var playerState = player.GetPlayerState();
            pageState.Add(playerStateKey, playerState);
            base.SaveState(pageState);

            if (teardown)
            {
                player.Dispose();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Application.Current.Suspending += App_Suspending;
            Application.Current.Resuming += App_Resuming;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            teardown = true; // delay teardown until after we've saved the state
            Application.Current.Suspending -= App_Suspending;
            Application.Current.Resuming -= App_Resuming;
            
            base.OnNavigatingFrom(e);
        }
    }
}
