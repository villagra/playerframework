﻿// <copyright file="ColorPickerControl.xaml.cs" company="Microsoft Corporation">
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
        #region Fields
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
        /// The color type dependency property
        /// </summary>
        public static readonly DependencyProperty ColorTypeProperty =
            DependencyProperty.Register("ColorType", typeof(ColorType), typeof(ColorPickerControl), new PropertyMetadata(ColorType.Default));
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the ColorPickerControl class.
        /// </summary>
        public ColorPickerControl()
        {
            this.InitializeComponent();
        }
        #endregion

        #region Events
        /// <summary>
        /// Color selected event
        /// </summary>
        public event EventHandler<ColorEventArgs> ColorSelected;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the selected color
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)this.GetValue(SelectedColorProperty); }
            set { this.SetValue(SelectedColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the current color type
        /// </summary>
        public ColorType ColorType
        {
            get { return (ColorType)this.GetValue(ColorTypeProperty); }
            set { this.SetValue(ColorTypeProperty, value); }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// select the tapped color
        /// </summary>
        /// <param name="sender">a color rectangle</param>
        /// <param name="e">the tapped routed event arguments</param>
        private void OnTappedColor(object sender, TappedRoutedEventArgs e)
        {
            byte transparency = 255;

            switch (this.ColorType)
            {
                case Model.ColorType.Default:
                    transparency = 255;
                    break;
                case Model.ColorType.Semitransparent:
                    transparency = 127;
                    break;

                case Model.ColorType.Solid:
                    transparency = 255;
                    break;

                case Model.ColorType.Transparent:
                    transparency = 0;
                    break;
            }

            var rect = sender as Rectangle;

            var color = rect.Name;

            switch (color)
            {
                case "White":
                    this.SelectedColor = Windows.UI.Colors.White.ToCaptionSettingsColor(transparency);
                    break;
                case "Black":
                    this.SelectedColor = Windows.UI.Colors.Black.ToCaptionSettingsColor(transparency);
                    break;
                case "Red":
                    this.SelectedColor = Windows.UI.Colors.Red.ToCaptionSettingsColor(transparency);
                    break;
                case "Green":
                    this.SelectedColor = Windows.UI.Colors.Green.ToCaptionSettingsColor(transparency);
                    break;
                case "Blue":
                    this.SelectedColor = Windows.UI.Colors.Blue.ToCaptionSettingsColor(transparency);
                    break;
                case "Yellow":
                    this.SelectedColor = Windows.UI.Colors.Yellow.ToCaptionSettingsColor(transparency);
                    break;
                case "Magenta":
                    this.SelectedColor = Windows.UI.Colors.Magenta.ToCaptionSettingsColor(transparency);
                    break;
                case "Cyan":
                    this.SelectedColor = Windows.UI.Colors.Cyan.ToCaptionSettingsColor(transparency);
                    break;
            }

            if (this.ColorSelected != null)
            {
                this.ColorSelected(this, new ColorEventArgs(this.SelectedColor));
            }
        }
        #endregion
    }
}