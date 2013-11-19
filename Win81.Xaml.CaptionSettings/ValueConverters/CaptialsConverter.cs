namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;

    public class CaptialsConverter : IValueConverter
    {
        /// <summary>
        /// Convert from a FontFamily to a FontCapticals
        /// </summary>
        /// <param name="value">a FontFamily value</param>
        /// <param name="targetType">a FontCapitals type</param>
        /// <param name="parameter">not used</param>
        /// <param name="language">language not used</param>
        /// <returns>FontCapitals.SmallCaps if value is FontFamily.Smallcaps, FontCapitals.Normal otherwise</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            FontFamily fontFamily = (FontFamily)value;

            if (fontFamily == FontFamily.Smallcaps)
            {
                return FontCapitals.SmallCaps;
            }

            return FontCapitals.Normal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
