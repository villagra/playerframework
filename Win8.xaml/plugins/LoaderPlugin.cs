#define CODE_ANALYSIS

using System.Linq;
using System;
using System.Diagnostics.CodeAnalysis;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A plugin used to display a UI with a play button on it to let the user choose to load the media.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Correctly named architectural pattern")]
    public sealed class LoaderPlugin : PluginBase
    {
        LoaderView loaderView;
        Panel loaderViewContainer;

        /// <summary>
        /// Gets or sets the style to be used for the ErrorView
        /// </summary>
        public Style LoaderViewStyle { get; set; }

        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            loaderViewContainer = MediaPlayer.Containers.OfType<Panel>().FirstOrDefault(e => e.Name == MediaPlayerTemplateParts.LoaderViewContainer);
            if (loaderViewContainer != null)
            {
                loaderView = new LoaderView();
                if (LoaderViewStyle != null) loaderView.Style = LoaderViewStyle;
                loaderView.Load += loaderViewElement_Load;
                loaderViewContainer.Children.Add(loaderView);
                return true;
            }
            return false;
        }

        void loaderViewElement_Load(object sender, RoutedEventArgs e)
        {
            MediaPlayer.AutoLoad = true;
        }

        /// <inheritdoc /> 
        protected override void OnDeactivate()
        {
            if (loaderViewContainer != null)
            {
                if (loaderView != null)
                {
                    loaderView.Load -= loaderViewElement_Load;
                    loaderViewContainer.Children.Remove(loaderView);
                    loaderView = null;
                }
                loaderViewContainer = null;
            }
        }
    }
}
