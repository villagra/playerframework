using System;
using System.Collections.Generic;

namespace Microsoft.Media.Advertising
{
    public sealed class Vmap
    {
        private string _Version = string.Empty;
        public string Version
        {
            get { return _Version; }
            set { _Version = value; }
        }

        private IList<VmapAdBreak> _AdBreaks;
        public IList<VmapAdBreak> AdBreaks
        {
            get { if (_AdBreaks == null) _AdBreaks = new List<VmapAdBreak>(); return _AdBreaks; }
            private set { _AdBreaks = value; }
        }

        private IList<VmapExtension> _Extensions;
        public IList<VmapExtension> Extensions
        {
            get { if (_Extensions == null) _Extensions = new List<VmapExtension>(); return _Extensions; }
            private set { _Extensions = value; }
        }
    }

    public sealed class VmapAdBreak
    {
        private string _TimeOffset = string.Empty;
        public string TimeOffset
        {
            get { return _TimeOffset; }
            set { _TimeOffset = value; }
        }

        private string _BreakType = string.Empty;
        public string BreakType
        {
            get { return _BreakType; }
            set { _BreakType = value; }
        }

        private string _BreakId = string.Empty;
        public string BreakId
        {
            get { return _BreakId; }
            set { _BreakId = value; }
        }

        private VmapAdSource _AdSource;
        public VmapAdSource AdSource
        {
            get { return _AdSource; }
            set { _AdSource = value; }
        }

        private IList<VmapTrackingEvent> _TrackingEvents;
        public IList<VmapTrackingEvent> TrackingEvents
        {
            get { if (_TrackingEvents == null) _TrackingEvents = new List<VmapTrackingEvent>(); return _TrackingEvents; }
            private set { _TrackingEvents = value; }
        }

        private IList<VmapExtension> _Extensions;
        public IList<VmapExtension> Extensions
        {
            get { if (_Extensions == null) _Extensions = new List<VmapExtension>(); return _Extensions; }
            private set { _Extensions = value; }
        }
    }

    public sealed class VmapAdSource
    {
        private string _Id = string.Empty;
        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        private bool _AllowMultipleAds;
        public bool AllowMultipleAds
        {
            get { return _AllowMultipleAds; }
            set { _AllowMultipleAds = value; }
        }

        private bool _FollowsRedirect;
        public bool FollowsRedirect
        {
            get { return _FollowsRedirect; }
            set { _FollowsRedirect = value; }
        }

        private string _VastData = string.Empty;
        public string VastData
        {
            get { return _VastData; }
            set { _VastData = value; }
        }

        private string _CustomAdData = string.Empty;
        public string CustomAdData
        {
            get { return _CustomAdData; }
            set { _CustomAdData = value; }
        }

        private string _CustomAdDataTemplateType = string.Empty;
        public string CustomAdDataTemplateType
        {
            get { return _CustomAdDataTemplateType; }
            set { _CustomAdDataTemplateType = value; }
        }

        private Uri _AdTag;
        public Uri AdTag
        {
            get { return _AdTag; }
            set { _AdTag = value; }
        }

        private string _AdTagTemplateType = string.Empty;
        public string AdTagTemplateType
        {
            get { return _AdTagTemplateType; }
            set { _AdTagTemplateType = value; }
        }
    }

    public sealed class VmapTrackingEvent
    {
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
        private string _Xml = string.Empty;
        public string Xml
        {
            get { return _Xml; }
            set { _Xml = value; }
        }

        private string _Type = string.Empty;
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
    }
}

