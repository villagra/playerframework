// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    var batchinglogAgent = null;

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
            args.setPromise(WinJS.UI.processAll());

            Microsoft.AudienceInsight.BatchingConfigFactory.load(new Windows.Foundation.Uri("ms-appx:///AudienceInsightConfig.xml"))
            .then(function (batchingConfig) {

                // Audience Insight config

                var dataClient = batchingConfig.batchAgent;
                dataClient.additionalHttpHeaders["Authorization-Token"] = "{2842C782-562E-4250-A1A2-F66D55B5EA15}";

                batchinglogAgent = new Microsoft.AudienceInsight.BatchingLogAgent(batchingConfig);
            });

            var sendButton = document.getElementById("sendButton");
            sendButton.addEventListener("click", buttonClickHandler, false);
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    function buttonClickHandler(eventInfo) {

        var customLog = {
            CustomProperty: "testing",
            CustomPropertyNumber: 3.14159,
            CustomPropertyBool: true
        };

        batchinglogAgent.logEntry(new Microsoft.AudienceInsight.Log("CustomLog", JSON.stringify(customLog)));
    }

    app.start();
})();
