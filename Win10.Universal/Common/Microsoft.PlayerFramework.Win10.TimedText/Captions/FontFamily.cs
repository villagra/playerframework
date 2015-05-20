
namespace Microsoft.Media.TimedText
{
    // Summary:
    //     Represents a family of related fonts.
    public class FontFamily
    {
        // Summary:
        //     Initializes a new instance of the FontFamily class from the specified font
        //     family string.
        //
        // Parameters:
        //   familyName:
        //     The family name of the font to represent. This can include a hashed suffix.
        public FontFamily(string familyName) 
        {
            Source = familyName;
        }

        // Summary:
        //     Gets the font family name that is used to construct the FontFamily object.
        //
        // Returns:
        //     The font family name of the FontFamily object.
        public string Source { get; private set; }

        public Windows.UI.Xaml.Media.FontFamily WindowsFontFamily
        {
            get { return new Windows.UI.Xaml.Media.FontFamily(Source); }
        }
    }
}
