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
#if MEF
    [System.ComponentModel.Composition.PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [System.ComponentModel.Composition.Export(typeof(IPlugin))]
#endif
    public class ChaptersPlugin : PluginBase
    {
        const string DefaultMarkerType = "NAME";

        /// <summary>
        /// Creates a new instance of the ChaptersPlugin
        /// </summary>
        public ChaptersPlugin()
        {
            ChapterMarkerType = DefaultMarkerType;
        }

        List<VisualMarker> ChapterMarkers;

        /// <summary>
        /// Gets or sets whether or not captions are enabled
        /// </summary>
        public string ChapterMarkerType { get; set; }

        /// <summary>
        /// Gets or sets whether or not captions are enabled
        /// </summary>
        public Style MarkerStyle { get; set; }

        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            LoadChapters();
            MediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            MediaPlayer.MediaEnded += MediaPlayer_MediaEnded;
            return true;
        }

        /// <inheritdoc /> 
        protected override void OnDeactivate()
        {
            UnloadChapters();
            MediaPlayer.MediaOpened -= MediaPlayer_MediaOpened;
            MediaPlayer.MediaEnded -= MediaPlayer_MediaEnded;
        }

        void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            LoadChapters();
        }

        void MediaPlayer_MediaEnded(object sender, MediaPlayerActionEventArgs e)
        {
            UnloadChapters();
        }

        void LoadChapters()
        {
            ChapterMarkers = new List<VisualMarker>();
            foreach (var chapter in MediaPlayer.Markers.Where(m => m.Type == ChapterMarkerType))
            {
                var marker = new VisualMarker() 
                { 
                    Text = chapter.Text, 
                    Time = chapter.Time,
                    Style = MarkerStyle
                };
                ChapterMarkers.Add(marker);
                MediaPlayer.VisualMarkers.Add(marker);
            }
        }

        void UnloadChapters()
        {
            if (ChapterMarkers != null)
            {
                foreach (var marker in ChapterMarkers)
                {
                    MediaPlayer.VisualMarkers.Remove(marker);
                }
                ChapterMarkers = null;
            }
        }
    }
}
