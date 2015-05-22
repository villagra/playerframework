using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// An ItemsControl used to position items in a linear path based on relative coordinates (defined by attached properties).
    /// </summary>
    public class PositionedItemsControl : PositionedItemsPanel
    {
        /// <summary>
        /// Raised when a new item is loaded/added
        /// </summary>
        public event EventHandler<FrameworkElementEventArgs> ItemLoaded;

        /// <summary>
        /// Raised when an item is unloaded/removed
        /// </summary>
        public event EventHandler<FrameworkElementEventArgs> ItemUnloaded;

        #region ItemsSource
        /// <summary>
        /// ItemsSource DependencyProperty definition.
        /// </summary>
#if SILVERLIGHT
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(PositionedItemsControl), new PropertyMetadata(null, (d, e) => ((PositionedItemsControl)d).OnItemsSourceChanged(e.OldValue as IEnumerable, e.NewValue as IEnumerable)));

#else
        // HACK: Bug in Win8 doesn't allow us to bind to IEnumerable. Remove when fixed.
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), typeof(PositionedItemsControl), new PropertyMetadata(null, (d, e) => ((PositionedItemsControl)d).OnItemsSourceChanged(e.OldValue as IEnumerable, e.NewValue as IEnumerable)));

#endif
        void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (oldValue != null)
            {
                if (oldValue is INotifyCollectionChanged)
                {
                    ((INotifyCollectionChanged)oldValue).CollectionChanged -= CollectionChanged;
                }
            }
            Children.Clear();

            if (newValue != null)
            {
                if (newValue is INotifyCollectionChanged)
                {
                    ((INotifyCollectionChanged)newValue).CollectionChanged += CollectionChanged;
                }

                foreach (var item in newValue)
                {
                    LoadNewItem(item);
                }
            }
        }

        void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                UnloadAllItems();
                foreach (var item in ItemsSource)
                {
                    LoadNewItem(item);
                }
            }
            else
            {
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        LoadNewItem(item);
                    }
                }
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        UnloadNewItem(item);
                    }
                }
            }
        }

        private void UnloadAllItems()
        {
            foreach (var child in Children.OfType<FrameworkElement>().ToList())
            {
                child.DataContext = null;
                Children.Remove(child);
                if (ItemUnloaded != null) ItemUnloaded(this, new FrameworkElementEventArgs(child));
            }
        }

        private void UnloadNewItem(object item)
        {
            var child = Children.OfType<FrameworkElement>().FirstOrDefault(c => c.DataContext == item);
            if (child != null)
            {
                child.DataContext = null;
                Children.Remove(child);
                if (ItemUnloaded != null) ItemUnloaded(this, new FrameworkElementEventArgs(child));
            }
        }

        private void LoadNewItem(object item)
        {
            var child = ItemTemplate.LoadContent() as FrameworkElement;
            if (child != null)
            {
                child.DataContext = item;
                Children.Add(child);
                if (ItemLoaded != null) ItemLoaded(this, new FrameworkElementEventArgs(child));
            }
        }

        /// <summary>
        /// Gets or sets the actual value of the slider to be able to maintain the value of the slider while the user is scrubbing.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        #endregion

        #region ItemTemplate
        /// <summary>
        /// ItemTemplate DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(PositionedItemsControl), new PropertyMetadata(null, (d, e) => ((PositionedItemsControl)d).OnItemTemplateChanged(e.NewValue as DataTemplate)));

        void OnItemTemplateChanged(DataTemplate newValue)
        {
        }

        /// <summary>
        /// Gets or sets the actual value of the slider to be able to maintain the value of the slider while the user is scrubbing.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        #endregion
    }

    /// <summary>
    /// Represents event args that contain a FrameworkElement.
    /// </summary>
    public class FrameworkElementEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new instance of FrameworkElementEventArgs.
        /// </summary>
        /// <param name="element">The element associated with the event args.</param>
        public FrameworkElementEventArgs(FrameworkElement element)
        {
            Element = element;
        }

        /// <summary>
        /// Gets the element associated with the event args.
        /// </summary>
        public FrameworkElement Element { get; private set; }
    }
}
