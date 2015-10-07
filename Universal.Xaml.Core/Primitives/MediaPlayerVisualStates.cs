
namespace Microsoft.PlayerFramework
{
    internal static class MediaPlayerVisualStates
    {
        internal static class GroupNames
        {
            internal const string InteractiveStates = "InteractiveStates";
            internal const string MediaStates = "MediaStates";
            internal const string PlayerStates = "PlayerStates";
            internal const string CaptionsStates = "CaptionsStates";
            internal const string FullScreenStates = "FullScreenStates";
            internal const string AdvertisingStates = "AdvertisingStates";
            internal const string PlayToStates = "PlayToStates";
            internal const string MediaTypeStates = "MediaTypeStates";
#if WINDOWS_UWP
            internal const string CastingStates = "CastingStates";
#endif
        }

        internal static class InteractiveStates
        {
            internal const string StartInteracting = "StartInteracting";
            internal const string StopInteracting = "StopInteracting";
            internal const string Visible = "Visible";
            internal const string Hidden = "Hidden";
        }

        internal static class MediaStates
        {
            internal const string Playing = "Playing";
            internal const string Closed = "Closed";
            internal const string Opening = "Opening";
            internal const string Paused = "Paused";
            internal const string Buffering = "Buffering";
        }

        internal static class PlayerStates
        {
            internal const string Unloaded = "Unloaded";
            internal const string Pending = "Pending";
            internal const string Loading = "Loading";
            internal const string Loaded = "Loaded";
            internal const string Opened = "Opened";
            internal const string Starting = "Starting";
            internal const string Started = "Started";
            internal const string Ending = "Ending";
            internal const string Failed = "Failed";
        }

        internal static class CaptionsStates
        {
            internal const string CapitonsActive = "CaptionsActive";
            internal const string CapitonsInactive = "CaptionsInactive";
        }

        internal static class FullScreenStates
        {
            internal const string FullScreen = "FullScreen";
            internal const string NotFullScreen = "NotFullScreen";
        }

        internal static class AdvertisingStates
        {
            internal const string LoadingAd = "LoadingAd";
            internal const string NoAd = "NoAd";
            internal const string LinearAd = "LinearAd";
            internal const string NonLinearAd = "NonLinearAd";
        }
   
#if !SILVERLIGHT
        internal static class PlayToStates
        {
            internal const string Connected = "PlayToConnected";
            internal const string Rendering = "PlayToRendering";
            internal const string Disconnected = "PlayToDisconnected";
        }

        internal static class MediaTypeStates
        {
            internal const string AudioOnly = "AudioOnly";
            internal const string AudioVideo = "AudioVideo";
        }
#endif
    }
}
