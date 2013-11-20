// <copyright file="ResourceConverter.cs" company="Michael S. Scherotter">
// Copyright (c) 2013 Michael S. Scherotter All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>synergist@charette.com</email>
// <date>2013-09-26</date>
// <summary>Resource converter</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using System.Collections.Generic;
    using System.Resources;
    using Windows.ApplicationModel.Resources;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Resource Converter
    /// </summary>
    public class ResourceConverter : IValueConverter
    {
        /// <summary>
        /// the resource loader
        /// </summary>
        private ResourceLoader loader;

        /// <summary>
        /// Convert from a caption font to a Font Family
        /// </summary>
        /// <param name="value">a string</param>
        /// <param name="targetType">a FontFamily type</param>
        /// <param name="parameter">not used</param>
        /// <param name="language">language not used</param>
        /// <returns>a <see cref="FontFamily"/></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (this.loader == null)
            {
                this.loader = ResourceLoader.GetForCurrentView("Microsoft.PlayerFramework.CaptionSettings/Resources");
            }

            var text = this.loader.GetString(value.ToString());

            if (string.IsNullOrEmpty(text))
            {
                System.Diagnostics.Debug.WriteLine("No localized resource for {0}", value);

                return value.ToString();
            }

            return text;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value">a FontFamily</param>
        /// <param name="targetType">a string</param>
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
