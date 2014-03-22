using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
#if SILVERLIGHT
using System.Windows;
#else
using Windows.Foundation;
#endif

namespace Microsoft.Media.Advertising
{
    /// <summary>
    /// Represents a VAST non linear creative to be used by a VPAID plugin
    /// </summary>
    internal class NonLinearSource : IDocumentCreativeSource
    {
        public NonLinear NonLinear { get; private set; }
        public CreativeNonLinears NonLinears { get; private set; }

        ICreative IDocumentCreativeSource.Creative
        {
            get { return NonLinears; }
        }

        internal NonLinearSource(NonLinear nonLinear, CreativeNonLinears nonLinears)
        {
            NonLinear = nonLinear;
            NonLinears = nonLinears;
        }

        public IEnumerable<Icon> Icons
        {
            get
            {
                return Enumerable.Empty<Icon>();
            }
        }

        public string MediaSource
        {
            get
            {
                if (NonLinear.Item is IFrameResource)
                {
                    return ((IFrameResource)NonLinear.Item).Value.OriginalString;
                }
                else if (NonLinear.Item is StaticResource)
                {
                    return ((StaticResource)NonLinear.Item).Value.OriginalString;
                }
                else if (NonLinear.Item is HtmlResource)
                {
                    return ((HtmlResource)NonLinear.Item).Value;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        public string Id
        {
            get { return NonLinear.Id ?? string.Empty; }
        }

        public TimeSpan? Duration
        {
            get
            {
                return NonLinear.MinSuggestedDuration;
            }
        }

        public string MimeType
        {
            get
            {
                if (NonLinear.Item is StaticResource)
                {
                    return ((StaticResource)NonLinear.Item).CreativeType ?? string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string Codec
        {
            get
            {
                return string.Empty;
            }
        }

        public IEnumerable<TrackingEvent> TrackingEvents
        {
            get
            {
                return NonLinears.TrackingEvents;
            }
        }

        public IEnumerable<string> ClickTracking
        {
            get
            {
                return NonLinear.ClickTracking;
            }
        }

        public Uri ClickUrl
        {
            get { return NonLinear.ClickThrough; }
        }

        public Size Dimensions
        {
            get
            {
                return new Size(NonLinear.Width, NonLinear.Height);
            }
        }

        public Size ExpandedDimensions
        {
            get
            {
                return new Size(NonLinear.ExpandedWidth.GetValueOrDefault(0), NonLinear.ExpandedHeight.GetValueOrDefault(0));
            }
        }

        public CreativeSourceType Type
        {
            get { return CreativeSourceType.NonLinear; }
        }

        public MediaSourceEnum MediaSourceType
        {
            get
            {
                if (NonLinear.Item is IFrameResource)
                {
                    return MediaSourceEnum.IFrame;
                }
                else if (NonLinear.Item is StaticResource)
                {
                    return MediaSourceEnum.Static;
                }
                else if (NonLinear.Item is HtmlResource)
                {
                    return MediaSourceEnum.HTML;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        public string ExtraInfo
        {
            get
            {
                return NonLinear.AdParameters ?? string.Empty;
            }
        }

        public bool IsScalable
        {
            get { return NonLinear.Scalable.GetValueOrDefault(false); }
        }

        public bool MaintainAspectRatio
        {
            get { return NonLinear.MaintainAspectRatio.GetValueOrDefault(true); }
        }

        public bool IsStreaming
        {
            get { return false; }
        }

        public FlexibleOffset SkippableOffset
        {
            get { return null; }
        }
        
        public string ApiFramework
        {
            get { return string.Empty; }
        }
    }
}
