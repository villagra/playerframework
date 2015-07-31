(function () {
    "use strict";

    var mediaPlayer = null;

    var prerollAd = new PlayerFramework.Advertising.PrerollAdvertisement();
    prerollAd.source = new Microsoft.PlayerFramework.Js.Advertising.RemoteAdSource();
    prerollAd.source.type = Microsoft.Media.Advertising.VastAdPayloadHandler.adType;
    prerollAd.source.uri = new Windows.Foundation.Uri("http://smf.blob.core.windows.net/samples/win8/ads/vast_linear.xml");
    prerollAd.source.headers.insert("User-Agent", Microsoft.Media.Advertising.Extensions.defaultUserAgent);

    var midrollAd = new PlayerFramework.Advertising.MidrollAdvertisement();
    midrollAd.source = new Microsoft.PlayerFramework.Js.Advertising.RemoteAdSource();
    midrollAd.source.type = Microsoft.Media.Advertising.VastAdPayloadHandler.adType;
    midrollAd.source.uri = new Windows.Foundation.Uri("http://smf.blob.core.windows.net/samples/win8/ads/vast_linear.xml");
    midrollAd.source.headers.insert("User-Agent", Microsoft.Media.Advertising.Extensions.defaultUserAgent);
    midrollAd.time = 15;

    var postrollAd = new PlayerFramework.Advertising.PostrollAdvertisement();
    postrollAd.source = new Microsoft.PlayerFramework.Js.Advertising.RemoteAdSource();
    postrollAd.source.type = Microsoft.Media.Advertising.VastAdPayloadHandler.adType;
    postrollAd.source.uri = new Windows.Foundation.Uri("http://smf.blob.core.windows.net/samples/win8/ads/vast_linear.xml");
    postrollAd.source.headers.insert("User-Agent", Microsoft.Media.Advertising.Extensions.defaultUserAgent);

    WinJS.UI.Pages.define("/pages/advertising/vast/vast.html", {
        // This function is called whenever a user navigates to this page.
        // It populates the page with data and initializes the media player control.
        ready: function (element, options) {
            var mediaPlayerElement = element.querySelector("[data-win-control='PlayerFramework.MediaPlayer']");
            mediaPlayer = mediaPlayerElement.winControl;
            mediaPlayer.adSchedulerPlugin.advertisements.push(prerollAd);
            mediaPlayer.adSchedulerPlugin.advertisements.push(midrollAd);
            mediaPlayer.adSchedulerPlugin.advertisements.push(postrollAd);
            mediaPlayer.focus();
        },

        // This function is called whenever a user navigates away from this page.
        // It resets the page and disposes of the media player control.
        unload: function () {
            if (mediaPlayer) {
                mediaPlayer.dispose();
                mediaPlayer = null;
            }
        }
    });
})();