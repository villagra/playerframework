// <copyright file="CaptionSettingsControl.xaml.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-24</date>
// <summary>Caption Settings Control class definition</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.Controls
{
    using System;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Microsoft.PlayerFramework.CaptionSettings.ViewModel;
    using Windows.ApplicationModel;
    using Windows.UI;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Caption Settings Control
    /// </summary>
    public sealed partial class CaptionSettingsControl : UserControl
    {
        #region Fields
        /// <summary>
        /// the isolated storage settings key for the override default caption settings flag
        /// </summary>
        public const string OverrideDefaultKey = "Microsoft.PlayerFramework.OverrideDefaultCaptionSettings";

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CaptionSettingsControl class.
        /// </summary>
        public CaptionSettingsControl()
        {
            this.InitializeComponent();

            if (DesignMode.DesignModeEnabled)
            {
                this.LayoutRoot.Background = new SolidColorBrush(Colors.Black);
            }

            var loader = Microsoft.PlayerFramework.CaptionSettings.AssemblyResources.Get();

            var viewModel = new CaptionSettingsFlyoutViewModel();

            this.DataContext = viewModel;

            object value;

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue(OverrideDefaultKey, out value))
            {
                viewModel.IsEnabled = (bool)value;
            }

            viewModel.PropertyChanged += this.OnViewModelPropertyChanged;
        }
        #endregion

        #region Events
        /// <summary>
        /// Event to save caption settings
        /// </summary>
        public event EventHandler<CustomCaptionSettingsEventArgs> OnApplyCaptionSettings;

        /// <summary>
        /// Is Default changed event handler
        /// </summary>
        public event EventHandler IsDefaultChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the caption settings
        /// </summary>
        public CustomCaptionSettings CaptionSettings
        {
            get
            {
                var dataContext = this.DataContext as CaptionSettingsFlyoutViewModel;

                if (dataContext.IsEnabled)
                {
                    return dataContext.Settings;
                }

                return null;
            }

            set
            {
                if (value != null)
                {
                    this.FontColorType.ItemsSource = new ColorType[] 
                    {
                        ColorType.Default,
                        ColorType.Solid,
                        ColorType.Semitransparent,
                    };

                    this.BackgroundColorType.ItemsSource = new ColorType[]
                    {
                        ColorType.Default,
                        ColorType.Solid,
                        ColorType.Semitransparent,
                        ColorType.Transparent
                    };

                    this.WindowColorType.ItemsSource = new ColorType[]
                    {
                        ColorType.Default,
                        ColorType.Solid,
                        ColorType.Semitransparent,
                        ColorType.Transparent
                    };
                }

                var dataContext = this.DataContext as CaptionSettingsFlyoutViewModel;

                dataContext.Settings = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the default caption 
        /// settings are being used
        /// </summary>
        public bool IsDefault
        {
            get
            {
                var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

                return !viewModel.IsEnabled;
            }

            set
            {
                var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

                viewModel.IsEnabled = !value;
            }
        }

        #endregion

        #region Implementation
        /// <summary>
        /// If the IsEnabled value changes on the viewModel, save that value to the local settings
        /// </summary>
        /// <param name="sender">the view model</param>
        /// <param name="e">the property changed event arguments</param>
        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsEnabled")
            {
                var dataContext = sender as CaptionSettingsFlyoutViewModel;

                Windows.Storage.ApplicationData.Current.LocalSettings.Values[OverrideDefaultKey] = dataContext.IsEnabled;

                if (this.IsDefaultChanged != null)
                {
                    this.IsDefaultChanged(this, new EventArgs());
                }

                if (!dataContext.IsEnabled)
                {
                    // If IsEnabled is turned off, reset the caption settings to defaults.
                    if (dataContext.Settings != null)
                    {
                        dataContext.Settings.BackgroundColor = null;
                        dataContext.Settings.FontColor = null;
                        dataContext.Settings.FontFamily = Model.FontFamily.Default;
                        dataContext.Settings.FontSize = null;
                        dataContext.Settings.FontStyle = Model.FontStyle.Default;
                        dataContext.Settings.WindowColor = null;
                    }

                    dataContext.WindowColorType = ColorType.Default;
                    dataContext.FontColorType = ColorType.Default;
                    dataContext.BackgroundColorType = ColorType.Default;
                }

                if (this.OnApplyCaptionSettings != null)
                {
                    var settings = dataContext.IsEnabled ? this.CaptionSettings : null;

                    this.OnApplyCaptionSettings(this, new CustomCaptionSettingsEventArgs(settings));
                }
            }
        }

        /// <summary>
        /// Update the font style when the user selects a different one
        /// </summary>
        /// <param name="sender">the font style combo box</param>
        /// <param name="e">the selection changed event arguments</param>
        private void OnFontStyleChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedFontStyle = (FontStyle)this.CaptionFontStyle.SelectedItem;

            var dataContext = this.DataContext as CaptionSettingsFlyoutViewModel;

            dataContext.Settings.FontStyle = selectedFontStyle;

            this.Preview.CaptionFontStyle = selectedFontStyle;
        }

        /// <summary>
        /// Pick the font color.
        /// </summary>
        /// <param name="sender">the font color button</param>
        /// <param name="e">the routed event arguments</param>
        private void FontColorButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.PickColor(
                sender, 
                delegate(Model.Color color)
                {
                    var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

                    this.CaptionSettings.FontColor = CaptionSettingsFlyoutViewModel.SetColorType(viewModel.FontColorType, color, new Model.Color { Alpha = 255, Red = 255, Green = 255, Blue = 255 });
                });
        }

        /// <summary>
        /// Pick a color
        /// </summary>
        /// <param name="sender">the button</param>
        /// <param name="setColor">the function to set the color</param>
        private void PickColor(object sender, Action<Microsoft.PlayerFramework.CaptionSettings.Model.Color> setColor)
        {
            var frameworkElement = sender as FrameworkElement;

            var buttonRect = frameworkElement.GetElementRect();

            var colorPicker = new ColorPickerControl();

            var flyout = new Popup
            {
                Child = colorPicker,
                HorizontalOffset = buttonRect.Right - colorPicker.Width,
                VerticalOffset = buttonRect.Bottom < Window.Current.Bounds.Height / 2 ? buttonRect.Bottom : buttonRect.Top - colorPicker.Height,
            };

            flyout.IsLightDismissEnabled = true;

            flyout.IsOpen = true;

            colorPicker.ColorSelected += new EventHandler<ColorEventArgs>(delegate(object sender2, ColorEventArgs colorSelected)
            {
                flyout.IsOpen = false;

                setColor(colorSelected.Color);
            });
        }

        /// <summary>
        /// Pick the background color
        /// </summary>
        /// <param name="sender">the background color button</param>
        /// <param name="e">the routed event arguments</param>
        private void BackgroundColorButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.PickColor(
                sender, 
                delegate(Model.Color color)
                {
                    var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

                    this.CaptionSettings.BackgroundColor = CaptionSettingsFlyoutViewModel.SetColorType(viewModel.BackgroundColorType, color, new Model.Color { Alpha = 0, Red = 0, Green = 0, Blue = 0 });
                });
        }

        /// <summary>
        /// Pick the window color
        /// </summary>
        /// <param name="sender">the window color button</param>
        /// <param name="e">the routed event arguments</param>
        private void WindowColorButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.PickColor(
                sender, 
                delegate(Model.Color color)
                {
                    var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

                    this.CaptionSettings.WindowColor = CaptionSettingsFlyoutViewModel.SetColorType(viewModel.WindowColorType, color, new Model.Color { Alpha = 0, Red = 0, Green = 0, Blue = 0 });
                });
        }
        #endregion
    }
}
