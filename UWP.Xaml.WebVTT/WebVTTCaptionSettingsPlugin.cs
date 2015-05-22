using Microsoft.Media.WebVTT;
using System.Collections.Generic;
using Windows.Media.ClosedCaptioning;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Microsoft.PlayerFramework.WebVTT.CaptionSettings
{
    /// <summary>
    /// WebVTT caption settings plug-in for MediaPlayer
    /// </summary>
    /// <remarks>Do not call MediaPlayer.Dispose() when navigating away from 
    /// the page hosting the player.
    /// </remarks>
    public class WebVTTCaptionSettingsPlugin : PluginBase
    {
        #region Fields
        /// <summary>
        /// default font size percent
        /// </summary>
        private const double DefaultFontSizePercent = 5.0;

        /// <summary>
        /// the font family map
        /// </summary>
        private static Dictionary<ClosedCaptionStyle, string> fontFamilyMap;

        /// <summary>
        /// the font map
        /// </summary>
        private static Dictionary<ClosedCaptionStyle, FontFamily> fontMap;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the WebVTTCaptionSettingsPlugin class.
        /// </summary>
        public WebVTTCaptionSettingsPlugin()
        {
            this.DropShadowOffset = 1.5;
            this.EdgeOffset = 2;
            this.DropShadowBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the pixel offset at 100% to offset the drop shadow 
        /// </summary>
        /// <remarks>default is 1.5 pixels</remarks>
        public double DropShadowOffset { get; set; }

        /// <summary>
        /// Gets or sets the edge offset for DepressedEdge and RaisedEdge font styles
        /// </summary>
        public double EdgeOffset { get; set; }

        /// <summary>
        /// Gets or sets the drop shadow brush
        /// </summary>
        /// <remarks>Default is a black brush with 50% opacity</remarks>
        public Brush DropShadowBrush { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Apply caption settings
        /// </summary>
        /// <param name="settings">the new caption settings</param>
        protected virtual void OnApplyCaptionSettings()
        {
            var plugin = this.MediaPlayer.GetWebVTTPlugin();
            var fontSize = plugin.CaptionsPanel.FontSize;
            if (ClosedCaptionProperties.FontSize != ClosedCaptionSize.Default)
            {
                var fontSizePercentage = DefaultFontSizePercent * GetComputedFontPercent(ClosedCaptionProperties.FontSize);
                plugin.CaptionsPanel.FontSizePercent = fontSizePercentage;
            }
            else
            {
                plugin.CaptionsPanel.FontSizePercent = DefaultFontSizePercent;
            }

            if (ClosedCaptionProperties.RegionColor != ClosedCaptionColor.Default || ClosedCaptionProperties.RegionOpacity != ClosedCaptionOpacity.Default)
            {
                SolidColorBrush brush = GetComputedBrush(ClosedCaptionProperties.RegionColor, ClosedCaptionProperties.ComputedRegionColor, ClosedCaptionProperties.RegionOpacity);
                plugin.CaptionsPanel.PanelBackground = brush;
            }
            else
            {
                plugin.CaptionsPanel.PanelBackground = null;
            }

            if (ClosedCaptionProperties.BackgroundColor != ClosedCaptionColor.Default || ClosedCaptionProperties.BackgroundOpacity != ClosedCaptionOpacity.Default)
            {
                var style = new Style(typeof(BoxElement));
                SolidColorBrush brush = GetComputedBrush(ClosedCaptionProperties.BackgroundColor, ClosedCaptionProperties.ComputedBackgroundColor, ClosedCaptionProperties.BackgroundOpacity);
                style.Setters.Add(new Setter(Control.BackgroundProperty, brush));

                plugin.CaptionsPanel.BoxStyle = style;
            }
            else
            {
                plugin.CaptionsPanel.BoxStyle = null;
            }
        }

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

        private static SolidColorBrush GetComputedBrush(ClosedCaptionColor color, Color computedColor, ClosedCaptionOpacity opacity)
        {
            SolidColorBrush brush;
            if (color != ClosedCaptionColor.Default)
            {
                brush = new SolidColorBrush(computedColor);
            }
            else
            {
                brush = new SolidColorBrush();
            }
            if (opacity != ClosedCaptionOpacity.Default)
            {
                brush.Opacity = GetComputedOpacity(opacity);
            }
            return brush;
        }

        /// <summary>
        /// hook up the NodeRendering event handler
        /// </summary>
        /// <returns>true if the WebVTTPlugin is in the MediaPlayer</returns>
        protected override bool OnActivate()
        {
            var plugin = this.MediaPlayer.GetWebVTTPlugin();

            if (plugin == null)
            {
                return false;
            }

            plugin.CaptionsPanel.TextRendering += this.OnTextRendering;
            OnApplyCaptionSettings();

            return true;
        }

        /// <summary>
        /// Detach the NodeRendering event handler
        /// </summary>
        protected override void OnDeactivate()
        {
            if (this.MediaPlayer != null)
            {
                var plugin = this.MediaPlayer.GetWebVTTPlugin();

                if (plugin != null)
                {
                    var panel = plugin.CaptionsPanel;

                    if (panel != null)
                    {
                        panel.TextRendering -= this.OnTextRendering;
                    }
                }
            }
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Text is about to be rendered so apply any styles
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the caption text event arguments</param>
        private void OnTextRendering(object sender, CaptionTextEventArgs e)
        {
            this.ApplyFontFamily(e);

            this.ApplyTextColor(e);

            this.ApplyFontStyle(e);
        }

        /// <summary>
        /// Apply the font family
        /// </summary>
        /// <param name="e">the caption text event arguments</param>
        private void ApplyFontFamily(CaptionTextEventArgs e)
        {
            switch (ClosedCaptionProperties.FontStyle)
            {
                case ClosedCaptionStyle.SmallCapitals:
                    Typography.SetCapitals(e.TextBlock, FontCapitals.SmallCaps);
                    break;

                case ClosedCaptionStyle.Default:
                    break;

                default:
                    e.TextBlock.FontFamily = GetFont();
                    break;
            }
        }

        /// <summary>
        /// Apply the text color
        /// </summary>
        /// <param name="e">the caption text event arguments</param>
        private void ApplyTextColor(CaptionTextEventArgs e)
        {
            if (e.Position == TextPosition.Center && (ClosedCaptionProperties.FontColor != ClosedCaptionColor.Default || ClosedCaptionProperties.FontOpacity != ClosedCaptionOpacity.Default))
            {
                if (ClosedCaptionProperties.FontColor != ClosedCaptionColor.Default)
                    e.TextBlock.Foreground = new SolidColorBrush(ClosedCaptionProperties.ComputedFontColor);
                if (ClosedCaptionProperties.FontOpacity != ClosedCaptionOpacity.Default)
                    e.TextBlock.Opacity = GetComputedOpacity(ClosedCaptionProperties.FontOpacity);
            }
        }

        /// <summary>
        /// Apply the font style
        /// </summary>
        /// <param name="e">the caption text event arguments</param>
        private void ApplyFontStyle(CaptionTextEventArgs e)
        {
            switch (ClosedCaptionProperties.FontEffect)
            {
                case ClosedCaptionEdgeEffect.Default:
                    break;

                case ClosedCaptionEdgeEffect.DropShadow:
                    this.ApplyDropShadow(e);
                    break;

                case ClosedCaptionEdgeEffect.Uniform:
                    break;

                case ClosedCaptionEdgeEffect.Depressed:
                    this.ApplyDepressedEdge(e);
                    break;

                case ClosedCaptionEdgeEffect.None:
                    if (e.Position != TextPosition.Center)
                    {
                        e.TextBlock.Visibility = Visibility.Collapsed;
                    }

                    break;

                case ClosedCaptionEdgeEffect.Raised:
                    this.ApplyRaisedEdge(e);
                    break;
            }
        }

        /// <summary>
        /// Apply a raised edge text style
        /// </summary>
        /// <param name="e">the caption text event arguments</param>
        private void ApplyRaisedEdge(CaptionTextEventArgs e)
        {
            var offset = this.EdgeOffset;

            if (ClosedCaptionProperties.FontSize != ClosedCaptionSize.Default)
            {
                offset = this.EdgeOffset * GetComputedFontPercent(ClosedCaptionProperties.FontSize);
            }

            switch (e.Position)
            {
                case TextPosition.Center:
                    break;

                case TextPosition.TopLeft:
                case TextPosition.Top:
                case TextPosition.TopRight:
                case TextPosition.Left:
                case TextPosition.BottomLeft:
                    e.TextBlock.Visibility = Visibility.Collapsed;
                    break;

                case TextPosition.Right:
                    e.TextBlock.RenderTransform = new TranslateTransform
                    {
                        X = offset
                    };
                    break;

                case TextPosition.Bottom:
                    e.TextBlock.RenderTransform = new TranslateTransform
                    {
                        Y = offset
                    };
                    break;
                case TextPosition.BottomRight:
                    e.TextBlock.RenderTransform = new TranslateTransform
                    {
                        X = offset,
                        Y = offset
                    };
                    break;
            }
        }

        /// <summary>
        /// Apply dark overlay to the upper left text blocks
        /// </summary>
        /// <param name="e">the caption text event arguments</param>
        private void ApplyDepressedEdge(CaptionTextEventArgs e)
        {
            var offset = this.EdgeOffset;

            if (ClosedCaptionProperties.FontSize != ClosedCaptionSize.Default)
            {
                offset = this.EdgeOffset * GetComputedFontPercent(ClosedCaptionProperties.FontSize);
            }

            switch (e.Position)
            {
                case TextPosition.Center:
                    break;

                case TextPosition.TopRight:
                case TextPosition.Bottom:
                case TextPosition.Right:
                case TextPosition.BottomLeft:
                case TextPosition.BottomRight:
                    e.TextBlock.Visibility = Visibility.Collapsed;
                    break;

                case TextPosition.TopLeft:
                    e.TextBlock.RenderTransform = new TranslateTransform
                    {
                        X = -offset,
                        Y = -offset
                    };
                    break;

                case TextPosition.Top:
                    e.TextBlock.RenderTransform = new TranslateTransform
                    {
                        Y = -offset
                    };
                    break;
                case TextPosition.Left:
                    e.TextBlock.RenderTransform = new TranslateTransform
                    {
                        X = -offset
                    };
                    break;
            }
        }

        /// <summary>
        /// Apply a drop shadow text effect
        /// </summary>
        /// <param name="e">the caption text event arguments</param>
        private void ApplyDropShadow(CaptionTextEventArgs e)
        {
            var plugin = this.MediaPlayer.GetWebVTTPlugin();
            var fontSize = plugin.CaptionsPanel.FontSize;

            var offset = this.DropShadowOffset;

            if (ClosedCaptionProperties.FontSize != ClosedCaptionSize.Default)
            {
                offset = this.DropShadowOffset * GetComputedFontPercent(ClosedCaptionProperties.FontSize);
            }

            switch (e.Position)
            {
                case TextPosition.Center:
                    break;

                case TextPosition.Bottom:
                    e.TextBlock.Foreground = this.DropShadowBrush;
                    e.TextBlock.RenderTransform = new TranslateTransform
                    {
                        Y = offset
                    };
                    break;

                case TextPosition.BottomRight:
                    e.TextBlock.Foreground = this.DropShadowBrush;
                    e.TextBlock.RenderTransform = new TranslateTransform
                    {
                        X = offset,
                        Y = offset
                    };
                    break;

                case TextPosition.Right:
                    e.TextBlock.Foreground = this.DropShadowBrush;
                    e.TextBlock.RenderTransform = new TranslateTransform
                    {
                        X = offset
                    };
                    break;

                default:
                    e.TextBlock.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        /// <summary>
        /// Gets the mapped font
        /// </summary>
        /// <returns>a native font family</returns>
        private static FontFamily GetFont()
        {
            if (fontMap == null)
            {
                fontMap = new Dictionary<ClosedCaptionStyle, FontFamily>();
            }

            FontFamily fontFamily;

            if (fontMap.TryGetValue(ClosedCaptionProperties.FontStyle, out fontFamily))
            {
                return fontFamily;
            }

            var name = GetFontFamilyName(ClosedCaptionProperties.FontStyle);

            if (name == null)
            {
                return null;
            }

            fontFamily = new FontFamily(name);

            fontMap[ClosedCaptionProperties.FontStyle] = fontFamily;

            return fontFamily;
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
