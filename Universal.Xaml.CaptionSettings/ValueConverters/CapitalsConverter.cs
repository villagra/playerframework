// <copyright file="CapitalsConverter.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-23</date>
// <summary>Capitals Converter</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Capitals converter
    /// </summary>
    public class CapitalsConverter : IValueConverter
    {
        /// <summary>
        /// Convert from a FontFamily to a FontCapitals
        /// </summary>
        /// <param name="value">a FontFamily value</param>
        /// <param name="targetType">a FontCapitals type</param>
        /// <param name="parameter">not used</param>
        /// <param name="language">language not used</param>
        /// <returns><see cref="FontCapitals.SmallCaps"/>if value is <see cref="FontFamily.Smallcaps"/>, <see cref="FontCapitals.Normal"/>otherwise</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            FontFamily fontFamily = (FontFamily)value;

            if (fontFamily == FontFamily.Smallcaps)
            {
                return FontCapitals.SmallCaps;
            }

            return FontCapitals.Normal;
        }

        /// <summary>
        /// Convert back not implemented
        /// </summary>
        /// <param name="value">value not used</param>
        /// <param name="targetType">target type not used</param>
        /// <param name="parameter">parameter not used</param>
        /// <param name="language">language not used</param>
        /// <returns>nothing returned</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
