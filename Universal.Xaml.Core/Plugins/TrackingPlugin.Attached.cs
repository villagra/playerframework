using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Microsoft.PlayerFramework
{    
    /// <summary>
    /// Helper class used to attach config data specific to each playlist item.
    /// </summary>
    public static class Tracking
    {
        /// <summary>
        /// Identifies the TrackingEvents attached property.
        /// </summary>
        public static readonly DependencyProperty TrackingEventsProperty = DependencyProperty.RegisterAttached("TrackingEvents", typeof(TrackingEventCollection), typeof(Tracking), null);

        /// <summary>
        /// Sets the TrackingEvents attached property value.
        /// </summary>
        /// <param name="obj">An instance of the MediaPlayer or PlaylistItem.</param>
        /// <param name="propertyValue">A value containing the TrackingEvents to apply to the plugin.</param>
        public static void SetTrackingEvents(DependencyObject obj, TrackingEventCollection propertyValue)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            obj.SetValue(TrackingEventsProperty, propertyValue);
        }

        /// <summary>
        /// Gets the TrackingEvents attached property value.
        /// </summary>
        /// <param name="obj">An instance of the MediaPlayer or PlaylistItem.</param>
        /// <returns>A value containing the TrackingEvents to apply to the plugin.</returns>
        public static TrackingEventCollection GetTrackingEvents(DependencyObject obj)
        {
            if (obj == null) throw new ArgumentNullException("obj");
            return obj.GetValue(TrackingEventsProperty) as TrackingEventCollection;
        }

    }

    /// <summary>
    /// A collection class containing the TrackingEvent objects associated with a PlaylistItem
    /// </summary>
    public class TrackingEventCollection : ObservableCollection<TrackingEventBase>
    { }
}
