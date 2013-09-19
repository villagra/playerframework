using System;
using Windows.UI.Xaml.Data;

namespace Microsoft.PlayerFramework.Xaml.DashDemo
{
    public class BitrateConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var v = System.Convert.ToInt64(value);
            return string.Format("{0} kbps", v / 1024);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
