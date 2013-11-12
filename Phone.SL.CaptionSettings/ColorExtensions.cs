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

    public static class ColorExtensions
    {
        /// <summary>
        /// Converts a CaptionSettings Color to a Windows Color
        /// </summary>
        /// <param name="color">the caption settings color</param>
        /// <param name="opacity">the opacity (0-100)</param>
        /// <returns>a Windows Color</returns>
        public static System.Windows.Media.Color ToColor(this Color color, uint opacity)
        {
            if (color == null)
            {
                return System.Windows.Media.Colors.Transparent;
            }

            var textOpacity = opacity * 255 / 100;

            return System.Windows.Media.Color.FromArgb(
                System.Convert.ToByte(textOpacity),
                color.Red,
                color.Green,
                color.Blue);
        }

        public static Color ToCaptionSettingsColor(this System.Windows.Media.Color color)
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
