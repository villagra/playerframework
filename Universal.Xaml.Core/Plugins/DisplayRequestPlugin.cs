using Windows.System.Display;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Media;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// A plugin used to help automatically manage the Windows 8 Display request (which prevents the screen from going dark after 2 minutes)
    /// </summary>
    public class DisplayRequestPlugin : IPlugin
    {
        DisplayRequest displayRequest;
        
        /// <summary>
        /// Gets if the display request is currently acitve
        /// </summary>
        public bool IsActive { get; private set; }

        bool isEnabled = true;
        /// <summary>
        /// Gets or sets if the plugin is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (isEnabled != value)
                {
                    if (isEnabled) Deactivate();
                    isEnabled = value;
                    if (isEnabled && (MediaPlayer.CurrentState == MediaElementState.Playing))
                    {
                        Activate();
                    }
                }
            }
        }

        DisplayRequest GetDisplayRequest()
        {
            if (displayRequest == null)
            {
                displayRequest = new DisplayRequest();
            }
            return displayRequest;
        }

        void MediaPlayer_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (isEnabled)
            {
                switch (MediaPlayer.CurrentState)
                {
                    case MediaElementState.Playing:
                        Activate();
                        break;
                    case MediaElementState.Paused:
                    case MediaElementState.Closed:
                    case MediaElementState.Stopped:
                        Deactivate();
                        break;
                }
            }
        }

        private void Deactivate()
        {
            if (IsActive)
            {
                GetDisplayRequest().RequestRelease();
                IsActive = false;
                displayRequest = null;
            }
        }

        private void Activate()
        {
            if (!IsActive)
            {
                GetDisplayRequest().RequestActive();
                IsActive = true;
            }
        }

        /// <inheritdoc /> 
        public void Load()
        {
            MediaPlayer.CurrentStateChanged += MediaPlayer_CurrentStateChanged;
        }

        /// <inheritdoc /> 
        public void Update(IMediaSource mediaSource)
        { }

        /// <inheritdoc /> 
        public void Unload()
        {
            MediaPlayer.CurrentStateChanged -= MediaPlayer_CurrentStateChanged;
            Deactivate();
            displayRequest = null;
        }

        /// <inheritdoc /> 
        public MediaPlayer MediaPlayer { get; set; }
    }
}
