// <copyright file="ColorTypeConverter.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-14</date>
// <summary>ColorType value converter</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Windows.ApplicationModel.Resources;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// ColorType value converter
    /// </summary>
    public class ColorTypeConverter : IValueConverter
    {
        /// <summary>
        /// the resource loader
        /// </summary>
        private ResourceLoader loader;

        /// <summary>
        /// Convert from a ColorType to its string equivalent
        /// </summary>
        /// <param name="value">the ColorType</param>
        /// <param name="targetType">the target type</param>
        /// <param name="parameter">not used</param>
        /// <param name="language">the language</param>
        /// <returns>a string</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ColorType colorType = (ColorType)value;

            if (this.loader == null)
            {
                this.loader = ResourceLoader.GetForCurrentView("Microsoft.PlayerFramework.CaptionSettings/Resources");
            }

            var text = this.loader.GetString(colorType.ToString());

            if (string.IsNullOrEmpty(text))
            {
                return value.ToString();
            }

            return text;
        }

        /// <summary>
        /// Not Implemented
        /// </summary>
        /// <param name="value">a string</param>
        /// <param name="targetType">a ColorType</param>
        /// <param name="parameter">not used</param>
        /// <param name="language">language not used</param>
        /// <returns>exception is thrown, no return</returns>
        /// <exception cref="NotImplementedException">Not implemented</exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
