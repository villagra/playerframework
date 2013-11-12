// <copyright file="CaptionSettingFlyout.xaml.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-10-28</date>
// <summary>Caption settings flyout</summary>

namespace Microsoft.PlayerFramework.CaptionSettings
{
    using System;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Microsoft.PlayerFramework.CaptionSettings.ViewModel;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// Caption Settings Flyout
    /// </summary>
    public sealed partial class CaptionSettingFlyout : SettingsFlyout
    {
        #region Fields
        /// <summary>
        /// the isolated storage settings key for the override default caption settings flag
        /// </summary>
        public const string OverrideDefaultKey = "Microsoft.PlayerFramework.OverrideDefaultCaptionSettings";

        #endregion
        /// <summary>
        /// Initializes a new instance of the CaptionSettingsFlyout class.
        /// </summary>
        public CaptionSettingFlyout()
        {
            this.InitializeComponent();

            this.FontColorType.ItemsSource = new ColorType[] 
            {
                ColorType.Default,
                ColorType.Solid,
                ColorType.Semitransparent
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

            var viewModel = new CaptionSettingsFlyoutViewModel();

            viewModel.PropertyChanged += this.value_PropertyChanged;
            this.DataContext = viewModel;

            this.CaptionSettings = new CustomCaptionSettings();

            object value;

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue(OverrideDefaultKey, out value))
            {
                viewModel.IsEnabled = (bool)value;
            }
        }

        /// <summary>
        /// Event to save caption settings
        /// </summary>
        public event EventHandler<CustomCaptionSettingsEventArgs> OnApplyCaptionSettings;

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
                var dataContext = this.DataContext as CaptionSettingsFlyoutViewModel;

                dataContext.Settings = value;

                if (value != null)
                {
                    value.PropertyChanged += this.value_PropertyChanged;
                }
            }
        }

        void value_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.OnApplyCaptionSettings != null)
            {
                if (e.PropertyName == "IsEnabled")
                {
                    var dataContext = sender as CaptionSettingsFlyoutViewModel;

                    Windows.Storage.ApplicationData.Current.LocalSettings.Values[OverrideDefaultKey] = dataContext.IsEnabled;
                }

                this.OnApplyCaptionSettings(this, new CustomCaptionSettingsEventArgs(this.CaptionSettings));
            }
        }

        private void OnFontStyleChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedFontStyle = (FontStyle)this.CaptionFontStyle.SelectedItem;

            var dataContext = this.DataContext as CaptionSettingsFlyoutViewModel;

            dataContext.Settings.FontStyle = selectedFontStyle;
            
            this.Preview.CaptionFontStyle = selectedFontStyle;
        }
    }
}
