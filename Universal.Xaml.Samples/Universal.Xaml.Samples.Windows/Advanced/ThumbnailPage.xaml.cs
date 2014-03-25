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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Microsoft.PlayerFramework.Samples.Common;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Microsoft.PlayerFramework.Samples
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ThumbnailPage : Page
    {
        private NavigationHelper navigationHelper;

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }
        string currentThumbnailUrl;
        const string thumbnailUriPattern = "http://smf.blob.core.windows.net/samples/thumbs/BBB/BigBuckBunny_{0:0000}.jpg";

        public ThumbnailPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            player.VirtualPositionChanged += player_VirtualPositionChanged;
            player.RateChanged += player_RateChanged;
            player.IsScrubbingChanged += player_IsScrubbingChanged;
        }

        void player_IsScrubbingChanged(object sender, RoutedEventArgs e)
        {
            player.IsThumbnailVisible = (player.IsScrubbing || player.PlaybackRate < -1 || player.PlaybackRate > 1);
        }

        void player_RateChanged(object sender, RateChangedRoutedEventArgs e)
        {
            player.IsThumbnailVisible = (player.IsScrubbing || player.PlaybackRate <= -1 || player.PlaybackRate > 1);
        }

        void player_VirtualPositionChanged(object sender, RoutedPropertyChangedEventArgs<TimeSpan> e)
        {
            if (player.IsThumbnailVisible)
            {
                int roundedPosition = (int)(Math.Round(e.NewValue.TotalSeconds) / 5) * 5;
                var thumbnailUrl = string.Format(thumbnailUriPattern, roundedPosition);
                if (thumbnailUrl != currentThumbnailUrl)
                {
                    currentThumbnailUrl = thumbnailUrl;
                    var thumbnailUri = new Uri(thumbnailUrl);
                    player.ThumbnailImageSource = new BitmapImage(thumbnailUri);
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            backButton.Command = this.navigationHelper.GoBackCommand;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            player.Dispose();
            base.OnNavigatedFrom(e);
        }
    }
}
