// <copyright file="CaptionSettingsPluginBase.Win81.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-10-28</date>
// <summary>Windows 8 Caption Settings UI</summary>

namespace Microsoft.PlayerFramework.CaptionSettings
{
    using System;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Windows.ApplicationModel.Resources;
    using Windows.UI.ApplicationSettings;
    using Windows.UI.Popups;
    using Windows.UI.Xaml;

    /// <summary>
    /// Windows 8.1 Caption Settings Plugin Base partial class
    /// </summary>
    public partial class CaptionSettingsPluginBase
    {
        #region Fields
        /// <summary>
        /// the local settings key
        /// </summary>
        private const string LocalSettingsKey = "Microsoft.PlayerFramework.CaptionSettings";

        /// <summary>
        /// the plug-in
        /// </summary>
        private CaptionSettingsPluginBase plugin;

        /// <summary>
        /// the load caption settings event handler
        /// </summary>
        private EventHandler<CustomCaptionSettingsEventArgs> loadCaptionSettings;

        /// <summary>
        /// the save caption settings event handler
        /// </summary>
        private EventHandler<CustomCaptionSettingsEventArgs> saveCaptionSettings;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CaptionSettingsPluginBase class.
        /// </summary>
        protected CaptionSettingsPluginBase()
        {
            var loader = ResourceLoader.GetForCurrentView("Microsoft.Win81.PlayerFramework.CaptionSettingsPlugIn.Xaml/Resources");

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
        public Style SettingsFlyoutStyle { get; set; }
        #endregion

        #region Methods
        internal void Activate(
            CaptionSettingsPluginBase plugin,
            EventHandler<CustomCaptionSettingsEventArgs> loadCaptionSettings,
            EventHandler<CustomCaptionSettingsEventArgs> saveCaptionSettings)
        {
            this.plugin = plugin;
            this.loadCaptionSettings = loadCaptionSettings;
            this.saveCaptionSettings = saveCaptionSettings;

            SettingsPane.GetForCurrentView().CommandsRequested += this.CaptionsSettingsPlugin_CommandsRequested;

            object value;

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue(LocalSettingsKey, out value))
            {
                var settingsString = value.ToString();

                plugin.Settings = CustomCaptionSettings.FromString(settingsString);
            }
            else
            {
                plugin.Settings = new CustomCaptionSettings();
            }
        }

        internal void Deactivate()
        {
            SettingsPane.GetForCurrentView().CommandsRequested -= this.CaptionsSettingsPlugin_CommandsRequested;

            this.plugin = null;
            this.saveCaptionSettings = null;
            this.loadCaptionSettings = null;
        }

        internal void Save(CustomCaptionSettings captionSettings)
        {
            if (captionSettings == null)
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values.Remove(LocalSettingsKey);
            }
            else
            {
                Windows.Storage.ApplicationData.Current.LocalSettings.Values[LocalSettingsKey] = captionSettings.ToXmlString();
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds the caption settings command to the plug-in
        /// </summary>
        /// <param name="sender">the settings pane</param>
        /// <param name="args">the settings pane commands requested event arguments</param>
        private void CaptionsSettingsPlugin_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            if (this.plugin.MediaPlayer == null)
            {
                return;
            }

            var command = new SettingsCommand(
                this.SettingsCommandId,
                this.Label,
                new Windows.UI.Popups.UICommandInvokedHandler(this.OnShowCaptionSettings));

            if (this.SettingsCommandIndex.HasValue)
            {
                args.Request.ApplicationCommands.Insert(this.SettingsCommandIndex.Value, command);
            }
            else
            {
                args.Request.ApplicationCommands.Add(command);
            }
        }

        /// <summary>
        /// Show the caption settings
        /// </summary>
        /// <param name="command">the command</param>
        private void OnShowCaptionSettings(IUICommand command)
        {
            if (this.loadCaptionSettings != null)
            {
                this.loadCaptionSettings(this, new CustomCaptionSettingsEventArgs(this.plugin.Settings));
            }

            var flyout = new CaptionSettingFlyout
            {
                CaptionSettings = this.plugin.Settings,
                Style = this.SettingsFlyoutStyle
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
            this.plugin.ApplyCaptionSettings(e.Settings);
        }

        /// <summary>
        /// Save the caption settings when the flyout unloads.
        /// </summary>
        /// <param name="sender">the flyout</param>
        /// <param name="e">the routed event arguments</param>
        private void OnUnloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var flyout = sender as CaptionSettingFlyout;

            var captionSettings = flyout.CaptionSettings;

            if (this.saveCaptionSettings != null)
            {
                this.saveCaptionSettings(this, new CustomCaptionSettingsEventArgs(captionSettings));
            }

            this.Save(captionSettings);
        }
        #endregion
    }
}
