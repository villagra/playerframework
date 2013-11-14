namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using System.Windows.Data;
    using System.Windows.Media;

    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            var captionColor = value as Model.Color;

            return new SolidColorBrush(captionColor.ToColor());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
