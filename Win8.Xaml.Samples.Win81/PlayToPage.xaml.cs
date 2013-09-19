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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Microsoft.PlayerFramework.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlayToPage : Microsoft.PlayerFramework.Samples.Common.LayoutAwarePage
    {
        private PlayToManager ptm;
        private CoreDispatcher dispatcher = Window.Current.CoreWindow.Dispatcher;

        public PlayToPage()
        {
            this.InitializeComponent();
            ptm = PlayToManager.GetForCurrentView();
            ptm.SourceRequested += SourceRequested;

            this.Loaded += PlayToPage_Loaded;
        }

        void PlayToPage_Loaded(object sender, RoutedEventArgs e)
        {
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
            this.Loaded -= PlayToPage_Loaded;
            ptm.SourceRequested -= SourceRequested;
            player.Dispose();
            base.OnNavigatedFrom(e);
        }

        private void PlayTo_Click(object sender, RoutedEventArgs e)
        {            
            // programmatically invoke the Play To UI. In most cases you would just let the user choose this from the Devices charm instead.
            PlayToManager.ShowPlayToUI();
        }
    }
}
