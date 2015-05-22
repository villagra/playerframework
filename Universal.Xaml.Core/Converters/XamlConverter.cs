using System;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Globalization;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Converter to turn a Xaml string in an object
    /// </summary>
    public sealed class XamlConverter : IValueConverter
    {
        /// <inheritdoc /> 
#if SILVERLIGHT
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#else
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
        {
            var xaml = value as string;
            if (xaml != null)
            {
                return XamlReader.Load(xaml);
            }
            else
            {
                return null;
            }
        }
        
        /// <inheritdoc /> 
#if SILVERLIGHT
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#else
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
        {
            throw new NotImplementedException();
        }
    }
}
