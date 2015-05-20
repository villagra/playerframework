(function (PlayerFramework, undefined) {
    "use strict";

    // Button Errors
    var invalidConstruction = "Invalid construction: Button constructor must be called using the \"new\" operator.",
        invalidElement = "Invalid argument: Button expects an element as the first argument.";

    // Button Events
    var events = [
        "click"
    ];

    // Button Class
    var Button = WinJS.Class.define(function (element, options) {
        if (!(this instanceof PlayerFramework.UI.Button)) {
            throw invalidConstruction;
        }

        if (!element) {
            throw invalidElement;
        }

        this._element = null;
        this._containerElement = null;
        this._contentElement = null;
        this._hoverContentElement = null;
        this._type = null;
        this._flyout = null;

        this._setElement(element);
        this._setOptions(options);
    }, {
        // Public Properties
        element: {
            get: function () {
                return this._element;
            }
        },

        type: {
            get: function () {
                return this._type;
            },
            set: function (value) {
                if (value === "flyout") {
                    this._type = "flyout";
                    this._element.setAttribute("aria-haspopup", true);
                } else {
                    this._type = "button";
                    this._element.setAttribute("aria-haspopup", false);
                }
            }
        },

        content: {
            get: function () {
            	return this._contentElement.textContent;
            },
            set: function (value) {
            	this._contentElement.textContent = value;
            }
        },
        
        hoverContent: {
            get: function () {
                return this._hoverContentElement.textContent;
            },
            set: function (value) {
                this._hoverContentElement.textContent = value;

                if (value) {
                    WinJS.Utilities.addClass(this._element, "pf-hover");
                } else {
                    WinJS.Utilities.removeClass(this._element, "pf-hover");
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

        flyout: {
            get: function () {
                var flyout = this._flyout;

                if (typeof flyout === "string") {
                    flyout = document.getElementById(flyout);
                }

                if (flyout && !flyout.element) {
                    flyout = flyout.winControl;
                }

                return flyout;
            },
            set: function (value) {
                var id = value;

                if (id && typeof id !== "string") {
                    if (id.element) {
                        id = id.element;
                    }

                    if (id) {
                        if (id.id) {
                            id = id.id;
                        } else {
                            id = id.uniqueID;
                        }
                    }
                }

                if (typeof id === "string") {
                    this._element.setAttribute("aria-owns", id);
                }

                this._flyout = value;
            }
        },

        // Private Methods
        _setElement: function (element) {
            this._element = element;
            this._element.winControl = this;
            this._element.setAttribute("role", "button");
            WinJS.Utilities.addClass(this._element, "pf-button pf-control pf-functional");

            this._containerElement = PlayerFramework.Utilities.createElement(this._element, ["span", { "class": "pf-button-container" }]);
            this._contentElement = PlayerFramework.Utilities.createElement(this._containerElement, ["span", { "class": "pf-button-content" }]);
            this._hoverContentElement = PlayerFramework.Utilities.createElement(this._containerElement, ["span", { "class": "pf-button-content" }]);

            this._bindEvent("click", this._element, this._onElementClick);
        },

        _setOptions: function (options) {
            PlayerFramework.Utilities.setOptions(this, options, {
                type: "button",
                content: "",
                hoverContent: "",
                label: "",
                tooltip: "",
                disabled: false,
                hidden: false,
                flyout: null
            });
        },

        _onElementClick: function (e) {
            if (this.type === "flyout") {
                var flyout = this.flyout;
                if (flyout && flyout.show) {
                    flyout.show(this);
                }
            }
        }
    });

    // Button Mixins
    WinJS.Class.mix(Button, WinJS.UI.DOMEventMixin);
    WinJS.Class.mix(Button, PlayerFramework.Utilities.createEventProperties(events));
    WinJS.Class.mix(Button, PlayerFramework.Utilities.eventBindingMixin);

    // Button Exports
    WinJS.Namespace.define("PlayerFramework.UI", {
        Button: Button
    });

})(PlayerFramework);

