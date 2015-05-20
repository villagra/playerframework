using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Documents;

namespace Microsoft.PlayerFramework.CaptionMarkers
{
    /// <summary>
    /// Represents a plugin for the player framework that can show closed captions
    /// </summary>
    public partial class CaptionsPlugin : PluginBase
    {
        const string DefaultMarkerType = "caption";

        CaptionsPanel captionsPanel;
        Panel captionsContainer;
        CancellationTokenSource cts;

        /// <summary>
        /// Creates a new instance of CaptionsPanel
        /// </summary>
        public CaptionsPlugin()
        {
            CaptionDuration = TimeSpan.FromSeconds(2);
            MarkerType = DefaultMarkerType;
        }

        /// <inheritdoc /> 
        protected override void OnUpdate()
        {
            ActiveCaptions.Clear();
            base.OnUpdate();
        }

        /// <inheritdoc /> 
        protected override void OnUnload()
        {
            ActiveCaptions = null;
            base.OnUnload();
        }

        /// <summary>
        /// Gets or sets whether or not captions are enabled
        /// </summary>
        public string MarkerType { get; set; }

        /// <summary>
        /// Gets or sets whether or not captions are enabled
        /// </summary>
        public TimeSpan CaptionDuration { get; set; }

        /// <summary>
        /// Gets or sets the style to be used for the CaptionsPanel
        /// </summary>
        public Style CaptionsPanelStyle { get; set; }

        /// <summary>
        /// Gets the list of active captions
        /// </summary>
        protected ObservableCollection<ActiveCaption> ActiveCaptions { get; private set; }

        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            var mediaContainer = MediaPlayer.Containers.OfType<Panel>().FirstOrDefault(c => c.Name == MediaPlayerTemplateParts.MediaContainer);
            captionsContainer = mediaContainer.Children.OfType<Panel>().FirstOrDefault(c => c.Name == MediaPlayerTemplateParts.CaptionsContainer);
            if (captionsContainer != null)
            {
                if (!MediaPlayer.AvailableCaptions.Any()) // if there are caption tracks specified, this means there is another plugin that will handle this work. Do nothing.
                {
                    captionsPanel = new CaptionsPanel();
                    captionsPanel.Style = CaptionsPanelStyle;

					ActiveCaptions = new ObservableCollection<ActiveCaption>();
                    captionsPanel.ActiveCaptions = ActiveCaptions;
                    captionsContainer.Children.Add(captionsPanel);
					cts = new CancellationTokenSource();
                    MediaPlayer.MarkerReached += MediaPlayer_MarkerReached;
                    MediaPlayer.CaptionsInvoked += MediaPlayer_CaptionsInvoked;
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc /> 
        protected override void OnDeactivate()
        {
            MediaPlayer.MarkerReached -= MediaPlayer_MarkerReached;
            MediaPlayer.CaptionsInvoked -= MediaPlayer_CaptionsInvoked;
            cts.Cancel();
            cts = null;
            captionsContainer.Children.Remove(captionsPanel);
            captionsContainer = null;
            captionsPanel.ActiveCaptions = null;
            captionsPanel = null;
            ActiveCaptions = null;
        }

        void MediaPlayer_CaptionsInvoked(object sender, RoutedEventArgs e)
		{
			captionsPanel.Style = CaptionsPanelStyle;
			MediaPlayer.IsCaptionsActive = !MediaPlayer.IsCaptionsActive;
        }

        async void MediaPlayer_MarkerReached(object sender, TimelineMarkerRoutedEventArgs e)
        {
            if (MediaPlayer.IsCaptionsActive)
            {
                if (MarkerType == null || e.Marker.Type == MarkerType)
                {
                    var activeCaption = new ActiveCaption() { Text = e.Marker.Text };
                    ActiveCaptions.Add(activeCaption);
                    try
                    {
                        await Task.Delay(CaptionDuration, cts.Token);
                    }
                    catch (OperationCanceledException) { /* ignore */ }
                    finally
                    {
                        if (ActiveCaptions != null)  // we could have been unloaded while we waited.
                        {
                            ActiveCaptions.Remove(activeCaption);
                        }
                    }
                }
            }
        }
    }
}
