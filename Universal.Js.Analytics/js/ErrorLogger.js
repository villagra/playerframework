(function (PlayerFramework, undefined) {
    "use strict";

    // ErrorAnalyticsPlugin Errors
    var invalidConstruction = "Invalid construction: ErrorLogger constructor must be called using the \"new\" operator.";

    // ErrorAnalyticsPlugin Class
    var ErrorLogger = WinJS.Class.define(function () {
        if (!(this instanceof PlayerFramework.Analytics.ErrorLogger)) {
            throw invalidConstruction;
        }

        this._preventUnhandledErrors = false;
        this._maxErrorLength = null;

        // wire up application error event
        this._bindEvent("error", WinJS.Application, this._onApplicationError);
    }, {

        preventUnhandledErrors: {
            get: function () {
                return this._preventUnhandledErrors;
            },
            set: function (value) {
                this._environmentMonitor = value;
            }
        },

        maxErrorLength: {
            get: function () {
                return this._maxErrorLength;
            },
            set: function (value) {
                this._maxErrorLength = value;
            }
        },

        // private methods

        _onApplicationError: function (e) {
            var errorMessage = "";
            if (e.detail.error.stack) errorMessage = e.detail.error.stack;
            else if (e.detail.error) errorMessage = e.detail.error;
            this.logError(errorMessage, "UnhandledException");
            return this.preventUnhandledErrors;
        },

        // public methods
        
        logError: function (error, applicationArea) {
            var errorLog = new Microsoft.Media.Analytics.ErrorLog(error, applicationArea);
            errorLog.maxErrorLength = this.maxErrorLength;
            Microsoft.Media.Analytics.LoggingService.current.log(errorLog);
        },

        dispose: function () {
            this._unbindEvent("error", WinJS.Application, this._onApplicationError);
        },

    });

    // ErrorLogger Mixins
    WinJS.Class.mix(ErrorLogger, PlayerFramework.Utilities.eventBindingMixin);

    // ErrorLogger Exports
    WinJS.Namespace.define("PlayerFramework.Analytics", {
        ErrorLogger: ErrorLogger
    });

})(PlayerFramework);

