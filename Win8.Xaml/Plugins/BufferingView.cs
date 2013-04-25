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
    /// A control that indicates buffering is occuring.
    /// </summary>
    public class BufferingView : Control
    {
        /// <summary>
        /// Creates a new instance of the control
        /// </summary>
        public BufferingView()
        {
            this.DefaultStyleKey = typeof(BufferingView);
        }

        /// <inheritdoc /> 
#if SILVERLIGHT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            UpdateVisualStates();
        }

        /// <summary>
        /// Identifies the MediaPlayer dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register("ViewModel", typeof(IInteractiveViewModel), typeof(BufferingView), new PropertyMetadata(null, (d, e) => ((BufferingView)d).OnViewModelChanged(e.OldValue as IInteractiveViewModel, e.NewValue as IInteractiveViewModel)));

        void OnViewModelChanged(IInteractiveViewModel oldValue, IInteractiveViewModel newValue)
        {
            if (oldValue != null)
            {
                oldValue.CurrentStateChanged -= CurrentStateChanged;
            }

            UpdateVisualStates();

            if (newValue != null)
            {
                newValue.CurrentStateChanged += CurrentStateChanged;
            }
        }

        /// <summary>
        /// The InteractiveMediaPlayer object used to provide state updates and serve user interaction requests.
        /// This is usually an instance of the MediaPlayer but could be a custom implementation to support unique interaction such as in the case of advertising.
        /// </summary>
        public IInteractiveViewModel ViewModel
        {
            get { return GetValue(ViewModelProperty) as IInteractiveViewModel; }
            set { SetValue(ViewModelProperty, value); }
        }

        void CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            UpdateVisualStates();
        }

        private void UpdateVisualStates()
        {
            if (ViewModel == null)
            {
                VisualStateManager.GoToState(this, MediaElementState.Closed.ToString(), true);
            }
            else
            {
                VisualStateManager.GoToState(this, ViewModel.CurrentState.ToString(), true);
            }
        }
    }
}
