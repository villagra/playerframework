using System;
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
    /// Represents a marker to appear visually in the timeline.
    /// </summary>
    public class VisualMarker : DependencyObject
    {
        /// <summary>
        /// Creates a new instance of VisualMarker
        /// </summary>
        public VisualMarker()
        {
            IsSeekable = true;
        }

        #region Text
        /// <summary>
        /// Text DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(VisualMarker), null);

        /// <summary>
        /// Gets or sets the actual value of the slider to be able to maintain the value of the slider while the user is scrubbing.
        /// </summary>
        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        #region Time
        /// <summary>
        /// Time DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty TimeProperty = DependencyProperty.Register("Time", typeof(TimeSpan), typeof(VisualMarker), new PropertyMetadata(TimeSpan.Zero));

        /// <summary>
        /// Gets or sets the actual value of the slider to be able to maintain the value of the slider while the user is scrubbing.
        /// </summary>
        public TimeSpan Time
        {
            get { return (TimeSpan)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        #endregion

        #region IsSeekable
        /// <summary>
        /// IsSeekable DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty IsSeekableProperty = DependencyProperty.Register("IsSeekable", typeof(bool), typeof(VisualMarker), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the actual value of the slider to be able to maintain the value of the slider while the user is scrubbing.
        /// </summary>
        public bool IsSeekable
        {
            get { return (bool)GetValue(IsSeekableProperty); }
            set { SetValue(IsSeekableProperty, value); }
        }

        #endregion

        #region Style
        /// <summary>
        /// Style DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty StyleProperty = DependencyProperty.Register("Style", typeof(Style), typeof(VisualMarker), null);

        /// <summary>
        /// Gets or sets the style of the button to show in the timeline.
        /// </summary>
        public Style Style
        {
            get { return (Style)GetValue(StyleProperty); }
            set { SetValue(StyleProperty, value); }
        }

        #endregion

    }
}
