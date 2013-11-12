namespace Microsoft.PlayerFramework.CaptionSettings.Controls
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Microsoft.PlayerFramework.CaptionSettings.Resources;
    using Microsoft.PlayerFramework.CaptionSettings.ViewModel;
    using System.IO.IsolatedStorage;
    using System.ComponentModel;
    using System.Windows.Documents;

    public partial class CaptionSettingsControl : UserControl
    {
        #region Fields
        /// <summary>
        /// the isolated storage settings key for the override default caption settings flag
        /// </summary>
        public const string OverrideDefaultKey = "Microsoft.PlayerFramework.OverrideDefaultCaptionSettings";
        #endregion

        #region Constructors
        public CaptionSettingsControl()
        {
            this.InitializeComponent();

            this.DataContext = new CaptionSettingsFlyoutViewModel();

            if (DesignerProperties.IsInDesignTool)
            {
                this.Settings = new CustomCaptionSettings
                {
                    FontColor = Colors.White.ToCaptionSettingsColor()
                };
            }

            VisualStateManager.GoToState(this, "Default", false);
        }
        #endregion

        #region Implementation
        void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
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

        private void UpdateFontStyle(object sender)
        {
            var customCaptionSettings = sender as CustomCaptionSettings;

            var stateName = customCaptionSettings.FontStyle.ToString();

            VisualStateManager.GoToState(this, stateName, true);
        }

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
        #endregion

        #region Properties
        public Model.CustomCaptionSettings Settings
        {
            get
            {
                var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

                return viewModel.Settings;
            }
            set
            {
                var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

                if (viewModel.Settings != null)
                {
                    viewModel.Settings.PropertyChanged -= this.Settings_PropertyChanged;
                }

                viewModel.Settings = value;

                if (viewModel.Settings != null)
                {
                    viewModel.Settings.PropertyChanged += this.Settings_PropertyChanged;
                }
            }
        }
        public Action<CustomCaptionSettings> ApplyCaptionSettings { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Set the viewmodel when navigating to this page
        /// </summary>
        /// <param name="e">the navigation event arguments</param>
        internal void OnNavigatedTo()
        {
            bool isEnabled = false;

            object value;

            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(OverrideDefaultKey, out value))
            {
                isEnabled = (bool)value;
            }

            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;


            viewModel.IsEnabled = isEnabled;
        }

        /// <summary>
        /// Apply the caption settings when navigating from this page
        /// </summary>
        /// <param name="e">the navigating cancel event arguments</param>
        internal void OnNavigatingFrom()
        {
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;
            
            IsolatedStorageSettings.ApplicationSettings[OverrideDefaultKey] = viewModel.IsEnabled;

            if (viewModel.IsEnabled)
            {
                if (ApplyCaptionSettings != null)
                {
                    ApplyCaptionSettings(viewModel.Settings);
                }
            }
        }
        #endregion

        #region Implementation
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

        void OnFontFamilyChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListSelector.SelectionChanged -= this.OnFontFamilyChanged;

            var selectedItem = (Model.FontFamily)this.ListSelector.SelectedItem;
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;
            viewModel.Settings.FontFamily = selectedItem;

            this.HideListSelector();
        }

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

        void OnFontSizeChanged(object sender, SelectionChangedEventArgs e)
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
        }

        void OnFontStyleChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListSelector.SelectionChanged -= this.OnFontStyleChanged;

            var selectedItem = (Model.FontStyle)this.ListSelector.SelectedItem;
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            viewModel.Settings.FontStyle = selectedItem;

            this.HideListSelector();
        }

        private void HideListSelector()
        {
            this.PageTitle.Text = AppResources.CaptionSettings.ToUpper(CultureInfo.CurrentCulture);

            VisualStateManager.GoToState(this, "ListHidden", true);
        }

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
                viewModel.Settings.FontColorType,
                OnFontColorTypeChanged,
                "ColorTypeTemplate");
        }

        void OnFontColorTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListSelector.SelectionChanged -= this.OnFontColorTypeChanged;

            var selectedItem = (Model.ColorType)this.ListSelector.SelectedItem;
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            viewModel.Settings.FontColorType = selectedItem;

            this.HideListSelector();
        }

        void OnBackgroundColorTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListSelector.SelectionChanged -= this.OnBackgroundColorTypeChanged;

            var selectedItem = (Model.ColorType)this.ListSelector.SelectedItem;
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            viewModel.Settings.BackgroundColorType = selectedItem;

            this.HideListSelector();
        }

        void OnWindowColorTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListSelector.SelectionChanged -= this.OnWindowColorTypeChanged;

            var selectedItem = (Model.ColorType)this.ListSelector.SelectedItem;
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            viewModel.Settings.WindowColorType = selectedItem;

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

            this.ShowListSelector(
                AppResources.BackgroundColor,
                new ColorType[]
                { 
                    ColorType.Default,
                    ColorType.Solid,
                    ColorType.Semitransparent,
                    ColorType.Transparent
                },
                viewModel.Settings.BackgroundColorType,
                this.OnBackgroundColorTypeChanged,
                "ColorTypeTemplate");
        }

        private void WindowColorType_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            this.ShowListSelector(
                AppResources.WindowColor,
                new ColorType[]
                { 
                    ColorType.Default,
                    ColorType.Solid,
                    ColorType.Semitransparent,
                    ColorType.Transparent
                },
                viewModel.Settings.WindowColorType,
                this.OnWindowColorTypeChanged,
                "ColorTypeTemplate");
        }

        private void FontColorButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            this.ListSelector.LayoutMode = LongListSelectorLayoutMode.Grid;
            this.ShowListSelector(
                AppResources.FontColor,
                new Microsoft.PlayerFramework.CaptionSettings.Model.Color[]
                { 
                    Colors.White.ToCaptionSettingsColor(),
                    Colors.Black.ToCaptionSettingsColor(),
                    Colors.Red.ToCaptionSettingsColor(),
                    Colors.Green.ToCaptionSettingsColor(),
                    Colors.Blue.ToCaptionSettingsColor(),
                    Colors.Yellow.ToCaptionSettingsColor(),
                    Colors.Magenta.ToCaptionSettingsColor(),
                    Colors.Cyan.ToCaptionSettingsColor()
                },
                viewModel.Settings.FontColor,
                this.OnFontColorChanged,
                "ColorTemplate");
        }

        private void OnFontColorChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListSelector.SelectionChanged -= this.OnFontColorChanged;
            this.ListSelector.LayoutMode = LongListSelectorLayoutMode.List;
            var selectedItem = (Model.Color)this.ListSelector.SelectedItem;
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            viewModel.Settings.FontColor = selectedItem;

            this.HideListSelector();
        }

        private void OnBackgroundColorChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListSelector.SelectionChanged -= this.OnBackgroundColorChanged;
            this.ListSelector.LayoutMode = LongListSelectorLayoutMode.List;
            var selectedItem = (Model.Color)this.ListSelector.SelectedItem;
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            viewModel.Settings.BackgroundColor = selectedItem;

            this.HideListSelector();
        }
        void OnWindowColorChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListSelector.SelectionChanged -= this.OnWindowColorChanged;
            this.ListSelector.LayoutMode = LongListSelectorLayoutMode.List;
            var selectedItem = (Model.Color)this.ListSelector.SelectedItem;
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            viewModel.Settings.WindowColor = selectedItem;

            this.HideListSelector();
        }

        private void BackgroundColorButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            this.ListSelector.LayoutMode = LongListSelectorLayoutMode.Grid;
            this.ShowListSelector(
                AppResources.BackgroundColor,
                new Microsoft.PlayerFramework.CaptionSettings.Model.Color[]
                { 
                    Colors.White.ToCaptionSettingsColor(),
                    Colors.Black.ToCaptionSettingsColor(),
                    Colors.Red.ToCaptionSettingsColor(),
                    Colors.Green.ToCaptionSettingsColor(),
                    Colors.Blue.ToCaptionSettingsColor(),
                    Colors.Yellow.ToCaptionSettingsColor(),
                    Colors.Magenta.ToCaptionSettingsColor(),
                    Colors.Cyan.ToCaptionSettingsColor()
                },
                viewModel.Settings.BackgroundColor,
                this.OnBackgroundColorChanged,
                "ColorTemplate");
        }

        private void WindowColorButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;

            this.ListSelector.LayoutMode = LongListSelectorLayoutMode.Grid;
            this.ShowListSelector(
                AppResources.WindowColor,
                new Microsoft.PlayerFramework.CaptionSettings.Model.Color[]
                { 
                    Colors.White.ToCaptionSettingsColor(),
                    Colors.Black.ToCaptionSettingsColor(),
                    Colors.Red.ToCaptionSettingsColor(),
                    Colors.Green.ToCaptionSettingsColor(),
                    Colors.Blue.ToCaptionSettingsColor(),
                    Colors.Yellow.ToCaptionSettingsColor(),
                    Colors.Magenta.ToCaptionSettingsColor(),
                    Colors.Cyan.ToCaptionSettingsColor()
                },
                viewModel.Settings.WindowColor,
                this.OnWindowColorChanged,
                "ColorTemplate");
        }
        #endregion
    }
}
