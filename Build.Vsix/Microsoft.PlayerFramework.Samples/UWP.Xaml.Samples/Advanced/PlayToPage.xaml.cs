using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.PlayTo;
using Windows.UI.Core;
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
    public sealed partial class PlayToPage : Page
    {
        private PlayToManager ptm;
        private CoreDispatcher dispatcher = Window.Current.CoreWindow.Dispatcher;

        public PlayToPage()
        {
            this.InitializeComponent();
            ptm = PlayToManager.GetForCurrentView();
        }

        async private void SourceRequested(Windows.Media.PlayTo.PlayToManager sender, Windows.Media.PlayTo.PlayToSourceRequestedEventArgs e)
        {
            var deferral = e.SourceRequest.GetDeferral();
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                e.SourceRequest.SetSource(player.PlayToSource);
                deferral.Complete();
            });
        }
        
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ptm.SourceRequested -= SourceRequested;
            player.Dispose();
            base.OnNavigatedFrom(e);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ptm.SourceRequested += SourceRequested;
            base.OnNavigatedTo(e);
        }

        private void PlayTo_Click(object sender, RoutedEventArgs e)
        {            
            // programmatically invoke the Play To UI. In most cases you would just let the user choose this from the Devices charm instead.
            PlayToManager.ShowPlayToUI();
        }
    }
}
