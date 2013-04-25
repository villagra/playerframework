(function (PlayerFramework, undefined) {
    "use strict";

    // TrackingPluginBase Errors
    var invalidConstruction = "Invalid construction: TrackingPluginBase is an abstract class.";

    // TrackingPluginBase Events
    var events = [
        "eventtracked"
    ];

    // TrackingPluginBase Class
    var TrackingPluginBase = WinJS.Class.derive(PlayerFramework.PluginBase, function (options) {
        this._trackingEvents = [];

        PlayerFramework.PluginBase.call(this, options);
    }, {
        // Public Properties
        trackingEvents: {
            get: function () {
                return this._trackingEvents;
            },
            set: function (value) {
                var oldValue = this._trackingEvents;
                if (oldValue !== value) {
                    this._uninitializeTrackingEvents();

                    this._trackingEvents = value;
                    this._observablePlugin.notify("trackingEvents", value, oldValue);

                    this._initializeTrackingEvents();
                }
            }
        },

        // Private Methods
        _initializeTrackingEvents: function () {
            if (this.trackingEvents) {
                for (var i = 0; i < this.trackingEvents.length; i++) {
                    var trackingEvent = this.trackingEvents[i];
                    this._initializeTrackingEvent(trackingEvent);
                }
            }
        },

        _uninitializeTrackingEvents: function () {
            if (this.trackingEvents) {
                for (var i = 0; i < this.trackingEvents.length; i++) {
                    var trackingEvent = this.trackingEvents[i];
                    this._uninitializeTrackingEvent(trackingEvent);
                }
            }
        },

        _initializeTrackingEvent: function (trackingEvent) {
        },

        _uninitializeTrackingEvent: function (trackingEvent) {
        },

        _onActivate: function () {
            this._initializeTrackingEvents();

            return true;
        },

        _onDeactivate: function () {
            this._uninitializeTrackingEvents();
        }
    })

    // TrackingPluginBase Mixins
    WinJS.Class.mix(TrackingPluginBase, PlayerFramework.Utilities.createEventProperties(events));

    // TrackingPluginBase Exports
    WinJS.Namespace.define("PlayerFramework", {
        TrackingPluginBase: TrackingPluginBase
    });

})(PlayerFramework);

