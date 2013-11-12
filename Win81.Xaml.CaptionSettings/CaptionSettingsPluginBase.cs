// <copyright file="CaptionSettingsPluginBase.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-10-28</date>
// <summary>Caption Settings Plugin base class</summary>

namespace Microsoft.PlayerFramework.CaptionSettings
{
    using System;
    using Microsoft.PlayerFramework.CaptionSettings.Model;

    /// <summary>
    /// base class for Caption Settings Plug-ins
    /// </summary>
    public partial class CaptionSettingsPluginBase : PluginBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CaptionsSettingsPluginBase class.
        /// </summary>
        #endregion

        #region Events
        /// <summary>
        /// Event to load caption settings
        /// </summary>
        public event EventHandler<CustomCaptionSettingsEventArgs> OnLoadCaptionSettings;

        /// <summary>
        /// Event to save caption settings
        /// </summary>
        public event EventHandler<CustomCaptionSettingsEventArgs> OnSaveCaptionSettings;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the settings
        /// </summary>
        public CustomCaptionSettings Settings { get; set; }

        #endregion

        #region Methods
        /// <summary>
        /// Apply the caption settings to the media player
        /// </summary>
        /// <param name="settings">the caption settings</param>
        public void ApplyCaptionSettings(CustomCaptionSettings settings)
        {
            if (this.MediaPlayer == null)
            {
                return;
            }

            this.Settings = settings;

            // let the derived class apply the settings
            this.OnApplyCaptionSettings(settings);

            if (this.OnSaveCaptionSettings != null)
            {
                this.OnSaveCaptionSettings(this, new CustomCaptionSettingsEventArgs(this.Settings));
            }

            this.Save(this.Settings);
        }

        /// <summary>
        /// Default implementation does nothing - derived classes should 
        /// override this to apply caption settings.
        /// </summary>
        /// <param name="settings">the custom caption settings</param>
        public virtual void OnApplyCaptionSettings(CustomCaptionSettings settings)
        {
        }

        /// <summary>
        /// Convert the color type to an opacity
        /// </summary>
        /// <param name="colorType">the color type</param>
        /// <returns>the opacity from 0-100</returns>
        protected static uint? GetOpacity(ColorType colorType)
        {
            switch (colorType)
            {
                case ColorType.Default:
                    return null;

                case ColorType.Semitransparent:
                    return 50;

                case ColorType.Solid:
                    return 100;

                case ColorType.Transparent:
                    return 0;
            }

            return null;
        }

        /// <summary>
        /// Activate the plug-in
        /// </summary>
        /// <returns>true if the CaptionsPlugIn is also installed</returns>
        protected override bool OnActivate()
        {
            this.Activate(this, this.OnLoadCaptionSettings, this.OnSaveCaptionSettings);

            if (this.OnLoadCaptionSettings != null)
            {
                var eventArgs = new CustomCaptionSettingsEventArgs(this.Settings);

                this.OnLoadCaptionSettings(this, eventArgs);

                this.Settings = eventArgs.Settings;
            }

            this.ApplyCaptionSettings(this.Settings);

            return true;
        }

        /// <summary>
        /// Disconnect the settings pane command requested and caption parsed event handlers.
        /// </summary>
        protected override void OnDeactivate()
        {
            this.Deactivate();
        }

        #endregion

        #region Implementation
        #endregion
    }
}
