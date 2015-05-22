using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A control that acts like a list box but presents a separate option for unselecting to the user.
    /// </summary>
    public class DeselectableListBox : Control
    {
        /// <summary>
        /// The internal instance listbox used by this control to display the items.
        /// </summary>
        protected ListBox ListBox { get; private set; }

        private EnumerableWrapper items;

        /// <summary>
        /// Indicates the selection has changed.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Creates a new instance of DeselectableListBox.
        /// </summary>
        public DeselectableListBox()
        {
            this.DefaultStyleKey = typeof(DeselectableListBox);
        }

        /// <summary>
        /// Identifies the SelectedIdentifierText dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIdentifierTextProperty = DependencyProperty.Register("SelectedIdentifierText", typeof(string), typeof(DeselectableListBox), new PropertyMetadata(DefaultSelectedIdentifierText));

        /// <summary>
        /// Gets or sets the text to identify that an item is selected.
        /// </summary>
        public string SelectedIdentifierText
        {
            get { return GetValue(SelectedIdentifierTextProperty) as string; }
            set { SetValue(SelectedIdentifierTextProperty, value); }
        }

        static string DefaultSelectedIdentifierText
        {
            get
            {
                return MediaPlayer.GetResourceString("SelectedIdentifierText");
            }
        }

        static string DefaultDeselectedItemText
        {
            get
            {
                return MediaPlayer.GetResourceString("DeselectedIdentifierText");
            }
        }

        /// <inheritdoc /> 
#if SILVERLIGHT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            ListBox = GetTemplateChild("ListBox") as ListBox;

            ListBox.ItemTemplate = ItemTemplate;
            ListBox.ItemsSource = items;
            ListBox.SelectedItem = SelectedItem;
            ListBox.SelectionChanged += ListBox_SelectionChanged;
        }

        void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItem = ListBox.SelectedItem == DeselectedItem ? null : ListBox.SelectedItem;
            if (SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }
        }

#if SILVERLIGHT

        /// <summary>
        /// ItemsSource DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(DeselectableListBox), new PropertyMetadata(null, (d, e) => ((DeselectableListBox)d).OnItemsSourceChanged(e.NewValue as IEnumerable)));

        /// <summary>
        /// Gets or sets the collection of items to be used as the item source.
        /// </summary>
        public IEnumerable ItemsSource
#else
        /// <summary>
        /// ItemsSource DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), typeof(DeselectableListBox), new PropertyMetadata(null, (d, e) => ((DeselectableListBox)d).OnItemsSourceChanged(e.NewValue as IEnumerable)));

        /// <summary>
        /// Gets or sets the collection of items to be used as the item source.
        /// </summary>
        public object ItemsSource
#endif
        {
            get { return GetValue(ItemsSourceProperty) as IEnumerable; }
            set { SetValue(ItemsSourceProperty, value); }
        }

        void OnItemsSourceChanged(IEnumerable itemsSource)
        {
            items = new EnumerableWrapper(itemsSource);
            items.StartingItem = DeselectedItem;
            items.IncludeStartingItem = SelectedItem != null;
            if (ListBox != null)
            {
                ListBox.ItemsSource = items;
                ListBox.SelectedItem = SelectedItem;
            }
        }

        /// <summary>
        /// SelectedItem DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(DeselectableListBox), new PropertyMetadata(null, (d, e) => ((DeselectableListBox)d).OnSelectedItemChanged(e.NewValue as object)));

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        void OnSelectedItemChanged(object newSelectedItem)
        {
            if (items != null)
            {
                items.IncludeStartingItem = newSelectedItem != null;
                if (ListBox != null)
                {
                    ListBox.SelectedItem = newSelectedItem;
                }
            }
        }

        /// <summary>
        /// DeselectedItem DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty DeselectedItemProperty = DependencyProperty.Register("DeselectedItem", typeof(object), typeof(DeselectableListBox), new PropertyMetadata(DefaultDeselectedItemText, (d, e) => ((DeselectableListBox)d).OnDeselectedItemChanged(e.NewValue as object)));

        /// <summary>
        /// Gets or sets the item that is used to indicate nothing is selected.
        /// </summary>
        public object DeselectedItem
        {
            get { return GetValue(DeselectedItemProperty); }
            set { SetValue(DeselectedItemProperty, value); }
        }

        void OnDeselectedItemChanged(object newDeselectedItem)
        {
            if (items != null)
            {
                items.StartingItem = newDeselectedItem;
            }
        }

        /// <summary>
        /// ItemTemplate DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(DeselectableListBox), new PropertyMetadata(null, (d, e) => ((DeselectableListBox)d).OnItemTemplateChanged(e.NewValue as DataTemplate)));

        /// <summary>
        /// Gets or sets the item template to be used to display each item in the list.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return GetValue(ItemTemplateProperty) as DataTemplate; }
            set { SetValue(ItemTemplateProperty, value); }
        }

        void OnItemTemplateChanged(DataTemplate newItemTemplate)
        {
            if (ListBox != null)
            {
                ListBox.ItemTemplate = newItemTemplate;
            }
        }

        class EnumerableWrapper : IEnumerable, INotifyCollectionChanged
        {
            IEnumerable items;
            ObservableCollection<object> allItems;
            bool includeStartingItem;

            /// <summary>
            /// Creates a new instance of EnumerableWrapper.
            /// </summary>
            /// <param name="Items">The collection of items to wrap.</param>
            public EnumerableWrapper(IEnumerable Items)
            {
                allItems = new ObservableCollection<object>(Items != null ? Items.Cast<object>() : new object[] { });
                allItems.CollectionChanged += new NotifyCollectionChangedEventHandler(allItems_CollectionChanged);

                items = Items;
                if (items is INotifyCollectionChanged)
                {
                    ((INotifyCollectionChanged)items).CollectionChanged += EnumerableWrapper_CollectionChanged;
                }
            }

            void allItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (CollectionChanged != null)
                {
                    CollectionChanged(this, e);
                }
            }

            void EnumerableWrapper_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    allItems.Clear();
                    if (IncludeStartingItem)
                    {
                        allItems.Add(StartingItem);
                    }
                    foreach (var item in items)
                    {
                        allItems.Add(item);
                    }
                }
                else
                {
                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems)
                        {
                            allItems.Remove(item);
                        }
                    }
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems)
                        {
                            allItems.Add(item);
                        }
                    }
                }
            }

            /// <summary>
            /// Gets or sets the starting item of the collection
            /// </summary>
            public object StartingItem { get; set; }

            /// <summary>
            /// Gets or sets whether or not to include StartingItem in the collection.
            /// </summary>
            public bool IncludeStartingItem
            {
                get { return includeStartingItem; }
                set
                {
                    if (includeStartingItem != value)
                    {
                        includeStartingItem = value;
                        if (includeStartingItem)
                        {
                            allItems.Insert(0, StartingItem);
                        }
                        else
                        {
                            allItems.RemoveAt(0);
                        }
                    }
                }
            }

            /// <inheritdoc /> 
            public IEnumerator GetEnumerator()
            {
                return allItems.GetEnumerator();
            }

            /// <inheritdoc /> 
            public event NotifyCollectionChangedEventHandler CollectionChanged;
        }
    }

    /// <summary>
    /// Represents a special listbox that contains ListBoxItem objects with knowlege of the listbox. This is useful for binding.
    /// </summary>
    public class ParentAwareListBox : ListBox
    {
        /// <inheritdoc /> 
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ParentAwareListBoxItem(this);
        }
    }

    /// <summary>
    /// Represents a special listbox that contains ListBoxItem objects with knowlege of the listbox. This is useful for binding.
    /// </summary>
    public class ParentAwareListBoxItem : ListBoxItem
    {
        /// <summary>
        /// Creates a new instance of ParentAwareListBoxItem.
        /// </summary>
        /// <param name="parentListBox">The parent ListBox</param>
        public ParentAwareListBoxItem(ParentAwareListBox parentListBox)
        {
            ParentListBox = parentListBox;
        }

        /// <summary>
        /// Identifies the ParentListBox dependency property.
        /// </summary>
        public static readonly DependencyProperty ParentListBoxProperty = DependencyProperty.Register("ParentListBox", typeof(ParentAwareListBox), typeof(ParentAwareListBoxItem), null);

        /// <summary>
        /// Gets the parent list box.
        /// </summary>
        public ParentAwareListBox ParentListBox
        {
            get { return GetValue(ParentListBoxProperty) as ParentAwareListBox; }
            private set { SetValue(ParentListBoxProperty, value); }
        }
    }
}
