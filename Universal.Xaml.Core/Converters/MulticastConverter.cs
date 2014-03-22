using System;
using System.Collections.Generic;
using System.Linq;
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
    /// IValueConverter impelmentation that converts a value by passing it through a list of other converters.
    /// The result of converter 1 is passed to converter 2 and so on.
    /// </summary>
    public class MulticastConverter : IValueConverter
    {
        /// <summary>
        /// Creates a new instance of MulticastConverter while initializing the child converters.
        /// </summary>
        /// <param name="converters">The child converters.</param>
        public MulticastConverter(IEnumerable<IValueConverter> converters)
        {
            this.Converters = converters.ToList();
        }

        /// <summary>
        /// Creates a new instance of MulticastConverter.
        /// </summary>
        public MulticastConverter()
        {
            Converters = new List<IValueConverter>();
        }

        /// <summary>
        /// A list of child converters used to convert the value.
        /// </summary>
        public List<IValueConverter> Converters { get; private set; }

        /// <inheritdoc /> 
#if SILVERLIGHT
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#else
        public object Convert(object value, Type targetType, object parameter, string culture)
#endif
        {
            object runningValue = value;
            foreach (var converter in Converters)
            {
                runningValue = converter.Convert(runningValue, targetType, parameter, culture);
            }
            return runningValue;
        }

        /// <inheritdoc /> 
#if SILVERLIGHT
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#else
        public object ConvertBack(object value, Type targetType, object parameter, string culture)
#endif
        {
            object runningValue = value;
            foreach (var converter in Converters.ToArray().Reverse())
            {
                runningValue = converter.ConvertBack(runningValue, targetType, parameter, culture);
            }
            return runningValue;
        }
    }
}
