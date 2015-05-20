using System.ComponentModel;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Automation;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A control that is used to kick off the loading of media.
    /// </summary>
    public class LoaderView : Control
    {
        /// <summary>
        /// Occurs when a button control is clicked.
        /// </summary>
        public event RoutedEventHandler Load;

        /// <summary>
        /// Creates a new instance of the control
        /// </summary>
        public LoaderView()
        {
            this.DefaultStyleKey = typeof(LoaderView);
        }

        /// <inheritdoc /> 
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var LoadButton = GetTemplateChild("LoadButton") as Button;
            if (LoadButton != null)
            {
                AutomationProperties.SetName(LoadButton, MediaPlayer.GetResourceString("LoadButtonLabel"));
                LoadButton.Click += LoadButton_Click;
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (Load != null) Load(this, e);
        }
    }
}
