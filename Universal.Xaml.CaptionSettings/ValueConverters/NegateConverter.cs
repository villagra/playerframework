// <copyright file="NegateConverter.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-24</date>
// <summary>Convert to the inverse of a number</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Negate converter
    /// </summary>
    public class NegateConverter : IValueConverter
    {
        /// <summary>
        /// Convert to the inverse of value (multiply by -1)
        /// </summary>
        /// <param name="value">a double value</param>
        /// <param name="targetType">a double type</param>
        /// <param name="parameter">not used</param>
        /// <param name="language">language not used</param>
        /// <returns>the inverse of value</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double number = (double)value;

            var inverse = number * -1.0d;

            return inverse;
        }

        /// <summary>
        /// Convert back not implemented
        /// </summary>
        /// <param name="value">the value</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">parameter not used</param>
        /// <param name="language">language not used</param>
        /// <returns>not implemented</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
