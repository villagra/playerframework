using System.ComponentModel;
using System.Windows.Input;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Automation;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Automation;
#endif


namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A control that indicates an error has occurred.
    /// </summary>
    public class ErrorView : Control
    {
        /// <summary>
        /// Occurs when a button control is clicked.
        /// </summary>
        public event RoutedEventHandler Retry;

        /// <summary>
        /// Creates a new instance of the control
        /// </summary>
        public ErrorView()
        {
            this.DefaultStyleKey = typeof(ErrorView);
            
            RetryText = MediaPlayer.GetResourceString("RetryButtonLabel");
            ErrorText = MediaPlayer.GetResourceString("RetryInstructionsText");
        }

        /// <inheritdoc /> 
#if SILVERLIGHT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();


            var RetryButton = GetTemplateChild("RetryButton") as Button;
            if (RetryButton != null)
            {
                RetryButton.Click += RetryButton_Click;
            }
        }

        private void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            if (Retry != null) Retry(this, e);
        }

        /// <summary>
        /// ErrorText DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty ErrorTextProperty = DependencyProperty.Register("ErrorText", typeof(string), typeof(ErrorView), null);

        /// <summary>
        /// Gets or sets the error text to be displayed to the user.
        /// </summary>
        public string ErrorText
        {
            get { return GetValue(ErrorTextProperty) as string; }
            set { SetValue(ErrorTextProperty, value); }
        }

        /// <summary>
        /// RetryText DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty RetryTextProperty = DependencyProperty.Register("RetryText", typeof(string), typeof(ErrorView), null);

        /// <summary>
        /// Gets or sets the error text to be displayed to the user.
        /// </summary>
        public string RetryText
        {
            get { return GetValue(RetryTextProperty) as string; }
            set { SetValue(RetryTextProperty, value); }
        }
    }
}
