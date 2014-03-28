(function (PlayerFramework, undefined) {
    "use strict";

    // MediaPackPlugin Errors
    var invalidConstruction = "Invalid construction: MediaPackPlugin constructor must be called using the \"new\" operator.";

    // MediaPackPlugin Class
    var MediaPackPlugin = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.MediaPackPlugin)) {
            throw invalidConstruction;
        }

        this._mediaPackUrl = "http://www.microsoft.com/en-ie/download/details.aspx?id=30685";

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Public Properties
        mediaPackUrl: {
            get: function () {
                return this._mediaPackUrl;
            },
            set: function (value) {
                this._mediaPackUrl = value;
            }
        },

        // Private Methods
        _onLoad: function () {
        },

        _onUnload: function () {
        },

        _onActivate: function () {
            this._bindEvent("loading", this.mediaPlayer, this._onMediaPlayerLoading);
            return true;
        },

        _onDeactivate: function () {
            this._unbindEvent("loading", this.mediaPlayer, this._onMediaPlayerLoading);
        },

        _isMediaPackRequired: function () {
            try {
                var junk = Windows.Media.VideoEffects.videoStabilization;
            }
            catch (error) {
                if (error.number === -2147221164) { // 'Class Not Registered'
                    return true;
                }
            }
            return false;
        },

        _onMediaPlayerLoading: function (e) {
            if (this._isMediaPackRequired()) {
                e.detail.setPromise(this._promptForMediaPack());
            }
        },

        _promptForMediaPack: function() {
            var messageDialog = new Windows.UI.Popups.MessageDialog(PlayerFramework.Utilities.getResourceString("MediaFeaturePackRequiredLabel"), PlayerFramework.Utilities.getResourceString("MediaFeaturePackRequiredText"));
            //Add buttons and set their callback functions
            messageDialog.commands.append(new Windows.UI.Popups.UICommand(PlayerFramework.Utilities.getResourceString("MediaFeaturePackDownloadLabel"),
               function (command) {
                   return Windows.System.Launcher.launchUriAsync(new Windows.Foundation.Uri(this.mediaPackUrl));
               }.bind(this)));
            messageDialog.commands.append(new Windows.UI.Popups.UICommand(PlayerFramework.Utilities.getResourceString("MediaFeaturePackCancelLabel")));
            return messageDialog.showAsync().then(function () { return false; });
        }
    });

    // MediaPackPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        MediaPackPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // MediaPackPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        MediaPackPlugin: MediaPackPlugin
    });

})(PlayerFramework);

