
namespace Windows.UI.Text
{
    public static class FontStyles
    {
        public const FontStyle Normal = FontStyle.Normal;
        public const FontStyle Oblique = FontStyle.Oblique;
        public const FontStyle Italic = FontStyle.Italic;
    }

    public static class FontStretches
    {
        public const FontStretch Undefined = FontStretch.Undefined;
        public const FontStretch UltraCondensed = FontStretch.UltraCondensed;
        public const FontStretch ExtraCondensed = FontStretch.ExtraCondensed;
        public const FontStretch Condensed = FontStretch.Condensed;
        public const FontStretch SemiCondensed = FontStretch.SemiCondensed;
        public const FontStretch Normal = FontStretch.Normal;
        public const FontStretch SemiExpanded = FontStretch.SemiExpanded;
        public const FontStretch Expanded = FontStretch.Expanded;
        public const FontStretch ExtraExpanded = FontStretch.ExtraExpanded;
        public const FontStretch UltraExpanded = FontStretch.UltraExpanded;
    }

}
namespace System.Net
{
    public static class HttpUtility
    {
        public static string HtmlDecode(string html)
        {
            return WebUtility.HtmlDecode(html);
        }
    }
}
