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
using Microsoft.PlayerFramework.Adaptive;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Microsoft.PlayerFramework.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CffProgressivePage : Microsoft.PlayerFramework.Samples.Common.LayoutAwarePage
    {
        public CffProgressivePage()
        {
            this.InitializeComponent();
            var adaptivePlugin = new Microsoft.PlayerFramework.Adaptive.AdaptivePlugin();
            player.Plugins.Add(adaptivePlugin);
            adaptivePlugin.DownloaderPlugin = new Microsoft.Media.AdaptiveStreaming.Dash.CffProgressiveDownloaderPlugin();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            player.Dispose();
            base.OnNavigatedFrom(e);
        }
    }
}
