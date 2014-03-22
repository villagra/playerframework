
namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Provides state related to advertising
    /// </summary>
    public enum AdvertisingState
    {
        /// <summary>
        /// No ad is playing.
        /// </summary>
        None,
        /// <summary>
        /// Ads are loading, this is before we know what kind of ad it is.
        /// </summary>
        Loading,
        /// <summary>
        /// A linear ad is playing.
        /// </summary>
        Linear,
        /// <summary>
        /// A non linear ad is playing.
        /// </summary>
        NonLinear
    }
}
