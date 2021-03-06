﻿(function () {
    "use strict";

    var mediaPlayer = null;

    WinJS.UI.Pages.define("/pages/advanced/suspendresume/suspendresume.html", {
        // This function is called whenever a user navigates to this page.
        // It populates the page with data and initializes the media player control.
        ready: function (element, options) {
            var mediaPlayerElement = element.querySelector("[data-win-control='PlayerFramework.MediaPlayer']");
            mediaPlayer = mediaPlayerElement.winControl;
            mediaPlayer.focus();

            WinJS.Application.addEventListener("checkpoint", onApplicationCheckpoint, false);

            if (WinJS.Application.sessionState.mediaPlayerState) {
                onApplicationReactivated(WinJS.Application.sessionState.mediaPlayerState);
            }
        },

        // This function is called whenever a user navigates away from this page.
        // It resets the page and disposes of the media player control.
        unload: function () {
            WinJS.Application.removeEventListener("checkpoint", onApplicationCheckpoint, false);

            if (mediaPlayer) {
                mediaPlayer.dispose();
                mediaPlayer = null;
            }
        }
    });

    function onApplicationReactivated(mediaPlayerState) {
        Utilities.log("Application reactivated after suspension");

        var currentPlaylistItem = mediaPlayer.playlistPlugin.playlist[mediaPlayerState.playlistPlugin.currentPlaylistItemIndex];

        if (currentPlaylistItem) {
            mediaPlayerState = PlayerFramework.Utilities.clone(currentPlaylistItem, mediaPlayerState);
        }

        Utilities.log("--- Media player state restored");
        mediaPlayer.update(mediaPlayerState);
    }

    function onApplicationCheckpoint(e) {
        Utilities.log("Application checkpoint occurred");

        if (mediaPlayer.playerState !== PlayerFramework.PlayerState.failed) {
            var mediaPlayerState = {
                src: mediaPlayer.src,
                autoload: mediaPlayer.playerState > PlayerFramework.PlayerState.pending ? true : mediaPlayer.autoload,
                autoplay: mediaPlayer.playerState > PlayerFramework.PlayerState.starting ? !mediaPlayer.paused : mediaPlayer.autoplay,
                startupTime: mediaPlayer.playerState > PlayerFramework.PlayerState.starting ? mediaPlayer.currentTime : mediaPlayer.startupTime,
                playlistPlugin: {
                    currentPlaylistItemIndex: mediaPlayer.playlistPlugin.currentPlaylistItemIndex
                }
            };

            Utilities.log("--- Media player state saved");
            WinJS.Application.sessionState.mediaPlayerState = mediaPlayerState;
        }
    }
})();