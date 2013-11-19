// <copyright file="FontSizeConverter.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-11</date>
// <summary>Font Size converter</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using System.Globalization;
    using Windows.UI.Xaml.Data;

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
            this.FontSize = 16.0;
        }

        /// <summary>
        /// Gets or sets the font size (default is 16.0)
        /// </summary>
        public double FontSize { get; set; }

        /// <summary>
        /// Converts a font size
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">parameter not used</param>
        /// <param name="language">language is not used</param>
        /// <returns>a font size</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType.Name == "Double")
            {
                if (value == null)
                {
                    return this.FontSize;
                }

                var intValue = (int)value;

                return System.Convert.ToDouble(intValue) * this.FontSize / 100.0d;
            }
            else
            {
                if (value == null)
                {
                    return "Default";
                }

                return value.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }

            if (value.ToString() == "Default")
            {
                return null;
            }

            return int.Parse(value.ToString(), CultureInfo.InvariantCulture);
        }
    }
}
