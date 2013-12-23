// <copyright file="WebVTTCaptionSettingsPlugin.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-13</date>
// <summary>WebVTT Caption Settings</summary>

namespace Microsoft.PlayerFramework.WebVTT.CaptionSettings
{
    using System.Collections.Generic;
    using System.Diagnostics;
#if WINDOWS_PHONE
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
#endif
    using Microsoft.PlayerFramework.CaptionSettings;
    using Microsoft.PlayerFramework.WebVTT;
    using Microsoft.WebVTT;

#if WINDOWS_PHONE
    using Media = System.Windows.Media;
    using UI = System.Windows.Media;
#else
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Documents;
    using Windows.UI.Xaml.Media;
    using Media = Windows.UI.Xaml.Media;
    using UI = Windows.UI;
#endif

    /// <summary>
    /// WebVTT caption settings plug-in for MediaPlayer
    /// </summary>
    /// <remarks>Do not call MediaPlayer.Dispose() when navigating away from 
    /// the page hosting the player.
    /// </remarks>
    public class WebVTTCaptionSettingsPlugin : CaptionSettingsPluginBase
    {
        #region Fields
        /// <summary>
        /// default font size percent
        /// </summary>
        private const double DefaultFontSizePercent = 5.0;

        /// <summary>
        /// the font map
        /// </summary>
        private Dictionary<Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily, Media.FontFamily> fontMap;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the WebVTTCaptionSettingsPlugin class.
        /// </summary>
        public WebVTTCaptionSettingsPlugin()
        {
            this.DropShadowOffset = 1.5;
            this.EdgeOffset = 2;
            this.DropShadowBrush = new Media.SolidColorBrush(UI.Color.FromArgb(128, 0, 0, 0));
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
        public Media.Brush DropShadowBrush { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Apply caption settings
        /// </summary>
        /// <param name="settings">the new caption settings</param>
        public override void OnApplyCaptionSettings(PlayerFramework.CaptionSettings.Model.CustomCaptionSettings settings)
        {
            var plugin = this.MediaPlayer.GetWebVTTPlugin();
            var fontSize = plugin.CaptionsPanel.FontSize;
            var fontSizePercentage = plugin.CaptionsPanel.FontSizePercent;
            if (settings != null && settings.FontSize.HasValue)
            {
                fontSizePercentage = DefaultFontSizePercent * System.Convert.ToDouble(settings.FontSize.Value) / 100.0;

                plugin.CaptionsPanel.FontSizePercent = fontSizePercentage;
            }
            else
            {
                plugin.CaptionsPanel.FontSizePercent = DefaultFontSizePercent;
            }

            if (settings != null && settings.WindowColor != null)
            {
                var color = this.Settings.WindowColor.ToColor();

                plugin.CaptionsPanel.PanelBackground = new Media.SolidColorBrush(color);
            }
            else
            {
                plugin.CaptionsPanel.PanelBackground = null;
            }

            if (settings != null && settings.BackgroundColor != null)
            {
                var color = this.Settings.BackgroundColor.ToColor();

                var style = new Style(typeof(BoxElement));

                style.Setters.Add(new Setter(Control.BackgroundProperty, new Media.SolidColorBrush(color)));

                plugin.CaptionsPanel.BoxStyle = style;
            }
            else
            {
                plugin.CaptionsPanel.BoxStyle = null;
            }
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
                Debug.WriteLine("You must add the WebVTTPlugIn in order to make the caption settings work.");

                return false;
            }

            plugin.CaptionsPanel.TextRendering += this.OnTextRendering;

            return base.OnActivate();
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

            base.OnDeactivate();
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
            if (this.Settings == null || this.IsDefault)
            {
                return;
            }

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
            switch (this.Settings.FontFamily)
            {
                case PlayerFramework.CaptionSettings.Model.FontFamily.Smallcaps:
                    Typography.SetCapitals(e.TextBlock, FontCapitals.SmallCaps);
                    break;

                case PlayerFramework.CaptionSettings.Model.FontFamily.Default:
                    break;

                default:
                    e.TextBlock.FontFamily = this.GetFont();
                    break;
            }
        }

        /// <summary>
        /// Apply the text color
        /// </summary>
        /// <param name="e">the caption text event arguments</param>
        private void ApplyTextColor(CaptionTextEventArgs e)
        {
            if (e.Position == TextPosition.Center && 
                this.Settings.FontColor != null)
            {
                var fontColor = this.Settings.FontColor.ToColor();

                e.TextBlock.Foreground = new Media.SolidColorBrush(fontColor);
            }
        }

        /// <summary>
        /// Apply the font style
        /// </summary>
        /// <param name="e">the caption text event arguments</param>
        private void ApplyFontStyle(CaptionTextEventArgs e)
        {
            switch (this.Settings.FontStyle)
            {
                case PlayerFramework.CaptionSettings.Model.FontStyle.Default:
                    break;

                case PlayerFramework.CaptionSettings.Model.FontStyle.DropShadow:
                    this.ApplyDropShadow(e);
                    break;

                case PlayerFramework.CaptionSettings.Model.FontStyle.Outline:
                    break;

                case PlayerFramework.CaptionSettings.Model.FontStyle.DepressedEdge:
                    this.ApplyDepressedEdge(e);
                    break;

                case PlayerFramework.CaptionSettings.Model.FontStyle.None:
                    if (e.Position != TextPosition.Center)
                    {
                        e.TextBlock.Visibility = Visibility.Collapsed;
                    }

                    break;

                case PlayerFramework.CaptionSettings.Model.FontStyle.RaisedEdge:
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

            if (this.Settings.FontSize.HasValue)
            {
                offset = this.EdgeOffset * System.Convert.ToDouble(this.Settings.FontSize.Value) / 100.0;
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
                    e.TextBlock.RenderTransform = new Media.TranslateTransform
                    {
                        X = offset
                    };
                    break;

                case TextPosition.Bottom:
                    e.TextBlock.RenderTransform = new Media.TranslateTransform
                    {
                        Y = offset
                    };
                    break;
                case TextPosition.BottomRight:
                    e.TextBlock.RenderTransform = new Media.TranslateTransform
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

            if (this.Settings.FontSize.HasValue)
            {
                offset = this.EdgeOffset * System.Convert.ToDouble(this.Settings.FontSize.Value) / 100.0;
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
                    e.TextBlock.RenderTransform = new Media.TranslateTransform
                    {
                        X = -offset,
                        Y = -offset
                    };
                    break;

                case TextPosition.Top:
                    e.TextBlock.RenderTransform = new Media.TranslateTransform
                    {
                        Y = -offset
                    };
                    break;
                case TextPosition.Left:
                    e.TextBlock.RenderTransform = new Media.TranslateTransform
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

            if (this.Settings.FontSize.HasValue)
            {
                offset = this.DropShadowOffset * System.Convert.ToDouble(this.Settings.FontSize.Value) / 100.0;
            }

            switch (e.Position)
            {
                case TextPosition.Center:
                    break;

                case TextPosition.Bottom:
                    e.TextBlock.Foreground = this.DropShadowBrush;
                    e.TextBlock.RenderTransform = new Media.TranslateTransform
                    {
                        Y = offset
                    };
                    break;

                case TextPosition.BottomRight:
                    e.TextBlock.Foreground = this.DropShadowBrush;
                    e.TextBlock.RenderTransform = new Media.TranslateTransform
                    {
                        X = offset,
                        Y = offset
                    };
                    break;

                case TextPosition.Right:
                    e.TextBlock.Foreground = this.DropShadowBrush;
                    e.TextBlock.RenderTransform = new Media.TranslateTransform
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
        private Media.FontFamily GetFont()
        {
            if (this.fontMap == null)
            {
                this.fontMap = new Dictionary<PlayerFramework.CaptionSettings.Model.FontFamily, Media.FontFamily>();
            }

            Media.FontFamily fontFamily;

            if (this.fontMap.TryGetValue(this.Settings.FontFamily, out fontFamily))
            {
                return fontFamily;
            }

            var name = GetFontFamilyName(this.Settings.FontFamily);

            if (name == null)
            {
                return null;
            }

            fontFamily = new Media.FontFamily(name);

            this.fontMap[this.Settings.FontFamily] = fontFamily;

            return fontFamily;
        }
        #endregion
    }
}
