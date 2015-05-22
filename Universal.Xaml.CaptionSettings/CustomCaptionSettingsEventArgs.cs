// <copyright file="CustomCaptionSettingsEventArgs.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-10-28</date>
// <summary>Custom Caption Settings event arguments</summary>

namespace Microsoft.PlayerFramework.CaptionSettings
{
    using System;
    using Microsoft.PlayerFramework.CaptionSettings.Model;

    /// <summary>
    /// custom caption settings event arguments
    /// </summary>
    public class CustomCaptionSettingsEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the CustomCaptionSettingsEventArgs class.
        /// </summary>
        /// <param name="settings">the settings</param>
        public CustomCaptionSettingsEventArgs(CustomCaptionSettings settings)
        {
            this.Settings = settings;
        }

        /// <summary>
        /// Gets or sets the settings
        /// </summary>
        public CustomCaptionSettings Settings { get; set; }
    }
}
