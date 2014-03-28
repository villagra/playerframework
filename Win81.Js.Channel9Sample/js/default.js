(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    function onApplicationActivated(e) {
        if (e.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            if (WinJS.Application.sessionState.history) {
                WinJS.Navigation.history = WinJS.Application.sessionState.history;
            }

            // TODO: Consider moving blocking data download and displaying feedback to user
            e.setPromise(Data.downloadFeed().then(
                function () {
                    return WinJS.UI.processAll();
                }
            ).then(
                function () {
                    if (WinJS.Navigation.location) {
                        WinJS.Navigation.history.current.initialPlaceholder = true;
                        return WinJS.Navigation.navigate(WinJS.Navigation.location, WinJS.Navigation.state);
                    } else {
                        return WinJS.Navigation.navigate(Application.navigator.home);
                    }
                }
            ));
        }
    }

    function onApplicationCheckpoint(e) {
        WinJS.Application.sessionState.history = WinJS.Navigation.history;
    }

    WinJS.Application.addEventListener("activated", onApplicationActivated, false);
    WinJS.Application.addEventListener("checkpoint", onApplicationCheckpoint, false);
    WinJS.Application.start();
})();