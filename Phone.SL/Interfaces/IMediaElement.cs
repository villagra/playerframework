#define CODE_ANALYSIS

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Defines a contract for a MediaElement that can be used by the MediaPlayer
    /// </summary>
    public interface IMediaElement
    {
        /// <summary>Occurs when the <see cref="P:System.Windows.Controls.MediaElement.BufferingProgress" /> property changes.</summary>
        event RoutedEventHandler BufferingProgressChanged;
        /// <summary>Occurs when the value of the <see cref="P:System.Windows.Controls.MediaElement.CurrentState" /> property changes.</summary>
        event RoutedEventHandler CurrentStateChanged;
        /// <summary>Occurs when the <see cref="P:System.Windows.Controls.MediaElement.DownloadProgress" /> property has changed.</summary>
        event RoutedEventHandler DownloadProgressChanged;
        /// <summary>Occurs when the log is ready.</summary>
        event LogReadyRoutedEventHandler LogReady;
        /// <summary>Occurs when a timeline marker is encountered during media playback.</summary>
        event TimelineMarkerRoutedEventHandler MarkerReached;
        /// <summary>Occurs when the <see cref="T:System.Windows.Controls.MediaElement" />  is no longer playing audio or video.</summary>
        event RoutedEventHandler MediaEnded;
        /// <summary>Occurs when there is an error associated with the media <see cref="P:System.Windows.Controls.MediaElement.Source" />.</summary>
        event EventHandler<ExceptionRoutedEventArgs> MediaFailed;
        /// <summary>Occurs when the media stream has been validated and opened, and the file headers have been read.</summary>
        event RoutedEventHandler MediaOpened;
        /// <summary>Pauses media at the current position.</summary>
        void Pause();
        /// <summary>Plays media from the current position.</summary>
        void Play();
        /// <summary>Sends a request to generate a log which will then be raised through the <see cref="E:System.Windows.Controls.MediaElement.LogReady" /> event.</summary>
        void RequestLog();
        /// <summary>Sets the <see cref="P:System.Windows.Controls.MediaElement.Source" /> property using the supplied stream.</summary>
        /// <param name="stream">A stream that contains a natively supported media source.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="stream" /> is null.</exception>
        void SetSource(Stream stream);
        /// <summary>This sets the source of a <see cref="T:System.Windows.Controls.MediaElement" /> to a subclass of <see cref="T:System.Windows.Media.MediaStreamSource" />.</summary>
        /// <param name="mediaStreamSource">A subclass of <see cref="T:System.Windows.Media.MediaStreamSource" />.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="mediaStreamSource" /> is null.</exception>
        /// <exception cref="T:System.InvalidOperationException">The specified <paramref name="mediaStreamSource" /> is invalid, or does not exist.</exception>
        void SetSource(MediaStreamSource mediaStreamSource);
        /// <summary>Stops and resets media to be played from the beginning.</summary>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", Justification = "MediaElement compatibility")]
        void Stop();
        /// <summary>Gets the number of audio streams available in the current media file. </summary>
        /// <returns>The number of audio streams that exist in the source media file. The default value is 0.</returns>
        int AudioStreamCount { get; }
        /// <summary>Gets or sets the index of the audio stream that plays along with the video component. The collection of audio streams is composed at run time and represents all audio streams available within the media file. </summary>
        /// <returns>The index within the media file of the audio component that plays along with the video component. The index can be unspecified, in which case the value is null. The default value is null (see the "Remarks" section).</returns>
        int? AudioStreamIndex { get; set; }
        /// <summary>Gets or sets a value that indicates whether media will begin playback automatically when the <see cref="P:System.Windows.Controls.MediaElement.Source" /> property is set. </summary>
        /// <returns>true if playback is automatic; otherwise, false. The default value is true.</returns>
        bool AutoPlay { get; set; }
        /// <summary>Gets or sets a ratio of volume across stereo speakers. </summary>
        /// <returns>The ratio of volume across speakers in the range between -1 and 1. The default value is 0.</returns>
        double Balance { get; set; }
        /// <summary>Gets a value that indicates the current buffering progress. </summary>
        /// <returns>The amount of buffering that is completed for media content. The value ranges from 0 to 1. Multiply by 100 to obtain a percentage. The default value is 0.</returns>
        double BufferingProgress { get; }
        /// <summary>Gets or sets the amount of time to buffer.</summary>
        /// <returns>The amount of time to buffer. The default value is a <see cref="T:System.TimeSpan" /> with value of 5 seconds (0:0:05).</returns>
        TimeSpan BufferingTime { get; set; }
        /// <summary>Gets a value indicating if media can be paused if the <see cref="M:System.Windows.Controls.MediaElement.Pause" /> method is called. </summary>
        /// <returns>true if the media can be paused; otherwise, false. The default is false.</returns>
        bool CanPause { get; }
        /// <summary>Gets a value indicating if media can be repositioned by setting the value of the <see cref="P:System.Windows.Controls.MediaElement.Position" /> property. </summary>
        /// <returns>true if the media can be repositioned; otherwise, false.The default value is false.</returns>
        bool CanSeek { get; }
        /// <summary>Gets the status of the <see cref="T:System.Windows.Controls.MediaElement" />. </summary>
        /// <returns>The current state of the <see cref="T:System.Windows.Controls.MediaElement" />. The state can be one of the following (as defined in the <see cref="T:System.Windows.Media.MediaElementState" /> enumeration): <see cref="F:System.Windows.Media.MediaElementState.Buffering" />, <see cref="F:System.Windows.Media.MediaElementState.Closed" />, <see cref="F:System.Windows.Media.MediaElementState.Opening" />, <see cref="F:System.Windows.Media.MediaElementState.Paused" />, <see cref="F:System.Windows.Media.MediaElementState.Playing" />, or <see cref="F:System.Windows.Media.MediaElementState.Stopped" />.The default value is <see cref="F:System.Windows.Media.MediaElementState.Closed" />.</returns>
        MediaElementState CurrentState { get; }
        /// <summary>Gets a percentage value indicating the amount of download completed for content located on a remote server.</summary>
        /// <returns>A value that indicates the amount of download completed for content that is located on a remote server. The value ranges from 0 to 1. Multiply by 100 to obtain a percentage. The default value is 0.</returns>
        double DownloadProgress { get; }
        /// <summary>Gets the offset of the download progress. </summary>
        /// <returns>The offset of the download progress.</returns>
        double DownloadProgressOffset { get; }
        /// <summary>Gets the number of frames per second being dropped by the media.</summary>
        /// <returns>The number of frames per second being dropped by the media.</returns>
        double DroppedFramesPerSecond { get; }
        /// <summary>Gets or sets a value indicating whether the audio is muted. </summary>
        /// <returns>true if audio is muted; otherwise, false. The default is false.</returns>
        bool IsMuted { get; set; }
        /// <summary>Gets or sets the <see cref="T:System.Windows.Media.LicenseAcquirer" /> associated with the <see cref="T:System.Windows.Controls.MediaElement" />. The <see cref="T:System.Windows.Media.LicenseAcquirer" /> handles acquiring licenses for DRM encrypted content.</summary>
        /// <returns>The <see cref="T:System.Windows.Media.LicenseAcquirer" /> associated with the <see cref="T:System.Windows.Controls.MediaElement" />. The default is null.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <see cref="T:System.Windows.Media.LicenseAcquirer" /> is null.</exception>
        LicenseAcquirer LicenseAcquirer { get; set; }
        /// <summary>Gets the collection of timeline markers associated with the currently loaded media file.</summary>
        /// <returns>The collection of timeline markers (represented as <see cref="T:System.Windows.Media.TimelineMarker" /> objects) associated with the currently loaded media file. The default value is an empty collection.</returns>
        TimelineMarkerCollection Markers { get; }
        /// <summary>Gets the duration of the media file currently opened.</summary>
        /// <returns>The natural duration of the media. The default value is <see cref="P:System.Windows.Duration.Automatic" />, which is the value held if you query this property before <see cref="E:System.Windows.Controls.MediaElement.MediaOpened" />. </returns>
        Duration NaturalDuration { get; }
        /// <summary>Gets the height of the video associated with the media.</summary>
        /// <returns>The height of the video that is associated with the media, in pixels. Audio files will return 0. The default value is 0.</returns>
        int NaturalVideoHeight { get; }
        /// <summary>Gets the width of the video associated with the media.</summary>
        /// <returns>The width of the video associated with the media. The default value is 0.</returns>
        int NaturalVideoWidth { get; }
        /// <summary>Gets or sets the current position of progress through the media's playback time.</summary>
        /// <returns>The amount of time since the beginning of the media. The default is a <see cref="T:System.TimeSpan" /> with value 0:0:0.</returns>
        TimeSpan Position { get; set; }
        /// <summary>Gets the number of frames per second being rendered by the media.</summary>
        /// <returns>The number of frames per second being rendered by the media.</returns>
        double RenderedFramesPerSecond { get; }
        /// <summary>Gets or sets a media source on the <see cref="T:System.Windows.Controls.MediaElement" />. </summary>
        /// <returns>A string that specifies the source of the element, as a Uniform Resource Identifier (URI). The default value is null.</returns>
        Uri Source { get; set; }
        /// <summary>Gets or sets a <see cref="T:System.Windows.Media.Stretch" /> value that describes how a <see cref="T:System.Windows.Controls.MediaElement" /> fills the destination rectangle. </summary>
        /// <returns>A value of the enumeration that specifies the stretch behavior for the rendered media. The default value is Uniform.</returns>
        Stretch Stretch { get; set; }
        /// <summary>Gets or sets the media's volume. </summary>
        /// <returns>The media's volume represented on a linear scale between 0 and 1. The default is 0.5.</returns>
        double Volume { get; set; }
        /// <summary>
        /// Gets a task that can be awaited for applying the template
        /// </summary>
        Task TemplateAppliedTask { get; }

#if !WINDOWS_PHONE
        /// <summary>Occurs when the <see cref="P:System.Windows.Controls.MediaElement.PlaybackRate" /> property changes.</summary>
        event RateChangedRoutedEventHandler RateChanged;
        /// <summary>Gets or sets the playback rate of the media.</summary>
        /// <returns>The playback rate.</returns>
        double PlaybackRate { get; set; }
        /// <summary>Gets a value that indicates whether the <see cref="T:System.Windows.Controls.MediaElement" /> is being decoded in hardware.</summary>
        /// <returns>true if the <see cref="T:System.Windows.Controls.MediaElement" /> is being decoded in hardware; otherwise, false.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", Justification = "MediaElement compatibility")]
        bool IsDecodingOnGPU { get; }
        /// <summary>Gets the collection of attributes that corresponds to the current entry in the ASX file that <see cref="P:System.Windows.Controls.MediaElement.Source" /> is set to.</summary>
        /// <returns>The collection of attributes that corresponds to the current entry in the ASX file that <see cref="P:System.Windows.Controls.MediaElement.Source" /> is set to.</returns>
        Dictionary<string, string> Attributes { get; }
#endif
    }
}
