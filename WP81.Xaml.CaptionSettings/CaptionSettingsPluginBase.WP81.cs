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
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Windows.UI.Xaml;
    using System.Collections.Generic;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Controls;
    using System.Globalization;
    using Windows.UI.Xaml.Media;
    using Windows.UI;
    using Windows.Phone.UI.Input;

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
        /// should video be resumed when done showing the popup?
        /// </summary>
        private bool resumeVideo;

        /// <summary>
        /// the caption settings control
        /// </summary>
        private CaptionSettingsControl control;
        #endregion

        #region Events
        /// <summary>
        /// Popup closed event handler triggered when ShowSettingsPopup() is closed.
        /// </summary>
        public event EventHandler<object> PopupClosed;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Style of the <see cref="Microsoft.PlayerFramework.CaptionSettings.CaptionSettingsControl"/>
        /// </summary>
        public Style CaptionSettingsControlStyle { get; set; }

        /// <summary>
        /// Gets a value indicating whether the popup from ShowSettingsPopup() is being shown
        /// </summary>
        public bool IsPopupOpen { get; private set; }
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
                fontFamilyMap[Model.FontFamily.MonospaceSerif] = GetDefaultFontFamily(Model.FontFamily.MonospaceSerif, "Courier New");
                fontFamilyMap[Model.FontFamily.ProportionalSerif] = GetDefaultFontFamily(Model.FontFamily.ProportionalSerif, "Times New Roman");

                // Windows Phone does not have a real monospace sans serif like Consolas.
                fontFamilyMap[Model.FontFamily.MonospaceSansSerif] = GetDefaultFontFamily(Model.FontFamily.MonospaceSansSerif, "Calibri");
                fontFamilyMap[Model.FontFamily.ProportionalSansSerif] = GetDefaultFontFamily(Model.FontFamily.ProportionalSansSerif, "Tahoma");
                fontFamilyMap[Model.FontFamily.Casual] = GetDefaultFontFamily(Model.FontFamily.Casual, "Comic Sans MS");

                // Windows Phone does not have a real cursive font like Segoe Script.
                fontFamilyMap[Model.FontFamily.Cursive] = GetDefaultFontFamily(Model.FontFamily.Cursive, "Calibri Light");
                fontFamilyMap[Model.FontFamily.Smallcaps] = GetDefaultFontFamily(Model.FontFamily.Smallcaps, "Tahoma");
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
        public void ShowSettingsPage(Frame service)
        {
            bool isEnabled = false;

            object value;

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue(CaptionSettingsPage.OverrideDefaultKey, out value))
            {
                isEnabled = (bool)value;
            }

            if (this.Settings == null)
            {
                this.Activate();
                this.Settings.PropertyChanged += this.Settings_PropertyChanged;
            }

            CaptionSettingsPage.Settings = this.Settings;
            CaptionSettingsPage.ApplyCaptionSettings = this.ApplyCaptionSettings;
            CaptionSettingsPage.ControlStyle = this.CaptionSettingsControlStyle;

            service.Navigate(typeof(CaptionSettingsPage), isEnabled);
        }

        /// <summary>
        /// Show the settings pane as a popup
        /// </summary>
        /// <param name="page">the current phone application page</param>
        /// <param name="parent">the parent panel on the page (typically the 
        /// LayoutRoot Grid)</param>
        /// <param name="pauseVideo">true to pause the video when the popup is 
        /// shown and play it when the popup is hidden.</param>
        public void ShowSettingsPopup(Page page, Panel parent, bool pauseVideo = false)
        {
            if (this.popup != null && this.popup.IsOpen)
            {
                return;
            }

            this.pauseVideo = pauseVideo;

            bool isEnabled = false;

            object value;

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue(CaptionSettingsPage.OverrideDefaultKey, out value))
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

                this.popup.Opened += delegate(object sender, object e)
                {
                    if (this.MediaPlayer != null)
                    {
                        this.MediaPlayer.IsEnabled = false;

                        if (this.pauseVideo)
                        {
                            if (this.MediaPlayer.CurrentState == MediaElementState.Playing)
                            {
                                this.resumeVideo = true;
                                this.MediaPlayer.InteractiveViewModel.Pause();
                            }
                        }
                    }

                    HardwareButtons.BackPressed += HardwareButtons_BackPressed;
                    this.IsPopupOpen = true;
                };

                this.popup.Closed += delegate(object sender, object e)
                {
                    if (this.MediaPlayer != null)
                    {
                        this.MediaPlayer.IsEnabled = true;

                        if (this.resumeVideo)
                        {
                            if (this.MediaPlayer.CurrentState == MediaElementState.Paused)
                            {
                                this.MediaPlayer.InteractiveViewModel.PlayResume();
                            }
                        }
                    }

                    HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
                    this.IsPopupOpen = false;

                    if (this.PopupClosed != null)
                    {
                        this.PopupClosed(this, e);
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
        partial void Activate()
        {
            bool isCustomCaptionSettings = false;

            object value;

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue(CaptionSettingsPage.OverrideDefaultKey, out value))
            {
                isCustomCaptionSettings = (bool)value;
            }

            if (isCustomCaptionSettings)
            {
                if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue(LocalSettingsKey, out value))
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
        partial void Deactivate()
        {
        }

        /// <summary>
        /// Save to Isolated storage
        /// </summary>
        internal void Save()
        {
            Windows.Storage.ApplicationData.Current.LocalSettings.Values[LocalSettingsKey] = this.Settings.ToXmlString();
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
            object fontName;

            var keyName = string.Format(CultureInfo.InvariantCulture, "FontFamilies.{0}", fontFamily);

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue(keyName, out fontName))
            {
                return (string)fontName;
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
        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (this.popup != null && this.popup.IsOpen)
            {
                e.Handled = true;

                if (this.control != null)
                {
                    if (!this.control.IsListSelectorShown)
                    {
                        this.popup.IsOpen = false;
                    }
                    else
                    {
                        this.control.HideListSelector();
                    }
                }
            }
        }
        #endregion
    }
}
