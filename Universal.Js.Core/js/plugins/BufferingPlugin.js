(function (PlayerFramework, undefined) {
    "use strict";

    // BufferingPlugin Errors
    var invalidConstruction = "Invalid construction: BufferingPlugin constructor must be called using the \"new\" operator.";

    // BufferingPlugin Class
    var BufferingPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.BufferingPlugin)) {
            throw invalidConstruction;
        }

        this._bufferingContainerElement = null;
        this._bufferingControlElement = null;
        this._isLoadingAd = false;
        this._isLoadingMediaPlayer = false;

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Public Methods
        show: function () {
            this.mediaPlayer.addClass("pf-show-buffering-container");
        },

        hide: function () {
            this.mediaPlayer.removeClass("pf-show-buffering-container");
        },

        // Private Methods
        _setElement: function () {
            this._bufferingContainerElement = PlayerFramework.Utilities.createElement(this.mediaPlayer.element, ["div", { "class": "pf-buffering-container" }]);
            this._bufferingControlElement = PlayerFramework.Utilities.createElement(this._bufferingContainerElement, ["progress", { "class": "pf-buffering-control" }]);
        },

        _onActivate: function () {
            this._setElement();

            this._bindEvent("advertisingstatechange", this.mediaPlayer, this._onMediaPlayerAdvertisingStateChange);
            this._bindEvent("playerstatechange", this.mediaPlayer, this._onMediaPlayerPlayerStateChange);
            this._bindEvent("canplaythrough", this.mediaPlayer, this._onMediaPlayerCanPlayThrough);

            if (this.mediaPlayer.advertisingState === PlayerFramework.AdvertisingState.loading || this.mediaPlayer.playerState === PlayerFramework.PlayerState.loading || this.mediaPlayer.playerState === PlayerFramework.PlayerState.loaded) {
                this.show();
            }

            return true;
        },

        _onDeactivate: function () {
            this.hide();

            this._unbindEvent("advertisingstatechange", this.mediaPlayer, this._onMediaPlayerAdvertisingStateChange);
            this._unbindEvent("playerstatechange", this.mediaPlayer, this._onMediaPlayerPlayerStateChange);
            this._unbindEvent("canplaythrough", this.mediaPlayer, this._onMediaPlayerCanPlayThrough);

            PlayerFramework.Utilities.removeElement(this._bufferingContainerElement);

            this._bufferingContainerElement = null;
            this._bufferingControlElement = null;
            this._isLoadingAd = false;
            this._isLoadingMediaPlayer = false;
        },

        _onMediaPlayerAdvertisingStateChange: function (e) {
            switch (this.mediaPlayer.advertisingState) {
                case PlayerFramework.AdvertisingState.loading:
                    this._isLoadingAd = true;
                    break;

                default:
                    this._isLoadingAd = false;
                    break;
            }

            this._updateVisibility();
        },

        _onMediaPlayerPlayerStateChange: function (e) {
            switch (this.mediaPlayer.playerState) {
                case PlayerFramework.PlayerState.loading:
                case PlayerFramework.PlayerState.loaded:
                    this._isLoadingMediaPlayer = true;
                    break;

                case PlayerFramework.PlayerState.unloaded:
                case PlayerFramework.PlayerState.pending:
                case PlayerFramework.PlayerState.ending:
                case PlayerFramework.PlayerState.ended:
                case PlayerFramework.PlayerState.failed:
                    this._isLoadingMediaPlayer = false;
                    break;
            }

            this._updateVisibility();
        },

        _onMediaPlayerCanPlayThrough: function (e) {
            this._isLoadingMediaPlayer = false;
            this._updateVisibility();
        },

        _updateVisibility: function () {
            if (this._isLoadingAd || this._isLoadingMediaPlayer) {
                this.show();
            } else {
                this.hide();
            }
        }
    });

    // BufferingPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        bufferingPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // BufferingPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        BufferingPlugin: BufferingPlugin
    });

})(PlayerFramework);

