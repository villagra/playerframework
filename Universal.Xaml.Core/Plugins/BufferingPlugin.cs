#define CODE_ANALYSIS

using System.Linq;
using System;
using System.Diagnostics.CodeAnalysis;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A plugin used to show the user that the media is buffering.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Correctly named architectural pattern")]
    public sealed class BufferingPlugin : PluginBase
    {
        BufferingView bufferingView;
        Panel bufferingContainer;

        /// <summary>
        /// Gets or sets the style to be used for the BufferingView
        /// </summary>
        public Style BufferingViewStyle { get; set; }

        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            bufferingContainer = MediaPlayer.Containers.OfType<Panel>().FirstOrDefault(e => e.Name == MediaPlayerTemplateParts.BufferingContainer);
            if (bufferingContainer != null)
            {
                bufferingView = new BufferingView();
                if (BufferingViewStyle != null) bufferingView.Style = BufferingViewStyle;
                bufferingContainer.Children.Add(bufferingView);
#if SILVERLIGHT && !WINDOWS_PHONE
                bufferingElement.SetBinding(BufferingView.ViewModelProperty, new Binding() { Path = new PropertyPath("InteractiveViewModel"), Source = MediaPlayer });
#else
                MediaPlayer.InteractiveViewModelChanged += MediaPlayer_InteractiveViewModelChanged;
                bufferingView.ViewModel = MediaPlayer.InteractiveViewModel;
#endif
                return true;
            }
            return false;
        }

        /// <inheritdoc /> 
        protected override void OnDeactivate()
        {
#if SILVERLIGHT && !WINDOWS_PHONE
            bufferingElement.ClearValue(BufferingView.ViewModelProperty);
#else
            MediaPlayer.InteractiveViewModelChanged -= MediaPlayer_InteractiveViewModelChanged;
            bufferingView.ViewModel = null;
#endif
            bufferingContainer.Children.Remove(bufferingView);
            bufferingView = null;
            bufferingContainer = null;
        }

#if !SILVERLIGHT || WINDOWS_PHONE
        void MediaPlayer_InteractiveViewModelChanged(object sender, RoutedPropertyChangedEventArgs<IInteractiveViewModel> e)
        {
            if (bufferingView != null)
            {
                bufferingView.ViewModel = MediaPlayer.InteractiveViewModel;
            }
        }
#endif
    }
}
