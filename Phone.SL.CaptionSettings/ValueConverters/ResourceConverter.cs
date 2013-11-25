// <copyright file="ResourceConverter.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-25</date>
// <summary>Resource Converter</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using System.Windows.Data;

    /// <summary>
    /// Resource converter
    /// </summary>
    public class ResourceConverter : IValueConverter
    {
        /// <summary>
        /// Convert from a string resource to the localized string 
        /// </summary>
        /// <param name="value">a string identifier</param>
        /// <param name="targetType">a string</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>a localized string</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            var resourceString = Resources.AppResources.ResourceManager.GetString(value.ToString());

            if (string.IsNullOrWhiteSpace(resourceString))
            {
                System.Diagnostics.Debug.WriteLine("No resource string for {0}", value);

                return value;
            }

            return resourceString;
        }

        /// <summary>
        /// Convert back not implemented
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>exception thrown</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
