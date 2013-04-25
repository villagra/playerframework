window.PlayerFramework = {};
window.PlayerFramework.Plugins = {};

PlayerFramework.defaultOptions = {};
PlayerFramework.resources = [];

PlayerFramework.setDefaultOptions = function(options)
{
	/// <summary>
	///		Sets the default options for the player framework.
	///	</summary>
	/// <param name="options" type="Object">
	///		The options to merge with the default options.
	///	</param>

	PlayerFramework.merge(PlayerFramework.defaultOptions, options);
};

PlayerFramework.setResources = function(name, resources)
{
	/// <summary>
	///		Sets the resources for a language.
	///	</summary>
	/// <param name="name" type="String">
	///		The resource language name (e.g. en, es, en-us).
	///	</param>
	/// <param name="resources" type="Object">
	///		The resources to merge with the default options.
	///	</param>

	PlayerFramework.resources[name] = resources;
	PlayerFramework.setDefaultOptions(resources);
};

PlayerFramework.addEvent = function(obj, type, fun)
{
	/// <summary>
	///		Wraps the different ways to add event listeners to a DOM element or object.
	///		Based on: http://www.quirksmode.org/blog/archives/2005/10/_and_the_winner_1.html
	///	</summary>
	/// <param name="obj" type="Object">
	///		The DOM element or object to add an event listener to.
	///	</param>
	/// <param name="type" type="String">
	///		The type of event to listen for.
	///	</param>
	/// <param name="fun" type="Function">
	///		The function to call when the event is dispatched.
	///	</param>

	if (window.navigator.msPointerEnabled)
	{
		switch (type)
		{
			case "mousemove":
				obj.addEventListener("MSPointerMove", fun, false);
				break;

			case "mouseup":
				obj.addEventListener("MSPointerUp", fun, false);
				break;

			case "mousedown":
				obj.addEventListener("MSPointerDown", fun, false);
				break;

			case "mouseover":
				obj.addEventListener("MSPointerOver", fun, false);
				break;

			case "mouseout":
				window.addEventListener("MSPointerOver", function(e) {
					if (e.srcElement != obj)
						fun(e);
				}, true);

				break;

			default:
				obj.addEventListener(type, fun, false);
				break;
		}
	}
	else if (obj.addEventListener)
	{
		obj.addEventListener(type, fun, false);
	}
	else if (obj.attachEvent)
	{
		obj["e" + type + fun] = fun;
		obj[type + fun] = function() { obj["e" + type + fun](window.event); }
		obj.attachEvent("on" + type, obj[type + fun]);
	}
};

PlayerFramework.getComputedStyle = function(element, name)
{
	///	<summary>
	///		Gets the CSS style for a given element and given style name.
	///		Based on: http://www.quirksmode.org/dom/getstyles.html
	///	</summary>
	///	<param name="element" type="Object">
	///		The element object to get the style for.
	///	</param>
	///	<param name="name" type="String">
	///		The name of the style attribute.
	///	</param>
	///	<returns type="String" />

	if (element.currentStyle)
		return element.currentStyle[name];
	else if (window.getComputedStyle)
		return document.defaultView.getComputedStyle(element, null).getPropertyValue(name);
};

PlayerFramework.removeEvent = function(obj, type, fun)
{
	/// <summary>
	///		Wraps the different ways to remove event listeners from a DOM element or object.
	///		Based on: http://www.quirksmode.org/blog/archives/2005/10/_and_the_winner_1.html
	///	</summary>
	/// <param name="obj" type="Object">
	///		The DOM element or object to remove an event listener from.
	///	</param>
	/// <param name="type" type="String">
	///		The type of event to stop listening for.
	///	</param>
	/// <param name="fun" type="Function">
	///		The function that was to be called when the event was dispatched.
	///	</param>

	if (obj.removeEventListener)
	{
		obj.removeEventListener(type, fun, false);
	}
	else if (obj.detachEvent)
	{
		obj.detachEvent("on" + type, obj[type + fun]);
		obj[type + fun] = null;
		obj["e" + type + fun] = null;
	}
};

PlayerFramework.padString = function(value, padLength, padString)
{
	/// <summary>
	///		Creates a string of a specified length by appending the pad string to the original value.
	///	</summary>
	/// <param name="value" type="String">
	///		The value to pad.
	///	</param>
	/// <param name="padLength" type="Number">
	///		The desired length of the resulting string.
	///	</param>
	/// <param name="padString" type="String">
	///		The string to append the the original value.
	///	</param>
	///	<returns type="String" />

	var result = new String(value);
	
	while (result.length < padLength)
		result = padString + result;
	
	return result;
};

PlayerFramework.typeExtendsFrom = function(derived, base)
{
	/// <summary>
	///		Determines if a derived type extends from a base type.
	///	</summary>
	/// <param name="derived" type="Object">
	///		The derived type.
	///	</param>
	/// <param name="base" type="Object">
	///		The base type.
	///	</param>
	///	<returns type="Boolean" />

	return derived.prototype instanceof base;
};

PlayerFramework.proxy = function(context, fun)
{
	///	<summary>
	///		Creates a delegate function that executes the specified method in the correct context.
	///	</summary>
	///	<param name="context" type="Object">
	///		The instance whose method should be executed.
	///	</param>
	///	<param name="fun" type="Function">
	///		The function to execute.
	///	</param>
	///	<returns type="Function" />
	
	var proxy = function()
	{
		return fun.apply(context, arguments);
	};

	return proxy;
};

PlayerFramework.mouseEventProxy = function(element, eventType)
{
	///	<summary>
	///		Returns a function that, when called, dispatches a mouse event on the specified element.
	///	</summary>
	///	<param name="element" type="Object">
	///		The element from which to dispatch the event.
	///	</param>
	///	<param name="eventType" type="String">
	///		The type of event to dispatch.
	///	</param>
	///	<returns type="Function" />

	var proxy = PlayerFramework.proxy(this, function(e)
	{
		var event = document.createEvent("MouseEvents");
		event.initMouseEvent(eventType, true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
		element.dispatchEvent(event);
	});

	return proxy;
};

PlayerFramework.xhr = function(options, completeCallback, errorCallback)
{
	///	<summary>
	///		Wraps an XMLHttpRequest.
	///	</summary>
	///	<param name="completeCallback" type="Function">
	///		The function to call when the request has completed.
	///	</param>
	///	<param name="errorCallback" type="Function">
	///		The function to call when request resulted in an error.
	///	</param>

	var request = null;
	
	// Set the inner request to wrap.
	if (window.XMLHttpRequest)
		request = new XMLHttpRequest();
	else if (window.ActiveXObject)
		request = new ActiveXObject("Microsoft.XMLHTTP");
	else
		throw new Error("XMLHttpRequest unavailable");

	request.onreadystatechange = function()
	{
		if (request.readyState == 4)
		{
			if(request.status >= 200 && request.status <= 300)
			{
				completeCallback(request);
			}
			else
			{
				errorCallback(request);
			}
		}
	};

	request.open(options.method || "GET", options.url, true);

	request.responseType = options.responseType || "";

	if (options.data)
	{
		request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
		request.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
		request.setRequestHeader("Connection", "close");
	}

	request.send(options.data);
};

PlayerFramework.merge = function(destination, source)
{
	///	<summary>
	///		Merges properties from a source object into a destination object.
	///	</summary>
	///	<param name="destination" type="Object">
	///		The destination object.
	///	</param>
	///	<param name="source" type="Object">
	///		The source object.
	///	</param>
	///	<returns type="Object" />

	if (!source)
		return;
	
	for (var i in source)
	{
		var destinationProperty = destination[i];
		var sourceProperty = source[i];
		
		if (sourceProperty == null)
			delete destination[i];
		else if (typeof(destinationProperty) == "object"
				&& typeof(sourceProperty) == "object"
				&& !(destinationProperty instanceof Array))
			PlayerFramework.merge(destinationProperty, sourceProperty);
		else
			destination[i] = sourceProperty;
	}
};

PlayerFramework.mergeOptions = function(superOptions, defaultOptions)
{
	var mergedOptions = {};
	PlayerFramework.merge(mergedOptions, superOptions);
	PlayerFramework.merge(mergedOptions, defaultOptions);
	return mergedOptions;
};

PlayerFramework.convertNodeListToArray = function(nodeList /*, thisp */)
{
	///	<summary>
	///		Takes a NodeList of DOM elements and converts it to an Array.
	///	</summary>
	///	<param name="nodeList" type="Object">
	///		NodeList of DOM elements.
	///	</param>
	///	<returns type="Array" />

	"use strict";

	if (nodeList === void 0 || nodeList === null)
		throw new TypeError();

	var t = Object(nodeList);
	var len = t.length >>> 0;

	var res = [];

	for (var i = t.length; i--; res.unshift(t[i])) {};
	
	return res;
};

PlayerFramework.getCharCode = function (e)
{
	///	<summary>
	///		Gets the character code for an event.
	///	</summary>
	///	<param name="e" type="Object">
	///		The event.
	///	</param>
	///	<returns type="Number" />

	if (e.which == null && (e.charCode != null || e.keyCode != null))
		return (e.charCode != null) ? e.charCode : e.keyCode;
	else
		return e.which;
};

PlayerFramework.getElementsByClass = function (className, node, tag)
{
	///	<summary>
	///		Gets all elements with the specified class name.
	///	</summary>
	///	<param name="className" type="String">
	///		The class name to filter by.
	///	</param>
	///	<param name="node" type="Object">
	///		The optional node to find children under (document is used if no node is specified).
	///	</param>
	///	<param name="tag" type="String">
	///		The optional tag name to filter by.
	///	</param>
	///	<returns type="Array" />

	var classElements = [];

	if (!node)
		node = document;

	if (!tag)
		tag = "*";
	
	var tagElements = node.getElementsByTagName(tag);
	var pattern = new RegExp("(^|\\s)" + className + "(\\s|$)");

	PlayerFramework.forEach(tagElements, function(element)
	{
		if (pattern.test(element.className))
			classElements.push(element);
	});

	return classElements;
};

PlayerFramework.getTotalOffsetLeft = function(element)
{
	///	<summary>
	///		Gets the left position relative to the top-most parent by recursively summing the left position of the parent.
	///	</summary>
	///	<param name="element" type="Number">
	///		The element to find the left position for.
	///	</param>
	///	<returns type="Number" />

	return element ? (element.offsetLeft + PlayerFramework.getTotalOffsetLeft(element.offsetParent)) : 0;
};

PlayerFramework.forEachAsync = function(array, fun /*, thisp */)
{
	///	<summary>
	///		Iterates over an array allowing the called function to explicitly continue iterating
	///		by calling a loop callback function passed to the called function.
	///	</summary>
	///	<param name="array" type="Array">
	///		The array to iterate over.
	///	</param>
	///	<param name="fun" type="Function">
	///		The function to call at each iteration.
	///	</param>

	"use strict";

	if (array === void 0 || array === null)
		throw new TypeError();

	var t = Object(array.concat());

	if (typeof fun !== "function")
		throw new TypeError();

	var loop = function()
	{
		var len = t.length >>> 0;
		if (len === 0)
			return;

		var thisp = arguments[2];
		var i = t.shift();

		fun.call(thisp, loop, i);
	};
	loop();
};

PlayerFramework.first = function(array, fun /*, thisp */)
{
	///	<summary>
	///		Gets the first item in the array that causes the function to return true when the
	///		function is called with the item as a parameter.
	///	</summary>
	///	<param name="array" type="Array">
	///		The array to iterate over.
	///	</param>
	///	<param name="fun" type="Function">
	///		The function to call at each iteration that must return a boolean value to be returned
	///		as the first item.
	///	</param>
	///	<returns type="Object" />

	"use strict";

	if (array === void 0 || array === null)
		throw new TypeError();

	var t = Object(array);
	var len = t.length >>> 0;
	if (typeof fun !== "function")
		throw new TypeError();

	var thisp = arguments[2];
	for (var i = 0; i < len; i++)
	{
		if (i in t && fun.call(thisp, t[i], i, t))
			return t[i];
	}

	return null;
};

PlayerFramework.filter = function(array, fun /*, thisp */)
{
	///	<summary>
	///		Gets all items in the array that cause the function to return true when the function is
	///		called with the item as a parameter.
	///		Based on: https://developer.mozilla.org/en/JavaScript/Reference/Global_Objects/Array/filter#Compatibility
	///	</summary>
	///	<param name="array" type="Array">
	///		The array to iterate over.
	///	</param>
	///	<param name="fun" type="Function">
	///		The function to call at each iteration that must return a boolean value to be included
	///		in the resulting array.
	///	</param>
	///	<returns type="Array" />

	"use strict";

	if (array === void 0 || array === null)
		throw new TypeError();

	var t = Object(array);
	var len = t.length >>> 0;
	if (typeof fun !== "function")
		throw new TypeError();

	var res = [];
	var thisp = arguments[2];
	for (var i = 0; i < len; i++)
	{
		if (i in t)
		{
		var val = t[i]; // in case fun mutates this
		if (fun.call(thisp, val, i, t))
			res.push(val);
		}
	}

	return res;
};

PlayerFramework.forEach = function(array, fun /*, thisp */)
{
	///	<summary>
	///		Iterates over each item in the array.
	///		Based on: https://developer.mozilla.org/en/JavaScript/Reference/Global_Objects/Array/forEach#Compatibility
	///	</summary>
	///	<param name="array" type="Array">
	///		The array to iterate over.
	///	</param>
	///	<param name="fun" type="Function">
	///		The function to call at each iteration.
	///	</param>

	"use strict";

	if (array === void 0 || array === null)
		throw new TypeError();

	var t = Object(array);
	var len = t.length >>> 0;
	if (typeof fun !== "function")
		throw new TypeError();

	var thisp = arguments[2];
	for (var i = 0; i < len; i++)
	{
		if (i in t)
			fun.call(thisp, t[i], i, t);
	}
};

PlayerFramework.requestAnimationFrame = (function()
{
	///	<summary>
	///		Sets the requestAnimationFrame function using any available browser-specific
	///		requestAnimationFrame or using setTimeout as a substitute. 
	///		Based on: http://paulirish.com/2011/requestanimationframe-for-smart-animating/
	///	</summary>
	///	<returns type="Function" />

	var requestAnimationFrameFunction =
		window.requestAnimationFrame ||
		window.webkitRequestAnimationFrame ||
		window.mozRequestAnimationFrame ||
		window.oRequestAnimationFrame ||
		window.msRequestAnimationFrame ||
		function(callback, element)
		{
			window.setTimeout(callback, 1000 / 60);
		};

	return PlayerFramework.proxy(window, requestAnimationFrameFunction);
})();

PlayerFramework.createElement = function(parentNode, jsonml)
{
	///	<summary>
	///		Creates an HTML element given the formatted JSONML array.
	///	</summary>
	///	<param name="parentNode" type="Object">
	///		The optional parent node to append the created element to.
	///	</param>
	///	<param name="jsonml" type="Array">
	///		The JSONML array containing the tag name and optional attributes and nested elements.
	///	</param>
	///	<returns type="Object" />
	
	if (!(jsonml instanceof Array) || jsonml.length === 0 || typeof(jsonml[0]) !== "string")
		throw new Error("Invalid JSONML.");

	var element = document.createElement(jsonml[0]);
	var attributes = jsonml[1];
	
	// Parse and set the element attributes if specified.
	if (attributes && typeof(attributes) === "object" && !(attributes instanceof Array)) 
	{			
		for (var key in attributes)
		{
			var value = attributes[key];

			if (value !== null && typeof(value) !== "undefined")
			{
				if (typeof(value) !== "string")
					element[key] = value;
				else
					element.setAttribute(key, value);
			}
		}
	}
	
	// Append child nodes.
	for (var i = 1; i < jsonml.length; i++) 
	{
		var childJsonml = jsonml[i];

		if (childJsonml instanceof Array)
			PlayerFramework.createElement(element, childJsonml);
		else if (typeof(childJsonml) === "string")
			element.appendChild(document.createTextNode(childJsonml));
	}

	if (parentNode)
		parentNode.appendChild(element);

	return element;
};

PlayerFramework.domReady = (function()
{
	/// <summary>
	///		Determines when the DOM is fully loaded and fires any registered handlers.
	///		Simplified adaptation based on work by Dean Edwards, John Resig, Matthias Miller, and Diego Perini.
	/// </summary>

	var handlers = [],
		loaded = false;
		
	var domContentLoaded = function ()
	{
		if (document.removeEventListener)
			document.removeEventListener("DOMContentLoaded", domContentLoaded, false);
		else if (document.detachEvent && document.readyState === "complete")
			document.detachEvent("onreadystatechange", domContentLoaded);

		ready();
	};

	var ready = function ()
	{
		if (!loaded)
		{
			loaded = true;

			for (var i = 0, len = handlers.length; i < len; i++)
				handlers[i].call(document);
		}
	};

	if (document.addEventListener)
	{
		document.addEventListener("DOMContentLoaded", domContentLoaded, false);
		window.addEventListener("load", ready, false);
	}
	else if (document.attachEvent)
	{
		document.attachEvent("onreadystatechange", domContentLoaded);
		window.attachEvent("onload", ready);

		if (document.documentElement.doScroll)
		{
			var intervalId = window.setInterval(function ()
			{
				try
				{
					// Throws an error if document is not ready yet
					document.documentElement.doScroll("left");
					window.clearInterval(intervalId);
					ready();
				} catch (e) {}
			}, 10);
		}
	}

	return function (handler)
	{
		// If the DOM is already loaded, execute the handler
		if (loaded)
			handler.call(document);
		else
			handlers.push(handler);
	};
})();
