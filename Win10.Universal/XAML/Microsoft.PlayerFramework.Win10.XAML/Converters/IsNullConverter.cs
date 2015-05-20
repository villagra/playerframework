using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// IValueConverter used to help Xaml determine if the value is null
    /// </summary>
    public class IsNullConverter : IValueConverter
    {
        /// <inheritdoc /> 
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            return value == null;
        }

        /// <inheritdoc /> 
        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}
