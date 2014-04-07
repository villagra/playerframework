(function () {
    "use strict";

    var mediaPlayer = null;
    var observableMediaPlayer = null;
    var currentThumbnailUrl = null;

    WinJS.UI.Pages.define("/pages/advanced/thumbnails/thumbnails.html", {
        // This function is called whenever a user navigates to this page.
        // It populates the page with data and initializes the media player control.
        ready: function (element, options) {
            var item = options && options.item ? Data.resolveItemReference(options.item) : Data.items.getAt(0);
            element.querySelector(".titlearea .pagetitle").textContent = item.title;
            if (WinJS.Utilities.isPhone) {
                document.getElementById("backButton").style.display = "none";
            }
            
            var mediaPlayerElement = element.querySelector("[data-win-control='PlayerFramework.MediaPlayer']");
            this.mediaPlayer = mediaPlayerElement.winControl;
            this.mediaPlayer.focus();

            this.observableMediaPlayer = WinJS.Binding.as(this.mediaPlayer);
            this.observableMediaPlayer.bind("virtualTime", this._onVirtualPositionChanged.bind(this));
            this.observableMediaPlayer.bind("scrubbing", this._onModeChanged.bind(this));
            this.observableMediaPlayer.bind("playbackRate", this._onModeChanged.bind(this));
        },

        _onModeChanged: function () {
            this.mediaPlayer.isThumbnailVisible = this.mediaPlayer.scrubbing || (this.mediaPlayer.playbackRate !== 0 && this.mediaPlayer.playbackRate !== 1);
        },

        _onVirtualPositionChanged: function () {
            if (this.mediaPlayer.isThumbnailVisible)
            {
                var roundedPosition = Math.floor(Math.round(this.mediaPlayer.virtualTime) / 5) * 5;
                var positionId = roundedPosition.toString();
                while (positionId.length < 4) positionId = "0" + positionId;

                var thumbnailUrl = "http://smf.blob.core.windows.net/samples/thumbs/BBB/BigBuckBunny_" + positionId + ".jpg";
                if (thumbnailUrl !== currentThumbnailUrl)
                {
                    currentThumbnailUrl = thumbnailUrl;
                    this.mediaPlayer.thumbnailImageSrc = thumbnailUrl;
                }
            }
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