(function () {
    "use strict";

    var mediaPlayer = null;
    var mediaProtectionManager = null;
    var playReadyMediaSource = "http://playready.directtaps.net/win/media/TallShip_with_Credits_folk_rock_soundtrack_encrypted.wmv";
    var playReadyLicenseAcquirerServiceUrl = "http://playready.directtaps.net/win/rightsmanager.asmx";
    var playReadyPromise = null;

    WinJS.UI.Pages.define("/pages/common/playready/playready.html", {
        // This function is called whenever a user navigates to this page.
        // It populates the page with data and initializes the media player control.
        ready: function (element, options) {
            var item = options && options.item ? Data.resolveItemReference(options.item) : Data.items.getAt(0);
            element.querySelector(".titlearea .pagetitle").textContent = item.title;
            if (WinJS.Utilities.isPhone) {
                document.getElementById("header").style.display = "none";
            }

            var mediaPlayerElement = element.querySelector("[data-win-control='PlayerFramework.MediaPlayer']");
            mediaPlayer = mediaPlayerElement.winControl;
            mediaPlayer.focus();

            console.log("Setting up media player");
            mediaPlayer.addEventListener("canplay", onMediaPlayerCanPlay, false);
            mediaPlayer.addEventListener("ended", onMediaPlayerEnded, false);
            mediaPlayer.addEventListener("error", onMediaPlayerError, false);

            console.log("Setting up media extension manager");
            mediaPlayer.mediaExtensionManager.registerByteStreamHandler("Microsoft.Media.PlayReadyClient.PlayReadyByteStreamHandler", ".pyv", null);
            mediaPlayer.mediaExtensionManager.registerByteStreamHandler("Microsoft.Media.PlayReadyClient.PlayReadyByteStreamHandler", ".pya", null);
            mediaPlayer.mediaExtensionManager.registerByteStreamHandler("Microsoft.Media.PlayReadyClient.PlayReadyByteStreamHandler", ".wmv", null);

            console.log("Setting up media protection manager");
            mediaProtectionManager = new Windows.Media.Protection.MediaProtectionManager();
            mediaProtectionManager.properties["Windows.Media.Protection.MediaProtectionSystemId"] = "{F4637010-03C3-42CD-B932-B48ADF3A6A54}";
            mediaProtectionManager.properties["Windows.Media.Protection.MediaProtectionSystemIdMapping"] = new Windows.Foundation.Collections.PropertySet();
            mediaProtectionManager.properties["Windows.Media.Protection.MediaProtectionSystemIdMapping"]["{F4637010-03C3-42CD-B932-B48ADF3A6A54}"] = "Microsoft.Media.PlayReadyClient.PlayReadyWinRTTrustedInput";
            mediaProtectionManager.addEventListener("servicerequested", onMediaProtectionManagerServiceRequested, false);
            mediaProtectionManager.addEventListener("componentloadfailed", onMediaProtectionManagerComponentLoadFailed, false);
            mediaPlayer.msSetMediaProtectionManager(mediaProtectionManager);

            console.log("Attempting media playback...");
            mediaPlayer.src = playReadyMediaSource;
        },

        // This function is called whenever a user navigates away from this page.
        // It resets the page and disposes of the media player control.
        unload: function () {
            if (playReadyPromise) {
                playReadyPromise.cancel();
                playReadyPromise = null;
            }

            if (mediaProtectionManager) {
                mediaProtectionManager.removeEventListener("servicerequested", onMediaProtectionManagerServiceRequested, false);
                mediaProtectionManager.removeEventListener("componentloadfailed", onMediaProtectionManagerComponentLoadFailed, false);
                mediaProtectionManager = null;
            }

            if (mediaPlayer) {
                mediaPlayer.removeEventListener("canplay", onMediaPlayerCanPlay, false);
                mediaPlayer.removeEventListener("ended", onMediaPlayerEnded, false);
                mediaPlayer.removeEventListener("error", onMediaPlayerError, false);
                mediaPlayer.dispose();
                mediaPlayer = null;
            }
        }
    });

    function onMediaPlayerCanPlay(e) {
        console.log("Media playback started");
        mediaPlayer.play();
    }

    function onMediaPlayerEnded(e) {
        console.log("Media playback ended");
    }

    function onMediaPlayerError(e) {
        console.log("Media error occurred");
        console.log("--- Message: " + PlayerFramework.Utilities.getMediaErrorMessage(e.detail.error));
    }

    function onMediaProtectionManagerServiceRequested(e) {
        console.log("Media protection manager service requested");
        handleServiceRequest(e.request, e.completion);
    }

    function onMediaProtectionManagerComponentLoadFailed(e) {
        console.log("Media protection manager component load failed");

        for (var i = 0; i < e.information.items.size; i++) {
            var component = e.information.items[i];
            console.log("--- Component: " + component.name + " (0x" + component.reasons + ")");
        }

        e.completion.complete(false);
    }

    function handleServiceRequest(serviceRequest, serviceCompletion) {
        console.log("--- Handling service request");

        if (serviceRequest.type === Microsoft.Media.PlayReadyClient.PlayReadyStatics.licenseAcquirerServiceRequestType) {
            serviceRequest.uri = new Windows.Foundation.Uri(playReadyLicenseAcquirerServiceUrl);
            serviceRequest.challengeCustomData = "Custom Data";
        }

        var serviceRequestType = getServiceRequestType(serviceRequest);

        if (serviceRequestType) {
            console.log("------ Type: " + serviceRequestType);
        }

        if (serviceRequest.uri) {
            console.log("------ URI: " + serviceRequest.uri.absoluteUri);
        }

        if (serviceRequest.challengeCustomData) {
            console.log("------ Challenge Custom Data: " + serviceRequest.challengeCustomData);
        }

        if (serviceRequest.protectionSystem) {
            console.log("------ Protection System: " + serviceRequest.protectionSystem);
        }

        playReadyPromise = serviceRequest.beginServiceRequest().then(
            function () {
                console.log("Media protection manager service request completed");
                serviceCompletion.complete(true);
            },
            function () {
                console.log("Media protection manager service request failed");
                var nextServiceRequest = serviceRequest.nextServiceRequest();
                if (nextServiceRequest) {
                    handleServiceRequest(nextServiceRequest, serviceCompletion);
                } else {
                    serviceCompletion.complete(false);
                }
            }
        );
    }

    function getServiceRequestType(serviceRequest) {
        switch (serviceRequest.type) {
            case Microsoft.Media.PlayReadyClient.PlayReadyStatics.licenseAcquirerServiceRequestType:
                return "PlayReadyLicenseAcquirerServiceRequest";
            case Microsoft.Media.PlayReadyClient.PlayReadyStatics.individualizationServiceRequestType:
                return "IndividualizationServiceRequest";
            case Microsoft.Media.PlayReadyClient.PlayReadyStatics.domainJoinServiceRequestType:
                return "PlayReadyDomainJoinServiceRequest";
            case Microsoft.Media.PlayReadyClient.PlayReadyStatics.domainLeaveServiceRequestType:
                return "PlayReadyDomainLeaveServiceRequest";
            case Microsoft.Media.PlayReadyClient.PlayReadyStatics.meteringReportServiceRequestType:
                return "PlayReadyMeteringServiceRequest";
            default:
                return null;
        }
    }
})();