(function (PlayerFramework, undefined) {
    "use strict";

    // Indicator Errors
    var invalidConstruction = "Invalid construction: Indicator constructor must be called using the \"new\" operator.",
        invalidElement = "Invalid argument: Indicator expects an element as the first argument.";

    // Indicator Class
    var Indicator = WinJS.Class.define(function (element, options) {
        if (!(this instanceof PlayerFramework.UI.Indicator)) {
            throw invalidConstruction;
        }

        if (!element) {
            throw invalidElement;
        }

        this._element = null;
        this._textElement = null;

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
                return this._textElement.textContent;
            },
            set: function (value) {
                this._textElement.textContent = value;
                this._element.setAttribute("aria-valuenow", value);
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
            WinJS.Utilities.addClass(this._element, "pf-indicator pf-control");

            this._textElement = PlayerFramework.Utilities.createElement(this._element, ["span", { "class": "pf-indicator-text" }]);
        },

        _setOptions: function (options) {
            PlayerFramework.Utilities.setOptions(this, options, {
                value: "",
                label: "",
                tooltip: "",
                disabled: false,
                hidden: false
            });
        }
    });

    // Indicator Mixins
    WinJS.Class.mix(Indicator, WinJS.UI.DOMEventMixin);

    // Indicator Exports
    WinJS.Namespace.define("PlayerFramework.UI", {
        Indicator: Indicator
    });

})(PlayerFramework);

