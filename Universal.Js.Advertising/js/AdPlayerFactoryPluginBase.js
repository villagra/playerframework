(function (PlayerFramework, undefined) {
    "use strict";

    // AdPlayerFactoryPluginBase Errors
    var invalidConstruction = "Invalid construction: AdPlayerFactoryPluginBase is an abstract class.";

    // AdPlayerFactoryPluginBase Class
    var AdPlayerFactoryPluginBase = WinJS.Class.derive(PlayerFramework.PluginBase, function () {
        throw invalidConstruction;
    }, {
        // Public Methods
        getPlayer: function (creativeSource) {
            return null;
        }
    })

    // AdPlayerFactoryPluginBase Exports
    WinJS.Namespace.define("PlayerFramework.Advertising", {
        AdPlayerFactoryPluginBase: AdPlayerFactoryPluginBase
    });

})(PlayerFramework);

