// <copyright file="MainPage.xaml.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-12-04</date>
// <summary>Main Page</summary>

namespace WP8.PlayerFramework.Test
{
    using Microsoft.Phone.Controls;

    /// <summary>
    /// Main Page code behind
    /// </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        /// Initializes a new instance of the MainPage class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            // Sample code to localize the ApplicationBar
            ////BuildLocalizedApplicationBar();
        }

        /// <summary>
        /// Show the settings page
        /// </summary>
        /// <param name="sender">the caption settings button</param>
        /// <param name="e">the event arguments</param>
        private void OnClickCaptionSettings(object sender, System.EventArgs e)
        {
            var plugin = new Microsoft.PlayerFramework.TTML.CaptionSettings.TTMLCaptionSettingsPlugin();

            var options = new Microsoft.PlayerFramework.CaptionSettings.CaptionSettingsPageOptions();

            options.Orientation = PageOrientation.Portrait;
            options.IsSystemTrayVisible = false;
            options.SupportedOrientation = SupportedPageOrientation.PortraitOrLandscape;

            plugin.ShowSettingsPage(this.NavigationService, options);
        }

        /// <summary>
        /// Show the caption settings page in portrait only with no system tray visible.
        /// </summary>
        /// <param name="sender">the button</param>
        /// <param name="e">the event arguments</param>
        private void OnClickCaptionSettingsPortrait(object sender, System.EventArgs e)
        {
            var plugin = new Microsoft.PlayerFramework.TTML.CaptionSettings.TTMLCaptionSettingsPlugin();

            var options = new Microsoft.PlayerFramework.CaptionSettings.CaptionSettingsPageOptions();

            options.Orientation = PageOrientation.Portrait;
            options.IsSystemTrayVisible = false;
            options.SupportedOrientation = SupportedPageOrientation.Portrait;

            plugin.ShowSettingsPage(this.NavigationService, options);
        }

        /// <summary>
        /// Show caption settings as a popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickCaptionSettingsPopup(object sender, System.EventArgs e)
        {
            var plugin = new Microsoft.PlayerFramework.TTML.CaptionSettings.TTMLCaptionSettingsPlugin();

            plugin.ShowSettingsPopup(this, this.LayoutRoot);
        }
    }
}