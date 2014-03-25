// <copyright file="ColorConverter.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-25</date>
// <summary>Color Converter</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using Windows.UI;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Converts from a Model.Color to a SolidColorBrush
    /// </summary>
    public class ColorConverter : IValueConverter
    {
        /// <summary>
        /// Convert from a <see cref="Model.Color"/> to a <see cref="SolidColorBrush"/>
        /// </summary>
        /// <param name="value">a <see cref="Model.Color"/></param>
        /// <param name="targetType">a <see cref="Brush"/></param>
        /// <param name="parameter">parameter not used</param>
        /// <param name="culture">culture not used</param>
        /// <returns>a <see cref="SolidColorBrush"/></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                if (parameter != null && parameter.ToString() == "White")
                {
                    return new SolidColorBrush(Colors.White);
                }

                return null;
            }

            var captionColor = value as Model.Color;

            return new SolidColorBrush(captionColor.ToColor());
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value">value not implemented</param>
        /// <param name="targetType">target type not implemented</param>
        /// <param name="parameter">parameter not implemented</param>
        /// <param name="culture">culture not implemented</param>
        /// <returns>exception thrown</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
