namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    using System;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Windows.ApplicationModel.Resources;
    using Windows.UI.Xaml.Data;

    public class ColorTypeConverter : IValueConverter
    {
        ResourceLoader loader;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            ColorType colorType = (ColorType)value;

            if (this.loader == null)
            {
                this.loader = ResourceLoader.GetForCurrentView("Microsoft.Win81.PlayerFramework.CaptionSettingsPlugIn.Xaml/Resources");
            }

            var text = this.loader.GetString(colorType.ToString());

            if (string.IsNullOrEmpty(text))
            {
                return value.ToString();
            }

            return text;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
