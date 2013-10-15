using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.PlayerFramework.Samples.Data
{
    public static class SampleData
    {
        public static IEnumerable<SampleDataGroup> GetGroups()
        {
            var result = new List<SampleDataGroup>();

            var group1 = new SampleDataGroup("Common",
                    "Common",
                    "Common Samples",
                    "Assets/DarkGray.png",
                    "Demonstrates common uses of the player framework");
            foreach (var sample in GetSamples())
            {
                group1.Items.Add(sample);
            }
            result.Add(group1);

            var groupAdaptive = new SampleDataGroup("Adaptive Streaming",
                    "Adaptive Streaming",
                    "Adaptive Streaming Samples",
                    "Assets/DarkGray.png",
                    "Demonstrates the various ways to use the smooth streaming SDK to support adaptive streaming");
            foreach (var sample in GetAdaptiveSamples())
            {
                groupAdaptive.Items.Add(sample);
            }
            result.Add(groupAdaptive);


            var group2 = new SampleDataGroup("Captions",
                    "Captions",
                    "Captioning Samples",
                    "Assets/DarkGray.png",
                    "Demonstrates the various ways to use the player framework's closed captioning and subtitles feature");
            foreach (var sample in GetCaptionSamples())
            {
                group2.Items.Add(sample);
            }
            result.Add(group2);

            var group3 = new SampleDataGroup("Advertising",
                    "Advertising",
                    "Advertising Samples",
                    "Assets/DarkGray.png",
                    "Demonstrates the various ways to use the player framework's advertising feature");
            foreach (var sample in GetAdvertisingSamples())
            {
                group3.Items.Add(sample);
            }
            result.Add(group3);

            var group4 = new SampleDataGroup("Advanced",
                    "Advanced",
                    "Advanced Samples",
                    "Assets/DarkGray.png",
                    "Demonstrates advanced player framework features");
            foreach (var sample in GetAdvancedSamples())
            {
                group4.Items.Add(sample);
            }
            result.Add(group4);

            return result;
        }

        static IEnumerable<SampleDataItem> GetAdaptiveSamples()
        {
            yield return new SampleDataItem(
                    "Smooth streaming (VOD)",
                    typeof(SmoothPage),
                    "Demonstrates basic playback of smooth streaming video using the Microsoft Smooth Streaming SDK.");

            yield return new SampleDataItem(
                    "Smooth streaming (Live)",
                    typeof(LivePage),
                    "Demonstrates basic playback of live smooth streaming video using the Microsoft Smooth Streaming SDK.");

            yield return new SampleDataItem(
                    "DASH streaming video",
                    typeof(DashPage),
                    "Demonstrates basic playback of the new W3C adaptive streaming technology called DASH.");

            yield return new SampleDataItem(
                    "CFF progressive download video",
                    typeof(CffProgressivePage),
                    "Demonstrates basic playback of the CFF encoding technology.");

            yield return new SampleDataItem(
                    "CFF offline video",
                    typeof(CffOfflinePage),
                    "Demonstrates basic playback of an offline CFF file.");

        }

        static IEnumerable<SampleDataItem> GetAdvertisingSamples()
        {
            yield return new SampleDataItem(
            "MAST ad scheduling",
            typeof(Advertising.MastPage),
            "Demonstrates scheduling ads using MAST (Media Abstract Sequencing Template)");
            //http://openvideoplayer.sourceforge.net/mast/mast_specification.pdf

            yield return new SampleDataItem(
            "Nonlinear ad",
            typeof(Advertising.VastLinearNonlinearPage),
            "Demonstrates playing a VAST file that contains a linear and nonlinear ad");

            yield return new SampleDataItem(
            "Schedule VAST ads",
            typeof(Advertising.AdSchedulingPage),
            "Demonstrates playing a VAST preroll, midroll, and postroll");

            yield return new SampleDataItem(
            "Schedule simple clip",
            typeof(Advertising.ScheduleClipPage),
            "Demonstrates how to schedule a simple clip (not VAST)");

            yield return new SampleDataItem(
            "Ad pod",
            typeof(Advertising.AdPodPage),
            "Demonstrates a VAST ad pod using VAST");

            yield return new SampleDataItem(
            "Programmatic ad",
            typeof(Advertising.ProgrammaticAdPage),
            "Demonstrates how to programmatically schedule and play an ad");

            yield return new SampleDataItem(
            "Companion ads",
            typeof(Advertising.VastLinearCompanionPage),
            "Demonstrates how to play an ad with companions");

            yield return new SampleDataItem(
            "VMAP ad scheduling",
            typeof(Advertising.VmapPage),
            "Demonstrates using VMAP to schedule ads");

            yield return new SampleDataItem(
            "FreeWheel advertising",
            typeof(Advertising.FreeWheelPage),
            "Demonstrates using FreeWheel's Smart XML to schedule and play ads");
        }

        static IEnumerable<SampleDataItem> GetCaptionSamples()
        {
            yield return new SampleDataItem(
                    "Plain text captions",
                    typeof(PlainTextPage),
                    "Demonstrates displaying plain-text closed captions/subtitles.");

            yield return new SampleDataItem(
                    "TTML captions",
                    typeof(TtmlPage),
                    "Demonstrates TTML closed captions/subtitles.");

            yield return new SampleDataItem(
                    "In-stream captions",
                    typeof(InstreamTtmlPage),
                    "Demonstrates playing in-stream TTML captions from smooth streaming text tracks.");

            yield return new SampleDataItem(
                    "WebVTT captions",
                    typeof(WebVTTPage),
                    "Demonstrates playing WebVTT captions.");
        }

        static IEnumerable<SampleDataItem> GetAdvancedSamples()
        {
            yield return new SampleDataItem(
                    "Thumbnails",
                    typeof(ThumbnailPage),
                    "Demonstrates how to show thumbnails during scrubbing, RW, & FF operations.");

            yield return new SampleDataItem(
                    "Tracking",
                    typeof(TrackingPage),
                    "Demonstrates how to get tracking events for analytics purposes.");

            yield return new SampleDataItem(
                    "Timeline Markers",
                    typeof(MarkersPage),
                    "Demonstrates showing visual markers in the timeline that the user can seek to.");

            yield return new SampleDataItem(
                    "Local playback demo",
                    typeof(LocalPlaybackPage),
                    "Demonstrates playing a local video and capturing input from the webcam.");

            yield return new SampleDataItem(
                    "Play To demo",
                    typeof(PlayToPage),
                    "Demonstrates how to use Play To, to enable users to stream media to a target device.");

            yield return new SampleDataItem(
                    "Suspend & resume demo",
                    typeof(SuspendResumePage),
                    "Demonstrates how to suspend and resume the player.");

        }

        static IEnumerable<SampleDataItem> GetSamples()
        {
            yield return new SampleDataItem(
                    "Progressive video",
                    typeof(ProgressivePage),
                    "Demonstrates basic playback of progressive download video.");

            yield return new SampleDataItem(
                    "Playlists",
                    typeof(PlaylistPage),
                    "Demonstrates playlist support.");

            yield return new SampleDataItem(
                    "Player Framework settings",
                    typeof(SettingsPage),
                    "Demonstrates a number of the settings that can be used to control the PlayerFramework.");

            yield return new SampleDataItem(
                    "Error handling UI",
                    typeof(ErrorPage),
                    "Demonstrates the built-in error handler UI.");

            yield return new SampleDataItem(
                    "Poster image",
                    typeof(PosterPage),
                    "Demonstrates showing a poster image.");

            yield return new SampleDataItem(
                    "Click to play",
                    typeof(AutoLoadPage),
                    "Demonstrates the built-in click to play feature to allow users to only start download once the user clicks.");

            yield return new SampleDataItem(
                    "PlayReady DRM",
                    typeof(PlayReadyPage),
                    "Demonstrates PlayReady DRM. Note: you must set the build configuration to x86 if you are running on 32-bit Windows.");

            yield return new SampleDataItem(
                    "Player Styling",
                    typeof(EntertainmentAppPage),
                    "Demonstrates how to style the player.");
        }
    }
}