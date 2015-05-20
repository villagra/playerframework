#define CODE_ANALYSIS

using System.Linq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A plugin used to show the user a poster image.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Correctly named architectural pattern")]
    public sealed class PosterPlugin : IPlugin
    {
        PosterView posterElement;
        Panel posterContainer;

        /// <summary>
        /// Gets or sets the style to be used for the PosterView
        /// </summary>
        public Style PosterViewStyle { get; set; }

        void MediaPlayer_PosterSourceChanged(object sender, RoutedPropertyChangedEventArgs<ImageSource> e)
        {
            if (e.NewValue != null)
            {
                if (posterElement != null)
                {
                    posterElement.Source = MediaPlayer.PosterSource;
                }
                else
                {
                    InitPoster();
                }
            }
            else
            {
                if (posterElement != null)
                {
                    DestroyPoster();
                }
            }
        }

        void MediaPlayer_StretchChanged(object sender, RoutedPropertyChangedEventArgs<Stretch> e)
        {
            posterElement.Stretch = e.NewValue;
        }

        void InitPoster()
        {
            if (MediaPlayer.PosterSource != null)
            {
                posterElement = new PosterView()
                {
                    Source = MediaPlayer.PosterSource,
                    Style = PosterViewStyle
                };
                posterContainer.Children.Add(posterElement);
                posterElement.Stretch = MediaPlayer.Stretch;
                MediaPlayer.StretchChanged += MediaPlayer_StretchChanged;
            }
        }

        void DestroyPoster()
        {
            if (posterElement != null)
            {
                posterContainer.Children.Remove(posterElement);
                posterElement = null;
                MediaPlayer.StretchChanged -= MediaPlayer_StretchChanged;
                MediaPlayer.PosterSourceChanged -= MediaPlayer_PosterSourceChanged;
            }
        }

        /// <inheritdoc /> 
        public void Load()
        {
            posterContainer = MediaPlayer.Containers.OfType<Panel>().FirstOrDefault(e => e.Name == MediaPlayerTemplateParts.PosterContainer);
            if (posterContainer != null)
            {
                InitPoster();
                MediaPlayer.PosterSourceChanged += MediaPlayer_PosterSourceChanged;
            }
        }

        /// <inheritdoc /> 
        public void Update(IMediaSource mediaSource)
        { }

        /// <inheritdoc /> 
        public void Unload()
        {
            if (posterContainer != null)
            {
                DestroyPoster();
                posterContainer = null;
            }
        }

        /// <inheritdoc /> 
        public MediaPlayer MediaPlayer { get; set; }
    }
}
