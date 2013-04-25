PlayerFramework.Plugins.ThrobberControlPlugin = PlayerFramework.ControlPlugin.extend(
{
	defaultOptions: function(player)
	{
		return PlayerFramework.mergeOptions(this._super(),
		{
			strings:
			{
				throbberTitle: "Buffering"
			},
			nodeCount: 12,
			maxOpacity: 0.85,
			animationDelay: 75 /* milliseconds */
		});
	},

	init: function(player, options)
	{
		///	<summary>
		///		Initializes the ControlPlugin that displays a "spinning wheel" while the UI loads.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the ThrobberControlPlugin.
		///	</param>
		///	<returns type="ThrobberControlPlugin" />

		this._super(player, options);

		this.createControl();
		this.animateNodes();
	},

	// ControlPlugin Event Handlers
	onCanPlayThrough: function(e)
	{
		///	<summary>
		///		Called when the media element can play through to the end without having to stop for further buffering.
		///	</summary>
		
		this.hide();
	},

	onError: function(e)
	{
		///	<summary>
		///		Called when the media element encounters an error.
		///	</summary>

		this.hide();
	},

	onUnloadingMediaPlugin: function(e)
	{
		this.element.style.display = "block";
		this.animateNodes();
	},

	onNetworkStateChange: function(e)
	{
		///	<summary>
		///		Called when the media element's network state changes.
		///	</summary>

		if (this.player.networkState() === PlayerFramework.VideoMediaPlugin.NetworkState.NETWORK_NO_SOURCE)
			this.hide();
	},

	// Functions
	createControl: function()
	{
		///	<summary>
		///		Creates and appends all markup for the controls to the DOM.
		///	</summary>
		
		// Throbber control
		this.element = PlayerFramework.createElement(this.player.containerElement,
		[
			"div",
			{
				"class": "pf-throbber-control",
				title: this.options.strings.throbberTitle
			}
		]);

		// Throbber container
		this.throbberContainerElement = PlayerFramework.createElement(this.element,
		[
			"div",
			{
				"class": "pf-throbber-container"
			}
		]);

		// Create throbber nodes
		this.throbberNodeElements = [];
		this.opacityDifference = this.options.maxOpacity / this.options.nodeCount;
		var separationAngle = (2 * Math.PI) / this.options.nodeCount;
		var throbberRadius = parseInt(PlayerFramework.getComputedStyle(this.throbberContainerElement, "width")) / 2;
		
		for (var i = 0; i < this.options.nodeCount; i++)
		{
			var throbberNodeElement = PlayerFramework.createElement(this.throbberContainerElement,
			[
				"div",
				{
					"class": "pf-throbber-node"
				}
			]);
			this.throbberNodeElements.push(throbberNodeElement);
			
			var nodeRadius = parseInt(PlayerFramework.getComputedStyle(throbberNodeElement, "width")) / 2;
			var x = throbberRadius * (Math.cos(i * separationAngle) + 1) - nodeRadius;
			var y = throbberRadius * (Math.sin(i * separationAngle) + 1) - nodeRadius;
			
			throbberNodeElement.style.left = x.toString() + "px";
			throbberNodeElement.style.top = y.toString() + "px";
			throbberNodeElement.style.opacity = this.options.maxOpacity - ((this.options.nodeCount - i) * this.opacityDifference);
		}
	},

	animateNodes: function()
	{
		if (this.element.style.display === "none")
			return;

		window.setTimeout(PlayerFramework.proxy(this, function()
		{
			PlayerFramework.requestAnimationFrame(PlayerFramework.proxy(this, this.animateNodes));
		}), this.options.animationDelay);

		this.fadeNodes();
	},

	fadeNodes: function()
	{
		for (var i = 0; i < this.options.nodeCount; i++)
		{
			var throbberNodeElement = this.throbberNodeElements[i];
			throbberNodeElement.style.opacity = this.options.maxOpacity - ((this.options.nodeCount - i) * this.opacityDifference);
		}

		this.throbberNodeElements.push(this.throbberNodeElements.shift());
	},

	hide: function()
	{
		window.setTimeout(PlayerFramework.proxy(this, function()
		{
			this.element.style.display = "none";
		}), 500);
	}
});
