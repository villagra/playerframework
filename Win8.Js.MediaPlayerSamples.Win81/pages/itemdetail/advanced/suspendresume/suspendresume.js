(function () {
    "use strict";

    var mediaPlayer = null;
    var mediaPlayerStateKey = "mediaPlayerState";

    WinJS.UI.Pages.define("/pages/itemdetail/advanced/suspendresume/suspendresume.html", {
        // This function is called whenever a user navigates to this page.
        // It populates the page with data and initializes the media player control.
        ready: function (element, options) {
            var item = options && options.item ? Data.resolveItemReference(options.item) : Data.items.getAt(0);
            element.querySelector(".titlearea .pagetitle").textContent = item.title;
            element.querySelector(".item-subtitle").textContent = item.subtitle;
            element.querySelector(".item-description").textContent = item.description;

            var mediaPlayerElement = element.querySelector("[data-win-control='PlayerFramework.MediaPlayer']");
            mediaPlayer = mediaPlayerElement.winControl;
            mediaPlayer.focus();

            WinJS.Application.addEventListener("checkpoint", onApplicationCheckpoint, false);
            Windows.UI.WebUI.WebUIApplication.addEventListener("suspending", onApplicationSuspending, false);
            Windows.UI.WebUI.WebUIApplication.addEventListener("resuming", onApplicationResuming, false);

            if (options && options.activatedArgs && options.activatedArgs.detail.previousExecutionState === Windows.ApplicationModel.Activation.ApplicationExecutionState.terminated) {
                onApplicationReactivated(options.activatedArgs);
            }
        },

        // This function is called whenever a user navigates away from this page.
        // It resets the page and disposes of the media player control.
        unload: function () {
            WinJS.Application.removeEventListener("checkpoint", onApplicationCheckpoint, false);
            Windows.UI.WebUI.WebUIApplication.removeEventListener("suspending", onApplicationSuspending, false);
            Windows.UI.WebUI.WebUIApplication.removeEventListener("resuming", onApplicationResuming, false);

            if (mediaPlayer) {
                mediaPlayer.dispose();
                mediaPlayer = null;
            }
        }
    });

    function onApplicationReactivated(e) {
        Utilities.log("Application reactivated after suspension");
        
        var mediaPlayerState = WinJS.Application.sessionState[mediaPlayerStateKey];

        if (mediaPlayerState) {
            var currentPlaylistItem = mediaPlayer.playlistPlugin.playlist[mediaPlayerState.playlistPlugin.currentPlaylistItemIndex];

            if (currentPlaylistItem) {
                mediaPlayerState = PlayerFramework.Utilities.clone(currentPlaylistItem, mediaPlayerState);
            }

            Utilities.log("--- Media player state restored");
            mediaPlayer.update(mediaPlayerState);
        }
    }

    function onApplicationCheckpoint(e) {
        Utilities.log("Application checkpoint occurred");
        
        if (mediaPlayer.playerState !== PlayerFramework.PlayerState.failed) {
            var mediaPlayerState = {
                src: mediaPlayer.src,
                autoload: mediaPlayer.playerState > PlayerFramework.PlayerState.pending ? true : mediaPlayer.autoload,
                autoplay: mediaPlayer.playerState > PlayerFramework.PlayerState.starting ?  !mediaPlayer.paused : mediaPlayer.autoplay,
                startupTime: mediaPlayer.playerState > PlayerFramework.PlayerState.starting ? mediaPlayer.currentTime : mediaPlayer.startupTime,
                playlistPlugin: {
                    currentPlaylistItemIndex: mediaPlayer.playlistPlugin.currentPlaylistItemIndex
                }
            };

            Utilities.log("--- Media player state saved");
            WinJS.Application.sessionState[mediaPlayerStateKey] = mediaPlayerState;
        }
    }

    function onApplicationSuspending(e) {
        Utilities.log("Application suspending");
        
        if (mediaPlayer.playerState === PlayerFramework.PlayerState.started) {
            Utilities.log("--- Media playback paused");
            mediaPlayer.interactiveViewModel.pause();
        }
    }

    function onApplicationResuming(e) {
        Utilities.log("Application resuming");
        
        if (mediaPlayer.playerState === PlayerFramework.PlayerState.started) {
            Utilities.log("--- Media playback resumed");
            mediaPlayer.interactiveViewModel.playResume();
        }
    }
})();