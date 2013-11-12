namespace Microsoft.PlayerFramework.CaptionSettings
{
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Shapes;

    public sealed partial class ColorPickerControl : UserControl
    {
        // Using a DependencyProperty as the backing store for SelectedColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
            "SelectedColor",
            typeof(Color),
            typeof(ColorPickerControl),
            new PropertyMetadata(null));

        public ColorPickerControl()
        {
            this.InitializeComponent();
        }

        public Color SelectedColor
        {
            get { return (Color)this.GetValue(SelectedColorProperty); }
            set { this.SetValue(SelectedColorProperty, value); }
        }

        private void OnTappedColor(object sender, TappedRoutedEventArgs e)
        {
            var rect = sender as Rectangle;

            var color = rect.Name;

            switch (color)
            {
                case "White":
                    this.SelectedColor = Windows.UI.Colors.White.ToCaptionSettingsColor();
                    break;
                case "Black":
                    this.SelectedColor = Windows.UI.Colors.Black.ToCaptionSettingsColor();
                    break;
                case "Red":
                    this.SelectedColor = Windows.UI.Colors.Red.ToCaptionSettingsColor();
                    break;
                case "Green":
                    this.SelectedColor = Windows.UI.Colors.Green.ToCaptionSettingsColor();
                    break;
                case "Blue":
                    this.SelectedColor = Windows.UI.Colors.Blue.ToCaptionSettingsColor();
                    break;
                case "Yellow":
                    this.SelectedColor = Windows.UI.Colors.Yellow.ToCaptionSettingsColor();
                    break;
                case "Magenta":
                    this.SelectedColor = Windows.UI.Colors.Magenta.ToCaptionSettingsColor();
                    break;
                case "Cyan":
                    this.SelectedColor = Windows.UI.Colors.Cyan.ToCaptionSettingsColor();
                    break;
            }

            var flyoutPresenter = this.Parent as FlyoutPresenter;

            var popup = flyoutPresenter.Parent as Popup;

            popup.IsOpen = false;
        }
    }
}