using System.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.PlayerFramework;
using Microsoft.PlayerFramework.Adaptive;
using Microsoft.PlayerFramework.Adaptive.Analytics;
using Microsoft.PlayerFramework.Analytics;
using Microsoft.PlayerFramework.Xaml.TTML.CaptionSettings;
using Microsoft.VideoAnalytics;
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
        TTMLCaptionSettingsPlugin captionSettingsPlugin;

        public MainPage()
        {
            InitializeComponent();
                        
            // add adaptive plugin in order to play smooth streaming
            adaptivePlugin = new Microsoft.PlayerFramework.Adaptive.AdaptivePlugin();
            player.Plugins.Add(adaptivePlugin);

            PhoneApplicationService.Current.Deactivated += Current_Deactivated;
            PhoneApplicationService.Current.Activated += Current_Activated;

            // This is to demonstrate caption settings
            player.Source = new Uri("http://smf.blob.core.windows.net/samples/videos/RealPCPride.mp4");
            player.Plugins.Add(new Microsoft.PlayerFramework.TimedText.CaptionsPlugin());
            player.IsCaptionSelectionVisible = true;
            player.SelectedCaption = player.AvailableCaptions.FirstOrDefault();
            this.captionSettingsPlugin = new TTMLCaptionSettingsPlugin();
            player.Plugins.Add(this.captionSettingsPlugin);
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

        /// <summary>
        /// Show the settings page
        /// </summary>
        /// <param name="sender">the Caption Settings button</param>
        /// <param name="e">the event arguments</param>
        private void CaptionSettings_Click(object sender, System.EventArgs e)
        {
            // Todo: save the current state of the player so that it can be 
            // restored when navigating back from the settings page
            this.captionSettingsPlugin.ShowSettingsPage(this.NavigationService);
        }
    }
}