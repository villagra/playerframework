using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.ApplicationModel.Resources;

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
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AudioList = GetTemplateChild("AudioList") as ListBox;
            LayoutRoot = GetTemplateChild("LayoutRoot") as Panel;
            AudioList.SelectionChanged += ListBox_SelectionChanged;
            LayoutRoot.PointerPressed += LayoutRoot_PointerPressed;
        }
        void LayoutRoot_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (Close != null) Close(this, EventArgs.Empty);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Close != null) Close(this, EventArgs.Empty);
        }
    }
}
