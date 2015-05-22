(function (PlayerFramework, undefined) {
    "use strict";

    // DynamicTextTrack Errors
    var invalidConstruction = "Invalid construction: DynamicTextTrack constructor must be called using the \"new\" operator.";

    // AdHandlerPlugin Events
    var events = [
        "payloadaugmented"
    ];

    // DynamicTextTrack Class
    var DynamicTextTrack = WinJS.Class.define(function (stream) {
        if (!(this instanceof PlayerFramework.DynamicTextTrack)) {
            throw invalidConstruction;
        }

        this._stream = stream;
    }, {
        // Public Properties
        stream: {
            get: function () {
                return this._stream;
            }
        },

        label: {
            get: function () {
                return this._stream.name;
            }
        },

        language: {
            get: function () {
                return this._stream.language;
            }
        },
        
        // Public Methods
        augmentPayload: function (payload, startTime, endTime) {
            this.dispatchEvent("payloadaugmented", {
                "payload": payload,
                "startTime": startTime,
                "endTime": endTime,
            });
        }
    });

    // DynamicTextTrack Mixins
    WinJS.Class.mix(DynamicTextTrack, WinJS.Utilities.eventMixin);
    WinJS.Class.mix(DynamicTextTrack, PlayerFramework.Utilities.eventBindingMixin);
    WinJS.Class.mix(DynamicTextTrack, PlayerFramework.Utilities.createEventProperties(events));
    
    // DynamicTextTrack Exports
    WinJS.Namespace.define("PlayerFramework", {
        DynamicTextTrack: DynamicTextTrack
    });

})(PlayerFramework);

