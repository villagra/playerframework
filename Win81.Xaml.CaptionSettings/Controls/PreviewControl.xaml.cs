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
        /// <summary>
        /// the font style
        /// </summary>
        private FontStyle fontStyle;

        /// <summary>
        /// Initializes a new instance of the PreviewControl class.
        /// </summary>
        public PreviewControl()
        {
            this.InitializeComponent();

            this.ViewModel = new PreviewControlViewModel();

            this.Loaded += this.PreviewControl_Loaded;
        }

        /// <summary>
        /// Sets the Caption Font Style when the control is loaded.
        /// </summary>
        /// <param name="sender">the preview control</param>
        /// <param name="e">the routed event arguments</param>
        private void PreviewControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.CaptionFontStyle = Model.FontStyle.Default;
        }

        /// <summary>
        /// Gets or sets the caption background brush
        /// </summary>
        public Brush CaptionBackground
        {
            get 
            { 
                return (Brush)GetValue(CaptionBackgroundProperty); 
            }

            set 
            { 
                SetValue(CaptionBackgroundProperty, value); 
            }
        }

        // Using a DependencyProperty as the backing store for CaptionBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaptionBackgroundProperty =
            DependencyProperty.Register("CaptionBackground", typeof(Brush), typeof(PreviewControl), new PropertyMetadata(null, OnCaptionBackgroundChanged));

        public Brush WindowColor
        {
            get { return (Brush)this.GetValue(WindowColorProperty); }
            set { this.SetValue(WindowColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WindowColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WindowColorProperty =
            DependencyProperty.Register("WindowColor", typeof(Brush), typeof(PreviewControl), new PropertyMetadata(null, OnWindowColorChanged));

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

        /// <summary>
        /// Gets or sets the caption font style
        /// </summary>
        public FontStyle CaptionFontStyle
        {
            get
            {
                return this.fontStyle;
            }

            set
            {
                this.fontStyle = value;

                this.GoToFontStyleState();
            }
        }

        /// <summary>
        /// Go to the font style state
        /// </summary>
        private async void GoToFontStyleState()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                VisualStateManager.GoToState(this, this.CaptionFontStyle.ToString(), true);
            }
            else
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    new Windows.UI.Core.DispatchedHandler(delegate
                    {
                        var stateName = this.CaptionFontStyle.ToString();

                        System.Diagnostics.Debug.WriteLine("State Changing to {0}", stateName);

                        var stateChanged = VisualStateManager.GoToState(this, stateName, true);

                        System.Diagnostics.Debug.WriteLine("State Changed to {0}: {1}", stateName, stateChanged);
                    }));
            }
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

        private static void OnWindowColorChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var previewControl = dependencyObject as PreviewControl;

            previewControl.PreviewWindow.Background = previewControl.WindowColor;
        }
    }
}
