using System;

namespace Microsoft.Media.Advertising
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
