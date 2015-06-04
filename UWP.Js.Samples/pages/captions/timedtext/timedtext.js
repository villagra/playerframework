(function () {
    "use strict";

    var mediaPlayer = null;

    WinJS.UI.Pages.define("/pages/captions/timedtext/timedtext.html", {
        // This function is called whenever a user navigates to this page.
        // It populates the page with data and initializes the media player control.
        ready: function (element, options) {
            var mediaPlayerElement = element.querySelector("[data-win-control='PlayerFramework.MediaPlayer']");
            mediaPlayer = mediaPlayerElement.winControl;
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