(function (PlayerFramework, undefined) {
    "use strict";

    // AnalyticsPlugin Errors
    var invalidConstruction = "Invalid construction: AnalyticsPlugin constructor must be called using the \"new\" operator.";

    // AnalyticsPlugin Class
    var AnalyticsPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.AnalyticsPlugin)) {
            throw invalidConstruction;
        }

        this._collector = null;
        this._playerMonitor = null;
        this._environmentMonitor = null;
        this._edgeServerMonitor = null;
        this._adaptiveMonitor = null;
        this._analyticsConfig = null;
        this._sessionData = [];
        this._mediaData = [];
        this._mediaPlayerAdapter = null;

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Public Properties
        environmentMonitor: {
            get: function () {
                return this._environmentMonitor;
            },
            set: function (value) {
                this._environmentMonitor = value;
            }
        },

        edgeServerMonitor: {
            get: function () {
                return this._edgeServerMonitor;
            },
            set: function (value) {
                this._edgeServerMonitor = value;
            }
        },

        adaptiveMonitor: {
            get: function () {
                return this._adaptiveMonitor;
            },
            set: function (value) {
                this._adaptiveMonitor = value;
            }
        },

        analyticsConfig: {
            get: function () {
                return this._analyticsConfig;
            },
            set: function (value) {
                this._analyticsConfig = value;
            }
        },
        
        analyticsCollector: {
            get: function () {
                return this._collector;
            }
        },

        sessionData: {
            get: function () {
                return this._sessionData;
            },
            set: function (value) {
                this._sessionData = value;
            }
        },

        mediaData: {
            get: function () {
                return this._mediaData;
            },
            set: function (value) {
                this._mediaData = value;
            }
        },

        // Private Methods
        _setOptions: function (options) {
            PlayerFramework.Utilities.setOptions(this, options, {
                isEnabled: true,
                analyticsConfig: new Microsoft.Media.Analytics.AnalyticsConfig()
            });
        },

        _onActivate: function () {
            // by default, we always add the AnalyticsCollector as a logging source
            this._collector = new Microsoft.Media.Analytics.AnalyticsCollector();
            // add session specific data
            this._addAdditionalData(this.sessionData);
            Microsoft.Media.Analytics.LoggingService.current.loggingSources.append(this._collector);

            // initialize the AnalyticsCollector. The analytics collector relies on other objects to pass it info.
            this._mediaPlayerAdapter = new PlayerFramework.Analytics.MediaPlayerAdapter(this.mediaPlayer);
            this._playerMonitor = this._mediaPlayerAdapter.nativeInstance;

            this._bindEvent("loadstart", this.mediaPlayer, this._onMediaPlayerLoadStart);
            this._bindEvent("emptied", this.mediaPlayer, this._onMediaPlayerEmptied);

            return true;
        },

        _onDeactivate: function () {
            if (this._collector.isAttached) {
                this._collector.detach();
            }
            this._unbindEvent("loadstart", this.mediaPlayer, this._onMediaPlayerLoadStart);
            this._unbindEvent("emptied", this.mediaPlayer, this._onMediaPlayerEmptied);
            // remove session specific data
            this._removeAdditionalData(this.sessionData);
            this._collector = null;
            this._mediaPlayerAdapter.dispose();
            this._mediaPlayerAdapter = null;
            this._playerMonitor = null;
        },

        _addAdditionalData: function (data) {
            if (data) {
                for (var i = 0; i < data.length; i++) {
                    var item = data[i];
                    this._collector.addtionalData.insert(item.key, item.value);
                }
            }
        },

        _removeAdditionalData: function (data) {
            if (data) {
                for (var i = 0; i < data.length; i++) {
                    var item = data[i];
                    this._collector.addtionalData.remove(item.key);
                }
            }
        },
        
        _onMediaPlayerLoadStart: function () {
            this._collector.configuration = this.analyticsConfig;

            // attach the AnalyticsCollector
            if (!this._collector.isAttached) {
                this._collector.attach(this._playerMonitor, this.adaptiveMonitor, this.environmentMonitor, this.edgeServerMonitor);
            }
            // remove media specific data
            this._addAdditionalData(this.mediaData);
        },
        
        _onMediaPlayerEmptied: function () {
            // remove media specific data
            this._removeAdditionalData(this.mediaData);
            // detach the AnalyticsCollector
            if (this._collector.isAttached) {
                this._collector.detach();
            }

            this._collector.configuration = null;
        },

        // Public Methods
        log: function (log) {
            this._collector.sendLog(log);
        }
    });

    AnalyticsPlugin.trackingEventArea = "analytics";

    // AnalyticsPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        AnalyticsPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // AnalyticsPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        AnalyticsPlugin: AnalyticsPlugin
    });

})(PlayerFramework);

