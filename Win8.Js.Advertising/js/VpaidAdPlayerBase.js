(function (PlayerFramework, undefined) {
    "use strict";

    // VpaidAdPlayerBase Errors
    var invalidConstruction = "Invalid construction: VpaidAdPlayerBase is an abstract class.";

    // VpaidAdPlayerBase Events
    var events = [
        "adloaded",
        "adstarted",
        "adstopped",
        "adplaying",
        "adpaused",
        "adexpandedchanged",
        "adlinearchanged",
        "advolumechanged",
        "advideostart",
        "advideofirstquartile",
        "advideomidpoint",
        "advideothirdquartile",
        "advideocomplete",
        "aduseracceptinvitation",
        "aduserclose",
        "aduserminimize",
        "adremainingtimechange",
        "adimpression",
        "adclickthru",
        "aderror",
        "adlog",
        "adskipped",
        "adsizechanged",
        "adskippablestatechange",
        "addurationchange",
        "adinteraction"
    ];

    // VpaidAdPlayerBase Class
    var VpaidAdPlayerBase = WinJS.Class.define(function () {
        throw invalidConstruction;
    }, {
        // Public Properties
        adElement: {
            get: function () {
                return null;
            }
        },

        adState: {
            get: function () {
                return PlayerFramework.Advertising.VpaidAdPlayerBase.AD_STATE_NONE;
            }
        },

        adSkippableState: {
            get: function () {
                return false;
            }
        },

        skippableOffset: {
            get: function () {
                return null;
            }
        },

        clickThru: {
            get: function () {
                return null;
            }
        },

        adLinear: {
            get: function () {
                return false;
            }
        },

        adExpanded: {
            get: function () {
                return true;
            }
        },

        adVolume: {
            get: function () {
                return 0;
            },
            set: function (value) {
            }
        },

        adWidth: {
            get: function () {
                return 0;
            }
        },

        adHeight: {
            get: function () {
                return 0;
            }
        },

        adDuration: {
            get: function () {
                return 0;
            }
        },

        adRemainingTime: {
            get: function () {
                return 0;
            }
        },

        adCompanions: {
            get: function () {
                return "";
            }
        },

        adIcons: {
            get: function () {
                return false;
            }
        },

        // Public Methods
        handshakeVersion: function (version) {
            if (version.indexOf("1.") === 0) {
                return version;
            } else {
                return "2.0"; // return the highest version of VPAID that we support
            }
        },

        initAd: function (width, height, viewMode, desiredBitrate, creativeData, environmentVariables) {
        },

        startAd: function () {
        },

        stopAd: function () {
        },

        pauseAd: function () {
        },

        resumeAd: function () {
        },

        resizeAd: function (width, height, viewMode) {
        },

        expandAd: function () {
        },

        collapseAd: function () {
        },

        skipAd: function () {
        }
    }, {
        // Constants
        AD_STATE_NONE: 0,
        AD_STATE_LOADING: 1,
        AD_STATE_LOADED: 2,
        AD_STATE_STARTING: 3,
        AD_STATE_PLAYING: 4,
        AD_STATE_PAUSED: 5,
        AD_STATE_COMPLETE: 6,
        AD_STATE_FAILED: 7
    });

    // VpaidAdPlayerBase Mixins
    WinJS.Class.mix(VpaidAdPlayerBase, WinJS.Utilities.eventMixin);
    WinJS.Class.mix(VpaidAdPlayerBase, PlayerFramework.Utilities.createEventProperties(events));
    WinJS.Class.mix(VpaidAdPlayerBase, PlayerFramework.Utilities.eventBindingMixin);
    
    // VpaidAdPlayerBase Exports
    WinJS.Namespace.define("PlayerFramework.Advertising", {
        VpaidAdPlayerBase: VpaidAdPlayerBase
    });

})(PlayerFramework);

