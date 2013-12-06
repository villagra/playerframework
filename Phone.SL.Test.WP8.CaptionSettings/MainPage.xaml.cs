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

            plugin.ShowSettingsPage(this.NavigationService);
        }

        // Sample code for building a localized ApplicationBar
        ////private void BuildLocalizedApplicationBar()
        ////{
        ////    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        ////    ApplicationBar = new ApplicationBar();

        ////    // Create a new button and set the text value to the localized string from AppResources.
        ////    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        ////    appBarButton.Text = AppResources.AppBarButtonText;
        ////    ApplicationBar.Buttons.Add(appBarButton);

        ////    // Create a new menu item with the localized string from AppResources.
        ////    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        ////    ApplicationBar.MenuItems.Add(appBarMenuItem);
        ////}
    }
}