#define CODE_ANALYSIS

using System.Linq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A plugin used to show the user a poster image.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Correctly named architectural pattern")]
#if MEF
    [System.ComponentModel.Composition.PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    [System.ComponentModel.Composition.Export(typeof(IPlugin))]
#endif
    public sealed class PosterPlugin : PluginBase
    {
        PosterView posterElement;
        Panel posterContainer;

        /// <summary>
        /// Gets or sets the style to be used for the PosterView
        /// </summary>
        public Style PosterViewStyle { get; set; }

        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            if (CurrentMediaSource.PosterSource != null)
            {
                posterContainer = MediaPlayer.Containers.OfType<Panel>().FirstOrDefault(e => e.Name == MediaPlayerTemplateParts.PosterContainer);
                if (posterContainer != null)
                {
                    posterElement = new PosterView()
                    {
                        Source = CurrentMediaSource.PosterSource,
                        Style = PosterViewStyle
                    };
                    posterContainer.Children.Add(posterElement);
#if SILVERLIGHT
                    posterElement.Stretch = MediaPlayer.Stretch;
                    MediaPlayer.StretchChanged += MediaPlayer_StretchChanged;
#endif
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc /> 
        protected override void OnDeactivate()
        {
            posterContainer.Children.Remove(posterElement);
            posterElement = null;
            posterContainer = null;

#if SILVERLIGHT
            MediaPlayer.StretchChanged -= MediaPlayer_StretchChanged;
#endif
        }

#if SILVERLIGHT
        void MediaPlayer_StretchChanged(object sender, RoutedPropertyChangedEventArgs<Stretch> e)
        {
            posterElement.Stretch = e.NewValue;
        }
#endif
    }
}
