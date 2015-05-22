#define CODE_ANALYSIS

using System.Linq;
using System;
using System.Diagnostics.CodeAnalysis;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Resources;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A plugin used to show the user that an error occurred.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Correctly named architectural pattern")]
    public sealed class ErrorPlugin : PluginBase
    {
        ErrorView errorView;
        Panel errorContainer;
        string errorText;

        /// <summary>
        /// Creates a new instance of ErrorPlugin.
        /// </summary>
        public ErrorPlugin()
        {
            errorText = DefaultErrorText;
        }

        static string DefaultErrorText
        {
            get
            {
                return MediaPlayer.GetResourceString("ErrorText"); 
            }
        }

        /// <summary>
        /// Gets or sets the style to be used for the ErrorView
        /// </summary>
        public Style ErrorViewStyle { get; set; }

        /// <summary>
        /// Gets or sets the text to be used in the ErrorView
        /// </summary>
        public string ErrorText
        {
            get
            {
                return errorText;
            }
            set
            {
                errorText = value;
                if (errorView != null) errorView.ErrorText = errorText;
            }
        }

        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            errorContainer = MediaPlayer.Containers.OfType<Panel>().FirstOrDefault(e => e.Name == MediaPlayerTemplateParts.ErrorsContainer);
            if (errorContainer != null)
            {
                errorView = new ErrorView()
                {
                    ErrorText = ErrorText
                };
                if (ErrorViewStyle != null) errorView.Style = ErrorViewStyle;
                errorView.Retry += errorViewElement_Retry;
                errorContainer.Children.Add(errorView);
                return true;
            }
            return false;
        }

        void errorViewElement_Retry(object sender, RoutedEventArgs e)
        {
            MediaPlayer.Retry();
        }

        /// <inheritdoc /> 
        protected override void OnDeactivate()
        {
            if (errorContainer != null)
            {
                if (errorView != null)
                {
                    errorView.Retry -= errorViewElement_Retry;
                    errorContainer.Children.Remove(errorView);
                    errorView = null;
                }
                errorContainer = null;
            }
        }
    }
}
