// <copyright file="CaptionSettingsPage2.xaml.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2014-01-06</date>
// <summary>Caption Setings page that hosts the caption settings control</summary>

namespace Microsoft.PlayerFramework.CaptionSettings
{
    using System;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using Microsoft.PlayerFramework.CaptionSettings.Model;

    /// <summary>
    /// Caption Settings page that hosts the template-based <see cref="Microsoft.PlayerFramework.CaptionSettings.CaptionSettingsControl"/>
    /// </summary>
    public partial class CaptionSettingsPage2 : PhoneApplicationPage
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CaptionSettingsPage2 class.
        /// </summary>
        public CaptionSettingsPage2()
        {
            this.InitializeComponent();

            this.Control.Settings = Settings;
            this.Control.ApplyCaptionSettings = ApplyCaptionSettings;
            this.Control.Style = ControlStyle;

            if (Options != null)
            {
                SystemTray.SetIsVisible(this, Options.IsSystemTrayVisible);

                this.SupportedOrientations = Options.SupportedOrientation;
                this.Orientation = Options.Orientation;
            }

            this.Control.Page = this;
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
        public static System.Windows.Style ControlStyle { get; set; }

        /// <summary>
        /// Gets or sets the page options
        /// </summary>
        public static CaptionSettingsPageOptions Options { get; set; }

        #endregion

        #region Methods
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