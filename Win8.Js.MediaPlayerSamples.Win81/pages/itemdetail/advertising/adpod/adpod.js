(function () {
    "use strict";

    var mediaPlayer = null;

    var midrollAd = new PlayerFramework.Advertising.MidrollAdvertisement();
    midrollAd.source = new Microsoft.PlayerFramework.Js.Advertising.RemoteAdSource();
    midrollAd.source.type = Microsoft.VideoAdvertising.VastAdPayloadHandler.adType;
    midrollAd.source.uri = new Windows.Foundation.Uri("http://smf.blob.core.windows.net/samples/win8/ads/vast_adpod.xml");
    midrollAd.time = 5;

    WinJS.UI.Pages.define("/pages/itemdetail/advertising/adpod/adpod.html", {
        // This function is called whenever a user navigates to this page.
        // It populates the page with data and initializes the media player control.
        ready: function (element, options) {
            var item = options && options.item ? Data.resolveItemReference(options.item) : Data.items.getAt(0);
            element.querySelector(".titlearea .pagetitle").textContent = item.title;
            element.querySelector(".item-subtitle").textContent = item.subtitle;
            element.querySelector(".item-description").textContent = item.description;

            var mediaPlayerElement = element.querySelector("[data-win-control='PlayerFramework.MediaPlayer']");
            mediaPlayer = mediaPlayerElement.winControl;
            mediaPlayer.adSchedulerPlugin.advertisements.push(midrollAd);
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