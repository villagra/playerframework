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
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Microsoft.PlayerFramework.CaptionSettings.ViewModel;
    using Windows.ApplicationModel.Resources;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Caption Settings Flyout
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
    public sealed partial class CaptionSettingFlyout : SettingsFlyout
    {
        #region Fields
        /// <summary>
        /// the isolated storage settings key for the override default caption settings flag
        /// </summary>
        public const string OverrideDefaultKey = "Microsoft.PlayerFramework.OverrideDefaultCaptionSettings";

        #endregion
        /// <summary>
        /// Initializes a new instance of the CaptionSettingFlyout class.
        /// </summary>
        public CaptionSettingFlyout()
        {
            this.InitializeComponent();

            var loader = AssemblyResources.Get();

            var viewModel = new CaptionSettingsFlyoutViewModel();

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

            viewModel.PropertyChanged += this.OnViewModelPropertyChanged;
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
                    value.PropertyChanged += this.OnViewModelPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Hide the color picker
        /// </summary>
        /// <param name="sender">the color picker</param>
        /// <param name="e">the color event arguments</param>
        private void OnFontColorSelected(object sender, ColorEventArgs e)
        {
            this.FontColorButton.Flyout.Hide();
        }

        /// <summary>
        /// Hide the color picker
        /// </summary>
        /// <param name="sender">the color picker</param>
        /// <param name="e">the color event arguments</param>
        private void OnBackgroundColorSelected(object sender, ColorEventArgs e)
        {
            this.BackgroundColorButton.Flyout.Hide();
        }

        /// <summary>
        /// Hide the window color picker
        /// </summary>
        /// <param name="sender">the color picker</param>
        /// <param name="e">the color event arguments</param>
        private void OnWindowColorSelected(object sender, ColorEventArgs e)
        {
            this.WindowColorButton.Flyout.Hide();
        }

        /// <summary>
        /// If the IsEnabled value changes on the viewModel, save that value to the local settings
        /// </summary>
        /// <param name="sender">the view model</param>
        /// <param name="e">the property changed event arguments</param>
        private void OnViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
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
    }
}
