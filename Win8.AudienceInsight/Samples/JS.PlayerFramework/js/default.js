// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";

    WinJS.Binding.optimizeBindingReferences = true;

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

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
        }

        var mediaPlayerElement = document.querySelector("[data-win-control='PlayerFramework.MediaPlayer']");
        var mediaPlayer = mediaPlayerElement.winControl;

        Microsoft.AudienceInsight.BatchingConfigFactory.load(new Windows.Foundation.Uri("ms-appx:///AudienceInsightConfig.xml"))
            .then(function (batchingConfig) {

                // Audience Insight config

                var dataClient = batchingConfig.batchAgent;
                dataClient.additionalHttpHeaders["Authorization-Token"] = "{2842C782-562E-4250-A1A2-F66D55B5EA15}";

                var batchinglogAgent = new Microsoft.AudienceInsight.BatchingLogAgent(batchingConfig);
                var aiLoggingTarget = new Microsoft.VideoAnalytics.AudienceInsight.AudienceInsightLoggingTarget(batchinglogAgent);

                Microsoft.VideoAnalytics.LoggingService.current.loggingTargets.append(aiLoggingTarget);

                return Microsoft.VideoAnalytics.AnalyticsConfig.load(new Windows.Foundation.Uri("ms-appx:///AudienceInsightConfig.xml"));

            })
            .then(function (analyticsConfig) {

                // Player Framework analytics config

                var analyticsPlugin = mediaPlayer.analyticsPlugin;

                var adaptivePlugin = mediaPlayer.adaptivePlugin;
                var adaptiveMonitorFactory = new Microsoft.AdaptiveStreaming.Analytics.AdaptiveMonitorFactory(adaptivePlugin.manager);

                var edgeServerMonitor = new Microsoft.VideoAnalytics.EdgeServerMonitor();

                analyticsPlugin.adaptiveMonitor = adaptiveMonitorFactory.adaptiveMonitor;
                analyticsPlugin.edgeServerMonitor = edgeServerMonitor;

                // Audience Insight ad tracking config

                analyticsPlugin.analyticsCollector.loggingSources.push(new Microsoft.VideoAnalytics.VideoAdvertising.AdvertisingLoggingSource(mediaPlayer.adHandlerPlugin.adHandlerController));

                // Set up ad

                var midrollAd = new PlayerFramework.Advertising.MidrollAdvertisement();
                midrollAd.source = new Microsoft.PlayerFramework.Js.Advertising.RemoteAdSource();
                midrollAd.source.type = Microsoft.VideoAdvertising.VastAdPayloadHandler.adType;
                midrollAd.source.uri = new Windows.Foundation.Uri("http://smf.blob.core.windows.net/samples/win8/ads/vast_adpod.xml");
                midrollAd.time = 5;

                mediaPlayer.adSchedulerPlugin.advertisements.push(midrollAd);
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
