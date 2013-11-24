// <copyright file="SettingsControl.cs" company="Michael S. Scherotter">
// Copyright (c) 2011 Michael S. Scherotter All Rights Reserved
// </copyright>
// <author>Michael S. Scherotter</author>
// <email>synergist@charette.com</email>
// <date>2012-10-25</date>
// <summary>Settings Control</summary>

namespace Microsoft.PlayerFramework.CaptionSettings.Controls
{
    using Windows.UI.ApplicationSettings;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Settings pane
    /// </summary>
    public class SettingsControl : ContentControl
    {
        #region Fields
        /// <summary>
        /// The Label dependency property
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(SettingsControl), new PropertyMetadata(string.Empty));
        #endregion

        /// <summary>
        /// Initializes a new instance of the SettingsControl class.
        /// </summary>
        public SettingsControl()
        {
            this.DefaultStyleKey = typeof(SettingsControl);
        }

        /// <summary>
        /// Gets or sets the setting pane label
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { this.SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// setup the back button event handler
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var backButton = this.GetTemplateChild("BackButton") as Button;

            if (backButton != null)
            {
                backButton.Click += this.OnBackButtonClick;
            }
        }

        /// <summary>
        /// Show the settings pane when the back button is clicked.
        /// </summary>
        /// <param name="sender">the button</param>
        /// <param name="e">the routed event arguments</param>
        private void OnBackButtonClick(object sender, RoutedEventArgs e)
        {
            SettingsPane.Show();
        }
    }
}
