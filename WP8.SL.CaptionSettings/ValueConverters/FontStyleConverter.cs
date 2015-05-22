// <copyright file="FontStyleConverter.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-12-06</date>
// <summary>Font Style Converter</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Font Style converter
    /// </summary>
    public class FontStyleConverter : IValueConverter
    {
        /// <summary>
        /// Converts FontFamily.Cursive to Italic FontStyle
        /// </summary>
        /// <param name="value">a <see cref="Model.FontFamily"/></param>
        /// <param name="targetType">a <see cref="FontStyle"/></param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>FontStyles.Italic if value is FontFamily.Cursive, FontStyles.Normal otherwise.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return FontStyles.Normal;
            }

            var fontFamily = (Model.FontFamily)value;

            if (fontFamily == Model.FontFamily.Cursive)
            {
                return FontStyles.Italic;
            }

            return FontStyles.Normal;
        }

        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>nothing returned</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
