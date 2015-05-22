using System;
using System.Collections.Generic;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.ApplicationModel.Resources;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A control that allows the user to select which captions they want or to turn off closed captioning.
    /// </summary>
    public class AudioSelectionView : Control
    {
        /// <summary>
        /// Creates a new instance of the control
        /// </summary>
        public AudioSelectionView()
        {
            this.DefaultStyleKey = typeof(AudioSelectionView);
        }

        /// <summary>
        /// Indicates the caption selector view should be closed.
        /// </summary>
        public event EventHandler Close;

        ListBox AudioList;
        Panel LayoutRoot;

        /// <inheritdoc /> 
#if SILVERLIGHT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            AudioList = GetTemplateChild("AudioList") as ListBox;
            LayoutRoot = GetTemplateChild("LayoutRoot") as Panel;

            AudioList.SelectionChanged += ListBox_SelectionChanged;
#if SILVERLIGHT
            LayoutRoot.MouseLeftButtonDown += LayoutRoot_MouseLeftButtonDown;
            AudioList.MouseLeftButtonDown += AudioList_MouseLeftButtonDown;
#else
            LayoutRoot.PointerPressed += LayoutRoot_PointerPressed;
#endif
        }

#if SILVERLIGHT
        void AudioList_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
#else
        void LayoutRoot_PointerPressed(object sender, PointerRoutedEventArgs e)
#endif
        {
            if (Close != null) Close(this, EventArgs.Empty);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Close != null) Close(this, EventArgs.Empty);
        }
    }
}
