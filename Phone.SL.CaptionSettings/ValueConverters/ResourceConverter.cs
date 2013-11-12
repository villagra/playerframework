namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using System.Windows.Data;

    public class ResourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            var resourceString = Resources.AppResources.ResourceManager.GetString(value.ToString());

            if (string.IsNullOrWhiteSpace(resourceString))
            {
                System.Diagnostics.Debug.WriteLine("No resource string for {0}", value);

                return value;
            }

            return resourceString;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
