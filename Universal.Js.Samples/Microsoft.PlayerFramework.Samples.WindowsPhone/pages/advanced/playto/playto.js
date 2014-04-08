(function () {
    "use strict";

    var playToManager = null;
    var mediaPlayer = null;

    WinJS.UI.Pages.define("/pages/advanced/playto/playto.html", {
        // This function is called whenever a user navigates to this page.
        // It populates the page with data and initializes the media player control.
        ready: function (element, options) {
            var item = options && options.item ? Data.resolveItemReference(options.item) : Data.items.getAt(0);
            element.querySelector(".titlearea .pagetitle").textContent = item.title;
            if (WinJS.Utilities.isPhone) {
                document.getElementById("header").style.display = "none";
            }

            playToManager = Windows.Media.PlayTo.PlayToManager.getForCurrentView();
            playToManager.addEventListener("sourcerequested", onPlayToSourceRequested, false);
            playToManager.addEventListener("sourceselected", onPlayToSourceSelected, false);

            var mediaPlayerElement = element.querySelector("[data-win-control='PlayerFramework.MediaPlayer']");
            mediaPlayer = mediaPlayerElement.winControl;
            mediaPlayer.msPlayToSource.connection.addEventListener("statechanged", onPlayToConnectionStateChanged, false);
            mediaPlayer.focus();

            Windows.Media.PlayTo.PlayToManager.showPlayToUI();
        },

        // This function is called whenever a user navigates away from this page.
        // It resets the page and disposes of the media player control.
        unload: function () {
            if (playToManager) {
                playToManager.removeEventListener("sourcerequested", onPlayToSourceRequested, false);
                playToManager.removeEventListener("sourceselected", onPlayToSourceSelected, false);
                playToManager = null;
            }

            if (mediaPlayer) {
                mediaPlayer.msPlayToSource.connection.removeEventListener("statechanged", onPlayToConnectionStateChanged, false);
                mediaPlayer.dispose();
                mediaPlayer = null;
            }
        }
    });

    function onPlayToSourceRequested(e) {
        Utilities.log("Type: " + e.type + ", Deadline: " + e.sourceRequest.deadline);

        try {
            e.sourceRequest.setSource(mediaPlayer.msPlayToSource);
        } catch (error) {
            console.error(error.message);
        }
    }

    function onPlayToSourceSelected(e) {
        Utilities.log("Type: " + e.type + ", Friendly Name: " + e.friendlyName);
    }

    function onPlayToConnectionStateChanged(e) {
        Utilities.log("Type: " + e.type + ", Previous State: " + e.previousState + ", Current State: " + e.currentState);
    }
})();