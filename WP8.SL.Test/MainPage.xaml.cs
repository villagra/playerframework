using System.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.PlayerFramework;
using Microsoft.PlayerFramework.Adaptive;
using Microsoft.PlayerFramework.Adaptive.Analytics;
using Microsoft.PlayerFramework.Analytics;
using Microsoft.PlayerFramework.TTML.CaptionSettings;
using Microsoft.Media.Analytics;
using Microsoft.Web.Media.Diagnostics;
using System;
using System.Windows.Navigation;

namespace Microsoft.Phone.PlayerFramework.SL.Test
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