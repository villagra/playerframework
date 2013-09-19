(function () {
    "use strict";

    // Items and groups should be sorted by descending date
    var groupedItems = new WinJS.Binding.List().createSorted(
        function (item1, item2) { return item2.date - item1.date; }
    ).createGrouped(
        function (item) { return item.group.key; },
        function (item) { return item.group; },
        function (groupKey1, groupKey2) { return groupKey2 - groupKey1; }
    );

    WinJS.Namespace.define("Data", {
        items: groupedItems,
        groups: groupedItems.groups,
        getItemReference: getItemReference,
        getItemsFromGroup: getItemsFromGroup,
        resolveGroupReference: resolveGroupReference,
        resolveItemReference: resolveItemReference,
        downloadFeed: downloadFeed
    });

    // Get a reference for an item, using the group key and item title as a
    // unique reference to the item that can be easily serialized.
    function getItemReference(item) {
        return [item.group.key, item.title];
    }

    // This function returns a WinJS.Binding.List containing only the items
    // that belong to the provided group.
    function getItemsFromGroup(group) {
        return groupedItems.createFiltered(function (item) { return item.group.key === group.key; });
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

    // Downloads the Channel 9 RSS feed
    function downloadFeed() {
        return WinJS.xhr({ url: "http://channel9.msdn.com/Shows/This+Week+On+Channel+9/RSS" }).then(
            // if success...
            function (result) {
                generateData(result.responseXML);
            },
            // if error...
            function (result) {
                // TODO: handle errors
            }
        );
    }

    // Generates data for the application from the Channel 9 RSS feed 
    function generateData(xml) {
        var nodes = xml.querySelectorAll("rss > channel > item");

        for (var i = 0; i < nodes.length; i++) {
            var node = nodes[i];
            var date = new Date(node.querySelector("pubDate").textContent);
            var link = node.querySelector("link").textContent;
            var title = node.querySelector("title").textContent;
            var author = node.querySelector("author").textContent;
            var description = node.querySelector("description").textContent;
            var imageSource = node.querySelector("thumbnail[width='512']").getAttribute("url");
            var videoSource = (node.querySelector("content[medium='video'][url$='manifest']") || node.querySelector("enclosure")).getAttribute("url");
            var tags = Array.prototype.slice.call(node.querySelectorAll("category")).map(function (node) { return node.textContent });

            var groupDate = new Date(date.getFullYear(), date.getMonth());
            var groupKey = groupDate.getTime();
            var group = resolveGroupReference(groupKey);

            if (!group) {
                group = {
                    key: groupKey,
                    date: groupDate
                };
            }

            groupedItems.push({
                group: group,
                date: date,
                link: link,
                title: title,
                author: author,
                description: description,
                imageSource: imageSource,
                videoSource: videoSource,
                tags: tags
            });
        }
    }
})();