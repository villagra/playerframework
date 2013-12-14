// <copyright file="FontFamilyConverter.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-11</date>
// <summary>Font Family Converter</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using System.Collections.Generic;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;
    using FF = Windows.UI.Xaml.Media;

    /// <summary>
    /// Font Family Converter
    /// </summary>
    public class FontFamilyConverter : IValueConverter
    {
        /// <summary>
        /// the font map
        /// </summary>
        private Dictionary<Model.FontFamily, FF.FontFamily> fontMap;

        /// <summary>
        /// convert from a Caption Settings font family to the equivalent 
        /// Windows installed font
        /// </summary>
        /// <param name="value">a Model.FontFamily</param>
        /// <param name="targetType">a Windows FontFamily</param>
        /// <param name="parameter">parameter not used</param>
        /// <param name="language">language not used</param>
        /// <returns>a <see cref="Windows.UI.Xaml.Media.FontFamily"/></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (this.fontMap == null)
            {
                this.fontMap = new Dictionary<Model.FontFamily, FF.FontFamily>();
            }

            var captionFontFamily = (Model.FontFamily)value;

            if (captionFontFamily == Model.FontFamily.Default)
            {
                return DependencyProperty.UnsetValue;
            }

            FF.FontFamily fontFamily;

            lock (this.fontMap)
            {
                if (this.fontMap.TryGetValue(captionFontFamily, out fontFamily))
                {
                    return fontFamily;
                }

                var familyName = CaptionSettingsPluginBase.GetFontFamilyName(captionFontFamily);

                if (string.IsNullOrWhiteSpace(familyName))
                {
                    return DependencyProperty.UnsetValue;
                }

                fontFamily = new FF.FontFamily(familyName);
                this.fontMap[captionFontFamily] = fontFamily;

                return fontFamily;
            }
        }

        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="value">value not used</param>
        /// <param name="targetType">target type not used</param>
        /// <param name="parameter">parameter not used</param>
        /// <param name="language">language not used</param>
        /// <returns>exception thrown</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
