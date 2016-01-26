using Microsoft.Media.Advertising;

namespace Microsoft.PlayerFramework.Js.Advertising
{
    /// <summary>
    /// Provides a way to define an ad source object that contains info about the ad to be played.
    /// </summary>
    public sealed class AdSource : IAdSource
    {
        /// <summary>
        /// Creates a new instance of AdSource.
        /// </summary>
        /// <param name="type">The type of the ad. Typically, this is "vast".</param>
        public AdSource() { }

        /// <summary>
        /// Creates a new instance of AdSource.
        /// </summary>
        /// <param name="type">The type of the ad. Typically, this is "vast".</param>
        public AdSource(string type)
            : this()
        {
            Type = type;
        }

        /// <summary>
        /// Creates a new instance of AdSource.
        /// </summary>
        /// <param name="payload">The payload for the ad. The type of object used here depends on the ad type.</param>
        /// <param name="type">The type of the ad. Typically, this is "vast".</param>
        public AdSource(object payload, string type)
            : this(type)
        {
            Payload = payload;
        }

        /// <inheritdoc /> 
        private object _Payload;
        public object Payload
        {
            get { return _Payload; }
            set { _Payload = value; }
        }

        /// <inheritdoc /> 
        private string _Key = string.Empty;
        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        /// <inheritdoc /> 
        private string _Type = string.Empty;
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        /// <inheritdoc />
        private bool _AllowMultipleAds = true;
        public bool AllowMultipleAds
        {
            get { return _AllowMultipleAds; }
            set { _AllowMultipleAds = value; }
        }

        /// <inheritdoc />
        public int? MaxRedirectDepth { get; set; }
    }
}