using System;
#if SILVERLIGHT
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Control used to display a thumbnail image (typically while scrubbing)
    /// </summary>
    public sealed class ThumbnailView : Control
    {
        int currentImageElementIndex = 0;
        const int imageElementCount = 2;
        readonly Image[] imageElements = new Image[imageElementCount];
        bool isTemplateApplied = false;

        /// <summary>
        /// Indicates that the thumbnail failed to load.
        /// </summary>
        public event EventHandler<ThumbnailLoadFailedEventArgs> ThumbnailLoadFailed;

        /// <summary>
        /// Creates a new instance of ThumbnailView
        /// </summary>
        public ThumbnailView()
        {
            this.DefaultStyleKey = typeof(ThumbnailView);
        }

        /// <inheritdoc /> 
#if SILVERLIGHT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            var container = GetTemplateChild("container") as Panel;
            if (container == null) throw new NullReferenceException("Container Panel element required");
            for (int i = 0; i < imageElementCount; i++)
            {
                var imageElement = new Image();
                imageElement.Visibility = currentImageElementIndex == i ? Visibility.Visible : Visibility.Collapsed;
                container.Children.Add(imageElement);
                imageElements[i] = imageElement;
                imageElement.ImageOpened += imageElement_ImageOpened;
                imageElement.ImageFailed += imageElement_ImageFailed;
            }
            isTemplateApplied = true;

            OnThumbnailImageSourceChanged(ThumbnailImageSource);
        }
        
        void imageElement_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
#if SILVERLIGHT
            if (ThumbnailLoadFailed != null) ThumbnailLoadFailed(this, new ThumbnailLoadFailedEventArgs(e.ErrorException.Message));
#else
            if (ThumbnailLoadFailed != null) ThumbnailLoadFailed(this, new ThumbnailLoadFailedEventArgs(e.ErrorMessage));
#endif
        }

        void imageElement_ImageOpened(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < imageElementCount; i++)
            {
                var imageElement = imageElements[i];
                if (imageElement == sender)
                {
                    imageElement.Visibility = Visibility.Visible;
                    currentImageElementIndex = i;
                }
                else
                {
                    imageElement.Visibility = Visibility.Collapsed;
                    imageElement.Source = null;
                }
            }
        }

        /// <summary>
        /// ThumbnailImageSource DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty ThumbnailImageSourceProperty = DependencyProperty.Register("ThumbnailImageSource", typeof(ImageSource), typeof(ThumbnailView), new PropertyMetadata(null, (d, e) => ((ThumbnailView)d).OnThumbnailImageSourceChanged(e.NewValue as ImageSource)));

        void OnThumbnailImageSourceChanged(ImageSource value)
        {
            if (isTemplateApplied)
            {
                int i = (currentImageElementIndex + 1) % imageElementCount;
                var imageElement = imageElements[i];
                imageElement.Source = value;
            }
        }

        /// <summary>
        /// Gets or sets the image of the thumbnail.
        /// </summary>
        public ImageSource ThumbnailImageSource
        {
            get { return GetValue(ThumbnailImageSourceProperty) as ImageSource; }
            set { SetValue(ThumbnailImageSourceProperty, value); }
        }
    }

    /// <summary>
    /// EventArgs associated with a thumbnail failure.
    /// </summary>
    public sealed class ThumbnailLoadFailedEventArgs : EventArgs
    {
        internal ThumbnailLoadFailedEventArgs(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// The error message associated with the failure.
        /// </summary>
        public string ErrorMessage { get; private set; }
    }
}
