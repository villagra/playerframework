// <copyright file="ColorExtensions.cs" company="Michael S. Scherotter">
// Copyright (c) 2013 Michael S. Scherotter All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>synergist@charette.com</email>
// <date>2013-09-26</date>
// <summary>Color Extensions</summary>

namespace Microsoft.PlayerFramework.CaptionSettings
{
    using Microsoft.PlayerFramework.CaptionSettings.Model;

#if WINDOWS_PHONE
    using Media = System.Windows.Media;
#else
    using Media = Windows.UI;
#endif

    /// <summary>
    /// Color extension methods
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Converts a CaptionSettings Color to a Windows Color
        /// </summary>
        /// <param name="color">the caption settings color</param>
        /// <returns>a Windows Color</returns>
        public static Media.Color ToColor(this Color color)
        {
            return Media.Color.FromArgb(
                color.Alpha,
                color.Red,
                color.Green,
                color.Blue);
        }

        /// <summary>
        /// Convert a Windows Color to a Caption Settings color
        /// </summary>
        /// <param name="color">a Windows color</param>
        /// <returns>a Caption Settings color</returns>
        public static Color ToCaptionSettingsColor(this Media.Color color)
        {
            return new Color
            {
                Red = color.R,
                Green = color.G,
                Blue = color.B,
                Alpha = color.A
            };
        }

        /// <summary>
        /// Convert a Windows Color to a Caption Settings color
        /// </summary>
        /// <param name="color">a Windows color</param>
        /// <param name="alpha">the alpha (transparency) value</param>
        /// <returns>a Caption Settings color</returns>
        public static Color ToCaptionSettingsColor(this Media.Color color, byte alpha)
        {
            return new Color
            {
                Red = color.R,
                Green = color.G,
                Blue = color.B,
                Alpha = alpha
            };
        }
    }
}
