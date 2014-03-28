(function (PlayerFramework, undefined) {
    "use strict";

    // ErrorPlugin Errors
    var invalidConstruction = "Invalid construction: ErrorPlugin constructor must be called using the \"new\" operator.";

    // ErrorPlugin Class
    var ErrorPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.ErrorPlugin)) {
            throw invalidConstruction;
        }

        this._errorContainerElement = null;
        this._errorTextElement = null;
        this._errorControlElement = null;
        this._errorLabelElement = null;

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Public Methods
        show: function () {
            this.mediaPlayer.addClass("pf-show-error-container");
        },

        hide: function () {
            this.mediaPlayer.removeClass("pf-show-error-container");
        },

        // Private Methods
        _setElement: function () {
            this._errorContainerElement = PlayerFramework.Utilities.createElement(this.mediaPlayer.element, ["div", { "class": "pf-error-container" }]);
            this._errorTextElement = PlayerFramework.Utilities.createElement(this._errorContainerElement, ["div", { "class": "pf-error-text" }, PlayerFramework.Utilities.getResourceString("ErrorText")]);
            this._errorControlElement = PlayerFramework.Utilities.createElement(this._errorContainerElement, ["button", { "type": "button", "class": "pf-error-control", "data-win-control": "PlayerFramework.UI.Button", "data-win-options": JSON.stringify({ "content": PlayerFramework.Utilities.getResourceString("ErrorIcon"), "label": PlayerFramework.Utilities.getResourceString("ErrorLabel"), "tooltip": PlayerFramework.Utilities.getResourceString("ErrorTooltip") }) }]);
            this._errorLabelElement = PlayerFramework.Utilities.createElement(this._errorContainerElement, ["div", { "class": "pf-error-label" }, PlayerFramework.Utilities.getResourceString("ErrorLabel")]);

            WinJS.UI.processAll(this._errorContainerElement);
        },

        _onActivate: function () {
            this._setElement();

            this._bindEvent("playerstatechange", this.mediaPlayer, this._onMediaPlayerPlayerStateChange);
            this._bindEvent("click", this._errorControlElement.winControl, this._onErrorControlClick);

            if (this.mediaPlayer.playerState === PlayerFramework.PlayerState.failed) {
                this.show();
            }

            return true;
        },

        _onDeactivate: function () {
            this.hide();

            this._unbindEvent("playerstatechange", this.mediaPlayer, this._onMediaPlayerPlayerStateChange);
            this._unbindEvent("click", this._errorControlElement.winControl, this._onErrorControlClick);

            PlayerFramework.Utilities.removeElement(this._errorContainerElement);

            this._errorContainerElement = null;
            this._errorTextElement = null;
            this._errorControlElement = null;
            this._errorLabelElement = null;
        },

        _onMediaPlayerPlayerStateChange: function (e) {
            if (this.mediaPlayer.playerState === PlayerFramework.PlayerState.failed) {
                this.show();
            } else {
                this.hide();
            }
        },

        _onErrorControlClick: function (e) {
            this.mediaPlayer.retry();
        }
    });

    // ErrorPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        errorPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // ErrorPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        ErrorPlugin: ErrorPlugin
    });

})(PlayerFramework);

