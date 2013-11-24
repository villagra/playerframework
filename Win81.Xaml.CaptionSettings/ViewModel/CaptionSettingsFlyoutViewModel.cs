// <copyright file="CaptionSettingsFlyoutViewModel.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-06</date>
// <summary>Caption Settings Flyout view model</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.ViewModel
{
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
#if WINDOWS_PHONE
    using Media = System.Windows.Media;
#else
    using Windows.ApplicationModel.Resources;
#endif
    /// <summary>
    /// Caption settings flyout view model
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
    public class CaptionSettingsFlyoutViewModel : BindableBase
    {
        #region Fields
        /// <summary>
        /// the settings
        /// </summary>
        private CustomCaptionSettings settings;

        /// <summary>
        /// are the settings enabled?
        /// </summary>
        private bool isEnabled;

        /// <summary>
        /// the preview text
        /// </summary>
        private string previewText;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CaptionSettingsFlyoutViewModel class.
        /// </summary>
        public CaptionSettingsFlyoutViewModel()
        {
            this.IsEnabled = false;
#if WINDOWS_PHONE
            this.PreviewText = Resources.AppResources.PreviewText;

            if (DesignerProperties.IsInDesignTool)
            {
                this.Settings = new CustomCaptionSettings
                {
                    FontColor = Media.Colors.White.ToCaptionSettingsColor()
                };
            }

            var defaultText = Resources.AppResources.Default;
#else
            var resources = AssemblyResources.Get();

            this.PreviewText = resources.GetString("PreviewText");
            var defaultText = resources.GetString("Default");

#endif

            this.FontFamilies = new FontFamily[]
            {
                FontFamily.Default,
                FontFamily.MonospaceSerif,
                FontFamily.ProportionalSerif,
                FontFamily.MonospaceSansSerif,
                FontFamily.ProportionalSansSerif,
                FontFamily.Casual,
                FontFamily.Cursive,
                FontFamily.Smallcaps
            };

            this.FontSizes = new string[]
            {
                defaultText,
                "50",
                "100",
                "150",
                "200"
            };

            this.FontStyles = new FontStyle[]
            {
                FontStyle.Default,
                FontStyle.None,
                FontStyle.RaisedEdge,
                FontStyle.DepressedEdge,
                FontStyle.Outline,
                FontStyle.DropShadow
            };
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the preview text
        /// </summary>
        public string PreviewText
        {
            get
            {
                return this.previewText;
            }

            set
            {
                this.SetProperty(ref this.previewText, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the options are enabled
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }

            set
            {
                if (this.SetProperty(ref this.isEnabled, value))
                {
                    this.EnabledPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the font color button is enabled
        /// </summary>
        public bool IsFontColorEnabled
        {
            get
            {
                return this.IsEnabled && this.Settings != null && this.Settings.FontColor != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the background color button is enabled
        /// </summary>
        public bool IsBackgroundColorEnabled
        {
            get
            {
                return this.IsEnabled && this.Settings != null && this.Settings.BackgroundColor != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the window color button is enabled
        /// </summary>
        public bool IsWindowColorEnabled
        {
            get
            {
                return this.IsEnabled && this.Settings != null && this.Settings.WindowColor != null;
            }
        }

        /// <summary>
        /// Gets the font families
        /// </summary>
        public FontFamily[] FontFamilies { get; private set; }

        /// <summary>
        /// Gets the font sizes
        /// </summary>
        public string[] FontSizes { get; private set; }

        /// <summary>
        /// Gets the font styles
        /// </summary>
        public FontStyle[] FontStyles { get; private set; }

        /// <summary>
        /// Gets or sets the font color type
        /// </summary>
        public ColorType FontColorType
        {
            get
            {
                if (this.Settings == null)
                {
                    return ColorType.Default;
                }

                return this.GetColorType(this.Settings.FontColor);
            }

            set
            {
                if (this.FontColorType != value)
                {
                    if (this.Settings == null)
                    {
                        return;
                    }

                    this.Settings.FontColor = this.SetColorType(
                        value,
                        this.Settings.FontColor,
                        new Color { Alpha = 255, Blue = 255, Green = 255, Red = 255 });

                    this.OnPropertyChanged();
                    this.OnPropertyChanged("IsFontColorEnabled");
                }
            }
        }

        /// <summary>
        /// Gets or sets the background color type
        /// </summary>
        public ColorType BackgroundColorType
        {
            get
            {
                if (this.Settings == null)
                {
                    return ColorType.Default;
                }

                return this.GetColorType(this.Settings.BackgroundColor);
            }

            set
            {
                if (this.BackgroundColorType != value)
                {
                    if (this.Settings == null)
                    {
                        return;
                    }

                    this.Settings.BackgroundColor = this.SetColorType(
                        value,
                        this.Settings.BackgroundColor,
                        new Color { Alpha = 0, Blue = 0, Green = 0, Red = 0 });

                    this.OnPropertyChanged();
                    this.OnPropertyChanged("IsBackgroundColorEnabled");
                }
            }
        }

        /// <summary>
        /// Gets or sets the window color type
        /// </summary>
        public ColorType WindowColorType
        {
            get
            {
                if (this.Settings == null)
                {
                    return ColorType.Default;
                }

                return this.GetColorType(this.Settings.WindowColor);
            }

            set
            {
                if (this.WindowColorType != value)
                {
                    if (this.Settings == null)
                    {
                        return;
                    }

                    this.Settings.WindowColor = this.SetColorType(
                        value,
                        this.Settings.WindowColor,
                        new Color { Alpha = 0, Blue = 0, Green = 0, Red = 0 });

                    this.OnPropertyChanged();
                    this.OnPropertyChanged("IsWindowColorEnabled");
                }
            }
        }

        /// <summary>
        /// Gets or sets the custom caption settings
        /// </summary>
        public CustomCaptionSettings Settings
        {
            get
            {
                return this.settings;
            }

            set
            {
                if (this.settings != null)
                {
                    this.settings.PropertyChanged -= this.CaptionSettingsPropertyChanged;
                }

                if (this.SetProperty(ref this.settings, value) && value != null)
                {
                    value.PropertyChanged += this.CaptionSettingsPropertyChanged;

                    this.OnPropertyChanged("FontColorType");
                    this.OnPropertyChanged("BackgroundColorType");
                    this.OnPropertyChanged("WindowColorType");
                }
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the color type
        /// </summary>
        /// <param name="color">the color</param>
        /// <returns>the color type</returns>
        private ColorType GetColorType(Color color)
        {
            if (this.Settings == null)
            {
                return ColorType.Default;
            }

            if (color == null)
            {
                return ColorType.Default;
            }

            switch (color.Alpha)
            {
                case 0:
                    return ColorType.Transparent;

                case 255:
                    return ColorType.Solid;

                default:
                    return ColorType.Semitransparent;
            }
        }

        /// <summary>
        /// Sets a color type
        /// </summary>
        /// <param name="type">the color type</param>
        /// <param name="color">the current color</param>
        /// <param name="defaultColor">the default color</param>
        /// <returns>the new color</returns>
        private Color SetColorType(ColorType type, Color color, Color defaultColor)
        {
            if (color == null)
            {
                color = new Color { Alpha = 255, Blue = 255, Green = 255, Red = 255 };
            }

            switch (type)
            {
                case ColorType.Default:
                    color = null;
                    break;

                case ColorType.Semitransparent:
                    color.Alpha = 127;
                    break;

                case ColorType.Solid:
                    color.Alpha = 255;
                    break;

                case ColorType.Transparent:
                    color.Alpha = 0;
                    break;
            }

            return color;
        }

        /// <summary>
        /// the Enabled property changed
        /// </summary>
        private void EnabledPropertyChanged()
        {
            this.OnPropertyChanged("IsFontColorEnabled");
            this.OnPropertyChanged("IsBackgroundColorEnabled");
            this.OnPropertyChanged("IsWindowColorEnabled");
        }

        /// <summary>
        /// Caption settings property changes event handler
        /// </summary>
        /// <param name="sender">the Custom Caption Settings</param>
        /// <param name="e">the property changed event arguments</param>
        private void CaptionSettingsPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FontColorType" || e.PropertyName == "BackgroundColorType" || e.PropertyName == "WindowColorType")
            {
                this.EnabledPropertyChanged();
            }
        }
        #endregion
    }
}
