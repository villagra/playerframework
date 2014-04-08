(function () {
    "use strict";

    var nav = WinJS.Navigation;
    var session = WinJS.Application.sessionState;
    var util = WinJS.Utilities;


    var commonData = Data.getItemsFromGroup(Data.resolveGroupReference("common"));
    var adaptiveData = Data.getItemsFromGroup(Data.resolveGroupReference("adaptive"));
    var advertisingData = Data.getItemsFromGroup(Data.resolveGroupReference("advertising"));
    var advancedData = Data.getItemsFromGroup(Data.resolveGroupReference("advanced"));
    var captionsData = Data.getItemsFromGroup(Data.resolveGroupReference("captions"));

    WinJS.UI.Pages.define("/pages/hub/hub.html", {
        processed: function (element) {
            return WinJS.Resources.processAll(element);
        },

        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            var hub = element.querySelector(".hub").winControl;
            //hub.onheaderinvoked = function (args) {
            //    args.detail.section.onheaderinvoked(args);
            //};
            hub.onloadingstatechanged = function (args) {
                if (args.srcElement === hub.element && args.detail.loadingState === "complete") {
                    hub.onloadingstatechanged = null;
                }
            }
        },

        commonDataSource: commonData.dataSource,
        adaptiveDataSource: adaptiveData.dataSource,
        advertisingDataSource: advertisingData.dataSource,
        advancedDataSource: advancedData.dataSource,
        captionsDataSource: captionsData.dataSource,
                
        commonItemNavigate: util.markSupportedForProcessing(function (args) {
            var item = commonData.getAt(args.detail.itemIndex);

            var name = item.subtitle.substring(item.subtitle.lastIndexOf("/") + 1);
            var page = "/pages/" + item.subtitle + "/" + name + ".html";
            nav.navigate(page, { item: Data.getItemReference(item) });
        }),
          
        adaptiveItemNavigate: util.markSupportedForProcessing(function (args) {
            var item = adaptiveData.getAt(args.detail.itemIndex);

            var name = item.subtitle.substring(item.subtitle.lastIndexOf("/") + 1);
            var page = "/pages/" + item.subtitle + "/" + name + ".html";
            nav.navigate(page, { item: Data.getItemReference(item) });
        }),
          
        advertisingItemNavigate: util.markSupportedForProcessing(function (args) {
            var item = advertisingData.getAt(args.detail.itemIndex);

            var name = item.subtitle.substring(item.subtitle.lastIndexOf("/") + 1);
            var page = "/pages/" + item.subtitle + "/" + name + ".html";
            nav.navigate(page, { item: Data.getItemReference(item) });
        }),
          
        advancedItemNavigate: util.markSupportedForProcessing(function (args) {
            var item = advancedData.getAt(args.detail.itemIndex);

            var name = item.subtitle.substring(item.subtitle.lastIndexOf("/") + 1);
            var page = "/pages/" + item.subtitle + "/" + name + ".html";
            nav.navigate(page, { item: Data.getItemReference(item) });
        }),

        captionsItemNavigate: util.markSupportedForProcessing(function (args) {
            var item = captionsData.getAt(args.detail.itemIndex);

            var name = item.subtitle.substring(item.subtitle.lastIndexOf("/") + 1);
            var page = "/pages/" + item.subtitle + "/" + name + ".html";
            nav.navigate(page, { item: Data.getItemReference(item) });
        }),

        unload: function () {
            // TODO: Respond to navigations away from this page.
        },

        updateLayout: function (element) {
            /// <param name="element" domElement="true" />

            // TODO: Respond to changes in layout.
        },
    });
})();