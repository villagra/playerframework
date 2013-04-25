#if SILVERLIGHT
using System.Windows;
#endif

namespace Windows.UI.Xaml
{
    /// <summary>
    /// Represents methods that will handle various routed events that track property value changes.
    /// </summary>
    /// <typeparam name="T">The type of the property value where changes in value are reported.</typeparam>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void RoutedPropertyChangedEventHandler<T>(object sender, RoutedPropertyChangedEventArgs<T> e);

    /// <summary>
    /// Provides data about a change in value to a dependency property as reported by particular routed events, including hte previous and current value of the property that changed.
    /// </summary>
    /// <typeparam name="T">The type of the dependency property that has changed.</typeparam>
    public class RoutedPropertyChangedEventArgs<T> : RoutedEventArgs
    {
        /// <summary>
        /// Creates a new instance of RoutedPropertyChangedEventArgs.
        /// </summary>
        /// <param name="oldValue">the previous value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        public RoutedPropertyChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// Gets the new value of the property.
        /// </summary>
        public T NewValue { get; internal set; }

        /// <summary>
        /// Gets the previous value of the property.
        /// </summary>
        public T OldValue { get; internal set; }
    }
}
