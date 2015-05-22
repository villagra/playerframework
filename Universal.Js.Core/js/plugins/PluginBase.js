(function (PlayerFramework, undefined) {
    "use strict";

    // PluginBase Errors
    var invalidConstruction = "Invalid construction: PluginBase constructor must be called using the \"new\" operator.",
        invalidState = "Invalid state: The state of the plugin is invalid for the requested operation.";

    // PluginBase Class
    var PluginBase = WinJS.Class.define(function (options) {
        if (!(this instanceof PlayerFramework.PluginBase)) {
            throw invalidConstruction;
        }

        this._isEnabled = false;
        this._isLoaded = false;
        this._isActive = false;
        this._mediaPlayer = null;
        this._currentMediaSource = null;
        this._activePromises = [];
        this._observablePlugin = WinJS.Binding.as(this);

        this._setOptions(options);
    }, {
        // Public Properties
        isEnabled: {
            get: function () {
                return this._isEnabled;
            },
            set: function (value) {
                var oldValue = this._isEnabled;
                if (oldValue !== value) {
                    this._isEnabled = value;
                    this._observablePlugin.notify("isEnabled", value, oldValue);

                    if (this.isLoaded) {
                        if (value) {
                            this._activate();
                        } else {
                            this._deactivate();
                        }
                    }
                }
            }
        },

        isLoaded: {
            get: function () {
                return this._isLoaded;
            }
        },

        isActive: {
            get: function () {
                return this._isActive;
            }
        },

        mediaPlayer: {
            get: function () {
                return this._mediaPlayer;
            },
            set: function (value) {
                this._mediaPlayer = value;
            }
        },

        currentMediaSource: {
            get: function () {
                return this._currentMediaSource;
            }
        },

        // Public Methods
        load: function () {
            if (this.isLoaded || !this.mediaPlayer) {
                throw invalidState;
            }

            this._onLoad();
            this._currentMediaSource = this.mediaPlayer;
            this._isLoaded = true;

            if (this.isEnabled) {
                this._activate();
            }
        },

        unload: function () {
            if (!this.isLoaded) {
                throw invalidState;
            }

            this._deactivate();
            this._currentMediaSource = null;
            this._onUnload();
            this._isLoaded = false;
        },

        update: function (mediaSource) {
            if (this.isLoaded) {
                this._cancelActivePromises();
                this._currentMediaSource = mediaSource;
                this._onUpdate();
            }
        },

        // Private Methods
        _setOptions: function (options) {
            PlayerFramework.Utilities.setOptions(this, options, {
                isEnabled: true
            });
        },

        _cancelActivePromises: function () {
            for (var i = 0; i < this._activePromises.length; i++) {
                var promise = this._activePromises[i];
                promise.cancel();
            }

            this._activePromises = [];
        },

        _activate: function () {
            if (!this.isActive) {
                this._isActive = this._onActivate();
            }
        },

        _deactivate: function () {
            if (this.isActive) {
                this._cancelActivePromises();
                this._onDeactivate();
                this._isActive = false;
            }
        },

        _onLoad: function () {
        },

        _onUnload: function () {
        },

        _onActivate: function () {
            return true;
        },

        _onDeactivate: function () {
        },

        _onUpdate: function () {
        }
    });

    // PluginBase Mixins
    WinJS.Class.mix(PluginBase, WinJS.Utilities.eventMixin);
    WinJS.Class.mix(PluginBase, PlayerFramework.Utilities.eventBindingMixin);
    WinJS.Class.mix(PluginBase, PlayerFramework.Utilities.propertyBindingMixin);
    
    // PluginBase Exports
    WinJS.Namespace.define("PlayerFramework", {
        PluginBase: PluginBase
    });

})(PlayerFramework);

