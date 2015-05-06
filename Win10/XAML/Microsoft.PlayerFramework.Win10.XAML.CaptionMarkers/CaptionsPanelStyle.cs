using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;

namespace Microsoft.PlayerFramework.CaptionMarkers
{
    /// <summary>
    /// Represents styling settings for a CaptionsPanel object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is used to apply text styling settings defined in the W3C Timed Text Markup Language to a CaptionMediaMarker object.
    /// For more detailed information, see http://www.w3.org/TR/ttaf1-dfxp/#styling
    /// </para>
    /// <para>
    /// Note that members have a <c>[ScriptableMember]</c> attribute allowing them to be accessed from your JavaScript code.
    /// </para>
    /// </remarks>
    public class CaptionsPanelStyle
	{
        private static readonly Color DefaultColor;
        private static readonly Color DefaultBackgroundColor;
        private static readonly FontFamily DefaultFontFamily;
		private static readonly FontCapitals DefaultFontCaps;

        static CaptionsPanelStyle()
        {
            DefaultColor = Colors.White;
            DefaultBackgroundColor = Colors.Transparent;
            DefaultFontFamily = new FontFamily("Arial");
			DefaultFontCaps = FontCapitals.Normal;
        }

        public CaptionsPanelStyle()
        {
            BackgroundColor = DefaultBackgroundColor;
            Color = DefaultColor;
            FontFamily = DefaultFontFamily;
			FontCapitals = DefaultFontCaps;
        }

        /// <summary>
        /// Gets or sets the background color for the caption.
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground text color for the caption.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Gets or sets the font family for the caption.
        /// </summary>
        [XmlIgnore]
        public FontFamily FontFamily { get; set; }

		/// <summary>
		/// Gets or sets the font capitalization for the caption.
		/// </summary>
		public FontCapitals FontCapitals { get; set; }
    }
}