using System.Linq;
using System;
using System.Diagnostics.CodeAnalysis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A plugin used to allow the user 
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Correctly named architectural pattern")]
    public sealed class AudioSelectionPlugin : PluginBase
    {
        /// <summary>
        /// Gets or sets the style to be used for the CaptionSelectorView
        /// </summary>
        public Style AudioSelectionViewStyle { get; set; }

        private AudioSelectionView selectorView;

        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            MediaPlayer.AudioSelectionInvoked += MediaPlayer_AudioSelectionInvoked;
            return true;
        }

        /// <inheritdoc /> 
        protected override void OnDeactivate()
        {
            MediaPlayer.AudioSelectionInvoked -= MediaPlayer_AudioSelectionInvoked;
            OnClose();
        }

        InteractionType deactivationMode;
        void MediaPlayer_AudioSelectionInvoked(object sender, RoutedEventArgs e)
        {
            if (MediaPlayer.AvailableAudioStreams.Any())
            {
                selectorView = new AudioSelectionView();
                if (AudioSelectionViewStyle != null) selectorView.Style = AudioSelectionViewStyle;
                selectorView.SetBinding(FrameworkElement.DataContextProperty, new Binding() { Path = new PropertyPath("InteractiveViewModel"), Source = MediaPlayer });
                selectorView.Close += SelectorView_Close;
                deactivationMode = MediaPlayer.InteractiveDeactivationMode;
                MediaPlayer.InteractiveDeactivationMode = InteractionType.None;
            }
        }

        void SelectorView_Close(object sender, EventArgs e)
        {
            OnClose();
        }

        private void OnClose()
        {
            if (selectorView != null)
            {
                selectorView.Close -= SelectorView_Close;
                selectorView.Visibility = Visibility.Collapsed;
                MediaPlayer.InteractiveDeactivationMode = deactivationMode;
                selectorView = null;
            }
        }
    }
}
