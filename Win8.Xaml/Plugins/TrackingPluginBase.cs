using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
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
    /// A plugin used to help track when specific events occur during playback.
    /// </summary>
    public abstract class TrackingPluginBase<T> : PluginBase, IEventTracker where T : TrackingEventBase
    {
        /// <summary>
        /// Creates a new instance of TrackingPlugin.
        /// </summary>
        public TrackingPluginBase()
        {
            TrackingEvents = new ObservableCollection<T>();
        }

        IList<T> trackedEvents = new List<T>();

        /// <inheritdoc /> 
        public event EventHandler<EventTrackedEventArgs> EventTracked;

        /// <summary>
        /// Identifies the PreloadTime dependency property.
        /// </summary>
        public static readonly DependencyProperty TrackingEventsProperty = DependencyProperty.Register("TrackingEvents", typeof(IList<T>), typeof(TrackingPluginBase<T>), new PropertyMetadata(null, (d, e) => ((TrackingPluginBase<T>)d).OnTrackingEventsChanged(e.OldValue as IList<T>, e.NewValue as IList<T>)));

        void OnTrackingEventsChanged(IList<T> oldValue, IList<T> newValue)
        {
            if (oldValue != null)
            {
                UninitializeTrackingEvents(oldValue);
                if (oldValue is INotifyCollectionChanged)
                {
                    ((INotifyCollectionChanged)oldValue).CollectionChanged -= TrackingPlugin_CollectionChanged;
                }
            }

            if (newValue != null)
            {
                InitializeTrackingEvents(newValue);
                if (newValue is INotifyCollectionChanged)
                {
                    ((INotifyCollectionChanged)newValue).CollectionChanged += TrackingPlugin_CollectionChanged;
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount of time before an ad will occur that preloading will begin. Set to null to disable preloading. Default is 5 seconds.
        /// </summary>
        public IList<T> TrackingEvents
        {
            get { return GetValue(TrackingEventsProperty) as IList<T>; }
            set { SetValue(TrackingEventsProperty, value); }
        }

        private void TrackingPlugin_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (MediaPlayer != null && MediaPlayer.PlayerState == PlayerState.Started) // make sure we're started. If not, wait until MediaStarted fires
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    foreach (var item in trackedEvents.ToList())
                    {
                        UninitializeTrackingEvent(item);
                    }
                    foreach (var item in TrackingEvents)
                    {
                        InitializeTrackingEvent(item);
                    }
                }
                else
                {
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems.Cast<T>())
                        {
                            UninitializeTrackingEvent(item);
                        }
                    }
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems.Cast<T>())
                        {
                            InitializeTrackingEvent(item);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Manually fires an event
        /// </summary>
        /// <param name="eventArgs">The event to fire</param>
        protected virtual void OnTrackEvent(EventTrackedEventArgs eventArgs)
        {
            if (EventTracked != null) EventTracked(this, eventArgs);
        }

        /// <summary>
        /// Unsubscribes all tracking events.
        /// </summary>
        protected virtual void UninitializeTrackingEvents(IList<T> trackingEvents)
        {
            foreach (var trackingEvent in trackingEvents)
            {
                UninitializeTrackingEvent(trackingEvent);
            }
        }

        /// <summary>
        /// Subscribes all tracking events.
        /// </summary>
        protected virtual void InitializeTrackingEvents(IList<T> trackingEvents)
        {
            foreach (var trackingEvent in trackingEvents)
            {
                InitializeTrackingEvent(trackingEvent);
            }
        }

        /// <summary>
        /// Provides an opportunity to initialize an individual tracking event.
        /// </summary>
        /// <param name="trackingEvent">The tracking event being subscribed to.</param>
        protected virtual void InitializeTrackingEvent(T trackingEvent)
        {
            trackedEvents.Add(trackingEvent);
        }

        /// <summary>
        /// Provides an opportunity to uninitialize an individual tracking event.
        /// </summary>
        /// <param name="trackingEvent">The tracking event being unsubscribed.</param>
        protected virtual void UninitializeTrackingEvent(T trackingEvent)
        {
            trackedEvents.Remove(trackingEvent);
        }

        /// <inheritdoc /> 
        protected override void OnUpdate()
        {
            TrackingEvents = Tracking.GetTrackingEvents((DependencyObject)CurrentMediaSource).OfType<T>().ToList();
            base.OnUpdate();
        }

        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            InitializeTrackingEvents(TrackingEvents);
            return true;
        }

        /// <inheritdoc /> 
        protected override void OnDeactivate()
        {
            UninitializeTrackingEvents(TrackingEvents);
        }
    }
    /// <summary>
    /// An interface used to implement a tracking plugin.
    /// </summary>
    public interface IEventTracker
    {
        /// <summary>
        /// Raised when a tracking event occurs
        /// </summary>
        event EventHandler<EventTrackedEventArgs> EventTracked;
    }

    /// <summary>
    /// A base class that all tracking events inherit.
    /// </summary>
    public abstract class TrackingEventBase
    {
        /// <summary>
        /// Gets or sets extra data associated with a tracking event.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets a string used to identify which component is in need of this event. 
        /// This can allow multiple components to register events to track and only care about the ones they created.
        /// </summary>
        public string Area { get; set; }
    }

    /// <summary>
    /// Contains additional information about a tracking event that has occurred.
    /// </summary>
    public class EventTrackedEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of EventTrackedEventArgs
        /// </summary>
        public EventTrackedEventArgs()
        {
            Timestamp = DateTimeOffset.Now;
        }

        /// <summary>
        /// Creates a new instance of EventTrackedEventArgs
        /// </summary>
        /// <param name="trackingEvent">The event that was tracked</param>
        public EventTrackedEventArgs(TrackingEventBase trackingEvent)
            : this()
        {
            TrackingEvent = trackingEvent;
        }

        /// <summary>
        /// Gets the timestamp when the event occurred.
        /// </summary>
        public DateTimeOffset Timestamp { get; private set; }

        /// <summary>
        /// Gets the tracking event instance that occurred.
        /// </summary>
        public TrackingEventBase TrackingEvent { get; private set; }
    }
}
