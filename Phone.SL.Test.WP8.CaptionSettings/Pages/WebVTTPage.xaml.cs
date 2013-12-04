// <copyright file="WebVTTPage.xaml.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-12-04</date>
// <summary>WebVTT Test Page</summary>

namespace WP8.PlayerFramework.Test.Pages
{
    using System;
    using System.Linq;
    using System.Windows;
    using Microsoft.Phone.Controls;
    using Microsoft.PlayerFramework.WebVTT.CaptionSettings;

    /// <summary>
    /// WebVTT Test Page
    /// </summary>
    public partial class WebVTTPage : PhoneApplicationPage
    {
        #region Fields
        /// <summary>
        /// the plug-in
        /// </summary>
        private WebVTTCaptionSettingsPlugin plugin;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the WebVTTPage class.
        /// </summary>
        public WebVTTPage()
        {
            this.InitializeComponent();

            this.plugin = new WebVTTCaptionSettingsPlugin();

            this.Player.Plugins.Add(this.plugin);

            this.Player.SelectedCaption = this.Player.AvailableCaptions.First();
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Show the caption settings page
        /// </summary>
        /// <param name="sender">the Caption Settings button</param>
        /// <param name="e">the event arguments</param>
        private void OnCaptionSettings(object sender, EventArgs e)
        {
            this.plugin.ShowSettingsPage(this.NavigationService);
        }
        #endregion
    }
}