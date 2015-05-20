using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// IValueConverter used to help Xaml flip the value when binding a boolean to a boolean.
    /// </summary>
    public class NotConverter : IValueConverter
    {
        /// <inheritdoc /> 
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            return !((bool)value);
        }

        /// <inheritdoc /> 
        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return !((bool)value);
        }
    }
}
