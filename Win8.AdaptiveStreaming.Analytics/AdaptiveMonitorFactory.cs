using Microsoft.VideoAnalytics;
using System;

namespace Microsoft.AdaptiveStreaming.Analytics
{
    public sealed class AdaptiveMonitorFactory : IDisposable
    {
        AdaptiveMonitor adaptiveMonitor;
        public AdaptiveStreamingManager Manager { get; private set; }

        public AdaptiveMonitorFactory(AdaptiveStreamingManager manager)
        {
            Manager = manager;
            adaptiveMonitor = new AdaptiveMonitor();
            manager.OpenedBackground += Manager_Opened;
            manager.ClosedBackground += Manager_Closed;
            manager.RefreshingState += Manager_RefreshingState;
        }
        
        void Manager_RefreshingState(object sender, RefreshingStateEventArgs e)
        {
            adaptiveMonitor.UpdatePosition(e.Position);
        }
        
        void Manager_Opened(object sender, object e)
        {
            adaptiveMonitor.Source = Manager.ActiveAdaptiveSource;
        }

        void Manager_Closed(object sender, object e)
        {
            adaptiveMonitor.Source = null;
        }

        public IAdaptiveMonitor AdaptiveMonitor
        {
            get { return adaptiveMonitor; }
        }

        public void Dispose()
        {
            Manager.OpenedBackground -= Manager_Opened;
            Manager.ClosedBackground -= Manager_Closed;
            Manager.RefreshingState -= Manager_RefreshingState;
            Manager = null;
            adaptiveMonitor = null;
        }
    }
}
