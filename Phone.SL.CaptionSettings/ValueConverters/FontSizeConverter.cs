// <copyright file="FontSizeConverter.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-25</date>
// <summary>Font Size Converter</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// Font Size Converter
    /// </summary>
    public class FontSizeConverter : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of the FontSizeConverter class.
        /// </summary>
        public FontSizeConverter()
        {
            this.BaseFontSize = 24.0;
        }

        /// <summary>
        /// Gets or sets the base font size
        /// </summary>
        public double BaseFontSize { get; set; }

        /// <summary>
        /// Convert from a Font Size Percentage to a size in points
        /// </summary>
        /// <param name="value">a font size as an <see cref="int?"/></param>
        /// <param name="targetType">a font size as a double</param>
        /// <param name="parameter">parameter not used</param>
        /// <param name="culture">culture not used</param>
        /// <returns>the font size as a double</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int? fontSize = value as int?;

            if (fontSize == null || !fontSize.HasValue)
            {
                return this.BaseFontSize;
            }

            var newFontSize = this.BaseFontSize * System.Convert.ToDouble(fontSize.Value) / 100d;

            return newFontSize;
        }

        /// <summary>
        /// Convert back not used
        /// </summary>
        /// <param name="value">value not used</param>
        /// <param name="targetType">target type not used</param>
        /// <param name="parameter">parameter not used</param>
        /// <param name="culture">culture not used</param>
        /// <returns>exception thrown</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
