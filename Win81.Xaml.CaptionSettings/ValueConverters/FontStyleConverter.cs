namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using Windows.UI.Xaml.Data;

    public class FontStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            System.Diagnostics.Debug.WriteLine("Converting from {0} to {1}", value, targetType);
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            System.Diagnostics.Debug.WriteLine("Converting Back {0} to {1}", value, targetType);

            return value;
        }
    }
}
