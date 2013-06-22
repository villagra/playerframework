using System;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace Microsoft.PlayerFramework.Xaml.DashDemo
{
    public class ResolutionConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var s = (Size)value;
            return string.Format("{0}x{1}", s.Width, s.Height);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
