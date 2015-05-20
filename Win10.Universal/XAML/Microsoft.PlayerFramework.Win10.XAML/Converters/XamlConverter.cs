using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Converter to turn a Xaml string in an object
    /// </summary>
    public sealed class XamlConverter : IValueConverter
    {
        /// <inheritdoc /> 
        public object Convert(object value, Type targetType, object parameter, string language)
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
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
