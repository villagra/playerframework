// <copyright file="ColorPickerControl.xaml.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-14</date>
// <summary>Color Picker Control</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.Controls
{
    using System;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Shapes;

    /// <summary>
    /// Color picker control
    /// </summary>
    public sealed partial class ColorPickerControl : UserControl
    {
        /// <summary>
        /// the SelectedColor dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(
            "SelectedColor",
            typeof(Color),
            typeof(ColorPickerControl),
            new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the ColorPickerControl class.
        /// </summary>
        public ColorPickerControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Color selected event
        /// </summary>
        public event EventHandler<ColorEventArgs> ColorSelected;

        /// <summary>
        /// Gets or sets the selected color
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)this.GetValue(SelectedColorProperty); }
            set { this.SetValue(SelectedColorProperty, value); }
        }

        /// <summary>
        /// select the tapped color
        /// </summary>
        /// <param name="sender">a color rectangle</param>
        /// <param name="e">the tapped routed event arguments</param>
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

            if (this.ColorSelected != null)
            {
                this.ColorSelected(this, new ColorEventArgs(this.SelectedColor));
            }
        }
    }
}