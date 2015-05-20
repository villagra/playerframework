using System;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Returns the parameter instead of the value if the value is null. Offers a good substitution for the TargetNullValue param on bindings in Silverlight or WPF
    /// </summary>
    public class TargetNullValueConverter : IValueConverter
    {
        /// <inheritdoc /> 
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            return value ?? parameter;
        }

        /// <inheritdoc /> 
        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}
