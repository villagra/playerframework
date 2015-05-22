using Microsoft.PlayerFramework.TimedText;
using System.Collections.Generic;
using Windows.Media.ClosedCaptioning;
using Windows.UI;
using TT = Microsoft.Media.TimedText;

namespace Microsoft.PlayerFramework.TTML.CaptionSettings
{
    /// <summary>
    /// TTML Caption Settings Plug-in for Microsoft Media Platform Player Framework
    /// </summary>
    /// <remarks>TimedTextStyle.FontFamily is not implemented for Windows Phone 8.
    /// Do not call MediaPlayer.Dispose() when navigating away from 
    /// the page hosting the player.
    /// </remarks>
    public class TTMLCaptionSettingsPlugin : PluginBase
    {
        #region Fields
        /// <summary>
        /// the font family map
        /// </summary>
        private static Dictionary<ClosedCaptionStyle, string> fontFamilyMap;

        /// <summary>
        /// the font map
        /// </summary>
        private static Dictionary<ClosedCaptionStyle, TT.FontFamily> fontMap;
        #endregion

        #region Methods
        /// <summary>
        /// Reset the selected caption to parse it with the new settings.
        /// </summary>
        public void OnApplyCaptionSettings()
        {
            var selectedCaptions = this.MediaPlayer.SelectedCaption;

            this.MediaPlayer.SelectedCaption = null;

            this.MediaPlayer.SelectedCaption = selectedCaptions;
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
                return false;
            }

            captionsPlugin.CaptionParsed += this.OnCaptionParsed;

            OnApplyCaptionSettings();

            return true;
        }

        /// <summary>
        /// Detach the CaptionParsed event handler
        /// </summary>
        protected override void OnDeactivate()
        {
            var captionsPlugin = this.MediaPlayer.GetCaptionsPlugin();

            if (captionsPlugin != null)
            {
                captionsPlugin.CaptionParsed -= this.OnCaptionParsed;
            }
        }

        #endregion

        #region Implementation

        private static double GetComputedFontPercent(ClosedCaptionSize size)
        {
            switch (size)
            {
                case ClosedCaptionSize.FiftyPercent:
                    return .5;
                case ClosedCaptionSize.OneHundredFiftyPercent:
                    return 1.5;
                case ClosedCaptionSize.TwoHundredPercent:
                    return 2;
                default:
                    return 1;
            }
        }

        private static double GetComputedOpacity(ClosedCaptionOpacity opacity)
        {
            switch (opacity)
            {
                case ClosedCaptionOpacity.SeventyFivePercent:
                    return .75;
                case ClosedCaptionOpacity.TwentyFivePercent:
                    return .25;
                case ClosedCaptionOpacity.ZeroPercent:
                    return 0;
                default:
                    return 1;
            }
        }

        private static Color GetComputedColor(Color computedColor, ClosedCaptionOpacity opacity)
        {
            var a = (byte)(int)(GetComputedOpacity(opacity) * 255d);
            return Color.FromArgb(a, computedColor.R, computedColor.G, computedColor.B);
        }

        /// <summary>
        /// Recursively update the caption elements
        /// </summary>
        /// <param name="captionElement">the caption element</param>
        /// <param name="level">the 0-based level of the caption element</param>
        private static void UpdateElement(
            TT.TimedTextElement captionElement,
            uint level)
        {
            if (level == 0)
            {
                if (ClosedCaptionProperties.RegionColor != ClosedCaptionColor.Default || ClosedCaptionProperties.RegionOpacity != ClosedCaptionOpacity.Default)
                {
                    captionElement.Style.BackgroundColor = GetComputedColor(ClosedCaptionProperties.ComputedRegionColor, ClosedCaptionProperties.RegionOpacity);
                }
            }
            else if (level == 1)
            {
                if (ClosedCaptionProperties.BackgroundColor != ClosedCaptionColor.Default || ClosedCaptionProperties.BackgroundOpacity != ClosedCaptionOpacity.Default)
                {
                    captionElement.Style.BackgroundColor = GetComputedColor(ClosedCaptionProperties.ComputedBackgroundColor, ClosedCaptionProperties.BackgroundOpacity);
                }
            }

            if (ClosedCaptionProperties.FontColor != ClosedCaptionColor.Default || ClosedCaptionProperties.FontOpacity != ClosedCaptionOpacity.Default)
            {
                captionElement.Style.Color = GetComputedColor(ClosedCaptionProperties.ComputedFontColor, ClosedCaptionProperties.FontOpacity);
            }

            if (ClosedCaptionProperties.FontSize != ClosedCaptionSize.Default)
            {
                captionElement.Style.FontSize = new TT.Length
                {
                    Unit = captionElement.Style.FontSize.Unit,
                    Value = GetComputedFontPercent(ClosedCaptionProperties.FontSize) * captionElement.Style.FontSize.Value
                };
            }

            var fontFamily = GetFontFamily();

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

            ApplyFontStyle(captionElement);

            var children = captionElement.Children as TT.MediaMarkerCollection<TT.TimedTextElement>;

            if (children != null)
            {
                foreach (var child in children)
                {
                    UpdateElement(child, level + 1);
                }
            }
        }

        /// <summary>
        /// Apply the font style
        /// </summary>
        /// <param name="captionElement">the caption element</param>
        private static void ApplyFontStyle(TT.TimedTextElement captionElement)
        {
            var outlineWidth = 1.0;

            if (ClosedCaptionProperties.FontSize != ClosedCaptionSize.Default)
            {
                outlineWidth = GetComputedFontPercent(ClosedCaptionProperties.FontSize);
            }

            var outlineColor = Colors.Black;

            if (ClosedCaptionProperties.FontColor != ClosedCaptionColor.Default)
            {
                outlineColor = Color.FromArgb(255, 0, 0, 0);
            }

            switch (ClosedCaptionProperties.FontEffect)
            {
                case ClosedCaptionEdgeEffect.Default:
                    captionElement.Style.TextStyle = TT.TextStyle.Default;

                    // Todo: look at code for calculation of OutlineWidth and OutlineBlur
                    captionElement.Style.OutlineWidth = new TT.Length { Value = outlineWidth, Unit = TT.LengthUnit.Pixel };
                    captionElement.Style.OutlineColor = outlineColor;
                    break;

                case ClosedCaptionEdgeEffect.Depressed:
                    captionElement.Style.TextStyle = TT.TextStyle.DepressedEdge;
                    captionElement.Style.OutlineColor = outlineColor;
                    captionElement.Style.OutlineWidth = new TT.Length { Value = outlineWidth, Unit = TT.LengthUnit.Pixel };
                    break;

                case ClosedCaptionEdgeEffect.DropShadow:
                    captionElement.Style.TextStyle = TT.TextStyle.DropShadow;

                    // Todo: look at code for calculation of OutlineWidth and OutlineBlur
                    captionElement.Style.OutlineColor = outlineColor;
                    captionElement.Style.OutlineWidth = new TT.Length { Value = outlineWidth, Unit = TT.LengthUnit.Pixel };
                    break;

                case ClosedCaptionEdgeEffect.None:
                    captionElement.Style.TextStyle = TT.TextStyle.None;
                    break;

                case ClosedCaptionEdgeEffect.Uniform:
                    captionElement.Style.TextStyle = TT.TextStyle.Outline;
                    captionElement.Style.OutlineWidth = new TT.Length { Value = outlineWidth, Unit = TT.LengthUnit.Pixel };
                    captionElement.Style.OutlineColor = outlineColor;
                    break;

                case ClosedCaptionEdgeEffect.Raised:
                    captionElement.Style.TextStyle = TT.TextStyle.RaisedEdge;
                    captionElement.Style.OutlineWidth = new TT.Length { Value = outlineWidth, Unit = TT.LengthUnit.Pixel };
                    captionElement.Style.OutlineColor = outlineColor;
                    break;
            }
        }

        /// <summary>
        /// Gets the font family from the user settings Font
        /// </summary>
        /// <returns>the font family</returns>
        private static TT.FontFamily GetFontFamily()
        {
            if (fontMap == null)
            {
                fontMap = new Dictionary<ClosedCaptionStyle, TT.FontFamily>();

                // _Smallcaps is a unique keyword that will trigger the usage of Typography.SetCapitals(textblock, FontCapitals.SmallCaps)
                // and the default font.
                fontMap[ClosedCaptionStyle.SmallCapitals] = new TT.FontFamily("_Smallcaps");
            }

            TT.FontFamily fontFamily;

            if (fontMap.TryGetValue(ClosedCaptionProperties.FontStyle, out fontFamily))
            {
                return fontFamily;
            }

            var name = GetFontFamilyName(ClosedCaptionProperties.FontStyle);

            if (name == null)
            {
                return null;
            }

            fontFamily = new TT.FontFamily(name);

            fontMap[ClosedCaptionProperties.FontStyle] = fontFamily;

            return fontFamily;
        }

        /// <summary>
        /// Update the caption elements when the captions are parsed
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the caption parsed event arguments</param>
        private void OnCaptionParsed(object sender, TT.CaptionParsedEventArgs e)
        {
            var captionRegion = e.CaptionMarker as TT.CaptionRegion;

            UpdateElement(captionRegion, 0);
        }

        /// <summary>
        /// Gets the Windows font family mapped to the Captions font Family
        /// </summary>
        /// <param name="fontFamily">the captions font family</param>
        /// <returns>the name of the Windows font family</returns>
        public static string GetFontFamilyName(ClosedCaptionStyle fontFamily)
        {
            if (fontFamilyMap == null)
            {
                fontFamilyMap = new Dictionary<ClosedCaptionStyle, string>();

                fontFamilyMap[ClosedCaptionStyle.Default] = null;
                fontFamilyMap[ClosedCaptionStyle.MonospacedWithSerifs] = "Courier New";
                fontFamilyMap[ClosedCaptionStyle.ProportionalWithSerifs] = "Times New Roman";
                fontFamilyMap[ClosedCaptionStyle.MonospacedWithoutSerifs] = "Consolas";
                fontFamilyMap[ClosedCaptionStyle.ProportionalWithoutSerifs] = "Tahoma";
                fontFamilyMap[ClosedCaptionStyle.Casual] = "Segoe Print";
                fontFamilyMap[ClosedCaptionStyle.Cursive] = "Segoe Script";
                fontFamilyMap[ClosedCaptionStyle.SmallCapitals] = "Tahoma";
            }

            return fontFamilyMap[fontFamily];
        }
        #endregion
    }
}
