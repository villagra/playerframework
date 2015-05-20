(function (PlayerFramework, undefined) {
    "use strict";

    // LoaderPlugin Errors
    var invalidConstruction = "Invalid construction: LoaderPlugin constructor must be called using the \"new\" operator.";

    // LoaderPlugin Class
    var LoaderPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.LoaderPlugin)) {
            throw invalidConstruction;
        }

        this._loaderContainerElement = null;
        this._loaderControlElement = null;

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Public Methods
        show: function () {
            this.mediaPlayer.addClass("pf-show-loader-container");
        },

        hide: function () {
            this.mediaPlayer.removeClass("pf-show-loader-container");
        },

        // Private Methods
        _setElement: function () {
            this._loaderContainerElement = PlayerFramework.Utilities.createElement(this.mediaPlayer.element, ["div", { "class": "pf-loader-container" }]);
            this._loaderControlElement = PlayerFramework.Utilities.createElement(this._loaderContainerElement, ["button", { "type": "button", "class": "pf-loader-control", "data-win-control": "PlayerFramework.UI.Button", "data-win-options": JSON.stringify({ "content": PlayerFramework.Utilities.getResourceString("LoaderIcon"), "label": PlayerFramework.Utilities.getResourceString("LoaderLabel"), "tooltip": PlayerFramework.Utilities.getResourceString("LoaderTooltip") }) }]);

            WinJS.UI.processAll(this._loaderContainerElement);
        },

        _onActivate: function () {
            this._setElement();

            this._bindEvent("playerstatechange", this.mediaPlayer, this._onMediaPlayerPlayerStateChange);
            this._bindEvent("click", this._loaderControlElement.winControl, this._onLoaderControlClick);

            if (!this.mediaPlayer.autoload && (this.mediaPlayer.playerState === PlayerFramework.PlayerState.unloaded || this.mediaPlayer.playerState === PlayerFramework.PlayerState.pending)) {
                this.show();
            }

            return true;
        },

        _onDeactivate: function () {
            this.hide();

            this._unbindEvent("playerstatechange", this.mediaPlayer, this._onMediaPlayerPlayerStateChange);
            this._unbindEvent("click", this._loaderControlElement.winControl, this._onLoaderControlClick);

            PlayerFramework.Utilities.removeElement(this._loaderContainerElement);

            this._loaderContainerElement = null;
            this._loaderControlElement = null;
        },

        _onMediaPlayerPlayerStateChange: function (e) {
            if (!this.mediaPlayer.autoload && (this.mediaPlayer.playerState === PlayerFramework.PlayerState.unloaded || this.mediaPlayer.playerState === PlayerFramework.PlayerState.pending)) {
                this.show();
            } else {
                this.hide();
            }
        },

        _onLoaderControlClick: function (e) {
            this.mediaPlayer.load();
        }
    });

    // LoaderPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        loaderPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // LoaderPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        LoaderPlugin: LoaderPlugin
    });

})(PlayerFramework);

