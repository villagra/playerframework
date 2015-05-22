using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.ComponentModel;
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
#else
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.UI.Xaml;
using System.Threading.Tasks;
#endif

namespace Microsoft.Media.TimedText
{
    public class TimedTextCaptions : Control
    {
        protected Panel CaptionsPresenterElement { get; private set; }

        const long DefaultMaximumCaptionSeekSearchWindowMillis = 60000; //1 minutes
        readonly Dictionary<CaptionRegion, CaptionBlockRegion> regions = new Dictionary<CaptionRegion, CaptionBlockRegion>();
        Size lastSize;
        TimeSpan? lastPosition;
        bool isTemplateApplied;

        /// <summary>
        /// Occurs when a caption region is reached.
        /// </summary>
        public event EventHandler<ParseFailedEventArgs> ParseFailed;

        /// <summary>
        /// Occurs when a caption region is reached.
        /// </summary>
        public event EventHandler<CaptionParsedEventArgs> CaptionParsed;

        /// <summary>
        /// Occurs when a caption region is reached.
        /// </summary>
        public event EventHandler<CaptionRegionEventArgs> CaptionReached;

        /// <summary>
        /// Occurs when a caption region is left.
        /// </summary>
        public event EventHandler<CaptionRegionEventArgs> CaptionLeft;

        readonly CaptionMarkerFactory factory = new CaptionMarkerFactory();
        readonly Func<MediaMarkerCollection<TimedTextElement>, IMarkerManager<TimedTextElement>> regionManagerFactory;
        readonly IMarkerManager<CaptionRegion> captionManager;

        public TimedTextCaptions(IMarkerManager<CaptionRegion> CaptionManager = null, Func<MediaMarkerCollection<TimedTextElement>, IMarkerManager<TimedTextElement>> RegionManagerFactory = null)
        {
            DefaultStyleKey = typeof(TimedTextCaptions);

            factory.NewMarkers += NewMarkers;
            factory.MarkersRemoved += MarkersRemoved;

            this.SizeChanged += this_SizeChanged;

            captionManager = CaptionManager ?? new MediaMarkerManager<CaptionRegion>();
            regionManagerFactory = RegionManagerFactory ?? (m => new MediaMarkerManager<TimedTextElement>() { Markers = m });

            Captions = new MediaMarkerCollection<CaptionRegion>();

            captionManager.MarkerLeft += captionManager_MarkerLeft;
            captionManager.MarkerReached += captionManager_MarkerReached;
        }
                
#if SILVERLIGHT
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            CaptionsPresenterElement = this.GetTemplateChild("CaptionsPresenterElement") as Panel;
            isTemplateApplied = true;
            if (lastPosition.HasValue)
            {
                UpdateCaptions(lastPosition.Value);
            }
        }

        public Style CaptionBlockRegionStyle { get; set; }

        public async Task AugmentTtml(string ttml, TimeSpan startTime, TimeSpan endTime)
        {
            // parse on a background thread
#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
            var markers = await TaskEx.Run(() => factory.ParseTtml(ttml, startTime, endTime));       
#else
            var markers = await Task.Run(() => factory.ParseTtml(ttml, startTime, endTime));
#endif
            if (CaptionParsed != null)
            {
                foreach (var marker in markers)
                {
                    CaptionParsed(this, new CaptionParsedEventArgs(marker));
                }
            }

            factory.MergeMarkers(markers);
        }

        public async Task ParseTtml(string ttml, bool forceRefresh)
        {
            try
            {
                // parse on a background thread
#if SILVERLIGHT && !WINDOWS_PHONE || WINDOWS_PHONE7
                var markers = await TaskEx.Run(() => factory.ParseTtml(ttml, TimeSpan.Zero, TimeSpan.MaxValue));       
#else
                var markers = await Task.Run(() => factory.ParseTtml(ttml, TimeSpan.Zero, TimeSpan.MaxValue));
#endif
                if (CaptionParsed != null)
                {
                    foreach (var marker in markers)
                    {
                        CaptionParsed(this, new CaptionParsedEventArgs(marker));
                    }
                }
            
                factory.UpdateMarkers(markers, forceRefresh);
            }
            catch (Exception ex)
            {
                if (ParseFailed != null) ParseFailed(this, new ParseFailedEventArgs(ex));
            }
        }

        void captionManager_MarkerReached(IMarkerManager<CaptionRegion> markerManager, CaptionRegion region)
        {
            OnCaptionRegionReached(region);
            if (!regions.ContainsKey(region))
            {
#if HACK_XAMLTYPEINFO
                var children = region.Children as MediaMarkerCollection<TimedTextElement>;
#else
                var children = region.Children;
#endif
                var regionBlock = new CaptionBlockRegion();
                if (CaptionBlockRegionStyle != null) regionBlock.Style = CaptionBlockRegionStyle;
                regionBlock.CaptionRegion = region;
                regionBlock.CaptionManager = regionManagerFactory(children);
                regions.Add(region, regionBlock);
                CaptionsPresenterElement.Children.Add(regionBlock);
                regionBlock.ApplyTemplate();
                VisibleCaptions.Add(region);
            }
        }

        void captionManager_MarkerLeft(IMarkerManager<CaptionRegion> markerManager, CaptionRegion region)
        {
            if (regions.ContainsKey(region))
            {
                var presenter = regions[region];
                CaptionsPresenterElement.Children.Remove(presenter);
                VisibleCaptions.Remove(region);
                regions.Remove(region);
                presenter.CaptionManager = null;
            }
            OnCaptionRegionLeft(region);
        }

        #region VisibleCaptions
        /// <summary>
        /// VisibleCaptions DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty VisibleCaptionsProperty = DependencyProperty.Register("VisibleCaptions", typeof(MediaMarkerCollection<CaptionRegion>), typeof(TimedTextCaptions), new PropertyMetadata(new MediaMarkerCollection<CaptionRegion>()));

        /// <summary>
        /// Gets the current caption markers.
        /// </summary>
        public MediaMarkerCollection<CaptionRegion> VisibleCaptions
        {
            get { return (MediaMarkerCollection<CaptionRegion>)GetValue(VisibleCaptionsProperty); }
            private set { SetValue(VisibleCaptionsProperty, value); }
        }

        #endregion

        public void UpdateCaptions(TimeSpan position)
        {
            lastPosition = position;

            if (isTemplateApplied)
            {
                captionManager.CheckMarkerPositions(position);

                foreach (var region in regions)
                {
                    var regionBlock = (CaptionBlockRegion)region.Value;
                    regionBlock.UpdateAnimations(position);
                    regionBlock.CaptionManager.CheckMarkerPositions(position);
                }
            }
        }

        #region Captions
        /// <summary>
        /// Captions DependencyProperty definition.
        /// </summary>
        public static readonly DependencyProperty CaptionsProperty = DependencyProperty.Register("Captions", typeof(MediaMarkerCollection<CaptionRegion>), typeof(TimedTextCaptions), new PropertyMetadata(null, OnCaptionsPropertyChanged));

        /// <summary>
        /// Gets the current caption markers.
        /// </summary>
        public MediaMarkerCollection<CaptionRegion> Captions
        {
            get { return (MediaMarkerCollection<CaptionRegion>)GetValue(CaptionsProperty); }
            set { SetValue(CaptionsProperty, value); }
        }

        private static void OnCaptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var captionHost = d as TimedTextCaptions;
            var oldValue = args.OldValue as MediaMarkerCollection<CaptionRegion>;
            var newValue = args.NewValue as MediaMarkerCollection<CaptionRegion>;

            if (captionHost.captionManager != null)
            {
                captionHost.captionManager.Markers = newValue;
            }
        }

        #endregion

        /// <summary>
        /// Raises the CaptionLeft event.
        /// </summary>
        protected virtual void OnCaptionRegionLeft(CaptionRegion region)
        {
            CaptionLeft.IfNotNull(i => i(this, new CaptionRegionEventArgs(region)));
        }

        /// <summary>
        /// Raises the CaptionReached event.
        /// </summary>
        protected virtual void OnCaptionRegionReached(CaptionRegion region)
        {
            CaptionReached.IfNotNull(i => i(this, new CaptionRegionEventArgs(region)));
        }

        void NewMarkers(IEnumerable<MediaMarker> newMarkers)
        {
            foreach (MediaMarker marker in newMarkers)
            {
                Captions.Add(marker as CaptionRegion);
            }
        }

        void MarkersRemoved(IEnumerable<MediaMarker> removedMarkers)
        {
            foreach (MediaMarker marker in removedMarkers)
            {
                Captions.Remove(marker as CaptionRegion);
            }
        }

        public void Clear()
        {
            factory.Clear();
            Captions.Clear();
            captionManager.Clear();
            if (CaptionsPresenterElement != null)
            {
                CaptionsPresenterElement.Children.Clear();
            }
        }

        void this_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ConfigureCaptionPresenterSize(e.NewSize);
        }

        Size naturalVideoSize;
        public Size NaturalVideoSize
        {
            get { return naturalVideoSize; }
            set
            {
                naturalVideoSize = value;
                ConfigureCaptionPresenterSize(lastSize);
            }
        }

        protected virtual void ConfigureCaptionPresenterSize(Size newSize)
        {
            lastSize = newSize;
            if (CaptionsPresenterElement != null)
            {
                var aspectRatio = NaturalVideoSize.Width / NaturalVideoSize.Height;
                var aspectPresentationWidth = newSize.Height * aspectRatio;

                if (aspectPresentationWidth > newSize.Width)
                {
                    //Video will have black bars on top and bottom
                    CaptionsPresenterElement.Width = newSize.Width;
                    CaptionsPresenterElement.Height = newSize.Width / aspectRatio;
                }
                else if (aspectPresentationWidth < newSize.Width)
                {
                    //Video will have black bars on the sides
                    CaptionsPresenterElement.Height = newSize.Height;
                    CaptionsPresenterElement.Width = newSize.Height * aspectRatio;
                }
                else
                {
                    CaptionsPresenterElement.Width = newSize.Width;
                    CaptionsPresenterElement.Height = newSize.Height;
                }
            }
        }
    }

    public sealed class CaptionRegionEventArgs : EventArgs
    {
        public CaptionRegionEventArgs(CaptionRegion captionRegion)
        {
            CaptionRegion = captionRegion;
        }

        public CaptionRegion CaptionRegion { get; private set; }
    }

    public sealed class CaptionParsedEventArgs : EventArgs
    {
        public CaptionParsedEventArgs(MediaMarker captionMarker)
        {
            CaptionMarker = captionMarker;
        }

        public MediaMarker CaptionMarker { get; private set; }
    }

    public sealed class ParseFailedEventArgs : EventArgs
    {
        public ParseFailedEventArgs(Exception error)
        {
            Error = error;
        }

        public Exception Error { get; private set; }
    }
}
