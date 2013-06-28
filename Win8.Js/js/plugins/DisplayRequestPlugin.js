(function (PlayerFramework, undefined) {
    "use strict";

    // DisplayRequestPlugin Errors
    var invalidConstruction = "Invalid construction: DisplayRequestPlugin constructor must be called using the \"new\" operator.";

    // DisplayRequestPlugin Class
    var DisplayRequestPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.DisplayRequestPlugin)) {
            throw invalidConstruction;
        }

        this._keepActiveWhilePaused = false;
        this._isRequestActive = false;
        this._displayRequest = null;

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Public Properties
        isRequestActive: {
            get: function () {
                return this._isRequestActive;
            }
        },

        // Private Methods
        _requestActive: function () {
            if (!this._isRequestActive) {
                this._getDisplayRequest().requestActive();
                this._isRequestActive = true;
            }
        },

        _requestRelease: function () {
            if (this._isRequestActive) {
                this._getDisplayRequest().requestRelease();
                this._isRequestActive = false;
                this._displayRequest = null;
            }
        },

        _getDisplayRequest: function () {
            if (this._displayRequest == null) {
                this._displayRequest = new Windows.System.Display.DisplayRequest();
            }
            return this._displayRequest;
        },

        _onActivate: function () {
            if (!this.mediaPlayer.paused) {
                this._requestActive();
            }
            this._bindEvent("emptied", this.mediaPlayer, this._onMediaPlayerPause);
            this._bindEvent("error", this.mediaPlayer, this._onMediaPlayerPause);
            this._bindEvent("ended", this.mediaPlayer, this._onMediaPlayerPause);
            this._bindEvent("pause", this.mediaPlayer, this._onMediaPlayerPause);
            this._bindEvent("playing", this.mediaPlayer, this._onMediaPlayerPlaying);

            return true;
        },

        _onDeactivate: function () {
            this._requestRelease();
            this._unbindEvent("emptied", this.mediaPlayer, this._onMediaPlayerPause);
            this._unbindEvent("error", this.mediaPlayer, this._onMediaPlayerPause);
            this._unbindEvent("ended", this.mediaPlayer, this._onMediaPlayerPause);
            this._unbindEvent("pause", this.mediaPlayer, this._onMediaPlayerPause);
            this._unbindEvent("playing", this.mediaPlayer, this._onMediaPlayerPlaying);
        },

        _onMediaPlayerPause: function (e) {
            this._requestRelease();
        },

        _onMediaPlayerPlaying: function (e) {
            this._requestActive();
        },
    });

    // DisplayRequestPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        DisplayRequestPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // DisplayRequestPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        DisplayRequestPlugin: DisplayRequestPlugin
    });

})(PlayerFramework);

