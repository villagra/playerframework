(function () {
    "use strict";

    var appView = Windows.UI.ViewManagement.ApplicationView;
    var appViewState = Windows.UI.ViewManagement.ApplicationViewState;
    var nav = WinJS.Navigation;
    var ui = WinJS.UI;

    ui.Pages.define("/pages/itemDetail/itemDetail.html", {
        // This function is called whenever a user navigates to this page.
        // It initializes the page and its controls.
        ready: function (element, options) {
            var item = options && options.item ? Data.resolveItemReference(options.item) : Data.items.getAt(0);
            
            // add play button click event listener
            this._playButtonClickEventListener = this._onPlayButtonClick.bind(this, item);
            playButton.addEventListener("click", this._playButtonClickEventListener);

            // process bindings
            WinJS.Binding.processAll(element, item);

            // set initial focus
            element.querySelector(".content").focus();
        },

        // This function is called whenever a user navigates away from this page.
        // It uninitializes the page and its controls.
        unload: function () {
            // remove event listeners
            playButton.removeEventListener("click", this._playButtonClickEventListener);
        },

        // Navigates to the player page with the current item.
        _onPlayButtonClick: function (item, args) {
            nav.navigate("/pages/itemPlayer/itemPlayer.html", { item: Data.getItemReference(item) });
        }
    });
})();