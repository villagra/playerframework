using System;

namespace Microsoft.Media.Advertising
{
    /// <summary>
    /// An actively running creative.
    /// </summary>
    internal sealed class ActiveAdUnit
    {
        internal ActiveAdUnit(ICreativeSource creativeSource, IVpaid player, object creativeConcept, IAdSource adSource)
        {
            CreativeSource = creativeSource;
            Player = player;
            CreativeConcept = creativeConcept;
            AdSource = adSource;
        }

        /// <summary>
        /// Provides ad unit being played.
        /// </summary>
        public ICreativeSource CreativeSource { get; private set; }

        /// <summary>
        /// The VPaid player responsible for playing the ad unit.
        /// </summary>
        public IVpaid Player { get; private set; }

        /// <summary>
        /// An object that represents the creative concept. There can be multiple active ads per creative concept and multiple creative concepts per ad.
        /// </summary>
        public object CreativeConcept { get; private set; }

        /// <summary>
        /// The ad source associated with the playing ad unit.
        /// </summary>
        public IAdSource AdSource { get; private set; }
    }
}
