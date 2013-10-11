#define CODE_ANALYSIS

using System.Linq;
using System;
using System.Diagnostics.CodeAnalysis;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Generic;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A plugin used to allow the user 
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Correctly named architectural pattern")]
    public sealed class CaptionSelectorPlugin : PluginBase
    {
        /// <summary>
        /// Gets or sets the style to be used for the CaptionSelectorView
        /// </summary>
        public Style CaptionSelectorViewStyle { get; set; }

        CaptionSelectorView captionSelectorView;

        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            MediaPlayer.CaptionsInvoked += MediaPlayer_CaptionsInvoked;
            return true;
        }

        /// <inheritdoc /> 
        protected override void OnDeactivate()
        {
            MediaPlayer.CaptionsInvoked -= MediaPlayer_CaptionsInvoked;
            OnClose();
        }

        Panel SettingsContainer
        {
            get
            {
                return MediaPlayer.Containers.OfType<Panel>().FirstOrDefault(c => c.Name == MediaPlayerTemplateParts.SettingsContainer);
            }
        }

        InteractionType deactivationMode;
        void MediaPlayer_CaptionsInvoked(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.AvailableCaptions.Any())
            {
                captionSelectorView = new CaptionSelectorView();
                if (CaptionSelectorViewStyle != null) captionSelectorView.Style = CaptionSelectorViewStyle;
                captionSelectorView.SetBinding(FrameworkElement.DataContextProperty, new Binding() { Path = new PropertyPath("InteractiveViewModel"), Source = MediaPlayer });
                SettingsContainer.Visibility = Visibility.Visible;
                SettingsContainer.Children.Add(captionSelectorView);
                captionSelectorView.Close += captionSelectorView_Close;
                deactivationMode = MediaPlayer.InteractiveDeactivationMode;
                MediaPlayer.InteractiveDeactivationMode = InteractionType.None;
            }
        }

        void captionSelectorView_Close(object sender, EventArgs e)
        {
            OnClose();
        }

        private void OnClose()
        {
            if (captionSelectorView != null)
            {
                captionSelectorView.Close -= captionSelectorView_Close;
                captionSelectorView.Visibility = Visibility.Collapsed;
                SettingsContainer.Children.Remove(captionSelectorView);
                SettingsContainer.Visibility = Visibility.Collapsed;
                MediaPlayer.InteractiveDeactivationMode = deactivationMode;
                captionSelectorView = null;
            }
        }
    }
}
