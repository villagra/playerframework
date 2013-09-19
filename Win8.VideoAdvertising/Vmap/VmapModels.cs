using System;
using System.Collections.Generic;

namespace Microsoft.VideoAdvertising
{
    public sealed class Vmap
    {
        internal Vmap()
        {
            AdBreaks = new List<VmapAdBreak>();
            Extensions = new List<VmapExtension>();
        }

        public string Version { get; set; }
        public IList<VmapAdBreak> AdBreaks { get; private set; }
        public IList<VmapExtension> Extensions { get; private set; }
    }

    public sealed class VmapAdBreak
    {
        internal VmapAdBreak()
        {
            TrackingEvents = new List<VmapTrackingEvent>();
            Extensions = new List<VmapExtension>();
        }

        public string TimeOffset { get; set; }
        public string BreakType { get; set; }
        public string BreakId { get; set; }
        public VmapAdSource AdSource { get; set; }
        public IList<VmapTrackingEvent> TrackingEvents { get; private set; }
        public IList<VmapExtension> Extensions { get; private set; }
    }

    public sealed class VmapAdSource
    {
        internal VmapAdSource()
        {
            Id = string.Empty;
            VastData = string.Empty;
            CustomAdData = string.Empty;
            CustomAdDataTemplateType = string.Empty;
            AdTagTemplateType = string.Empty;
        }

        public string Id { get; set; }
        public bool AllowMultipleAds { get; set; }
        public bool FollowsRedirect { get; set; }
        public string VastData { get; set; }
        public string CustomAdData { get; set; }
        public string CustomAdDataTemplateType { get; set; }
        public Uri AdTag { get; set; }
        public string AdTagTemplateType { get; set; }
    }

    public sealed class VmapTrackingEvent
    {
        internal VmapTrackingEvent()
        { }

        public Uri TrackingUri { get; set; }
        public VmapTrackingEventType EventType { get; set; }
    }

    public enum VmapTrackingEventType
    {
        BreakStart,
        BreakEnd,
        Error
    }

    public sealed class VmapExtension
    {
        internal VmapExtension()
        { }

        public string Xml { get; set; }
        public string Type { get; set; }
    }
}

