// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509

// TODO: secure these credentials and put them in one place
var credentials = "ArPzSbmnfYJucWzarJwB7Gk29sfWPy97C6rpz-Rm1xTO_AWa-_hCg-5Oo-mAHkcR";

// TODO: figure out a good place to serialize all of this information - it should
// probably be some kind of cloud service that I run with a document database.
// The reasoning behind this is to allow people to easily share their trips and
// to find trips that intersect?
var locations = new WinJS.Binding.List([
    { location: "Seattle, WA" },
    { location: "Portland, OR" }
]);

(function () {
    "use strict";

    var app = WinJS.Application;

    // Do an XHR request
    app.search = function (location) {
        $.ajax({
            url: 'http://dev.virtualearth.net/REST/v1/Locations/' + location + '?output=json&key=' + credentials,
            dataType: 'json',
            success: function (data, text_status) {
                // Emit the search results as a bindable list of locations that the user can choose from in the flyout
                var results = new WinJS.Binding.List();
                for (var i = 0; i < data.resourceSets[0].resources.length; i++) {
                    results.push({ location: data.resourceSets[0].resources[i].name });
                    console.log(data.resourceSets[0].resources[i].bbox);
                    console.log(data.resourceSets[0].resources[i].name);
                }
                var resultsList = $("#searchResultsList")[0].winControl;
                resultsList.itemDataSource = results.dataSource;
                resultsList.itemTemplate = $("#searchResultsTemplate")[0];
            }
        });
    };

    app.onactivated = function (eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            if (eventObject.detail.previousExecutionState !== Windows.ApplicationModel.Activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize 
                // your application here.

                // Bind event handlers
                $("#list")[0].winControl.addEventListener("iteminvoked", function (eventObject) {
                    eventObject.detail.itemPromise.then(function (invokedItem) {
                        document.frames["map"].postMessage(invokedItem.data.location, "ms-appx-web://" + document.location.host);
                        console.log("clicked on: " + invokedItem.data.location);
                    });
                }, false);

                $("#buttonSearch").click(function () {
                    app.search($("#searchInput")[0].value);
                });

                // TODO: note that this is probably not the right heuristic to use here - when the user searches for
                // something they might want to see where it is beyond what the location text is ... need to test this further
                $("#searchResultsList")[0].winControl.addEventListener("iteminvoked", function (eventObject) {
                    eventObject.detail.itemPromise.then(function (invokedItem) {
                        // Add to the locations list the new item and jump there
                        document.frames["map"].postMessage(invokedItem.data.location, "ms-appx-web://" + document.location.host);
                        locations.push({ location: invokedItem.data.location });

                        // Dismiss the flyout control
                        $("#searchFlyout")[0].winControl.hide();
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

    app.start();
})();
