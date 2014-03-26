// <copyright file="CaptionSettingsPageOptions.cs" company="Microsoft Corporation">
// Copyright (c) 2014 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2014-01-08</date>
// <summary>Caption Settings Page options</summary>

namespace Microsoft.PlayerFramework.CaptionSettings
{
    using Microsoft.Phone.Controls;

    /// <summary>
    /// Caption Settings Page options
    /// </summary>
    public class CaptionSettingsPageOptions
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CaptionSettingsPageOptions class.
        /// </summary>
        public CaptionSettingsPageOptions()
        {
            this.IsSystemTrayVisible = false;
            this.Orientation = PageOrientation.None;
            this.SupportedOrientation = SupportedPageOrientation.PortraitOrLandscape;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether the system tray is visible when the caption settings are shown
        /// </summary>
        /// <remarks>default is false</remarks>
        public bool IsSystemTrayVisible { get; set; }

        /// <summary>
        /// Gets or sets the page orientation
        /// </summary>
        /// <remarks>default is None</remarks>
        public PageOrientation Orientation { get; set; }

        /// <summary>
        /// Gets or sets the supported orientations
        /// </summary>
        /// <remarks>default is PortraitOrLandscape</remarks>
        public SupportedPageOrientation SupportedOrientation { get; set; }
        #endregion
    }
}
