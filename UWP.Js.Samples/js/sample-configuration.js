//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var sampleTitle = "JavaScript samples";

    var scenarios = [
        // common
        { url: "/pages/common/progressive/progressive.html", title: "Progressive Video", description: "Demonstrates playback of progressive download video" },
        { url: "/pages/common/playlist/playlist.html", title: "Playlists", description: "Demonstrates playlist support" },
        { url: "/pages/common/options/options.html", title: "Player Options", description: "Demonstrates a number of the options available in the Player Framework" },
        { url: "/pages/common/poster/poster.html", title: "Poster Image", description: "Demonstrates showing a poster image" },
        { url: "/pages/common/clicktoplay/clicktoplay.html", title: "Click to Play", description: "Demonstrates delaying media download until the user clicks a button" },
        { url: "/pages/common/light/light.html", title: "Light Theme", description: "Demonstrates a player using the light theme" },
        { url: "/pages/common/error/error.html", title: "Error Handling", description: "Demonstrates the UI displayed by default when media errors occur and handling non-critical errors" },
        //{ url: "/pages/common/playready/playready.html", title: "PlayReady", description: "Demonstrates secure content protection using PlayReady" },

        // adaptive streaming
        //{ url: "/pages/adaptive/vod/vod.html", title: "Smooth streaming (VOD)", description: "Demonstrates basic playback of smooth streaming video using the Microsoft Smooth Streaming SDK." },
        //{ url: "/pages/adaptive/live/live.html", title: "Smooth streaming (Live)", description: "Demonstrates basic playback of live smooth streaming video using the Microsoft Smooth Streaming SDK." },
        { url: "/pages/adaptive/dash/dash.html", title: "DASH streaming video", description: "Demonstrates basic playback of the new W3C adaptive streaming technology called DASH." },
        { url: "/pages/adaptive/hls/hls.html", title: "HLS streaming video", description: "Demonstrates basic playback of the Apple HLS (Http Live Streaming) format." },

        // closed captioning
        { url: "/pages/captions/ttml/ttml.html", title: "TTML Captions", description: "Demonstrates TTML captions using the native closed captioning features of the video element" },
        { url: "/pages/captions/webvtt/webvtt.html", title: "WebVTT Captions", description: "Demonstrates playing WebVTT captions using the native closed captioning feature of the video element" },
        { url: "/pages/captions/timedtext/timedtext.html", title: "Timed Text Captions", description: "Demonstrates caption styling and layout using TTML (Timed Text Markup Language) using the captions plugin" },
        //{ url: "/pages/captions/instreamttml/instreamttml.html", title: "In-stream Captions", description: "Demonstrates playing in-stream TTML captions from smooth streaming text tracks" },

        // advertising
        { url: "/pages/advertising/mast/mast.html", title: "MAST Ad Scheduling", description: "Demonstrates scheduling ads using MAST (Media Abstract Sequencing Template)" },
        { url: "/pages/advertising/vast/vast.html", title: "VAST Ad Scheduling", description: "Demonstrates scheduling preroll, midroll, and postroll ads using VAST (Video Ad Serving Template)" },
        { url: "/pages/advertising/vmap/vmap.html", title: "VMAP Ad Scheduling", description: "Demonstrates scheduling ads using VMAP (Video Multiple Ad Playlist)" },
        { url: "/pages/advertising/freewheel/freewheel.html", title: "FreeWheel Ad Scheduling", description: "Demonstrates scheduling ads using FreeWheel's Smart XML" },
        { url: "/pages/advertising/adpod/adpod.html", title: "Ad Pod", description: "Demonstrates playing an ad pod using VAST" },
        { url: "/pages/advertising/linearnonlinear/linearnonlinear.html", title: "Linear and Non-Linear Ads", description: "Demonstrates playing linear and non-linear ads using VAST" },
        { url: "/pages/advertising/companion/companion.html", title: "Companion Ads", description: "Demonstrates playing an ad with companions using VAST" },
        { url: "/pages/advertising/clip/clip.html", title: "Clip Ad", description: "Demonstrates playing a simple clip ad" },
        { url: "/pages/advertising/programmatic/programmatic.html", title: "Programmatic Ad", description: "Demonstrates how to programmatically create, play, and cancel an ad" },

        // advanced
        { url: "/pages/advanced/thumbnails/thumbnails.html", title: "Thumbnails", description: "Demonstrates how to show thumbnails during scrubbing, RW, & FF operations." },
        { url: "/pages/advanced/tracking/tracking.html", title: "Event Tracking", description: "Demonstrates event tracking for analytic purposes" },
        { url: "/pages/advanced/playto/playto.html", title: "Play To", description: "Demonstrates streaming media from the player to a target device using Play To" },
        { url: "/pages/advanced/local/local.html", title: "Local Playback", description: "Demonstrates playing a local video file and capturing input from a webcam" },
        { url: "/pages/advanced/suspendresume/suspendresume.html", title: "Suspend and Resume", description: "Demonstrates managing the state of the player throughout the application lifecycle" },
        { url: "/pages/advanced/visualmarkers/visualmarkers.html", title: "Visual Markers", description: "Allow the user to see markers (ticks) in the timeline" }
    ];

    WinJS.Namespace.define("SdkSample", {
        sampleTitle: sampleTitle,
        scenarios: new WinJS.Binding.List(scenarios)
    });
})();
