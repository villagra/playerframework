using System;
using System.Globalization;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// IValueConverter used to help Xaml format an object before binding it to a string.
    /// </summary>
    public class StringFormatConverter : DependencyObject, IValueConverter
    {
        #region StringFormat
        /// <summary>
        /// StringFormat DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty StringFormatProperty = DependencyProperty.Register("StringFormat", typeof(string), typeof(StringFormatConverter), null);

        /// <summary>
        /// Gets or sets the string format to use.
        /// </summary>
        public string StringFormat
        {
            get { return GetValue(StringFormatProperty) as string; }
            set { SetValue(StringFormatProperty, value); }
        }

        #endregion

        /// <inheritdoc /> 
#if SILVERLIGHT
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#else
        public object Convert(object value, Type targetType, object parameter, string culture)
#endif
        {
            if (value == null) throw new ArgumentNullException("value");

            string stringFormat = parameter as string ?? StringFormat;
            if (value is string)
            {
                return string.Format(CultureInfo.CurrentCulture, stringFormat, (string)value);
            }
            else if (value is TimeSpan)
            {
                return ((TimeSpan)value).ToString(stringFormat, CultureInfo.CurrentCulture);
            }
            else if (value is DateTime)
            {
                return ((DateTime)value).ToString(stringFormat, CultureInfo.CurrentCulture);
            }
            else if (value is DateTimeOffset)
            {
                return ((DateTimeOffset)value).ToString(stringFormat, CultureInfo.CurrentCulture);
            }
            else if (value is int)
            {
                return ((int)value).ToString(stringFormat, CultureInfo.CurrentCulture);
            }
            else if (value is long)
            {
                return ((double)value).ToString(stringFormat, CultureInfo.CurrentCulture);
            }
            else if (value is double)
            {
                return ((double)value).ToString(stringFormat, CultureInfo.CurrentCulture);
            }
            else if (value is float)
            {
                return ((double)value).ToString(stringFormat, CultureInfo.CurrentCulture);
            }
            else
            {
                return string.Format(CultureInfo.CurrentCulture, stringFormat, value.ToString());
            }
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
