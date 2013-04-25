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
    /// IValueConverter used to help Xaml bind a boolean to a Visibility property.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Instantiates a new instance of the BoolToVisibilityConverter class.
        /// </summary>
        public BoolToVisibilityConverter() { }

        /// <summary>
        /// Instantiates a new instance of the BoolToVisibilityConverter class and allows you to pass in the inverse value.
        /// </summary>
        /// <param name="inverse">A flag indicating whether or not the value should be flipped so false == Visible</param>
        public BoolToVisibilityConverter(bool inverse)
        {
            this.Inverse = inverse;
        }

        /// <summary>
        /// A flag indicating whether or not the value should be flipped so false == Visible. Default false (true = Visible).
        /// </summary>
        public bool Inverse { get; set; }

        /// <summary>
        /// Converts a boolean to a Visibility enum
        /// </summary>
        /// <param name="value">the boolean to convert</param>
        /// <param name="inverse">A flag indicating whether or not the value should be flipped so false == Visible.</param>
        /// <returns>Visibility enum</returns>
        public static Visibility Convert(bool value, bool inverse)
        {
            if (inverse)
                return value ? Visibility.Collapsed : Visibility.Visible;
            else
                return value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a Visibility enum to a boolean
        /// </summary>
        /// <param name="value">The Visibility enum to convert</param>
        /// <param name="inverse">A flag indicating whether or not the value should be flipped so false == Visible.</param>
        /// <returns>A boolean value</returns>
        public static bool ConvertBack(Visibility value, bool inverse)
        {
            if (inverse)
                return value == Visibility.Collapsed;
            else
                return value == Visibility.Visible;
        }

        /// <inheritdoc /> 
#if SILVERLIGHT
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#else
        public object Convert(object value, Type targetType, object parameter, string culture)
#endif
        {
            bool bValue;
            if (value is bool)
            {
                bValue = (bool)value;
            }
            else
            {
                bValue = value != null;
            }
            return Convert(bValue, Inverse);
        }

        /// <inheritdoc /> 
#if SILVERLIGHT
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#else
        public object ConvertBack(object value, Type targetType, object parameter, string culture)
#endif
        {
            return ConvertBack((Visibility)value, Inverse);
        }
    }
}