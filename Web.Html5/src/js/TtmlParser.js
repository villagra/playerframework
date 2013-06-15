PlayerFramework.TtmlParser = PlayerFramework.Object.extend(
{
    init: function (options) {
        /// <summary>
        ///     Parses a TTML file per the W3C specification: http://www.w3.org/TR/ttaf1-dfxp/
        ///     Based on a library written by Sean Hayes.
        /// </summary>

        this._super();

        this.mergeOptions(options,
        {
            xmlNamespace: "http://www.w3.org/XML/1998/namespace",
            xhtmlNamespace: "http://www.w3.org/1999/xhtml",
            ttmlNamespace: "http://www.w3.org/ns/ttml",
            ttmlStyleNamespace: "http://www.w3.org/ns/ttml#styling",
            ttmlMetaNamespace: "http://www.w3.org/ns/ttml#metadata",
            ttmlNamespaceOld: "http://www.w3.org/2006/10/ttaf1",
            ttmlStyleNamespaceOld: "http://www.w3.org/2006/10/ttaf1#styling",
            ttmlMetaNamespaceOld: "http://www.w3.org/2006/10/ttaf1#metadata",
            smpteNamespace: "http://www.smpte-ra.org/schemas/2052-1/2010/smpte-tt",
            audioNamespace: "http://www.microsoft.com/enable#media",
            trackIdPrefix: "",
            mediaFrameRate: 30,
            mediaFrameRateMultiplier: 1,
            mediaSubFrameRate: 1,
            mediaTickRate: 1000,
            mediaStart: 0,
            mediaDuration: Math.pow(2, 53), // maximum JavaScript integer
            clockTime: /^(\d{2,}):(\d{2}):(\d{2})((?:\.\d{1,})|:(\d{2,}(?:\.\d{1,})?))?$/, // hours ":" minutes ":" seconds ( fraction | ":" frames ( "." sub-frames )? )?
            offsetTime: /^(\d+(\.\d+)?)(ms|[hmsft])$/ // time-count fraction? metric
        });

        this.root = null;
        this.layout = null;
        this.head = null;
        this.body = null;
        this.regions = null;

        // True unless we see a region definition in the TTML.
        this.usingDefaultRegion = true;

        // Ordered list of events containing times (in ms) and corresponding element.
        this.ttmlEvents = [];

        // List of audio descriptions.
        this.descriptions = [];

        // List of cues.
        this.cues = [];

        // Tree of navigation points.
        this.navigation = null;

        // Store styles here because IE doesn't support expandos on XML elements.
        this.styleSetCache = {};
        this.styleSetId = 0;

        // SMPTE-TT image support.
        this.imageCache = {};

        // Keep track of the rightmost element at each level, so that we can include left and right links.
        this.rightMostInLevel = [];

        // TtmlParser Enums
        this.nodeType = {
            elementNode: 1,
            attributeNode: 2,
            textNode: 3,
            cdataSectionNode: 4,
            entityReferenceNode: 5,
            entityNode: 6,
            processingInstructionNode: 7,
            commentNode: 8,
            documentNode: 9,
            documentTypeNode: 10,
            documentFragmentNode: 11,
            notationNode: 12
        };
    },

    parseTtml: function (document, startTime, endTime) {
        // Parse an XML document and returns its TTML captions, audio descriptions, and navigation points.
        if (startTime === undefined) startTime = this.options.mediaStart;
        if (endTime === undefined) endTime = this.options.mediaDuration;

        // Find the tt root node.
        this.root = this.getElementByTagNameNS(document, "tt", this.options.ttmlNamespace);
        if (!this.root) {
            this.options.ttmlNamespace = this.options.ttmlNamespaceOld;
            this.options.ttmlStyleNamespace = this.options.ttmlStyleNamespaceOld;
            this.options.ttmlMetaNamespace = this.options.ttmlMetaNamespaceOld;
            this.root = this.getElementByTagNameNS(document, "tt", this.options.ttmlNamespace);
        }

        if (this.root) {
            // Find the head, body, and layout nodes.
            this.head = this.getElementByTagNameNS(this.root, "head", this.options.ttmlNamespace);
            this.body = this.getElementByTagNameNS(this.root, "body", this.options.ttmlNamespace);
            this.layout = this.head ? this.getElementByTagNameNS(this.head, "layout", this.options.ttmlNamespace) : null;

            // TTML that doesn't declare any layout regions uses a default region.
            if (this.layout) {
                this.regions = this.getElementsByTagNameNS(this.layout, "region", this.options.ttmlNamespace);
                this.usingDefaultRegion = (this.regions.length === 0);
            } else {
                this.regions = [];
                this.usingDefaultRegion = true;
            }

            // Load SMPTE images.
            this.imageCache = {};

            PlayerFramework.forEach(this.getElementsByTagNameNS(this.root, "image", this.options.smpteNamespace), PlayerFramework.proxy(this, function (image) {
                var id = this.getAttributeNS(image, "id", this.options.xmlNamespace);
                if (id !== null) this.imageCache["#" + id] = image.textContent;
            }));

            // Apply the style inheritance over the tree.
            this.applyStyling();

            // Apply the time intervals over the tree.
            this.applyTiming(this.root, { first: startTime, second: endTime }, true);

            // Use the time containment as a structured navigation map.
            this.navigation = this.getNavigation(this.body);

            // Get the cues.
            this.cues = this.getCues();
        }
    },

    applyTiming: function (element, bound, isParallelContext) {
        // Walk the tree to determine the absolute start and end times of all the
        // elements using the TTML subset of the SMIL timing model.
        // The reference times passed in "bound" are absolute times, the result of
        // calling this is to set the local start time and end time to absolute times
        // between these two reference times, based on the begin, end and dur attributes
        // and to recursively set all of the children.

        var startTime, endTime;
        var beginAttr = this.getAttributeNS(element, "begin", this.options.ttmlNamespace);
        var durAttr = this.getAttributeNS(element, "dur", this.options.ttmlNamespace);
        var endAttr = this.getAttributeNS(element, "end", this.options.ttmlNamespace);

        if (beginAttr !== null) {
            // Begin attested.
            startTime = bound.first + this.getTime(beginAttr) + 0.01; // extra time added to fix cues that begin exactly when the previous cue ends
        } else {
            startTime = bound.first;
        }

        if (durAttr !== null && endAttr !== null) {
            // Both dur and end attested, the minimum interval applies.
            endTime = Math.min(Math.min(startTime + this.getTime(durAttr), bound.first + this.getTime(endAttr)), bound.second);
        } else if (endAttr !== null) {
            // Only end attested.
            endTime = Math.min(bound.first + this.getTime(endAttr), bound.second);
        } else if (durAttr !== null) {
            // Only dur attested.
            endTime = Math.min(startTime + this.getTime(durAttr), bound.second);
        } else {
            // No direct timing attested, so use default based on context.
            // "par" children have indefinite default duration, truncated by bounds.
            // "seq" children have zero default duration.
            if (isParallelContext) {
                if (startTime <= bound.second) {
                    endTime = bound.second;
                } else {
                    endTime = 0;
                }
            }
        }

        if (endTime < startTime) {
            endTime = startTime;
        }

        element.setAttribute("data-time-start", startTime);
        element.setAttribute("data-time-end", endTime);

        PlayerFramework.binaryInsert(this.ttmlEvents, { tick: startTime, elementScope: element }, this.compareTtmlEvents);
        PlayerFramework.binaryInsert(this.ttmlEvents, { tick: endTime, elementScope: element }, this.compareTtmlEvents);

        if (this.getAttributeNS(element, "role", this.options.ttmlMetaNamespace)) {
            var uri = this.getAttributeNS(element, "audio", this.options.audioNamespace);
            if (uri) {
                this.descriptions.push({
                    uri: uri,
                    startTime: startTime,
                    endTime: endTime
                });
                this.descriptions.sort(this.compareDescriptions);
            }
        }

        var seqStartTime = startTime;
        var timeContext = this.getAttributeNS(element, "timeContainer", this.options.ttmlNamespace);

        PlayerFramework.forEach(this.getChildElements(element), PlayerFramework.proxy(this, function (childElement) {
            if (timeContext !== "seq") {
                this.applyTiming(childElement, { first: startTime, second: endTime }, true);
            } else {
                this.applyTiming(childElement, { first: seqStartTime, second: endTime }, false);
                seqStartTime = new Number(this.getAttribute(childElement, "data-time-end"));
            }
        }));
    },

    getTime: function (timeExpression) {
        // Utility object to handle TTML time expressions. Could be improved, but seems to do the job.
        // In particular, we are not currently handling TTML parameters for tick rate and so on.

        // NOTE: IE cannot parse time formats containing frames (e.g. "00:00:04.18" works, but not "00:00:04:18")
        // To overlay custom and native captions for testing purposes, use the CaptionsPlugin.displayMode option.

        var v1 = this.options.clockTime.exec(timeExpression);
        var v2 = this.options.offsetTime.exec(timeExpression);

        if (v1 != null) {
            var hours = new Number(v1[1]);
            var minutes = new Number(v1[2]);
            var seconds = new Number(v1[3]);
            var frames = 0;

            if (!isNaN(v1[4])) {
                seconds += new Number(v1[4]);
            }

            if (!isNaN(v1[5])) {
                frames = new Number(v1[5]);
            }

            return hours * this.getMetricMultiplier("h") + minutes * this.getMetricMultiplier("m") + seconds * this.getMetricMultiplier("s") + frames * this.getMetricMultiplier("f");
        } else if (v2 != null) {
            return new Number(v2[1]) * this.getMetricMultiplier(v2[3]);
        } else {
            return 0;
        }
    },

    getMetricMultiplier: function (timeExpression) {
        switch (timeExpression) {
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

    compareTtmlEvents: function (a, b) {
        // Compare TTML events for sorting purposes.

        return a.tick - b.tick;
    },

    compareDescriptions: function (a, b) {
        // Compare descriptions for sorting purposes.

        return a.startTime - b.startTime;
    },

    getNavigation: function (element) {
        // Navigation elements are marked with the extensions role="x-nav-..."
        // We want to find the lists of nav-labels, where each label goes in the right level of list.
        // The structure of this is loosely based on daisy NCK files.

        return this.getNavigationPoint(element, null, 0);
    },

    getNavigationList: function (element, parent, level) {
        // A nav list is supposed to be a list of nav points, but a nav point can be a degenerate nav label.

        var list = [];
        var role = this.getAttributeNS(element, "role", this.options.ttmlMetaNamespace);

        if (role !== null && !PlayerFramework.first(role, function (r) { return r === "x-nav-list"; })) {
            var childElements = this.getChildElements(element);
            for (var i = 0; i < childElements.length; i++) {
                var point = this.getNavigationPoint(childElements[i], parent, level);
                if (point != null) {
                    list.push(point);
                }
            }
        }

        return list;
    },

    getNavigationPoint: function (element, parent, level) {
        // A nav point is an element tagged with the x-nav-point role, containing one label, and one list.
        // If the list is empty, then the label can stand on its own for the whole point.

        var label = null;
        var subtree = [];
        var node = {};

        // Keep the high tide mark for how deep in the tree we are.
        if (this.rightMostInLevel.length <= level) {
            this.rightMostInLevel.push(null);
        }

        var role = this.getAttributeNS(element, "role", this.options.ttmlMetaNamespace);

        switch (role) {
            case "x-nav-label": // Degenerate form.
                label = this.getNavigationLabel(element);
                break;
            case "x-nav-point": // Full form contains a label and a list.
                PlayerFramework.forEach(this.getChildElements(element), PlayerFramework.proxy(this, function (childElement) {
                    var childRole = this.getAttributeNS(childElement, "role", this.options.ttmlMetaNamespace);
                    switch (childRole) {
                        // Should only be one of each. but allow last to win.
                        case "x-nav-label":  // Contains text, and use its timing.
                            label = this.getNavigationLabel(childElement);
                            break;
                        case "x-nav-list":   // Contains either a list of navPoints.
                            subtree = this.getNavigationList(childElement, node, level + 1);
                            break;
                        default:
                            break;
                    }
                }));
                break;
            default:
                break;  // Ignore anything else.
        }

        if (label !== null) {
            node.text = label.text;
            node.startTime = new Number(label.startTime) / 1000 + 0.01;
            node.endTime = new Number(label.endTime) / 1000;
            node.parent = parent;
            node.left = this.rightMostInLevel[level];
            node.right = null;
            node.children = subtree;

            if (this.rightMostInLevel[level] !== null) {
                this.rightMostInLevel[level].right = node;
            }

            this.rightMostInLevel[level] = node;

            return node;
        } else {
            return null;
        }
    },

    getNavigationLabel: function (element) {
        // A nav label is just text, but we use its timing to create an interval into the media for navigation.

        var role = this.getAttributeNS(element, "role", this.options.ttmlMetaNamespace);

        if (role !== null && !PlayerFramework.first(role, function (r) { return r === "x-nav-label"; })) {
            return {
                text: element.innerHTML,
                startTime: this.getAttribute(element, "data-time-start"),
                endTime: this.getAttribute(element, "data-time-end")
            };
        }
    },

    getCues: function () {
        // Get all cues for the set of TTML events.
        // Unroll using ttmlEvents and getCuesAtTime.
        // This then makes it easier to use the <track> APIs and also use the same interface for WebVTT, SRT etc.

        var cues = [];

        for (var i = 0; i < this.ttmlEvents.length; i++) {
            var ttmlEvent = this.ttmlEvents[i];

            if (ttmlEvent.elementScope === this.root) {
                continue;
            }

            var ttmlEventCues = this.getCuesAtTime(ttmlEvent.elementScope, ttmlEvent.tick);

            if (i > 0) {
                for (var j = i - 1; j >= 0; j--) {
                    var previousTtmlEvent = this.ttmlEvents[j];

                    if (previousTtmlEvent.elementScope === this.root || previousTtmlEvent.elementScope === ttmlEvent.elementScope) {
                        continue;
                    }

                    var overlappingCues = this.getCuesAtTime(previousTtmlEvent.elementScope, ttmlEvent.tick);

                    if (overlappingCues.length > 0) {
                        ttmlEventCues.push(overlappingCues[0]);
                    } else {
                        break;
                    }
                }
            }

            for (var k = 0; k < ttmlEventCues.length; k++) {
                var ttmlEventCue = ttmlEventCues[k];
                var nextTtmlEvent = this.ttmlEvents[i + 1];

                cues.push({
                    cue: ttmlEventCue,
                    startTime: ttmlEvent.tick / 1000,
                    endTime: (nextTtmlEvent) ? nextTtmlEvent.tick / 1000 : this.options.mediaDuration
                });
            }
        }

        return cues;
    },

    getCuesAtTime: function (element, tick) {
        // Get cues for a given time instant.

        var cues = [];

        if (element && this.isTemporallyActive(element, tick)) {
            if (!this.usingDefaultRegion) {
                PlayerFramework.forEach(this.regions, PlayerFramework.proxy(this, function (region) {
                    var cueElement = this.translateMarkup(region, tick);

                    if (cueElement) {
                        // Create a new subtree for the body element, prune elements not associated
                        // with the region, and if it's not empty then select it into this region by
                        // adding it to cue element container.

                        var regionId = this.getAttributeNS(region, "id", this.options.xmlNamespace);
                        var prunedElement = this.prune(element, regionId, tick);

                        if (prunedElement) {
                            cueElement.appendChild(prunedElement);
                        }

                        if (cueElement.getAttribute("data-showBackground") !== "whenActive" && cueElement.innerHTML.trim() !== "") {
                            cues.push(cueElement);
                        }
                    }
                }));
            } else {
                var cueElement = document.createElement("div");
                cueElement.className = "pf-cue";

                var prunedElement = this.prune(element, "", tick);

                if (prunedElement) {
                    cueElement.appendChild(prunedElement);
                }

                if (this.getChildElements(cueElement).length > 0) {
                    cues.push(cueElement);
                }
            }
        }

        return cues;
    },

    isTemporallyActive: function (element, tick) {
        var startTime = Math.round(1000 * parseFloat(this.getAttribute(element, "data-time-start"))) / 1000;
        var endTime = Math.round(1000 * parseFloat(this.getAttribute(element, "data-time-end"))) / 1000;
        var time = Math.round(1000 * tick) / 1000;

        return (startTime <= time) && (endTime > time);
    },

    translateMarkup: function (element, tick) {
        // Convert elements in TTML to their equivalent in HTML.

        var translation;
        var name = this.getTagNameEquivalent(element);
        var htmlName = "";
        var htmlClass = "";
        var htmlAttrs = {};

        if (element && this.isTemporallyActive(element, tick)) {
            switch (name) {
                case "tt:region":
                case "tt:tt": // We get this if there is no region.
                    htmlClass = "pf-cue "; // To simulate the ::cue selector.
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
                    PlayerFramework.forEach(element.attributes, function (attribute) { htmlAttrs[attribute.name] = attribute.value; });
                    break;
            }

            var roleAttr = this.getAttributeNS(element, "role", this.options.ttmlMetaNamespace);
            if (roleAttr) htmlClass += roleAttr + " ";

            var classAttr = this.getAttributeNS(element, "class", this.options.xhtmlNamespace);
            if (classAttr) htmlClass += classAttr + " ";

            // Hack until display:ruby on other elements works.
            if (roleAttr === "x-ruby") htmlName = "ruby";
            if (roleAttr === "x-rubybase") htmlName = "rb";
            if (roleAttr === "x-rubytext") htmlName = "rt";

            // Convert image based captions here; and move the text into its alt.
            // If I could get inline CSS to work div's then this would be set as style.
            var imageAttr = this.getAttribute(element, "image");
            if (imageAttr !== null) htmlName = "img";

            if (htmlName !== "") {
                translation = document.createElement(htmlName);
                translation.className = htmlClass.trim();

                // Move ID's over. Use trackIdPrefix to ensure there are no name clases on id's already in target doc.
                var idAttr = this.getAttributeNS(element, "id", this.options.xmlNamespace);
                if (idAttr) translation.setAttribute("id", this.options.trackIdPrefix + idAttr);

                // Copy style from element over to html, translating into CSS as we go
                this.translateStyle(element, translation, tick);

                // If we are copying over html elements, then copy any attributes too.
                for (var attr in htmlAttrs) {
                    translation.setAttribute(attr, htmlAttrs[attr]);
                }

                if (imageAttr !== null) {
                    translation.setAttribute("src", imageAttr);
                    translation.setAttribute("alt", element.innerHTML);
                }
            }
        }

        return translation;
    },

    translateStyle: function (element, htmlElement, tick) {
        // Convert from TTML style names to CSS.

        // Clone of the base style set.
        var computedStyleSet = {};
        var styles = this.styleSetCache[this.getAttribute(element, "__styleSet__")];

        for (var name in styles) {
            computedStyleSet[name] = styles[name];
        }

        // Apply inline styles.
        PlayerFramework.forEach(this.getElementsByTagNameNS(element, "set", this.options.ttmlNamespace), PlayerFramework.proxy(this, function (childElement) {
            if (this.isTemporallyActive(childElement, tick)) {
                this.applyInlineStyles(childElement, computedStyleSet);
            }
        }));

        // Apply CSS styles.
        for (var name in computedStyleSet) {
            var value = computedStyleSet[name];
            switch (name) {
                case "origin":
                    var coords = value.split(/\s/);
                    htmlElement.style.position = "absolute";
                    htmlElement.style.left = coords[0];
                    htmlElement.style.top = coords[1];
                    break;
                case "extent":
                    var coords = value.split(/\s/);
                    htmlElement.style.width = coords[0];
                    htmlElement.style.height = coords[1];
                    break;
                case "displayAlign":
                    htmlElement.style.textAlign = value;
                    break;
                case "wrapOption":
                    htmlElement.style.whiteSpace = (value === "nowrap") ? "nowrap" : "normal";
                    break;
                default:
                    htmlElement.style[name] = value;
                    break;
            }
        }
    },

    applyStyling: function () {
        // Apply styling to every element in the body.

        var nodes = this.root.getElementsByTagName("*");

        for (var i = 0; i < nodes.length; i++) {
            this.applyStyle(nodes[i]);
        }
    },

    applyStyle: function (element) {
        // Apply styles in the correct order to element.

        var styleSet = {};

        // Find all the applicable styles and set them as properties on styleSet.
        this.applyStylesheet(element, styleSet);
        this.applyInlineStyles(element, styleSet);

        // Record the applied set to the element
        this.styleSetCache[this.styleSetId] = styleSet;
        element.setAttribute("__styleSet__", this.styleSetId++);
    },

    applyStylesheet: function (element, styleSet) {
        // For each style id on element, find the corresponding style element
        // and then apply the stylesheet into styleset; this recurses over the tree of referenced styles.

        var styleAttr = this.getAttributeNS(element, "style", this.options.ttmlNamespace);

        if (styleAttr !== null) {
            var styleIds = styleAttr.split(/\s/); // Find all the style ID references.
            var isStyleElement = this.hasTagNameNS(element, "style", this.options.ttmlNamespace); // Detect if we are referencing style from a style.

            PlayerFramework.forEach(styleIds, PlayerFramework.proxy(this, function (styleId) {
                // Find all the style elements in the TTML namespace.
                PlayerFramework.forEach(this.getElementsByTagNameNS(this.head, "style", this.options.ttmlNamespace), PlayerFramework.proxy(this, function (styleElement) {
                    if (this.getAttributeNS(styleElement, "id", this.options.xmlNamespace) === styleId) {
                        this.applyStylesheet(styleElement, styleSet);

                        // If the element is region, do nested styles (note regions can only be referenced from elements in the body).
                        if (!isStyleElement && this.hasTagNameNS(styleElement, "region", this.options.ttmlNamespace)) {
                            PlayerFramework.forEach(this.getElementsByTagNameNS(styleElement, "style", this.options.ttmlNamespace), PlayerFramework.proxy(this, function (childElement) {
                                this.applyStylesheet(childElement, styleSet);
                            }));
                        }

                        // Do inline styles.
                        this.applyInlineStyles(styleElement, styleSet);
                    }
                }));
            }));
        }
    },

    applyInlineStyles: function (element, styleSet) {
        // Apply style attributes into styleset.
        PlayerFramework.forEach(element.attributes, PlayerFramework.proxy(this, function (attribute) {
            var ns = this.getNamespace(attribute);
            if (ns === this.options.ttmlStyleNamespace) {
                styleSet[this.getLocalTagName(attribute)] = attribute.nodeValue;
            }
            else if (ns === this.options.smpteNamespace && this.getLocalTagName(attribute) === "backgroundImage") {
                var imageId = attribute.nodeValue;
                if (imageId.indexOf("#") === 0) {
                    element.setAttribute("image", "data:image/png;base64," + this.imageCache[imageId]);
                } else {
                    element.setAttribute("image", imageId);
                }
            }
        }));
    },

    isInRegion: function (element, regionId) {
        // A content element is associated with a region according to the following ordered rules,
        // where the first rule satisfied is used and remaining rules are skipped:

        // Quick test: Out of normal order, but makes following rules simpler.
        if (regionId === "" || regionId === undefined) {
            return this.usingDefaultRegion;
        }

        // 1. If the element specifies a region attribute, then the element is associated with the
        // region referenced by that attribute;
        if (this.getAttributeNS(element, "region", this.options.ttmlNamespace) === regionId) {
            return true;
        }

        // 2. If some ancestor of that element specifies a region attribute, then the element is
        // associated with the region referenced by the most immediate ancestor that specifies
        // this attribute;
        var ancestor = element.parentNode;
        while (ancestor !== null && ancestor.nodeType === this.nodeType.elementNode) {
            var id = this.getAttributeNS(ancestor, "region", this.options.ttmlNamespace);
            if (id) return id === regionId;
            ancestor = ancestor.parentNode;
        }

        // 3. If the element contains a descendant element that specifies a region attribute,
        // then the element is associated with the region referenced by that attribute;
        var nodes = element.getElementsByTagName("*");
        for (var i = 0; i < nodes.length; i++) {
            var node = nodes[i];
            if (this.getAttributeNS(node, "region", this.options.ttmlNamespace) === regionId && this.getNamespace(node) === this.options.ttmlNamespace) {
                return true;
            }
        }

        // 4. If a default region was implied (due to the absence of any region element), then
        // the element is associated with the default region;
        if (this.usingDefaultRegion) {
            return regionId === "";
        }

        // 5. The element is not associated with any region.
        return false;
    },

    prune: function (element, regionId, tick) {
        /// Convert the element if it is in the region, then recurse into its contents.
        /// If it ends up with no children, then we dont add it to the region.

        var clone = undefined;

        if (this.isInRegion(element, regionId)) {
            clone = this.translateMarkup(element, tick);

            if (!clone) {
                return clone;
            }

            for (var i = 0; i < element.childNodes.length; i++) {
                var childElement = element.childNodes[i];
                if (childElement.nodeType !== this.nodeType.commentNode) {
                    if (childElement.nodeType === this.nodeType.textNode) {
                        clone.appendChild(document.createTextNode(childElement.data));
                    } else {
                        var prunedChildElement = this.prune(childElement, regionId, tick);
                        if (prunedChildElement) {
                            clone.appendChild(prunedChildElement);
                        }
                    }
                }
            }
        }

        return clone;
    },

    getTagNameEquivalent: function (element) {
        var name = this.getLocalTagName(element);

        switch (this.getNamespace(element)) {
            case this.options.xhtmlNamespace:
                return name;
            case this.options.ttmlNamespace:
                return "tt:" + name;
            case this.options.smpteNamespace:
                return "smpte:" + name;
            default:
                return "";
        }
    },

    getLocalTagName: function (element) {
        if (element.localName) {
            return element.localName;
        } else {
            return element.baseName;
        }
    },

    hasTagNameNS: function (element, name, namespace) {
        if (element.localName) {
            return (name === element.localName && this.getNamespace(element) === namespace);
        } else {
            return (name === element.baseName && this.getNamespace(element) === namespace);
        }
    },

    getElementByTagNameNS: function (element, name, namespace) {
        return this.getElementsByTagNameNS(element, name, namespace)[0] || null;
    },

    getElementsByTagNameNS: function (element, name, namespace) {
        if (element.getElementsByTagNameNS) {
            return PlayerFramework.getArray(element.getElementsByTagNameNS(namespace, name));
        }
        else {
            return PlayerFramework.getArray(element.getElementsByTagName(name)).filter(function (element) { return element.namespaceUri === namespace; });
        }
    },

    getAttributeNS: function (element, name, namespace) {
        var result = element.getAttributeNS(namespace, name);
        if (result === "") {
            result = this.getAttribute(element, name);
        }
        return result;
    },

    getAttribute: function (element, name) {
        if (element.nodeType === this.nodeType.elementNode) {
            var value = element.getAttribute(name);

            if (value) {
                return value;
            } else if (element.prefix) {
                // Bug in Encoder moves unprefixed attributes
                return element.getAttribute(element.prefix + ":" + name);
            }
        }

        return null;
    },

    getNamespace: function (element) {
        return (element.namespaceUri) ? element.namespaceUri : element.namespaceURI;
    },

    getChildElements: function (element) {
        var childElements = [];

        for (var childElement = element.firstChild; childElement; childElement = childElement.nextSibling) {
            if (childElement.nodeType === this.nodeType.elementNode) {
                childElements.push(childElement);
            }
        }

        return childElements;
    }
});

