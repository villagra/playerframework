using System;
using System.Globalization;
#if SILVERLIGHT
using System.Windows.Media;
#else
using Windows.UI;
#endif

namespace TimedText.Styling
{
    public sealed class ColorExpression
    {
        private ColorExpression() { }

        /*
        <color>
          : "#" rrggbb
          | "#" rrggbbaa
          | "rgb" "(" r-value "," g-value "," b-value ")"
          | "rgba" "(" r-value "," g-value "," b-value "," a-value ")"
          | <namedColor>
        rrggbb
          :  <hexDigit>{6}
        rrggbbaa
          :  <hexDigit>{8}
        r-value | g-value | b-value | a-value
          : component-value
        component-value
          : non-negative-integer                    // valid range: [0,255]
        non-negative-integer
          : <digit>+
        */

        /// <summary>
        /// Create a Color object from a timed text colour expression
        /// </summary>
        /// <param name="colorExpression">colour expression</param>
        /// <returns>color</returns>
        public static Color Parse(string colorExpression)
        {
            Color result;
            if (TryParse(colorExpression, out result))
            {
                return result;
            }
            else
            {
                throw new TimedTextException("Invalid colour format string");
            }
        }


        /// <summary>
        /// Create a Color object from a timed text colour expression
        /// </summary>
        /// <param name="colorExpression">colour expression</param>
        /// <returns>color</returns>
        public static bool TryParse(string colorExpression, out Color rgb)
        {
            string input = colorExpression.Trim();
            rgb = Color.FromArgb(0xff, 0, 0, 0);

            char[] separators = { '(', ',', ')' };
            try
            {
                if (input.Contains("#"))
                {
                    rgb.R = Byte.Parse(input.Substring(1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    rgb.G = Byte.Parse(input.Substring(3, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    rgb.B = Byte.Parse(input.Substring(5, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    if (input.Length > 7)
                    {
                        rgb.A = Byte.Parse(input.Substring(7, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    }
                }
                else if (input.Contains("rgb("))
                {
                    string[] parts = input.Split(separators);
                    // should be 5 parts, the first part is prefix, last is null.
                    NumberStyles digitOnly = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;
                    rgb.R = Byte.Parse(parts[1], digitOnly, CultureInfo.InvariantCulture);
                    rgb.G = Byte.Parse(parts[2], digitOnly, CultureInfo.InvariantCulture);
                    rgb.B = Byte.Parse(parts[3], digitOnly, CultureInfo.InvariantCulture);
                }
                else if (input.Contains("rgba("))
                {
                    string[] parts = input.Split(separators);
                    // should be 5 parts, the first part is prefix, last is null..
                    NumberStyles digitOnly = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite;
                    rgb.R = Byte.Parse(parts[1], digitOnly, CultureInfo.InvariantCulture);
                    rgb.G = Byte.Parse(parts[2], digitOnly, CultureInfo.InvariantCulture);
                    rgb.B = Byte.Parse(parts[3], digitOnly, CultureInfo.InvariantCulture);
                    rgb.A = Byte.Parse(parts[4], digitOnly, CultureInfo.InvariantCulture);
                }
                else
                {
                    return TryParseNamedColor(input.ToLower(), out rgb);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// return a colour from one of the allowed timed text names.
        /// </summary>
        /// <param name="input">name of color</param>
        /// <returns>color</returns>
        private static Color ParseNamedColor(string input)
        {
            Color result;
            if (TryParseNamedColor(input, out result))
            {
                return result;
            }
            else
            {
                throw new TimedTextException("named colour " + input + " not allowed");
            }
        }

        /// <summary>
        /// return a colour from one of the allowed timed text names.
        /// </summary>
        /// <param name="input">name of color</param>
        /// <returns>color</returns>
        private static bool TryParseNamedColor(string input, out Color result)
        {
            switch (input)
            {
                case "transparent":
                    result = Color.FromArgb(0x00, 0x00, 0x00, 0x00);
                    break;
                case "black":
                    result = Color.FromArgb(0xff, 0x00, 0x00, 0x00);
                    break;
                case "silver":
                    result = Color.FromArgb(0xff, 0xc0, 0xc0, 0xc0);
                    break;
                case "gray":
                    result = Color.FromArgb(0xff, 0x80, 0x80, 0x80);
                    break;
                case "white":
                    result = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
                    break;
                case "maroon":
                    result = Color.FromArgb(0xff, 0x80, 0x00, 0x00);
                    break;
                case "red":
                    result = Color.FromArgb(0xff, 0xff, 0x00, 0x00);
                    break;
                case "purple":
                    result = Color.FromArgb(0xff, 0x80, 0x00, 0x80);
                    break;
                case "fuchsia":
                    result = Color.FromArgb(0xff, 0xff, 0x00, 0xff);
                    break;
                case "magenta":
                    result = Color.FromArgb(0xff, 0xff, 0x00, 0xff);
                    break;
                case "green":
                    result = Color.FromArgb(0xff, 0x00, 0x80, 0x00);
                    break;
                case "lime":
                    result = Color.FromArgb(0xff, 0x00, 0xff, 0x00);
                    break;
                case "olive":
                    result = Color.FromArgb(0xff, 0x80, 0x80, 0x00);
                    break;
                case "yellow":
                    result = Color.FromArgb(0xff, 0xff, 0xff, 0x00);
                    break;
                case "navy":
                    result = Color.FromArgb(0xff, 0x00, 0x00, 0x80);
                    break;
                case "blue":
                    result = Color.FromArgb(0xff, 0x00, 0x00, 0xff);
                    break;
                case "teal":
                    result = Color.FromArgb(0xff, 0x00, 0x80, 0x80);
                    break;
                case "aqua":
                    result = Color.FromArgb(0xff, 0xff, 0x00, 0xff);
                    break;
                case "cyan":
                    result = Color.FromArgb(0xff, 0x00, 0xff, 0xff);
                    break;
                default:
                    result = new Color();
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Test the colour parser. Not comprehensive at this point
        /// </summary>
        /// <returns></returns>
        public static bool UnitTests()
        {
            Color reference = Color.FromArgb(0xff, 0xff, 0, 0);
            bool pass = true;

            // some basic tests, try to come up with some more devilish ones.
            pass &= Parse("red") == reference;
            pass &= Parse("rgb(255,00,00)") == reference;
            pass &= Parse("rgb(255,00,00,255)") == reference;
            pass &= Parse("#ff0000") == reference;
            pass &= Parse("#FF0000") == reference;
            pass &= Parse("#ff0000ff") == reference;
            pass &= Parse("#fF0000fF") == reference;

            return pass;
        }

    }
}
