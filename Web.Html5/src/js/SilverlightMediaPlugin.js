PlayerFramework.Plugins.SilverlightMediaPlugin = PlayerFramework.Plugins.SilverlightMediaPluginBase.extend(
{
	defaultOptions: function(player, playerOptions)
	{
		return PlayerFramework.mergeOptions(this._super(),
		{
			params:
			{
			    source: "http://smf.cloudapp.net/html5/xap/SilverlightPlayer.xap",
				onError: "onSilverlightError",
				onLoad: "onSilverlightLoad",
				minRuntimeVersion: "5.0.61118.0",
				autoUpgrade: false,
				enableGPUAcceleration: true,
				windowless: true
			},
			initParams:
			{
			    scriptablename: "Player",
                captionsvisibility: "hidden",
				autoplay: !!playerOptions.autoplay
			}
		});
	},

	init: function(player, options, playerOptions)
	{
		///	<summary>
		///		Initializes the VideoMediaPlugin that injects and wraps the Silverlight player.
		///	</summary>
		///	<param name="player" type="Object">
		///		The Player object.
		///	</param>
		///	<param name="options" type="Object">
		///		The options for the SilverlightMediaPlugin.
		///	</param>
		///	<param name="playerOptions" type="Object">
		///		The merged player options for the current media source.
		///	</param>
		///	<returns type="SilverlightMediaPlugin" />

		this._super(player, options, playerOptions);

		this.render();
	},

	render: function()
	{
		///	<summary>
		///		Creates and sets the MediaPlugin's element given the plugin and player options
		///		and a specific template.
		///	</summary>

		var element = PlayerFramework.createElement(null,
		[
			"object",
			{
				"class": this.options["class"],
				data: "data:application/x-silverlight-2,",
				type: "application/x-silverlight-2",
				width: this.playerOptions.width,
				height: this.playerOptions.height,
				"data-poster": this.playerOptions.poster || null
			}
		]);

		this.createParams(element);

		// Element is cloned for IE, otherwise it does not display.
		this.setElement(element.cloneNode(true));
	},

	createParams: function(element)
	{
		///	<summary>
		///		Creates param child nodes on the specified element given the media plugin's options.
		///	</summary>

		// Push all object params.
		for(var p in this.options.params)
		{
			PlayerFramework.createElement(element,
			[
				"param",
				{
					name: p,
					value: this.options.params[p]
				}
			]);
		}

		var mediaUrl = this.options.initParams.mediaurl;
        if (!mediaUrl)
        {
            mediaUrl = this.getMediaUrl();
			PlayerFramework.merge(this.options.initParams,
			{
			    mediaurl: mediaUrl
			});
        }

        var deliveryMethod = this.options.initParams.deliveryMethod;
        if (!deliveryMethod) {
            deliveryMethod = this.getDeliveryMethod(mediaUrl);
		    PlayerFramework.merge(this.options.initParams,
			{
			    deliverymethod: deliveryMethod
			});
        }

		var initParams = "";
		// Concatenate and add the special "InitParams" object param.
		for(var p in this.options.initParams)
		{
			if (initParams)
				initParams += ",";

			initParams += p + "=" + this.options.initParams[p];
		}

		PlayerFramework.createElement(element,
		[
			"param",
			{
				name: "InitParams",
				value: initParams
			}
		]);
	},

	getMediaUrl: function()
	{
		var firstSupportedSource = PlayerFramework.first(this.playerOptions.sources, PlayerFramework.proxy(this, function(s)
		{
			return this.canPlayType(s.type);
		}));
		
		if (!firstSupportedSource)
			return null;

		return firstSupportedSource.src.indexOf('://') != -1
											? firstSupportedSource.src
											: this.qualifyURL(firstSupportedSource.src);
	},

	getDeliveryMethod: function(mediaUrl)
	{
	    if (mediaUrl.toLowerCase().indexOf('/manifest') != -1) {
	        return "AdaptiveStreaming";
	    } else {
	        return "ProgressiveDownload";
	    }
	},

	escapeHTML: function(s)
	{
		return s.split('&').join('&amp;').split('<').join('&lt;').split('"').join('&quot;');
	},

	qualifyURL: function(url)
	{
		var el= document.createElement('div');
		el.innerHTML= '<a href="'+ this.escapeHTML(url) +'">x</a>';
		return el.firstChild.href;
	}
});
