// <copyright file="CaptionSettingsControl.cs" company="Microsoft Corporation">
// Copyright (c) 2014 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2014-01-09</date>
// <summary>Template-based caption settings control</summary>

namespace Microsoft.PlayerFramework.CaptionSettings
{
    using System;
    using System.ComponentModel;
    using Microsoft.PlayerFramework.CaptionSettings.Controls;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Microsoft.PlayerFramework.CaptionSettings.ViewModel;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Caption Settings Control
    /// </summary>
    #region Control Contract
    [TemplatePartAttribute(Name = "FontColorType", Type = typeof(ComboBox))]
    [TemplatePartAttribute(Name = "BackgroundColorType", Type = typeof(ComboBox))]
    [TemplatePartAttribute(Name = "WindowColorType", Type = typeof(ComboBox))]
    [TemplatePartAttribute(Name = "FontColorButton", Type = typeof(Button))]
    [TemplatePartAttribute(Name = "BackgroundColorButton", Type = typeof(Button))]
    [TemplatePartAttribute(Name = "WindowColorButton", Type = typeof(Button))]
    [TemplatePartAttribute(Name = "CaptionFontStyle", Type = typeof(ComboBox))]
    [TemplatePartAttribute(Name = "Preview", Type = typeof(PreviewControl))]
    [TemplatePartAttribute(Name = "WindowColorButton", Type = typeof(Button))]
    [TemplatePartAttribute(Name = "FontColorPicker", Type = typeof(ColorPickerControl))]
    [TemplatePartAttribute(Name = "BackgroundColorPicker", Type = typeof(ColorPickerControl))]
    [TemplatePartAttribute(Name = "WindowColorPicker", Type = typeof(ColorPickerControl))]
    #endregion
    public sealed class CaptionSettingsControl : Control
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
            this.DefaultStyleKey = typeof(CaptionSettingsControl);

            var viewModel = new CaptionSettingsFlyoutViewModel();

            viewModel.PropertyChanged += this.OnViewModelPropertyChanged;

            this.DataContext = viewModel;

            this.CaptionSettings = new CustomCaptionSettings();

            object value;

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.TryGetValue(OverrideDefaultKey, out value))
            {
                viewModel.IsEnabled = (bool)value;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event to save caption settings
        /// </summary>
        public event EventHandler<CustomCaptionSettingsEventArgs> OnApplyCaptionSettings;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the caption settings
        /// </summary>
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
        /// Gets or sets the font color type ComboBox
        /// </summary>
        private ComboBox FontColorType { get; set; }

        /// <summary>
        /// Gets or sets the BackgroundColorType ComboBox
        /// </summary>
        private ComboBox BackgroundColorType { get; set; }

        /// <summary>
        /// Gets or sets the WindowColorType ComboBox
        /// </summary>
        private ComboBox WindowColorType { get; set; }

        /// <summary>
        /// Gets or sets the FontColor Button 
        /// </summary>
        private Button FontColorButton { get; set; }

        /// <summary>
        /// Gets or sets the background color button
        /// </summary>
        private Button BackgroundColorButton { get; set; }

        /// <summary>
        /// Gets or sets the window color button
        /// </summary>
        private Button WindowColorButton { get; set; }

        /// <summary>
        /// Gets or sets the caption font style ComboBox
        /// </summary>
        private ComboBox CaptionFontStyle { get; set; }

        /// <summary>
        /// Gets or sets the preview control
        /// </summary>
        private PreviewControl Preview { get; set; }

        /// <summary>
        /// Gets or sets the font color picker
        /// </summary>
        private ColorPickerControl FontColorPicker { get; set; }

        /// <summary>
        /// Gets or sets the background color picker
        /// </summary>
        private ColorPickerControl BackgroundColorPicker { get; set; }

        /// <summary>
        /// Gets or sets the window color picker
        /// </summary>
        private ColorPickerControl WindowColorPicker { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Apply templates
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.FontColorType = this.GetTemplateChild("FontColorType") as ComboBox;
            this.BackgroundColorType = this.GetTemplateChild("BackgroundColorType") as ComboBox;
            this.WindowColorType = this.GetTemplateChild("WindowColorType") as ComboBox;
            this.CaptionFontStyle = this.GetTemplateChild("CaptionFontStyle") as ComboBox;
            this.FontColorButton = this.GetTemplateChild("FontColorButton") as Button;
            this.BackgroundColorButton = this.GetTemplateChild("BackgroundColorButton") as Button;
            this.WindowColorButton = this.GetTemplateChild("WindowColorButton") as Button;

            this.FontColorPicker = this.GetColorPicker("FontColorPicker", this.OnFontColorSelected);
            this.BackgroundColorPicker = this.GetColorPicker("BackgroundColorPicker", this.OnBackgroundColorSelected);
            this.WindowColorPicker = this.GetColorPicker("WindowColorPicker", this.OnWindowColorSelected);            

            this.Preview = this.GetTemplateChild("Preview") as PreviewControl;

            this.Initialize();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets a color picker from the control template
        /// </summary>
        /// <param name="name">the name of the control</param>
        /// <param name="colorSelected">the color selected event handler</param>
        /// <returns>the color picker control</returns>
        private ColorPickerControl GetColorPicker(string name, EventHandler<ColorEventArgs> colorSelected)
        {
            var picker = this.GetTemplateChild(name) as ColorPickerControl;

            if (picker != null)
            {
                picker.ColorSelected += colorSelected;
            }

            return picker;
        }

        /// <summary>
        /// Initialize the controls and view model
        /// </summary>
        private void Initialize()
        {
            if (this.CaptionFontStyle != null)
            {
                this.CaptionFontStyle.SelectionChanged += this.OnFontStyleChanged;
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
                var dataContext = this.DataContext as CaptionSettingsFlyoutViewModel;

                if (e.PropertyName == "IsEnabled")
                {
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values[OverrideDefaultKey] = dataContext.IsEnabled;
                }

                var settings = dataContext.IsEnabled ? this.CaptionSettings : null;

                this.OnApplyCaptionSettings(this, new CustomCaptionSettingsEventArgs(settings));
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
        
        #endregion
    }
}
