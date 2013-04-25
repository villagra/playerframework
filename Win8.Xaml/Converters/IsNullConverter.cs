using System;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Data;
using System.Globalization;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// IValueConverter used to help Xaml determine if the value is null
    /// </summary>
    public class IsNullConverter : IValueConverter
    {
        /// <inheritdoc /> 
#if SILVERLIGHT
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#else
        public object Convert(object value, Type targetType, object parameter, string culture)
#endif
        {
            return value == null;
        }

        /// <inheritdoc /> 
#if SILVERLIGHT
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#else
        public object ConvertBack(object value, Type targetType, object parameter, string culture)
#endif
        {
            throw new NotImplementedException();
        }
    }
}
