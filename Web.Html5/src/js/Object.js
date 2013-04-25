PlayerFramework.Object = PlayerFramework.Class.extend(
{
	init: function(options)
	{
		///	<summary>
		///		Initializes an PlayerFramework Object, the base for all objects in the framework.
		///		Stores the object options and provides event listening and dispatching support.
		///	</summary>

		this.options = options || {};
		this.eventListeners = [];
	},

	addEventListener: function (type, callback, capture) 
	{
		///	<summary>
		///		Adds an event listener for a derived class to be called when the event is dispatched.
		///	</summary>
		///	<param name="type" type="String">
		///		The type of event to listen for.
		///	</param>
		///	<param name="callback" type="Function">
		///		The function to call when the event is dispatched.
		///	</param>
		///	<param name="capture" type="Boolean">
		///		Indicates whether the event should be prevented from bubbling up (included for future use and to match the syntax of Node.addEventListener).
		///	</param>

		this.eventListeners.push(
		{
			type: type,
			callback: callback,
			capture: capture
		}); 
	},

	dispatchEvent: function(eventObject)
	{
		///	<summary>
		///		Dispatches an event for a derived class and calls each listener callback.
		///	</summary>
		///	<param name="eventObject" type="Object">
		///		The object containing the type to match and the listener callback.
		///	</param>
		
		PlayerFramework.forEach(this.eventListeners, function(l)
		{
			if (l.type === eventObject.type)
				l.callback(eventObject);
		});
	},

	mergeOptions: function(userOptions, defaultOptions)
	{
		///	<summary>
		///		Merges user specified options with default options.
		///	</summary>
		///	<param name="userOptions" type="Object">
		///		The object containing options specified by the caller creating the instance.
		///	</param>
		///	<param name="defaultOptions" type="Object">
		///		The object containing options specified by the class itself.
		///	</param>

		PlayerFramework.merge(this.options, defaultOptions);
		PlayerFramework.merge(this.options, userOptions);
	}
});
