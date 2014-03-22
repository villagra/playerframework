using System;
using System.ComponentModel;
using System.Xml.Serialization;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Text;
#endif

namespace Microsoft.Media.TimedText
{
    /// <summary>
    /// Represents styling settings for a CaptionMediaMarker object.
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
    public class TimedTextStyle
    {
        private const DisplayAlign DefaultDisplayAlign = DisplayAlign.Before;
        private const double DefaultOpacity = 1;
        private const Overflow DefaultOverflow = Overflow.Hidden;
        private const ShowBackground DefaultShowBackground = ShowBackground.Always;
        private const TextAlignment DefaultTextAlignment = TextAlignment.Center;
        private const FlowDirection DefaultDirection = FlowDirection.LeftToRight;
        private const Visibility DefaultVisibility = Visibility.Visible;
        private const TextWrapping DefaultWrapOption = TextWrapping.Wrap;

        /// <summary>
        /// Default text style
        /// </summary>
        private const TextStyle DefaultTextStyle = TextStyle.Default;

        private static readonly Color DefaultColor;
        private static readonly Color DefaultBackgroundColor;
        private static readonly Extent DefaultExtent;
        private static readonly FontFamily DefaultFontFamily;
        private static readonly FontStyle DefaultFontStyle;
        private static readonly Weight DefaultFontWeight;
        private static readonly Origin DefaultOrigin;
        private static readonly Padding DefaultPadding;
        private static readonly Color DefaultOutlineColor;

        static TimedTextStyle()
        {
            DefaultColor = Colors.White;
            DefaultBackgroundColor = Colors.Transparent;
            DefaultExtent = Extent.Auto;
            DefaultFontFamily = new FontFamily("Portable User Interface");
            DefaultFontStyle = FontStyles.Normal;
            DefaultFontWeight = Weight.Normal;
            DefaultOrigin = Origin.Empty;
            DefaultPadding = Padding.Empty;
            DefaultOutlineColor = Colors.Black;
        }

        public TimedTextStyle()
        {
            BackgroundColor = DefaultBackgroundColor;
            Color = DefaultColor;
            DisplayAlign = DefaultDisplayAlign;
            Extent = DefaultExtent;
            FontFamily = DefaultFontFamily;
            FontSize = new Length
            {
                Unit = LengthUnit.Cell,
                Value = 1
            };
            FontStyle = DefaultFontStyle;
            FontWeight = DefaultFontWeight;
            Id = Guid.NewGuid().ToString();
            LineHeight = null;
            Opacity = DefaultOpacity;
            Origin = DefaultOrigin;
            Overflow = DefaultOverflow;
            Padding = DefaultPadding;
            ShowBackground = DefaultShowBackground;
            TextAlign = DefaultTextAlignment;
            Direction = DefaultDirection;
            Visibility = DefaultVisibility;
            WrapOption = DefaultWrapOption;
            OutlineColor = DefaultOutlineColor;
            OutlineWidth = new Length { Value = 0.0, Unit = LengthUnit.Pixel };
            OutlineBlur = new Length { Value = 0.0, Unit = LengthUnit.Pixel };
            this.TextStyle = DefaultTextStyle;
        }

        /// <summary>
        /// Gets or sets an identifier.
        /// </summary>
        /// <remarks>
        /// The Id is used when polling occurs to determine which of the caption markers are new.
        /// </remarks>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the background image for the caption.
        /// </summary>
        public string BackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets the background image vertical positioning.
        /// </summary>
        public PositionLength BackgroundImageVertical { get; set; }

        /// <summary>
        /// Gets or sets the background image vertical positioning.
        /// </summary>
        public PositionLength BackgroundImageHorizontal { get; set; }

        /// <summary>
        /// Gets or sets the background color for the caption.
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the foreground text color for the caption.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Set the outline color for the font
        /// </summary>
        public Color OutlineColor { get; set; }

        /// <summary>
        /// Sets the thickness of the font outline (stroke)
        /// </summary>
        public Length OutlineWidth { get; set; }

        /// <summary>
        /// Sets the thickess of a shadow around the font
        /// </summary>
        public Length OutlineBlur { get; set; }

        /// <summary>
        /// Gets or sets the font family for the caption.
        /// </summary>
        [XmlIgnore]
        public FontFamily FontFamily { get; set; }

        /// <summary>
        /// Text representation of FontFamily.  Really only here to support XML serialization.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string FontFamilyText
        {
            get
            {
                return FontFamily != null
                           ? FontFamily.Source
                           : string.Empty;
            }

            set
            {
                FontFamily = !string.IsNullOrEmpty(value)
                                 ? new FontFamily(value)
                                 : null;
            }
        }

        /// <summary>
        /// Gets or sets the text size for the caption.
        /// </summary>
        /// <remarks>
        /// The only unit of measurement supported is pixels (px).
        /// </remarks>
        public Length FontSize { get; set; }

        /// <summary>
        /// Gets or sets the text style for the caption.
        /// </summary>
        /// <remarks>
        /// Only Normal and Italic styles are supported.
        /// </remarks>
        public FontStyle FontStyle { get; set; }

        /// <summary>
        /// Gets or sets the font weight for the caption.
        /// </summary>
        public Weight FontWeight { get; set; }

        /// <summary>
        /// Gets or sets the line height for the caption.
        /// </summary>
        /// <remarks>
        /// The only unit of measurement supported is pixels (px).
        /// </remarks>
        public Length LineHeight { get; set; }

        /// <summary>
        /// Gets or sets the percent that the caption is transparent (as a value between 0 and 1.0).
        /// </summary>
        public double Opacity { get; set; }

        /// <summary>
        /// Gets or sets the margin for the caption.
        /// </summary>
        public Origin Origin { get; set; }

        /// <summary>
        /// Gets or sets if the origin is specified (vs. inherited)
        /// </summary>
        public bool IsOriginSpecified { get; set; }

        /// <summary>
        /// Gets or sets the amount of padding used for the caption.
        /// </summary>
        public Padding Padding { get; set; }

        /// <summary>
        /// Gets or sets the text alignment setting for the caption.
        /// </summary>
        /// <remarks>
        /// The only horizontal text alignments supported are left, center, and right.
        /// </remarks>
        public TextAlignment TextAlign { get; set; }

        /// <summary>
        /// Gets or sets the visibility of the caption.
        /// </summary>
        public Visibility Visibility { get; set; }

        /// <summary>
        /// Gets or sets the display of the caption, this differs from visibility because visibility still takes space.
        /// </summary>
        public Visibility Display { get; set; }

        /// <summary>
        /// Gets or sets the direction (left to right or right to left) of the caption.
        /// </summary>
        public FlowDirection Direction { get; set; }

        /// <summary>
        /// Gets or sets the text wrapping setting for the caption.
        /// </summary>
        public TextWrapping WrapOption { get; set; }

        /// <summary>
        /// Gets or sets the display alignment for the caption.
        /// </summary>
        public DisplayAlign DisplayAlign { get; set; }

        /// <summary>
        /// Gets or sets when the background should be displayed for the caption.
        /// </summary>
        public ShowBackground ShowBackground { get; set; }

        /// <summary>
        /// Gets or sets the overflow for the caption.
        /// </summary>
        public Overflow Overflow { get; set; }

        /// <summary>
        /// Gets or sets the extent for the caption.
        /// </summary>
        public Extent Extent { get; set; }

        /// <summary>
        /// Gets or sets if the extent is specified (vs. inherited)
        /// </summary>
        public bool IsExtentSpecified { get; set; }

        /// <summary>
        /// Gets or sets the zindex for the caption.
        /// </summary>
        public int ZIndex { get; set; }

        /// <summary>
        /// Gets or sets the text style
        /// </summary>
        public TextStyle TextStyle { get; set; }

        /// <summary>
        /// Creates a memberwise close of this style.
        /// </summary>
        /// <returns></returns>
        public TimedTextStyle Clone()
        {
            return MemberwiseClone() as TimedTextStyle;
        }
    }
}