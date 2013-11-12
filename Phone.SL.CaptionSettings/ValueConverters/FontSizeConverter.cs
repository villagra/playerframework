using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    public class FontSizeConverter : IValueConverter
    {
        public FontSizeConverter()
        {
            this.BaseFontSize = 24.0;
        }

        public double BaseFontSize { get; set; }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int? fontSize = value as int?;

            if (fontSize == null || !fontSize.HasValue)
            {
                return this.BaseFontSize;
            }

            var newFontSize = this.BaseFontSize * System.Convert.ToDouble(fontSize.Value) / 100d;

            return newFontSize;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
