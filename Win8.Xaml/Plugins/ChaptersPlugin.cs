using System;
using System.Collections.Generic;
using System.Linq;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// An plugin responsible for turning MediaElement markers into visual markers that can be seen in the timeline.
    /// </summary>
    public class ChaptersPlugin : IPlugin
    {
        const string DefaultMarkerType = "NAME";

        List<VisualMarker> chapterMarkers;

        /// <summary>
        /// Creates a new instance of the ChaptersPlugin
        /// </summary>
        public ChaptersPlugin()
        {
            ChapterMarkerType = DefaultMarkerType;
            DefaultChapterCount = 10;
            AutoCreateDefaultChapters = false;
            AutoCreateChaptersFromMarkers = false;
        }

        /// <summary>
        /// Gets or sets the total number of default chapters to create in the timeline. Default is 10.
        /// </summary>
        public int DefaultChapterCount { get; set; }

        /// <summary>
        /// Gets or sets whether to automatically create default chapters when the media loads. Default is false.
        /// </summary>
        public bool AutoCreateDefaultChapters { get; set; }

        /// <summary>
        /// Gets or sets whether to automatically create chapters from markers embedded in the stream. Note: not supported on Windows 8 due to limitations of the MediaElement. Default is false.
        /// </summary>
        public bool AutoCreateChaptersFromMarkers { get; set; }

        /// <summary>
        /// Gets or sets whether or not captions are enabled
        /// </summary>
        public string ChapterMarkerType { get; set; }

        /// <summary>
        /// Gets or sets whether or not captions are enabled
        /// </summary>
        public Style MarkerStyle { get; set; }

        /// <inheritdoc /> 
        public void Load()
        {
            MediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            MediaPlayer.MediaClosed += MediaPlayer_MediaClosed;
        }

        /// <inheritdoc /> 
        public void Update(IMediaSource mediaSource)
        {
            // do nothing
        }

        /// <inheritdoc /> 
        public void Unload()
        {
            MediaPlayer.MediaOpened -= MediaPlayer_MediaOpened;
            MediaPlayer.MediaClosed -= MediaPlayer_MediaClosed;
        }

        /// <inheritdoc /> 
        public MediaPlayer MediaPlayer { get; set; }

        void MediaPlayer_MediaClosed(object sender, RoutedEventArgs e)
        {
            UnloadChapters();
        }

        void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            LoadChapters();
        }

        void LoadChapters()
        {
            chapterMarkers = new List<VisualMarker>();
            if (AutoCreateChaptersFromMarkers)
            {
                foreach (var chapter in MediaPlayer.Markers.Where(m => m.Type == ChapterMarkerType))
                {
                    var marker = new VisualMarker()
                    {
                        Text = chapter.Text,
                        Time = chapter.Time,
                        IsSeekable = true
                    };
                    if (MarkerStyle != null) marker.Style = MarkerStyle;
                    chapterMarkers.Add(marker);
                    MediaPlayer.VisualMarkers.Add(marker);
                }
            }
            if (AutoCreateDefaultChapters)
            {
                var chapterLength = MediaPlayer.Duration.TotalSeconds / DefaultChapterCount;
                for (int i = 0; i <= DefaultChapterCount; i++)
                {
                    var marker = new VisualMarker()
                    {
                        Time = TimeSpan.FromSeconds(chapterLength * i),
                        IsSeekable = true
                    };
                    chapterMarkers.Add(marker);
                    MediaPlayer.VisualMarkers.Add(marker);
                }
            }
        }

        void UnloadChapters()
        {
            if (chapterMarkers != null)
            {
                foreach (var marker in chapterMarkers)
                {
                    MediaPlayer.VisualMarkers.Remove(marker);
                }
                chapterMarkers = null;
            }
        }
    }
}
