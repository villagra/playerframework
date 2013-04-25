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
    public sealed partial class MarkersPage : Microsoft.PlayerFramework.Samples.Common.LayoutAwarePage
    {
        public MarkersPage()
        {
            this.InitializeComponent();
            player.VisualMarkers.Add(new VisualMarker() { Text = "Test 1", Time = TimeSpan.FromSeconds(5) });
            player.VisualMarkers.Add(new VisualMarker() { Text = "Test 2", Time = TimeSpan.FromSeconds(18) });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            player.Dispose();
            base.OnNavigatedFrom(e);
        }
    }
}
