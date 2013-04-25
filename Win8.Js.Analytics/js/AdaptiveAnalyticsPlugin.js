(function (PlayerFramework, undefined) {
    "use strict";

    // AdaptiveAnalyticsPlugin Errors
    var invalidConstruction = "Invalid construction: AdaptiveAnalyticsPlugin constructor must be called using the \"new\" operator.";

    // AdaptiveAnalyticsPlugin Class
    var AdaptiveAnalyticsPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.AdaptiveAnalyticsPlugin)) {
            throw invalidConstruction;
        }

        this._adaptiveMonitorFactory = null;

        PlayerFramework.PluginBase.call(this, options);
    }, {
        
        _setOptions: function (options) {
            PlayerFramework.Utilities.setOptions(this, options, {
                isEnabled: true
            });
        },
        
        _onActivate: function () {
            if (this.mediaPlayer.analyticsPlugin) {
                if (this.mediaPlayer.adaptivePlugin) {
                    this._adaptiveMonitorFactory = new Microsoft.AdaptiveStreaming.Analytics.AdaptiveMonitorFactory(this.mediaPlayer.adaptivePlugin.manager);
                    this.mediaPlayer.analyticsPlugin.adaptiveMonitor = this._adaptiveMonitorFactory.adaptiveMonitor;
                    return true;
                }
            }
            return false;
        },

        _onDeactivate: function () {
            this.mediaPlayer.analyticsPlugin.adaptiveMonitor = null;
            this._adaptiveMonitorFactory.close();
            this._adaptiveMonitorFactory = null;
        }
    });

    // AdaptiveAnalyticsPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        AdaptiveAnalyticsPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // AdaptiveAnalyticsPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        AdaptiveAnalyticsPlugin: AdaptiveAnalyticsPlugin
    });

})(PlayerFramework);

