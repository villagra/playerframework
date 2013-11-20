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
    using Microsoft.PlayerFramework.CaptionSettings.Model;
#if WINDOWS_PHONE
    using Media = System.Windows.Media;
#else
    using Windows.ApplicationModel.Resources;
#endif
    /// <summary>
    /// Caption settings flyout view model
    /// </summary>
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
            var resources = ResourceLoader.GetForCurrentView("Microsoft.PlayerFramework.CaptionSettings/Resources");

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

        public bool IsFontColorEnabled
        {
            get
            {
                return this.IsEnabled && this.Settings != null && this.Settings.FontColor != null;
            }
        }

        public bool IsBackgroundColorEnabled
        {
            get
            {
                return this.IsEnabled && this.Settings != null && this.Settings.BackgroundColor != null;
            }
        }

        public bool IsWindowColorEnabled
        {
            get
            {
                return this.IsEnabled && this.Settings != null && this.Settings.WindowColor != null;
            }
        }

        public FontFamily[] FontFamilies { get; private set; }

        public string[] FontSizes { get; private set; }

        public FontStyle[] FontStyles { get; private set; }

        public ColorType FontColorType
        {
            get
            {
                return GetColorType(this.Settings.FontColor);
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

        public ColorType BackgroundColorType
        {
            get
            {
                return GetColorType(this.Settings.BackgroundColor);
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
        public ColorType WindowColorType
        {
            get
            {
                return GetColorType(this.Settings.WindowColor);
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
                    value.PropertyChanged -= this.value_PropertyChanged;
                }

                if (this.SetProperty(ref this.settings, value) && value != null)
                {
                    value.PropertyChanged += this.value_PropertyChanged;
                }
            }
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

        private void value_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FontColorType" || e.PropertyName == "BackgroundColorType" || e.PropertyName == "WindowColorType")
            {
                this.EnabledPropertyChanged();
            }
        }
    }
}
