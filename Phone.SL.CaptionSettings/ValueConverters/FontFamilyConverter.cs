// <copyright file="FontFamilyConverter.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-25</date>
// <summary>Font Family Converter</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Data;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Media = System.Windows.Media;

    /// <summary>
    /// Font family converter
    /// </summary>
    public class FontFamilyConverter : IValueConverter
    {
        #region Fields
        /// <summary>
        /// the font map
        /// </summary>
        private Dictionary<Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily, Media.FontFamily> fontMap;
        #endregion

        /// <summary>
        /// Convert from a Caption Settings font family to a Windows Font Family
        /// </summary>
        /// <param name="value">a Caption Settings <see cref="Model.FontFamily"/></param>
        /// <param name="targetType">a Windows <see cref="Media.FontFamily"/></param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>a <see cref="Media.FontFamily"/></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            var fontFamily = (Model.FontFamily)value;

            if (fontFamily == FontFamily.Default)
            {
                return DependencyProperty.UnsetValue;
            }

            return this.GetFontFamily(fontFamily);
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="value">value not used</param>
        /// <param name="targetType">target type not used</param>
        /// <param name="parameter">parameter not used</param>
        /// <param name="culture">culture not used</param>
        /// <returns>nothing returned</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #region Implementation

        /// <summary>
        /// Gets the font family from the user settings Font
        /// </summary>
        /// <param name="fontFamily">the font family</param>
        /// <returns>the Windows font family</returns>
        private Media.FontFamily GetFontFamily(Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily fontFamily)
        {
            if (this.fontMap == null)
            {
                this.fontMap = new Dictionary<Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily, Media.FontFamily>();

                this.fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.MonospaceSerif] = new Media.FontFamily("Courier New");
                this.fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.ProportionalSerif] = new Media.FontFamily("Cambria");
                this.fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.MonospaceSansSerif] = new Media.FontFamily("Consolas");
                this.fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.ProportionalSansSerif] = new Media.FontFamily("Arial");
                this.fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.Casual] = new Media.FontFamily("Comic Sans MS");
                this.fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.Cursive] = new Media.FontFamily("Segoe Script");
                this.fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.Smallcaps] = new Media.FontFamily("Segoe UI");
            }

            var fontName = this.fontMap[fontFamily];

            return fontName;
        }

        #endregion
    }
}
