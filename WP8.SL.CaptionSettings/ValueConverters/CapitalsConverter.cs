// <copyright file="CapitalsConverter.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-25</date>
// <summary>Capitals Converter</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Documents;

    /// <summary>
    /// Converts <see cref="Model.FontFamily"/> to <see cref="FontCapitals"/>
    /// </summary>
    public class CapitalsConverter : IValueConverter
    {
        /// <summary>
        /// Convert from <see cref="Model.FontFamily"/> to <see cref="FontCapitals"/>
        /// </summary>
        /// <param name="value">a <see cref="Model.FontFamily"/></param>
        /// <param name="targetType">a <see cref="FontCapitals"/> type</param>
        /// <param name="parameter">parameter not used</param>
        /// <param name="culture">culture not used</param>
        /// <returns>if the <see cref="Model.FontFamily"/> is SmallCaps, then return <see cref="FontCapitals.SmallCaps"/></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            Model.FontFamily fontFamily = (Model.FontFamily)value;

            if (fontFamily == Model.FontFamily.Smallcaps)
            {
                return FontCapitals.SmallCaps;
            }

            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Convert back not implemented
        /// </summary>
        /// <param name="value">a FontCapitals</param>
        /// <param name="targetType">a FontFamily</param>
        /// <param name="parameter">parameter not used</param>
        /// <param name="culture">culture not used</param>
        /// <returns>exception thrown</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
