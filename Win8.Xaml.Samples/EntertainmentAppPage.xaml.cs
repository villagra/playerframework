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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Microsoft.PlayerFramework.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EntertainmentAppPage : Microsoft.PlayerFramework.Samples.Common.LayoutAwarePage
    {
        public EntertainmentAppPage()
        {
            this.InitializeComponent();

            MediaControls.SetBehavior(ReplayButton, player.CreateMediaControlBehavior<ReplayButtonBehavior>());
            MediaControls.SetBehavior(CaptionSelectionButton, player.CreateMediaControlBehavior<CaptionSelectionButtonBehavior>());
            MediaControls.SetBehavior(AudioSelectionButton, player.CreateMediaControlBehavior<AudioSelectionButtonBehavior>());

            ReplayButton.Loaded += StartLayoutUpdates;
            ReplayButton.Unloaded += StopLayoutUpdates;
            CaptionSelectionButton.Loaded += StartLayoutUpdates;
            CaptionSelectionButton.Unloaded += StopLayoutUpdates;
            AudioSelectionButton.Loaded += StartLayoutUpdates;
            AudioSelectionButton.Unloaded += StopLayoutUpdates;

            // register the control panel so it participates in view state changes
            player.Initialized += player_Initialized;
        }

        void player_Initialized(object sender, RoutedEventArgs e)
        {
            player.ControlPanel.Loaded += StartLayoutUpdates;
            player.ControlPanel.Unloaded += StopLayoutUpdates;

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
            player.ControlPanel.Loaded -= StartLayoutUpdates;
            player.ControlPanel.Unloaded -= StopLayoutUpdates;
            player.Dispose();
            base.OnNavigatedFrom(e);
        }
    }
}
