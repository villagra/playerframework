(function (PlayerFramework, undefined) {
    "use strict";

    // Meter Errors
    var invalidConstruction = "Invalid construction: Meter constructor must be called using the \"new\" operator.",
        invalidElement = "Invalid argument: Meter expects an element as the first argument.";

    // Meter Class
    var Meter = WinJS.Class.define(function (element, options) {
        if (!(this instanceof PlayerFramework.UI.Meter)) {
            throw invalidConstruction;
        }

        if (!element) {
            throw invalidElement;
        }

        this._element = null;

        this._setElement(element);
        this._setOptions(options);
    }, {
        // Public Properties
        element: {
            get: function () {
                return this._element;
            }
        },

        value: {
            get: function () {
                return parseFloat(this._element.getAttribute("aria-valuenow"));
            },
            set: function (value) {
                this._element.setAttribute("aria-valuenow", value);

                if (value < 0.25) {
                    this._element.setAttribute("aria-valuetext", "none");
                } else if (value < 0.5) {
                    this._element.setAttribute("aria-valuetext", "low");
                } else if (value < 0.75) {
                    this._element.setAttribute("aria-valuetext", "medium");
                } else {
                    this._element.setAttribute("aria-valuetext", "high");
                }
            }
        },

        label: {
            get: function () {
                return this._element.getAttribute("aria-label");
            },
            set: function (value) {
                this._element.setAttribute("aria-label", value);
            }
        },

        tooltip: {
            get: function () {
                return this._element.title;
            },
            set: function (value) {
                this._element.title = value;
            }
        },

        disabled: {
            get: function () {
                return this._element.disabled;
            },
            set: function (value) {
                this._element.disabled = value;
                this._element.setAttribute("aria-disabled", value);
            }
        },

        hidden: {
            get: function () {
                return WinJS.Utilities.hasClass(this._element, "pf-hidden");
            },
            set: function (value) {
                if (value) {
                    WinJS.Utilities.addClass(this._element, "pf-hidden");
                    this._element.setAttribute("aria-hidden", true);
                } else {
                    WinJS.Utilities.removeClass(this._element, "pf-hidden");
                    this._element.setAttribute("aria-hidden", false);
                }
            }
        },

        // Private Methods
        _setElement: function (element) {
            this._element = element;
            this._element.winControl = this;
            this._element.setAttribute("role", "status");
            WinJS.Utilities.addClass(this._element, "pf-meter pf-control");

            PlayerFramework.Utilities.createElement(this._element, ["div", { "class": "pf-meter-bar" }]);
            PlayerFramework.Utilities.createElement(this._element, ["div", { "class": "pf-meter-bar" }]);
            PlayerFramework.Utilities.createElement(this._element, ["div", { "class": "pf-meter-bar" }]);
        },

        _setOptions: function (options) {
            PlayerFramework.Utilities.setOptions(this, options, {
                value: 0,
                label: "",
                tooltip: "",
                disabled: false,
                hidden: false
            });
        }
    });

    // Meter Mixins
    WinJS.Class.mix(Meter, WinJS.UI.DOMEventMixin);

    // Meter Exports
    WinJS.Namespace.define("PlayerFramework.UI", {
        Meter: Meter
    });

})(PlayerFramework);

