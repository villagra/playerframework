(function (PlayerFramework, undefined) {
    "use strict";

    // Slider Errors
    var invalidConstruction = "Invalid construction: Slider constructor must be called using the \"new\" operator.",
        invalidElement = "Invalid argument: Slider expects an element as the first argument.";

    // Slider Events
    var events = [
        "start",
        "update",
        "complete"
    ];

    // Slider Class
    var Slider = WinJS.Class.define(function (element, options) {
        if (!(this instanceof PlayerFramework.UI.Slider)) {
            throw invalidConstruction;
        }

        if (!element) {
            throw invalidElement;
        }

        this._element = null;
        this._containerElement = null;
        this._progressElement = null;
        this._inputElement = null;
        this._altStep1 = 0;
        this._altStep2 = 0;
        this._altStep3 = 0;
        this._hasCapture = false;

        this._setElement(element);
        this._setOptions(options);
    }, {
        // Public Properties
        element: {
            get: function () {
                return this._element;
            }
        },

        min: {
            get: function () {
                return this._inputElement.min;
            },
            set: function (value) {
                this._inputElement.min = value;
                this._element.setAttribute("aria-valuemin", value);
            }
        },

        max: {
            get: function () {
                return this._inputElement.max;
            },
            set: function (value) {
                this._inputElement.max = value;
                this._element.setAttribute("aria-valuemax", value);
            }
        },

        value: {
            get: function () {
                return this._inputElement.valueAsNumber;
            },
            set: function (value) {
                var clampedValue = PlayerFramework.Utilities.clamp(value, this.min, this.max);
                this._inputElement.value = clampedValue;
                this._element.setAttribute("aria-valuenow", clampedValue);
            }
        },

        progress: {
            get: function () {
                return this._progressElement.value;
            },
            set: function (value) {
                this._progressElement.value = value;
            }
        },

        step: {
            get: function () {
                return this._inputElement.step;
            },
            set: function (value) {
                this._inputElement.step = value;
            }
        },

        altStep1: {
            get: function () {
                return this._altStep1;
            },
            set: function (value) {
                this._altStep1 = value;
            }
        },

        altStep2: {
            get: function () {
                return this._altStep2;
            },
            set: function (value) {
                this._altStep2 = value;
            }
        },

        altStep3: {
            get: function () {
                return this._altStep3;
            },
            set: function (value) {
                this._altStep3 = value;
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

        vertical: {
            get: function () {
                return WinJS.Utilities.hasClass(this._element, "pf-vertical");
            },
            set: function (value) {
                if (value) {
                    WinJS.Utilities.addClass(this._element, "pf-vertical");
                } else {
                    WinJS.Utilities.removeClass(this._element, "pf-vertical");
                }
            }
        },

        disabled: {
            get: function () {
                return this._inputElement.disabled;
            },
            set: function (value) {
                this._inputElement.disabled = value;
                this._element.setAttribute("aria-disabled", value);

                if (this._inputElement.disabled) {
                    this._inputElement.releaseCapture();
                    this._onInputElementMSLostPointerCapture();
                }
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
            this._element.setAttribute("role", "slider");
            WinJS.Utilities.addClass(this._element, "pf-slider pf-control pf-functional");

            this._containerElement = PlayerFramework.Utilities.createElement(this._element, ["div", { "class": "pf-slider-container" }]);
            this._progressElement = PlayerFramework.Utilities.createElement(this._containerElement, ["progress"]);
            this._inputElement = PlayerFramework.Utilities.createElement(this._containerElement, ["input", { "type": "range" }]);

            this._bindEvent("MSGotPointerCapture", this._inputElement, this._onInputElementMSGotPointerCapture);
            this._bindEvent("MSLostPointerCapture", this._inputElement, this._onInputElementMSLostPointerCapture);
            this._bindEvent("change", this._inputElement, this._onInputElementChange);
            this._bindEvent("keydown", this._inputElement, this._onInputElementKeydown);
        },

        _setOptions: function (options) {
            PlayerFramework.Utilities.setOptions(this, options, {
                min: 0,
                max: 100,
                value: 0,
                progress: 0,
                step: "any",
                altStep1: 0,
                altStep2: 0,
                altStep3: 0,
                label: "",
                tooltip: "",
                vertical: false,
                disabled: false,
                hidden: false
            });
        },

        _onInputElementMSGotPointerCapture: function (e) {
            if (!this._hasCapture) {
                this._hasCapture = true;
                this.dispatchEvent("start");
                this.dispatchEvent("update");
            }
        },

        _onInputElementMSLostPointerCapture: function (e) {
            if (this._hasCapture) {
                this._hasCapture = false;
                this.dispatchEvent("complete");
            }
        },

        _onInputElementChange: function (e) {
            if (this._hasCapture) {
                this.dispatchEvent("update");
            }
        },

        _onInputElementKeydown: function (e) {
            if (e.keyCode >= 37 && e.keyCode <= 40) {
                e.preventDefault();

                // handle arrow keys
                if (this.vertical) {
                    if (e.key === "Up") {
                        this._increaseValue(e);
                    } else if (e.key === "Down") {
                        this._decreaseValue(e);
                    }
                } else if (window.getComputedStyle(this._inputElement).direction === "rtl") {
                    if (e.key === "Left") {
                        this._increaseValue(e);
                    } else if (e.key === "Right") {
                        this._decreaseValue(e);
                    }
                } else {
                    if (e.key === "Left") {
                        this._decreaseValue(e);
                    } else if (e.key === "Right") {
                        this._increaseValue(e);
                    }
                }
            }
        },

        _increaseValue: function (e) {
            if (!e.shiftKey && !e.ctrlKey && !e.altKey && !e.metaKey) {
                this._changeValue(this.value + this.altStep1);
            } else if (e.shiftKey && !e.ctrlKey && !e.altKey && !e.metaKey) {
                this._changeValue(this.value + this.altStep2);
            } else if (e.ctrlKey && !e.shiftKey && !e.altKey && !e.metaKey) {
                this._changeValue(this.value + this.altStep3);
            }
        },

        _decreaseValue: function (e) {
            if (!e.shiftKey && !e.ctrlKey && !e.altKey && !e.metaKey) {
                this._changeValue(this.value - this.altStep1);
            } else if (e.shiftKey && !e.ctrlKey && !e.altKey && !e.metaKey) {
                this._changeValue(this.value - this.altStep2);
            } else if (e.ctrlKey && !e.shiftKey && !e.altKey && !e.metaKey) {
                this._changeValue(this.value - this.altStep3);
            }
        },

        _changeValue: function (value) {
            var oldValue = this.value;
            this.value = value;

            if (oldValue !== this.value) {
                this.dispatchEvent("start");
                this.dispatchEvent("update");
                this.dispatchEvent("complete");
            }
        }
    });

    // Slider Mixins
    WinJS.Class.mix(Slider, WinJS.UI.DOMEventMixin);
    WinJS.Class.mix(Slider, PlayerFramework.Utilities.createEventProperties(events));
    WinJS.Class.mix(Slider, PlayerFramework.Utilities.eventBindingMixin);

    // Slider Exports
    WinJS.Namespace.define("PlayerFramework.UI", {
        Slider: Slider
    });

})(PlayerFramework);

