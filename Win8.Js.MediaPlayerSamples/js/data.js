(function () {
    "use strict";

    var list = new WinJS.Binding.List();

    var groupedItems = list.createGrouped(
        function groupKeySelector(item) { return item.group.key; },
        function groupDataSelector(item) { return item.group; }
    );

    generateSampleData().forEach(function (item) {
        list.push(item);
    });

    // Get the page for an item.
    function getItemPage(item) {
        var index = item.subtitle.lastIndexOf("/");
        var name = item.subtitle.substring(index + 1);
        return "/pages/itemdetail/" + item.subtitle + "/" + name + ".html";
    }

    // Get a reference for an item, using the group key and item title as a
    // unique reference to the item that can be easily serialized.
    function getItemReference(item) {
        return [item.group.key, item.title];
    }

    // This function returns a WinJS.Binding.List containing only the items
    // that belong to the provided group.
    function getItemsFromGroup(group) {
        return list.createFiltered(function (item) { return item.group.key === group.key; });
    }

    // Get the unique group corresponding to the provided group key.
    function resolveGroupReference(key) {
        for (var i = 0; i < groupedItems.groups.length; i++) {
            if (groupedItems.groups.getAt(i).key === key) {
                return groupedItems.groups.getAt(i);
            }
        }
    }

    // Get a unique item from the provided string array, which should contain a
    // group key and an item title.
    function resolveItemReference(reference) {
        for (var i = 0; i < groupedItems.length; i++) {
            var item = groupedItems.getAt(i);
            if (item.group.key === reference[0] && item.title === reference[1]) {
                return item;
            }
        }
    }

    // Returns an array of sample data that is added to the application's data list. 
    function generateSampleData() {
        // sample images
        var sampleImage1 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXY3B0cPoPAANMAcOba1BlAAAAAElFTkSuQmCC";
        var sampleImage2 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXY5g8dcZ/AAY/AsAlWFQ+AAAAAElFTkSuQmCC";
        var sampleImage3 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXY7h4+cp/AAhpA3h+ANDKAAAAAElFTkSuQmCC";

        // sample groups
        var sampleGroups = [
            { key: "sampleGroup1", backgroundImage: sampleImage1, subtitle: "common", title: "Common", description: "These samples demonstrate basic usage and features of the Microsoft Player Framework." },
            { key: "sampleGroup2", backgroundImage: sampleImage2, subtitle: "adaptive", title: "Adaptive Streaming", description: "These samples demonstrate the adaptive streaming features of the Microsoft Player Framework." },
            { key: "sampleGroup3", backgroundImage: sampleImage3, subtitle: "captions", title: "Closed Captioning", description: "These samples demonstrate the closed captioning features of the Microsoft Player Framework." },
            { key: "sampleGroup4", backgroundImage: sampleImage1, subtitle: "advertising", title: "Advertising", description: "These samples demonstrate the advertising features of the Microsoft Player Framework." },
            { key: "sampleGroup5", backgroundImage: sampleImage2, subtitle: "advanced", title: "Advanced", description: "These samples demonstrate advanced usage and features of the Microsoft Player Framework." }
        ];

        // sample items
        var sampleItems = [
            // common
            { group: sampleGroups[0], backgroundImage: sampleImage1, subtitle: "common/progressive", title: "Progressive Video", description: "Demonstrates playback of progressive download video" },
            { group: sampleGroups[0], backgroundImage: sampleImage1, subtitle: "common/playlist", title: "Playlists", description: "Demonstrates playlist support" },
            { group: sampleGroups[0], backgroundImage: sampleImage1, subtitle: "common/options", title: "Player Options", description: "Demonstrates a number of the options available in the Player Framework" },
            { group: sampleGroups[0], backgroundImage: sampleImage1, subtitle: "common/poster", title: "Poster Image", description: "Demonstrates showing a poster image" },
            { group: sampleGroups[0], backgroundImage: sampleImage1, subtitle: "common/clicktoplay", title: "Click to Play", description: "Demonstrates delaying media download until the user clicks a button" },
            { group: sampleGroups[0], backgroundImage: sampleImage1, subtitle: "common/light", title: "Light Theme", description: "Demonstrates a player using the light theme" },
            { group: sampleGroups[0], backgroundImage: sampleImage1, subtitle: "common/error", title: "Error Handling", description: "Demonstrates the UI displayed by default when media errors occur and handling non-critical errors" },
            { group: sampleGroups[0], backgroundImage: sampleImage1, subtitle: "common/playready", title: "PlayReady", description: "Demonstrates secure content protection using PlayReady" },

            // adaptive streaming
            { group: sampleGroups[1], backgroundImage: sampleImage2, subtitle: "adaptive/vod", title: "Video On Demand", description: "Demonstrates playback of smooth streaming video on demand using the adaptive plugin" },
            { group: sampleGroups[1], backgroundImage: sampleImage2, subtitle: "adaptive/live", title: "Live Smooth Streaming", description: "Demonstrates the live smooth streaming features of the Player Framework using the adaptive plugin" },

            // closed captioning
            { group: sampleGroups[2], backgroundImage: sampleImage3, subtitle: "captions/plaintext", title: "Plain Text Captions", description: "Demonstrates plain text captions using the native closed captioning features of the video element" },
            { group: sampleGroups[2], backgroundImage: sampleImage3, subtitle: "captions/timedtext", title: "Timed Text Captions", description: "Demonstrates caption styling and layout using TTML (Timed Text Markup Language) and the captions plugin" },
            { group: sampleGroups[2], backgroundImage: sampleImage3, subtitle: "captions/instreamttml", title: "In-stream Captions", description: "Demonstrates playing in-stream TTML captions from smooth streaming text tracks" },
            { group: sampleGroups[2], backgroundImage: sampleImage3, subtitle: "captions/webvtt", title: "WebVTT Captions", description: "Demonstrates playing WebVTT captions using the native closed captioning feature of the video element" },

            // advertising
            { group: sampleGroups[3], backgroundImage: sampleImage1, subtitle: "advertising/mast", title: "MAST Ad Scheduling", description: "Demonstrates scheduling ads using MAST (Media Abstract Sequencing Template)" },
            { group: sampleGroups[3], backgroundImage: sampleImage1, subtitle: "advertising/vast", title: "VAST Ad Scheduling", description: "Demonstrates scheduling preroll, midroll, and postroll ads using VAST (Video Ad Serving Template)" },
            { group: sampleGroups[3], backgroundImage: sampleImage1, subtitle: "advertising/vmap", title: "VMAP Ad Scheduling", description: "Demonstrates scheduling ads using VMAP (Video Multiple Ad Playlist)" },
            { group: sampleGroups[3], backgroundImage: sampleImage1, subtitle: "advertising/freewheel", title: "FreeWheel Ad Scheduling", description: "Demonstrates scheduling ads using FreeWheel's Smart XML" },
            { group: sampleGroups[3], backgroundImage: sampleImage1, subtitle: "advertising/adpod", title: "Ad Pod", description: "Demonstrates playing an ad pod using VAST" },
            { group: sampleGroups[3], backgroundImage: sampleImage1, subtitle: "advertising/linearnonlinear", title: "Linear and Non-Linear Ads", description: "Demonstrates playing linear and non-linear ads using VAST" },
            { group: sampleGroups[3], backgroundImage: sampleImage1, subtitle: "advertising/companion", title: "Companion Ads", description: "Demonstrates playing an ad with companions using VAST" },
            { group: sampleGroups[3], backgroundImage: sampleImage1, subtitle: "advertising/clip", title: "Clip Ad", description: "Demonstrates playing a simple clip ad" },
            { group: sampleGroups[3], backgroundImage: sampleImage1, subtitle: "advertising/programmatic", title: "Programmatic Ad", description: "Demonstrates how to programmatically create, play, and cancel an ad" },

            // advanced
            { group: sampleGroups[4], backgroundImage: sampleImage2, subtitle: "advanced/tracking", title: "Event Tracking", description: "Demonstrates event tracking for analytic purposes" },
            { group: sampleGroups[4], backgroundImage: sampleImage2, subtitle: "advanced/playto", title: "Play To", description: "Demonstrates streaming media from the player to a target device using Play To" },
            { group: sampleGroups[4], backgroundImage: sampleImage2, subtitle: "advanced/local", title: "Local Playback", description: "Demonstrates playing a local video file and capturing input from a webcam" },
            { group: sampleGroups[4], backgroundImage: sampleImage2, subtitle: "advanced/suspendresume", title: "Suspend and Resume", description: "Demonstrates managing the state of the player throughout the application lifecycle" }
        ];

        return sampleItems;
    }

    WinJS.Namespace.define("Data", {
        items: groupedItems,
        groups: groupedItems.groups,
        getItemPage: getItemPage,
        getItemReference: getItemReference,
        getItemsFromGroup: getItemsFromGroup,
        resolveGroupReference: resolveGroupReference,
        resolveItemReference: resolveItemReference
    });
})();