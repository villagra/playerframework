using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
#endif

namespace Microsoft.Media.TimedText
{
    public static class Extensions
    {
        public static bool ContainsKeyIgnoreCase<TValue>(this IDictionary<string, TValue> dictionary, string key)
        {
            return dictionary.Keys.Any(i => i.Equals(key, StringComparison.CurrentCultureIgnoreCase));
        }

        public static TValue GetEntryIgnoreCase<TValue>(this IDictionary<string, TValue> dictionary, string key)
        {
            return dictionary.GetEntryIgnoreCase(key, default(TValue));
        }

        public static TValue GetEntryIgnoreCase<TValue>(this IDictionary<string, TValue> dictionary, string key,
                                                        TValue defaultValue)
        {
            key = dictionary.Keys.Where(i => i.Equals(key, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            return !key.IsNullOrWhiteSpace()
                       ? dictionary[key]
                       : defaultValue;
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
#if WINDOWS_PHONE || SILVERLIGHT3
            return (str == null) || (str.Trim().Length == 0) ;
#else
            return string.IsNullOrWhiteSpace(str);
#endif
        }

        public static bool EnumTryParse<T>(this string strType, bool ignoreCase, out T result)
        {
            try
            {
                result = (T)Enum.Parse(typeof(T), strType, ignoreCase);
                return true;
            }
            catch
            {
                result = default(T);
                return false;
            }
        }

        public static TParent GetVisualParent<TParent>(this DependencyObject child) where TParent : DependencyObject
        {
            DependencyObject parent = child;

            while (parent != null && !(parent is TParent))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as TParent;
        }

        public static TChild GetVisualChild<TChild>(this DependencyObject parent) where TChild : DependencyObject
        {
            TChild visualChild = null;
            int children = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; visualChild == null && i < children; i++)
            {
                DependencyObject target = VisualTreeHelper.GetChild(parent, i);
                if (target is TChild)
                {
                    visualChild = target as TChild;
                }
                else
                {
                    visualChild = GetVisualChild<TChild>(target);
                }
            }

            return visualChild;
        }

        public static TChild GetVisualChild<TChild>(this DependencyObject parent, string name) where TChild : FrameworkElement
        {
            TChild visualChild = null;
            int children = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; visualChild == null && i < children; i++)
            {
                DependencyObject target = VisualTreeHelper.GetChild(parent, i);
                if (target is TChild && ((FrameworkElement)target).Name == name)
                {
                    visualChild = target as TChild;
                }
                else
                {
                    visualChild = GetVisualChild<TChild>(target, name);
                }
            }

            return visualChild;
        }

        public static IList<TChild> GetVisualChildren<TChild>(this DependencyObject parent)
            where TChild : DependencyObject
        {
            var visualChildren = new List<TChild>();
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject target = VisualTreeHelper.GetChild(parent, i);
                if (target is TChild)
                {
                    visualChildren.Add(target as TChild);
                }
                else
                {
                    IList<TChild> result = GetVisualChildren<TChild>(target);
                    visualChildren.AddRange(result);
                }
            }

            return visualChildren;
        }


        public static IList<string> ToDelimitedList(this string list, string delimiter = ",")
        {
            var delimiters = new[] { delimiter };
            return !list.IsNullOrWhiteSpace()
                       ? list.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
                             .Select(i => i.Trim())
                             .ToList()
                       : new List<string>();
        }

        public static IEnumerable<TItem> ForEach<TItem>(this IEnumerable<TItem> items, Action<TItem> action)
        {
            if (items != null)
            {
                foreach (TItem item in items)
                {
                    action(item);
                }
            }

            return items;
        }

        public static TResult[] ForEach<TItem, TResult>(this IEnumerable<TItem> items, Func<TItem, TResult> func)
        {
            var results = new List<TResult>();

            foreach (TItem item in items)
            {
                TResult result = func(item);
                results.Add(result);
            }

            return results.ToArray();
        }

        public static ObservableCollection<TItem> ToObservableCollection<TItem>(this IEnumerable<TItem> items)
        {
#if WINDOWS_PHONE || SILVERLIGHT3
            var ret = new ObservableCollection<TItem>();
            foreach (TItem itm in items)
            {
                ret.Add(itm);
            }
            return ret;
#else
            return new ObservableCollection<TItem>(items);
#endif
        }

        public static void IfType<TTarget>(this object item, Action<TTarget> action)
        {
            if (item is TTarget)
            {
                action((TTarget)item);
            }
        }

        public static void IfNotNull<TItem>(this TItem item, Action<TItem> action) where TItem : class
        {
            if (item != null)
            {
                action(item);
            }
        }

        public static void IfNotNull<TItem>(this IEnumerable<TItem> items, Action<TItem> action) where TItem : class
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    item.IfNotNull(action);
                }
            }
        }

        public static void IfTrue(this bool predicate, Action action)
        {
            if (predicate)
            {
                action();
            }
        }
        
        public static void GoToVisualState(this Control control, string state, bool useTransitions = true)
        {
            VisualStateManager.GoToState(control, state, useTransitions);
        }
        
        public static double GetEffectiveWidth(this FrameworkElement visualElement)
        {
            return !double.IsNaN(visualElement.Width) && visualElement.Width > 0
                                ? visualElement.Width
                                : !double.IsNaN(visualElement.ActualWidth)
                                    ? visualElement.ActualWidth
                                    : 0;
        }

        public static double GetEffectiveHeight(this FrameworkElement visualElement)
        {
            return !double.IsNaN(visualElement.Height) && visualElement.Height > 0
                                ? visualElement.Height
                                : !double.IsNaN(visualElement.ActualHeight)
                                    ? visualElement.ActualHeight
                                    : 0;
        }

        public static Size GetEffectiveSize(this FrameworkElement visualElement)
        {
            return new Size
            {
                Width = visualElement.GetEffectiveWidth(),
                Height = visualElement.GetEffectiveHeight()
            };
        }

        public static IEnumerable<TItem> FlattenList<TItem>(this IEnumerable<TItem> items, Func<TItem, IEnumerable<TItem>> selectChild)
        {
            var children = items != null && items.Any()
                                ? items.SelectMany(i => selectChild(i)).FlattenList(selectChild)
                                : Enumerable.Empty<TItem>();

            return items.Concat(children);
        }

        public static void Remove<T>(this Queue<T> source, T itemToRemove)
        {
            var items = source.ToArray();
            source.Clear();
            foreach (T item in items)
            {
                if (!object.ReferenceEquals(item, itemToRemove))
                    source.Enqueue(item);
            }
        }
    }
}