// <copyright file="TTMLCaptionSettingsPlugin.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-07</date>
// <summary>TTML Caption Settings Plug-in</summary>

namespace Microsoft.PlayerFramework.TTML.CaptionSettings
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.PlayerFramework.CaptionSettings;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Microsoft.PlayerFramework.TimedText;
    using Microsoft.TimedText;
#if WINDOWS_PHONE
    using FF = System.Windows.Media;
    using Media = System.Windows.Media;
#else
    using FF = Microsoft.TimedText;
    using Media = Windows.UI;
#endif

    /// <summary>
    /// TTML Caption Settings Plug-in for Microsoft Media Platform Player Framework
    /// </summary>
    /// <remarks>TimedTextStyle.FontFamily is not implemented for Windows Phone 8.
    /// Do not call MediaPlayer.Dispose() when navigating away from 
    /// the page hosting the player.
    /// </remarks>
    public class TTMLCaptionSettingsPlugin : CaptionSettingsPluginBase
    {
        #region Fields
        /// <summary>
        /// the font map
        /// </summary>
        private static Dictionary<Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily, FF.FontFamily> fontMap;
        #endregion

        #region Methods
        /// <summary>
        /// Reset the selected caption to parse it with the new settings.
        /// </summary>
        /// <param name="settings">the updated caption settings</param>
        public override void OnApplyCaptionSettings(CustomCaptionSettings settings)
        {
            if (settings == null)
            {
                var selectedCaptions = this.MediaPlayer.SelectedCaption;

                this.MediaPlayer.SelectedCaption = null;

                this.MediaPlayer.SelectedCaption = selectedCaptions;
            }
        }

        /// <summary>
        /// Attach the CaptionParsed event handler
        /// </summary>
        /// <returns>true if the TTML CaptionsPlugin is registered</returns>
        protected override bool OnActivate()
        {
            var captionsPlugin = this.MediaPlayer.GetCaptionsPlugin();

            if (captionsPlugin == null)
            {
                System.Diagnostics.Debug.WriteLine("No TTML CaptionsPlugin is installed in this player - caption settings will not be used.");

                return false;
            }

            captionsPlugin.CaptionParsed += this.OnCaptionParsed;

            var succeeded = base.OnActivate();

            if (succeeded)
            {
                if (this.Settings != null)
                {
                    this.Settings.PropertyChanged += this.Settings_PropertyChanged;
                }
            }

            return succeeded;
        }

        /// <summary>
        /// Detach the CaptionParsed event handler
        /// </summary>
        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            var captionsPlugin = this.MediaPlayer.GetCaptionsPlugin();

            if (captionsPlugin != null)
            {
                captionsPlugin.CaptionParsed -= this.OnCaptionParsed;
            }
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Recursively update the caption elements
        /// </summary>
        /// <param name="captionElement">the caption element</param>
        /// <param name="userSettings">the user settings</param>
        /// <param name="level">the 0-based level of the caption element</param>
        private static void UpdateElement(
            TimedTextElement captionElement,
            CustomCaptionSettings userSettings,
            uint level)
        {
            if (level == 0)
            {
                if (userSettings.WindowColor != null)
                {
                    captionElement.Style.BackgroundColor = userSettings.WindowColor.ToColor();
                }
            }
            else if (level == 1)
            {
                if (userSettings.BackgroundColor != null)
                {
                    captionElement.Style.BackgroundColor = userSettings.BackgroundColor.ToColor();
                }
            }

            if (userSettings.FontColor != null)
            {
                captionElement.Style.Color = userSettings.FontColor.ToColor();
            }

            if (userSettings.FontSize.HasValue)
            {
                captionElement.Style.FontSize = new Length
                {
                    Unit = captionElement.Style.FontSize.Unit,
                    Value = System.Convert.ToDouble(userSettings.FontSize.Value) * captionElement.Style.FontSize.Value / 100.0
                };
            }

            var fontFamily = GetFontFamily(userSettings);

            if (fontFamily != null)
            {
                captionElement.Style.FontFamily = fontFamily;

#if WINDOWS_PHONE
                if (userSettings.FontFamily == FontFamily.Cursive)
                {
                    captionElement.Style.FontStyle = System.Windows.FontStyles.Italic;
                }
#endif
            }

            ApplyFontStyle(captionElement, userSettings);

            var children = captionElement.Children as MediaMarkerCollection<TimedTextElement>;

            if (children != null)
            {
                foreach (var child in children)
                {
                    UpdateElement(child, userSettings, level + 1);
                }
            }
        }

        /// <summary>
        /// Apply the font style
        /// </summary>
        /// <param name="captionElement">the caption element</param>
        /// <param name="userSettings">the user settings</param>
        private static void ApplyFontStyle(TimedTextElement captionElement, CustomCaptionSettings userSettings)
        {
            var outlineWidth = 1.0;

            if (userSettings.FontSize.HasValue)
            {
                outlineWidth = System.Convert.ToDouble(userSettings.FontSize.Value) / 100.0;
            }

            var outlineColor = Media.Colors.Black;

            if (userSettings.FontColor != null)
            {
                outlineColor = Media.Color.FromArgb(255, 0, 0, 0);
            }

            switch (userSettings.FontStyle)
            {
                case FontStyle.Default:
                    captionElement.Style.TextStyle = TextStyle.Default;

                    // Todo: look at code for calculation of OutlineWidth and OutlineBlur
                    captionElement.Style.OutlineWidth = new Length { Value = outlineWidth, Unit = LengthUnit.Pixel };
                    captionElement.Style.OutlineColor = outlineColor;
                    break;

                case FontStyle.DepressedEdge:
                    captionElement.Style.TextStyle = TextStyle.DepressedEdge;
                    captionElement.Style.OutlineColor = outlineColor;
                    captionElement.Style.OutlineWidth = new Length { Value = outlineWidth, Unit = LengthUnit.Pixel };
                    break;

                case FontStyle.DropShadow:
                    captionElement.Style.TextStyle = TextStyle.DropShadow;

                    // Todo: look at code for calculation of OutlineWidth and OutlineBlur
                    captionElement.Style.OutlineColor = outlineColor;
                    captionElement.Style.OutlineWidth = new Length { Value = outlineWidth, Unit = LengthUnit.Pixel };
                    break;

                case FontStyle.None:
                    captionElement.Style.TextStyle = TextStyle.None;
                    break;

                case FontStyle.Outline:
                    captionElement.Style.TextStyle = TextStyle.Outline;
                    captionElement.Style.OutlineWidth = new Length { Value = outlineWidth, Unit = LengthUnit.Pixel };
                    captionElement.Style.OutlineColor = outlineColor;
                    break;

                case FontStyle.RaisedEdge:
                    captionElement.Style.TextStyle = TextStyle.RaisedEdge;
                    captionElement.Style.OutlineWidth = new Length { Value = outlineWidth, Unit = LengthUnit.Pixel };
                    captionElement.Style.OutlineColor = outlineColor;
                    break;
            }
        }

        /// <summary>
        /// Gets the font family from the user settings Font
        /// </summary>
        /// <param name="userSettings">the user settings</param>
        /// <returns>the font family</returns>
        private static FF.FontFamily GetFontFamily(CustomCaptionSettings userSettings)
        {
            if (fontMap == null)
            {
                fontMap = new Dictionary<Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily, FF.FontFamily>();

                // _Smallcaps is a unique keyword that will trigger the usage of Typography.SetCapitals(textblock, FontCapitals.SmallCaps)
                // and the default font.
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.Smallcaps] = new FF.FontFamily("_Smallcaps");
            }

            FF.FontFamily fontFamily;

            if (fontMap.TryGetValue(userSettings.FontFamily, out fontFamily))
            {
                return fontFamily;
            }

            var name = GetFontFamilyName(userSettings.FontFamily);

            if (name == null)
            {
                return null;
            }

            fontFamily = new FF.FontFamily(name);

            fontMap[userSettings.FontFamily] = fontFamily;

            return fontFamily;
        }

        /// <summary>
        /// Update the caption elements when the captions are parsed
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the caption parsed event arguments</param>
        private void OnCaptionParsed(object sender, Microsoft.TimedText.CaptionParsedEventArgs e)
        {
            if (this.Settings == null || this.IsDefault)
            {
                return;
            }

            var captionRegion = e.CaptionMarker as CaptionRegion;

            UpdateElement(captionRegion, this.Settings, 0);
        }

        /// <summary>
        /// Update the media player captions when the settings change
        /// </summary>
        /// <param name="sender">the caption settings</param>
        /// <param name="e">the property changed event arguments</param>
        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var selectedCaptions = this.MediaPlayer.SelectedCaption;

            this.MediaPlayer.SelectedCaption = null;

            this.MediaPlayer.SelectedCaption = selectedCaptions;
        }
        #endregion
    }
}
