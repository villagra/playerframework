using System;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace Microsoft.PlayerFramework
{
    /// <summary>
    /// Provides a base class to make it easy to implement a plugin
    /// </summary>
    public abstract class PluginBase :
#if SILVERLIGHT
        DependencyObject
#else
        FrameworkElement
#endif
, IPlugin
    {
        /// <summary>
        /// Gets the current IMediaSource implemenation being used by the player framework.
        /// </summary>
        protected IMediaSource CurrentMediaSource { get; private set; }

        /// <summary>
        /// Gets whether or not the plugin is loaded.
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Gets whether or not the plugin is activate.
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
                    isEnabled = value;
                    if (IsLoaded)
                    {
                        if (isEnabled)
                        {
                            Activate();
                        }
                        else
                        {
                            Deactivate();
                        }
                    }
                }
            }
        }

        /// <inheritdoc /> 
        public MediaPlayer MediaPlayer { get; set; }

        void IPlugin.Load()
        {
            OnLoad();
            IsLoaded = true;
            CurrentMediaSource = MediaPlayer;
            if (IsEnabled)
            {
                Activate();
            }
        }

        /// <summary>
        /// Can be overridden in order to get notification of the plugin being loaded.
        /// </summary>
        protected virtual void OnLoad()
        { }

        void IPlugin.Update(IMediaSource mediaSource)
        {
            CurrentMediaSource = mediaSource;
            OnUpdate();
            IsEnabled = mediaSource == null ? false : GetIsEnabled((DependencyObject)mediaSource);
        }

        /// <summary>
        /// Can be overridden to get notification of the plugin being updated.
        /// </summary>
        protected virtual void OnUpdate()
        { }

        void IPlugin.Unload()
        {
            Deactivate();
            CurrentMediaSource = null;
            OnUnload();
            IsLoaded = false;
        }

        /// <summary>
        /// Can be overridden in order to get notification of the plugin being unloaded.
        /// </summary>
        protected virtual void OnUnload()
        { }

        void Activate()
        {
            if (!IsActive)
            {
                IsActive = OnActivate();
            }
        }

        void Deactivate()
        {
            if (IsActive)
            {
                OnDeactivate();
                IsActive = false;
            }
        }

        /// <summary>
        /// Activates the plugin at the appropriate time.
        /// </summary>
        /// <returns>A boolean indicating whether or not it was successfully activated.</returns>
        protected abstract bool OnActivate();

        /// <summary>
        /// Deactivates the plugin at the appropriate time.
        /// </summary>
        protected abstract void OnDeactivate();

        /// <summary>
        /// Identifies the IsEnabled attached property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(PluginBase), new PropertyMetadata(true));

        /// <summary>
        /// Sets the IsEnabled attached property value.
        /// </summary>
        /// <param name="obj">An instance of the MediaPlayer or PlaylistItem.</param>
        /// <param name="propertyValue">A value indicating if the plugin should be enabled.</param>
        public static void SetIsEnabled(DependencyObject obj, bool propertyValue)
        {
            obj.SetValue(IsEnabledProperty, propertyValue);
        }

        /// <summary>
        /// Gets the IsEnabled attached property value.
        /// </summary>
        /// <param name="obj">An instance of the MediaPlayer or PlaylistItem.</param>
        /// <returns>A value indicating if the plugin should be enabled.</returns>
        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }
    }
}
