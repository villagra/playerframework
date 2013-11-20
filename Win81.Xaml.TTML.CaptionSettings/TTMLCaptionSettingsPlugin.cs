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
    /// <remarks>TimedTextStyle.FontFamily is not implemented for Windows Phone 8.</remarks>
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
            var selectedCaption = this.MediaPlayer.SelectedCaption;

            this.MediaPlayer.SelectedCaption = null;

            this.MediaPlayer.SelectedCaption = selectedCaption;
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

            return base.OnActivate();
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
        /// <param name="isRoot">is this the root element?</param>
        private static void UpdateElement(
            TimedTextElement captionElement,
            CustomCaptionSettings userSettings,
            bool isRoot)
        {
            if (isRoot)
            {
                if (userSettings.WindowColor != null)
                {
                    captionElement.Style.BackgroundColor = userSettings.WindowColor.ToColor();
                }
            }
            else
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
                    Unit = LengthUnit.Pixel,
                    Value = System.Convert.ToDouble(userSettings.FontSize.Value) * captionElement.Style.FontSize.Value / 100.0
                };
            }

            var fontFamily = GetFontFamily(userSettings);

            if (fontFamily != null)
            {
                captionElement.Style.FontFamily = fontFamily;
            }

            ApplyFontStyle(captionElement, userSettings);

            var children = captionElement.Children as MediaMarkerCollection<TimedTextElement>;

            if (children != null)
            {
                foreach (var child in children)
                {
                    UpdateElement(child, userSettings, false);
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

            switch (userSettings.FontStyle)
            {
                case FontStyle.Default:
                    captionElement.Style.TextStyle = TextStyle.Default;
                    
                    // Todo: look at code for calculation of OutlineWidth and OutlineBlur
                    captionElement.Style.OutlineWidth = new Length { Value = outlineWidth, Unit = LengthUnit.Pixel };
                    captionElement.Style.OutlineColor = Media.Colors.Black;
                    break;

                case FontStyle.DepressedEdge:
                    captionElement.Style.TextStyle = TextStyle.DepressedEdge;
                    captionElement.Style.OutlineColor = Media.Colors.Black;
                    captionElement.Style.OutlineWidth = new Length { Value = outlineWidth, Unit = LengthUnit.Pixel };
                    break;

                case FontStyle.DropShadow:
                    captionElement.Style.TextStyle = TextStyle.DropShadow;
                    
                    // Todo: look at code for calculation of OutlineWidth and OutlineBlur
                    captionElement.Style.OutlineColor = Media.Colors.Black;
                    captionElement.Style.OutlineWidth = new Length { Value = outlineWidth * 2, Unit = LengthUnit.Pixel };
                    break;

                case FontStyle.None:
                    captionElement.Style.TextStyle = TextStyle.None;
                    break;

                case FontStyle.Outline:
                    captionElement.Style.TextStyle = TextStyle.Outline;
                    captionElement.Style.OutlineWidth = new Length { Value = outlineWidth, Unit = LengthUnit.Pixel };
                    captionElement.Style.OutlineColor = Media.Colors.Black;
                    break;

                case FontStyle.RaisedEdge:
                    captionElement.Style.TextStyle = TextStyle.RaisedEdge;
                    captionElement.Style.OutlineWidth = new Length { Value = outlineWidth, Unit = LengthUnit.Pixel };
                    captionElement.Style.OutlineColor = Media.Colors.Black;
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
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.Default] = null;
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.MonospaceSerif] = new FF.FontFamily("Courier New");
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.ProportionalSerif] = new FF.FontFamily("Cambria");
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.MonospaceSansSerif] = new FF.FontFamily("Consolas");
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.ProportionalSansSerif] = new FF.FontFamily("Arial");
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.Casual] = new FF.FontFamily("Comic Sans MS");
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.Cursive] = new FF.FontFamily("Segoe Script");

                // _Smallcaps is a unique keyword that will trigger the usage of Typography.SetCapitals(textblock, FontCapitals.SmallCaps)
                // and the default font.
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.Smallcaps] = new FF.FontFamily("_Smallcaps");
            }

            var fontName = fontMap[userSettings.FontFamily];

            return fontName;
        }

        /// <summary>
        /// Update the caption elements when the captions are parsed
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the caption parsed event arguments</param>
        private void OnCaptionParsed(object sender, Microsoft.TimedText.CaptionParsedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Caption parsed.");

            if (this.Settings == null)
            {
                Debug.WriteLine("Captions parsed without user settings.");

                return;
            }

            var captionRegion = e.CaptionMarker as CaptionRegion;

            UpdateElement(captionRegion, this.Settings, true);
        }
        #endregion
    }
}
