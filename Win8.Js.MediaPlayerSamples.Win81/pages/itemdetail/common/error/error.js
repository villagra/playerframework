(function () {
    "use strict";

    var mediaPlayer = null;

    WinJS.UI.Pages.define("/pages/itemdetail/common/error/error.html", {
        // This function is called whenever a user navigates to this page.
        // It populates the page with data and initializes the media player control.
        ready: function (element, options) {
            var item = options && options.item ? Data.resolveItemReference(options.item) : Data.items.getAt(0);
            element.querySelector(".titlearea .pagetitle").textContent = item.title;
            element.querySelector(".item-subtitle").textContent = item.subtitle;
            element.querySelector(".item-description").textContent = item.description;

            var mediaPlayerElement = element.querySelector("[data-win-control='PlayerFramework.MediaPlayer']");
            mediaPlayer = mediaPlayerElement.winControl;
            mediaPlayer.addEventListener("error", onMediaPlayerError, false);
            mediaPlayer.focus();
        },

        // This function is called whenever a user navigates away from this page.
        // It resets the page and disposes of the media player control.
        unload: function () {
            if (mediaPlayer) {
                mediaPlayer.removeEventListener("error", onMediaPlayerError, false);
                mediaPlayer.dispose();
                mediaPlayer = null;
            }
        }
    });

    function onMediaPlayerError(e) {
        var error = e.detail.error;
        var message = PlayerFramework.Utilities.getMediaErrorMessage(error);
        var extendedCode = PlayerFramework.Utilities.convertDecimalToHex(error.msExtendedCode);

        if (extendedCode === "0x80004002") {
            // cancel any non-critical errors to prevent them from interrupting the player
            e.detail.canceled = true;
            console.warn(message);
        } else {
            // otherwise, allow the player to enter the failed state and display the error UI
            console.error(message);
        }
    }
})();