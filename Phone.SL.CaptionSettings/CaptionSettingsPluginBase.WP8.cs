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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.PlayerFramework.CaptionSettings.Model;

    /// <summary>
    /// Windows Phone Caption Settings UI
    /// </summary>
    [StyleTypedProperty(Property = "CaptionSettingsControlStyle", StyleTargetType = typeof(CaptionSettingsControl))]
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

        /// <summary>
        /// the popup
        /// </summary>
        private Popup popup;

        /// <summary>
        /// should video be paused when showing the popup?
        /// </summary>
        private bool pauseVideo;

        /// <summary>
        /// the caption settings control
        /// </summary>
        private CaptionSettingsControl control;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Style of the <see cref="Microsoft.PlayerFramework.CaptionSettings.CaptionSettingsControl"/>
        /// </summary>
        public Style CaptionSettingsControlStyle { get; set; }
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
        /// <param name="options">the phone page options</param>
        /// <example>
        /// This is the handler for an app bar menu button click event:
        /// <code>
        /// private void CaptionSettings_Click(object sender, System.EventArgs e)
        /// {
        ///     this.captionSettingsPlugin.ShowSettingsPage(this.NavigationService);
        /// }
        /// </code>
        /// </example>
        public void ShowSettingsPage(NavigationService service, CaptionSettingsPageOptions options = null)
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
                "/{0};component/CaptionSettingsPage2.xaml?IsEnabled={1}",
                assemblyName,
                isEnabled);

            var source = new Uri(uriString, UriKind.Relative);

            CaptionSettingsPage2.Options = options;
            CaptionSettingsPage2.Settings = this.Settings;
            CaptionSettingsPage2.ApplyCaptionSettings = this.ApplyCaptionSettings;
            CaptionSettingsPage2.ControlStyle = this.CaptionSettingsControlStyle;

            service.Navigate(source);
        }

        /// <summary>
        /// Show the settings pane as a popup
        /// </summary>
        /// <param name="page">the current phone application page</param>
        /// <param name="parent">the parent panel on the page (typically the 
        /// LayoutRoot Grid)</param>
        /// <param name="pauseVideo">true to pause the video when the popup is 
        /// shown and play it when the popup is hidden.</param>
        public void ShowSettingsPopup(PhoneApplicationPage page, Panel parent, bool pauseVideo = false)
        {
            if (this.popup != null && this.popup.IsOpen)
            {
                return;
            }

            this.pauseVideo = pauseVideo;

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

            if (!isEnabled)
            {
                this.Settings.BackgroundColor = null;
                this.Settings.FontColor = null;
                this.Settings.FontFamily = Model.FontFamily.Default;
                this.Settings.FontSize = null;
                this.Settings.FontStyle = Model.FontStyle.Default;
                this.Settings.WindowColor = null;
            }

            Border border = null;

            if (this.popup == null)
            {
                page.BackKeyPress += this.OnBackKeyPress;

                this.control = new CaptionSettingsControl
                {
                    ApplyCaptionSettings = this.ApplyCaptionSettings,
                    Settings = this.Settings,
                    Page = page,
                    Width = page.ActualWidth,
                    Height = page.ActualHeight,
                    Style = this.CaptionSettingsControlStyle,
                    IsOverrideDefaults = isEnabled
                };

                this.popup = new Popup
                {
                    Child = new Border
                    {
                        Child = this.control
                    }
                };

                this.popup.Opened += delegate(object sender, EventArgs e)
                {
                    if (this.MediaPlayer != null)
                    {
                        this.MediaPlayer.IsEnabled = false;

                        if (this.pauseVideo)
                        {
                            if (this.MediaPlayer.CurrentState == MediaElementState.Playing)
                            {
                                this.MediaPlayer.Pause();
                            }
                        }
                    }
                };

                this.popup.Closed += delegate(object sender, EventArgs e)
                {
                    if (this.MediaPlayer != null)
                    {
                        this.MediaPlayer.IsEnabled = true;

                        if (this.pauseVideo)
                        {
                            if (this.MediaPlayer.CurrentState == MediaElementState.Paused)
                            {
                                this.MediaPlayer.Play();
                            }
                        }
                    }
                };

                if (parent != null)
                {
                    parent.Children.Add(this.popup);
                }

                border = this.popup.Child as Border;
            }
            else
            {
                border = this.popup.Child as Border;

                var control = border.Child as CaptionSettingsControl;

                control.Page = page;
                control.Style = this.CaptionSettingsControlStyle;
                control.IsOverrideDefaults = isEnabled;
            }

            if (this.MediaPlayer == null)
            {
                border.Background = new SolidColorBrush(Colors.Black);
            }
            else
            {
                border.Background = null;
            }

            this.popup.IsOpen = true;
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

            if (isCustomCaptionSettings)
            {
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(LocalSettingsKey, out value))
                {
                    var xml = value.ToString();

                    this.Settings = CustomCaptionSettings.FromString(xml);
                }
                else
                {
                    this.Settings = new CustomCaptionSettings();
                }

                this.IsDefault = false;
            }
            else
            {
                this.Settings = new CustomCaptionSettings();

                this.IsDefault = true;
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

        /// <summary>
        /// If the popup is being shown handle the back button and hide the popup
        /// </summary>
        /// <param name="sender">the page</param>
        /// <param name="e">the cancel event arguments</param>
        private void OnBackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var page = sender as PhoneApplicationPage;

            if (this.popup != null && this.popup.IsOpen)
            {
                e.Cancel = true;

                if (this.control != null && !this.control.IsListSelectorShown)
                {
                    this.popup.IsOpen = false;
                }
            }
        }
        #endregion
    }
}
