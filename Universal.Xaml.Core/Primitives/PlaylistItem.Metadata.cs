using System;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Helper class used to attach config data specific to each playlist item.
    /// </summary>
    public static class PlaylistItemMetadata
    {
        /// <summary>
        /// Identifies the Title attached property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.RegisterAttached("Title", typeof(string), typeof(PlaylistItemMetadata), null);

        /// <summary>
        /// Sets the Title attached property value.
        /// </summary>
        /// <param name="obj">An instance of the PlaylistItem.</param>
        /// <param name="propertyValue">A value containing the title of the PlaylistItem.</param>
        public static void SetTitle(DependencyObject obj, string propertyValue)
        {
            obj.SetValue(TitleProperty, propertyValue);
        }

        /// <summary>
        /// Gets the Title attached property value.
        /// </summary>
        /// <param name="obj">An instance of the MediaPlayer or PlaylistItem.</param>
        public static string GetTitle(DependencyObject obj)
        {
            return obj.GetValue(TitleProperty) as string;
        }

        /// <summary>
        /// Identifies the Description attached property.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.RegisterAttached("Description", typeof(string), typeof(PlaylistItemMetadata), null);

        /// <summary>
        /// Sets the Description attached property value.
        /// </summary>
        /// <param name="obj">An instance of the PlaylistItem.</param>
        /// <param name="propertyValue">A value containing the description of the PlaylistItem.</param>
        public static void SetDescription(DependencyObject obj, string propertyValue)
        {
            obj.SetValue(DescriptionProperty, propertyValue);
        }

        /// <summary>
        /// Gets the Description attached property value.
        /// </summary>
        /// <param name="obj">An instance of the MediaPlayer or PlaylistItem.</param>
        public static string GetDescription(DependencyObject obj)
        {
            return obj.GetValue(DescriptionProperty) as string;
        }

        /// <summary>
        /// Identifies the Thumbnail attached property.
        /// </summary>
        public static readonly DependencyProperty ThumbnailProperty = DependencyProperty.RegisterAttached("Thumbnail", typeof(ImageSource), typeof(PlaylistItemMetadata), null);

        /// <summary>
        /// Sets the Thumbnail attached property value.
        /// </summary>
        /// <param name="obj">An instance of the PlaylistItem.</param>
        /// <param name="propertyValue">A value containing the thumbnail image of the PlaylistItem.</param>
        public static void SetThumbnail(DependencyObject obj, ImageSource propertyValue)
        {
            obj.SetValue(ThumbnailProperty, propertyValue);
        }

        /// <summary>
        /// Gets the Thumbnail attached property value.
        /// </summary>
        /// <param name="obj">An instance of the MediaPlayer or PlaylistItem.</param>
        public static ImageSource GetThumbnail(DependencyObject obj)
        {
            return obj.GetValue(ThumbnailProperty) as ImageSource;
        }

        /// <summary>
        /// Identifies the Duration attached property.
        /// </summary>
        public static readonly DependencyProperty DurationProperty = DependencyProperty.RegisterAttached("Duration", typeof(TimeSpan), typeof(PlaylistItemMetadata), null);

        /// <summary>
        /// Sets the Duration attached property value.
        /// </summary>
        /// <param name="obj">An instance of the PlaylistItem.</param>
        /// <param name="propertyValue">A value containing the duration of the PlaylistItem.</param>
        public static void SetDuration(DependencyObject obj, TimeSpan propertyValue)
        {
            obj.SetValue(DurationProperty, propertyValue);
        }

        /// <summary>
        /// Gets the Duration attached property value.
        /// </summary>
        /// <param name="obj">An instance of the MediaPlayer or PlaylistItem.</param>
        public static TimeSpan GetDuration(DependencyObject obj)
        {
            return (TimeSpan)obj.GetValue(DurationProperty);
        }
    }
}
