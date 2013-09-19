(function () {
    "use strict";

    var dayNames = [
        "Sunday",
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday"
    ];

    var monthNames = [
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December"
    ];

    WinJS.Namespace.define("Converters", {
        longDateConverter: WinJS.Binding.converter(longDateConverter),
        shortDateConverter: WinJS.Binding.converter(shortDateConverter),
        backgroundImageConverter: WinJS.Binding.converter(backgroundImageConverter),
        itemTitleConverter: WinJS.Binding.converter(itemTitleConverter),
        itemDateConverter: WinJS.Binding.converter(itemDateConverter),
        itemAuthorConverter: WinJS.Binding.converter(itemAuthorConverter),
        itemTagsConverter: WinJS.Binding.converter(itemTagsConverter)
    });

    function longDateConverter(value) {
        var day = dayNames[value.getDay()];
        var month = monthNames[value.getMonth()];
        var date = value.getDate();
        var year = value.getFullYear();
        var hours = value.getHours() === 0 ? 12 : value.getHours() > 12 ? value.getHours() - 12 : value.getHours();
        var minutes = value.getMinutes();
        var tt = value.getHours() < 12 ? "AM" : "PM";

        return day + ", " + month + " " + date + ", " + year + ", " + hours + ":" + ("0" + minutes).slice(-2) + " " + tt;
    }

    function shortDateConverter(value) {
        var month = monthNames[value.getMonth()];
        var year = value.getFullYear();

        return month + " " + year;
    }

    function backgroundImageConverter(value) {
        return "url(" + value + ")";
    }

    function itemTitleConverter(value) {
        return value ? value.replace("TWC9: ", "") : "";
    }

    function itemDateConverter(value) {
        return value ? "Posted: " + longDateConverter(value) : "";
    }

    function itemAuthorConverter(value) {
        return value ? "By: " + value : "";
    }

    function itemTagsConverter(value) {
        return value && value.length ? "Tags: " + value.join(", ") : "";
    }
})();