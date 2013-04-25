(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    function onApplicationActivated(e) {
        if (e.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            e.setPromise(WinJS.UI.processAll());
        }
    }

    WinJS.Application.addEventListener("activated", onApplicationActivated, false);
    WinJS.Application.start();
})();