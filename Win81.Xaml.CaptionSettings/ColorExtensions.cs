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

    /// <summary>
    /// Color extension methods
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Converts a CaptionSettings Color to a Windows Color
        /// </summary>
        /// <param name="color">the caption settings color</param>
        /// <param name="opacity">the opacity (0-100)</param>
        /// <returns>a Windows Color</returns>
        public static Windows.UI.Color ToColor(this Color color, uint opacity)
        {
            var textOpacity = opacity * 255 / 100;

            return Windows.UI.Color.FromArgb(
                System.Convert.ToByte(textOpacity),
                color.Red,
                color.Green,
                color.Blue);
        }

        /// <summary>
        /// Convert a Windows Color to a Caption Settings color
        /// </summary>
        /// <param name="color">a Windows color</param>
        /// <returns>a Caption Settings color</returns>
        public static Color ToCaptionSettingsColor(this Windows.UI.Color color)
        {
            return new Color
            {
                Red = color.R,
                Green = color.G,
                Blue = color.B
            };
        }
    }
}
