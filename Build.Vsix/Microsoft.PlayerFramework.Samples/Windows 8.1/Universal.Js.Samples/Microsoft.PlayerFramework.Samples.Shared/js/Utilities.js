(function () {
    "use strict";

    function log(message) {
        var element = document.querySelector(".log > div");
        if (element) {
            var text = element.textContent + "\n" + message;
            element.textContent = text.trim();
            element.scrollTop = element.scrollHeight;
        }
    }

    WinJS.Namespace.define("Utilities", {
        log: log
    });
})();