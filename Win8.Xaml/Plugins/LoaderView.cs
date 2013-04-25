using System.ComponentModel;
using System.Windows.Input;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Automation;
#endif

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
#if SILVERLIGHT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
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
