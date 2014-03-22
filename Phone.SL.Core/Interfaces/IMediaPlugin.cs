using System;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Extends the IPlugin interface to allow a plugin to provide the MediaElement
    /// </summary>
    public interface IMediaPlugin : IPlugin
    {
        /// <summary>
        /// Gets the IMediaElement instance used to play video. This is unique to IMediaPlugin implementations.
        /// </summary>
        IMediaElement MediaElement { get; }
    }
}
