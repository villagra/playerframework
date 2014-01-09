// <copyright file="CaptionSettingFlyout.xaml.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-10-28</date>
// <summary>Caption settings flyout</summary>

namespace Microsoft.PlayerFramework.CaptionSettings
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Microsoft.PlayerFramework.CaptionSettings.ViewModel;
    using Windows.ApplicationModel.Resources;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml;

    /// <summary>
    /// Caption Settings Flyout
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
    [StyleTypedProperty(Property = "ControlStyle", StyleTargetType = typeof(CaptionSettingsControl))]
    public sealed partial class CaptionSettingFlyout : SettingsFlyout
    {
        /// <summary>
        /// Initializes a new instance of the CaptionSettingFlyout class.
        /// </summary>
        public CaptionSettingFlyout()
        {
            this.InitializeComponent();

            this.Control.OnApplyCaptionSettings += this.Control_OnApplyCaptionSettings;
        }

        /// <summary>
        /// Event to save caption settings
        /// </summary>
        public event EventHandler<CustomCaptionSettingsEventArgs> OnApplyCaptionSettings;

        /// <summary>
        /// Gets or sets the custom caption settings
        /// </summary>
        public CustomCaptionSettings CaptionSettings
        {
            get
            {
                return this.Control.CaptionSettings;
            }

            set
            {
                this.Control.CaptionSettings = value;
            }
        }

        /// <summary>
        /// Gets or sets the style for the <see cref="Microsoft.PlayerFramework.CaptionSettings.CaptionSettingsControl"/>
        /// </summary>
        public Windows.UI.Xaml.Style ControlStyle
        {
            get
            {
                return this.Control.Style;
            }

            set
            {
                this.Control.Style = value;
            }
        }
        
        /// <summary>
        /// Route the control's OnApplyCaptionSettings event
        /// </summary>
        /// <param name="sender">the CaptionSettingsControl</param>
        /// <param name="e">the custom caption settings event arguments</param>
        private void Control_OnApplyCaptionSettings(object sender, CustomCaptionSettingsEventArgs e)
        {
            if (this.OnApplyCaptionSettings != null)
            {
                this.OnApplyCaptionSettings(sender, e);
            }
        }
    }
}
