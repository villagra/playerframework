using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.Foundation;
#endif

namespace Microsoft.VideoAdvertising
{
    /// <summary>
    /// Represents a VAST linear creative to be used by a VPAID plugin
    /// </summary>
    internal class LinearSource : IDocumentCreativeSource
    {
        public CreativeLinear Linear { get; private set; }
        public MediaFile Media { get; private set; }

        ICreative IDocumentCreativeSource.Creative
        {
            get { return Linear; }
        }

        internal LinearSource(CreativeLinear linear, MediaFile media)
        {
            Linear = linear;
            Media = media;
        }

        public IEnumerable<Icon> Icons
        {
            get
            {
                return Linear.Icons;
            }
        }

        public string MediaSource
        {
            get { return Media.Value.OriginalString; }
        }

        public string Id
        {
            get { return (Media != null ? Media.Id : null) ?? string.Empty; }
        }

        public IEnumerable<TrackingEvent> TrackingEvents
        {
            get
            {
                return Linear.TrackingEvents;
            }
        }

        public IEnumerable<string> ClickTracking
        {
            get
            {
                return Linear.ClickTracking;
            }
        }

        public TimeSpan? Duration
        {
            get
            {
                return Linear.Duration;
            }
        }

        public string MimeType
        {
            get { return Media.Type ?? string.Empty; }
        }

        public string Codec
        {
            get { return Media.Codec ?? string.Empty; }
        }

        public Uri ClickUrl
        {
            get { return Linear.ClickThrough; }
        }

        public Size Dimensions
        {
            get
            {
                return new Size(Media.Width, Media.Height);
            }
        }

        public Size ExpandedDimensions
        {
            get { return Size.Empty; }
        }

        public CreativeSourceType Type
        {
            get { return CreativeSourceType.Linear; }
        }

        public MediaSourceEnum MediaSourceType
        {
            get
            {
                return MediaSourceEnum.Static;
            }
        }

        public string ExtraInfo
        {
            get
            {
                return Linear.AdParameters ?? string.Empty;
            }
        }

        public bool IsScalable
        {
            get
            {
                return Media.Scalable.GetValueOrDefault(true);
            }
        }

        public bool MaintainAspectRatio
        {
            get
            {
                return Media.MaintainAspectRatio.GetValueOrDefault(true);
            }
        }

        public bool IsStreaming
        {
            get { return Media.Delivery == MediaFileDelivery.Streaming; }
        }

        public FlexibleOffset SkippableOffset
        {
            get
            {
                return Linear.SkipOffset;
            }
        }

        public string ApiFramework
        {
            get { return Media.ApiFramework; }
        }
    }

}
