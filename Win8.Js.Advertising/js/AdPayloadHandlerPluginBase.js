(function (PlayerFramework, undefined) {
    "use strict";

    // AdPayloadHandlerPluginBase Errors
    var invalidConstruction = "Invalid construction: AdPayloadHandlerPluginBase is an abstract class.";

    // AdPayloadHandlerPluginBase Class
    var AdPayloadHandlerPluginBase = WinJS.Class.derive(PlayerFramework.PluginBase, function () {
        throw invalidConstruction;
    }, {
        // Public Properties
        nativeInstance: {
            get: function () {
                return null;
            }
        }
    })

    // AdPayloadHandlerPluginBase Exports
    WinJS.Namespace.define("PlayerFramework.Advertising", {
        AdPayloadHandlerPluginBase: AdPayloadHandlerPluginBase
    });
    
})(PlayerFramework);

