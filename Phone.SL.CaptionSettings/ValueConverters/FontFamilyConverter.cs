using Microsoft.PlayerFramework.CaptionSettings.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Media = System.Windows.Media;

namespace Microsoft.PlayerFramework.CaptionSettings.ValueConverters
{
    public class FontFamilyConverter : IValueConverter
    {
        #region Fields
        /// <summary>
        /// the font map
        /// </summary>
        private Dictionary<Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily, Media.FontFamily> fontMap;
        #endregion

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            var fontFamily = (Model.FontFamily) value;

            if (fontFamily == FontFamily.Default)
            {
                return DependencyProperty.UnsetValue;
            }

            return GetFontFamily(fontFamily);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #region Implementation

        /// <summary>
        /// Gets the font family from the user settings Font
        /// </summary>
        /// <param name="settings">the custom caption settings</param>
        /// <returns>the font family</returns>
        private Media.FontFamily GetFontFamily(Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily fontFamily)
        {
            if (this.fontMap == null)
            {
                this.fontMap = new Dictionary<Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily, Media.FontFamily>();

                this.fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.MonospaceSerif] = new Media.FontFamily("Courier New");
                this.fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.ProportionalSerif] = new Media.FontFamily("Cambria");
                this.fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.MonospaceSansSerif] = new Media.FontFamily("Consolas");
                this.fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.ProportionalSansSerif] = new Media.FontFamily("Arial");
                this.fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.Casual] = new Media.FontFamily("Comic Sans MS");
                this.fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.Cursive] = new Media.FontFamily("Segoe Script");
                this.fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.Smallcaps] = new Media.FontFamily("Segoe UI");
            }

            var fontName = this.fontMap[fontFamily];

            return fontName;
        }

        #endregion
    }
}
