using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.PlayerFramework.WP81.Test.Resources;
using Microsoft.PlayerFramework.Adaptive;

namespace Microsoft.PlayerFramework.WP81.Test
{
    public partial class MainPage : PhoneApplicationPage
    {
        AdaptivePlugin adaptivePlugin;
        MediaState deactivatedState;
        MediaState playerState;

        public MainPage()
        {
            InitializeComponent();

            // add adaptive plugin in order to play smooth streaming
            adaptivePlugin = new Microsoft.PlayerFramework.Adaptive.AdaptivePlugin();
            player.Plugins.Add(adaptivePlugin);
            player.VisualMarkers.Add(new VisualMarker() { Time = TimeSpan.FromSeconds(10) });
            player.VisualMarkers.Add(new VisualMarker() { Time = TimeSpan.FromSeconds(20) });

            PhoneApplicationService.Current.Deactivated += Current_Deactivated;
            PhoneApplicationService.Current.Activated += Current_Activated;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            playerState = player.GetMediaState();
            base.OnNavigatingFrom(e);
        }

        void Current_Activated(object sender, ActivatedEventArgs e)
        {
            if (deactivatedState != null)
            {
                player.RestoreMediaState(deactivatedState);
            }
        }

        void Current_Deactivated(object sender, DeactivatedEventArgs e)
        {
            player.Close(); // shut things like ads down.
            deactivatedState = playerState;
        }
    }
}