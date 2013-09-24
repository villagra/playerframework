(function (undefined) {
    "use strict";

    // Constants
    var invalidArgument = "Invalid argument.";
    var invalidResourceId = "Invalid resource identifier: {0}";
    var zeroDate = new Date(0, 0, 0, 0, 0, 0, 0);
    var isWinJS1 = (WinJS.Utilities.Scheduler === undefined);

    // Globalization
    var languages = Windows.System.UserProfile.GlobalizationPreferences.languages;
    var geographicRegion = Windows.System.UserProfile.GlobalizationPreferences.homeGeographicRegion;
    var calendar = Windows.System.UserProfile.GlobalizationPreferences.calendars[0];
    var clock = Windows.Globalization.ClockIdentifiers.twentyFourHour;

    // Formatters
    var timeFormatter = new Windows.Globalization.DateTimeFormatting.DateTimeFormatter(getResourceString("TimeFormat"), languages, geographicRegion, calendar, clock);
    var percentFormatter = new Windows.Globalization.NumberFormatting.PercentFormatter(languages, geographicRegion);

    percentFormatter.integerDigits = 1;
    percentFormatter.fractionDigits = 0;

    // Enumerations
    var AdvertisingState = {
        /// <field>No ad is loading or playing.</field>
        none: 0,
        /// <field>An ad is loading.</field>
        loading: 1,
        /// <field>A linear ad is playing.</field>
        linear: 2,
        /// <field>A non-linear ad is playing.</field>
        nonLinear: 3
    };

    var PlayerState = {
        /// <field>The player is unloaded and no media source is set.</field>
        unloaded: 0,
        /// <field>The media source is set and the player is waiting to load the media (e.g. autoload is false).</field>
        pending: 1,
        /// <field>The media source is set, but the player is still executing loading operations.</field>
        loading: 2,
        /// <field>The media has finished loading, but has not been opened yet.</field>
        loaded: 3,
        /// <field>The media can be played.</field>
        opened: 4,
        /// <field>The media has been told to start playing, but the player is still executing starting operations.</field>
        starting: 5,
        /// <field>The media has been started and the player is either playing or paused.</field>
        started: 6,
        /// <field>The media has finished, but the player is still executing ending operations.</field>
        ending: 7,
        /// <field>The media has ended.</field>
        ended: 8,
        /// <field>The media has failed and the player must be reloaded.</field>
        failed: 9
    };

    var ReadyState = {
        /// <field>The player has no information for the audio/video.</field>
        nothing: 0,
        /// <field>The player has metadata for the audio/video.</field>
        metadata: 1,
        /// <field>The player has data for the current playback position, but not enough data to play the next frame.</field>
        currentData: 2,
        /// <field>The player has data for the current playback position and at least the next frame.</field>
        futureData: 3,
        /// <field>The player has enough data available to start playing.</field>
        enoughData: 4
    };

    var NetworkState = {
        /// <field>The player has not yet initialized any audio/video.</field>
        empty: 0,
        /// <field>The player has active audio/video and has selected a resource, but is not using the network.</field>
        idle: 1,
        /// <field>The player is downloading data.</field>
        loading: 2,
        /// <field>The player has no audio/video source.</field>
        noSource: 3
    };

    var MediaQuality = {
        /// <field>Typically indicates less than 720p media quality.</field>
        standardDefinition: 0,
        /// <field>Typically indicates greater than or equal to 720p media quality.</field>
        highDefinition: 1
    };

    var MediaErrorCode = {
        /// <field>An unknown media error occurred.</field>
        unknown: 0,
        /// <field>Media playback was aborted.</field>
        aborted: 1,
        /// <field>Media download failed due to a network error.</field>
        network: 2,
        /// <field>Media playback was aborted due to a corruption problem or because unsupported features were used.</field>
        decode: 3,
        /// <field>Media source could not be loaded either because the server or network failed or because the format is not supported.</field>
        notSupported: 4
    };

    var ImageErrorCode = {
        /// <field>An unknown image error occurred.</field>
        unknown: 0,
        /// <field>Image download was aborted.</field>
        aborted: 1
    };

    var AutohideBehavior = {
        /// <field>No behaviors are applied to the autohide feature.</field>
        none: 0,
        /// <field>Autohide is allowed during media playback only.</field>
        allowDuringPlaybackOnly: 1,
        /// <field>Autohide is prevented when the pointer is over interactive components such as the control panel.</field>
        preventDuringInteractiveHover: 2,
        /// <field>All behaviors are applied to the autohide feature.</field>
        all: 3
    };

    var InteractionType = {
        /// <field>Indicates no interaction.</field>
        none: 0,
        /// <field>Indicates a "soft" interaction such as mouse movement or a timeout occurring.</field>
        soft: 1,
        /// <field>Indicates a "hard" interaction such as a tap, click, or a key is pressed.</field>
        hard: 2,
        /// <field>Indicates both "soft" and "hard" interactions.</field>
        all: 3
    };

    var TextTrackMode;
    if (isWinJS1) {
        TextTrackMode = {
            /// <field>The track is disabled.</field>
            off: 0,
            /// <field>The track is active, but the player is not actively displaying cues.</field>
            hidden: 1,
            /// <field>The track is active and the player is actively displaying cues.</field>
            showing: 2
        };
    }
    else {
        TextTrackMode = {
            /// <field>The track is disabled.</field>
            off: "off",
            /// <field>The track is active, but the player is not actively displaying cues.</field>
            hidden: "hidden",
            /// <field>The track is active and the player is actively displaying cues.</field>
            showing: "showing"
        };
    }

    var TextTrackDisplayMode = {
        /// <field>Indicates tracks should not be displayed.</field>
        none: 0,
        /// <field>Indicates tracks should be displayed using custom UI.</field>
        custom: 1,
        /// <field>Indicates tracks should be displayed using native UI.</field>
        native: 2,
        /// <field>Indicates tracks should be displayed using both custom and native UI. This is useful for debugging.</field>
        all: 3
    };

    var TextTrackReadyState = {
        /// <field>The track is unloaded.</field>
        none: 0,
        /// <field>The track is currently loading.</field>
        loading: 1,
        /// <field>The track is loaded.</field>
        loaded: 2,
        /// <field>The track failed to load.</field>
        error: 3
    };

    // Functions
    function formatTime(value) {
        /// <summary>Formats the specified time value (in seconds) as a string.</summary>
        /// <param name="value" type="Number">The value to format.</param>
        /// <returns type="String">The formatted string.</returns>

        if (isFinite(value) && !isNaN(value) && value > 0) {
            value = new Date(0, 0, 0, 0, 0, value, 0);
        } else {
            value = zeroDate;
        }

        return timeFormatter.format(value);
    }

    function formatPercentage(value) {
        /// <summary>Formats the specified percentage value as a string.</summary>
        /// <param name="value" type="Number">The value to format.</param>
        /// <returns type="String">The formatted string.</returns>

        if (isFinite(value) && !isNaN(value) && value) {
            value = value.toFixed(4);
        } else {
            value = 0;
        }

        return percentFormatter.format(value);
    }

    function formatString(string /* , arg1, arg2, argN */) {
        /// <summary>Formats the specified string using the additional arguments provided.</summary>
        /// <param name="string" type="String">The string to format.</param>
        /// <returns type="String">The formatted string.</returns>

        // TODO: remove dependency on private implementation
        return WinJS.Resources._formatString.apply(null, arguments);
    }

    function formatResourceString(resourceId /* , arg1, arg2, argN */) {
        /// <summary>Formats the specified resource string using the additional arguments provided.</summary>
        /// <param name="resourceId" type="String">The resource identifier.</param>
        /// <returns type="String">The formatted resource string.</returns>

        var string = getResourceString(resourceId);
        
        var args = Array.prototype.slice.call(arguments, 1);
        args.unshift(string);

        return formatString.apply(null, args);
    }

    function getResourceString(resourceId) {
        /// <summary>Returns the specified resource string.</summary>
        /// <param name="resourceId" type="String">The resource identifier.</param>
        /// <returns type="String">The resource string.</returns>

        // look in app resources
        var string = WinJS.Resources.getString("/PlayerFramework/" + resourceId);

        // look in component resources
        if (string.empty) {
            string = WinJS.Resources.getString("/Microsoft.PlayerFramework.Js/Resources/" + resourceId);
        }

        // resource not found
        if (string.empty) {
            throw formatString(invalidResourceId, resourceId);
        }

        return string.value;
    }

    function getMediaErrorMessage(error) {
        /// <summary>Gets an error message for the specified media error.</summary>
        /// <param name="error" type="MediaError">The error.</param>
        /// <returns type="String">The error message.</returns>

        return error ? getMediaErrorMessageForCode(error.code, error.msExtendedCode) : getMediaErrorMessageForCode(MediaErrorCode.unknown);
    }

    function getMediaErrorMessageForCode(code, extendedCode) {
        /// <summary>Gets an error message for the specified media error code.</summary>
        /// <param name="code" type="PlayerFramework.MediaErrorCode">The error code.</param>
        /// <param name="extendedCode" type="Number" optional="true">The extended error code.</param>
        /// <returns type="String">The error message.</returns>

        var message;

        switch (code) {
            case MediaErrorCode.aborted:
                message = "MEDIA_ERR_ABORTED"; 
                break;
            case MediaErrorCode.network:
                message = "MEDIA_ERR_NETWORK";
                break;
            case MediaErrorCode.decode:
                message = "MEDIA_ERR_DECODE";
                break;
            case MediaErrorCode.notSupported:
                message = "MEDIA_ERR_SRC_NOT_SUPPORTED";
                break;
            default:
                message = "MEDIA_ERR_UNKNOWN";
                break;
        }

        if (typeof extendedCode === "number") {
            message += " (" + convertDecimalToHex(extendedCode) + ")";
        }

        return message;
    }

    function getImageErrorMessageForCode(code) {
        /// <summary>Gets an error message for the specified image error code.</summary>
        /// <param name="code" type="PlayerFramework.ImageErrorCode">The error code.</param>
        /// <returns type="String">The error message.</returns>

        var message;

        switch (code) {
            case ImageErrorCode.aborted:
                message = "IMAGE_ERR_ABORTED";
                break;
            default:
                message = "IMAGE_ERR_UNKNOWN";
                break;
        }

        return message;
    }

    function getImageMimeTypes() {
        /// <summary>Returns an array of common image MIME types.</summary>
        /// <returns type="Array">The MIME types.</returns>

        var mimeTypes = [];
        var decoders = Windows.Graphics.Imaging.BitmapDecoder.getDecoderInformationEnumerator();

        for (var i = 0; i < decoders.length; i++) {
            var decoder = decoders[i];
            for (var j = 0; j < decoder.mimeTypes.length; j++) {
                var mimeType = decoder.mimeTypes[j];
                mimeTypes.push(mimeType);
            }
        }

        return mimeTypes;
    }

    function getArray(obj) {
        /// <summary>Gets an array from an "enumerable" object.</summary>
        /// <param name="obj" type="Object">The target object.</param>
        /// <returns type="Array">The array.</returns>

        if (obj) {
            if (Array.isArray(obj)) {
                return obj;
            } else if (typeof obj.length !== "undefined") {
                return Array.prototype.slice.call(obj);
            } else if (typeof obj.first === "function") {
                var array = [];

                for (var i = obj.first() ; i.hasCurrent; i.moveNext()) {
                    array.push(i.current);
                }

                return array;
            }
        }

        throw invalidArgument;
    }

    function setOptions(obj, options, defaults) {
        /// <summary>Applies a set of options to the properties and events of the specified object.</summary>
        /// <param name="obj" type="Object">The target object.</param>
        /// <param name="options" type="Object" optional="true">The options to apply.</param>
        /// <param name="defaults" type="Object" optional="true">The optional defaults to apply.</param>

        if (defaults) {
            options = extend({}, defaults, options);
        }

        if (options) {
            var keys = Object.keys(options);
            for (var i = 0; i < keys.length; i++) {
                var key = keys[i];
                var target = obj[key];
                var value = options[key];
                if (target instanceof PlayerFramework.PluginBase) {
                    setOptions(target, value);
                } else if (key.length > 2 && key.indexOf("on") === 0 && typeof value === "function" && obj.addEventListener) {
                    obj.addEventListener(key.substring(2), value);
                } else {
                    obj[key] = value;
                }
            }
        }
    }

    function convertDecimalToHex(value) {
        /// <summary>Converts a signed decimal value to a hexadecimal string.</summary>
        /// <param name="value" type="Number">The decimal value to convert.</param>
        /// <returns type="String">The hexadecimal string.</returns>

        return "0x" + (value >>> 0).toString(16).toUpperCase();
    }

    function convertHexToDecimal(value) {
        /// <summary>Converts a hexidecimal value to a signed decimal number.</summary>
        /// <param name="value" type="String">The hexidecimal value to convert.</param>
        /// <returns type="Number">The decimal number.</returns>

        var result = parseInt(value, 16);

        if ((result < 0x100) && (result & 0x80)) {
            result -= 0x100;
        } else if ((result < 0x10000) && (result & 0x8000)) {
            result -= 0x10000;
        } else if ((result < 0x100000000) && (result & 0x80000000)) {
            result -= 0x100000000;
        }

        return result;
    }

    function convertSecondsToTicks(value) {
        /// <summary>Converts the specified value (in seconds) to ticks.</summary>
        /// <param name="value" type="Number">The number of seconds to convert.</param>
        /// <returns type="Number">The number of ticks.</returns>

        return Math.round(value * 10000000);
    }

    function convertTicksToSeconds(value) {
        /// <summary>Converts the specified value (in ticks) to seconds.</summary>
        /// <param name="value" type="Number">The number of ticks to convert.</param>
        /// <returns type="Number">The number of seconds.</returns>

        return value / 10000000;
    }

    function convertMillisecondsToTicks(value) {
        /// <summary>Converts the specified value (in milliseconds) to ticks.</summary>
        /// <param name="value" type="Number">The number of milliseconds to convert.</param>
        /// <returns type="Number">The number of ticks.</returns>

        return value * 10000;
    }

    function convertTicksToMilliseconds(value) {
        /// <summary>Converts the specified value (in ticks) to milliseconds.</summary>
        /// <param name="value" type="Number">The number of ticks to convert.</param>
        /// <returns type="Number">The number of milliseconds.</returns>

        return value / 10000;
    }

    function calculateElapsedTime(currentTime, startTime, endTime) {
        /// <summary>Calculates the elapsed time (in seconds).</summary>
        /// <param name="currentTime" type="Number">The current time (in ticks).</param>
        /// <param name="startTime" type="Number">The start time (in ticks).</param>
        /// <param name="endTime" type="Number">The end time (in ticks).</param>
        /// <returns type="Number">The elapsed time.</returns>

        return Math.floor(convertTicksToSeconds(clamp(currentTime, startTime, endTime)));
    }

    function calculateRemainingTime(currentTime, startTime, endTime) {
        /// <summary>Calculates the remaining time (in seconds).</summary>
        /// <param name="currentTime" type="Number">The current time (in ticks).</param>
        /// <param name="startTime" type="Number">The start time (in ticks).</param>
        /// <param name="endTime" type="Number">The end time (in ticks).</param>
        /// <returns type="Number">The remaining time.</returns>

        return Math.floor(convertTicksToSeconds(endTime)) - Math.floor(convertTicksToSeconds(clamp(currentTime, startTime, endTime)));
    }

    function calculateBufferedTime(buffered) {
        /// <summary>Calculates the total buffered time.</summary>
        /// <param name="buffered" type="TimeRanges">The buffered time ranges.</param>
        /// <returns type="Number">The buffered time.</returns>

        if (!buffered) {
            return NaN;
        }

        var value = 0;

        for (var i = 0; i < buffered.length; i++) {
            value += buffered.end(i) - buffered.start(i);
        }

        return value;
    }

    function calculateBufferedPercentage(buffered, duration) {
        /// <summary>Calculates the buffered time as a percentage of the specified duration.</summary>
        /// <param name="buffered" type="TimeRanges">The buffered time ranges.</param>
        /// <param name="duration" type="Number">The duration.</param>
        /// <returns type="Number">The buffered percentage.</returns>

        if (!buffered || !duration) {
            return NaN;
        }

        var value = calculateBufferedTime(buffered) / duration;

        return clamp(value, 0, 1);
    }

    function launch(uri) {
        /// <summary>Launches the default application associated with the specified URI.</summary>
        /// <param name="uri" type="String">The URI.</param>
        /// <returns type="WinJS.Promise">The promise.</returns>

        if (uri) {
            return Windows.System.Launcher.launchUriAsync(new Windows.Foundation.Uri(uri));
        }

        return null;
    }

    function clamp(value, min, max) {
        /// <summary>Clamps a value to the specified minimum and maximum values.</summary>
        /// <param name="value" type="Number">The value to clamp.</param>
        /// <param name="min" type="Number">The minimum value.</param>
        /// <param name="max" type="Number">The maximum value.</param>
        /// <returns type="Number">The clamped value.</returns>

        if (!isNaN(min) && !isNaN(max)) {
            return Math.max(min, Math.min(max, value));
        } else if (!isNaN(min)) {
            return Math.max(min, value);
        } else if (!isNaN(max)) {
            return Math.min(max, value);
        } else {
            return value;
        }
    }

    function clone(obj /* , arg1, arg2, argN */) {
        /// <summary>Clones the specified object and extends it with the properties of the additional arguments provided.</summary>
        /// <param name="obj" type="Object">The object to clone.</param>
        /// <returns type="Object">The cloned object.</returns>

        var args = Array.prototype.slice.call(arguments);
        args.unshift({});

        return extend.apply(null, args);
    }

    function extend(obj /* , arg1, arg2, argN */) {
        /// <summary>Extends the specified object with the properties of the additional arguments provided.</summary>
        /// <param name="obj" type="Object">The object to extend.</param>
        /// <returns type="Object">The extended object.</returns>

        var args = Array.prototype.slice.call(arguments, 1);

        for (var i = 0; i < args.length; i++) {
            var arg = args[i];
            for (var property in arg) {
                obj[property] = arg[property];
            }
        }

        return obj;
    }

    function first(array, callback, thisObj) {
        /// <summary>Returns the first item in the array that passes the test implemented by the specified callback function.</summary>
        /// <param name="array" type="Array">The array to search.</param>
        /// <param name="callback" type="Function">A function that should return true if an item passes the test.</param>
        /// <param name="thisObj" type="Object" optional="true">The optional object to use as "this" when executing the callback.</param>
        /// <returns type="Object">The first item that passes the test.</returns>

        if (array === null || array === undefined) {
            throw invalidArgument;
        }

        if (typeof callback !== "function") {
            throw invalidArgument;
        }

        var obj = Object(array);
        var len = obj.length >>> 0;

        for (var i = 0; i < len; i++) {
            if (i in obj && callback.call(thisObj, obj[i], i, obj)) {
                return obj[i];
            }
        }

        return null;
    }

    function remove(array, item) {
        /// <summary>Removes the specified item from an array.</summary>
        /// <param name="array" type="Array">The array to search.</param>
        /// <param name="item" type="Object">The item to remove.</param>
        /// <returns type="Boolean">True if the item is removed.</returns>

        if (!(array instanceof Array)) {
            throw invalidArgument;
        }

        var index = array.indexOf(item);

        if (index !== -1) {
            array.splice(index, 1);
            return true;
        }

        return false;
    }

    function binaryInsert(array, value, comparer) {
        /// <summary>Inserts a value into a sorted array if it does not already exist.</summary>
        /// <param name="array" type="Array">The target array.</param>
        /// <param name="value" type="Object">The value to insert.</param>
        /// <param name="comparer" type="Function">The comparison function by which the array is sorted.</param>
        /// <returns type="Boolean">True if the value was inserted.</returns>

        var index = binarySearch(array, value, comparer);

        if (index < 0) {
            array.splice(-(index + 1), 0, value);
            return true;
        }

        return false;
    }

    function binarySearch(array, value, comparer) {
        /// <summary>Searches a sorted array for the specified value using the binary search algorithm.</summary>
        /// <param name="array" type="Array">The array to search.</param>
        /// <param name="value" type="Object">The value to search for.</param>
        /// <param name="comparer" type="Function">The comparison function by which the array is sorted.</param>
        /// <returns type="Number">The lowest index of the value if found, otherwise the insertion point.</returns>

        var left = 0;
        var right = array.length;
        var middle, compareResult, found;

        while (left < right) {
            middle = (left + right) >> 1;
            compareResult = comparer(value, array[middle]);
            if (compareResult > 0) {
                left = middle + 1;
            } else {
                right = middle;
                found = !compareResult;
            }
        }

        return found ? left : ~left;
    }

    function createElement(parent, options, namespace) {
        /// <summary>Creates a DOM element using the specified options.</summary>
        /// <param name="parent" type="HTMLElement" domElement="true" optional="true">The optional parent element.</param>
        /// <param name="options" type="Array">A JSON array containing the tag name, attributes, and child elements for the element.</param>
        /// <param name="namespace" type="Object" optional="true">An optional namespace for the element.</param>
        /// <returns type="HTMLElement" domElement="true">The created element.</returns>

        if (!(options instanceof Array) || options.length === 0 || typeof options[0] !== "string") {
            throw invalidArgument;
        }

        var element, attributes, namespace;

        if (options[1] && typeof options[1] === "object" && !(options[1] instanceof Array)) {
            attributes = options[1];
            namespace = attributes["xmlns"] || namespace;
        }

        // create the element
        if (namespace) {
            element = document.createElementNS(namespace, options[0]);
        } else {
            element = document.createElement(options[0]);
        }

        // set the attributes
        if (attributes) {
            for (var name in attributes) {
                var value = attributes[name];
                if (typeof value !== "string") {
                    element[name] = value;
                } else {
                    element.setAttribute(name, value);
                }
            }
        }

        // append the child elements
        for (var i = 1; i < options.length; i++) {
            var childOptions = options[i];
            if (childOptions instanceof Array) {
                createElement(element, childOptions, namespace);
            } else if (typeof childOptions === "string") {
                element.appendChild(document.createTextNode(childOptions));
            }
        }

        // append the element
        if (parent) {
            parent.appendChild(element);
        }

        return element;
    }

    function appendElement(parent, element) {
        /// <summary>Appends a DOM element.</summary>
        /// <param name="parent" type="HTMLElement" domElement="true">The parent element.</param>
        /// <param name="element" type="HTMLElement" domElement="true">The element to append.</param>

        if (!parent) {
            throw invalidArgument;
        }

        if (!element) {
            throw invalidArgument;
        }

        if (parent !== element.parentNode) {
            parent.appendChild(element);
        }
    }

    function removeElement(element) {
        /// <summary>Removes a DOM element.</summary>
        /// <param name="element" type="HTMLElement" domElement="true">The element to remove.</param>

        if (!element) {
            throw invalidArgument;
        }

        if (element.parentNode) {
            element.parentNode.removeChild(element);
        }
    }

    function measureElement(element) {
        /// <summary>Measures the size of a DOM element.</summary>
        /// <param name="element" type="HTMLElement" domElement="true">The element to measure.</param>
        /// <returns type="Object">The element size.</returns>
        var scale = 1.0;
        if (isWinJS1) {
            var scale = Windows.Graphics.Display.DisplayProperties.resolutionScale / 100;
        }
        else {
            var scale = Windows.Graphics.Display.DisplayInformation.getForCurrentView().resolutionScale / 100;
        }
        var w = Math.ceil(WinJS.Utilities.getTotalWidth(element) * scale); // use ceil instead of round because WinJS reports whole numbers only
        var h = Math.ceil(WinJS.Utilities.getTotalHeight(element) * scale);
        return { width: w, height: h };
    }
    
    function addHideFocusClass(element) {
        /// <summary>Prevents the specified element from showing focus.</summary>
        /// <param name="element" type="HTMLElement">The target element.</param>

        if (element) {
            WinJS.Utilities.addClass(element, "pf-hide-focus");

            var onFocusOut = function (e) {
                if (element !== document.activeElement) {
                    WinJS.Utilities.removeClass(element, "pf-hide-focus");
                    element.removeEventListener("focusout", onFocusOut, false);
                }
            };

            element.addEventListener("focusout", onFocusOut, false);
        }
    }

    function createEventProperties(/* arg1, arg2, argN */) {
        /// <summary>Creates an object that has one event for each name in the arrays provided.</summary>
        /// <returns type="Object">The object with the specified event properties.</returns>

        var events = [];

        // concatenate the events
        for (var i = 0; i < arguments.length; i++) {
            events = events.concat(arguments[i]);
        }

        // lowercase the event names
        events = events.map(function (name) {
            return name.toLowerCase();
        })

        return WinJS.Utilities.createEventProperties.apply(null, events);
    }

    function addEventListener(type, target, listener, useCapture) {
        /// <summary>Adds an event listener to the specified target.</summary>
        /// <param name="type" type="String">The event type.</param>
        /// <param name="target" type="Object">The event target.</param>
        /// <param name="listener" type="Function">The event listener to add.</param>
        /// <param name="useCapture" type="Boolean" optional="true">True to initiate capture.</param>

        if (type === "resize" && typeof target.attachEvent === "function") {
            target.attachEvent("on" + type, listener);
        } else {
            target.addEventListener(type, listener, !!useCapture);
        }
    }

    function removeEventListener(type, target, listener, useCapture) {
        /// <summary>Removes an event listener from the specified target.</summary>
        /// <param name="type" type="String">The event type.</param>
        /// <param name="target" type="Object">The event target.</param>
        /// <param name="listener" type="Function">The event listener to remove.</param>
        /// <param name="useCapture" type="Boolean" optional="true">True if the event listener was registered as a capturing listener.</param>

        if (type === "resize" && typeof target.detachEvent === "function") {
            target.detachEvent("on" + type, listener);
        } else {
            target.removeEventListener(type, listener, !!useCapture);
        }
    }

    function processAll(rootElement, dataContext, skipRoot, bindingCache) {
        /// <summary>Binds values from the specified data context to an element and its descendants.</summary>
        /// <param name="rootElement" type="HTMLElement" optional="true">The element to search for binding declarations.</param>
        /// <param name="dataContext" type="Object" optional="true">The object containing values to use for data binding.</param>
        /// <param name="skipRoot" type="Boolean" optional="true">True to skip the root element during binding.</param>
        /// <param name="bindingCache">The cached binding data.</param>
        /// <returns type="WinJS.Promise">A promise that completes when all binding declarations have been processed.</returns>

        var promises = [];

        // process root element
        var promise = WinJS.Binding.processAll(rootElement, dataContext, skipRoot, bindingCache);
        promises.push(promise);

        return WinJS.Promise.join(promises);
    }

    function getPropertyValue(obj, properties) {
        /// <summary>Gets a property value on an object using the specified path.</summary>
        /// <param name="obj" type="Object">The object.</param>
        /// <param name="properties" type="Array">The path on the object to the property.</param>
        /// <returns type="Object">The property value.</returns>

        var value = WinJS.Utilities.requireSupportedForProcessing(obj);

        if (properties) {
            for (var i = 0; i < properties.length && value !== null && value !== undefined; i++) {
                var property = properties[i];
                if (value instanceof PlayerFramework.InteractiveViewModel) {
                    value = value[property];
                }
                else {
                    value = WinJS.Utilities.requireSupportedForProcessing(value[property]);
                }
            }
        }

        return value;
    }

    function setPropertyValue(obj, properties, value) {
        /// <summary>Sets a property value on an object using the specified path.</summary>
        /// <param name="obj" type="Object">The object.</param>
        /// <param name="properties" type="Array">The path on the object to the property.</param>
        /// <param name="value" type="Object">The value to set.</param>

        WinJS.Utilities.requireSupportedForProcessing(value);
        var target = WinJS.Utilities.requireSupportedForProcessing(obj);

        if (properties) {
            for (var i = 0; i < properties.length - 1; i++) {
                var property = properties[i];
                target = WinJS.Utilities.requireSupportedForProcessing(target[property]);
            }

            var property = properties[properties.length - 1];
            target[property] = value;
        }
    }

    function setIconText(source, sourceProperties, dest, destProperties) {
        /// <summary>Sets text to the icon property for controls which only support one-letter icon glyphs or images (e.g. AppBarCommand).</summary>
        /// <param name="source" type="Object">The source object.</param>
        /// <param name="sourceProperties" type="Array">The path on the source object to the source property.</param>
        /// <param name="dest" type="Object">The destination object.</param>
        /// <param name="destProperties" type="Array">The path on the destination object to the destination property.</param>

        var value = getPropertyValue(source, sourceProperties);
        var element = dest.querySelector(".win-commandimage");

        // see ui.js (line 32564)
        element.innerText = value;
        element.style.backgroundImage = "";
        element.style.msHighContrastAdjust = "";
    }

    function setEventHandler(source, sourceProperties, dest, destProperties) {
        /// <summary>Sets an event handler using the specified source as the context.</summary>
        /// <param name="source" type="Object">The source object.</param>
        /// <param name="sourceProperties" type="Array">The path on the source object to the source property.</param>
        /// <param name="dest" type="Object">The destination object.</param>
        /// <param name="destProperties" type="Array">The path on the destination object to the destination property.</param>

        var value = getPropertyValue(source, sourceProperties);

        if (value) {
            // we can safely mark the resulting bind function as supported for processing because value has already been verified in getPropertyValue
            var eventHandler = WinJS.Utilities.markSupportedForProcessing(value.bind(source));
            setPropertyValue(dest, destProperties, eventHandler);
        }
    }

    function setTransitionEndEventHandler(source, sourceProperties, dest, destProperties) {
        /// <summary>Sets the transitionend event handler (since the ontransitionend property does not exist) using the specified source as the context.</summary>
        /// <param name="source" type="Object">The source object.</param>
        /// <param name="sourceProperties" type="Array">The path on the source object to the source property.</param>
        /// <param name="dest" type="Object">The destination object.</param>
        /// <param name="destProperties" type="Array">The path on the destination object to the destination property.</param>

        var value = getPropertyValue(source, sourceProperties);

        if (value) {
            var eventHandler = value.bind(source);
            dest.addEventListener("transitionend", eventHandler, false);
        }
    }

    var mediaPackUrl = "http://www.microsoft.com/en-ie/download/details.aspx?id=30685";

    function isMediaPackRequired() {
        try {
            var junk = Windows.Media.VideoEffects.videoStabilization;
        }
        catch (error) {
            if (error.number === -2147221164) { // 'Class Not Registered'
                return true;
            }
        }
        return false;
    }

    function testForMediaPack() {
        if (isMediaPackRequired()) {
            promptForMediaPack();
            return false;
        }
        return true;
    }

    function promptForMediaPack() {
        var messageDialog = new Windows.UI.Popups.MessageDialog(PlayerFramework.Utilities.getResourceString("MediaFeaturePackRequiredLabel"), PlayerFramework.Utilities.getResourceString("MediaFeaturePackRequiredText"));
        //Add buttons and set their callback functions
        messageDialog.commands.append(new Windows.UI.Popups.UICommand(PlayerFramework.Utilities.getResourceString("MediaFeaturePackDownloadLabel"),
           function (command) {
               return Windows.System.Launcher.launchUriAsync(new Windows.Foundation.Uri(mediaPackUrl));
           }.bind(this)));
        messageDialog.commands.append(new Windows.UI.Popups.UICommand(PlayerFramework.Utilities.getResourceString("MediaFeaturePackCancelLabel")));
        return messageDialog.showAsync();
    }

    // Mixins
    var eventBindingMixin = {
        _eventBindings: null,

        _bindEvent: function (type, target, callback /* , arg1, arg2, argN */) {
            if (!this._eventBindings) {
                this._eventBindings = [];
            }

            var listener;

            if (arguments.length > 3) {
                var args = Array.prototype.slice.call(arguments, 3);
                args.unshift(this);

                listener = callback.bind.apply(callback, args);
            } else {
                listener = callback.bind(this);
            }

            var binding = {
                type: type,
                target: target,
                callback: callback,
                listener: listener,
                useCapture: false
            };

            this._eventBindings.push(binding);
            addEventListener(binding.type, binding.target, binding.listener, binding.useCapture);
        },

        _unbindEvent: function (type, target, callback) {
            if (this._eventBindings) {
                var bindings = this._eventBindings.filter(function (binding) {
                    return binding.type === type && binding.target === target && binding.callback === callback && binding.useCapture === false;
                });

                bindings.forEach(function (binding) {
                    removeEventListener(binding.type, binding.target, binding.listener, binding.useCapture);
                    remove(this._eventBindings, binding);
                }, this);

                if (!this._eventBindings.length) {
                    this._eventBindings = null;
                }
            }
        },

        _unbindEvents: function () {
            if (this._eventBindings) {
                this._eventBindings.forEach(function (binding) {
                    removeEventListener(binding.type, binding.target, binding.listener, binding.useCapture);
                }, this);

                this._eventBindings = null;
            }
        }
    };

    var propertyBindingMixin = {
        _propertyBindings: null,

        _bindProperty: function (name, target, callback /* , arg1, arg2, argN */) {
            if (!this._propertyBindings) {
                this._propertyBindings = [];
            }

            var action;

            if (arguments.length > 3) {
                var args = Array.prototype.slice.call(arguments, 3);
                args.unshift(this);

                action = callback.bind.apply(callback, args);
            } else {
                action = callback.bind(this);
            }

            var binding = {
                name: name,
                target: target,
                callback: callback,
                action: action
            };

            this._propertyBindings.push(binding);
            binding.target.bind(binding.name, binding.action);
        },

        _unbindProperty: function (name, target, callback) {
            if (this._propertyBindings) {
                var bindings = this._propertyBindings.filter(function (binding) {
                    return binding.name === name && binding.target === target && binding.callback === callback;
                });

                bindings.forEach(function (binding) {
                    binding.target.unbind(binding.name, binding.action);
                    remove(this._propertyBindings, binding);
                }, this);

                if (!this._propertyBindings.length) {
                    this._propertyBindings = null;
                }
            }
        },

        _unbindProperties: function () {
            if (this._propertyBindings) {
                this._propertyBindings.forEach(function (binding) {
                    binding.target.unbind(binding.name, binding.action);
                }, this);

                this._propertyBindings = null;
            }
        }
    };

    // Classes
    var DeferrableOperation = WinJS.Class.define(function () {
        this._promises = [];
    }, {
        getPromise: function () {
            return WinJS.Promise.join(this._promises);
        },

        setPromise: function (promise) {
            this._promises.push(promise);
        }
    });

    // Exports
    WinJS.Namespace.define("PlayerFramework", {
        AdvertisingState: AdvertisingState,
        PlayerState: PlayerState,
        ReadyState: ReadyState,
        NetworkState: NetworkState,
        MediaQuality: MediaQuality,
        MediaErrorCode: MediaErrorCode,
        ImageErrorCode: ImageErrorCode,
        AutohideBehavior: AutohideBehavior,
        InteractionType: InteractionType,
        TextTrackMode: TextTrackMode,
        TextTrackDisplayMode: TextTrackDisplayMode,
        TextTrackReadyState: TextTrackReadyState
    });

    WinJS.Namespace.define("PlayerFramework.Utilities", {
        formatTime: formatTime,
        formatPercentage: formatPercentage,
        formatString: formatString,
        formatResourceString: formatResourceString,
        getResourceString: getResourceString,
        getMediaErrorMessage: getMediaErrorMessage,
        getMediaErrorMessageForCode: getMediaErrorMessageForCode,
        getImageErrorMessageForCode: getImageErrorMessageForCode,
        getImageMimeTypes: getImageMimeTypes,
        getArray: getArray,
        setOptions: setOptions,
        convertDecimalToHex: convertDecimalToHex,
        convertHexToDecimal: convertHexToDecimal,
        convertSecondsToTicks: convertSecondsToTicks,
        convertTicksToSeconds: convertTicksToSeconds,
        convertMillisecondsToTicks: convertMillisecondsToTicks,
        convertTicksToMilliseconds: convertTicksToMilliseconds,
        calculateElapsedTime: calculateElapsedTime,
        calculateRemainingTime: calculateRemainingTime,
        calculateBufferedTime: calculateBufferedTime,
        calculateBufferedPercentage: calculateBufferedPercentage,
        launch: launch,
        clamp: clamp,
        clone: clone,
        extend: extend,
        first: first,
        remove: remove,
        binaryInsert: binaryInsert,
        binarySearch: binarySearch,
        createElement: createElement,
        appendElement: appendElement,
        removeElement: removeElement,
        measureElement: measureElement,
        addHideFocusClass: addHideFocusClass,
        createEventProperties: createEventProperties,
        eventBindingMixin: eventBindingMixin,
        propertyBindingMixin: propertyBindingMixin,
        DeferrableOperation: DeferrableOperation
    });

    WinJS.Namespace.define("PlayerFramework.Binding", {
        processAll: processAll,
        timeConverter: WinJS.Binding.converter(formatTime),
        setIconText: WinJS.Binding.initializer(setIconText),
        setEventHandler: WinJS.Binding.initializer(setEventHandler),
        setTransitionEndEventHandler: WinJS.Binding.initializer(setTransitionEndEventHandler)
    });

    WinJS.Namespace.define("PlayerFramework.MediaPackHelper", {
        mediaPackUrl: mediaPackUrl,
        isMediaPackRequired: isMediaPackRequired,
        testForMediaPack: testForMediaPack,
        promptForMediaPack: promptForMediaPack
    });

})();

