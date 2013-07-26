using System;
using System.Net;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework.Advertising
{
    /// <summary>
    /// Helper class used to attach config data specific to each playlist item.
    /// </summary>
    public static class FreeWheel
    {
        /// <summary>
        /// Identifies the Source attached property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached("Source", typeof(Uri), typeof(FreeWheel), null);

        /// <summary>
        /// Sets the Source attached property value.
        /// </summary>
        /// <param name="obj">An instance of the MediaPlayer or PlaylistItem.</param>
        /// <param name="propertyValue">A value containing the MAST Source.</param>
        public static void SetSource(DependencyObject obj, Uri propertyValue)
        {
            obj.SetValue(SourceProperty, propertyValue);
        }

        /// <summary>
        /// Gets the Source attached property value.
        /// </summary>
        /// <param name="obj">An instance of the MediaPlayer or PlaylistItem.</param>
        /// <returns>A value containing the MAST Source.</returns>
        public static Uri GetSource(DependencyObject obj)
        {
            return obj.GetValue(SourceProperty) as Uri;
        }

    }
}
