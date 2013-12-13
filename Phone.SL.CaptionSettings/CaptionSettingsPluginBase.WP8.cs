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
    using System.Collections.Generic;
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

        /// <summary>
        /// The Font Family Map
        /// </summary>
        private static Dictionary<Model.FontFamily, string> fontFamilyMap;
        #endregion

        #region Methods
        /// <summary>
        /// Gets the Windows Phone Font Family name
        /// </summary>
        /// <param name="fontFamily">the caption font family</param>
        /// <returns>the Windows Phone font family name</returns>
        public static string GetFontFamilyName(Model.FontFamily fontFamily)
        {
            if (fontFamilyMap == null)
            {
                fontFamilyMap = new Dictionary<Model.FontFamily, string>();

                fontFamilyMap[Model.FontFamily.Default] = null;
                fontFamilyMap[Model.FontFamily.MonospaceSerif] = GetDefaultFontFamily(fontFamily, "Courier New");
                fontFamilyMap[Model.FontFamily.ProportionalSerif] = GetDefaultFontFamily(fontFamily, "Times New Roman");

                // Windows Phone does not have a real monospace sans serif like Consolas.
                fontFamilyMap[Model.FontFamily.MonospaceSansSerif] = GetDefaultFontFamily(fontFamily, "Calibri");
                fontFamilyMap[Model.FontFamily.ProportionalSansSerif] = GetDefaultFontFamily(fontFamily, "Tahoma");
                fontFamilyMap[Model.FontFamily.Casual] = GetDefaultFontFamily(fontFamily, "Comic Sans MS");
                
                // Windows Phone does not have a real cursive font like Segoe Script.
                fontFamilyMap[Model.FontFamily.Cursive] = GetDefaultFontFamily(fontFamily, "Calibri Light");
                fontFamilyMap[Model.FontFamily.Smallcaps] = GetDefaultFontFamily(fontFamily, "Tahoma");
            }

            return fontFamilyMap[fontFamily];
        }

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

            if (this.Settings == null)
            {
                this.Activate();
                this.Settings.PropertyChanged += this.Settings_PropertyChanged;
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
                this.Settings = new CustomCaptionSettings();
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
            IsolatedStorageSettings.ApplicationSettings.Save();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the font family from application data local settings if it has been overridden.
        /// </summary>
        /// <param name="fontFamily">the font family</param>
        /// <param name="defaultName">the default font name</param>
        /// <returns>the font family name</returns>
        private static string GetDefaultFontFamily(Model.FontFamily fontFamily, string defaultName)
        {
            string fontName;

            var keyName = string.Format(CultureInfo.InvariantCulture, "FontFamilies.{0}", fontFamily);

            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(keyName, out fontName))
            {
                return fontName;
            }

            return defaultName;
        }

        /// <summary>
        /// Save the settings when activated without a player
        /// </summary>
        /// <param name="sender">the settings</param>
        /// <param name="e">the property changed event arguments</param>
        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Save();
        }
        #endregion
    }
}
