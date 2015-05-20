using Microsoft.PlayerFramework.CaptionMarkers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace Microsoft.PlayerFramework.Win10.Sample.XAML.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CaptionsProgrammatic : Page
    {
        public CaptionsProgrammatic()
        {
            this.InitializeComponent();
			this.Loaded += CaptionsProgrammatic_Loaded;
        }

		private void CaptionsProgrammatic_Loaded(object sender, RoutedEventArgs e)
		{
			var plugin = player.Plugins.FirstOrDefault(p => p.GetType() == typeof(CaptionsPlugin));
			if(plugin != null)
			{
				var captionPlugin = plugin as CaptionsPlugin;
                player.Markers.Add(new TimelineMarker() { Type = captionPlugin.MarkerType, Text = "Hello World", Time = TimeSpan.FromSeconds(2) });
				player.Markers.Add(new TimelineMarker() { Type = captionPlugin.MarkerType, Text = "Hello Mars", Time = TimeSpan.FromSeconds(2.5) });
				player.Markers.Add(new TimelineMarker() { Type = captionPlugin.MarkerType, Text = "What a strange conversation!", Time = TimeSpan.FromSeconds(4.5) });
				player.Markers.Add(new TimelineMarker() { Type = captionPlugin.MarkerType, Text = "It must have written by a programmer.", Time = TimeSpan.FromSeconds(10) });
			}
		}

		private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
