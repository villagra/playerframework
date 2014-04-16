// <copyright file="CaptionSettingsPage.xaml.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2014-01-06</date>
// <summary>Caption Setings page that hosts the caption settings control</summary>

namespace Microsoft.PlayerFramework.CaptionSettings
{
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using System;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Caption Settings page that hosts the template-based <see cref="Microsoft.PlayerFramework.CaptionSettings.CaptionSettingsControl"/>
    /// </summary>
    public partial class CaptionSettingsPage : Page
    {
        #region Fields

        /// <summary>
        /// the isolated storage settings key for the override default caption settings flag
        /// </summary>
        public const string OverrideDefaultKey = "Microsoft.PlayerFramework.OverrideDefaultCaptionSettings";

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CaptionSettingsPage class.
        /// </summary>
        public CaptionSettingsPage()
        {
            this.InitializeComponent();

            this.Control.Settings = Settings;
            this.Control.ApplyCaptionSettings = ApplyCaptionSettings;
            this.Control.Style = ControlStyle;
            this.Control.Page = this;

            this.Loaded += (sender, e) =>
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            };

            this.Unloaded += (sender, e) =>
            {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            };
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the custom caption settings
        /// </summary>
        public static Model.CustomCaptionSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the Apply caption settings action
        /// </summary>
        public static Action<CustomCaptionSettings> ApplyCaptionSettings { get; set; }

        /// <summary>
        /// Gets or sets the control style for the 
        /// <see cref="Microsoft.PlayerFramework.CaptionSettings.CaptionSettingsControl"/>
        /// </summary>
        public static Style ControlStyle { get; set; }

        #endregion

        #region Methods
        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            if (this.Frame != null && this.Frame.CanGoBack)
            {
                e.Handled = true;
                if (this.Frame != null && this.Frame.CanGoBack) this.Frame.GoBack();
            }
        }

        /// <summary>
        /// Navigate to the page and control
        /// </summary>
        /// <param name="e">the navigation event arguments</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.Control.OnNavigatedTo();
        }

        /// <summary>
        /// Navigating from the control
        /// </summary>
        /// <param name="e">the navigating cancel event arguments</param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            this.Control.OnNavigatingFrom(e);
        }
        #endregion
    }
}