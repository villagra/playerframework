using System;

namespace Microsoft.VideoAnalytics
{
    /// <summary>
    /// Provides heuristic information about adaptive streaming for analytic purposes
    /// </summary>
    public interface IAdaptiveMonitor
    {
        /// <summary>
        /// Called from a background thread to indicate that the data should refresh. Note, this does not have to be used.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Gets the current bitrate
        /// </summary>
        uint CurrentBitrate { get; }

        /// <summary>
        /// Gets the size of the audio buffer
        /// </summary>
        uint AudioBufferSize { get; }

        /// <summary>
        /// Gets the size of the video buffer
        /// </summary>
        uint VideoBufferSize { get; }

        /// <summary>
        /// Gets the perceived bandwidth in bps
        /// </summary>
        uint PerceivedBandwidth { get; }

        /// <summary>
        /// Gets the maximum bitrate of the stream
        /// </summary>
        uint MaxBitrate { get; }

        /// <summary>
        /// Gets the minimum bitrate of the stream
        /// </summary>
        uint MinBitrate { get; }

        /// <summary>
        /// Notifies when the current bitrate has changed
        /// </summary>
        event EventHandler<CurrentBitrateChangedEventArgs> CurrentBitrateChanged;

        /// <summary>
        /// Notifies when a new chunk has downloaded
        /// </summary>
        event EventHandler<ChunkDownloadedEventArgs> ChunkDownloaded;

        /// <summary>
        /// Notifies when a chunk fails to download
        /// </summary>
        event EventHandler<ChunkFailureEventArgs> ChunkFailure;
    }

    /// <summary>
    /// Represents event args for the CurrentBitrateChanged event.
    /// </summary>
    public sealed class CurrentBitrateChangedEventArgs : Object
    { }

	/// <summary>
	/// Represents event args for the ChunkDownloaded event.
	/// </summary>
	public sealed class ChunkDownloadedEventArgs
	{
        /// <summary>
        /// The index of the time stamp of the chunk
        /// </summary>
        public int ChunkId { get; set; }

        /// <summary>
        /// The stream type (e.g. audio or video)
        /// </summary>
        public string StreamType { get; set; }

        /// <summary>
        /// The timestamp of the chunk. Matches the timestamp that is part of the url for the chunk itself.
        /// </summary>
        public ulong StartTime { get; set; }

        /// <summary>
        /// Gets or sets the time it took to download the chunk in milliseconds
        /// </summary>
        public uint DownloadTimeMs { get; set; }

        /// <summary>
        /// Gets or sets the perceived bandwidth for the chunk in bps.
        /// </summary>
        public uint PerceivedBandwidth { get; set; }

        /// <summary>
        /// Gets or sets the bitrate of the chunk in bps.
        /// </summary>
        public uint Bitrate { get; set; }

        /// <summary>
        /// Gets or sets the byte count of the chunk.
        /// </summary>
        public uint ByteCount { get; set; }
    }

	/// <summary>
	/// Represents event args for the ChunkFailure event.
	/// </summary>
	public sealed class ChunkFailureEventArgs
	{
        /// <summary>
        /// The ID or url of the chunk
        /// </summary>
        public string ChunkId { get; set; }

        /// <summary>
        /// The HTTP error code for the chunk.
        /// </summary>
        public int HttpResponse { get; set; }
    }
}
