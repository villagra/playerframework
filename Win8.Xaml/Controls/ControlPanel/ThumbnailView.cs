using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Microsoft.PlayerFramework
{
    public sealed class ThumbnailView : Control
    {
        int currentImageElementIndex = 0;
        const int imageElementCount = 2;
        readonly Image[] imageElements = new Image[imageElementCount];
        bool isTemplateApplied = false;

        public event EventHandler<ThumbnailLoadFailedEventArgs> ThumbnailLoadFailed;

        public ThumbnailView()
        {
            this.DefaultStyleKey = typeof(ThumbnailView);
        }

        protected override void OnApplyTemplate()
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
            if (ThumbnailLoadFailed != null) ThumbnailLoadFailed(this, new ThumbnailLoadFailedEventArgs(e.ErrorMessage));
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
        /// Gets or sets the pattern of the Uri to use.
        /// </summary>
        public ImageSource ThumbnailImageSource
        {
            get { return GetValue(ThumbnailImageSourceProperty) as ImageSource; }
            set { SetValue(ThumbnailImageSourceProperty, value); }
        }
    }

    public sealed class ThumbnailLoadFailedEventArgs : EventArgs
    {
        internal ThumbnailLoadFailedEventArgs(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; private set; }
    }
}
