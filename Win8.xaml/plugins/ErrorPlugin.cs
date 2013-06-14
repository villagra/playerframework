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
#if MEF
    [System.ComponentModel.Composition.PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [System.ComponentModel.Composition.Export(typeof(IPlugin))]
#endif
    public sealed class ErrorPlugin : PluginBase
    {
        ErrorView errorViewElement;
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
#if SILVERLIGHT
                return Microsoft.PlayerFramework.Resources.ErrorText;
#else
                return MediaPlayer.GetResourceString("ErrorText"); 
#endif
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
                if (errorViewElement != null) errorViewElement.ErrorText = errorText;
            }
        }

        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            errorContainer = MediaPlayer.Containers.OfType<Panel>().FirstOrDefault(e => e.Name == MediaPlayerTemplateParts.ErrorsContainer);
            if (errorContainer != null)
            {
                errorViewElement = new ErrorView()
                {
                    Style = ErrorViewStyle,
                    ErrorText = ErrorText
                };
                errorViewElement.Retry += errorViewElement_Retry;
                errorContainer.Children.Add(errorViewElement);
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
                if (errorViewElement != null)
                {
                    errorViewElement.Retry -= errorViewElement_Retry;
                    errorContainer.Children.Remove(errorViewElement);
                    errorViewElement = null;
                }
                errorContainer = null;
            }
        }
    }
}
