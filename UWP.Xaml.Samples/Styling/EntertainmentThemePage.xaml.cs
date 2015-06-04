using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.PlayerFramework.Samples.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Microsoft.PlayerFramework.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EntertainmentThemePage : Page
    {

        public EntertainmentThemePage()
        {
            this.InitializeComponent();

            MediaControls.SetBehavior(ReplayButton, player.CreateMediaControlBehavior<ReplayButtonBehavior>());
            MediaControls.SetBehavior(CaptionSelectionButton, player.CreateMediaControlBehavior<CaptionSelectionButtonBehavior>());
            MediaControls.SetBehavior(AudioSelectionButton, player.CreateMediaControlBehavior<AudioSelectionButtonBehavior>());

            // register the control panel so it participates in view state changes
            player.Initialized += player_Initialized;
        }

        void player_Initialized(object sender, RoutedEventArgs e)
        {
            var audioSelectionPlugin = player.Plugins.OfType<AudioSelectionPlugin>().FirstOrDefault();
            audioSelectionPlugin.AudioSelectionViewStyle = new Style(typeof(AudioSelectionView));
            audioSelectionPlugin.AudioSelectionViewStyle.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(0, 0, 0, 90)));

            var captionSelectorPlugin = player.Plugins.OfType<CaptionSelectorPlugin>().FirstOrDefault();
            captionSelectorPlugin.CaptionSelectorViewStyle = new Style(typeof(CaptionSelectorView));
            captionSelectorPlugin.CaptionSelectorViewStyle.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(0, 0, 0, 90)));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            player.Initialized -= player_Initialized;

            player.Dispose();
            base.OnNavigatedFrom(e);
        }
    }
}
