using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Threading;
using Microsoft.Web.Media.SmoothStreaming;
using System.Diagnostics;

namespace Microsoft.Media.AdaptiveStreaming.Helper
{
    internal class StreamSelectionManager: QueuedRetryManager
    {
        public event Action<StreamSelectionManager, SegmentInfo, IEnumerable<StreamInfo>> RetryingStreamSelection, StreamSelectionExceededMaximumRetries;
        public event Action<StreamSelectionManager, SegmentInfo, IEnumerable<StreamInfo>, StreamUpdatedListEventArgs> StreamSelectionCompleted;

        private readonly ManifestInfo _manifestInfo;

        public StreamSelectionManager(ManifestInfo ManifestInfo)
        {
            MaximumConcurrentRequests = 1;
            Timeout = TimeSpan.FromSeconds(15);

            _manifestInfo = ManifestInfo;
            _manifestInfo.SelectStreamsCompleted += ManifestInfo_SelectStreamsCompleted;
        }

        public void EnqueueRequest(SegmentInfo segment, IEnumerable<StreamInfo> streams)
        {
            var request = new StreamSelectionRequest(segment, streams);
            AddRequest(request);
        }

        public void EnqueueRequest(SegmentInfo segment, IEnumerable<StreamInfo> streamsToAdd, IEnumerable<StreamInfo> streamsToRemove)
        {
            AddRequest(new StreamModifyRequest(segment, streamsToAdd, streamsToRemove));
        }

        protected override void BeginRequest(RetryQueueRequest request)
        {
            //Adding an intentional delay here to work around a known timing issue
            //with the SSME.  If this is called too soon after initialization the
            //request will be aborted with no indication raised from the SSME.
            //TODO: Remove this workaround once the SSME has been fixed.
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(250);
            EventHandler tickHandler = null;
            tickHandler = (s, e) =>
                {
                    SegmentInfo segment = null;
                    List<StreamInfo> streams = null;
                    var streamSelectionRequest = request as StreamSelectionRequest;
                    if (streamSelectionRequest != null)
                    {
                        streams = streamSelectionRequest.Streams.ToList();
                        segment = streamSelectionRequest.Segment;
                    }
                    else
                    {
                        var streamModifyRequest = request as StreamModifyRequest;
                        if (streamModifyRequest != null)
                        {
                            streams = streamModifyRequest.Streams.ToList();
                            segment = streamModifyRequest.Segment;
                        }
                    }                    
                    segment.SelectStreamsAsync(streams, request);
                    timer.Tick -= tickHandler;
                    timer.Stop();
                };
            timer.Tick += tickHandler;
            timer.Start();
        }

        private void ManifestInfo_SelectStreamsCompleted(object sender, StreamUpdatedListEventArgs e)
        {
            var request = e.UserState as IStreamSelectionRequest;

            if (request != null)
            {
                NotifyRequestSuccessful(request as RetryQueueRequest);
                StreamSelectionCompleted(this, request.Segment, request.Streams, e);
            }
        }


        protected override void OnRequestExceededMaximumRetryAttempts(RetryQueueRequest request)
        {
            base.OnRequestExceededMaximumRetryAttempts(request);

            var streamSelectionRequest = request as IStreamSelectionRequest;
            if (StreamSelectionExceededMaximumRetries != null && streamSelectionRequest != null)
            {
                StreamSelectionExceededMaximumRetries(this, streamSelectionRequest.Segment, streamSelectionRequest.Streams);
            }
        }

        protected override void OnRetryingRequest(RetryQueueRequest request)
        {
            base.OnRetryingRequest(request);

            var streamSelectionRequest = request as IStreamSelectionRequest;
            if (RetryingStreamSelection != null && streamSelectionRequest != null)
            {
                RetryingStreamSelection(this, streamSelectionRequest.Segment, streamSelectionRequest.Streams);
            }
        }

        public override void Dispose()
        {
            _manifestInfo.SelectStreamsCompleted -= ManifestInfo_SelectStreamsCompleted;
            base.Dispose();
        }
    }

    interface IStreamSelectionRequest
    {
        SegmentInfo Segment { get; }
        IEnumerable<StreamInfo> Streams { get; }
    }

    internal class StreamModifyRequest : RetryQueueRequest, IStreamSelectionRequest
    {
        public SegmentInfo Segment { get; private set; }
        public IEnumerable<StreamInfo> StreamsToAdd { get; private set; }
        public IEnumerable<StreamInfo> StreamsToRemove { get; private set; }

        public StreamModifyRequest(SegmentInfo segment, IEnumerable<StreamInfo> StreamInfosToAdd, IEnumerable<StreamInfo> StreamInfosToRemove)
        {
            Segment = segment;
            StreamsToAdd = StreamInfosToAdd;
            StreamsToRemove = StreamInfosToRemove;
        }

        public IEnumerable<StreamInfo> Streams
        {
            get
            {
                List<StreamInfo> ret;
                ret = new List<StreamInfo>();
                foreach (var s in Segment.SelectedStreams)
                {
                    ret.Add(s as StreamInfo);
                }
                if (StreamsToAdd != null)
                {
                    foreach (var s in StreamsToAdd)
                    {
                        if (ret.Contains(s) == false) ret.Add(s);
                    }
                }
                if (StreamsToRemove != null)
                {
                    foreach (var stream in StreamsToRemove)
                    {
                        if (ret.Contains(stream))
                        {
                            ret.Remove(stream);
                        }
                    }
                }
                return ret;
            }
        }
    }

    internal class StreamSelectionRequest : RetryQueueRequest, IStreamSelectionRequest
    {
        public SegmentInfo Segment { get; private set; }
        public IEnumerable<StreamInfo> Streams { get; private set; }

        public StreamSelectionRequest(SegmentInfo segment, IEnumerable<StreamInfo> StreamInfos)
        {
            Segment = segment;
            Streams = StreamInfos;
        }
    }
}
