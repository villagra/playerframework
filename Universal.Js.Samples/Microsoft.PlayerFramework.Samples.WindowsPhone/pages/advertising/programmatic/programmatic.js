(function () {
    "use strict";

    var mediaPlayer = null;

    var creativeCompanions = Microsoft.Media.Advertising.AdModelFactory.createCreativeCompanions();
    creativeCompanions.sequence = 1;

    var companion1 = Microsoft.Media.Advertising.AdModelFactory.createCompanion();
    companion1.item = Microsoft.Media.Advertising.AdModelFactory.createStaticResource();
    companion1.item.value = new Windows.Foundation.Uri("http://smf.blob.core.windows.net/samples/ads/media/6.jpg");
    companion1.item.creativeType = "image/jpeg";
    companion1.adSlotId = "companionAd1";
    creativeCompanions.companions.append(companion1);

    var companion2 = Microsoft.Media.Advertising.AdModelFactory.createCompanion();
    companion2.item = Microsoft.Media.Advertising.AdModelFactory.createStaticResource();
    companion2.item.value = new Windows.Foundation.Uri("http://smf.blob.core.windows.net/samples/ads/media/1.jpg");
    companion2.item.creativeType = "image/jpeg";
    companion2.adSlotId = "companionAd2";
    creativeCompanions.companions.append(companion2);

    var creativeLinear = Microsoft.Media.Advertising.AdModelFactory.createCreativeLinear();
    creativeLinear.sequence = 1;
    creativeLinear.duration = 10000;
    creativeLinear.clickThrough = new Windows.Foundation.Uri("http://clk.atdmt.com/000/sgo/135109908/direct;ct.1/01/633718663702241839");

    var mediaFile = Microsoft.Media.Advertising.AdModelFactory.createMediaFile();
    mediaFile.value = new Windows.Foundation.Uri("http://smf.blob.core.windows.net/samples/ads/media/XBOX_HD_DEMO_700_2_000_700_4x3.wmv");
    mediaFile.type = "video/x-ms-wmv";
    creativeLinear.mediaFiles.append(mediaFile);

    var icon1 = Microsoft.Media.Advertising.AdModelFactory.createIcon();
    icon1.item = Microsoft.Media.Advertising.AdModelFactory.createStaticResource();
    icon1.item.value = new Windows.Foundation.Uri("http://smf.blob.core.windows.net/samples/win8/ads/media/6.jpg");
    icon1.item.creativeType = "image/jpeg";
    icon1.width = 300;
    icon1.height = 60;
    icon1.program = "program1";
    icon1.viewTracking.append("http://view.example.com/1");
    icon1.clickTracking.append("http://click.example.com/1");
    icon1.clickThrough = new Windows.Foundation.Uri("http://clk.atdmt.com/000/sgo/135109908/direct;ct.1/01/633718663702241839");
    icon1.offset = 2000;
    creativeLinear.icons.append(icon1);

    var icon2 = Microsoft.Media.Advertising.AdModelFactory.createIcon();
    icon2.item = Microsoft.Media.Advertising.AdModelFactory.createStaticResource();
    icon2.item.value = new Windows.Foundation.Uri("http://smf.blob.core.windows.net/samples/win8/ads/media/1.jpg");
    icon2.item.creativeType = "image/jpeg";
    icon2.xposition = "right";
    icon2.yposition = "bottom";
    icon2.duration = 5000;
    creativeLinear.icons.append(icon2);

    var creativeNonLinears = Microsoft.Media.Advertising.AdModelFactory.createCreativeNonLinears();
    creativeNonLinears.sequence = 2;

    var nonLinear = Microsoft.Media.Advertising.AdModelFactory.createNonLinear();
    nonLinear.item = Microsoft.Media.Advertising.AdModelFactory.createStaticResource();
    nonLinear.item.value = new Windows.Foundation.Uri("http://smf.blob.core.windows.net/samples/ads/media/1.jpg");
    nonLinear.item.creativeType = "image/jpeg";
    nonLinear.clickThrough = new Windows.Foundation.Uri("http://clk.atdmt.com/000/sgo/135109908/direct;ct.1/01/633718663702241839");
    nonLinear.minSuggestedDuration = 5000;
    creativeNonLinears.nonLinears.append(nonLinear);

    var ad = Microsoft.Media.Advertising.AdModelFactory.createAd();
    ad.creatives.append(creativeCompanions);
    ad.creatives.append(creativeLinear);
    ad.creatives.append(creativeNonLinears);

    var adPod = Microsoft.Media.Advertising.AdModelFactory.createAdPod();
    adPod.ads.append(ad);

    var adSource = new Microsoft.PlayerFramework.Js.Advertising.AdSource();
    adSource.type = Microsoft.Media.Advertising.DocumentAdPayloadHandler.adType;
    adSource.payload = Microsoft.Media.Advertising.AdModelFactory.createAdDocumentPayload();
    adSource.payload.adPods.append(adPod);

    var adPromise = null;

    WinJS.UI.Pages.define("/pages/advertising/programmatic/programmatic.html", {
        // This function is called whenever a user navigates to this page.
        // It populates the page with data and initializes the media player control.
        ready: function (element, options) {
            var item = options && options.item ? Data.resolveItemReference(options.item) : Data.items.getAt(0);
            element.querySelector(".titlearea .pagetitle").textContent = item.title;
            if (WinJS.Utilities.isPhone) {
                document.getElementById("header").style.display = "none";
            }

            var mediaPlayerElement = element.querySelector("[data-win-control='PlayerFramework.MediaPlayer']");
            mediaPlayer = mediaPlayerElement.winControl;
            mediaPlayer.addEventListener("timeupdate", onMediaPlayerTimeUpdate, false);
            mediaPlayer.focus();
        },

        // This function is called whenever a user navigates away from this page.
        // It resets the page and disposes of the media player control.
        unload: function () {
            if (adPromise) {
                adPromise.cancel();
                adPromise = null;
            }

            if (mediaPlayer) {
                mediaPlayer.removeEventListener("timeupdate", onMediaPlayerTimeUpdate, false);
                mediaPlayer.dispose();
                mediaPlayer = null;
            }
        }
    });

    function onMediaPlayerTimeUpdate(e) {
        if (mediaPlayer.currentTime >= 5) {
            mediaPlayer.removeEventListener("timeupdate", onMediaPlayerTimeUpdate, false);
            adPromise = mediaPlayer.adHandlerPlugin.playAd(adSource).then(
                function (result) {
                    console.log("Ad played.");
                },
                function (result) {
                    if (result && result.message === "Canceled") {
                        console.log("Ad canceled.");
                    } else {
                        console.error("Ad failed.");
                    }
                }
            );
        }
    }
})();