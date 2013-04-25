PlayerFramework.TtmlParser = PlayerFramework.Object.extend(
{
	init: function(options)
	{
		///	<summary>
		///		Parses a TTML file per the W3C specification: http://www.w3.org/TR/ttaf1-dfxp/
		///		Based on a library written by Sean Hayes.
		///	</summary>

		this._super();
		
		this.mergeOptions(options,
		{
			xmlNamespace: 'http://www.w3.org/XML/1998/namespace',
			ttmlNamespace: "http://www.w3.org/ns/ttml",
			ttmlStyleNamespace: "http://www.w3.org/ns/ttml#styling",
			ttmlMetaNamespace: "http://www.w3.org/ns/ttml#metadata",
			smpteNamespace: "http://www.smpte-ra.org/schemas/2052-1/2010/smpte-tt",
			trackIdPrefix: "",
			mediaFrameRate: 30,
			mediaFrameRateMultiplier: 1,
			mediaSubFrameRate: 1,
			mediaTickRate: 1000,
			mediaStart: 0,
			mediaDuration: Math.pow(2, 53), // maximum JavaScript integer
			clocktime: /^(\d{2,}):(\d{2}):(\d{2})((?:\.\d{1,})|:(\d{2,}(?:\.\d{1,})?))?$/, // hours ":" minutes ":" seconds ( fraction | ":" frames ( "." sub-frames )? )?
			offsettime: /^(\d+(\.\d+)?)(ms|[hmsft])$/ // time-count fraction? metric
		});

		this.root = null;
		this.layout = null;
		this.head = null;
		this.body = null;
		this.regions = null;

		// True unless we see a region definition in the TTML.
		this.usingDefaultRegion = true;

		// Ordered list of events containing times (in ms) and corresponding element.
		this.TTMLEvents = [];

		// List of all audio descriptions.
		this.Descriptions = [];

		// Tree of navigation points.
		this.Navigation = null;

		// Store styles here because IE doesnt support expando's on XML elements.
		this.StyleSetCache = {};
		this.styleSetId = 0;

		// SMPTE-TT image support.
		this.ImageCache = {};
				
		// Keep track of the rightmost element at each level, so that we can include left and right links.
		this.rightMostInLevel = [];

		this.nodeType =
		{
			ELEMENT_NODE: 1,
			ATTRIBUTE_NODE: 2,
			TEXT_NODE: 3,
			CDATA_SECTION_NODE: 4,
			ENTITY_REFERENCE_NODE: 5,
			ENTITY_NODE: 6,
			PROCESSING_INSTRUCTION_NODE: 7,
			COMMENT_NODE: 8,
			DOCUMENT_NODE: 9,
			DOCUMENT_TYPE_NODE: 10,
			DOCUMENT_FRAGMENT_NODE: 11,
			NOTATION_NODE: 12
		};
	},

	parseTtml: function(source)
	{
		// Source is either a wrapped XMLDocument, or a string.
		this.xmlDoc = (typeof source == "string") ? this.parseXml(source) : source; 

		this.root = this.getElementsByTagNameNS(this.xmlDoc, this.options.ttmlNamespace, "tt")[0];
		
		// Exactly one tt root object allowed.
		if (this.root)
		{
			// Find the landmark nodes in the XML.
			this.body = this.getElementsByTagNameNS(this.root, this.options.ttmlNamespace, 'body')[0] || null;
			this.head = this.getElementsByTagNameNS(this.root, this.options.ttmlNamespace, 'head')[0] || null;
			this.layout = this.head ? (this.getElementsByTagNameNS(this.root, this.options.ttmlNamespace, 'layout')[0] || null) : null;
			
			this.loadSMPTEImages();

			// TTML that doesn't declare any layout regions uses a default region.
			if (this.layout)
			{
				this.regions = this.getElementsByTagNameNS(this.layout, this.options.ttmlNamespace, 'region');
				this.usingDefaultRegion = this.regions.length == 0;
			}
			else
			{
				this.regions = [];
				this.usingDefaultRegion = true;
			}

			// Apply the intervals over the tree.
			this.applyTiming(this.root, { first: this.options.mediaStart, second: this.options.mediaDuration }, true);

			// Use the time containement as a structured navigation map.
			this.Navigation = this.getNavigation(this.body)

			// Apply the style inheritance over the tree.
			this.applyStyling();
		}

		// Unroll using TTMLEvents and getCuesAtTime.
		// This then makes it easier to use the <track> API's and also use the same interface for WebVTT, SRT etc.
		var captionArray = [];
		
		for (var i = 0, max = this.TTMLEvents.length; i < max; i++)
		{
			if (this.TTMLEvents[i].elementScope == this.root)
				continue;

			var cues = this.getCuesAtTime(this.TTMLEvents[i].tick, this.TTMLEvents[i].elementScope);
			
			if (i > 0)
			{
				for (var pastCuesIndex = i - 1; pastCuesIndex >= 0; pastCuesIndex--)
				{
					if (this.TTMLEvents[pastCuesIndex].elementScope == this.root || this.TTMLEvents[pastCuesIndex].elementScope == this.TTMLEvents[i].elementScope)
						continue;

					var overlappingCues = this.getCuesAtTime(this.TTMLEvents[i].tick, this.TTMLEvents[pastCuesIndex].elementScope);

					if (overlappingCues.length != 0)
						cues.push(overlappingCues[0]);
					else
						break;
				}
			}
			
			for (var j = 0, cueCount = cues.length; j < cueCount; j++)
			{
				captionArray.push(
				{
					start: this.TTMLEvents[i].tick,
					end: (i + 1 < max) ? this.TTMLEvents[i + 1].tick : this.options.mediaDuration,
					id: "caption" + (captionArray.length + 1),
					caption: cues[j]
				});
			}
		}
		
		return {
			captions: captionArray,
			descriptions: this.Descriptions,
			navigation: this.Navigation,
			xml: this.root
		};
	},

	parseXml: function(data)
	{
		var xml;

		try
		{
			if (window.DOMParser)
			{
				var domParser = new DOMParser();
				xml = domParser.parseFromString(data, "application/xml");
			}
			else
			{
				xml = new ActiveXObject("Microsoft.XMLDOM");
				xml.async = false;
				xml.loadXML(data);
			}
		}
		catch (err)
		{
			xml = null;
		}

		return xml;
	},

	getCuesAtTime: function(tick, elementScope) 
	{
		// Get cue for a given time instant.
		var cues = [];

		if (elementScope && this.isTemporallyActive(elementScope, tick))
		{
			if (!this.usingDefaultRegion)
			{
				PlayerFramework.forEach(this.regions, PlayerFramework.proxy(this, function (region)
				{
					var regionId = this.attrNS(region, 'id', this.options.xmlNamespace);
					var div = this.translateMarkup(region, tick);

					if (div)
					{
						// Create a new subtree for the body element, prune elements not associated
						// with the region, and if its not empty then select it into this region by
						// adding it to div container.
	
						var prunedElement = this.prune(elementScope, regionId, tick);

						if (prunedElement)
							div.appendChild(prunedElement);

						var showBackground = div.getAttribute("data-showBackground") != 'whenActive';
						var text = div.innerHTML.replace(/\s/g, "");

						if (showBackground && text != "")
							cues.push(div);
					}
				}));
			}
			else
			{
				var div = PlayerFramework.createElement(null,
				[
					"div",
					{
						"class": "cue"
					}
				]);

				var prunedElement = this.prune(elementScope, "", tick)

				if (prunedElement)
					div.appendChild(prunedElement);

				if (this.children(div).length > 0)
					cues.push(div);
			}
		}

		return cues;
	},

	applyTiming: function(element, bound, isParallelContext) 
	{
		// Walk the tree to determine the absolute start and end times of all the 
		// elements using the TTML subset of the SMIL timing model.
		// The reference times passed in 'bound' are absolute times, the result of 
		// calling this is to set the local start time and end time to absolute times 
		// between these two reference times, based on the begin, end and dur attributes
		// and to recursively set all of the children.
		
		var begin, end;
		var startTime, endTime;
		var defaultDur, dur;

		if (this.hasAttr(element, "begin"))
		{
			// Begin attested.
			begin = this.getTime(this.getAttr(element, "begin")) + 0.01; // workaround to allow cues that begin exactly when the previous cue ends
			startTime = bound.first + begin;
		}
		else
		{
			startTime = bound.first;
		}

		if (!this.hasAttr(element, "dur") && !this.hasAttr(element, "end"))
		{
			// No direct timing attested, so use default based on context.
			if (isParallelContext)
			{
				// "par" children have indefinite default duration, truncated by bounds.
				// "seq" children have zero default duration.
				if (startTime <= bound.second)
				{
					defaultDur = Math.max(0, bound.second - startTime);
					endTime = bound.second;
				}
				else
				{
					defaultDur = 0;
					endTime = 0;
				}
			}
		}
		else if (this.hasAttr(element, "dur") && this.hasAttr(element, "end"))
		{
			// Both dur and end attested, the minimum interval applies.
			dur = this.getTime(this.getAttr(element, "dur"));
			end = this.getTime(this.getAttr(element, "end"));
			var minEnd = Math.min(startTime + dur, bound.first + end);
			endTime = Math.min(minEnd, bound.second);
		}
		else if (this.hasAttr(element, "end"))
		{
			// Only end attested.
			end = this.getTime(this.getAttr(element, "end"));
			endTime = Math.min(bound.first + end, bound.second);
		}
		else
		{
			// Only dur attested.
			dur = this.getTime(this.getAttr(element, "dur"));
			endTime = Math.min(startTime + dur, bound.second);
		}

		if (endTime < startTime)
			endTime = startTime;

		element.setAttribute("data-time-start", startTime);
		element.setAttribute("data-time-end", endTime);

		if (!PlayerFramework.first(this.TTMLEvents, function(event)
		{
			return (event.tick == startTime); // && event.elementScope == element.parentNode);
		}))
		{
			this.TTMLEvents.push(
			{
				tick: startTime,
				elementScope: element
			});
		}
			
		if (!PlayerFramework.first(this.TTMLEvents, function(event)
		{
			return (event.tick == endTime); // && event.elementScope == element.parentNode);
		}))
		{
			this.TTMLEvents.push(
			{
				tick: endTime,
				elementScope: element
			});
		}

		 // Keep events in order.
		this.TTMLEvents.sort(function (a, b) { return a.tick - b.tick; })

		if (this.attrNS(element, 'role', this.options.ttmlMetaNamespace))
		{
			var srcAudio = this.attrNS(element, 'audio', 'http://www.microsoft.com/enable#media');

			if (srcAudio)
			{
				this.Descriptions.push(
				{
					start: startTime,
					end: endTime,
					id: "description" + (this.Descriptions.length + 1),
					uri: srcAudio,
					caption: ""
				});

				// Sort the descriptions by start time.
				this.Descriptions.sort(function (a, b) { return a.start - b.start; });
			}
		}

		var s = startTime;
		var timeContext = this.getAttr(element, "timeContainer");

		PlayerFramework.forEach(this.children(element), PlayerFramework.proxy(this, function (childElement)
		{
			if (timeContext != "seq")
			{
				// Parallel is default so null is OK here.
				this.applyTiming(childElement,
				{
					first: startTime,
					second: endTime
				}, true);
			}
			else
			{
				this.applyTiming(childElement,
				{
					first: s,
					second: endTime
				}, false);
				s = new Number(this.getAttr(childElement, "data-time-end"));
			}
		}));
	},

	getTime: function (timeExpression)
	{
		// Utility object to handle TTML time expressions. Could be improved, but seems to do the job.
		// In particular, we are not currently handling TTML parameters for tick rate and so on.

		// NOTE: IE 10 Consumer Preview cannot parse time formats containing frames (e.g. "00:00:04.18" works, but not "00:00:04:18")
		// To overlay default and native captions for testing/comparison, use the PlayerFramework.TextTrack.DisplayPreference option.

		var time;
		var v1 = this.options.clocktime.exec(timeExpression);
		var v2 = this.options.offsettime.exec(timeExpression);

		if (v1 != null)
		{
			var hours = new Number(v1[1]);
			var minutes = new Number(v1[2]);
			var seconds = new Number(v1[3]);
			var frames = 0;

			if (!isNaN(v1[4]))
				seconds += new Number(v1[4]);

			if (!isNaN(v1[5]))
				frames = new Number(v1[5]);

			time = hours * this.getMetricMultiplier("h") +
				minutes * this.getMetricMultiplier("m") +
				seconds * this.getMetricMultiplier("s") +
				frames * this.getMetricMultiplier("f");
		}
		else if (v2 != null)
		{
			var value = new Number(v2[1]);
			var multiplier = this.getMetricMultiplier(v2[3]);
			
			time = value * multiplier;
		}
		else
		{
			time = 0;
		}

		//console.log("time = " + time);

		return time;
	},

	getMetricMultiplier: function(timeExpression) 
	{
		switch (timeExpression)
		{
			case "h":
				return 1000 * 60 * 60;
			case "m":
				return 1000 * 60;
			case "s":
				return 1000;
			case "ms":
				return 1;
			case "f":
				return 1000 / this.options.mediaFrameRate;
			case "t":
				return 1000 / this.options.mediaTickRate;
			default:
				return 0;
		}
	},

	isTemporallyActive: function(element, time)
	{
		var s = Math.round(1000 * parseFloat(this.getAttr(element, "data-time-start"))) / 1000;
		var e = Math.round(1000 * parseFloat(this.getAttr(element, "data-time-end"))) / 1000;
		var t = Math.round(1000 * time) / 1000;

		//console.log("s = " + s + ", e = " + e + ", t = " + t + ", (s <= t) = " + (s <= t) + ", (t < e) = " + (t < e));

		return (s <= t) && (t < e);
	},

	getNavigation: function(element)
	{
		// Navigation elements are marked with the extensions role="x-nav-..." 
		// we want to find the lists of nav-labels, where each label goes in the right level of list
		// the structure of this is loosely based on daisy NCK files.

		return this.getNavPoint(element, null, 0);
	},

	getNavPoint: function(element, parent, level) 
	{
		// A nav point is an element tagged with role=x-nav-point; containing one label and one list
		// however if the list is empty, then the label can stand on its own for the whole point.

		var label = null;
		var subtree = [];
		var node = {};

		// Keep the high tide mark for how deep in the tree we are.
		if (this.rightMostInLevel.length <= level)
			this.rightMostInLevel.push(null);

		var role = this.attrNS(element, 'role', this.options.ttmlMetaNamespace);
		switch (role)
		{
			case "x-nav-label": // Degenerate form.
				label = this.getNavLabel(element);
				break;
			case "x-nav-point": // Full form contains a label and a list.
				PlayerFramework.forEach(this.children(element), PlayerFramework.proxy(this, function (childElement)
				{
					var childRole = this.attrNS(childElement, 'role', this.options.ttmlMetaNamespace);
					switch (childRole)
					{ 
						// Should only be one of each. but allow last to win.
						case "x-nav-label": // Contains text, and use its timing.
							label = this.getNavLabel(childElement);
							break;
						case "x-nav-list": // Contains either a list of navPoints.
							subtree = this.getNavList(childElement, node, level + 1);
							break;
						default:
							break;
					}
				}));
				break;
			default:
				break; // Ignore anything else.
		}

		if (label != null)
		{
			node.text = label.text;
			node.startTime = new Number(label.startTime) / 1000 + 0.01;
			node.endTime = new Number(label.endTime) / 1000;
			node.parent = parent;
			node.left = this.rightMostInLevel[level]; // Could be null.
			node.right = null;
			node.children = subtree;

			if (this.rightMostInLevel[level] != null)
			{
				this.rightMostInLevel[level].right = node;
			}
			
			this.rightMostInLevel[level] = node;

			return node;
		}
		else
		{
			return null;
		}
	},

	getNavLabel: function(element) 
	{
		// A nav label is just text, but we use its timing to create an interval into the media for navigation.

		var role = this.attrNS(element, 'role', this.options.ttmlMetaNamespace);
		if (role != null && !PlayerFramework.first(role, function(r) { return r === "x-nav-label"; } )) 
		{
			return {
				text: element.innerHTML,
				startTime: this.getAttr(element, "data-time-start"),
				endTime: this.getAttr(element, "data-time-end")
			}
		}
	},

	getNavList: function(element, node, level)
	{
		// A navList is supposed to be a list of navPoints, but a navPoint can be a degenerate navLabel

		var list = [];
		var role = this.attrNS(element, 'role', this.options.ttmlMetaNamespace);

		if (role != null && !PlayerFramework.first(role, function(r) { return r === "x-nav-list"; } ))
		{
			PlayerFramework.forEach(this.children(element), function (childElement)
			{
				var item = getNavPoint(childElement, node, level);
				if (item != null)
				{
					list.push(item);
				}
			});
		}

		return list;
	},

	translateMarkup: function(element, tick)
	{
		// Convert elements in TTML to their equivalent in HTML.

		var translation = undefined;
		var name = this.getTagNameEquivalent(element);
		var htmlName = "";
		var htmlClass = "";
		var htmlAttrs = {};

		if (element && this.isTemporallyActive(element, tick))
		{
			switch (name) {
				case "tt:region":
				case "tt:tt": // We get this if there is no region.
					htmlClass = "cue "; // To simulate the ::cue selector.
					htmlName = "div";
					break;
				case "tt:body":
				case "tt:div":
					htmlName = "div";
					break;
				case "tt:p":
					htmlName = "p";
					break;
				case "tt:span":
					htmlName = "span";
					break;
				case "tt:br":
					htmlName = "br";
					break;
				case "":
					break;
				default:
					htmlName = name;
					
					PlayerFramework.forEach(PlayerFramework.convertNodeListToArray(element.attributes), function (pair)
					{
						htmlAttrs[pair.name] = pair.value;
					});

					break;
			}
			var role = this.attrNS(element, 'role', this.options.ttmlMetaNamespace);
			if (role) htmlClass += ((role) + ' ');

			var classAttr = this.attrNS(element, 'class', 'http://www.w3.org/1999/xhtml');
			if (classAttr) htmlClass += ((classAttr) + ' ');

			// Hack until display:ruby on other elements works.
			if (role == "x-ruby") htmlName = ("ruby");
			if (role == "x-rubybase") htmlName = ("rb");
			if (role == "x-rubytext") htmlName = ("rt");

			// Convert image based captions here; and move the text into its alt.
			// If I could get inline CSS to work div's then this would be set as style. 
			var image = this.getAttr(element, 'image');
			if (image != null)
				htmlName = "img"

			if (htmlName != "")
			{
				translation = PlayerFramework.createElement(null,
				[
					htmlName,
					{
						"class": htmlClass.replace(/^\s+|\s+$/g, "") || null
					}
				]);

				// Move ID's over. Use trackIdPrefix to ensure there are no name clases on id's already in target doc.
				var id = this.attrNS(element, 'id', this.options.xmlNamespace);
				
				// If (id) translation.attr('id', this.options.trackIdPrefix + id);
				if (id) translation.setAttribute('id', this.options.trackIdPrefix + id);

				// Copy style from element over to html, translating into CSS as we go
				this.translateStyle(element, translation, tick);

				// If we are copying over html elements, then copy any attributes too.
				for (attr in htmlAttrs)
				{
					translation.setAttribute(attr, htmlAttrs[attr]);
				}

				if (image != null)
				{
					translation.setAttribute("src", image);
					translation.setAttribute("alt", element.innerHTML);
				}
			}
		}

		return translation;
	},

	translateStyle: function(element, htmlElement, tick) 
	{
		// Convert from TTML style names to CSS.

		// Clone of the base style set.
		var computedStyleSet = {};

		// Iterate over the computed styleset and copy into computed style set.
		var styles = this.StyleSetCache[this.getAttr(element, "__styleSet__")];
		for (var name in styles)
		{
			computedStyleSet[name] = styles[name];
		}

		PlayerFramework.forEach(this.getElementsByTagNameNS(element, this.options.ttmlNamespace, 'set'), PlayerFramework.proxy(this, function (childElement)
		{
			if (this.isTemporallyActive(childElement, tick))
				this.applyInlineStyles(computedStyleSet, childElement);
		}));

		// Apply properties.
		for (var name in computedStyleSet)
		{
			this.convertTtmlToCss(htmlElement, name, computedStyleSet[name]);
		}
	},

	convertTtmlToCss: function(obj, style, value)
	{
		// Convert TTML style into CSS equivalent. Mostly just the name change used in js, but some are slightly different.

		var cssStyle;

		switch (style)
		{
			case "origin":
				var coords = value.split(/\s/); // Get the individual components.
				cssStyle =
				{
					position: "absolute",
					left: coords[0],
					top: coords[1]
				};
				break;
			case "extent":
				var coords = value.split(/\s/); // Get the individual components.
				cssStyle = 
				{
					width: coords[0],
					height: coords[1]
				};
				break;
			case "displayAlign":
				cssStyle =
				{
					textAlign: value
				};
				break;
			case "wrapOption":
				cssStyle =
				{
					whiteSpace: value == "nowrap" ? "nowrap" : "normal"
				};
				break;
			default:
				cssStyle = {};
				cssStyle[style] = value;
				break;
		}

		this.setStyle(obj, cssStyle);

		return obj;
	},

	applyStyling: function() 
	{
		// Apply style to every element in the body.

		PlayerFramework.forEach(this.root.getElementsByTagName("*"), PlayerFramework.proxy(this, function (element)
		{
				this.applyStyle(element);
		}));
	},

	applyStyle: function(element)
	{
		// Apply styles in the correct order to element.

		var styleSet = {};

		// Find all the applicable styles and set them as properties on styleSet. 
		this.applyStylesheet(styleSet, element);
		this.applyInlineStyles(styleSet, element);

		// Record the applied set to the element
		this.StyleSetCache[this.styleSetId] = styleSet;
		element.setAttribute("__styleSet__", this.styleSetId++);
	},

	applyInlineStyles: function(styleSet, element)
	{
		// Apply style attributes into styleset.
		
		PlayerFramework.forEach(PlayerFramework.convertNodeListToArray(element.attributes), PlayerFramework.proxy(this, function (attribute)
		{
			if (attribute.namespaceURI == this.options.ttmlStyleNamespace)
			{
				styleSet[this.getLocalTagName(attribute)] = attribute.nodeValue;
			}

			if (attribute.namespaceURI == this.options.smpteNamespace && this.getLocalTagName(this) == "backgroundImage")
			{
				var imageId = attribute.nodeValue;

				if (imageId.indexOf("#") == 0)
					element.setAttribute("image", "data:image/png;base64," + this.ImageCache[imageId]);
				else
					element.setAttribute("image", imageId);
			}
		}));
	},

	applyStylesheet: function(styleSet, element) 
	{
		// For each style id on element, find the corresponding style element 
		// and then apply the stylesheet into styleset; this recurses over the tree of referenced styles.

		if (!this.hasAttr(element, 'style'))
			return;

		var isStyle = this.isTagNS(element, "style", this.options.ttmlNamespace); // Detect if we are referencing style from a style.
		var ids = this.getAttr(element, 'style').split(/\s/); // Find all the style ID references.
			
		PlayerFramework.forEach(ids, PlayerFramework.proxy(this, function (styleId) 
		{
			// Find all the style elements in the TTML namespace.
			PlayerFramework.forEach(this.getElementsByTagNameNS(this.head, this.options.ttmlNamespace, 'style'), PlayerFramework.proxy(this, function (headChildElement)
			{
				if (this.attrNS(headChildElement, 'id', this.options.xmlNamespace) == styleId)
				{
					this.applyStylesheet(styleSet, headChildElement);

					// If the element is region, do nested styles (note regions can only be referenced from elements in the body).
					if (!isStyle && this.isTagNS(headChildElement, "region", this.options.ttmlNamespace))
					{
						//this.getElementsByTagNameNS(element, this.options.ttmlNamespace, 'style').each(function () {
						PlayerFramework.forEach(this.getElementsByTagNameNS(headChildElement, this.options.ttmlNamespace, 'style'), PlayerFramework.proxy(this, function (childElement)
						{
							this.applyStylesheet(styleSet, childElement);
						}));
					}

					// Do inline styles.
					this.applyInlineStyles(styleSet, headChildElement);
				}
			}));
		}));
	},

	isInRegion: function(element, regionId) 
	{
		// A content element is associated with a region according to the following ordered rules, 
		// where the first rule satisfied is used and remaining rules are skipped:

		// Quick test: Out of normal order, but makes following rules simpler.
		if (regionId == "" || regionId == undefined) return this.usingDefaultRegion;

		// 1. If the element specifies a region attribute, then the element is
		// associated with the region referenced by that attribute;
		if (this.getAttr(element, 'region') == regionId) return true;

		// 2. If some ancestor of that element specifies a region attribute, then the element is 
		// associated with the region referenced by the most immediate ancestor that specifies 
		// this attribute;
		var ancestor = element.parentNode;
		while (ancestor != null && ancestor.nodeType == this.nodeType.ELEMENT_NODE) 
		{
			var r = this.getAttr(ancestor, "region");
			if (r)
				return r == regionId;
			ancestor = ancestor.parentNode;
		}

		// 3. If the element contains a descendant element that specifies a region attribute, 
		//	then the element is associated with the region referenced by that attribute;
		var nodes = element.getElementsByTagName('*');
		for (var n = 0, len = nodes.length; n < len; n++)
		{
			if ((this.getAttr(nodes[n], 'region') == regionId) && nodes[n].namespaceURI == this.options.ttmlNamespace)
				// Can't cache this result, because a node may contain more than one region.
				return true;
		}

		// 4. If a default region was implied (due to the absence of any region element), 
		//	then the element is associated with the default region;
		if (this.usingDefaultRegion)
			return regionId == "";

		// 5. The element is not associated with any region.
		return false;
	},

	loadSMPTEImages: function()
	{
		this.ImageCache = {};
		var images = this.getElementsByTagNameNS(this.root, this.options.smpteNamespace, 'image');

		PlayerFramework.forEach(images, PlayerFramework.proxy(this, function (image)
		{
			var id = this.attrNS(image, 'id', this.options.xmlNamespace);
			if (id != null)
				this.ImageCache["#" + id] = image.innerHTML;
		}));
	},

	getTagNameEquivalent: function(element) 
	{
		var tag = this.getLocalTagName(element);
		switch (element.namespaceURI)
		{
			case 'http://www.w3.org/1999/xhtml':
				return tag;
			case 'http://www.w3.org/ns/ttml':
				return 'tt:' + tag;
			case 'http://www.smpte-ra.org/schemas/2052-1/2010/smpte-tt':
				return 'smpte:' + tag;
			default:
				return "";
		}
	},

	prune: function(element, regionId, tick)
	{
		/// Convert the element if it is in the region, then recurse into its contents.
		/// If it ends up with no children, then we dont add it to the region.

		var clone = undefined;
		
		if (this.isInRegion(element, regionId))
		{
			clone = this.translateMarkup(element, tick);

			if (!clone)
				return clone;

			PlayerFramework.forEach(element.childNodes, PlayerFramework.proxy(this, function (childElement)
			{
				if (childElement.nodeType != this.nodeType.COMMENT_NODE)
				{
					if (childElement.nodeType == this.nodeType.TEXT_NODE)
					{
						clone.appendChild(document.createTextNode(childElement.data));
					}
					else
					{
						var prunedChildElement = this.prune(childElement, regionId, tick);

						if (prunedChildElement)
							clone.appendChild(prunedChildElement);
					}
				}
			}));
		}

		return clone;
	},

	getLocalTagName: function(element) 
	{
		if (element.localName) // W3C
			return element.localName;
		else
			return element.baseName;
	},

	getElementsByTagNameNS: function(element, namespace, name) 
	{
		var result = [];
		var nodes = element.getElementsByTagName('*');

		for (var i = 0; i < nodes.length; i++)
		{
			if (this.getLocalTagName(nodes[i]) == name && nodes[i].namespaceURI == namespace)
			{
				result.push((nodes[i]));
			}
		}

		return result; 
	},

	prefixNS: function(element, ns)
	{
		// Find the closest ancestor that attests an xmlns with the required value, and return the prefix it used.

		if (ns == 'http://www.w3.org/XML/1998/namespace') return "xml:"
		if (element == null || element == undefined || element.nodeType == this.nodeType.DOCUMENT_NODE)
			return "";
			
		var attributes = element.attributes;
		for (var i = 0; i < attributes.length; i++)
		{
			var attribute = attributes[i];
				
			if ((attribute.name.indexOf("xmlns") == 0) && attribute.value == ns)
			{
				if (attribute.localName) return attribute.localName + ":";
				if (attribute.baseName) return attribute.baseName + ":";
			}
		}
	
		return this.prefixNS(element.parentNode, ns);
	},

	attrNS: function(element, name, ns, value)
	{
		var prefix = ns ? this.prefixNS(element, ns) : "";

		return element.getAttribute(prefix + name);
	},

	hasAttr: function(element, name)
	{
		// Test for the existence of an attribute.

		return this.getAttr(element, name) != undefined;
	},

	getAttr: function(element, name)
	{
		// Return an attribute.

		if (element.nodeType != this.nodeType.ELEMENT_NODE)
			return undefined;

		var val = element.getAttribute(name);

		// Bug in Encoder moves unprefixed attributes
		if (val == undefined && (element.prefix != null && element.prefix != ""))
			val = element.getAttribute(element.prefix + ":" + name);

		return val;
	},

	isTagNS: function(element, tag, namespace)
	{
		// Check an elements against a tag name in a namespace.

		var match = false;

		if (element.localName) // W3C
			match = (tag == element.localName);
		else
			match = (tag == element.baseName);

		match &= (element.namespaceURI == namespace);
		return (match);
	},

	children: function(element)
	{
		var childElement = element.firstChild;
		var children = [];

		for ( ; childElement; childElement = childElement.nextSibling)
		{
			if (childElement.nodeType === this.nodeType.ELEMENT_NODE)
				children.push(childElement);
		}

		return children;
	},

	style: function(styles)
	{
		var style = "";

		for(var name in styles)
		{
			style += name + ": " + styles[name] + "; ";
		}

		return style;
	},

	setStyle: function(element, styles)
	{
		for(var name in styles)
		{
			element.style[name] = styles[name];
		}
	}
});
