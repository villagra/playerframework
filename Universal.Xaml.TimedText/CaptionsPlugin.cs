using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.Media.TimedText;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
#endif

namespace Microsoft.PlayerFramework.TimedText
{
    /// <summary>
    /// A player framework plugin capable of displaying timed text captions.
    /// </summary>
    public class CaptionsPlugin : PluginBase
    {
        Panel captionsContainer;
        DispatcherTimer timer;
        Style timedTextCaptionsStyle;
        Style captionRegionStyle;
        TimedTextCaptions captionsPanel;

        /// <summary>
        /// Occurs when a caption region is reached.
        /// </summary>
        public event EventHandler<CaptionParsedEventArgs> CaptionParsed;

        /// <summary>
        /// Occurs when a caption track fails to parse.
        /// </summary>
        public event EventHandler<ParseFailedEventArgs> ParseFailed;

        /// <summary>
        /// Creates a new instance of the CaptionsPlugin
        /// </summary>
        public CaptionsPlugin()
        {
            PollingInterval = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// Gets or sets the amount of time to check the server for updated data. Only applies when MediaPlayer.IsLive = true
        /// </summary>
        public TimeSpan PollingInterval { get; set; }

        /// <summary>
        /// Gets whether the captions panel is visible. Returns true if any captions were found.
        /// </summary>
        public bool IsSourceLoaded { get; private set; }

        /// <summary>
        /// Gets or sets the style to be used for the TimedTextCaptions
        /// </summary>
        public Style TimedTextCaptionsStyle
        {
            get { return timedTextCaptionsStyle; }
            set
            {
                timedTextCaptionsStyle = value;
                if (captionsPanel != null)
                {
                    captionsPanel.Style = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the style to be used for each CaptionBlockRegion instance.
        /// </summary>
        public Style CaptionRegionStyle
        {
            get { return captionRegionStyle; }
            set
            {
                captionRegionStyle = value;
                if (captionsPanel != null)
                {
                    captionsPanel.CaptionBlockRegionStyle = value;
                }
            }
        }

        void MediaPlayer_SelectedCaptionChanged(object sender, RoutedPropertyChangedEventArgs<PlayerFramework.Caption> e)
        {
            if (e.OldValue != null)
            {
                e.OldValue.PayloadChanged -= caption_PayloadChanged;
                e.OldValue.PayloadAugmented -= caption_PayloadAugmented;
            }
            MediaPlayer.IsCaptionsActive = (e.NewValue as Caption != null);
            UpdateCaption(e.NewValue as Caption);
            if (e.NewValue != null)
            {
                e.NewValue.PayloadChanged += caption_PayloadChanged;
                e.NewValue.PayloadAugmented += caption_PayloadAugmented;
            }
        }

        void MediaPlayer_PositionChanged(object sender, RoutedPropertyChangedEventArgs<TimeSpan> e)
        {
            if (MediaPlayer.SelectedCaption != null)
            {
                captionsPanel.UpdateCaptions(MediaPlayer.Position);
            }
        }

        void MediaPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            captionsPanel.NaturalVideoSize = new Size(MediaPlayer.NaturalVideoWidth, MediaPlayer.NaturalVideoHeight);
        }

        void MediaPlayer_IsLiveChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (MediaPlayer.IsLive)
            {
                InitializeTimer();
            }
            else
            {
                ShutdownTimer();
            }
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = PollingInterval;
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void ShutdownTimer()
        {
            if (timer != null)
            {
                timer.Tick -= timer_Tick;
                if (timer.IsEnabled) timer.Stop();
                timer = null;
            }
        }

        void timer_Tick(object sender, object e)
        {
            var caption = MediaPlayer.SelectedCaption as Caption;
            RefreshCaption(caption, false);
        }

        /// <inheritdoc /> 
        protected override bool OnActivate()
        {
            var mediaContainer = MediaPlayer.Containers.OfType<Panel>().FirstOrDefault(c => c.Name == MediaPlayerTemplateParts.MediaContainer);
            captionsContainer = mediaContainer.Children.OfType<Panel>().FirstOrDefault(c => c.Name == MediaPlayerTemplateParts.CaptionsContainer);
            if (captionsContainer != null)
            {
                //PoC: to use a custom marker manager
                //captionsPanel = new TimedTextCaptions(new MarkerManager<CaptionRegion>(MediaPlayer), m => new MarkerManager<TimedTextElement>(MediaPlayer) { Markers = m });
                captionsPanel = new TimedTextCaptions();
                captionsPanel.CaptionParsed += captionsPanel_CaptionParsed;
                captionsPanel.ParseFailed += captionsPanel_ParseFailed;
                captionsPanel.NaturalVideoSize = new Size(MediaPlayer.NaturalVideoWidth, MediaPlayer.NaturalVideoHeight);
                if (TimedTextCaptionsStyle != null) captionsPanel.Style = TimedTextCaptionsStyle;
                if (CaptionRegionStyle != null) captionsPanel.CaptionBlockRegionStyle = CaptionRegionStyle;

                MediaPlayer.IsCaptionsActive = (MediaPlayer.SelectedCaption as Caption != null);
                captionsContainer.Children.Add(captionsPanel);
                UpdateCaption(MediaPlayer.SelectedCaption as Caption);

                MediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
                MediaPlayer.PositionChanged += MediaPlayer_PositionChanged;
                MediaPlayer.SelectedCaptionChanged += MediaPlayer_SelectedCaptionChanged;
                MediaPlayer.IsLiveChanged += MediaPlayer_IsLiveChanged;
                if (MediaPlayer.IsLive) InitializeTimer();

                return true;
            }
            return false;
        }

        /// <inheritdoc /> 
        protected override void OnDeactivate()
        {
            MediaPlayer.MediaOpened -= MediaPlayer_MediaOpened;
            MediaPlayer.PositionChanged -= MediaPlayer_PositionChanged;
            MediaPlayer.SelectedCaptionChanged -= MediaPlayer_SelectedCaptionChanged;
            MediaPlayer.IsLiveChanged -= MediaPlayer_IsLiveChanged;
            MediaPlayer.IsCaptionsActive = false;
            captionsContainer.Children.Remove(captionsPanel);
            captionsContainer = null;
            captionsPanel.Clear();
            captionsPanel.CaptionParsed -= captionsPanel_CaptionParsed;
            captionsPanel.ParseFailed -= captionsPanel_ParseFailed;
            captionsPanel = null;
            IsSourceLoaded = false;
        }

        /// <summary>
        /// Updates the current caption track.
        /// Will cause the caption source to download and get parsed, and will will start playing.
        /// </summary>
        /// <param name="caption">The caption track to use.</param>
        public void UpdateCaption(Caption caption)
        {
            captionsPanel.Clear();
            RefreshCaption(caption, true);
        }

        void captionsPanel_ParseFailed(object sender, ParseFailedEventArgs e)
        {
            if (ParseFailed != null) ParseFailed(this, e);
        }

        void captionsPanel_CaptionParsed(object sender, CaptionParsedEventArgs e)
        {
            if (CaptionParsed != null) CaptionParsed(this, e);
        }

        void caption_PayloadChanged(object sender, EventArgs e)
        {
            RefreshCaption(sender as Caption, false);
        }

        void caption_PayloadAugmented(object sender, PayloadAugmentedEventArgs e)
        {
            AugmentCaption(sender as Caption, e.Payload, e.StartTime, e.EndTime);
        }

        private async void AugmentCaption(Caption caption, object payload, TimeSpan startTime, TimeSpan endTime)
        {
            if (caption != null)
            {
                string result = null;
                if (payload is byte[])
                {
                    var byteArray = (byte[])payload;
#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
                    result = await TaskEx.Run(() => System.Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length));
#else
                    result = await Task.Run(() => System.Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length));
#endif
                    //result = System.Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                }
                else if (payload is string)
                {
                    result = (string)payload;
                }
                if (result != null)
                {
                    allTasks = EnqueueTask(() => captionsPanel.AugmentTtml(result, startTime, endTime), allTasks);
                    await allTasks;
                }
            }
        }

        private async void RefreshCaption(Caption caption, bool forceRefresh)
        {
            if (caption != null)
            {
                string result = null;
                if (caption.Payload is Uri)
                {
                    try
                    {
                        result = await ((Uri)caption.Payload).LoadToString();
                    }
                    catch
                    {
                        // TODO: expose event to log errors
                        return;
                    }
                }
                else if (caption.Payload is byte[])
                {
                    var byteArray = (byte[])caption.Payload;
#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
                    result = await TaskEx.Run(() => System.Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length));
#else
                    result = await Task.Run(() => System.Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length));
#endif
                    //result = System.Text.Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                }
                else if (caption.Payload is string)
                {
                    result = (string)caption.Payload;
                }

                if (result != null)
                {
                    allTasks = EnqueueTask(() => captionsPanel.ParseTtml(result, forceRefresh), allTasks);
                    await allTasks;
                    IsSourceLoaded = true;

                    // refresh the caption based on the current position. Fixes issue where caption is changed while paused.
                    if (IsLoaded) // make sure we didn't get unloaded by the time this completed.
                    {
                        captionsPanel.UpdateCaptions(MediaPlayer.Position);
                    }
                }
            }
        }

        Task allTasks;
        static async Task EnqueueTask(Func<Task> newTask, Task taskQueue)
        {
            if (taskQueue != null) await taskQueue;
            await newTask();
        }
    }
}
