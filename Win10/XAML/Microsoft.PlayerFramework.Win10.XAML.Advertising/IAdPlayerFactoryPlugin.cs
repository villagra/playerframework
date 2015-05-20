using Microsoft.VideoAdvertising;

namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// An interface for creating VPAID ad players
    /// </summary>
    public interface IAdPlayerFactoryPlugin : IPlugin
    {
        /// <summary>
        /// Called when a new VPAID player has been requested.
        /// </summary>
        /// <param name="creativeSource">The creative source of the ad that needs a player.</param>
        /// <returns>The VPAID ad player.</returns>
        IVpaid GetPlayer(ICreativeSource creativeSource);
    }
}
