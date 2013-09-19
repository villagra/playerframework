(function () {
    "use strict";

    var appView = Windows.UI.ViewManagement.ApplicationView;
    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    var nav = WinJS.Navigation;
    var ui = WinJS.UI;

    ui.Pages.define("/pages/itemPlayer/itemPlayer.html", {
        // This function is called whenever a user navigates to this page.
        // It initializes the page and its controls.
        ready: function (element, options) {
            var item = options && options.item ? Data.resolveItemReference(options.item) : Data.items.getAt(0);

            // initialize media player
            mediaPlayer.winControl.playlistPlugin.playlist = Data.items.map(function (item) { return { data: item, src: item.videoSource }; });
            mediaPlayer.winControl.playlistPlugin.currentPlaylistItemIndex = Data.items.indexOf(item);
            mediaPlayer.winControl.focus();

            // initialize flyout
            aboutButton.winControl.flyout = aboutFlyout.winControl;

            // add back button click event listener
            this._backButtonClickEventListener = this._onBackButtonClick.bind(this);
            backButton.addEventListener("click", this._backButtonClickEventListener);

            // add window resize event listener
            this._windowResizeEventListener = this._onWindowResize.bind(this);
            window.addEventListener("resize", this._windowResizeEventListener);

            // initialize layout
            this._updateLayout();
        },

        // This function is called whenever a user navigates away from this page.
        // It uninitializes the page and its controls.
        unload: function () {
            // remove event listeners
            backButton.removeEventListener("click", this._backButtonClickEventListener);
            window.removeEventListener("resize", this._windowResizeEventListener);

            // fixes a focus issue with the back button
            topAppBar.winControl.hide();
            bottomAppBar.winControl.hide();

            // shutdown the media player and release all resources
            mediaPlayer.winControl.dispose();
        },

        _onBackButtonClick: function (args) {
            nav.back();
        },

        _onWindowResize: function (args) {
            this._updateLayout();
        },

        _updateLayout: function () {
            if (appView.value === appViewState.snapped) {
                mediaPlayer.winControl.isFullScreen = false;
            } else {
                mediaPlayer.winControl.isFullScreen = true;
            }
        }
    });
})();