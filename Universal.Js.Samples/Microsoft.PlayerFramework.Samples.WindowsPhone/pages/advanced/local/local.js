(function () {
    "use strict";

    var mediaPlayer = null;

    WinJS.UI.Pages.define("/pages/advanced/local/local.html", {
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
            mediaPlayer.focus();

            var openFileButtonElement = element.querySelector(".item-content button:nth-child(1)");
            openFileButtonElement.onclick = onOpenFileButtonClick;

            var openWebcamButtonElement = element.querySelector(".item-content button:nth-child(2)");
            openWebcamButtonElement.onclick = onOpenWebcamButtonClick;

            var resetButtonElement = element.querySelector(".item-content button:nth-child(3)");
            resetButtonElement.onclick = onResetButtonClick;
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

    function onOpenFileButtonClick(e) {
        var dialog = new Windows.Storage.Pickers.FileOpenPicker();
        dialog.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.videosLibrary;
        dialog.fileTypeFilter.replaceAll([".avi", ".mp4", ".mpeg", ".wmv"]);
        dialog.pickSingleFileAsync().done(onFileSelected);
    }

    function onOpenWebcamButtonClick(e) {
        var dialog = new Windows.Media.Capture.CameraCaptureUI();
        dialog.videoSettings.format = Windows.Media.Capture.CameraCaptureUIVideoFormat.mp4;
        dialog.captureFileAsync(Windows.Media.Capture.CameraCaptureUIMode.video).done(onFileSelected);
    }

    function onResetButtonClick(e) {
        mediaPlayer.src = null;
    }

    function onFileSelected(file) {
        if (file) {
            mediaPlayer.src = URL.createObjectURL(file);
        }
    }
})();