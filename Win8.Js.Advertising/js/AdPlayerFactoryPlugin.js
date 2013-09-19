(function (PlayerFramework, undefined) {
    "use strict";

    // AdPlayerFactoryPlugin Errors
    var invalidConstruction = "Invalid construction: AdPlayerFactoryPlugin constructor must be called using the \"new\" operator.";

    // AdPlayerFactoryPlugin Class
    var AdPlayerFactoryPlugin = WinJS.Class.derive(PlayerFramework.Advertising.AdPlayerFactoryPluginBase, function (options) {
        if (!(this instanceof PlayerFramework.Plugins.AdPlayerFactoryPlugin)) {
            throw invalidConstruction;
        }

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Public Properties
        supportedVideoMimeTypes: {
            get: function () {
                return this._supportedVideoMimeTypes;
            },
            set: function (value) {
                this._supportedVideoMimeTypes = value;
            }
        },

        skippableOffset: {
            get: function () {
                return this._skippableOffset;
            },
            set: function (value) {
                this._skippableOffset = value;
            }
        },

        clickThruLinearText: {
            get: function () {
                return this._clickThruLinearText;
            },
            set: function (value) {
                this._clickThruLinearText = value;
            }
        },

        clickThruNonLinearText: {
            get: function () {
                return this._clickThruNonLinearText;
            },
            set: function (value) {
                this._clickThruNonLinearText = value;
            }
        },

        // Public Methods
        getPlayer: function (creativeSource) {
            if (creativeSource.type === Microsoft.VideoAdvertising.CreativeSourceType.linear){
                if (creativeSource.apiFramework == "VPAID") {
                    // TODO: support application/x-javascript
                }
                else{
                    if (this._supportsMimeType(creativeSource.mimeType, this.supportedVideoMimeTypes) || this._canPlayType(creativeSource.codec)) {
                        var result = new PlayerFramework.Advertising.VpaidVideoAdPlayer(creativeSource.skippableOffset || Microsoft.VideoAdvertising.FlexibleOffset.parse(this.skippableOffset), creativeSource.duration, creativeSource.clickUrl, this.clickThruLinearText);
                        result.msAudioCategory = this.mediaPlayer.msAudioCategory;
                        return result;
                    }
                }
            }
            else if (creativeSource.type === Microsoft.VideoAdvertising.CreativeSourceType.nonLinear) {
                switch (creativeSource.mediaSourceType) {
                    case Microsoft.VideoAdvertising.MediaSourceEnum.static:
                        if (this._supportsMimeType(creativeSource.mimeType, PlayerFramework.Utilities.getImageMimeTypes())) {
                            return new PlayerFramework.Advertising.VpaidImageAdPlayer(creativeSource.skippableOffset || Microsoft.VideoAdvertising.FlexibleOffset.parse(this.skippableOffset), creativeSource.duration, creativeSource.clickUrl, this.clickThruNonLinearText, creativeSource.dimensions);
                        }
                    case Microsoft.VideoAdvertising.MediaSourceEnum.iframe:
                        return new PlayerFramework.Advertising.VpaidIFrameAdPlayer(creativeSource.skippableOffset || Microsoft.VideoAdvertising.FlexibleOffset.parse(this.skippableOffset), creativeSource.duration, creativeSource.clickUrl, this.clickThruNonLinearText, creativeSource.dimensions);
                    case Microsoft.VideoAdvertising.MediaSourceEnum.html:
                        return new PlayerFramework.Advertising.VpaidHtmlAdPlayer(creativeSource.skippableOffset || Microsoft.VideoAdvertising.FlexibleOffset.parse(this.skippableOffset), creativeSource.duration, creativeSource.clickUrl, this.clickThruNonLinearText, creativeSource.dimensions);
                }

            }
            return null;
        },

        // Private Methods
        _canPlayType: function(codec) {
            try {
                return this.mediaPlayer.canPlayType(codec);
            } catch (e) {
                // HACK: this will crash on some types due to bug in IE
                return false;
            }
        },

        _setOptions: function (options) {
            PlayerFramework.Utilities.setOptions(this, options, {
                isEnabled: true,
                skippableOffset: null,
                clickThruLinearText: "",
                clickThruNonLinearText: "",
                supportedVideoMimeTypes: [
                    "video/mp4",
                    "video/x-ms-wmv"
                ]
            });
        },

        _supportsMimeType: function (mimeType, supportedMimeTypes) {
            return !!PlayerFramework.Utilities.first(supportedMimeTypes, function (supportedMimeType) {
                return supportedMimeType.toLowerCase() === mimeType.toLowerCase();
            });
        }
    });

    // AdPlayerFactoryPlugin Mixins
    WinJS.Class.mix(PlayerFramework.MediaPlayer, {
        adPlayerFactoryPlugin: {
            value: null,
            writable: true,
            configurable: true
        }
    });

    // AdPlayerFactoryPlugin Exports
    WinJS.Namespace.define("PlayerFramework.Plugins", {
        AdPlayerFactoryPlugin: AdPlayerFactoryPlugin
    });

})(PlayerFramework);

