// <copyright file="CaptionSettingsPluginBase.WP8.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-14</date>
// <summary>CaptionSettingsPluginBase partial class for Windows Phone 8</summary>

namespace Microsoft.PlayerFramework.CaptionSettings
{
    using System;
    using System.Globalization;
    using System.IO.IsolatedStorage;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using Microsoft.PlayerFramework.CaptionSettings.Model;

    /// <summary>
    /// Windows Phone Caption Settings UI
    /// </summary>
    public partial class CaptionSettingsPluginBase
    {
        #region Fields
        /// <summary>
        /// the isolated storage settings key for the caption settings
        /// </summary>
        private const string LocalSettingsKey = "Microsoft.PlayerFramework.CaptionSettings";
        #endregion

        #region Methods

        /// <summary>
        /// Show the settings page if there is a CaptionsPlugin.
        /// </summary>
        /// <param name="service">the navigation service</param>
        /// <example>
        /// This is the handler for an app bar menu button click event:
        /// <code>
        /// private void CaptionSettings_Click(object sender, System.EventArgs e)
        /// {
        ///     this.captionSettingsPlugin.ShowSettingsPage(this.NavigationService);
        /// }
        /// </code>
        /// </example>
        public void ShowSettingsPage(NavigationService service)
        {
            bool isEnabled = false;

            object value;

            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(CaptionSettingsPage.OverrideDefaultKey, out value))
            {
                isEnabled = (bool)value;
            }

            var assembly = typeof(CaptionSettingsPage).Assembly;

            var assemblyName = System.IO.Path.GetFileNameWithoutExtension(assembly.ManifestModule.Name);

            var uriString = string.Format(
                CultureInfo.InvariantCulture,
                "/{0};component/CaptionSettingsPage.xaml?IsEnabled={1}",
                assemblyName,
                isEnabled);

            var source = new Uri(uriString, UriKind.Relative);

            if (service.Navigate(source))
            {
                CaptionSettingsPage.Settings = this.Settings;
                CaptionSettingsPage.ApplyCaptionSettings = this.ApplyCaptionSettings;
            }
        }

        /// <summary>
        /// Activate the caption settings UI
        /// </summary>
        internal void Activate()
        {
            bool isCustomCaptionSettings = false;

            object value;

            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(CaptionSettingsPage.OverrideDefaultKey, out value))
            {
                isCustomCaptionSettings = (bool)value;
            }

            if (isCustomCaptionSettings && IsolatedStorageSettings.ApplicationSettings.TryGetValue(LocalSettingsKey, out value))
            {
                var xml = value.ToString();

                this.Settings = CustomCaptionSettings.FromString(xml);
            }
            else
            {
                this.Settings = new CustomCaptionSettings
                {
                    FontColor = Colors.White.ToCaptionSettingsColor()
                };
            }
        }

        /// <summary>
        /// Deactivate the Windows Phone UI
        /// </summary>
        internal void Deactivate()
        {
        }

        /// <summary>
        /// Save to Isolated storage
        /// </summary>
        internal void Save()
        {
            IsolatedStorageSettings.ApplicationSettings[LocalSettingsKey] = this.Settings.ToXmlString();
        }
        #endregion
    }
}
