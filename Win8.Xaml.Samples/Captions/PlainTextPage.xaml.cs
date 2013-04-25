using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.PlayerFramework.CaptionMarkers;
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
    public sealed partial class PlainTextPage : Microsoft.PlayerFramework.Samples.Common.LayoutAwarePage
    {
        CaptionsPlugin captionPlugin;

        public PlainTextPage()
        {
            this.InitializeComponent();
            this.Loaded += CaptionsPage_Loaded;
            captionPlugin = new CaptionsPlugin();
            player.Plugins.Add(captionPlugin);
        }

        void CaptionsPage_Loaded(object sender, RoutedEventArgs e)
        {
            player.Markers.Add(new TimelineMarker() { Type = captionPlugin.MarkerType, Text = "Hello World", Time = TimeSpan.FromSeconds(2) });
            player.Markers.Add(new TimelineMarker() { Type = captionPlugin.MarkerType, Text = "Hello Mars", Time = TimeSpan.FromSeconds(2.5) });
            player.Markers.Add(new TimelineMarker() { Type = captionPlugin.MarkerType, Text = "What a strange conversation!", Time = TimeSpan.FromSeconds(4.5) });
            player.Markers.Add(new TimelineMarker() { Type = captionPlugin.MarkerType, Text = "It must have written by a programmer.", Time = TimeSpan.FromSeconds(10) });      
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            player.Dispose();
            base.OnNavigatedFrom(e);
        }
    }
}
