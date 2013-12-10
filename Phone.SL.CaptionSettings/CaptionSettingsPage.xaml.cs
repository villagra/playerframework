// <copyright file="CaptionSettingsPage.xaml.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-14</date>
// <summary>Caption Settings page</summary>

namespace Microsoft.PlayerFramework.CaptionSettings
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Microsoft.PlayerFramework.CaptionSettings.Resources;
    using Microsoft.PlayerFramework.CaptionSettings.ViewModel;

    /// <summary>
    /// Caption Settings page
    /// </summary>
    public partial class CaptionSettingsPage : PhoneApplicationPage
    {
        #region Fields
        /// <summary>
        /// the isolated storage settings key for the override default caption settings flag
        /// </summary>
        public const string OverrideDefaultKey = "Microsoft.PlayerFramework.OverrideDefaultCaptionSettings";

        /// <summary>
        /// Is the list selector being shown
        /// </summary>
        private bool isListSelectorShown = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CaptionSettingsPage class.
        /// </summary>
        public CaptionSettingsPage()
        {
            this.InitializeComponent();

            if (DesignerProperties.IsInDesignTool)
            {
                CaptionSettingsPage.Settings = new CustomCaptionSettings
                {
                    FontColor = Colors.White.ToCaptionSettingsColor()
                };
            }

            this.DataContext = new CaptionSettingsFlyoutViewModel
            {
                Settings = CaptionSettingsPage.Settings
            };

            VisualStateManager.GoToState(this, "Default", false);

            CaptionSettingsPage.Settings.PropertyChanged += this.Settings_PropertyChanged;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the custom caption settings
        /// </summary>
        public static Model.CustomCaptionSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the Apply caption settings action
        /// </summary>
        public static Action<CustomCaptionSettings> ApplyCaptionSettings { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Set the view model when navigating to this page
        /// </summary>
        /// <param name="e">the navigation event arguments</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            bool isEnabled = false;

            object value;

            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(OverrideDefaultKey, out value))
            {
                isEnabled = (bool)value;
            }

            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            viewModel.IsEnabled = isEnabled;

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Apply the caption settings when navigating from this page
        /// </summary>
        /// <param name="e">the navigating cancel event arguments</param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (this.isListSelectorShown)
            {
                this.HideListSelector();
                this.RemoveSelectionChanged();
                e.Cancel = true;
            }
            else
            {
                var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

                IsolatedStorageSettings.ApplicationSettings[OverrideDefaultKey] = viewModel.IsEnabled;
                IsolatedStorageSettings.ApplicationSettings.Save();

                if (viewModel.IsEnabled)
                {
                    if (ApplyCaptionSettings != null)
                    {
                        ApplyCaptionSettings(viewModel.Settings);
                    }
                }

                base.OnNavigatingFrom(e);
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Settings property changed
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the property changed event arguments</param>
        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "FontFamily":
                    this.UpdateCaptials(sender);
                    break;
                case "FontStyle":
                    this.UpdateFontStyle(sender);
                    break;
            }
        }

        /// <summary>
        /// Update the font style
        /// </summary>
        /// <param name="sender">the sender</param>
        private void UpdateFontStyle(object sender)
        {
            var customCaptionSettings = sender as CustomCaptionSettings;

            var stateName = customCaptionSettings.FontStyle.ToString();

            VisualStateManager.GoToState(this, stateName, true);
        }

        /// <summary>
        /// Update the capitals
        /// </summary>
        /// <param name="sender">the sender</param>
        private void UpdateCaptials(object sender)
        {
            var customCaptionSettings = sender as CustomCaptionSettings;

            foreach (var item in this.PreviewTextElements.Children.OfType<TextBlock>())
            {
                if (customCaptionSettings.FontFamily == Model.FontFamily.Smallcaps)
                {
                    Typography.SetCapitals(item, FontCapitals.SmallCaps);
                }
                else
                {
                    Typography.SetCapitals(item, FontCapitals.Normal);
                }
            }
        }

        /// <summary>
        /// show the font family selector
        /// </summary>
        /// <param name="sender">the button</param>
        /// <param name="e">the routed event arguments</param>
        private void FontFamilyButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            this.ShowListSelector(
                AppResources.FontFamily,
                viewModel.FontFamilies,
                viewModel.Settings.FontFamily,
                this.OnFontFamilyChanged,
                "FontFamilyTemplate");
        }

        /// <summary>
        /// Remove the selection changed event handlers
        /// </summary>
        private void RemoveSelectionChanged()
        {
            this.ListSelector.SelectionChanged -= this.OnBackgroundColorChanged;
            this.ListSelector.SelectionChanged -= this.OnBackgroundColorTypeChanged;
            this.ListSelector.SelectionChanged -= this.OnFontColorChanged;
            this.ListSelector.SelectionChanged -= this.OnFontColorTypeChanged;
            this.ListSelector.SelectionChanged -= this.OnFontFamilyChanged;
            this.ListSelector.SelectionChanged -= this.OnFontStyleChanged;
            this.ListSelector.SelectionChanged -= this.OnWindowColorChanged;
            this.ListSelector.SelectionChanged -= this.OnWindowColorTypeChanged;
        }

        /// <summary>
        /// Update the font family
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the selection changed event arguments</param>
        private void OnFontFamilyChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListSelector.SelectionChanged -= this.OnFontFamilyChanged;

            var selectedItem = (Model.FontFamily)this.ListSelector.SelectedItem;
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;
            viewModel.Settings.FontFamily = selectedItem;

            this.HideListSelector();
        }

        /// <summary>
        /// show the font size selector
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the routed event arguments</param>
        private void FontSizeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            this.ShowListSelector(
                AppResources.FontSize,
                viewModel.FontSizes,
                viewModel.Settings.FontSize,
                this.OnFontSizeChanged,
                "FontSizeTemplate");
        }

        /// <summary>
        /// update the font size
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the selection changed event arguments</param>
        private void OnFontSizeChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListSelector.SelectionChanged -= this.OnFontSizeChanged;

            var selectedItem = this.ListSelector.SelectedItem.ToString();
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            if (selectedItem == "Default")
            {
                viewModel.Settings.FontSize = null;
            }
            else
            {
                viewModel.Settings.FontSize = int.Parse(selectedItem);
            }

            this.HideListSelector();
        }

        /// <summary>
        /// Show the font style list selector
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the routed event arguments</param>
        private void FontStyleButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            this.ShowListSelector(
                AppResources.FontStyle,
                viewModel.FontStyles, 
                viewModel.Settings.FontStyle, 
                this.OnFontStyleChanged, 
                "FontStyleTemplate");
        }

        /// <summary>
        /// Show the list selector
        /// </summary>
        /// <param name="title">the title</param>
        /// <param name="itemsSource">the items source</param>
        /// <param name="selectedItem">the selected item</param>
        /// <param name="selectionChanged">the selection changed event handler</param>
        /// <param name="templateName">the template name</param>
        private void ShowListSelector(
            string title, 
            IList itemsSource, 
            object selectedItem, 
            SelectionChangedEventHandler selectionChanged, 
            string templateName)
        {
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;
            this.PageTitle.Text = title;
            this.ListSelector.ItemsSource = itemsSource;
            this.ListSelector.SelectedItem = selectedItem;
            this.ListSelector.SelectionChanged += selectionChanged;
            this.ListSelector.ItemTemplate = this.Resources[templateName] as DataTemplate;

            VisualStateManager.GoToState(this, "ListShown", true);

            this.isListSelectorShown = true;
        }

        /// <summary>
        /// update the font style
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the selection changed event arguments</param>
        private void OnFontStyleChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListSelector.SelectionChanged -= this.OnFontStyleChanged;

            var selectedItem = (Model.FontStyle)this.ListSelector.SelectedItem;
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            viewModel.Settings.FontStyle = selectedItem;

            this.HideListSelector();
        }

        /// <summary>
        /// Hide the list selector
        /// </summary>
        private void HideListSelector()
        {
            this.isListSelectorShown = false;

            this.PageTitle.Text = AppResources.CaptionSettings.ToUpper(CultureInfo.CurrentCulture);

            VisualStateManager.GoToState(this, "ListHidden", true);
        }

        /// <summary>
        /// show the font color type list selector
        /// </summary>
        /// <param name="sender">the button</param>
        /// <param name="e">the routed event arguments</param>
        private void FontColorTypeButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            var itemsSource = new ColorType[]
                { 
                    ColorType.Default,
                    ColorType.Solid,
                    ColorType.Semitransparent
                };

            this.ShowListSelector(
                AppResources.FontColor,
                itemsSource,
                viewModel.FontColorType,
                this.OnFontColorTypeChanged,
                "ColorTypeTemplate");
        }

        /// <summary>
        /// Update the font color
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the selection changed event arguments</param>
        private void OnFontColorTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListSelector.SelectionChanged -= this.OnFontColorTypeChanged;

            var selectedItem = (Model.ColorType)this.ListSelector.SelectedItem;
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            viewModel.FontColorType = selectedItem;

            this.HideListSelector();
        }

        /// <summary>
        /// Update the background color type
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the selection changed event arguments</param>
        private void OnBackgroundColorTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListSelector.SelectionChanged -= this.OnBackgroundColorTypeChanged;

            var selectedItem = (Model.ColorType)this.ListSelector.SelectedItem;
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            viewModel.BackgroundColorType = selectedItem;

            this.HideListSelector();
        }

        /// <summary>
        /// Update the window color type
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the selection changed event arguments</param>
        private void OnWindowColorTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListSelector.SelectionChanged -= this.OnWindowColorTypeChanged;

            var selectedItem = (Model.ColorType)this.ListSelector.SelectedItem;
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            viewModel.WindowColorType = selectedItem;

            this.HideListSelector();
        }

        /// <summary>
        /// Change the background color type
        /// </summary>
        /// <param name="sender">the button</param>
        /// <param name="e">the routed event arguments</param>
        private void BackgroundColorType_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            var colorTypes = new ColorType[]
            { 
                ColorType.Default,
                ColorType.Solid,
                ColorType.Semitransparent,
                ColorType.Transparent
            };

            this.ShowListSelector(
                AppResources.BackgroundColor,
                colorTypes,
                viewModel.BackgroundColorType,
                this.OnBackgroundColorTypeChanged,
                "ColorTypeTemplate");
        }

        /// <summary>
        /// Show the window color picker
        /// </summary>
        /// <param name="sender">the window color button</param>
        /// <param name="e">the routed event arguments</param>
        private void WindowColorType_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            var colorTypes = new ColorType[]
            { 
                ColorType.Default,
                ColorType.Solid,
                ColorType.Semitransparent,
                ColorType.Transparent
            };

            this.ShowListSelector(
                AppResources.WindowColor,
                colorTypes,
                viewModel.WindowColorType,
                this.OnWindowColorTypeChanged,
                "ColorTypeTemplate");
        }

        /// <summary>
        /// Show the font color picker
        /// </summary>
        /// <param name="sender">the font color button</param>
        /// <param name="e">the routed event arguments</param>
        private void FontColorButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            this.ListSelector.LayoutMode = LongListSelectorLayoutMode.Grid;

            var colors = new Microsoft.PlayerFramework.CaptionSettings.Model.Color[]
            { 
                Colors.White.ToCaptionSettingsColor(),
                Colors.Black.ToCaptionSettingsColor(),
                Colors.Red.ToCaptionSettingsColor(),
                Colors.Green.ToCaptionSettingsColor(),
                Colors.Blue.ToCaptionSettingsColor(),
                Colors.Yellow.ToCaptionSettingsColor(),
                Colors.Magenta.ToCaptionSettingsColor(),
                Colors.Cyan.ToCaptionSettingsColor()
            };

            this.ShowListSelector(
                AppResources.FontColor,
                colors,
                viewModel.Settings.FontColor,
                this.OnFontColorChanged,
                "ColorTemplate");
        }

        /// <summary>
        /// Update the font color
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the selection changed event arguments</param>
        private void OnFontColorChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListSelector.SelectionChanged -= this.OnFontColorChanged;
            this.ListSelector.LayoutMode = LongListSelectorLayoutMode.List;
            var selectedItem = (Model.Color)this.ListSelector.SelectedItem;
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            viewModel.Settings.FontColor = CaptionSettingsFlyoutViewModel.SetColorType(viewModel.FontColorType, selectedItem, selectedItem);

            this.HideListSelector();
        }

        /// <summary>
        /// Update the background color
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the selection changed event arguments</param>
        private void OnBackgroundColorChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListSelector.SelectionChanged -= this.OnBackgroundColorChanged;
            this.ListSelector.LayoutMode = LongListSelectorLayoutMode.List;
            var selectedItem = (Model.Color)this.ListSelector.SelectedItem;
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            viewModel.Settings.BackgroundColor = CaptionSettingsFlyoutViewModel.SetColorType(viewModel.BackgroundColorType, selectedItem, selectedItem);

            this.HideListSelector();
        }

        /// <summary>
        /// Update the window color
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the selection changed event arguments</param>
        private void OnWindowColorChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListSelector.SelectionChanged -= this.OnWindowColorChanged;
            this.ListSelector.LayoutMode = LongListSelectorLayoutMode.List;
            var selectedItem = (Model.Color)this.ListSelector.SelectedItem;
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            viewModel.Settings.WindowColor = CaptionSettingsFlyoutViewModel.SetColorType(viewModel.WindowColorType, selectedItem, selectedItem);

            this.HideListSelector();
        }

        /// <summary>
        /// show the background color selector
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the routed event arguments</param>
        private void BackgroundColorButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            this.ListSelector.LayoutMode = LongListSelectorLayoutMode.Grid;

            var colors = new Microsoft.PlayerFramework.CaptionSettings.Model.Color[]
            { 
                Colors.White.ToCaptionSettingsColor(),
                Colors.Black.ToCaptionSettingsColor(),
                Colors.Red.ToCaptionSettingsColor(),
                Colors.Green.ToCaptionSettingsColor(),
                Colors.Blue.ToCaptionSettingsColor(),
                Colors.Yellow.ToCaptionSettingsColor(),
                Colors.Magenta.ToCaptionSettingsColor(),
                Colors.Cyan.ToCaptionSettingsColor()
            };

            this.ShowListSelector(
                AppResources.BackgroundColor,
                colors,
                viewModel.Settings.BackgroundColor,
                this.OnBackgroundColorChanged,
                "ColorTemplate");
        }

        /// <summary>
        /// Show the window color selector
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the routed event arguments</param>
        private void WindowColorButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            this.ListSelector.LayoutMode = LongListSelectorLayoutMode.Grid;

            var colors = new Microsoft.PlayerFramework.CaptionSettings.Model.Color[]
            { 
                Colors.White.ToCaptionSettingsColor(),
                Colors.Black.ToCaptionSettingsColor(),
                Colors.Red.ToCaptionSettingsColor(),
                Colors.Green.ToCaptionSettingsColor(),
                Colors.Blue.ToCaptionSettingsColor(),
                Colors.Yellow.ToCaptionSettingsColor(),
                Colors.Magenta.ToCaptionSettingsColor(),
                Colors.Cyan.ToCaptionSettingsColor()
            };

            this.ShowListSelector(
                AppResources.WindowColor,
                colors,
                viewModel.Settings.WindowColor,
                this.OnWindowColorChanged,
                "ColorTemplate");
        }
        #endregion
    }
}