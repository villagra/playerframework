using System;

namespace Microsoft.VideoAdvertising
{
    /// <summary>
    /// Represents a creative source that comes from an ad document
    /// </summary>
    public interface IDocumentCreativeSource : ICreativeSource
    {
        /// <summary>
        /// Gets the associated creative
        /// </summary>
        ICreative Creative { get; }
    }
}
