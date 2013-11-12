// <copyright file="CustomCaptionSettings.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-11</date>
// <summary>Custom Caption Settings</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.Model
{
    using System.Globalization;
    using System.IO;
    using System.Xml.Serialization;
    using Microsoft.PlayerFramework.CaptionSettings;
    using Microsoft.PlayerFramework.CaptionSettings.Model;

    /// <summary>
    /// Custom Caption Settings
    /// </summary>
    [XmlRoot]
    public class CustomCaptionSettings : BindableBase
    {
        #region Fields
        /// <summary>
        /// the font family
        /// </summary>
        private FontFamily fontFamily;

        /// <summary>
        /// the font size
        /// </summary>
        private int? fontSize;

        /// <summary>
        /// the font style
        /// </summary>
        private FontStyle fontStyle;

        /// <summary>
        /// the font color type
        /// </summary>
        private ColorType fontColorType;
        
        /// <summary>
        /// the background color type
        /// </summary>
        private ColorType backgroundColorType;
        
        /// <summary>
        /// the window color type
        /// </summary>
        private ColorType windowColorType;

        /// <summary>
        /// the font color
        /// </summary>
        private Color fontColor;

        /// <summary>
        /// the background color
        /// </summary>
        private Color backgroundColor;

        /// <summary>
        /// the window color
        /// </summary>
        private Color windowColor;
        #endregion

        #region Constructors
        public CustomCaptionSettings()
        {
            this.FontColorType = ColorType.Default;
            this.BackgroundColorType = ColorType.Default;
            this.WindowColorType = ColorType.Default;
            this.FontStyle = Model.FontStyle.Default;
            this.FontFamily = Model.FontFamily.Default;
            this.FontColor = new Color { Blue = 255, Green = 255, Red = 255 };
        }
        #endregion

        #region Properties
        [XmlElement]
        public FontFamily FontFamily
        {
            get
            {
                return this.fontFamily;
            }

            set
            {
                this.SetProperty(ref this.fontFamily, value);
            }
        }

        /// <summary>
        /// Gets or sets the font size in %
        /// </summary>
        [XmlElement]
        public int? FontSize
        {
            get
            {
                return this.fontSize;
            }

            set
            {
                this.SetProperty(ref this.fontSize, value);
            }
        }

        /// <summary>
        /// Gets or sets the font style
        /// </summary>
        [XmlElement]
        public FontStyle FontStyle
        {
            get
            {
                return this.fontStyle;
            }

            set
            {
                System.Diagnostics.Debug.WriteLine("FontStyle Setter: {0}", value);

                this.SetProperty(ref this.fontStyle, value);
            }
        }

        [XmlElement]
        public ColorType FontColorType
        {
            get
            {
                return this.fontColorType;
            }

            set
            {
                this.SetProperty(ref this.fontColorType, value);
            }
        }

        [XmlElement]
        public ColorType BackgroundColorType
        {
            get
            {
                return this.backgroundColorType;
            }

            set
            {
                this.SetProperty(ref this.backgroundColorType, value);
            }
        }

        [XmlElement]
        public ColorType WindowColorType
        {
            get
            {
                return this.windowColorType;
            }

            set
            {
                this.SetProperty(ref this.windowColorType, value);
            }
        }

        [XmlElement]
        public Color FontColor
        {
            get
            {
                return this.fontColor;
            }

            set
            {
                this.SetProperty(ref this.fontColor, value);
            }
        }

        [XmlElement]
        public Color BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }

            set
            {
                this.SetProperty(ref this.backgroundColor, value);
            }
        }

        [XmlElement]
        public Color WindowColor
        {
            get
            {
                return this.windowColor;
            }

            set
            {
                this.SetProperty(ref this.windowColor, value);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the XML string for the user settings
        /// </summary>
        /// <returns>an XML string</returns>
        public string ToXmlString()
        {
            var serializer = new XmlSerializer(typeof(CustomCaptionSettings));

            using (var writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                serializer.Serialize(writer, this);

                return writer.GetStringBuilder().ToString();
            }
        }

        /// <summary>
        /// Creates a <see cref="UserSettings"/> object from an xml string
        /// </summary>
        /// <param name="xml">the xml string</param>
        /// <returns>a new UserSettings</returns>
        public static CustomCaptionSettings FromString(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return null;
            }

            using (var reader = new StringReader(xml))
            {
                var serializer = new XmlSerializer(typeof(CustomCaptionSettings));

                return serializer.Deserialize(reader) as CustomCaptionSettings;
            }
        }
        #endregion
    }
}
