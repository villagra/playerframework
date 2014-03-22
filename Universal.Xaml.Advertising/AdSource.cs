using System.Collections.Generic;
using Microsoft.Media.Advertising;
#if NETFX_CORE
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// Provides a way to define an ad source object that contains info about the ad to be played.
    /// </summary>
    public sealed class AdSource :
#if NETFX_CORE
 FrameworkElement,
#endif
 IAdSource
    {
        /// <summary>
        /// Creates a new instance of AdSource.
        /// </summary>
        public AdSource()
        {
            AllowMultipleAds = true;
        }

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
        public object Payload { get; set; }

        /// <inheritdoc /> 
        public string Key { get; set; }

        /// <inheritdoc /> 
        public string Type { get; set; }

        /// <inheritdoc />
        public bool AllowMultipleAds { get; set; }

        /// <inheritdoc />
        public int? MaxRedirectDepth { get; set; }
    }
}
