// <copyright file="PreviewControl.xaml.cs" company="Microsoft Corporation">
// Copyright (c) 2013 Microsoft Corporation All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>mischero@microsoft.com</email>
// <date>2013-11-14</date>
// <summary>Caption Preview Control</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.Controls
{
    using System;
    using Microsoft.PlayerFramework.CaptionSettings.Model;
    using Microsoft.PlayerFramework.CaptionSettings.ViewModel;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// the preview control
    /// </summary>
    public sealed partial class PreviewControl : UserControl
    {
        #region Fields
        /// <summary>
        /// WindowColor dependency property
        /// </summary>
        public static readonly DependencyProperty WindowColorProperty =
            DependencyProperty.Register("WindowColor", typeof(Brush), typeof(PreviewControl), new PropertyMetadata(null, OnWindowColorChanged));

        /// <summary>
        /// CaptionBackground dependency property
        /// </summary>
        public static readonly DependencyProperty CaptionBackgroundProperty =
            DependencyProperty.Register("CaptionBackground", typeof(Brush), typeof(PreviewControl), new PropertyMetadata(null, OnCaptionBackgroundChanged));

        /// <summary>
        /// Caption Font Style dependency property
        /// </summary>
        public static readonly DependencyProperty CaptionFontStyleProperty =
            DependencyProperty.Register(
                "CaptionFontStyle",
                typeof(FontStyle),
                typeof(PreviewControl),
                new PropertyMetadata(Model.FontStyle.Default, new PropertyChangedCallback(OnFontStyleChanged)));

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the PreviewControl class.
        /// </summary>
        public PreviewControl()
        {
            this.InitializeComponent();

            this.ViewModel = new PreviewControlViewModel();

            this.Loaded += this.PreviewControl_Loaded;
        }
        #endregion

        /// <summary>
        /// Gets or sets the caption background brush
        /// </summary>
        public Brush CaptionBackground
        {
            get
            {
                return (Brush)this.GetValue(CaptionBackgroundProperty);
            }

            set
            {
                this.SetValue(CaptionBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the window color
        /// </summary>
        public Brush WindowColor
        {
            get { return (Brush)this.GetValue(WindowColorProperty); }
            set { this.SetValue(WindowColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the caption font style
        /// </summary>
        public FontStyle CaptionFontStyle
        {
            get { return (FontStyle)this.GetValue(CaptionFontStyleProperty); }
            set { this.SetValue(CaptionFontStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the outline width
        /// </summary>
        public double OutlineWidth
        {
            get
            {
                return this.ViewModel.OutlineWidth;
            }

            set
            {
                this.ViewModel.OutlineWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the preview text
        /// </summary>
        public string PreviewText
        {
            get
            {
                return this.ViewModel.PreviewText;
            }

            set
            {
                this.Center.Text = value;
                this.ViewModel.PreviewText = value;
            }
        }

        /// <summary>
        /// Gets or sets the preview control view model
        /// </summary>
        private PreviewControlViewModel ViewModel
        {
            get
            {
                return this.LayoutRoot.DataContext as PreviewControlViewModel;
            }

            set
            {
                this.LayoutRoot.DataContext = value;
            }
        }

        #region Implementation

        /// <summary>
        /// go to the font style state when the font style changes
        /// </summary>
        /// <param name="sender">the preview control</param>
        /// <param name="args">the dependency property changed event arguments</param>
        private static void OnFontStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var previewControl = sender as PreviewControl;

            previewControl.GoToFontStyleState();
        }

        /// <summary>
        /// Sets the preview background when the caption background changes
        /// </summary>
        /// <param name="dependencyObject">the PreviewControl</param>
        /// <param name="args">the dependency property changed event arguments</param>
        private static void OnCaptionBackgroundChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs args)
        {
            var previewControl = dependencyObject as PreviewControl;

            previewControl.PreviewBackground.Background = previewControl.CaptionBackground;
        }

        /// <summary>
        /// Update the window color in the control when the dependency property changes
        /// </summary>
        /// <param name="dependencyObject">the preview control</param>
        /// <param name="args">the dependency property changed event arguments</param>
        private static void OnWindowColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var previewControl = dependencyObject as PreviewControl;

            previewControl.PreviewWindow.Background = previewControl.WindowColor;
        }

        /// <summary>
        /// Go to the font style state
        /// </summary>
        /// <param name="useTransitions">use transitions for the state changes</param>
        private async void GoToFontStyleState(bool useTransitions = false)
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                VisualStateManager.GoToState(this, this.CaptionFontStyle.ToString(), useTransitions);
            }
            else
            {
                await this.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    new DispatchedHandler(delegate
                    {
                        var stateName = this.CaptionFontStyle.ToString();

                        ////System.Diagnostics.Debug.WriteLine("Changing Preview state to {0}", stateName);

                        var stateChanged = VisualStateManager.GoToState(this, stateName, useTransitions);

                        System.Diagnostics.Debug.WriteLineIf(!stateChanged, "Could not change preview state to " + stateName);
                    }));
            }
        }

        /// <summary>
        /// Sets the Caption Font Style when the control is loaded.
        /// </summary>
        /// <param name="sender">the preview control</param>
        /// <param name="e">the routed event arguments</param>
        private void PreviewControl_Loaded(object sender, RoutedEventArgs e)
        {
            ////this.CaptionFontStyle = Model.FontStyle.Default;
        }

        #endregion
    }
}
