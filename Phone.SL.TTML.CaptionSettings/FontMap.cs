// <copyright file="FontMap.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-12-04</date>
// <summary>Windows Phone 8 Font Map</summary>

namespace Microsoft.PlayerFramework.TTML.CaptionSettings
{
    using System.Collections.Generic;
    using Microsoft.PlayerFramework.CaptionSettings.Model;

#if WINDOWS_PHONE
    using FF = System.Windows.Media;

#else
    using FF = Microsoft.TimedText;
    using Media = Windows.UI;
#endif

    /// <summary>
    /// Windows Phone 8 Font Map
    /// </summary>
    public class FontMap
    {
        /// <summary>
        /// the font map
        /// </summary>
        private static Dictionary<Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily, FF.FontFamily> fontMap;

        /// <summary>
        /// Gets the Windows Phone 8 font family from the user settings Font Family
        /// </summary>
        /// <param name="userSettings">the user settings</param>
        /// <returns>the font family</returns>
        /// <remarks>See 
        /// <a href="http://msdn.microsoft.com/en-us/library/windowsphone/develop/cc189010(v=vs.105).aspx#silverlight_fonts">Text and fonts for Windows Phone</a>
        /// for more details on the fonts available for Windows Phone 8.
        /// </remarks>
        public static FF.FontFamily GetFontFamily(CustomCaptionSettings userSettings)
        {
            if (fontMap == null)
            {
                fontMap = new Dictionary<Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily, FF.FontFamily>();
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.Default] = null;
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.MonospaceSerif] = new FF.FontFamily("Courier New");
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.ProportionalSerif] = new FF.FontFamily("Cambria");
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.MonospaceSansSerif] = new FF.FontFamily("Consolas");
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.ProportionalSansSerif] = new FF.FontFamily("Segoe UI");
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.Casual] = new FF.FontFamily("Comic Sans MS");

                // Since Windows Phone 8 does not ship with a cursive font, we use this.
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.Cursive] = new FF.FontFamily("Comic Sans MS");

                // _Smallcaps is a unique keyword that will trigger the usage of Typography.SetCapitals(textblock, FontCapitals.SmallCaps)
                // and the default font.
                fontMap[Microsoft.PlayerFramework.CaptionSettings.Model.FontFamily.Smallcaps] = new FF.FontFamily("_Smallcaps");
            }

            var fontName = fontMap[userSettings.FontFamily];

            return fontName;
        }
    }
}
