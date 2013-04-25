PlayerFramework.setDefaultOptions(
{
	containerClassName: "pf-container",
	autoplay: false,
	initialVolume: 0.5,
	overlayControls: true,
	overlayPlayButton: true,
	plugins: PlayerFramework.Plugins,
	pluginOptions: {}
});

PlayerFramework.Player = PlayerFramework.Object.extend(
{
	init: function(element, options)
	{
		///	<summary>
		///		Initializes a Player object, the entry point for the framework.
		///	</summary>
		///	<param name="element" type="String">
		///		The DOM ID of the primary media element. 
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the Player object.
		///	</param>
		///	<returns type="Player" />

		this._super();

		this.mergeOptions(options, PlayerFramework.defaultOptions);

		this.plugins = [];
		this.modules = [];
		this.fallbackMediaPlugins = [];
		this.mediaPlugin = null;
		this.controls = [];
		this.isReady = false;

		PlayerFramework.domReady(PlayerFramework.proxy(this, function()
		{
			if (!element)
				throw new Error("Element is null.");

			// Check if element is a DOM ID.
			if (typeof(element) === "string")
			{
				element = document.getElementById(element);
			
				if (!element)
					throw new Error("Element not found.");
			}

			// Check if element is an HTMLElement.
			if (element.tagName != null)
			{
				if (element.className === this.options.containerClassName)
				{
					this.containerElement = element;
					this.containerElement.style.width = this.options.width;
					this.containerElement.style.height = this.options.height;
				}
				else if (element.parentNode.className !== this.options.containerClassName)
				{
					this.createParentContainer(element);
				}
			}
		
			this.loadPlugins();

			if (!this.options.mediaPluginFallbackOrder)
				this.setMediaPluginFallbackOrderFromDeclaration(element);

			if (!this.options.mediaPluginFallbackOrder || this.options.sources)
				this.setMediaPlugin();
		}), 500);
	},

	// Functions
	createParentContainer: function(element)
	{
		///	<summary>
		///		Creates the parent div container and appends the element passed into the Player Object.
		///	</summary>

		this.containerElement = PlayerFramework.createElement(element.parentNode,
		[
			"div",
			{
				"class": "pf-container",
				style: "width: " + element.width + "px; height: " + element.height + "px;"
			}
		]);
		this.containerElement.appendChild(element);
	},

	loadPlugins: function()
	{
		///	<summary>
		///		Iterates over the plugins, initializing the module plugins and adding them to the
		///		"modules" property on the Player object and adding other plugins the "plugins"
		///		property on the Player object.
		///	</summary>

		if (!this.options || !this.options.plugins)
			return;

		for(var name in this.options.plugins)
		{
			var plugin = PlayerFramework.Plugins[name];
			
			if (PlayerFramework.typeExtendsFrom(plugin, PlayerFramework.ModulePlugin))
			{
				var options = plugin.prototype.defaultOptions(this);
				PlayerFramework.merge(options, this.options.pluginOptions[name]);

				if (plugin.prototype.isEnabled(this, options))
					this.modules.push(new plugin(this, options));
			}
			else
			{
				this.plugins.push(plugin);
			}
		}
	},

	ready: function(callback)
	{
		///	<summary>
		///		Adds an event listner for the "ready" event of the Player object for convenience.
		///	</summary>
		
		if (this.isReady)
		{
			callback();
		}
		else
		{
			this.addEventListener("ready", function()
			{
				window.setTimeout(callback, 1);
			}, false);
		}
	},

	setMediaPluginFallbackOrderFromDeclaration: function(primaryElement)
	{
		///	<summary>
		///		Iterates over the nested fallback elements in the DOM starting with the element
		///		passed to the Player object and finds a matching media plugin using the class name of the element.
		///	</summary>

		var fallbackMediaPlugins = [];

		var mediaPlugins = PlayerFramework.filter(this.plugins, function(p)
		{
			return PlayerFramework.typeExtendsFrom(p, (PlayerFramework.MediaPlugin));
		});

		// Use a recursive function to assign media plugins given the class name of an element.
		var findMediaPluginsForElements = function(elements)
		{
			// Loop through all child elements (some fallback elements, some not).
			for(var i = 0; i < elements.length; i++)
			{
				var element = elements[i];

				if (element.className)
				{
					var matchingPlugin = PlayerFramework.first(mediaPlugins, PlayerFramework.proxy(this, function(p)
					{
						var playerOptions = {};

						// Copy player options.
						PlayerFramework.merge(playerOptions, this.options);

						// Get default plugin options.
						var pluginOptions = p.prototype.defaultOptions(this, playerOptions);

						// Merge plugin options passed during player instantiation with default plugin options.
						PlayerFramework.merge(pluginOptions, this.options.pluginOptions[name]);
						
						return pluginOptions["class"] === element.className && !p.prototype.render;
					}));

					if (!matchingPlugin && i === (elements.length - 1))
						throw new Error("No matching media plugin.");

					// Stop searching once an element with a class name from a media plugin is found.
					if (matchingPlugin)
					{
						fallbackMediaPlugins.push(
						{
							type: matchingPlugin,
							element: element
						});

						PlayerFramework.proxy(this, findMediaPluginsForElements)(element.childNodes);
						break;
					}
				}
			}
		};
		
		PlayerFramework.proxy(this, findMediaPluginsForElements)([ primaryElement ]);

		this.fallbackMediaPlugins = fallbackMediaPlugins;
	},

	setMediaPluginFallbackOrderFromOptions: function(options)
	{
		///	<summary>
		///		Iterates over the media plugin fallback order array and finds a matching media plugin using the item string.
		///	</summary>

		this.fallbackMediaPlugins = [];

		// Loop through the given class names to find matching media plugins.
		for(var i = 0; i < options.mediaPluginFallbackOrder.length; i++)
		{
			var mediaPluginName = options.mediaPluginFallbackOrder[i];
			var matchingPlugin = options.plugins[mediaPluginName];

			if (!matchingPlugin)
				throw new Error("No matching media plugin.");

			this.fallbackMediaPlugins.push(
			{
				type: matchingPlugin,
				name: mediaPluginName
			});
		}
	},

	setMediaPlugin: function(options)
	{
		///	<summary>
		///		Finds and sets a media plugin supported by the browser given the set of media plugins passed in the options.
		///	</summary>

		var playerOptions = {};

		// Copy player options.
		PlayerFramework.merge(playerOptions, this.options);

		// Merge item-specific player options with player options.
		if (options)
			PlayerFramework.merge(playerOptions, options);

		if (playerOptions.mediaPluginFallbackOrder)
			this.setMediaPluginFallbackOrderFromOptions(playerOptions);

		var previousMediaPluginElement = null;
		PlayerFramework.forEachAsync(this.fallbackMediaPlugins, PlayerFramework.proxy(this, function(loopCallback, p)
		{
			// If the element was declared and was a fallback, remove the element from the DOM to promote it.
			if (p.element && previousMediaPluginElement)
			{
				var elementToClone = p.element.parentNode.removeChild(p.element);
				//IE needs a cloned node instead of promoting the existing nested child node.
				p.element = elementToClone.cloneNode(true);
			}

			// Get default plugin options.
			var pluginOptions = p.type.prototype.defaultOptions(this, playerOptions);
			
			if (p.element)
				pluginOptions.element = p.element;

			// Merge plugin options passed during player instantiation with default plugin options.
			PlayerFramework.merge(pluginOptions, this.options.pluginOptions[p.name]);

			// Create new instance of the media plugin.
			var mediaPlugin = new p.type(this, pluginOptions, playerOptions);

			// Add the element to the DOM if it is being promoted or injected.
			if (!mediaPlugin.element.parentNode)
			{
				this.containerElement.insertBefore(mediaPlugin.element, this.containerElement.firstChild);

				if (previousMediaPluginElement)
					this.containerElement.removeChild(previousMediaPluginElement);
			}

			mediaPlugin.checkSupport(PlayerFramework.proxy(this, function(isSupported)
			{
				if (isSupported)
				{
					this.mediaPlugin = mediaPlugin;
			
					if (this.mediaPlugin instanceof PlayerFramework.VideoMediaPlugin)
					{
						this.mediaPlugin.setup();
						
						window.setTimeout(PlayerFramework.proxy(this, function()
						{
							if (this.isReady === false)
							{
								this.isReady = true;
								this.dispatchEvent({ type: "ready" });
							}

							this.mediaPlugin.onLoadedMediaPlugin();
						}), 1);
					}
				}
				else
				{
					previousMediaPluginElement = mediaPlugin.element;
					loopCallback();
				}
			}));
		}));
	}
});
