// <copyright file="ColorToBrushConverter.cs" company="Michael S. Scherotter">
// Copyright (c) 2013 Michael S. Scherotter All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>synergist@charette.com</email>
// <date>2013-09-26</date>
// <summary>Color to brush converter</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Color to Brush converter
    /// </summary>
    public class ColorToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="CaptionSettings.Portable.Color"/> to a <see cref="SolidColorBrush"/>
        /// </summary>
        /// <param name="value">a color</param>
        /// <param name="targetType">a Brush type</param>
        /// <param name="parameter">not used</param>
        /// <param name="language">language not used</param>
        /// <returns>a SolidColorBrush</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }

            var color = value as Color;

            if (color == null)
            {
                return null;
            }

            var xamlColor = color.ToColor();

            return new SolidColorBrush(xamlColor);
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value">a brush</param>
        /// <param name="targetType">a color</param>
        /// <param name="parameter">not used</param>
        /// <param name="language">language not used</param>
        /// <returns>exception thrown</returns>
        /// <exception cref="NotImplementedException">Not implemented</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
