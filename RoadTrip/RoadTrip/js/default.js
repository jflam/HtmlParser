// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509

// TODO: secure these credentials and put them in one place
var bing_maps_key = "ArPzSbmnfYJucWzarJwB7Gk29sfWPy97C6rpz-Rm1xTO_AWa-_hCg-5Oo-mAHkcR";
var bing_search_key = "2402CFD2EA26B9328072CCBF88051BE17B5547C8";

// TODO: figure out a good place to serialize all of this information - it should
// probably be some kind of cloud service that I run with a document database.
// The reasoning behind this is to allow people to easily share their trips and
// to find trips that intersect?
var locations = new WinJS.Binding.List([
    { location: "Seattle, WA", bbox: [47.25339889526367, -123.16571807861328, 47.946136474609375, -121.5034408569336] },
    { location: "Portland, OR", bbox: [45.1732063293457, -123.4850082397461, 45.865943908691406, -121.88526153564453] }
]);

(function () {
    "use strict";

    var app = WinJS.Application;
    var current_bbox = null;

    // Search Bing Maps for the location
    app.search_map = function (location) {
        $.ajax({
            url: 'http://dev.virtualearth.net/REST/v1/Locations/' + location + '?output=json&key=' + bing_maps_key,
            dataType: 'json',
            success: function (data, text_status) {
                // Emit the search results as a bindable list of locations that the user can choose from in the flyout
                var results = new WinJS.Binding.List();
                for (var i = 0; i < data.resourceSets[0].resources.length; i++) {
                    results.push({
                        location: data.resourceSets[0].resources[i].name,
                        bbox: data.resourceSets[0].resources[i].bbox
                    });
                }
                var resultsList = $("#searchStopResultsList")[0].winControl;
                resultsList.itemDataSource = results.dataSource;
                resultsList.itemTemplate = $("#searchStopResultsTemplate")[0];
            }
        });
    };

    app.make_pushpins = function (locations) {
        var message = {
            action: 'drop_pins',
            locations: locations,
            credentials: bing_maps_key
        };
        document.frames['map'].postMessage(message, 'ms-appx-web://' + document.location.host);
    };

    // Search Bing for the term given the bounding box of the map
    app.search_bing = function (term, bbox) {
        // Compute center of bbox
        var latitude = (bbox[0] + bbox[2]) / 2;
        var longitude = (bbox[1] + bbox[3]) / 2;
        // Roughly 69 miles per degree of latitude. Use lattitude difference in bbox to compute radius
        var radius = (bbox[2] - bbox[0]) * 69 / 2;
        $.ajax({
            url: 'http://api.bing.net/json.aspx?AppId=' + bing_search_key + '&Sources=Phonebook&Phonebook.Count=25&Query=' + term + '&Latitude=' + latitude + '&Longitude=' + longitude + '&Radius=' + radius,
            dataType: 'json',
            success: function (data, text_status) {
                app.make_pushpins(data.SearchResponse.Phonebook.Results);
            }
        });
    }

    // Zoom the map to the bbox
    app.zoom_to = function (bbox) {
        current_bbox = bbox;
        var message = {
            action: 'zoom_to',
            bbox: bbox,
            credentials: bing_maps_key
        };
        document.frames["map"].postMessage(message, "ms-appx-web://" + document.location.host);
    };

    app.onactivated = function (eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            if (eventObject.detail.previousExecutionState !== Windows.ApplicationModel.Activation.ApplicationExecutionState.terminated) {
                // Bind event handlers
                $("#list")[0].winControl.addEventListener("iteminvoked", function (eventObject) {
                    eventObject.detail.itemPromise.then(function (invokedItem) {
                        app.zoom_to(invokedItem.data.bbox);
                    });
                }, false);

                $("#buttonSearchStop").click(function () {
                    app.search_map($("#searchStopInput").val());
                    $("#addStopFlyout")[0].winControl.hide();
                });

                $("#buttonSearch").click(function () {
                    app.search_bing($("#searchInput").val(), current_bbox);
                    $("#searchFlyout")[0].winControl.hide();
                });

                // TODO: note that this is probably not the right heuristic to use here - when the user searches for
                // something they might want to see where it is beyond what the location text is ... need to test this further
                $("#searchStopResultsList")[0].winControl.addEventListener("iteminvoked", function (eventObject) {
                    eventObject.detail.itemPromise.then(function (invokedItem) {
                        app.zoom_to(invokedItem.data.bbox);
                        locations.push({
                            location: invokedItem.data.location,
                            bbox: invokedItem.data.bbox
                        });

                        // Dismiss the flyout control
                        $("#addStopFlyout")[0].winControl.hide();
                    });
                }, false);
                } else {
                // TODO: This application has been reactivated from suspension. 
                // Restore application state here.
            }
            WinJS.UI.processAll();
        }
    };

    app.oncheckpoint = function (eventObject) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the 
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // eventObject.setPromise(). 
    };

    app.receive_message = function (e) {
        var message = e.data;
        switch (message.action) {
            case 'map_resized':
                current_bbox = message.bbox;
                break;
            default:
                console.log('invalid action: ' + message.action);
                break;
        }
    };

    window.addEventListener("message", app.receive_message, false);

    app.start();
})();