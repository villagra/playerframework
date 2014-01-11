// <copyright file="CaptionSettingsControl.cs" company="Microsoft Corporation">
// Copyright (c) 2014 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2014-01-06</date>
// <summary>Templated caption settings control</summary>

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
    /// Template Caption Settings Control
    /// </summary>
    #region Template Control Attributes
    [TemplatePartAttribute(Name = "FontFamilyButton", Type = typeof(Button))]
    [TemplatePartAttribute(Name = "FontSizeButton", Type = typeof(Button))]
    [TemplatePartAttribute(Name = "FontStyleButton", Type = typeof(Button))]
    [TemplatePartAttribute(Name = "FontColorTypeButton", Type = typeof(Button))]
    [TemplatePartAttribute(Name = "FontColorButton", Type = typeof(Button))]
    [TemplatePartAttribute(Name = "BackgroundColorType", Type = typeof(Button))]
    [TemplatePartAttribute(Name = "BackgroundColorButton", Type = typeof(Button))]
    [TemplatePartAttribute(Name = "WindowColorType", Type = typeof(Button))]
    [TemplatePartAttribute(Name = "WindowColorButton", Type = typeof(Button))]
    [TemplatePartAttribute(Name = "LayoutRoot", Type = typeof(Grid))]
    [TemplatePartAttribute(Name = "ListSelector", Type = typeof(LongListSelector))]
    [TemplatePartAttribute(Name = "OutlineEdges", Type = typeof(Grid))]
    [TemplatePartAttribute(Name = "PageTitle", Type = typeof(TextBlock))]
    [TemplatePartAttribute(Name = "PreviewTextElements", Type = typeof(Grid))]
    [TemplateVisualState(Name = "ListShown", GroupName = "ListGroup")]
    [TemplateVisualState(Name = "ListHidden", GroupName = "ListGroup")]
    [TemplateVisualState(Name = "Default", GroupName = "PreviewStates")]
    [TemplateVisualState(Name = "RaisedEdge", GroupName = "PreviewStates")]
    [TemplateVisualState(Name = "DepressedEdge", GroupName = "PreviewStates")]
    [TemplateVisualState(Name = "Outline", GroupName = "PreviewStates")]
    [TemplateVisualState(Name = "DropShadow", GroupName = "PreviewStates")]
    [TemplateVisualState(Name = "Portrait", GroupName = "OrientationStates")]
    [TemplateVisualState(Name = "Landscape", GroupName = "OrientationStates")]
    #endregion
    public class CaptionSettingsControl : Control
    {
        #region Fields
        /// <summary>
        /// the isolated storage settings key for the override default caption settings flag
        /// </summary>
        public const string OverrideDefaultKey = "Microsoft.PlayerFramework.OverrideDefaultCaptionSettings";

        /// <summary>
        /// PreviewVisibility dependency property
        /// </summary>
        public static readonly DependencyProperty PreviewVisibilityProperty =
            DependencyProperty.Register("PreviewVisibility", typeof(Visibility), typeof(CaptionSettingsControl), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Is the list selector being shown
        /// </summary>
        private bool isListSelectorShown = false;

        /// <summary>
        /// the hosting phone page
        /// </summary>
        private PhoneApplicationPage page;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the CaptionSettingsControl class
        /// </summary>
        public CaptionSettingsControl()
        {
            this.DefaultStyleKey = typeof(CaptionSettingsControl);

            this.ViewModel = new CaptionSettingsFlyoutViewModel
            {
                Settings = new CustomCaptionSettings()
            };

            this.DataContext = this.ViewModel;

            this.ViewModel.PropertyChanged += this.OnViewModelPropertyChanged;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the custom caption settings
        /// </summary>
        [Browsable(false)]
        public CustomCaptionSettings Settings
        {
            get
            {
                return this.ViewModel.Settings;
            }

            set
            {
                if (this.ViewModel.Settings != value)
                {
                    if (this.ViewModel.Settings != null)
                    {
                        this.ViewModel.Settings.PropertyChanged -= this.Settings_PropertyChanged;
                    }

                    this.ViewModel.Settings = value;

                    if (this.ViewModel.Settings != null)
                    {
                        this.ViewModel.Settings.PropertyChanged += this.Settings_PropertyChanged;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the Apply caption settings action
        /// </summary>
        [Browsable(false)]
        public Action<CustomCaptionSettings> ApplyCaptionSettings { get; set; }

        /// <summary>
        /// Gets or sets the hosting page
        /// </summary>
        [Browsable(false)]
        public PhoneApplicationPage Page
        {
            get
            {
                return this.page;
            }

            set
            {
                if (this.page != value)
                {
                    if (this.page != null)
                    {
                        this.page.OrientationChanged -= this.OnOrientationChanged;
                    }

                    this.page = value;

                    if (this.page != null)
                    {
                        this.page.OrientationChanged += this.OnOrientationChanged;

                        this.GoToOrientationState(this.page.Orientation);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the preview control visibility
        /// </summary>
        public Visibility PreviewVisibility
        {
            get { return (Visibility)this.GetValue(PreviewVisibilityProperty); }
            set { this.SetValue(PreviewVisibilityProperty, value); }
        }        

        /// <summary>
        /// Gets or sets a value indicating whether default setting should be overridden
        /// </summary>
        public bool IsOverrideDefaults
        {
            get
            {
                return this.ViewModel.IsEnabled;
            }

            set
            {
                this.ViewModel.IsEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the view model
        /// </summary>
        private CaptionSettingsFlyoutViewModel ViewModel { get; set; }

        /// <summary>
        /// Gets or sets the font family button
        /// </summary>
        private Button FontFamilyButton { get; set; }

        /// <summary>
        /// Gets or sets the font size button
        /// </summary>
        private Button FontSizeButton { get; set; }
        
        /// <summary>
        /// Gets or sets the font style button
        /// </summary>
        private Button FontStyleButton { get; set; }
        
        /// <summary>
        /// Gets or sets the list selector
        /// </summary>
        private LongListSelector ListSelector { get; set; }
        
        /// <summary>
        /// Gets or sets the font color type button
        /// </summary>
        private Button FontColorTypeButton { get; set; }
        
        /// <summary>
        /// Gets or sets the font color button
        /// </summary>
        private Button FontColorButton { get; set; }
        
        /// <summary>
        /// Gets or sets the background color type
        /// </summary>
        private Button BackgroundColorType { get; set; }
        
        /// <summary>
        /// Gets or sets the background color button
        /// </summary>
        private Button BackgroundColorButton { get; set; }
        
        /// <summary>
        /// Gets or sets the window color type
        /// </summary>
        private Button WindowColorType { get; set; }

        /// <summary>
        /// Gets or sets the window color button
        /// </summary>
        private Button WindowColorButton { get; set; }

        /// <summary>
        /// Gets or sets the outline edges
        /// </summary>
        private Panel OutlineEdges { get; set; }

        /// <summary>
        /// Gets or sets the page title
        /// </summary>
        private TextBlock PageTitle { get; set; }

        /// <summary>
        /// Gets or sets the preview text elements
        /// </summary>
        private Panel PreviewTextElements { get; set; }

        /// <summary>
        /// Gets or sets the layout root
        /// </summary>
        private Grid LayoutRoot { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Update the page orientation when the page loads
        /// </summary>
        /// <param name="pageOrientation">the page orientation</param>
        public void UpdateOrientation(PageOrientation pageOrientation)
        {
            this.GoToOrientationState(pageOrientation);
        }

        /// <summary>
        /// Apply the template
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.FontFamilyButton = this.GetButton("FontFamilyButton", this.FontFamilyButton_Click);
            this.FontSizeButton = this.GetButton("FontSizeButton", this.FontSizeButton_Click);
            this.FontStyleButton = this.GetButton("FontStyleButton", this.FontStyleButton_Click);
            this.FontColorTypeButton = this.GetButton("FontColorTypeButton", this.FontColorTypeButton_Click);
            this.FontColorButton = this.GetButton("FontColorButton", this.FontColorButton_Click);
            this.BackgroundColorType = this.GetButton("BackgroundColorType", this.BackgroundColorType_Click);
            this.BackgroundColorButton = this.GetButton("BackgroundColorButton", this.BackgroundColorButton_Click);
            this.WindowColorType = this.GetButton("WindowColorType", this.WindowColorType_Click);
            this.WindowColorButton = this.GetButton("WindowColorButton", this.WindowColorButton_Click);

            this.LayoutRoot = this.GetTemplateChild("LayoutRoot") as Grid;
            this.ListSelector = this.GetTemplateChild("ListSelector") as LongListSelector;
            this.OutlineEdges = this.GetTemplateChild("OutlineEdges") as Panel;
            this.PageTitle = this.GetTemplateChild("PageTitle") as TextBlock;
            this.PreviewTextElements = this.GetTemplateChild("PreviewTextElements") as Panel;

            this.UpdateFontStyle(this.Settings);

            this.UpdateCaptials(this.Settings);

            if (this.Page != null)
            {
                this.UpdateOrientation(this.Page.Orientation);
            }
        }

        /// <summary>
        /// Set the view model when navigating to this page
        /// </summary>
        public void OnNavigatedTo()
        {
            bool isEnabled = false;

            object value;

            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(OverrideDefaultKey, out value))
            {
                isEnabled = (bool)value;
            }

            this.ViewModel.IsEnabled = isEnabled;
        }

        /// <summary>
        /// Apply the caption settings when navigating from this page
        /// </summary>
        /// <param name="e">the navigating cancel event arguments</param>
        public void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (this.isListSelectorShown)
            {
                this.HideListSelector();
                this.RemoveSelectionChanged();
                e.Cancel = true;
            }
            else
            {
                this.NotifyApplyCaptionSettings();

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// update the capitals for all of the TextBlock elements in a panel.
        /// </summary>
        /// <param name="customCaptionSettings">the custom caption settings
        /// </param>
        /// <param name="panel">the panel</param>
        /// <exception cref="ArgumentNullException">if customCaptionSettings 
        /// or panel are null.</exception>
        private static void UpdateCaptials(
            CustomCaptionSettings customCaptionSettings,
            Panel panel)
        {
            if (customCaptionSettings == null)
            {
                throw new ArgumentNullException(
                    "customCaptionSettings",
                    "customCaptionSettings cannot be null.");
            }

            if (panel == null)
            {
                return;
            }

            var children = panel.Children.OfType<TextBlock>();

            foreach (var item in children)
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
        /// Notify to apply caption settings
        /// </summary>
        private void NotifyApplyCaptionSettings()
        {
            IsolatedStorageSettings.ApplicationSettings[OverrideDefaultKey] = this.ViewModel.IsEnabled;

            if (this.ViewModel.IsEnabled)
            {
                if (this.ApplyCaptionSettings != null)
                {
                    this.ApplyCaptionSettings(this.ViewModel.Settings);
                }
            }
            else
            {
                if (this.ApplyCaptionSettings != null)
                {
                    this.ApplyCaptionSettings(null);
                }
            }
        }

        /// <summary>
        /// Update the orientation when the page orientation changes
        /// </summary>
        /// <param name="sender">the phone application page</param>
        /// <param name="e">the orientation changed event arguments</param>
        private void OnOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            this.UpdateOrientation(e.Orientation);
        }

        /// <summary>
        /// Get a button from the control template and put a click handler on it
        /// </summary>
        /// <param name="name">the name of the button</param>
        /// <param name="clickHandler">the click handler</param>
        /// <returns>the button</returns>
        private Button GetButton(string name, RoutedEventHandler clickHandler)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name", "button name cannot be null or empty.");
            }

            if (clickHandler == null)
            {
                throw new ArgumentNullException("clickHandler", "button click handler cannot be null or empty.");
            }

            var button = this.GetTemplateChild(name) as Button;

            if (button != null)
            {
                button.Click += clickHandler;
            }

            return button;
        }

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

            if (this.ApplyCaptionSettings != null)
            {
                this.ApplyCaptionSettings(this.Settings);
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

            VisualStateManager.GoToState(this, stateName, false);
        }

        /// <summary>
        /// Update the capitals
        /// </summary>
        /// <param name="sender">the sender</param>
        private void UpdateCaptials(object sender)
        {
            var customCaptionSettings = sender as CustomCaptionSettings;

            if (!DesignerProperties.IsInDesignTool)
            {
                UpdateCaptials(customCaptionSettings, this.PreviewTextElements);

                UpdateCaptials(customCaptionSettings, this.OutlineEdges);
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
            if (this.ListSelector == null)
            {
                return;
            }

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
            var listSelector = sender as LongListSelector;

            listSelector.SelectionChanged -= this.OnFontFamilyChanged;

            var selectedItem = (Model.FontFamily)listSelector.SelectedItem;
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
            var listSelector = sender as LongListSelector;

            listSelector.SelectionChanged -= this.OnFontSizeChanged;

            var selectedItem = listSelector.SelectedItem.ToString();
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
            if (this.ListSelector == null)
            {
                return;
            }

            var viewModel = this.DataContext as CaptionSettingsFlyoutViewModel;
            
            if (this.PageTitle != null)
            {
                this.PageTitle.Text = title;
            }

            this.ListSelector.ItemsSource = itemsSource;
            this.ListSelector.SelectedItem = selectedItem;
            this.ListSelector.SelectionChanged += selectionChanged;

            if (this.LayoutRoot != null)
            {
                this.ListSelector.ItemTemplate = this.LayoutRoot.Resources[templateName] as DataTemplate;

                if (this.ListSelector.ItemTemplate == null)
                {
                    throw new InvalidOperationException("Could not find template in LayoutRoot: " + templateName);
                }
            }

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
            var listSelector = sender as LongListSelector;

            listSelector.SelectionChanged -= this.OnFontStyleChanged;

            var selectedItem = (Model.FontStyle)listSelector.SelectedItem;
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

            if (this.PageTitle != null)
            {
                this.PageTitle.Text = AppResources.CaptionSettings.ToUpper(CultureInfo.CurrentCulture);
            }

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
            var listSelector = sender as LongListSelector;

            listSelector.SelectionChanged -= this.OnFontColorTypeChanged;

            var selectedItem = (Model.ColorType)listSelector.SelectedItem;
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
            var listSelector = sender as LongListSelector;

            listSelector.SelectionChanged -= this.OnBackgroundColorTypeChanged;

            var selectedItem = (Model.ColorType)listSelector.SelectedItem;
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
            var listSelector = sender as LongListSelector;

            listSelector.SelectionChanged -= this.OnWindowColorTypeChanged;

            var selectedItem = (Model.ColorType)listSelector.SelectedItem;
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

            if (this.ListSelector != null)
            {
                this.ListSelector.LayoutMode = LongListSelectorLayoutMode.Grid;
            }

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
            var listSelector = sender as LongListSelector;

            listSelector.SelectionChanged -= this.OnFontColorChanged;
            listSelector.LayoutMode = LongListSelectorLayoutMode.List;
            var selectedItem = (Model.Color)listSelector.SelectedItem;
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
            var listSelector = sender as LongListSelector;

            listSelector.SelectionChanged -= this.OnBackgroundColorChanged;
            listSelector.LayoutMode = LongListSelectorLayoutMode.List;
            var selectedItem = (Model.Color)listSelector.SelectedItem;
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
            var listSelector = sender as LongListSelector;

            listSelector.SelectionChanged -= this.OnWindowColorChanged;
            listSelector.LayoutMode = LongListSelectorLayoutMode.List;
            var selectedItem = (Model.Color)listSelector.SelectedItem;
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
            if (this.ListSelector == null)
            {
                return;
            }

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
            if (this.ListSelector == null)
            {
                return;
            }

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

        /// <summary>
        /// Go to the orientation state
        /// </summary>
        /// <param name="orientation">the orientation state</param>
        private void GoToOrientationState(PageOrientation orientation)
        {
            var stateName = "Landscape";

            if (orientation == PageOrientation.Portrait ||
                orientation == PageOrientation.PortraitDown ||
                orientation == PageOrientation.PortraitUp)
            {
                stateName = "Portrait";
            }

            var succeeded = VisualStateManager.GoToState(this, stateName, false);

            if (!succeeded)
            {
                System.Diagnostics.Debug.WriteLine("Failed to go to orientation state: {0}", stateName);
            }
        }

        /// <summary>
        /// When a ViewModel.IsEnabled property changes, set the Settings to default values 
        /// </summary>
        /// <param name="sender">the view model</param>
        /// <param name="e">the property changed event arguments</param>
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsEnabled")
            {
                this.NotifyApplyCaptionSettings();

                if (!this.ViewModel.IsEnabled)
                {
                    this.ViewModel.Settings.BackgroundColor = null;
                    this.ViewModel.Settings.FontColor = null;
                    this.ViewModel.Settings.FontFamily = Model.FontFamily.Default;
                    this.ViewModel.Settings.FontSize = null;
                    this.ViewModel.Settings.FontStyle = Model.FontStyle.Default;
                    this.ViewModel.Settings.WindowColor = null;
                    this.ViewModel.WindowColorType = ColorType.Default;
                    this.ViewModel.FontColorType = ColorType.Default;
                    this.ViewModel.BackgroundColorType = ColorType.Default;
                }
            }
        }

        #endregion
    }
}
