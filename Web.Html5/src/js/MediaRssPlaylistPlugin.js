PlayerFramework.Plugins.MediaRssPlaylistPlugin = PlayerFramework.Plugins.PlaylistPlugin.extend(
{
	isEnabled: function(player, options)
	{
		return !!player.options.mediaRssPlaylistUrl
	},

	init: function(player, options)
	{
		///	<summary>
		///		Initializes the MediaRssPlaylistPlugin that provides an API for playlist management and downloading a media RSS feed.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the MediaRssPlaylistPlugin.
		///	</param>
		///	<returns type="MediaRssPlaylistPlugin" />

		this._super(player, options);
	},

	// Functions
	loadPlaylistItems: function()
	{
		///	<summary>
		///		Loads the playlist items from the media RSS URL specified in the options.
		///	</summary>

		PlayerFramework.xhr({ url: this.player.options.mediaRssPlaylistUrl }, PlayerFramework.proxy(this, function(result)
		{
			this.playlistItems = this.parseMediaRssXml(result.responseXML);
			
			if (this.playlistItems)
				this.setPlaylistItemOptions();
			else
				this.player.dispatchEvent({ type: "error" });
		}),
		PlayerFramework.proxy(this, function(result)
		{
			this.player.dispatchEvent({ type: "error" });
		}));
	},

	parseMediaRssXml: function(xml)
	{
		///	<summary>
		///		Parses the XML document as a media RSS feed.
		///	</summary>
		///	<returns type="Array" />

		var mediaRssObject = this.parseXmlDocument(xml);

		if(!mediaRssObject || !mediaRssObject.rss || !mediaRssObject.rss.channel || !mediaRssObject.rss.channel.item)
			return;
		
		var mediaRssItemNodes = mediaRssObject.rss.channel.item;

		if (!(mediaRssItemNodes instanceof Array))
			mediaRssItemNodes = [ mediaRssItemNodes ];

		var playlistItems = [];
		
		for(var i = 0; i < mediaRssItemNodes.length; i++)
		{
			var mediaRssItemNode = mediaRssItemNodes[i];

			// Parse <media:content> elements. Treat each <media:content> element that
			// isn't in a <media:group> element as a single playlist item.
			var mediaContentNodes = mediaRssItemNode["media:content"];
			if (mediaContentNodes)
				playlistItems = playlistItems.concat(this.parseMediaContentNodes(mediaContentNodes));

			// Parse <media:group> elements. Treat each <media:content> element under the
			// <media:group> element as a different encoding of the same video (<source>).
			var mediaGroupNodes = mediaRssItemNode["media:group"];
			if (mediaGroupNodes)
				playlistItems = playlistItems.concat(this.parseMediaGroupNodes(mediaGroupNodes));
		}

		return playlistItems;
	},

	parseMediaContentNodes: function(nodes)
	{
		///	<summary>
		///		Parses the specified nodes as media content nodes, each representing a single source.
		///	</summary>
		///	<param name="nodes" type="Array">
		///		An array of "media:content" nodes.
		///	</param>
		///	<returns type="Array" />

		var playlistItems = [];
		
		if (!(nodes instanceof Array))
			nodes = [ nodes ];

		for(var i = 0; i < nodes.length; i++)
		{
			var node = nodes[i];

			if (node.url)
			{
				var playlistItem =
				{
					sources: []
				};

				var source =
				{
					src: node.url
				};

				if (node.type)
					source.type = node.type

				if (node["media:title"])
					playlistItem.title = node["media:title"];

				if (node["media:thumbnail"])
					playlistItem.poster = node["media:thumbnail"].url;

				playlistItem.sources.push(source);

				playlistItems.push(playlistItem);
			}
		}

		return playlistItems;
	},

	parseMediaGroupNodes: function(nodes)
	{
		///	<summary>
		///		Parses the specified nodes as media group nodes, each representing a group of sources.
		///	</summary>
		///	<param name="nodes" type="Array">
		///		An array of "media:group" nodes.
		///	</param>
		///	<returns type="Array" />

		var playlistItems = [];		

		if (!(nodes instanceof Array))
			nodes = [ nodes ];
							
		for(var i = 0; i < nodes.length; i++)
		{
			var node = nodes[i];

			var playlistItem =
			{
				sources: []
			};

			var contentNodes = node["media:content"];

			if (!(contentNodes instanceof Array))
				contentNodes = [ contentNodes ];

			for(var j = 0; j < contentNodes.length; j++)
			{
				var contentNode = contentNodes[j];

				if (contentNode.url)
				{
					var source =
					{
						src: contentNode.url
					};

					if (contentNode.type)
						source.type = contentNode.type;

					if (contentNode["media:title"])
						playlistItem.title = contentNode["media:title"];
					else if (node["media:title"])
						playlistItem.title = node["media:title"];

					if (contentNode["media:thumbnail"])
						playlistItem.poster = contentNode["media:thumbnail"].url;
					else if (node["media:thumbnail"])
						playlistItem.poster = node["media:thumbnail"].url;

					playlistItem.sources.push(source);
				}
			}

			if (playlistItem.sources.length > 0)
				playlistItems.push(playlistItem);
		}

		return playlistItems;
	},

	parseXmlDocument: function(document)
	{
		/// <summary>
		///		Parses a JSON object from the specified XML document.
		///		Based on: http://slideshow.codeplex.com/SourceControl/changeset/view/25074#177488
		///	</summary>
		/// <param name="document">
		///		The document to parse.</param>
		/// <returns type="Object">
		///		The parsed object.
		///	</returns>
		
		var element = document.documentElement;
		
		if (!element)
			return;
		
		var elementName = element.nodeName;
		var elementType = element.nodeType;
		var elementValue = this.parseXmlNode(element);
		
		// document fragment
		if (elementType == 11)
			return elementValue;
		
		var obj = {};
		obj[elementName] = elementValue;
		return obj;
	},
	
	parseXmlNode: function(node)
	{
		/// <summary>
		///		Recursively parses a JSON object from the specified XML node.
		///	</summary>
		/// <param name="element" type="Object">
		///		The node to parse.
		///	</param>
		/// <returns type="Object">
		///		The parsed object.
		///	</returns>
		
		switch (node.nodeType)
		{
			// comment
			case 8:
				return;
			
			// text and cdata
			case 3:
			case 4:
			
				var nodeValue = node.nodeValue;
				
				if (!nodeValue.match(/\S/))
					return;
				
				return this.formatValue(nodeValue);
			
			default:
				
				var obj;
				var counter = {};
				var attributes = node.attributes;
				var childNodes = node.childNodes;
				
				if (attributes && attributes.length)
				{
					obj = {};
					
					for (var i = 0, j = attributes.length; i < j; i++)
					{
						var attribute = attributes[i];
						var attributeName = attribute.nodeName.toLowerCase(); // lowered in order to be consistent with Safari
						var attributeValue = attribute.nodeValue;
						
						if (typeof(counter[attributeName]) == "undefined")
							counter[attributeName] = 0;
						
						this.addProperty(obj, attributeName, this.formatValue(attributeValue), ++counter[attributeName]);
					}
				}
				
				if (childNodes && childNodes.length)
				{
					var textOnly = true;
					
					if (obj)
						textOnly = false;
					
					for (var k = 0, l = childNodes.length; k < l && textOnly; k++)
					{
						var childNodeType = childNodes[k].nodeType;
						
						// text or cdata
						if (childNodeType == 3 || childNodeType == 4)
							continue;
						
						textOnly = false;
					}
					
					if (textOnly)
					{
						if (!obj)
							obj = "";
					
						for (var m = 0, n = childNodes.length; m < n; m++)
							obj += this.formatValue(childNodes[m].nodeValue);
					}
					else
					{
						if (!obj)
							obj = {};
						
						for (var o = 0, p = childNodes.length; o < p; o++)
						{
							var childNode = childNodes[o];
							var childName = childNode.nodeName;
							
							if (typeof(childName) != "string")
								continue;
							
							var childValue = this.parseXmlNode(childNode);
							
							if (!childValue)
								continue;
							
							if (typeof(counter[childName]) == "undefined")
								counter[childName] = 0;
							
							this.addProperty(obj, childName, this.formatValue(childValue), ++counter[childName]);
						}
					}
				}
				
				return obj;
		}
	},
	
	formatValue: function(value)
	{
		/// <summary>
		///		Formats the specified value to its most suitable type.
		///	</summary>
		/// <param name="value">
		///		The value to format.
		///	</param>
		/// <returns type="String">
		///		The formatted value or the original value if no more suitable type exists.
		///	</returns>
		
		if (typeof(value) == "string" && value.length > 0)
		{
			var loweredValue = value.toLowerCase();
			
			if (loweredValue == "true")
				return true;
			else if (loweredValue == "false")
				return false;
			
			if (!isNaN(value))
				return new Number(value).valueOf(); // fixes number issue with option values
		}
		
		return value;
	},
	
	addProperty: function(obj, name, value, count)
	{
		/// <summary>
		///		Adds a property to the specified object.
		///	</summary>
		/// <param name="obj" type="Object">
		///		The target object.
		///	</param>
		/// <param name="name" type="String">
		///		The name of the property.
		///	</param>
		/// <param name="value" type="String">
		///		The value of the property.
		///	</param>
		/// <param name="count" type="Number">
		///		A count that indicates whether or not the property should be an array.
		///	</param>
		
		switch (count)
		{
			case 1:
				obj[name] = value;
				break;
				
			case 2:
				obj[name] = [ obj[name], value ];
				break;
				
			default:
				obj[name][obj[name].length] = value;
				break;
		}
	}
});
