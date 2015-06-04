//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.PlayerFramework.Samples
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "Xaml Samples";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() {Title = "Progressive video", Description = "Demonstrates basic playback of progressive download video.", ClassType = typeof(ProgressivePage) },
            new Scenario() {Title = "Playlists", Description = "Demonstrates playlist support.", ClassType = typeof(PlaylistPage) },
            new Scenario() {Title = "Player Framework settings", Description = "Demonstrates a number of the settings that can be used to control the PlayerFramework.", ClassType = typeof(SettingsPage) },
            new Scenario() {Title = "Error handling UI", Description = "Demonstrates the built-in error handler UI.", ClassType = typeof(ErrorPage) },
            new Scenario() {Title = "Poster image", Description = "Demonstrates showing a poster image.", ClassType = typeof(PosterPage) },
            new Scenario() {Title = "Click to play", Description = "Demonstrates the built-in click to play feature to allow users to only start download once the user clicks.", ClassType = typeof(AutoLoadPage) },
            new Scenario() {Title = "Classic theme", Description = "Uses the IE default browser look and feel.", ClassType = typeof(ClassicThemePage) },
            new Scenario() {Title = "Phone theme", Description = "Uses the Windows Phone 8 video theme.", ClassType = typeof(PhoneThemePage) },
            new Scenario() {Title = "Entertainment theme", Description = "Uses the Windows Entertainment app theme.", ClassType = typeof(EntertainmentThemePage) },
            new Scenario() {Title = "MPEG-DASH streaming video", Description = "Demonstrates basic playback of the new W3C adaptive streaming technology called DASH.", ClassType = typeof(DashPage) },
            new Scenario() {Title = "HLS streaming video", Description = "Demonstrates basic playback of the Apple HLS (Http Live Streaming) format.", ClassType = typeof(HlsPage) },
            new Scenario() {Title = "WebVTT captions (side car)", Description = "Demonstrates playing WebVTT captions.", ClassType = typeof(WebVTTPage) },
            new Scenario() {Title = "TTML captions (side car)", Description = "Demonstrates TTML closed captions/Descriptions.", ClassType = typeof(TtmlPage) },
            new Scenario() {Title = "Schedule VAST ads", Description = "Demonstrates playing a VAST preroll, midroll, and postroll", ClassType = typeof(AdSchedulingPage) },
            new Scenario() {Title = "VMAP ad scheduling", Description = "Demonstrates using VMAP to schedule ads", ClassType = typeof(VmapPage) },
            new Scenario() {Title = "FreeWheel SmartXML", Description = "Demonstrates using FreeWheel's Smart XML to schedule and play ads", ClassType = typeof(FreeWheelPage) },
            new Scenario() {Title = "MAST ad scheduling", Description = "Demonstrates scheduling ads using MAST (Media Abstract Sequencing Template)", ClassType = typeof(MastPage) },
            new Scenario() {Title = "Schedule simple clip", Description = "Demonstrates how to schedule a simple clip (not VAST)", ClassType = typeof(ScheduleClipPage) },
            new Scenario() {Title = "Programmatic ad", Description = "Demonstrates how to programmatically schedule and play an ad", ClassType = typeof(ProgrammaticAdPage) },
            new Scenario() {Title = "Nonlinear ad", Description = "Demonstrates playing a VAST file that contains a linear and nonlinear ad", ClassType = typeof(VastLinearNonlinearPage) },
            new Scenario() {Title = "Ad pod", Description = "Demonstrates a VAST ad pod using VAST", ClassType = typeof(AdPodPage) },
            new Scenario() {Title = "Companion ads", Description = "Demonstrates how to play an ad with companions", ClassType = typeof(VastLinearCompanionPage) },
            new Scenario() {Title = "Thumbnails", Description = "Demonstrates how to show thumbnails during scrubbing, RW, & FF operations.", ClassType = typeof(ThumbnailPage) },
            new Scenario() {Title = "Tracking", Description = "Demonstrates how to get tracking events for analytics purposes.", ClassType = typeof(TrackingPage) },
            new Scenario() {Title = "Timeline Markers", Description = "Demonstrates showing visual markers in the timeline that the user can seek to.", ClassType = typeof(MarkersPage) },
            new Scenario() {Title = "Local playback", Description = "Demonstrates playing a local video and capturing input from the webcam.", ClassType = typeof(LocalPlaybackPage) },
            new Scenario() {Title = "Play To", Description = "Demonstrates how to use Play To, to enable users to stream media to a target device.", ClassType = typeof(PlayToPage) },
            new Scenario() {Title = "Suspend & resume", Description = "Demonstrates how to suspend and resume the player.", ClassType = typeof(SuspendResumePage) }
            };
    }

    public class Scenario
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Type ClassType { get; set; }
    }
}
