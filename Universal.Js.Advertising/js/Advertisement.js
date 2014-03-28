(function (PlayerFramework, undefined) {
    "use strict";

    // Advertisement Errors
    var invalidConstruction = "Invalid construction: AdvertisementBase constructor must be called using the \"new\" operator.",
        invalidMidrollAdvertisementConstruction = "Invalid construction: MidrollAdvertisement constructor must be called using the \"new\" operator.",
        invalidPrerollAdvertisementConstruction = "Invalid construction: PrerollAdvertisement constructor must be called using the \"new\" operator.",
        invalidPostrollAdvertisementConstruction = "Invalid construction: PostrollAdvertisement constructor must be called using the \"new\" operator.";

    // AdvertisementBase Class
    var AdvertisementBase = WinJS.Class.define(function () {
        if (!(this instanceof PlayerFramework.Advertising.AdvertisementBase)) {
            throw invalidConstruction;
        }

        this._source = null;
    } , {
        // Public Properties
        source: {
            get: function () {
                return this._source;
            },
            set: function (value) {
                this._source = value;
            }
        }
    });

    // MidrollAdvertisement Class
    var MidrollAdvertisement = WinJS.Class.derive(AdvertisementBase, function () {
        if (!(this instanceof PlayerFramework.Advertising.MidrollAdvertisement)) {
            throw invalidMidrollAdvertisementConstruction;
        }

        this._time = null;
        this._timePercentage = null;

        PlayerFramework.Advertising.AdvertisementBase.call(this);
    }, {
        // Public Properties
        time: {
            get: function () {
                return this._time;
            },
            set: function (value) {
                this._time = value;
            }
        },

        timePercentage: {
            get: function () {
                return this._timePercentage;
            },
            set: function (value) {
                this._timePercentage = value;
            }
        }
    });

    // PrerollAdvertisement Class
    var PrerollAdvertisement = WinJS.Class.derive(AdvertisementBase, function () {
        if (!(this instanceof PlayerFramework.Advertising.PrerollAdvertisement)) {
            throw invalidPrerollAdvertisementConstruction;
        }

        PlayerFramework.Advertising.AdvertisementBase.call(this);
    });

    // PostrollAdvertisement Class
    var PostrollAdvertisement = WinJS.Class.derive(AdvertisementBase, function () {
        if (!(this instanceof PlayerFramework.Advertising.PostrollAdvertisement)) {
            throw invalidPostrollAdvertisementConstruction;
        }

        PlayerFramework.Advertising.AdvertisementBase.call(this);
    });

    // Advertisement Exports
    WinJS.Namespace.define("PlayerFramework.Advertising", {
        AdvertisementBase: AdvertisementBase,
        MidrollAdvertisement: MidrollAdvertisement,
        PrerollAdvertisement: PrerollAdvertisement,
        PostrollAdvertisement: PostrollAdvertisement
    });

})(PlayerFramework);

