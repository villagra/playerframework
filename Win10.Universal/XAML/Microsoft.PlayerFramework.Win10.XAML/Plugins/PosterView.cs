using System.ComponentModel;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A control that is used to display a poster.
    /// </summary>
    public class PosterView : Control
    {
        /// <summary>
        /// The Image control used to display the poster.
        /// </summary>
        protected Image ImageElement { get; private set; }

        /// <summary>
        /// Creates a new instance of the control
        /// </summary>
        public PosterView()
        {
            this.DefaultStyleKey = typeof(PosterView);
        }

        /// <inheritdoc /> 
#if SILVERLIGHT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            ImageElement = GetTemplateChild("ImageElement") as Image;
            if (ImageElement != null)
            {
                ImageElement.Source = Source;
                ImageElement.Stretch = Stretch;
            }
        }

        #region Source
        /// <summary>
        /// Identifies the Source dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(PosterView), new PropertyMetadata(null, (d, e) => ((PosterView)d).OnSourceChanged(e.NewValue as ImageSource)));

        void OnSourceChanged(ImageSource newValue)
        {
            if (ImageElement != null) ImageElement.Source = newValue;
        }

        /// <summary>
        /// Gets or sets the image source for the poster to display.
        /// </summary>
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        #endregion

        #region Stretch
        /// <summary>
        /// Identifies the Stretch dependency property.
        /// </summary>
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(PosterView), new PropertyMetadata(Stretch.Uniform, (d, e) => ((PosterView)d).OnStretchChanged((Stretch)e.NewValue)));

        void OnStretchChanged(Stretch newValue)
        {
            if (ImageElement != null) ImageElement.Stretch = newValue;
        }

        /// <summary>
        /// Gets or sets the Stretch property of the poster.
        /// </summary>
        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }
        #endregion
    }
}
