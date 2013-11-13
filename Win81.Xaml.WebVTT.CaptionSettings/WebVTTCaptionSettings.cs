namespace Microsoft.PlayerFramework.Xaml.WebVTT.CaptionSettings
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.PlayerFramework.CaptionSettings;
    using Microsoft.PlayerFramework.WebVTT;

#if WINDOWS_PHONE
    using System.Windows;
    using System.Windows.Documents;
    using Media = System.Windows.Media;
#else
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Documents;
    using Media = Windows.UI.Xaml.Media;
    using Microsoft.WebVTT;
    using Windows.UI.Xaml.Controls;
#endif

    /// <summary>
    /// WebVTT caption settings plug-in for MediaPlayer
    /// </summary>
    public class WebVTTCaptionSettingsPlugin : CaptionSettingsPluginBase
    {
        #region Fields
        private const double DefaultFontSizePercent = 5.0;
        /// <summary>
        /// the font map
        /// </summary>
        private Dictionary<Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily, Media.FontFamily> fontMap;
        #endregion

        #region Methods
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

            plugin.CaptionsPanel.NodeRendering += this.OnNodeRendering;

            return base.OnActivate();
        }

        public override void OnApplyCaptionSettings(PlayerFramework.CaptionSettings.Model.CustomCaptionSettings settings)
        {
            var plugin = this.MediaPlayer.GetWebVTTPlugin();
            var fontSize = plugin.CaptionsPanel.FontSize;
            var fontSizePercentage = plugin.CaptionsPanel.FontSizePercent;
            if (settings.FontSize.HasValue)
            {
                fontSizePercentage = DefaultFontSizePercent * System.Convert.ToDouble(settings.FontSize.Value) / 100.0;

                plugin.CaptionsPanel.FontSizePercent = fontSizePercentage;
            }
            else
            {
                plugin.CaptionsPanel.FontSizePercent = DefaultFontSizePercent;
            }
            
            if (settings.WindowColor != null &&
                (settings.WindowColorType == PlayerFramework.CaptionSettings.Model.ColorType.Solid || settings.WindowColorType == PlayerFramework.CaptionSettings.Model.ColorType.Semitransparent))
            {
                uint opacity = 100;
                if (this.Settings.WindowColorType == PlayerFramework.CaptionSettings.Model.ColorType.Semitransparent)
                {
                    opacity = 50;
                }

                var color = this.Settings.WindowColor.ToColor(opacity);

                plugin.CaptionsPanel.PanelBackground = new Media.SolidColorBrush(color);
            }
            else
            {
                plugin.CaptionsPanel.PanelBackground = null;
            }

            if (settings.BackgroundColor != null && 
                (settings.BackgroundColorType == PlayerFramework.CaptionSettings.Model.ColorType.Solid || settings.BackgroundColorType == PlayerFramework.CaptionSettings.Model.ColorType.Semitransparent))
            {
                uint opacity = 100;
                if (this.Settings.BackgroundColorType == PlayerFramework.CaptionSettings.Model.ColorType.Semitransparent)
                {
                    opacity = 50;
                }

                var color = this.Settings.BackgroundColor.ToColor(opacity);

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
                        panel.NodeRendering -= this.OnNodeRendering;
                    }
                }
            }

            base.OnDeactivate();
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Set the inline style based on the custom settings
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the node rendering event arguments</param>
        private void OnNodeRendering(object sender, Microsoft.WebVTT.NodeRenderingEventArgs e)
        {
            if (this.Settings == null)
            {
                return;
            }

            var plugin = this.MediaPlayer.GetWebVTTPlugin();

            switch (this.Settings.FontFamily)
            {
                case PlayerFramework.CaptionSettings.Model.FontFamily.Smallcaps:
                    Typography.SetCapitals(e.Inline, FontCapitals.SmallCaps);
                    break;

                case PlayerFramework.CaptionSettings.Model.FontFamily.Default:
                    break;

                default:
                    e.Inline.FontFamily = this.GetFont();
                    break;
            }
    
            if (e.TextPosition == TextPosition.Center && this.Settings.FontColor != null && this.Settings.FontColorType == PlayerFramework.CaptionSettings.Model.ColorType.Solid || this.Settings.FontColorType == PlayerFramework.CaptionSettings.Model.ColorType.Semitransparent)
            {
                uint opacity = 100;
                if (this.Settings.FontColorType == PlayerFramework.CaptionSettings.Model.ColorType.Semitransparent)
                {
                    opacity = 50;
                }

                var fontColor = this.Settings.FontColor.ToColor(opacity);

                e.Inline.Foreground = new Media.SolidColorBrush(fontColor);
            }

            switch (this.Settings.FontStyle)
            {
                case PlayerFramework.CaptionSettings.Model.FontStyle.Default:
                    break;

                case PlayerFramework.CaptionSettings.Model.FontStyle.DropShadow:
                    ApplyDropShadow(e);
                    break;

                case PlayerFramework.CaptionSettings.Model.FontStyle.Outline:
                    break;

                case PlayerFramework.CaptionSettings.Model.FontStyle.DepressedEdge:
                    break;

                case PlayerFramework.CaptionSettings.Model.FontStyle.None:
                    if (e.TextPosition != TextPosition.Center)
                    {
                        e.Inline.Foreground = new Media.SolidColorBrush(Colors.Transparent);
                    }
                    break;

                case PlayerFramework.CaptionSettings.Model.FontStyle.RaisedEdge:
                    break;
            }
        }

        private void ApplyDropShadow(NodeRenderingEventArgs e)
        {
            switch (e.TextPosition)
            {
                case TextPosition.Center:
                    break;

                case TextPosition.Bottom:
                case TextPosition.BottomRight:
                case TextPosition.Right:
                    e.Inline.Foreground = new Media.SolidColorBrush(Colors.Black);
                    break;

                default:
                    e.Inline.Foreground = new Media.SolidColorBrush(Colors.Transparent);
                    break;
            }
        }

        private Media.FontFamily GetFont()
        {
            if (this.fontMap == null)
            {
                this.fontMap = new Dictionary<PlayerFramework.CaptionSettings.Model.FontFamily, Media.FontFamily>();

                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.MonospaceSerif] = new Media.FontFamily("Courier New");
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.ProportionalSerif] = new Media.FontFamily("Cambria");
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.MonospaceSansSerif] = new Media.FontFamily("Consolas");
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.ProportionalSansSerif] = new Media.FontFamily("Arial");
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.Casual] = new Media.FontFamily("Comic Sans MS");
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.Cursive] = new Media.FontFamily("Segoe Script");
            }

            return this.fontMap[this.Settings.FontFamily];
        }
        #endregion
    }
}
