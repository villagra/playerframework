// <copyright file="TTMLPage.xaml.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-12-04</date>
// <summary>TTML Caption Settings Test Page</summary>

namespace WP8.PlayerFramework.Test.Pages
{
    using System.Linq;
    using System.Windows;
    using Microsoft.Phone.Controls;
    using Microsoft.PlayerFramework.TTML.CaptionSettings;

    /// <summary>
    /// TTML Caption Settings Test Page
    /// </summary>
    public partial class TTMLPage : PhoneApplicationPage
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the TTMLPage class.
        /// </summary>
        public TTMLPage()
        {
            this.InitializeComponent();

            this.Player.SelectedCaption = this.Player.AvailableCaptions.First();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Show the Caption Settings Page
        /// </summary>
        /// <param name="sender">the caption settings button</param>
        /// <param name="e">the event arguments</param>
        private void OnCaptionSettings(object sender, System.EventArgs e)
        {
            var plugin = (from item in this.Player.Plugins.OfType<TTMLCaptionSettingsPlugin>()
                          select item).First();

            plugin.ShowSettingsPopup(this, this.LayoutRoot);
        }
        #endregion
    }
}