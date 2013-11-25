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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
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
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
        public Style SettingsFlyoutStyle { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Add the settings pane and load the settings from local storage
        /// </summary>
        internal void Activate()
        {
            SettingsPane.GetForCurrentView().CommandsRequested += this.CaptionsSettingsPlugin_CommandsRequested;

            object value;

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue(LocalSettingsKey, out value))
            {
                var settingsString = value.ToString();

                this.Settings = CustomCaptionSettings.FromString(settingsString);
            }
            else
            {
                this.Settings = new CustomCaptionSettings();
            }
        }

        /// <summary>
        /// Detach the settings pane
        /// </summary>
        internal void Deactivate()
        {
            SettingsPane.GetForCurrentView().CommandsRequested -= this.CaptionsSettingsPlugin_CommandsRequested;
        }

        /// <summary>
        /// Save the settings to local storage
        /// </summary>
        internal void Save()
        {
            if (this.Settings == null)
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
        /// Adds the caption settings command to the plug-in
        /// </summary>
        /// <param name="sender">the settings pane</param>
        /// <param name="args">the settings pane commands requested event arguments</param>
        private void CaptionsSettingsPlugin_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            if (this.MediaPlayer == null)
            {
                return;
            }

            var command = (from item in args.Request.ApplicationCommands
                                where item.Id.ToString() == this.SettingsCommandId
                                select item).FirstOrDefault();

            if (command == null)
            {
                // the command isn't already in the list
                command = new SettingsCommand(
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
            else
            {
                // the command is in the list so only hook up the Invoked handler
                command.Invoked += this.OnShowCaptionSettings;
            }
        }

        /// <summary>
        /// Show the caption settings
        /// </summary>
        /// <param name="command">the command</param>
        private void OnShowCaptionSettings(IUICommand command)
        {
            if (this.OnLoadCaptionSettings != null)
            {
                this.OnLoadCaptionSettings(this, new CustomCaptionSettingsEventArgs(this.Settings));
            }

            var flyout = new CaptionSettingFlyout
            {
                CaptionSettings = this.Settings,
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

            this.Settings = captionSettings;

            this.Save();
        }
        #endregion
    }
}
