// <copyright file="CaptionSettingsPluginBase.Win81.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-10-28</date>
// <summary>Windows 8 Caption Settings UI</summary>

namespace Microsoft.PlayerFramework.CaptionSettings
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Windows 8.1 Caption Settings Plugin Base partial class
    /// </summary>
    [StyleTypedProperty(Property = "SettingsFlyoutStyle", StyleTargetType = typeof(SettingsFlyout))]
    [StyleTypedProperty(Property = "SettingsControlStyle", StyleTargetType = typeof(CaptionSettingsControl))]
    public partial class CaptionSettingsPluginBase
    {
        #region Fields
        /// <summary>
        /// the local settings key
        /// </summary>
        private const string LocalSettingsKey = "Microsoft.PlayerFramework.CaptionSettings";

        /// <summary>
        /// the font family map
        /// </summary>
        private static Dictionary<FontFamily, string> fontFamilyMap;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CaptionSettingsPluginBase class.
        /// </summary>
        protected CaptionSettingsPluginBase()
        {
            var loader = AssemblyResources.Get();

            this.SettingsCommandId = "CaptionSettings";

            this.Label = loader.GetString("CaptionSettings");
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the settings command Id for the Settings pane
        /// </summary>
        public string SettingsCommandId { get; set; }

        /// <summary>
        /// Gets or sets the label for the settings pane
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the index of the settings command
        /// </summary>
        public int? SettingsCommandIndex { get; set; }

        /// <summary>
        /// Gets or sets the settings flyout style (use style for SettingsFlyout)
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
        public Style SettingsFlyoutStyle { get; set; }

        /// <summary>
        /// Gets or sets the style for the <see cref="Microsoft.PlayerFramework.CaptionSettings.CaptionSettingsControl"/>
        /// </summary>
        public Style SettingsControlStyle { get; set; }

        /// <summary>
        /// Gets or sets the icon source of the caption settings pane
        /// </summary>
        public Windows.UI.Xaml.Media.ImageSource IconSource { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the Windows font family mapped to the Captions font Family
        /// </summary>
        /// <param name="fontFamily">the captions font family</param>
        /// <returns>the name of the Windows font family</returns>
        public static string GetFontFamilyName(FontFamily fontFamily)
        {
            if (fontFamilyMap == null)
            {
                fontFamilyMap = new Dictionary<FontFamily, string>();

                fontFamilyMap[FontFamily.Default] = null;
                fontFamilyMap[FontFamily.MonospaceSerif] = GetDefaultFontFamily(fontFamily, "Courier New");
                fontFamilyMap[FontFamily.ProportionalSerif] = GetDefaultFontFamily(fontFamily, "Times New Roman");
                fontFamilyMap[FontFamily.MonospaceSansSerif] = GetDefaultFontFamily(fontFamily, "Consolas");
                fontFamilyMap[FontFamily.ProportionalSansSerif] = GetDefaultFontFamily(fontFamily, "Tahoma");
                fontFamilyMap[FontFamily.Casual] = GetDefaultFontFamily(fontFamily, "Segoe Print");
                fontFamilyMap[FontFamily.Cursive] = GetDefaultFontFamily(fontFamily, "Segoe Script");
                fontFamilyMap[FontFamily.Smallcaps] = GetDefaultFontFamily(fontFamily, "Tahoma");
            }

            return fontFamilyMap[fontFamily];
        }
        
        /// <summary>
        /// Save the settings to local storage
        /// </summary>
        internal void Save()
        {
            if (this.Settings == null || this.IsDefault)
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(LocalSettingsKey);
            }
            else
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values[LocalSettingsKey] = this.Settings.ToXmlString();
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the font family from application data local settings if it has been overridden.
        /// </summary>
        /// <param name="fontFamily">the font family</param>
        /// <param name="defaultName">the default font name</param>
        /// <returns>the font family name</returns>
        private static string GetDefaultFontFamily(FontFamily fontFamily, string defaultName)
        {
            var container = Windows.Storage.ApplicationData.Current.LocalSettings.CreateContainer("Font Families", Windows.Storage.ApplicationDataCreateDisposition.Always);

            object value;

            if (container.Values.TryGetValue(fontFamily.ToString(), out value))
            {
                string fontName = value.ToString();

                return fontName;
            }

            return defaultName;
        }

        /// <summary>
        /// Initialize the settings
        /// </summary>
        private void InitializeSettings()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            object value;

            bool overrideDefaults = false;

            if (localSettings.Values.TryGetValue(CaptionSettingsControl.OverrideDefaultKey, out value))
            {
                overrideDefaults = (bool)value;
            }

            if (overrideDefaults)
            {
                this.IsDefault = false;

                if (localSettings.Values.TryGetValue(LocalSettingsKey, out value))
                {
                    var settingsString = value.ToString();

                    this.Settings = CustomCaptionSettings.FromString(settingsString);
                }
                else
                {
                    this.Settings = new CustomCaptionSettings();
                }
            }
            else
            {
                this.Settings = new CustomCaptionSettings();

                this.IsDefault = true;
            }
        }

        partial void Activate()
        {
            if (this.Settings == null)
            {
                this.InitializeSettings();
            }
        }

        /// <summary>
        /// Show the caption settings
        /// </summary>
        /// <param name="command">the command</param>
        public void ShowCaptionSettings()
        {
            if (this.OnLoadCaptionSettings != null)
            {
                this.OnLoadCaptionSettings(this, new CustomCaptionSettingsEventArgs(this.Settings));
            }

            var flyout = new CaptionSettingFlyout
            {
                CaptionSettings = this.Settings,
                Style = this.SettingsFlyoutStyle,
                ControlStyle = this.SettingsControlStyle,
                Title = this.Label,
                IconSource = this.IconSource
            };

            flyout.OnApplyCaptionSettings += this.OnApplyCaptionSettings;

            flyout.Unloaded += this.OnUnloaded;

            flyout.Show();
        }

        /// <summary>
        /// Apply the caption settings
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the custom caption settings event arguments</param>
        private void OnApplyCaptionSettings(object sender, CustomCaptionSettingsEventArgs e)
        {
            this.IsDefault = e.Settings == null;

            if (!this.IsDefault)
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(LocalSettingsKey);
            }

            this.ApplyCaptionSettings(e.Settings);
        }

        /// <summary>
        /// Save the caption settings when the flyout unloads.
        /// </summary>
        /// <param name="sender">the flyout</param>
        /// <param name="e">the routed event arguments</param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
        private void OnUnloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var flyout = sender as CaptionSettingFlyout;

            var captionSettings = flyout.CaptionSettings;

            if (this.OnSaveCaptionSettings != null)
            {
                this.OnSaveCaptionSettings(this, new CustomCaptionSettingsEventArgs(captionSettings));
            }

            this.Save();
        }
        #endregion
    }
}
