// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            args.setPromise(WinJS.UI.processAll());
        }

        var mediaPlayerElement = document.querySelector("[data-win-control='PlayerFramework.MediaPlayer']");
        var mediaPlayer = mediaPlayerElement.winControl;
        var configUrl = new Windows.Foundation.Uri("ms-appx:///AudienceInsightConfig.xml");

        Microsoft.AudienceInsight.BatchingConfigFactory.load(configUrl)
            .then(function (batchingConfig) {

                // Audience Insight config

                var dataClient = batchingConfig.batchAgent;
                dataClient.additionalHttpHeaders["Authorization-Token"] = "{2842C782-562E-4250-A1A2-F66D55B5EA15}";

                var batchinglogAgent = new Microsoft.AudienceInsight.BatchingLogAgent(batchingConfig);
                var aiLoggingTarget = new Microsoft.Media.Analytics.AudienceInsight.AudienceInsightLoggingTarget(batchinglogAgent);

                Microsoft.Media.Analytics.LoggingService.current.loggingTargets.append(aiLoggingTarget);

                return Microsoft.Media.Analytics.AnalyticsConfig.load(configUrl);

            })
            .then(function (analyticsConfig) {

                // Player Framework analytics config

                var analyticsPlugin = mediaPlayer.analyticsPlugin;

                var adaptivePlugin = mediaPlayer.adaptivePlugin;
                var adaptiveMonitorFactory = new Microsoft.AdaptiveStreaming.Analytics.AdaptiveMonitorFactory(adaptivePlugin.manager);

                var edgeServerMonitor = new Microsoft.Media.Analytics.EdgeServerMonitor();

                analyticsPlugin.adaptiveMonitor = adaptiveMonitorFactory.adaptiveMonitor;
                analyticsPlugin.edgeServerMonitor = edgeServerMonitor;

                // Audience Insight ad tracking config

                analyticsPlugin.analyticsCollector.loggingSources.push(new Microsoft.Media.Analytics.VideoAdvertising.AdvertisingLoggingSource(mediaPlayer.adHandlerPlugin.adHandlerController));

                mediaPlayer.focus();
            });
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    app.start();
})();
